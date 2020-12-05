using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMqWorker.Integration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMqWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public Worker(ILogger<Worker> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceScopeFactory.CreateScope();

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                var queueIntegration = scope.ServiceProvider.GetRequiredService<IQueueIntegration>();

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
