using System;
using System.Collections.Generic;
using System.Linq;

namespace STTestBackend.Repository
{
    public class Repository<T> : IRepository<T>
    where T : class
    {
        public IEnumerable<T> All()
        {
            using (var connection = new System.Data.SqlClient.SqlConnection())
            {

            }
        }

        public void Create(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
