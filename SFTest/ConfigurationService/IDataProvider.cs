using System.Threading.Tasks;

namespace ConfigurationService
{
    public interface IDataProvider<TKey, TConfig>
    {
        Task<TConfig> GetAsync(TKey id);
    }
}
