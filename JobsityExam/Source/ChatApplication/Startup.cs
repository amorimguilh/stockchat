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
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();

            services.AddCors(o => o.AddDefaultPolicy(builder =>
            {
            builder
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithExposedHeaders("Origin", "X-Requested-With", "Content-Type", "Accept", "Authorization", "Access", "Access-Control-Allow-Origin")
            .WithHeaders("Origin", "X-Requested-With", "Content-Type", "Accept", "Authorization", "Access", "Access-Control-Allow-Origin")
            .AllowAnyOrigin();
                //.AllowCredentials()
                //.WithOrigins("http://frontend:80","http://frontend:4200",
                //"http://localhost:4200", "http://localhost:80");
            }));

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
            app.UseCors();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("/chatsocket");
            });
        }
    }
}
