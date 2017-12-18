using Microsoft.ServiceFabric.Services.Client;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestConfigService.Integration
{
    class Program
    {
        static void Main(string[] args)
        {
            ServicePartitionResolver resolver = ServicePartitionResolver.GetDefault();
            var cancellationTokenS = new CancellationTokenSource();
            ResolvedServicePartition partition =
                resolver.ResolveAsync(
                    new Uri("fabric:/SFTest/ConfigurationService"), 
                    new ServicePartitionKey(0), 
                    cancellationTokenS.Token).GetAwaiter().GetResult();
            Console.WriteLine("ID:{0}, Key:{1}", partition.Info.Id, partition.Info.Kind);
            Console.ReadKey();
        }
    }
}
