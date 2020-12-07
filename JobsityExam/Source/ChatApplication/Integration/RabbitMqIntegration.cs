using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Tasks;

namespace ChatApplication.Integration
{
    public class RabbitMqIntegration : IQueueIntegration
    {
        private const string post_chat_queue = "post_chat_queue";
        //private readonly IModel _channel;

        public RabbitMqIntegration()//IModel channel)
        {
            //_channel = channel;
            //_channel.QueueDeclare(queue: post_chat_queue,
            //                     durable: false,
            //                     exclusive: false,
            //                     autoDelete: false,
            //                     arguments: null);

            //var consumer = new EventingBasicConsumer(_channel);
        }

        public async Task PublishMessage(string message)
        {
            //var body = Encoding.UTF8.GetBytes(message);

            //_channel.BasicPublish(exchange: string.Empty,
            //                    routingKey: post_chat_queue,
            //                    basicProperties: null,
            //                    body: body);
        }
    }
}
