using RabbitMQ.Client;
using System.Text;

namespace StockInfoParserAPI.Integration
{
    public class RabbitMqIntegration : IQueueIntegration
    {
        private readonly IConnectionFactory _factory;
        private readonly static string _defaultQueue = "stock_quote_queue";

        public RabbitMqIntegration(string hostName, string userName, string password, int port = 5672)
        {
            _factory = new ConnectionFactory { HostName = hostName, UserName = userName, Password = password, Port = port };
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
