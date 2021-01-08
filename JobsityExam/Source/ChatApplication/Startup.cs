using ChatApplication.Configurations;
using ChatApplication.Hubs;
using ChatApplication.Integration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using System;
using System.Threading;

namespace ChatApplication
{
    public class Startup
    {
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(
                                name: MyAllowSpecificOrigins,
                                builder =>
                                {
                                    builder
                                    .AllowAnyMethod()
                                    .AllowAnyHeader()
                                    .AllowCredentials()
                                    .SetIsOriginAllowed((host) => true);
                                });
            });

            services.AddSignalR(conf => 
            {
                conf.EnableDetailedErrors = true;
            });

            var envVariable = Environment.GetEnvironmentVariable("RABBIT_MQ_HOST");

            Thread.Sleep(8000);

            var factory = new ConnectionFactory()
            {
                Uri = new Uri($"amqp://user:mysecretpassword@{envVariable}")
            };

            var channel = factory.CreateConnection().CreateModel();
            services.AddSingleton(channel);

            services.AddSingleton<IChatConfiguration, ChatConfiguration>();
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
            
            // UseCors adds the CORS middleware. The call to UseCors must be placed after UseRouting, but before UseAuthorization
            app.UseCors(MyAllowSpecificOrigins);

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("/chatsocket");
            });
        }
    }
}
