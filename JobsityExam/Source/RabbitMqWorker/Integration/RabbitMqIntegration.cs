using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RabbitMqWorker.Integration
{
    /// <summary>
    /// Implemments a rabbit mq integration 
    /// </summary>
    public class RabbitMqIntegration : IQueueIntegration
    {
        private const string post_chat_queue = "post_chat_queue";
        private const string get_stock_quote_queue = "get_stock_quote_queue";
        private readonly IModel _channel;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RabbitMqIntegration(IModel channel, IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));

            _channel = channel;
            _channel.QueueDeclare(queue: post_chat_queue,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            _channel.QueueDeclare(queue: get_stock_quote_queue,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += ProcessMessage;

            _channel.BasicConsume(queue: post_chat_queue,
                                  autoAck: true,
                                  consumer: consumer);

            _channel.BasicConsume(queue: get_stock_quote_queue,
                                  autoAck: true,
                                  consumer: consumer);
        }

        /// <summary>
        /// Reads a message in both queues and directs to the correct processor
        /// A message can be send to the chat, or be processed to get the stock value
        /// post_chat_queue: Queue that stores the info to post the stock value in the chat app
        /// get_stock_quote_queue: Queue that stores the requests of users for the quotation of stocks
        /// </summary>
        public void ProcessMessage(object model, BasicDeliverEventArgs ea)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            
            var body = ea.Body.ToArray();
            var content = Encoding.UTF8.GetString(body);

            switch (ea.RoutingKey)
            {
                case post_chat_queue:
                    {
                        var chatIntegration = scope.ServiceProvider.GetRequiredService<IChatIntegrationEndpoint>();
                        chatIntegration.PostStockInfoOnChat(content);
                    }
                    break;
                case get_stock_quote_queue:
                    {
                        var stockInfoIntegration = scope.ServiceProvider.GetRequiredService<IStockInfoIntegrationEndpoint>();
                        stockInfoIntegration.SendRequestStockInfo(content);
                    }
                    break;
            }
        }
    }
}
