using System.Collections.Generic;

namespace STTestBackend.Repository
{
    public interface IRepository<T> where T : class
    {
        bool Create(T entity);
    }
}
