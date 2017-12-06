using System.Threading.Tasks;

namespace STTestBackend.Repository
{
    public interface IRepository<T> where T : class
    {
        Task<bool> CreateAsync(T entity);
    }
}
