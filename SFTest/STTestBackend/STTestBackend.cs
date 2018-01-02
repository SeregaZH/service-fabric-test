using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
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

            try
            {
                await eventProcessorHost.RegisterEventProcessorFactoryAsync(new A(connectionString, Context));
                cancellationToken.Register(async () => await eventProcessorHost.UnregisterEventProcessorAsync());
            }
            catch (Exception e)
            {

            }
            finally
            {
                await base.RunAsync(cancellationToken);
            }
        }
    }

    public class A : IEventProcessorFactory
    {
        private readonly string _connectionString;
        private readonly ServiceContext _context;

        public A(string connectionString, ServiceContext context)
        {
            _connectionString = connectionString;
            _context = context;
        }

        public IEventProcessor CreateEventProcessor(PartitionContext context)
        {
            return new PersonEventProcessor(
                _context,
                new PersonRepository(_connectionString),
                new TemperatureRepository(_connectionString),
                new PressureRepository(_connectionString)
                );
        }
    }
}