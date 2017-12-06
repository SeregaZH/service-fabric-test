using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;

namespace SFTestStateless.Intergation.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new HttpClient()
            {
                BaseAddress = new Uri("http://localhost:9057")
            };

            for (int i = 0; i < 100; i++)
            {
                var jsonContent = JsonConvert.SerializeObject(new PersonProxy($"Joe-{i}", $"Foe-{i}", DateTime.Now));
                HttpContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var result = client.PostAsync("/api/persons", content).GetAwaiter().GetResult();
                var responseContent = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Console.WriteLine($"Status: {result.StatusCode}, Content: {responseContent}");
            }

            Console.ReadKey(); 
        }
    }
}
