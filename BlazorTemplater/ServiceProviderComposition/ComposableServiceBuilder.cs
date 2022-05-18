using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlazorTemplater.ServiceProviderComposition
{

    /// <summary>
    /// a service builder able to create a service provider that can resolve services form a core service provider or from additional ones
    /// </summary>
    internal class ComposableServiceBuilder
    {
        private readonly IServiceProvider coreServiceProvider;
        private readonly IEnumerable<IServiceProvider> composingProviders;

        /// <summary>
        /// creates a composables service builder
        /// </summary>
        /// <param name="coreServiceProvider">the base core service provider</param>
        /// <param name="composingProviders">additional service provider to query one after the other</param>
        public ComposableServiceBuilder(IServiceProvider coreServiceProvider, IEnumerable<IServiceProvider> composingProviders = null)
        {
            this.coreServiceProvider = coreServiceProvider;
            this.composingProviders = composingProviders;
        }

        /// <summary>
        /// build a new service provider
        /// </summary>
        /// <returns>
        /// The returned service provider is the coreServiceProvider if no composingProviders are passed in the constructor,
        /// otherwise a ComposingServiceProvider for all the service providers and scopes will be created
        /// </returns>
        public IServiceProvider Build()
        {
            if (composingProviders is IEnumerable<IServiceProvider> composingServiceProviders)
            {
                var scopes = composingServiceProviders.Select(sp => sp.CreateScope());
                return new ComposingServiceProvider(coreServiceProvider, scopes);
            }
            return coreServiceProvider;
        }
    }
}