using System;
using Microsoft.ServiceFabric.Data;

namespace ConfigurationService
{
    public class CustomConfig
    {
        public Guid Id { set; get; }

        public string ApproximationType { set; get; }
    }
}
