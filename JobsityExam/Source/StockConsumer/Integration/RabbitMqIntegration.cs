using RabbitMQ.Client;
using System.Text;

namespace StockConsumer.Integration
{
    /// <summary>
    /// Implemments a rabbit mq integration 
    /// </summary>
    public class RabbitMqIntegration : IQueueIntegration
    {
        private readonly IModel _channel;

        private readonly static string post_chat_queue = "post_chat_queue";
        
        /// <summary>
        /// Instantiate a object to publish a message in rabbitmq
        /// </summary>
        public RabbitMqIntegration(IModel channel)
        {
            _channel = channel;
            _channel.QueueDeclareNoWait(queue: post_chat_queue,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
        }

        /// <summary>
        /// Publishes a message in a rabbit mq queue
        /// </summary>
        public void PostMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: string.Empty,
                                routingKey: post_chat_queue,
                                basicProperties: null,
                                body: body);
        }
    }
}