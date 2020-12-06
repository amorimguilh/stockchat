using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace StockConsumer.Integration
{
    public class StockFileInfoIntegration : IStockFileInfoIntegration
    {
        private static readonly string url = "https://stooq.com/q/l/?s={0}&f=sd2t2ohlcv&h&e=csv";
        private static readonly char splitCharacter = ',';

        public StockFileInfoIntegration()
        {

        }

        public async Task<(List<string> fileHeaders, List<string> fileContent)> GetStockInfoFile(string stock)
        {
            List<string> fileHeaders = null;
            List<string> fileContent = null;

            using (var client = new HttpClient())
            using (var response = await client.GetAsync(string.Format(url, stock)))
            using (var fileStream = await response.Content.ReadAsStreamAsync())
            using (var reader = new StreamReader(fileStream))
            {
                if (!reader.EndOfStream)
                {
                    fileHeaders = reader.ReadLine().Split(splitCharacter).ToList();
                    fileContent = reader.ReadLine().Split(splitCharacter).ToList();
                }
            }

            return (fileHeaders, fileContent);
        }
    }
}
