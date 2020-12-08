using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMqWorker.Models;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMqWorker.Integration
{
    public class StockInfoIntegrationEndpoint : IStockInfoIntegrationEndpoint
    {
        private static readonly string _endpointUri = "http://stockconsumer/api/stockconsumer";
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
                var jsonObject = JsonConvert.SerializeObject(new StockConsumerRequest
                {
                    StockName = stockName.Replace(" ", String.Empty)
                });
                var stringContent = new StringContent(jsonObject, Encoding.UTF8, "application/json");
                await client.PostAsync(string.Empty, stringContent);
            }
        }
    }
}
