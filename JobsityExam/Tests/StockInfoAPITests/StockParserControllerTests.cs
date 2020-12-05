using JobsityExam.Controllers;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockInfoParserAPI.Integration;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telerik.JustMock;

namespace StockInfoParserAPI.Tests
{
    [TestClass]
    public class StockParserControllerTests
    {
        private readonly StockParserController _controller;
        private readonly IQueueIntegration _queueIntegration;
        private readonly IStockFileInfoIntegration _stockFileInfoIntegration;
        private readonly ILogger<StockParserController> _logger;

        public StockParserControllerTests()
        {
            _stockFileInfoIntegration = Mock.Create<IStockFileInfoIntegration>();
            _queueIntegration = Mock.Create<IQueueIntegration>();
            _logger = Mock.Create<ILogger<StockParserController>>();
            _controller = new StockParserController(_queueIntegration, _stockFileInfoIntegration, _logger);

            // Prevent any posts in RabbitMq
            Mock.Arrange(() => _queueIntegration.PostMessage(Arg.AnyString)).DoInstead(() => { });
        }

        [TestMethod]
        public async Task WhenCallingGetMethod_PassingAValidStockName_ShouldReturnTheExpectedSucessMessage()
        {
            //Arrange
            var stockName = "T.US";
            var stockValue = 29.30f;
            var expectedMessage = $"{stockName} quote is ${stockValue} per share.";
            var expectedTimesCalledWithExpectedMessage = 1;

            var header = new List<string>
            {
                "header0",
                "header1",
                "header2",
                "header3",
                "header4",
                "header5",
                "Close",
            };

            var stockInfo = new List<string>
            {
                "name",
                "non-used",
                "non-used",
                "non-used",
                "non-used",
                "non-used",
                stockValue.ToString(),
            };

            Mock.Arrange(() => _stockFileInfoIntegration.GetStockInfoFile(Arg.AnyString)).TaskResult((header, stockInfo));

            // Call
            await _controller.Get(stockName);
            var timesCalled = Mock.GetTimesCalled(() => _queueIntegration.PostMessage(expectedMessage));

            // Assert
            Assert.AreEqual(expectedTimesCalledWithExpectedMessage, timesCalled);
        }

        [TestMethod]
        public async Task WhenCallingGetMethod_PassingAValidStockName_AndTheOrderOfTheHeaderAndContentChanges_ShouldReturnTheExpectedSucessMessage()
        {
            //Arrange
            var stockName = "T.US";
            var stockValue = 29.30f;
            var expectedMessage = $"{stockName} quote is ${stockValue} per share.";
            var expectedTimesCalledWithExpectedMessage = 1;

            var header = new List<string>
            {
                "header0",
                "header1",
                "header2",
                "Close",
                "header4",
                "header5",
                "header6",
            };

            var stockInfo = new List<string>
            {
                "header0",
                "header1",
                "header2",
                stockValue.ToString(),
                "header4",
                "header5",
                "header6",
            };

            Mock.Arrange(() => _stockFileInfoIntegration.GetStockInfoFile(Arg.AnyString)).TaskResult((header, stockInfo));

            // Call
            await _controller.Get(stockName);
            var timesCalled = Mock.GetTimesCalled(() => _queueIntegration.PostMessage(expectedMessage));

            // Assert
            Assert.AreEqual(expectedTimesCalledWithExpectedMessage, timesCalled);
        }

        [TestMethod]
        public async Task WhenCallingGetMethod_AndSomethingWrongHappens_ShouldPostAnErrorMessage()
        {
            //Arrange
            var stockName = "T.US";
            var stockValue = 29.30f;
            var expectedMessage = $"Unable to get {stockName} stock information.";
            var expectedTimesCalledWithExpectedMessage = 1;

            var header = new List<string>
            {
                "header0",
                "header1",
                "header2",
                "Close",
                "header4",
                "header5",
                "header6",
            };

            var stockInfo = new List<string>
            {
                "header0",
                "header1",
                "header2",
                stockValue.ToString(),
                "header4",
                "header5",
                "header6",
            };

            Mock.Arrange(() => _stockFileInfoIntegration.GetStockInfoFile(Arg.AnyString)).Throws(new System.Exception());

            // Call
            await _controller.Get(stockName);
            var timesCalled = Mock.GetTimesCalled(() => _queueIntegration.PostMessage(expectedMessage));

            // Assert
            Assert.AreEqual(expectedTimesCalledWithExpectedMessage, timesCalled);
        }

        [TestMethod]
        public async Task WhenCallingGetMethod_AndTheValueOfTheStockCannotBeFound_ShouldPostAnErrorMessage()
        {
            //Arrange
            var stockName = "T.US";
            var stockValue = 29.30f;
            var expectedMessage = $"Unable to get {stockName} stock information.";
            var expectedTimesCalledWithExpectedMessage = 1;

            var header = new List<string>
            {
                "header0",
                "header1",
                "header2",
                "header3",
                "header4",
                "header5",
                "header6",
            };

            var stockInfo = new List<string>
            {
                "header0",
                "header1",
                "header2",
                stockValue.ToString(),
                "header4",
                "header5",
                "header6",
            };

            Mock.Arrange(() => _stockFileInfoIntegration.GetStockInfoFile(Arg.AnyString)).Throws(new System.Exception());

            // Call
            await _controller.Get(stockName);
            var timesCalled = Mock.GetTimesCalled(() => _queueIntegration.PostMessage(expectedMessage));

            // Assert
            Assert.AreEqual(expectedTimesCalledWithExpectedMessage, timesCalled);
        }

        [TestMethod]
        public async Task WhenCallingGetMethod_AndTheAmountOfHeadersDiffersFromTheAmountOfValues_ShouldPostAnErrorMessage()
        {
            //Arrange
            var stockName = "T.US";
            var stockValue = 29.30f;
            var expectedMessage = $"Unable to get {stockName} stock information.";
            var expectedTimesCalledWithExpectedMessage = 1;

            var header = new List<string>
            {
                "header1",
                "header2",
                "Close",
                "header4",
                "header5",
                "header6",
            };

            var stockInfo = new List<string>
            {
                "header0",
                "header1",
                "header2",
                stockValue.ToString(),
                "header4",
                "header5",
                "header6",
            };

            Mock.Arrange(() => _stockFileInfoIntegration.GetStockInfoFile(Arg.AnyString)).Throws(new System.Exception());

            // Call
            await _controller.Get(stockName);
            var timesCalled = Mock.GetTimesCalled(() => _queueIntegration.PostMessage(expectedMessage));

            // Assert
            Assert.AreEqual(expectedTimesCalledWithExpectedMessage, timesCalled);
        }

        [TestMethod]
        public async Task WhenCallingGetMethod_AndTheSystemCannotParseTheStockValue_ShouldPostAnErrorMessage()
        {
            //Arrange
            var stockName = "T.US";
            var stockValue = "N/D";
            var expectedMessage = $"{stockName} was not found. Please check the entire name of the stock, most stock names end with '.US'";
            var expectedTimesCalledWithExpectedMessage = 1;

            var header = new List<string>
            {
                "header0",
                "header1",
                "header2",
                "Close",
                "header4",
                "header5",
                "header6",
            };

            var stockInfo = new List<string>
            {
                "header0",
                "header1",
                "header2",
                stockValue,
                "header4",
                "header5",
                "header6",
            };

            Mock.Arrange(() => _stockFileInfoIntegration.GetStockInfoFile(Arg.AnyString)).TaskResult((header, stockInfo));

            // Call
            await _controller.Get(stockName);
            var timesCalled = Mock.GetTimesCalled(() => _queueIntegration.PostMessage(expectedMessage));

            // Assert
            Assert.AreEqual(expectedTimesCalledWithExpectedMessage, timesCalled);
        }
    }
}