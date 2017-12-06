using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace STTestBackend.Repository
{
    public class PersonRepository<T> : IRepository<Model.Person>
    {
        private readonly string _connectionString;

        public PersonRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<bool> CreateAsync(Model.Person entity)
        {
            int rowsAffected = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                rowsAffected = await connection.ExecuteAsync(
                    @"INSERT Persons([Id],[FirstName],[LastName],[FullName],[BirthDate]) values (@Id, @FirstName, @LastName,@FullName,@BirthDate)", 
                    new {
                        Id = entity.Id,
                        LastName = entity.LastName,
                        FirstName = entity.FirstName,
                        FullName = entity.FullName,
                        BirthDate = entity.BirthDate
                    });
            }

            return rowsAffected > 0;
        }
    }
}
