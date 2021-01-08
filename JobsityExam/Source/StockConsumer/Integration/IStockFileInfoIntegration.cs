using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockConsumer.Integration
{
    /// <summary>
    /// Contract to the external service integration that returns the stock value
    /// </summary>
    public interface IStockFileInfoIntegration
    {
        /// <summary>
        /// consumes an external service and return a list containing the header and lines 
        /// of the csv file that holds the stock price information
        /// </summary>
        Task<(List<string> fileHeaders, List<string> fileContent)> GetStockInfoFile(string stock);
    }
}
