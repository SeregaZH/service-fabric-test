using System;

namespace ConfigurationService
{
    public interface IConfigurationManager
    {
        string ConnectionString { get; }
        Uri ServiceUri { get; }
    }
}
