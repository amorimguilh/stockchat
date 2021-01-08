using System.Threading.Tasks;

namespace RabbitMqWorker.Integration
{
    /// <summary>
    /// Implements the IChatIntegrationEndpoint in order to allow the system to post 
    /// messages on the chat
    /// </summary>
    public interface IChatIntegrationEndpoint
    {
        /// <summary>
        /// Posts a given message in the chat application
        /// </summary>
        Task PostStockInfoOnChat(string message);
    }
}
