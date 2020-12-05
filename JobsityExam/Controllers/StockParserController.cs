using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StockInfoParserAPI.Exceptions;
using StockInfoParserAPI.Integration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JobsityExam.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockParserController : ControllerBase
    {
        private readonly ILogger<StockParserController> _logger;
        private readonly IQueueIntegration _queueIntegration;
        private static readonly string url = "https://stooq.com/q/l/?s={0}&f=sd2t2ohlcv&h&e=csv";
        private static readonly char splitCharacter = ',';
        private static readonly string closeValueHeaderDescription = "Close";

        public StockParserController(
            IQueueIntegration queueIntegration,
            ILogger<StockParserController> logger)
        {
            _logger = logger;
            _queueIntegration = queueIntegration;
        }

        [HttpGet("{stock:minlength(1)}")]
        public async Task Get(string stock)
        {
            string message = string.Empty;
            
            try
            {
                using (var client = new HttpClient())
                using (var response = await client.GetAsync(string.Format(url, stock)))
                using (var fileStream = await response.Content.ReadAsStreamAsync())
                using (var reader = new StreamReader(fileStream))
                {
                    if (!reader.EndOfStream)
                    {
                        var fileHeaders = reader.ReadLine().Split(splitCharacter).ToList();

                        var stockValueIndex = GetStockNameAndValueIndexes(fileHeaders);

                        var fileContent = reader.ReadLine().Split(splitCharacter);

                        ValidateHeadersAndColumnsQuantity(fileHeaders, fileContent);

                        message = GenerateMessage(stock, stockValueIndex, fileContent);
                    }
                }
            }
            catch(Exception)
            {
                message = $"Unable to get {stock} stock information."; 
            }
            finally
            {
                _queueIntegration.PostMessage(message);
            }
        }

        private static string GenerateMessage(string stock, int stockValueIndex, string[] fileContent)
        {
            var stockValue = -1f;
            if (float.TryParse(fileContent[stockValueIndex], out stockValue))
            {
                return $"{stock} quote is ${stockValue} per share.";
            }

            return $"{stock} was not found. Please check the entire name of the stock, most stock names end with '.US'";
        }

        private static int GetStockNameAndValueIndexes(List<string> fileHeaders)
        {
            var stockValueIndex = fileHeaders.IndexOf(closeValueHeaderDescription);

            ValidateIndexes(stockValueIndex);

            return stockValueIndex;
        }

        private static void ValidateHeadersAndColumnsQuantity(List<string> fileHeaders, string[] fileContent)
        {
            if (fileHeaders.Count != fileContent.Length)
            {
                throw new HeadersColumnsLengthMismatchException();
            }
        }

        private static void ValidateIndexes(int stockValueIndex)
        {
            if (stockValueIndex == -1)
            {
                throw new NotValidIndexException();
            }
        }
    }
}
