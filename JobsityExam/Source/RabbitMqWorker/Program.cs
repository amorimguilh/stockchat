using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMqWorker.Integration;

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
                    //services.AddScoped<IStockFileInfoIntegration, StockFileInfoIntegration>();

                    var rabbitMqConf = hostContext.Configuration.GetSection("RabbitMq");
                    var factory = new ConnectionFactory
                    {
                        HostName = rabbitMqConf.GetValue<string>("host"),
                        UserName = rabbitMqConf.GetValue<string>("username"),
                        Password = rabbitMqConf.GetValue<string>("password"),
                        Port = rabbitMqConf.GetValue<int>("port")
                    };

                    var connection = factory.CreateConnection();
                    services.AddSingleton(connection);
                   

                    services.AddSingleton<IQueueIntegration, RabbitMqIntegration>();
                    services.AddHostedService<Worker>();
                });
    }
}
