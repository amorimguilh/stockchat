using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using StockInfoParserAPI.Integration;

namespace JobsityExam
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
            var rabbitMqConf = Configuration.GetSection("RabbitMq");
            services.AddSingleton<IConnectionFactory>(
                new ConnectionFactory
                {
                    HostName = rabbitMqConf.GetValue<string>("host"),
                    UserName = rabbitMqConf.GetValue<string>("username"),
                    Password = rabbitMqConf.GetValue<string>("password"),
                    Port = rabbitMqConf.GetValue<int>("port")
                });

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
