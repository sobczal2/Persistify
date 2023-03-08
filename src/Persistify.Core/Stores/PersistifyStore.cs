using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistify.Core.Stores
{
    public class PersistifyStore<T>
    {
        private bool _isInitialized;

        public PersistifyStore()
        {
            _isInitialized = false;
        }
        
        public async Task<Guid> IndexAsync(T item)
        {
            return await Task.FromResult(Guid.NewGuid());
        }
        
        public async Task<T> GetAsync(Guid id)
        {
            return await Task.FromResult(default(T));
        }
        
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await Task.FromResult(default(IEnumerable<T>));
        }
        
        public async Task<bool> SearchAsync(string query)
        {
            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await Task.FromResult(true);
        }
    }
}