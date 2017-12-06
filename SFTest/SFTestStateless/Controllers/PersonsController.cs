using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFTestStateless.Models;

namespace SFTestStateless.Controllers
{
    [Route("api/[controller]")]
    public class PersonsController: Controller
    {
        private readonly IQueueClient _sender;

        public PersonsController(IQueueClient sender)
        {
            _sender = sender;
        }

        [HttpPost]
        public async void PostAsync([FromBody]Person persons)
        {
            await _sender.SendAsync(persons);
        }

        [HttpGet]
        public IEnumerable<Person> Get()
        {
            await 
        }
    }
}
