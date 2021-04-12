using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;

namespace BlazorTemplater
{
    /// <summary>
    /// Rendering 'host' that supports service injection
    /// </summary>
    public class BlazorTemplater
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public BlazorTemplater()
        {
            // define a lazy service provider
            _serviceProvider = new Lazy<IServiceProvider>(() =>
            {
                return _serviceCollection.BuildServiceProvider();
            });

            // define lazy renderer
            _renderer = new Lazy<HtmlRenderer>(() =>
            {
                var loggerFactory = Services.GetService<ILoggerFactory>() ?? new NullLoggerFactory();
                return new HtmlRenderer(Services, loggerFactory);
            });
        }

        private readonly ServiceCollection _serviceCollection = new ServiceCollection();
        private readonly Lazy<HtmlRenderer> _renderer;
        private readonly Lazy<IServiceProvider> _serviceProvider;

        /// <summary>
        /// Services provided by DI
        /// </summary>
        public IServiceProvider Services => _serviceProvider.Value;

        /// <summary>
        /// Gets lazy renderer
        /// </summary>
        private HtmlRenderer Renderer => _renderer.Value;

        /// <summary>
        /// Add a service for injection - do this before rendering
        /// </summary>
        /// <typeparam name="TContract"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <param name="implementation"></param>
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
        /// <typeparam name="T"></typeparam>
        /// <param name="implementation"></param>
        public void AddService<T>(T implementation)
            => AddService<T, T>(implementation);

        /// <summary>
        /// Render a component to HTML
        /// </summary>
        /// <typeparam name="TComponent">The component type to render</typeparam>
        /// <param name="parameters">Optional dictionary of parameters</param>
        /// <returns></returns>

        public string RenderComponent<TComponent>(IDictionary<string, object> parameters = null) where TComponent : IComponent
        {
            // convert parameters to ParameterView
            var pv = GetParameterView(parameters);

            // generate a render model
            var component = new RenderedComponent<TComponent>(Renderer);

            // set the parameters
            component.SetParametersAndRender(pv);

            // get markup
            return component.GetMarkup();
        }

        /// <summary>
        /// Create ParameterView from dictionary
        /// </summary>
        /// <param name="parameters">optional dictionary</param>
        /// <returns></returns>
        private ParameterView GetParameterView(IDictionary<string, object> parameters)
        {
            if (parameters == null) return ParameterView.Empty;
            return ParameterView.FromDictionary(parameters);
        }
    }
}