using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using StockConsumer.Integration;
using System;
using System.Threading;

namespace StockConsumer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var envVariable = Environment.GetEnvironmentVariable("RABBIT_MQ_HOST");

            Thread.Sleep(8000);

            var factory = new ConnectionFactory()
            {
                Uri = new Uri($"amqp://user:mysecretpassword@{envVariable}")
            };

            var channel = factory.CreateConnection().CreateModel();
            services.AddSingleton(channel);

            services.AddScoped<IStockFileInfoIntegration, StockFileInfoIntegration>();
            services.AddScoped<IQueueIntegration, RabbitMqIntegration>();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
