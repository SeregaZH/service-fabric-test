using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFTestStateless.Intergation.Tests;

namespace SFTestStateless.Intergation
{
    class Program
    {
        static IDictionary<string, Func<HttpClient, int, CancellationTokenSource, CancellationTokenSource>> tests 
            = new Dictionary<string, Func<HttpClient, int, CancellationTokenSource, CancellationTokenSource>>()
        {
            {"person", PersonTest},
            {"temperature", TemperatureTest },
            {"pressure", PressureTest }
        };

        static void Main(string[] args)
        {
            var client = new HttpClient()
            {
                BaseAddress = new Uri("http://localhost:9057")
            };

            var delayArg = args.FirstOrDefault(arg => arg.StartsWith("delay"));
            var delay = int.Parse(delayArg?.Split(':')[1] ?? "1000");
            var tokens = args
                .TakeWhile(arg => !arg.StartsWith("delay"))
                .Select(arg => (arg, tests[arg].Invoke(client, delay, new CancellationTokenSource())))
                .ToDictionary(typle => typle.Item1, tuple => tuple.Item2);

            string exit = "";
            while (!exit.Equals("exit"))
            {
                exit = Console.ReadLine();
                if (tokens.ContainsKey(exit))
                {
                    if (!tokens[exit].IsCancellationRequested)
                    {
                        tokens[exit].Cancel();
                    }
                }
            }
        }

        static CancellationTokenSource PersonTest(HttpClient client, int delay, CancellationTokenSource tokenSource)
        {
            Task.Factory.StartNew(() => 
            {
                try
                {
                    for (int i = 0; i < 10; i++)
                    {
                        var jsonContent = JsonConvert.SerializeObject(new PersonProxy($"Joe-{i}", $"Foe-{i}", DateTime.Now));
                        HttpContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                        var result = client.PostAsync("/api/persons", content).GetAwaiter().GetResult();
                        var responseContent = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        Console.WriteLine($"Status: {result.StatusCode}, Content: {responseContent}, Type: Person");
                        Task.Delay(delay).Wait();
                    }
                }
                catch (Exception e) 
                {
                    Console.WriteLine("Person is off");
                }
                
            }, tokenSource.Token);

            return tokenSource;
        }

        static CancellationTokenSource TemperatureTest(HttpClient client, int delay, CancellationTokenSource tokenSource)
        {
            var random = new Random();
            Task.Factory.StartNew(() => 
            {
                try
                {
                    while (!tokenSource.IsCancellationRequested)
                    {
                        var temperature = new Quantity<int>(random.Next(288, 320), new Unit("K"));
                        var jsonTemperature = JsonConvert.SerializeObject(temperature);
                        HttpContent content = new StringContent(jsonTemperature, Encoding.UTF8, "application/json");
                        var result = client.PostAsync("api/temperature", content, tokenSource.Token).GetAwaiter().GetResult();
                        var responseContent = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        Console.WriteLine($"Status: {result.StatusCode}, Content: {responseContent}, Type: Temperature");
                        Task.Delay(delay).Wait();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Temperature is off");
                }                
            }, tokenSource.Token);

            return tokenSource;
        }

        static CancellationTokenSource PressureTest(HttpClient client, int delay, CancellationTokenSource tokenSource)
        {
            var random = new Random();
            Task.Factory.StartNew(() =>
            {
                try
                {
                    while (!tokenSource.IsCancellationRequested)
                    {
                        var temperature = new Quantity<int>(random.Next(200, 400), new Unit("Ba"));
                        var jsonTemperature = JsonConvert.SerializeObject(temperature);
                        HttpContent content = new StringContent(jsonTemperature, Encoding.UTF8, "application/json");
                        var result = client.PostAsync("api/pressure", content, tokenSource.Token).GetAwaiter().GetResult();
                        var responseContent = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        Console.WriteLine($"Status: {result.StatusCode}, Content: {responseContent}, Type: Pressure");
                        Task.Delay(delay).Wait();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Pressure is off");
                }
            }, tokenSource.Token);

            return tokenSource;
        }
    }
}
