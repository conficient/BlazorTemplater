using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;

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
                return _serviceCollection.BuildServiceProvider();
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
        private readonly ServiceCollection _serviceCollection = new();

        /// <summary>
        /// Lazy HtmlRenderer instance
        /// </summary>
        private readonly Lazy<HtmlRenderer> _renderer;

        /// <summary>
        /// Lazy ServiceProvider instance
        /// </summary>
        private readonly Lazy<IServiceProvider> _serviceProvider;

        /// <summary>
        /// Services provided by Dependency Injection
        /// </summary>
        public IServiceProvider Services => _serviceProvider.Value;

        /// <summary>
        /// Gets Renderer
        /// </summary>
        private HtmlRenderer Renderer => _renderer.Value;

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
        private static ParameterView GetParameterView(IDictionary<string, object> parameters)
        {
            if (parameters == null) return ParameterView.Empty;
            return ParameterView.FromDictionary(parameters);
        }

        /// <summary>
        /// Method for ComponentBuilder to use
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <param name="parameters"></param>
        /// <returns></returns>
        internal string RenderComponent<TComponent>(ParameterView parameters) where TComponent : IComponent
        {
            // generate a render model
            var component = new RenderedComponent<TComponent>(Renderer);

            // set the parameters
            component.SetParametersAndRender(parameters);

            // get markup
            return component.GetMarkup();
        }

    }
}