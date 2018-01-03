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

namespace STTestBackend
{
    public class PersonEventProcessor : IEventProcessor
    {
        private readonly IRepository<Model.Person> _personRepository;
        private readonly IRepository<Quantity<int>> _temperatureRepository;
        private readonly IRepository<Quantity<int>> _pressureRepository;
        private readonly ServiceContext _context;

        public PersonEventProcessor(
            ServiceContext context,
            IRepository<Model.Person> personRepository,
            IRepository<Quantity<int>> temperatureRepository,
            IRepository<Quantity<int>> pressureRepository
            )
        {
            _context = context;
            _personRepository = personRepository;
            _pressureRepository = pressureRepository;
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
                ServiceEventSource.Current.ServiceMessage(_context, "batch");
                foreach (var eventData in messages)
                {
                    var data = Encoding.UTF8.GetString(eventData?.Body != null ? eventData.Body.Array : new byte[0],
                        eventData.Body.Offset, eventData.Body.Count);
                    var eventObj = JsonConvert.DeserializeObject<Event>(data);
                    // ServiceEventSource.Current.ServiceMessage(_context, eventObj.Type);
                    switch (eventObj.Type)
                    {
                        case "temperature":
                        {
                            var temperature = JsonConvert.DeserializeObject<Quantity<int>>(eventObj.Content);
                            await _temperatureRepository.CreateAsync(temperature);
                            break;
                        }
                        case "pressure":
                        {
                            var pressure = JsonConvert.DeserializeObject<Quantity<int>>(eventObj.Content);
                            await _pressureRepository.CreateAsync(pressure);
                            break;
                        }
                        case "persons":
                        {
                            var person = JsonConvert.DeserializeObject<Person>(eventObj.Content);
                            var dataPerson = new Model.Person()
                            {
                                FirstName = person.FirstName,
                                LastName = person.LastName,
                                BirthDate = person.DateOfBirdth,
                                Id = Guid.NewGuid(),
                                FullName = $"{person.FirstName} {person.LastName}"
                            };
                            await _personRepository.CreateAsync(dataPerson);
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
    }
}
