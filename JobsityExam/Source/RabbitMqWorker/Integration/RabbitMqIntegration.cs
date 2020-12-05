using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace RabbitMqWorker.Integration
{
    public class RabbitMqIntegration : IQueueIntegration
    {
        private readonly IConnection _connection;
        private readonly static string _defaultQueue = "stock_quote_queue";
        private readonly static string _defaultQueue2 = "stock_quote_queue2";
        private readonly IModel _channel;

        public RabbitMqIntegration(IConnection connection)
        {
            _connection = connection;
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: _defaultQueue,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            _channel.QueueDeclare(queue: _defaultQueue2,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += ProcessMessage;

            _channel.BasicConsume(queue: _defaultQueue,
                                  autoAck: true,
                                  consumer: consumer);

            _channel.BasicConsume(queue: _defaultQueue2,
                                  autoAck: true,
                                  consumer: consumer);
        }

        public void ProcessMessage(object? model, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
        }
    }
}
