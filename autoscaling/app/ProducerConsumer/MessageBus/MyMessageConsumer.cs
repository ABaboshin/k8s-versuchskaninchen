using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace SampleApp.MessageBus
{
    public class MyMessageConsumer :
        IConsumer<MyMessage>
    {
        private readonly ILogger<MyMessageConsumer> _logger;

        public MyMessageConsumer(ILogger<MyMessageConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<MyMessage> context)
        {
            await Task.Delay(5000);

            _logger.LogInformation("MyMessageConsumer.Consume.Done");
        }
    }
}
