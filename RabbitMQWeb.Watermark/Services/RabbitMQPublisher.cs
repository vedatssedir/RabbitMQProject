using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace RabbitMQWeb.Watermark.Services
{
    public class RabbitMqPublisher
    {
        private readonly RabbitMqClientService _clientService;

        public RabbitMqPublisher(RabbitMqClientService clientService)
        {
            _clientService = clientService;
        }

        public void Publish(ProductImageCreatedEvent productImageCreatedEvent)
        {
            var channel = _clientService.Connect();
            var bodyString = JsonSerializer.Serialize(productImageCreatedEvent);
            var bodyByte = Encoding.UTF8.GetBytes(bodyString);
            var property = channel.CreateBasicProperties();
            property.Persistent = true;
            channel.BasicPublish(exchange:RabbitMqClientService.ExchangeName,routingKey:RabbitMqClientService.RoutingWatermark,basicProperties:property,body:bodyByte);
        }

    }
}
