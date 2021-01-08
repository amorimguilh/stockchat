using System.Threading.Tasks;

namespace RabbitMqWorker.Integration
{
    /// <summary>
    /// Contract to the stock consumer api
    /// </summary>
    public interface IStockInfoIntegrationEndpoint
    {
        /// <summary>
        /// Integrates with the stock consumer sending a stock name to be processed
        /// </summary>
        Task SendRequestStockInfo(string stockName);
    }
}
