using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SampleApp.MessageBus;

namespace SampleApp.Controllers
{
    /// <summary>
    /// demo controller
    /// </summary>
    [Route("api/")]
    [ApiController]
    public class PublishController : ControllerBase
    {
        private readonly ILogger<PublishController> _logger;
        private readonly IBusControl _busControl;

        public PublishController(ILogger<PublishController> logger, IBusControl busControl)
        {
            _logger = logger;
            _busControl = busControl;
        }

        /// <summary>
        /// publish a message with masstransit
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("publish/{count}")]
        public async Task<ActionResult<string>> Publish(int count)
        {
            _logger.LogInformation("Publish {count}", count);

            for (int i = 0; i < count; i++)
            {
                await _busControl.Publish(new MyMessage());
            }
            
            return Ok();
        }
    }
}
