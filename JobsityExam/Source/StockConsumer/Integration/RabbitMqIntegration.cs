using RabbitMQ.Client;
using System.Text;

namespace StockConsumer.Integration
{
    public class RabbitMqIntegration : IQueueIntegration
    {
        private readonly IModel _channel;

        private readonly static string post_chat_queue = "post_chat_queue";

        public RabbitMqIntegration(IModel channel)
        {
            _channel = channel;
            _channel.QueueDeclareNoWait(queue: post_chat_queue,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
        }

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
