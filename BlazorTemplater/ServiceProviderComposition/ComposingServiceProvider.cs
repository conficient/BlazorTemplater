using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlazorTemplater.ServiceProviderComposition
{
    /// <summary>
    /// A service provider able to resolve services from a core service provider or from a service scope list
    /// </summary>
    internal class ComposingServiceProvider : IServiceProvider, IDisposable
    {
        private IServiceProvider _coreServiceProvider;
        private IEnumerable<IServiceScope> _scopes;


        /// <summary>
        /// Creates a composing service provider
        /// </summary>
        /// <param name="coreServiceProvider">the base service provider used first</param>
        /// <param name="scopes">a series of additional service scope to use une after other to perform additional resolutions</param>
        public ComposingServiceProvider(IServiceProvider coreServiceProvider, IEnumerable<IServiceScope> scopes)
        {
            _coreServiceProvider = coreServiceProvider;                      
            _scopes = new List<IServiceScope>(scopes);  //force enumeration so to not need to re-execute the enumerator each time later in the foreach of GetService
            //HINT: NOTE: do not use scopes.ToList() but use new List<IServiceScope>(scopes) instead because .ToList will not work as the real implementation of "scope"
            //is an object that implments both IEnumerable<IServiceDescriptor> and IEnumerable<IServiceScope> 
            //the .ToList will default to the IEnumerable<IServiceScope>.GetEnumerator and will result in copying nothing.
            //instead the new List<IServiceScope> will use the correct override of the GetEnumerator to extract the elements
        }

        /// <summary>
        /// obtain a service of the given type
        /// </summary>
        /// <param name="serviceType">the type to search and instantiate</param>
        /// <returns>an instance of the searched type fromthe core service provider, or form one of the additional scopes searched in order</returns>
        public object GetService(Type serviceType)
        {
            if (_coreServiceProvider.GetService(serviceType) is object foundService) return foundService;

            foreach (var scope in _scopes)
            {
                if (scope.ServiceProvider.GetService(serviceType) is object serviceFromScope) return serviceFromScope;
            }            

            return null;
        }

        //disposes all the scopes
        public void Dispose()
        {
            foreach (var scope in _scopes) scope.Dispose();
        }
    }
}