using Dapper;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace STTestBackend.Repository
{
    public class PersonRepository : IRepository<Model.Person>
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
                         entity.Id,
                         entity.LastName,
                         entity.FirstName,
                         entity.FullName,
                         entity.BirthDate
                    });
            }

            return rowsAffected > 0;
        }
    }
}
