using System;
using System.Collections.Generic;
using System.Fabric;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using STTestBackend.Repository;

namespace STTestBackend
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance. 
    /// </summary>
    internal sealed class STTestBackend : StatelessService
    {
        private const string LeaseConnection = "lease1";

        public STTestBackend(StatelessServiceContext context)
            : base(context)
        {
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            var configurationPackage = Context.CodePackageActivationContext.GetConfigurationPackageObject("Config");
            var connectionString = configurationPackage.Settings.Sections["ConnectionString"].Parameters["SFTestDBConnection"].Value;
            var eventProcessorHost = new EventProcessorHost(
                    configurationPackage.Settings.Sections["ConnectionString"].Parameters["EntityPath"].Value,
                    PartitionReceiver.DefaultConsumerGroupName,
                    configurationPackage.Settings.Sections["ConnectionString"].Parameters["SFTestEventHub"].Value,
                    configurationPackage.Settings.Sections["ConnectionString"].Parameters["SFTestDB"].Value,
                    LeaseConnection);
            await eventProcessorHost.RegisterEventProcessorFactoryAsync(new A(connectionString));
            cancellationToken.Register(async () => await eventProcessorHost.UnregisterEventProcessorAsync());
            await base.RunAsync(cancellationToken);
        }
    }

    public class A : IEventProcessorFactory
    {
        private readonly string _connectionString;

        public A(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEventProcessor CreateEventProcessor(PartitionContext context)
        {
            return new PersonEventProcessor(new PersonRepository<Model.Person>(_connectionString));
        }
    }
}