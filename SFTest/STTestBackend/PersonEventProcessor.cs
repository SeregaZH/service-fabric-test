using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using Newtonsoft.Json;
using STTestBackend.Repository;
using SFTestBackend.Models;
using System.Fabric;
using System.Linq;
using Microsoft.Azure.ServiceBus;
using STTestBackend.Model;

namespace STTestBackend
{
    public class PersonEventProcessor : IEventProcessor
    {
        private readonly IRepository<Model.Person> _personRepository;
        private readonly IRepository<Quantity<int>> _temperatureRepository;
        private readonly IRepository<Quantity<int>> _pressureRepository;
        private readonly ITopicClient _topicClient;
        private readonly ServiceContext _context;
        
        public PersonEventProcessor(
            ServiceContext context,
            IRepository<Model.Person> personRepository,
            IRepository<Quantity<int>> temperatureRepository,
            IRepository<Quantity<int>> pressureRepository, 
            ITopicClient topicClient)
        {
            _context = context;
            _personRepository = personRepository;
            _pressureRepository = pressureRepository;
            _topicClient = topicClient;
            _temperatureRepository = temperatureRepository;
        }

        public Task OpenAsync(PartitionContext context)
        {
            return Task.CompletedTask;
        }

        public Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            return Task.CompletedTask;
        }

        public async Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            try
            {
                var messageGroups = messages
                    .Select(eventData =>
                        Encoding.UTF8.GetString(eventData?.Body != null ? eventData.Body.Array : new byte[0],
                            eventData.Body.Offset, eventData.Body.Count))
                    .Select(JsonConvert.DeserializeObject<Event>)
                    .GroupBy(x => x.Type)
                    .AsParallel();

                foreach (var group in messageGroups)
                {
                    switch (group.Key)
                    {
                        case "temperature":
                        {
                            var aggregateEvents = this.AggregatedEquipmentEvent(
                                group.Select(g => JsonConvert.DeserializeObject<Quantity<int>>(g.Content)),
                                new Unit("K"));
                            await _topicClient.SendAsync(new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(aggregateEvents))));
                            break;
                        }
                        case "pressure":
                        {
                            var aggregateEvents = this.AggregatedEquipmentEvent(group.Select(g => JsonConvert.DeserializeObject<Quantity<int>>(g.Content)), new Unit("Ba"));
                            break;
                        }
                        case "persons":
                        {
                            foreach (var personEvent in group)
                            {
                                var person = JsonConvert.DeserializeObject<Person>(personEvent.Content);
                                var dataPerson = new Model.Person()
                                {
                                    FirstName = person.FirstName,
                                    LastName = person.LastName,
                                    BirthDate = person.DateOfBirdth,
                                    Id = Guid.NewGuid(),
                                    FullName = $"{person.FirstName} {person.LastName}"
                                };
                                await _personRepository.CreateAsync(dataPerson);

                            }

                            break;
                        }
                    }
                }

            }
            catch (Exception e)
            {
                // ServiceEventSource.Current.ServiceMessage(_context, e.Message);
            }
            finally
            {
                await context.CheckpointAsync();
            }
        }

        public Task ProcessErrorAsync(PartitionContext context, Exception error)
        {
            return Task.CompletedTask;
        }

        public AggregatedEquipmentEvent AggregatedEquipmentEvent(IEnumerable<Quantity<int>> events, Unit unit)
        {
            return new AggregatedEquipmentEvent()
            {
                Value = new RangeQuantity<int>(events.Select(x => x.Value).ToList(), unit),
                Timestamp = DateTime.Now.ToUniversalTime()
            };
        }
    }
}
