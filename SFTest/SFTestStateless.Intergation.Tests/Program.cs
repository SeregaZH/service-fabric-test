using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFTestStateless.Intergation.Tests
{
    class Program
    {
        static IDictionary<string, Func<HttpClient, CancellationTokenSource, CancellationTokenSource>> tests 
            = new Dictionary<string, Func<HttpClient, CancellationTokenSource, CancellationTokenSource>>()
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

            var tokens = args
                .Select(arg => (arg, tests[arg].Invoke(client, new CancellationTokenSource())))
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

        static CancellationTokenSource PersonTest(HttpClient client, CancellationTokenSource tokenSource)
        {
            var task = Task.Factory.StartNew(() => 
            {
                for (int i = 0; i < 10; i++)
                {
                    var jsonContent = JsonConvert.SerializeObject(new PersonProxy($"Joe-{i}", $"Foe-{i}", DateTime.Now));
                    HttpContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    var result = client.PostAsync("/api/persons", content).GetAwaiter().GetResult();
                    var responseContent = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    Console.WriteLine($"Status: {result.StatusCode}, Content: {responseContent}");
                }
            }, tokenSource.Token);

            return tokenSource;
        }

        static CancellationTokenSource TemperatureTest(HttpClient client, CancellationTokenSource tokenSource)
        {
            return tokenSource;
        }

        static CancellationTokenSource PressureTest(HttpClient client, CancellationTokenSource tokenSource)
        {
            return tokenSource;
        }
    }
}
