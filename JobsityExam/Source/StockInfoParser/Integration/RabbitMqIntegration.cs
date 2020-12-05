using RabbitMQ.Client;
using System.Text;

namespace StockInfoParserAPI.Integration
{
    public class RabbitMqIntegration : IQueueIntegration
    {
        private readonly IConnectionFactory _factory;
        private readonly static string _defaultQueue = "stock_quote_queue2";
        
        public RabbitMqIntegration(IConnectionFactory factory)
        {
            _factory = factory;
        }

        public void PostMessage(string message)
        {
            using (var connection = _factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclareNoWait(queue: _defaultQueue,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: string.Empty,
                                    routingKey: _defaultQueue,
                                    basicProperties: null,
                                    body: body);
            }
        }
    }
}
