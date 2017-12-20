using Dapper;
using Microsoft.ServiceFabric.Data;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ConfigurationService
{
    public class CustomConfigProvider : CacheableProvider<Guid, CustomConfig>
    {
        private readonly IConfigurationManager _configurationManager;

        public CustomConfigProvider(
            IConfigurationManager configurationManager,
            IReliableStateManager reliableStateManager
        ) : base(reliableStateManager)
        {
            _configurationManager = configurationManager;
        }

        protected override async Task<CustomConfig> GetByIdAsync(Guid id)
        {
            using (var connection = new SqlConnection(_configurationManager.ConnectionString))
            {
                var config = await connection.QuerySingleOrDefault(
                @"SELECT [Id],[ApproximationType] WHERE Id = @Id",
                new { Id = id });
                return config;
            }
        }
    }
}
