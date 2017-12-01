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

namespace STTestBackend
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance. 
    /// </summary>
    internal sealed class STTestBackend : StatelessService
    {
        private const string ConnectionStirng = "Endpoint=sb://sftest-ns.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=us5ko0G3XUu4qZhD2nmyvaXWjAfxKUI9mPihWzy3f58=";
        private const string EntityPath = "sftest-eh";
        private const string LeaseConnection = "lease1";

        private const string StoreConnectionString =
            "DefaultEndpointsProtocol=https;AccountName=sftestsz;AccountKey=7FhNYKX4Q7eCPykhDrPdTWiVaf6b6XfMirivcykniW2DB5hwCEzvDn8/J5GTGpvs0rKQGL+cDwscyIrTvKHtPQ==;EndpointSuffix=core.windows.net";
        public STTestBackend(StatelessServiceContext context)
            : base(context)
        { }

        protected override Task RunAsync(CancellationToken cancellationToken)
        {
            var eventProcessorHost = new EventProcessorHost(
                    EntityPath,
                    PartitionReceiver.DefaultConsumerGroupName,
                    ConnectionStirng,
                    StoreConnectionString,
                    LeaseConnection);
            eventProcessorHost.RegisterEventProcessorAsync<PersonEventProcessor>();
            cancellationToken.Register(async () => await eventProcessorHost.UnregisterEventProcessorAsync());
            return base.RunAsync(cancellationToken);
        }
    }
}
