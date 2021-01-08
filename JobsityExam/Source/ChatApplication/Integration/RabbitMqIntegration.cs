using ChatApplication.Configurations;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace ChatApplication.Integration
{
    /// <summary>
    /// Implemments a rabbit mq integration 
    /// </summary>
    public class RabbitMqIntegration : IQueueIntegration
    {
        private readonly IModel _channel;
        private readonly string _queueName;

        /// <summary>
        /// Instantiate a object to publish a message in rabbitmq
        /// </summary>
        public RabbitMqIntegration(
            IModel channel,
            IChatConfiguration configuration)
        {
            _queueName = configuration.QueueName;
            _channel = channel;
            _channel.QueueDeclare(queue: _queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
        }

        /// <summary>
        /// Publishes a message in a rabbit mq queue
        /// </summary>
        public void PublishMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: string.Empty,
                                routingKey: _queueName,
                                basicProperties: null,
                                body: body);
        }
    }
}