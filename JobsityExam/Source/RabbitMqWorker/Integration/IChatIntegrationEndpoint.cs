using System.Threading.Tasks;

namespace RabbitMqWorker.Integration
{
    public interface IChatIntegrationEndpoint
    {
        Task PostStockInfoOnChat(string message);
    }
}
