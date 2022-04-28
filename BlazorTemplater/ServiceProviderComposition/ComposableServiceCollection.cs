using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorTemplater.ServiceProviderComposition
{
    /// <summary>
    /// Enables the composition of existing service provider on a service collection
    /// </summary>
    internal class ComposableServiceCollection : ServiceCollection, ICollection<IServiceProvider>, IEnumerable<IServiceProvider>, IList<IServiceProvider>
    {
        private readonly IList<IServiceProvider> _serviceProvider = new List<IServiceProvider>();

        IServiceProvider IList<IServiceProvider>.this[int index] { get => _serviceProvider[index]; set => _serviceProvider[index] = value; }

        /// <summary>
        /// Allows to add an existing service provider to the the reslution ability of the current service collection
        /// </summary>
        /// <remarks>Composed service providers are queryed in the order they are added</remarks>
        /// <param name="item">An additional service provider to use to resolve services, all the services from the composed service provider will be available for the resolution</param>
        public void Add(IServiceProvider item)
        {
            _serviceProvider.Add(item);
        }

        /// <summary>
        /// clears the collection of services and service providers
        /// </summary>
        public new void Clear()
        {
            base.Clear();
            _serviceProvider.Clear();
        }

        /// <summary>
        /// check if the service provider is included in the collection
        /// </summary>
        /// <param name="item">the item to check</param>
        /// <returns>true if the item is in the collection of service providers, false otherwise</returns>
        public bool Contains(IServiceProvider item)
        {
            return _serviceProvider.Contains(item);
        }

        /// <summary>
        /// copy the service providers to an array
        /// </summary>
        /// <param name="array">target array to fill</param>
        /// <param name="arrayIndex">index to use as start</param>
        public void CopyTo(IServiceProvider[] array, int arrayIndex)
        {
            _serviceProvider.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// retutns the index of the searched item
        /// </summary>
        /// <param name="item">item to search</param>
        /// <returns>the index of the searched item if present -1 otherwise</returns>
        public int IndexOf(IServiceProvider item)
        {
            return _serviceProvider.IndexOf(item);
        }

        /// <summary>
        /// inserts an item at the given idex
        /// </summary>
        /// <param name="index">index of the inserted item after the insertion</param>
        /// <param name="item">item to insert</param>
        public void Insert(int index, IServiceProvider item)
        {
            _serviceProvider.Insert(index, item);
        }

        /// <summary>
        /// removes a service provider from the list of service provider used for the resolution
        /// </summary>
        /// <param name="item">item to remove</param>
        /// <returns>true if the item was present and therefore removed, false otherwise</returns>
        public bool Remove(IServiceProvider item)
        {
            return _serviceProvider.Remove(item);
        }

        /// <summary>
        /// obtains en enumerators of all the service providers
        /// </summary>
        /// <returns>an enumerator of all the service providers in the collection</returns>
        IEnumerator<IServiceProvider> IEnumerable<IServiceProvider>.GetEnumerator()
        {
            return _serviceProvider.GetEnumerator();
        }
    }
}
