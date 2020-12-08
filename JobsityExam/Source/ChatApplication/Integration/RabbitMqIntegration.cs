using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Tasks;

namespace ChatApplication.Integration
{
    public class RabbitMqIntegration : IQueueIntegration
    {
        private const string get_stock_quote_queue = "get_stock_quote_queue";
        private readonly IModel _channel;

        public RabbitMqIntegration(IModel channel)
        {
            _channel = channel;
            _channel.QueueDeclare(queue: get_stock_quote_queue,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
        }

        public async Task PublishMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: string.Empty,
                                routingKey: get_stock_quote_queue,
                                basicProperties: null,
                                body: body);
        }
    }
}
