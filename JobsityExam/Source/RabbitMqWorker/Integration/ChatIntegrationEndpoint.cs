using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMqWorker.Integration
{
    public class ChatIntegrationEndpoint : IChatIntegrationEndpoint
    {
        private static readonly string _endpointUri = "https://localhost:44382/api/postmessage"; //change localhost to environment variable
        private readonly ILogger<ChatIntegrationEndpoint> _logger;
        
        public ChatIntegrationEndpoint(ILogger<ChatIntegrationEndpoint> logger)
        {
            _logger = logger;
        }

        public async Task PostStockInfoOnChat(string message)
        {
            if(string.IsNullOrWhiteSpace(message))
            {
                _logger.LogError($"{nameof(ChatIntegrationEndpoint)} called with empty argument");
                return;
            }

            await Task.Yield();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_endpointUri);
                var stringContent = new StringContent(message, Encoding.UTF8, "application/json");
                client.PostAsync(string.Empty, stringContent);
            }
        }
    }
}
