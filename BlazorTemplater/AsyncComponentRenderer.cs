#pragma warning disable BL0006

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace BlazorTemplater
{
    public class AsyncComponentRenderer<TComponent> : Renderer
        where TComponent : IComponent, new()
    {
        private static readonly HtmlEncoder _htmlEncoder = HtmlEncoder.Default;

        private static readonly HashSet<string> _selfClosingElements = new(StringComparer.OrdinalIgnoreCase)
        {
            "area", "base", "br", "col", "embed", "hr", "img", "input", "link", "meta", "param", "source", "track", "wbr",
        };

        private readonly Dispatcher _dispatcher;
        private readonly StringBuilder _sb;

        private Exception? _exception;
        private string? _closestSelectValue;

        public AsyncComponentRenderer(IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
            : base(serviceProvider, loggerFactory)
        {
            _dispatcher = Dispatcher.CreateDefault();
            _sb = new StringBuilder();
        }

        public override Dispatcher Dispatcher => _dispatcher;

        public async Task<string> RenderAsync(IDictionary<string, object> parameters)
        {
            var component = new TComponent();
            var componentId = AssignRootComponentId(component);

            await Dispatcher.InvokeAsync(() => RenderRootComponentAsync(componentId, ParameterView.FromDictionary(parameters)));

            if (_exception is not null)
            {
                throw _exception;
            }

            return GetHtml(componentId);
        }

        protected override void HandleException(Exception exception)
        {
            _exception = exception;
        }

        protected override Task UpdateDisplayAsync(in RenderBatch renderBatch)
        {
            return Task.CompletedTask;
        }

        private string GetHtml(int componentId)
        {
            var frames = GetCurrentRenderTreeFrames(componentId);
            RenderFrames(frames);

            return _sb.ToString();
        }

        private int RenderFrames(ArrayRange<RenderTreeFrame> frames)
        {
            return RenderFrames(frames, 0, frames.Count);
        }

        private int RenderFrames(ArrayRange<RenderTreeFrame> frames, int position, int maxElements)
        {
            var nextPosition = position;
            var endPosition = position + maxElements;
            while (position < endPosition)
            {
                nextPosition = RenderCore(frames, position);
                if (position == nextPosition)
                {
                    throw new InvalidOperationException("We didn't consume any input.");
                }

                position = nextPosition;
            }

            return nextPosition;
        }

        private int RenderCore(ArrayRange<RenderTreeFrame> frames, int position)
        {
            ref var frame = ref frames.Array[position];
            switch (frame.FrameType)
            {
                case RenderTreeFrameType.Element:
                    return RenderElement(frames, position);

                case RenderTreeFrameType.Attribute:
                    throw new InvalidOperationException($"Attributes should only be encountered within {nameof(RenderElement)}");

                case RenderTreeFrameType.Text:
                    _sb.Append(_htmlEncoder.Encode(frame.TextContent));
                    return ++position;

                case RenderTreeFrameType.Markup:
                    _sb.Append(frame.MarkupContent);
                    return ++position;

                case RenderTreeFrameType.Component:
                    return RenderChildComponent(frames, position);

                case RenderTreeFrameType.Region:
                    return RenderFrames(frames, position + 1, frame.RegionSubtreeLength - 1);

                case RenderTreeFrameType.ElementReferenceCapture:
                case RenderTreeFrameType.ComponentReferenceCapture:
                    return ++position;

                default:
                    throw new InvalidOperationException($"Invalid element frame type '{frame.FrameType}'.");
            }
        }

        private int RenderChildComponent(ArrayRange<RenderTreeFrame> frames, int position)
        {
            ref var frame = ref frames.Array[position];
            var childFrames = GetCurrentRenderTreeFrames(frame.ComponentId);
            RenderFrames(childFrames);
            return position + frame.ComponentSubtreeLength;
        }

        private int RenderElement(ArrayRange<RenderTreeFrame> frames, int position)
        {
            ref var frame = ref frames.Array[position];
            _sb.Append('<');
            _sb.Append(frame.ElementName);
            var afterAttributes = RenderAttributes(frames, position + 1, frame.ElementSubtreeLength - 1, out var capturedValueAttribute);

            // When we see an <option> as a descendant of a <select>, and the option's "value" attribute matches the
            // "value" attribute on the <select>, then we auto-add the "selected" attribute to that option. This is
            // a way of converting Blazor's select binding feature to regular static HTML.
            if (_closestSelectValue != null
                && string.Equals(frame.ElementName, "option", StringComparison.OrdinalIgnoreCase)
                && string.Equals(capturedValueAttribute, _closestSelectValue, StringComparison.Ordinal))
            {
                _sb.Append(" selected");
            }

            var remainingElements = frame.ElementSubtreeLength + position - afterAttributes;
            if (remainingElements > 0)
            {
                _sb.Append('>');

                var isSelect = string.Equals(frame.ElementName, "select", StringComparison.OrdinalIgnoreCase);
                if (isSelect)
                {
                    _closestSelectValue = capturedValueAttribute;
                }

                var afterElement = RenderChildren(frames, afterAttributes, remainingElements);

                if (isSelect)
                {
                    // There's no concept of nested <select> elements, so as soon as we're exiting one of them,
                    // we can safely say there is no longer any value for this
                    _closestSelectValue = null;
                }

                _sb.Append("</");
                _sb.Append(frame.ElementName);
                _sb.Append('>');

                return afterElement;
            }
            else
            {
                if (_selfClosingElements.Contains(frame.ElementName))
                {
                    _sb.Append(" />");
                }
                else
                {
                    _sb.Append('>');
                    _sb.Append("</");
                    _sb.Append(frame.ElementName);
                    _sb.Append('>');
                }

                return afterAttributes;
            }
        }

        private int RenderChildren(ArrayRange<RenderTreeFrame> frames, int position, int maxElements)
        {
            if (maxElements == 0)
            {
                return position;
            }

            return RenderFrames(frames, position, maxElements);
        }

        private int RenderAttributes(ArrayRange<RenderTreeFrame> frames, int position, int maxElements, out string? capturedValueAttribute)
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
                    _sb.Append(' ');
                    _sb.Append(frame.AttributeName);
                    _sb.Append("=\"");
                    _sb.Append(frame.AttributeEventHandlerId);
                    _sb.Append('"');
                    continue;
                }

                switch (frame.AttributeValue)
                {
                    case bool flag when flag:
                        _sb.Append(' ');
                        _sb.Append(frame.AttributeName);
                        break;

                    case string value:
                        _sb.Append(' ');
                        _sb.Append(frame.AttributeName);
                        _sb.Append('=');
                        _sb.Append('"');
                        _sb.Append(_htmlEncoder.Encode(value));
                        _sb.Append('"');
                        break;

                    default:
                        break;
                }
            }

            return position + maxElements;
        }
    }
}