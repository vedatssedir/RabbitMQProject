using System;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace RabbitMQWeb.Watermark.Services
{
    public class RabbitMqClientService :IDisposable
    {
        private readonly ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;
        public const string ExchangeName = "ImageDirectExchange";
        public const string RoutingWatermark = "watermark-route-image";
        public const string QueueName = "queue-watermark-image";
        private readonly ILogger<RabbitMqClientService> _logger;

        public RabbitMqClientService(ConnectionFactory connectionFactory, ILogger<RabbitMqClientService> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
            Connect();
        }
        public IModel Connect()
        {
            _connection = _connectionFactory.CreateConnection();
            if (_channel is { IsOpen: true })
            {
                return _channel;
            }
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(ExchangeName,type:ExchangeType.Direct,true);
            _channel.QueueDeclare(QueueName, true, false, false, null);
            _channel.QueueBind(exchange:ExchangeName,queue:QueueName,routingKey:RoutingWatermark);
            _logger.LogInformation("RabbitMQ ile baglantı kuruldu.");
            return _channel;
        }


        public void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
            _logger.LogInformation("RabbitMQ ile baglantı kopt");
        }
    }
}
