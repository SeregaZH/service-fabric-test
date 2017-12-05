using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace STTestBackend.Repository
{
    public class PersonRepository<T> : IRepository<Model.Person>
    {
        private readonly string _connectionString;

        public PersonRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool Create(Model.Person entity)
        {
            int rowsAffected = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                rowsAffected = connection.Execute(@"INSERT Persons([CustomerFirstName],[CustomerLastName],[IsActive]) values (@CustomerFirstName, @CustomerLastName, @IsActive)", new { CustomerFirstName = ourCustomer.CustomerFirstName, CustomerLastName = ourCustomer.CustomerLastName, IsActive = true });
            }

            return rowsAffected > 0;
        }
    }
}
