using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMqWorker.Integration;
using System;
using System.Threading;

namespace RabbitMqWorker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var envVariable = Environment.GetEnvironmentVariable("RABBIT_MQ_HOST");

                    Thread.Sleep(8000);

                    var factory = new ConnectionFactory()
                    {
                        Uri = new Uri($"amqp://user:mysecretpassword@{envVariable}")
                    };

                    var channel = factory.CreateConnection().CreateModel();
                    services.AddSingleton(channel);
                    services.AddScoped<IStockInfoIntegrationEndpoint, StockInfoIntegrationEndpoint>();
                    services.AddScoped<IChatIntegrationEndpoint, ChatIntegrationEndpoint>();
                    services.AddSingleton<IQueueIntegration, RabbitMqIntegration>();

                    services.AddHostedService<Worker>();
                });
    }
}
