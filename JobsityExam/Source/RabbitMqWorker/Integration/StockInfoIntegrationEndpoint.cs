using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMqWorker.Integration
{
    public class StockInfoIntegrationEndpoint : IStockInfoIntegrationEndpoint
    {
        private static readonly string _endpointUri = "https://localhost:44382/api/stockinfo";
        private readonly ILogger<StockInfoIntegrationEndpoint> _logger;

        public StockInfoIntegrationEndpoint(ILogger<StockInfoIntegrationEndpoint> logger)
        {
            _logger = logger;
        }

        public async Task SendRequestStockInfo(string stockName)
        {
            if (string.IsNullOrWhiteSpace(stockName))
            {
                _logger.LogError($"{nameof(StockInfoIntegrationEndpoint)} called with empty argument");
                return;
            }

            await Task.Yield();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_endpointUri);
                var stringContent = new StringContent(stockName.Replace(" ", String.Empty), Encoding.UTF8, "application/json");
                client.PostAsync(string.Empty, stringContent);
            }
        }
    }
}
