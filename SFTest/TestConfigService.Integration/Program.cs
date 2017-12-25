using Microsoft.ServiceFabric.Services.Client;
using Newtonsoft.Json;
using System;
using System.Fabric;
using System.Net.Http;
using System.Threading;

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
            var endpoint = partition.GetEndpoint();
            dynamic endpoints = JsonConvert.DeserializeObject(endpoint.Address);
            var client = new HttpClient();
            var response = client.GetAsync(endpoints.Endpoints[""].Value + "/api/config/1c05d13fbf3448309816d614883e8df2").GetAwaiter().GetResult();
            var config = JsonConvert.DeserializeObject<CustomConfig>(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());

            if (config != null)
            {
                Console.WriteLine("ID:{0}, AppType:{1}", config.Id, config.ApproximationType);
            }
            
            Console.ReadKey();
        }
    }

    public class CustomConfig
    {
        public Guid Id { set; get; }

        public string ApproximationType { set; get; }
    }
}
