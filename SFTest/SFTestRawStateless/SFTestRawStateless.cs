using System.Collections.Generic;
using System.Fabric;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace SFTestRawStateless
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class SFTestRawStateless : StatelessService
    {
        public SFTestRawStateless(StatelessServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[]
            {
                new ServiceInstanceListener(
                    context => 
                        new WebListenerCommunicationListener(context, "ServiceEndpoint", (url, listener) => {

                            var hostBuilder = new WebHostBuilder()
                                .UseWebListener()
                                .ConfigureServices(services =>  {
                                    services.AddSingleton<StatelessServiceContext>(context);
                                })
                                .UseContentRoot(Directory.GetCurrentDirectory())
                                .UseStartup<Startup>()
                                .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None)
                                .UseUrls(url);

                            return hostBuilder.Build();
                        })
                    )
            };
        }

    }
}
