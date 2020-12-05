using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StockInfoParserAPI.Exceptions;
using StockInfoParserAPI.Integration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JobsityExam.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockParserController : ControllerBase
    {
        private readonly ILogger<StockParserController> _logger;
        private readonly IQueueIntegration _queueIntegration;
        private readonly IStockFileInfoIntegration _stockInfoIntegration;

        private static readonly string closeValueHeaderDescription = "Close";

        public StockParserController(
            IQueueIntegration queueIntegration,
            IStockFileInfoIntegration stockInfoIntegration,
            ILogger<StockParserController> logger)
        {
            _logger = logger;
            _stockInfoIntegration = stockInfoIntegration;
            _queueIntegration = queueIntegration;
        }

        [HttpGet("{stock:minlength(1)}")]
        public async Task Get(string stock)
        {
            string message = string.Empty;
            
            try
            {
                List<string> fileHeaders = null;
                List<string> fileContent = null;

                (fileHeaders, fileContent) = await _stockInfoIntegration.GetStockInfoFile(stock);

                var stockValueIndex = GetStockNameAndValueIndexes(fileHeaders);

                ValidateHeadersAndColumnsQuantity(fileHeaders, fileContent);

                message = GenerateMessage(stock, stockValueIndex, fileContent);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                message = $"Unable to get {stock} stock information."; 
            }
            finally
            {
                _queueIntegration.PostMessage(message);
            }
        }

        private static string GenerateMessage(string stock, int stockValueIndex, List<string> fileContent)
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

        private static void ValidateHeadersAndColumnsQuantity(List<string> fileHeaders, List<string> fileContent)
        {
            if (fileHeaders.Count != fileContent.Count)
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
