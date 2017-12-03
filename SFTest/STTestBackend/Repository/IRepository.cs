using System.Collections.Generic;

namespace STTestBackend.Repository
{
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Return all instances of type T.
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> All();
        void Create(T entity);
    }
}
