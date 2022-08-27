#nullable enable

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System;

namespace BlazorTemplater
{
    public interface IAsyncComponentRendererFactory
    {
        AsyncComponentRenderer CreateRenderer();
    }

    public interface IAsyncComponentRendererFactory<TComponent>
        where TComponent : IComponent
    {
        AsyncComponentRenderer CreateRenderer();
    }

    public class AsyncComponentRendererFactory : IAsyncComponentRendererFactory
    {
        private readonly Func<AsyncComponentRenderer> _factoryMethod;

        public AsyncComponentRendererFactory(Type componentType, IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
        {
            if (!typeof(IComponent).IsAssignableFrom(componentType))
            {
                throw new ArgumentException("Component type must implement IComponent interface.", nameof(componentType));
            }

            var componentConstructor = componentType.GetConstructor(Array.Empty<Type>());
            if (componentConstructor is null)
            {
                throw new ArgumentException("Component type must have a parameterless constructor.", nameof(componentType));
            }

            _factoryMethod = () => new AsyncComponentRenderer(
                componentType,
                serviceProvider,
                loggerFactory);
        }

        public AsyncComponentRendererFactory(Type componentType, Type? layoutType, IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
        {
            if (!typeof(IComponent).IsAssignableFrom(componentType))
            {
                throw new ArgumentException("Component type must implement IComponent interface.", nameof(componentType));
            }

            var componentConstructor = componentType.GetConstructor(Array.Empty<Type>());
            if (componentConstructor is null)
            {
                throw new ArgumentException("Component type must have a parameterless constructor.", nameof(componentType));
            }
            
            if (layoutType is not null && !typeof(LayoutComponentBase).IsAssignableFrom(layoutType))
            { 
                throw new ArgumentException("Layout type must inherit from LayoutComponentBase.", nameof(layoutType));
            }

            _factoryMethod = () => new AsyncComponentRenderer(
                componentType,
                layoutType,
                serviceProvider,
                loggerFactory);
        }

#if NET5_0_OR_GREATER
        public AsyncComponentRendererFactory(Type componentType, IServiceProvider serviceProvider, ILoggerFactory loggerFactory, IComponentActivator componentActivator)
        {
            if (!typeof(IComponent).IsAssignableFrom(componentType))
            {
                throw new ArgumentException("Component type must implement IComponent interface.", nameof(componentType));
            }

            var componentConstructor = componentType.GetConstructor(Array.Empty<Type>());
            if (componentConstructor is null)
            {
                throw new ArgumentException("Component type must have a parameterless constructor.", nameof(componentType));
            }

            _factoryMethod = () => new AsyncComponentRenderer(
                componentType,
                serviceProvider,
                loggerFactory,
                componentActivator);
        }

        public AsyncComponentRendererFactory(Type componentType, Type? layoutType, IServiceProvider serviceProvider, ILoggerFactory loggerFactory, IComponentActivator componentActivator)
        {
            if (!typeof(IComponent).IsAssignableFrom(componentType))
            {
                throw new ArgumentException("Component type must implement IComponent interface.", nameof(componentType));
            }

            var componentConstructor = componentType.GetConstructor(Array.Empty<Type>());
            if (componentConstructor is null)
            {
                throw new ArgumentException("Component type must have a parameterless constructor.", nameof(componentType));
            }
            
            if (layoutType is not null && !typeof(LayoutComponentBase).IsAssignableFrom(layoutType))
            { 
                throw new ArgumentException("Layout type must inherit from LayoutComponentBase.", nameof(layoutType));
            }

            _factoryMethod = () => new AsyncComponentRenderer(
                componentType,
                layoutType,
                serviceProvider,
                loggerFactory,
                componentActivator);
        }
#endif

        public AsyncComponentRenderer CreateRenderer() => _factoryMethod.Invoke();
    }

    public class AsyncComponentRendererFactory<TComponent> : IAsyncComponentRendererFactory, IAsyncComponentRendererFactory<TComponent>
        where TComponent : IComponent, new()
    {
        private readonly Func<AsyncComponentRenderer> _factoryMethod;

        public AsyncComponentRendererFactory(IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
        {
            _factoryMethod = () => new AsyncComponentRenderer(
                typeof(TComponent),
                serviceProvider,
                loggerFactory);
        }

#if NET5_0_OR_GREATER
        public AsyncComponentRendererFactory(IServiceProvider serviceProvider, ILoggerFactory loggerFactory, IComponentActivator componentActivator)
        {
            _factoryMethod = () => new AsyncComponentRenderer(
                typeof(TComponent),
                serviceProvider,
                loggerFactory,
                componentActivator);
        }
#endif

        public AsyncComponentRenderer CreateRenderer() => _factoryMethod.Invoke();
    }

    public class AsyncComponentRendererFactory<TComponent, TLayout> : IAsyncComponentRendererFactory, IAsyncComponentRendererFactory<TComponent>
        where TComponent : IComponent, new()
        where TLayout : LayoutComponentBase
    {
        private readonly Func<AsyncComponentRenderer> _factoryMethod;

        public AsyncComponentRendererFactory(IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
        {
            _factoryMethod = () => new AsyncComponentRenderer(
                typeof(TComponent),
                typeof(TLayout),
                serviceProvider,
                loggerFactory);
        }

#if NET5_0_OR_GREATER
        public AsyncComponentRendererFactory(IServiceProvider serviceProvider, ILoggerFactory loggerFactory, IComponentActivator componentActivator)
        {
            _factoryMethod = () => new AsyncComponentRenderer(
                typeof(TComponent),
                typeof(TLayout),
                serviceProvider,
                loggerFactory,
                componentActivator);
        }
#endif

        public AsyncComponentRenderer CreateRenderer() => _factoryMethod.Invoke();
    }
}