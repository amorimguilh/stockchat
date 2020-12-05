using System.Threading.Tasks;

namespace RabbitMqWorker.Integration
{
    public interface IStockInfoIntegrationEndpoint
    {
        Task SendRequestStockInfo(string stockName);
    }
}
