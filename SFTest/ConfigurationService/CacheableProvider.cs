using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using System;
using System.Threading.Tasks;

namespace ConfigurationService
{
    public abstract class CacheableProvider<TKey, TConfig> : IDataProvider<TKey,TConfig> where TConfig: class
                                                                                         where TKey: IComparable<TKey>, IEquatable<TKey>
    {
        private readonly IReliableStateManager _reliableStateManager;
        
        protected CacheableProvider(IReliableStateManager reliableStateManager)
        {
            _reliableStateManager = reliableStateManager;
        }

        public async Task<TConfig> GetAsync(TKey id)
        {
            var collection = await _reliableStateManager.GetOrAddAsync<IReliableDictionary<TKey, TConfig>>("ConfigStore");

            using (var tran = _reliableStateManager.CreateTransaction())
            {
                if (await collection.ContainsKeyAsync(tran, id))
                {
                    return (await collection.TryGetValueAsync(tran, id)).Value;
                }

                var config = await this.GetByIdAsync(id);
                await collection.AddAsync(tran, id, config);
                await tran.CommitAsync();
                return config;
                
            }
        }

        protected abstract Task<TConfig> GetByIdAsync(TKey id);
    }
}
