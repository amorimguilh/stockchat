using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockInfoParserAPI.Integration
{
    public interface IStockFileInfoIntegration
    {
        Task<(List<string> fileHeaders, List<string> fileContent)> GetStockInfoFile(string stock);
    }
}
