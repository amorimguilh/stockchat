using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StockInfoParserAPI.Integration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace JobsityExam.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StockParserController : ControllerBase
    {
        private readonly ILogger<StockParserController> _logger;
        private readonly IQueueIntegration _queueIntegration;
        private static readonly string url = "https://stooq.com/q/l/?s=aapl.us&f=sd2t2ohlcv&h&e=csv";
        private static readonly char splitCharacter = ',';
        private static readonly string symbolHeaderDescription = "Symbol";
        private static readonly string closeValueHeaderDescription = "Close";

        public StockParserController(
            IQueueIntegration queueIntegration,
            ILogger<StockParserController> logger)
        {
            _logger = logger;
            _queueIntegration = queueIntegration;
        }

        [HttpGet]
        public async Task Get()
        {
            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync(url))
                using (var fileStream = await response.Content.ReadAsStreamAsync())
                {
                    if (fileStream.CanRead)
                    {
                        using (var reader = new StreamReader(fileStream))
                        {
                            if(!reader.EndOfStream)
                            {
                                var fileHeaders = reader.ReadLine().Split(splitCharacter);
                                var fileContent = reader.ReadLine().Split(splitCharacter);

                                if(fileHeaders.Length == fileContent.Length)
                                {
                                    // throw Exception - number of headers and columns does no match
                                }

                                var fileHeadersList = fileHeaders.ToList();
                                fileHeaders = null;

                                var stockNameIndex = fileHeadersList.IndexOf(symbolHeaderDescription);
                                var stockValueIndex = fileHeadersList.IndexOf(closeValueHeaderDescription);

                                //Not found indexes
                                if(stockNameIndex == -1 || stockValueIndex == -1)
                                {
                                    // throw Exception - not found required indexes
                                }

                                _queueIntegration.PostMessage($"{fileContent[stockNameIndex]} quote is ${fileContent[stockValueIndex]} per share");
                            }
                        }
                    }
                }
            }
        }
    }
}
