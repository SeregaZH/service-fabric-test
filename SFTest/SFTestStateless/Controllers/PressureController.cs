using Microsoft.AspNetCore.Mvc;
using SFTestStateless.Models;


namespace SFTestStateless.Controllers
{
    [Route("api/[controller]")]
    public class PressureController: Controller
    {
        private readonly IQueueClient _sender;

        public PressureController(IQueueClient sender)
        {
            _sender = sender;
        }

        [HttpPost]
        public async void PostAsync([FromBody]Quantity<int> temperature)
        {
            await _sender.SendAsync(temperature, "pressure");
        }
    }
}
