#nullable enable

#pragma warning disable BL0006

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorTemplater
{
    public class AsyncComponentRenderer : Renderer
    {
        private static readonly HtmlEncoder _htmlEncoder = HtmlEncoder.Default;

        private static readonly HashSet<string> _selfClosingElements = new(StringComparer.OrdinalIgnoreCase)
        {
            "area", "base", "br", "col", "embed", "hr", "img", "input", "link", "meta", "param", "source", "track", "wbr",
        };

        private readonly Type _componentType;
        private readonly int _rootComponentId;
        private readonly Dispatcher _dispatcher;
        private readonly SemaphoreSlim _semaphoreSlim;
        private readonly IDictionary<string, object?> _layoutParameters;

        private Exception? _exception;
        private IEnumerable<KeyValuePair<string, object?>> _componentParameters;

        public AsyncComponentRenderer(Type componentType, IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
            : base(serviceProvider, loggerFactory)
        {
            ValidateTypes(componentType, null);

            _componentType = componentType;
            _rootComponentId = AssignRootComponentId(new LayoutView());
            _dispatcher = Dispatcher.CreateDefault();
            _semaphoreSlim = new SemaphoreSlim(1, 1);

            RenderFragment childContent = builder =>
            {
                builder.OpenComponent(0, _componentType);
                builder.AddMultipleAttributes(1, _componentParameters);
                builder.CloseComponent();
            };
            
            _layoutParameters = new Dictionary<string, object?>()
            {
                { nameof(LayoutView.ChildContent), childContent },
                { nameof(LayoutView.Layout), Templater.GetLayoutFromAttribute(_componentType) },
            };
        }

        public AsyncComponentRenderer(Type componentType, Type? layoutType, IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
            : base(serviceProvider, loggerFactory)
        {
            ValidateTypes(componentType, layoutType);

            _componentType = componentType;
            _rootComponentId = AssignRootComponentId(new LayoutView());
            _dispatcher = Dispatcher.CreateDefault();
            _semaphoreSlim = new SemaphoreSlim(1, 1);

            RenderFragment childContent = builder =>
            {
                builder.OpenComponent(0, _componentType);
                builder.AddMultipleAttributes(1, _componentParameters);
                builder.CloseComponent();
            };
            
            _layoutParameters = new Dictionary<string, object?>()
            {
                { nameof(LayoutView.ChildContent), childContent },
                { nameof(LayoutView.Layout), layoutType ?? Templater.GetLayoutFromAttribute(_componentType) },
            };
        }

#if NET5_0_OR_GREATER
        public AsyncComponentRenderer(Type componentType, IServiceProvider serviceProvider, ILoggerFactory loggerFactory, IComponentActivator componentActivator)
            : base(serviceProvider, loggerFactory, componentActivator)
        {
            ValidateTypes(componentType, null);

            _componentType = componentType;
            _rootComponentId = AssignRootComponentId(new LayoutView());
            _dispatcher = Dispatcher.CreateDefault();
            _semaphoreSlim = new SemaphoreSlim(1, 1);

            RenderFragment childContent = builder =>
            {
                builder.OpenComponent(0, _componentType);
                builder.AddMultipleAttributes(1, _componentParameters);
                builder.CloseComponent();
            };
            
            _layoutParameters = new Dictionary<string, object?>()
            {
                { nameof(LayoutView.ChildContent), childContent },
                { nameof(LayoutView.Layout), Templater.GetLayoutFromAttribute(_componentType) },
            };
        }

        public AsyncComponentRenderer(Type componentType, Type? layoutType, IServiceProvider serviceProvider, ILoggerFactory loggerFactory, IComponentActivator componentActivator)
            : base(serviceProvider, loggerFactory, componentActivator)
        {
            ValidateTypes(componentType, layoutType);

            _componentType = componentType;
            _rootComponentId = AssignRootComponentId(new LayoutView());
            _dispatcher = Dispatcher.CreateDefault();
            _semaphoreSlim = new SemaphoreSlim(1, 1);

            RenderFragment childContent = builder =>
            {
                builder.OpenComponent(0, _componentType);
                builder.AddMultipleAttributes(1, _componentParameters);
                builder.CloseComponent();
            };
            
            _layoutParameters = new Dictionary<string, object?>()
            {
                { nameof(LayoutView.ChildContent), childContent },
                { nameof(LayoutView.Layout), layoutType ?? Templater.GetLayoutFromAttribute(_componentType) },
            };
        }
#endif

        public override Dispatcher Dispatcher => _dispatcher;

        public async Task<string> RenderAsync(IEnumerable<KeyValuePair<string, object?>>? parameters)
        {
            var sb = new StringBuilder();
            var writer = new StringWriter(sb);

            await RenderAsync(parameters, writer);

            return sb.ToString();
        }

        public async Task RenderAsync(IEnumerable<KeyValuePair<string, object?>>? parameters, TextWriter textWriter)
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                _exception = null;
                _componentParameters = parameters ?? new Dictionary<string, object?>();

                await Dispatcher.InvokeAsync(() => RenderRootComponentAsync(_rootComponentId, ParameterView.FromDictionary(_layoutParameters)));

                if (_exception is not null)
                {
                    ExceptionDispatchInfo.Capture(_exception).Throw();
                }

                var context = new AsyncComponentRendererContext(textWriter);

                RenderHtml(context, _rootComponentId);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        protected override void HandleException(Exception exception)
        {
            _exception = exception;
        }

        protected override Task UpdateDisplayAsync(in RenderBatch renderBatch)
        {
            return Task.CompletedTask;
        }

        private static void ValidateTypes(Type componentType, Type? layoutType)
        {
            if (!typeof(IComponent).IsAssignableFrom(componentType))
            {
                throw new ArgumentException("Component type must implement IComponent.", nameof(componentType));
            }
            
            if (layoutType is not null && !typeof(LayoutComponentBase).IsAssignableFrom(layoutType))
            { 
                throw new ArgumentException("Layout type must inherit from LayoutComponentBase.", nameof(layoutType));
            }
        }

        private void RenderHtml(AsyncComponentRendererContext context, int componentId)
        {
            var frames = GetCurrentRenderTreeFrames(componentId);
            RenderFrames(context, frames);
        }

        private int RenderFrames(AsyncComponentRendererContext context, ArrayRange<RenderTreeFrame> frames)
        {
            return RenderFrames(context, frames, 0, frames.Count);
        }

        private int RenderFrames(AsyncComponentRendererContext context, ArrayRange<RenderTreeFrame> frames, int position, int maxElements)
        {
            var nextPosition = position;
            var endPosition = position + maxElements;
            while (position < endPosition)
            {
                nextPosition = RenderCore(context, frames, position);
                if (position == nextPosition)
                {
                    throw new InvalidOperationException("We didn't consume any input.");
                }

                position = nextPosition;
            }

            return nextPosition;
        }

        private int RenderCore(AsyncComponentRendererContext context, ArrayRange<RenderTreeFrame> frames, int position)
        {
            ref var frame = ref frames.Array[position];
            switch (frame.FrameType)
            {
                case RenderTreeFrameType.Element:
                    return RenderElement(context, frames, position);

                case RenderTreeFrameType.Attribute:
                    throw new InvalidOperationException($"Attributes should only be encountered within {nameof(RenderElement)}");

                case RenderTreeFrameType.Text:
                    context.TextWriter.Write(_htmlEncoder.Encode(frame.TextContent));
                    return ++position;

                case RenderTreeFrameType.Markup:
                    context.TextWriter.Write(frame.MarkupContent);
                    return ++position;

                case RenderTreeFrameType.Component:
                    return RenderChildComponent(context, frames, position);

                case RenderTreeFrameType.Region:
                    return RenderFrames(context, frames, position + 1, frame.RegionSubtreeLength - 1);

                case RenderTreeFrameType.ElementReferenceCapture:
                case RenderTreeFrameType.ComponentReferenceCapture:
                    return ++position;

                default:
                    throw new InvalidOperationException($"Invalid element frame type '{frame.FrameType}'.");
            }
        }

        private int RenderChildComponent(AsyncComponentRendererContext context, ArrayRange<RenderTreeFrame> frames, int position)
        {
            ref var frame = ref frames.Array[position];
            var childFrames = GetCurrentRenderTreeFrames(frame.ComponentId);
            RenderFrames(context, childFrames);
            return position + frame.ComponentSubtreeLength;
        }

        private int RenderElement(AsyncComponentRendererContext context, ArrayRange<RenderTreeFrame> frames, int position)
        {
            ref var frame = ref frames.Array[position];
            context.TextWriter.Write('<');
            context.TextWriter.Write(frame.ElementName);
            var afterAttributes = RenderAttributes(context, frames, position + 1, frame.ElementSubtreeLength - 1, out var capturedValueAttribute);

            // When we see an <option> as a descendant of a <select>, and the option's "value" attribute matches the
            // "value" attribute on the <select>, then we auto-add the "selected" attribute to that option. This is
            // a way of converting Blazor's select binding feature to regular static HTML.
            if (context.ClosestSelectValue != null
                && string.Equals(frame.ElementName, "option", StringComparison.OrdinalIgnoreCase)
                && string.Equals(capturedValueAttribute, context.ClosestSelectValue, StringComparison.Ordinal))
            {
                context.TextWriter.Write(" selected");
            }

            var remainingElements = frame.ElementSubtreeLength + position - afterAttributes;
            if (remainingElements > 0)
            {
                context.TextWriter.Write('>');

                var isSelect = string.Equals(frame.ElementName, "select", StringComparison.OrdinalIgnoreCase);
                if (isSelect)
                {
                    context.ClosestSelectValue = capturedValueAttribute;
                }

                var afterElement = RenderChildren(context, frames, afterAttributes, remainingElements);

                if (isSelect)
                {
                    // There's no concept of nested <select> elements, so as soon as we're exiting one of them,
                    // we can safely say there is no longer any value for this
                    context.ClosestSelectValue = null;
                }

                context.TextWriter.Write("</");
                context.TextWriter.Write(frame.ElementName);
                context.TextWriter.Write('>');

                return afterElement;
            }
            else
            {
                if (_selfClosingElements.Contains(frame.ElementName))
                {
                    context.TextWriter.Write(" />");
                }
                else
                {
                    context.TextWriter.Write('>');
                    context.TextWriter.Write("</");
                    context.TextWriter.Write(frame.ElementName);
                    context.TextWriter.Write('>');
                }

                return afterAttributes;
            }
        }

        private int RenderChildren(AsyncComponentRendererContext context, ArrayRange<RenderTreeFrame> frames, int position, int maxElements)
        {
            if (maxElements == 0)
            {
                return position;
            }

            return RenderFrames(context, frames, position, maxElements);
        }

        private int RenderAttributes(AsyncComponentRendererContext context, ArrayRange<RenderTreeFrame> frames, int position, int maxElements, out string? capturedValueAttribute)
        {
            capturedValueAttribute = null;

            if (maxElements == 0)
            {
                return position;
            }

            for (var i = 0; i < maxElements; i++)
            {
                var candidateIndex = position + i;
                ref var frame = ref frames.Array[candidateIndex];
                if (frame.FrameType != RenderTreeFrameType.Attribute)
                {
                    return candidateIndex;
                }

                if (frame.AttributeName.Equals("value", StringComparison.OrdinalIgnoreCase))
                {
                    capturedValueAttribute = frame.AttributeValue as string;
                }

                if (frame.AttributeEventHandlerId > 0)
                {
                    context.TextWriter.Write(' ');
                    context.TextWriter.Write(frame.AttributeName);
                    context.TextWriter.Write("=\"");
                    context.TextWriter.Write(frame.AttributeEventHandlerId);
                    context.TextWriter.Write('"');
                    continue;
                }

                switch (frame.AttributeValue)
                {
                    case bool flag when flag:
                        context.TextWriter.Write(' ');
                        context.TextWriter.Write(frame.AttributeName);
                        break;

                    case string value:
                        context.TextWriter.Write(' ');
                        context.TextWriter.Write(frame.AttributeName);
                        context.TextWriter.Write('=');
                        context.TextWriter.Write('"');
                        context.TextWriter.Write(_htmlEncoder.Encode(value));
                        context.TextWriter.Write('"');
                        break;

                    default:
                        break;
                }
            }

            return position + maxElements;
        }
    }
}