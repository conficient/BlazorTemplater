using BlazorTemplater.ServiceProviderComposition;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BlazorTemplater
{
    /// <summary>
    /// Templating host that supports service injection and rendering
    /// </summary>
    public class Templater
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public Templater()
        {
            // define a lazy service provider
            _serviceProvider = new Lazy<IServiceProvider>(() =>
            {
                // creates a service provider from all providers
                var factory = new ComposableServiceProviderFactory();
                return factory.CreateServiceProvider(
                    factory.CreateBuilder(_serviceCollection));
            });

            // define lazy renderer
            _renderer = new Lazy<HtmlRenderer>(() =>
            {
                var loggerFactory = Services.GetService<ILoggerFactory>() ?? new NullLoggerFactory();
                return new HtmlRenderer(Services, loggerFactory);
            });
        }

        /// <summary>
        /// Service collection instance
        /// </summary>
        private readonly ComposableServiceCollection _serviceCollection = new();

        /// <summary>
        /// Lazy HtmlRenderer instance
        /// </summary>
        private readonly Lazy<HtmlRenderer> _renderer;

        /// <summary>
        /// Lazy ServiceProvider instance
        /// </summary>
        private readonly Lazy<IServiceProvider> _serviceProvider;

        /// <summary>
        /// Store the Scoped CSS. Css Isolated.
        /// </summary>
        private StringBuilder _style = null;

        /// <summary>
        /// Services provided by Dependency Injection
        /// </summary>
        public IServiceProvider Services => _serviceProvider.Value;

        /// <summary>
        /// Gets Renderer
        /// </summary>
        private HtmlRenderer Renderer => _renderer.Value;

        /// <summary>
        /// Override layout on top-level component
        /// </summary>
        private Type layout;

        /// <summary>
        /// Add a service for injection - do this before rendering
        /// </summary>
        /// <typeparam name="TContract">The interface/contract</typeparam>
        /// <typeparam name="TImplementation">The implementation type</typeparam>
        /// <param name="implementation">Instance to return</param>
        public void AddService<TContract, TImplementation>(TImplementation implementation) where TImplementation : TContract
        {
            if (_renderer.IsValueCreated)
            {
                throw new InvalidOperationException("Cannot configure services after the host has started operation");
            }
            _serviceCollection.AddSingleton(typeof(TContract), implementation);
        }

        /// <summary>
        /// Add a service with implementation
        /// </summary>
        /// <typeparam name="T">Type of service</typeparam>
        /// <param name="implementation">Instance to return</param>
        public void AddService<T>(T implementation) => AddService<T, T>(implementation);

        /// <summary>
        /// Add a new Service Provider to the collection
        /// </summary>
        /// <param name="serviceProvider"></param>
        public void AddServiceProvider(IServiceProvider serviceProvider)
        {
            // add service provider if not present
            if (!_serviceCollection.Contains(serviceProvider))
                _serviceCollection.Add(serviceProvider);
        }

        /// <summary>
        /// Add additional isolated CSS from a nested components
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        public Templater AddScoppedCss<TComponent>()
        {
            Type type = typeof(TComponent);
            AddScoppedCss(type);
            return this;
        }

        private void AddScoppedCss(Type componentType)
        {
            if (componentType != null)
            {
                string cssFile = $@"{componentType.FullName}.razor.css";
                StringBuilder css = new StringBuilder();
                try
                {
                    Assembly assembly = componentType.Assembly;
                    using Stream stream = assembly.GetManifestResourceStream(cssFile);
                    using StreamReader sr = new StreamReader(stream!);
                    string content = sr.ReadToEndAsync().GetAwaiter().GetResult();
                    if (!string.IsNullOrEmpty(content))
                    {
                        css.Append(content);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Component = {componentType.Name} => Exception: {ex.Message}");
                    css.Clear();
                }
                finally
                {
                    if (_style == null)
                        _style = new();
                    _style.Append(css);
                }
            }
        }

        /// <summary>
        /// Render a component to HTML
        /// </summary>
        /// <typeparam name="TComponent">The component type to render</typeparam>
        /// <param name="parameters">Optional dictionary of parameters</param>
        /// <returns></returns>
        /// <remarks>
        /// Updated version renders inside a LayoutView so that layouts are applied
        /// </remarks>
        public string RenderComponent<TComponent>(IDictionary<string, object> parameters = null) where TComponent : IComponent
        {
            var componentType = typeof(TComponent);
            return RenderComponent(componentType, parameters);
        }

        /// <summary>
        /// Render a component to HTML (non-generic version)
        /// </summary>
        /// <param name="componentType">the Type of the component</param>
        /// <param name="parameters">Optional dictionary of parameters</param>
        /// <returns></returns>
        /// <remarks>
        /// This non-generic version can accept a Type
        /// </remarks>
        public string RenderComponent(Type componentType, IDictionary<string, object> parameters = null)
        {
            ValidateComponentType(componentType);
            var layout = GetLayout(componentType);
            AddScoppedCss(layout);
            AddScoppedCss(componentType);

            // create a RenderFragment from the component
            RenderFragment childContent = builder =>
            {
                int componentId = 0;
                if (_style != null && _style.Length > 0)
                {
                    builder.AddMarkupContent(componentId, $@"<style type=""text/css"">{_style}</style>");
                    componentId++;
                }
                builder.OpenComponent(componentId, componentType);
                // add parameters if any
                if (parameters != null && parameters.Any())
                {
                    componentId++;
                    builder.AddMultipleAttributes(componentId, parameters);
                }
                // add scoped css if any
                builder.CloseComponent();
            };

            // render a LayoutView and use the TComponent as the child content
            var layoutView = new RenderedComponent<LayoutView>(Renderer);
            var layoutParams = new Dictionary<string, object>()
            {
                { nameof(LayoutView.Layout), layout },
                { nameof(LayoutView.ChildContent), childContent }
            };
            layoutView.SetParametersAndRender(GetParameterView(layoutParams));

            return layoutView.GetMarkup();
        }

        /// <summary>
        /// Check that a Type is an IComponent
        /// </summary>
        /// <param name="componentType"></param>
        private static void ValidateComponentType(Type componentType)
        {
            if (!_iComponentType.IsAssignableFrom(componentType))
                throw new ArgumentException("Type must implement IComponent", nameof(componentType));
        }

        private static readonly Type _iComponentType = typeof(IComponent);

        /// <summary>
        /// Create ParameterView from dictionary
        /// </summary>
        /// <param name="parameters">optional dictionary</param>
        /// <returns></returns>
        private static ParameterView GetParameterView(IDictionary<string, object> parameters)
        {
            if (parameters == null) return ParameterView.Empty;
            return ParameterView.FromDictionary(parameters);
        }

        #region Layouts

        /// <summary>
        /// Sets a Layoutusing a generic parameter
        /// </summary>
        /// <typeparam name="TLayout">Layout type to use. Must inherit from LayoutComponentBase</typeparam>
        public void UseLayout<TLayout>() where TLayout : LayoutComponentBase
        {
            layout = typeof(TLayout);
        }

        /// <summary>
        /// Sets a layout to use from a Type
        /// </summary>
        /// <param name="layout">The type to use. Must inherit from LayoutComponentBase</param>
        public void UseLayout(Type layoutType)
        {
            // allow null values so users can remove override
            if (layoutType is null)
            {
                layout = null;
                return;
            }

            // validate that layoutType inherits from LayoutComponentBase
            if (!layoutBaseType.IsAssignableFrom(layoutType))
                throw new ArgumentException("Layouts should inherit from LayoutComponentBase", nameof(layoutType));
            layout = layoutType;
        }

        private static readonly Type layoutBaseType = typeof(LayoutComponentBase);

        ///// <summary>
        ///// Render a component with a Layout
        ///// </summary>
        ///// <typeparam name="TComponent">Component type to render</typeparam>
        ///// <param name="layout"></param>
        ///// <param name="parameters"></param>
        ///// <returns>HTML string</returns>
        //private string RenderLayoutView<TComponent>(Type layout, IDictionary<string, object> parameters) where TComponent : IComponent
        //{
        //    // create a RenderFragment from the component
        //    var childContent = (RenderFragment)(builder =>
        //    {
        //        builder.OpenComponent(0, typeof(TComponent));
        //        // add parameters
        //        if (parameters != null && parameters.Any())
        //            builder.AddMultipleAttributes(1, parameters);
        //        builder.CloseComponent();
        //    });
        //    // render a LayoutView and use the TComponent as the child content
        //    var lv = new RenderedComponent<LayoutView>(Renderer);
        //    var lvp = new Dictionary<string, object>()
        //    {
        //        { nameof(LayoutView.Layout), layout },
        //        { nameof(LayoutView.ChildContent), childContent }
        //    };
        //    lv.SetParametersAndRender(GetParameterView(lvp));

        //    return lv.GetMarkup();
        //}

        /// <summary>
        /// Get the layout in a component
        /// </summary>
        /// <param name="componentType"></param>
        /// <returns></returns>
        private Type GetLayout(Type componentType)
        {
            // Use layout override if set
            if (layout != null)
                return layout;

            // check top-level component for a layout attribute
            return GetLayoutFromAttribute(componentType);
        }

        /// <summary>
        /// Find first LayoutAttribute in a component if present
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        public static Type GetLayoutFromAttribute<TComponent>() where TComponent : IComponent
        {
            return GetLayoutFromAttribute(typeof(TComponent));
        }

        /// <summary>
        /// Find first LayoutAttribute in a component if present
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        public static Type GetLayoutFromAttribute(Type componentType)
        {
            var layoutAttrs = (LayoutAttribute[])componentType.GetCustomAttributes(typeof(LayoutAttribute), true);
            if (layoutAttrs != null && layoutAttrs.Length > 0)
                return layoutAttrs[0].LayoutType;
            else
                return null;
        }

        #endregion Layouts
    }
}