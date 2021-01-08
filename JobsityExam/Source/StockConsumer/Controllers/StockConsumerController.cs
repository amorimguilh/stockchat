using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StockConsumer.Exceptions;
using StockConsumer.Integration;
using StockConsumer.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockConsumer.Controllers
{
    /// <summary>
    /// Controller that consumes an external service to retrive stock price
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class StockConsumerController : ControllerBase
    {
        private readonly ILogger<StockConsumerController> _logger;
        private readonly IQueueIntegration _queueIntegration;
        private readonly IStockFileInfoIntegration _stockInfoIntegration;
        private static readonly string closeValueHeaderDescription = "Close";

        public StockConsumerController(
            IQueueIntegration queueIntegration,
            IStockFileInfoIntegration stockInfoIntegration,
            ILogger<StockConsumerController> logger)
        {
            _logger = logger;
            _stockInfoIntegration = stockInfoIntegration;
            _queueIntegration = queueIntegration;
        }

        /// <summary>
        /// Sends a request to an external service to retrieve a csv file with the stock price of 
        /// a desired stock and publishes the stock information in a queue
        /// </summary>
        [HttpPost]
        public async Task RetrieveStockPriceRequest([FromBody] StockConsumerRequest stock)
        {
            var stockName = stock.StockName;
            string queueMessage = string.Empty;

            try
            {
                List<string> fileHeaders = null;
                List<string> fileContent = null;

                (fileHeaders, fileContent) = await _stockInfoIntegration.GetStockInfoFile(stockName);

                var stockValueIndex = GetStockCloseValueIndex(fileHeaders);

                ValidateHeadersAndColumnsQuantity(fileHeaders, fileContent);

                queueMessage = GenerateQueueMessage(stockName, stockValueIndex, fileContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                queueMessage = $"Unable to get {stockName} stock information.";
            }
            finally
            {
                _queueIntegration.PostMessage(queueMessage);
            }
        }

        /// <summary>
        /// Generates a message with the requested stock information
        /// </summary>
        private static string GenerateQueueMessage(string stock, int stockValueIndex, List<string> fileContent)
        {
            var stockValue = -1f;
            if (float.TryParse(fileContent[stockValueIndex], out stockValue))
            {
                return $"{stock} quote is ${stockValue} per share.";
            }

            return $"{stock} was not found. Please check the entire name of the stock, most stock names end with '.US'";
        }

        /// <summary>
        /// Gets the index in the csv file that represents the close value of a stock.
        /// </summary>
        private static int GetStockCloseValueIndex(List<string> fileHeaders)
        {
            var stockValueIndex = fileHeaders.IndexOf(closeValueHeaderDescription);

            ValidateIndex(stockValueIndex);

            return stockValueIndex;
        }

        /// <summary>
        /// Validates the amount of headers and columns in the csv file to guarantee its consistency
        /// </summary>
        private static void ValidateHeadersAndColumnsQuantity(List<string> fileHeaders, List<string> fileContent)
        {
            if (fileHeaders.Count != fileContent.Count)
            {
                throw new HeadersColumnsLengthMismatchException();
            }
        }

        /// <summary>
        /// Validate if a index in the file is valid
        /// </summary>
        private static void ValidateIndex(int stockValueIndex)
        {
            if (stockValueIndex == -1)
            {
                throw new NotValidIndexException();
            }
        }
    }
}