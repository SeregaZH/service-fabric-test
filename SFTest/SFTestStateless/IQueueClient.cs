using System.Threading.Tasks;

namespace SFTestStateless
{
    public interface IQueueClient
    {
        Task SendAsync<T>(T entity, string type);
    }
}
