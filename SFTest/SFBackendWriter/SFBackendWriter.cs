using System;
using System.Collections.Generic;
using System.Fabric;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Microsoft.ServiceFabric.Services.Runtime;

namespace SFBackendWriter
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class SFBackendWriter : StatelessService
    {
        public SFBackendWriter(StatelessServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {

            var configurationPackage = Context.CodePackageActivationContext.GetConfigurationPackageObject("Config");
            var serviceBusConnectionString = configurationPackage.Settings.Sections["ConnectionString"]
                .Parameters["SFTestServiceBus"].Value;

            var factory = MessagingFactory.CreateFromConnectionString(serviceBusConnectionString);
            var subClient = factory.CreateSubscriptionClient("temperature", "temperature-writer");

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                IEnumerable<BrokeredMessage> messages = await subClient.ReceiveBatchAsync(20);

                if (messages.Any())
                {
                    foreach (var message in messages)
                    {
                        // var st = message.GetBody<Stream>();
                    }
                }

                ServiceEventSource.Current.ServiceMessage(Context, messages.Count().ToString());
                await Task.Delay(1000, cancellationToken);
            }
        }
    }
}
