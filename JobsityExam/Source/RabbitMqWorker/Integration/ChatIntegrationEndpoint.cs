using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMqWorker.Models;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMqWorker.Integration
{
    public class ChatIntegrationEndpoint : IChatIntegrationEndpoint
    {
        private static readonly string _endpointUri = "http://localhost:15198/api/chat/send";
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
                var jsonObject = JsonConvert.SerializeObject(new MessageRequest
                {
                    User = "bot",
                    Message = message.Replace(" ", String.Empty)
                });
                var stringContent = new StringContent(jsonObject, Encoding.UTF8, "application/json");
                var result = await client.PostAsync(string.Empty, stringContent);
            }
        }
    }
}
