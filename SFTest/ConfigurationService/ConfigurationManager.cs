using System;

namespace ConfigurationService
{
    public class ConfigurationManager : IConfigurationManager
    {
        public ConfigurationManager(
            string connectionString,
            Uri serviceUri)
        {
            ConnectionString = connectionString;
            ServiceUri = serviceUri;
        }

        public string ConnectionString { get; private set; }

        public Uri ServiceUri { get; private set; }
    }
}
