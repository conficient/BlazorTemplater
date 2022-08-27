#nullable enable

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;

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
            var componentConstructor = GetComponentConstructor(componentType);

            _factoryMethod = () => new AsyncComponentRenderer(
                (IComponent)componentConstructor.Invoke(Array.Empty<object>()),
                serviceProvider,
                loggerFactory);
        }

#if NET5_0_OR_GREATER
        public AsyncComponentRendererFactory(Type componentType, IServiceProvider serviceProvider, ILoggerFactory loggerFactory, IComponentActivator componentActivator)
        {
            var componentConstructor = GetComponentConstructor(componentType);

            _factoryMethod = () => new AsyncComponentRenderer(
                (IComponent)componentConstructor.Invoke(Array.Empty<object>()),
                serviceProvider,
                loggerFactory,
                componentActivator);
        }
#endif

        public AsyncComponentRenderer CreateRenderer() => _factoryMethod.Invoke();

        private static ConstructorInfo GetComponentConstructor(Type componentType)
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

            return componentConstructor;
        }
    }

    public class AsyncComponentRendererFactory<TComponent> : IAsyncComponentRendererFactory, IAsyncComponentRendererFactory<TComponent>
        where TComponent : IComponent, new()
    {
        private readonly Func<AsyncComponentRenderer> _factoryMethod;

        public AsyncComponentRendererFactory(IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
        {
            _factoryMethod = () => new AsyncComponentRenderer(
                new TComponent(),
                serviceProvider,
                loggerFactory);
        }

#if NET5_0_OR_GREATER
        public AsyncComponentRendererFactory(IServiceProvider serviceProvider, ILoggerFactory loggerFactory, IComponentActivator componentActivator)
        {
            _factoryMethod = () => new AsyncComponentRenderer(
                new TComponent(),
                serviceProvider,
                loggerFactory,
                componentActivator);
        }
#endif

        public AsyncComponentRenderer CreateRenderer() => _factoryMethod.Invoke();
    }
}