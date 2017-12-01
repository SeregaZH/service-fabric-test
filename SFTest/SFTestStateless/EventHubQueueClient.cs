using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Newtonsoft.Json;

namespace SFTestStateless
{
    public class EventHubQueueClient : IQueueClient
    {
        private const string ConnectionStirng = "Endpoint=sb://sftest-ns.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=us5ko0G3XUu4qZhD2nmyvaXWjAfxKUI9mPihWzy3f58=";
        private const string EntityPath = "sftest-eh";
        private readonly EventHubClient _eventHubClient;

        public EventHubQueueClient()
        {
            var connectionStringBuilder = new EventHubsConnectionStringBuilder(ConnectionStirng)
            {
                EntityPath = EntityPath
            };
            _eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());
        }

        public async Task SendAsync<T>(T entity)
        {
            var message = JsonConvert.SerializeObject(entity);

            await _eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(message)));
            await _eventHubClient.CloseAsync();
        }
    }
}
