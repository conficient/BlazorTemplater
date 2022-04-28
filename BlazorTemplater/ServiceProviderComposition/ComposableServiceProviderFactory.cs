using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazorTemplater.ServiceProviderComposition
{
    /// <summary>
    /// provides a way to create a service provider able to use other service provider, if the service collection is also an IEnumerable<IServiceProvider> 
    /// </summary>
    internal class ComposableServiceProviderFactory : IServiceProviderFactory<ComposableServiceBuilder>
    {

        /// <summary>
        /// create a builder for a service collection
        /// </summary>
        /// <param name="services">a collection of services, if it is also a collection of service providers then the generated builder will produce a ComposingServiceProvider</param>
        /// <returns>a builder for the service collection</returns>
        public ComposableServiceBuilder CreateBuilder(IServiceCollection services)
        {
            var coreServiceProvider = services.BuildServiceProvider();
            if (services is IEnumerable<IServiceProvider> composingServiceProviders)
            {
                return new(coreServiceProvider, composingServiceProviders);
            }
            return new(coreServiceProvider);
        }
        
        /// <summary>
        /// produces a service provider  for the composable service builder
        /// </summary>
        /// <param name="containerBuilder">the container of services and service provider to use as a source for the creation of the serviceprovider</param>
        /// <returns>a service provider that will be able to resolve services from the service collection or from one of the composed service provider</returns>
        public IServiceProvider CreateServiceProvider(ComposableServiceBuilder containerBuilder)
        {
            return containerBuilder.Build();
        }
    }
}
