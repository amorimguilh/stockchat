using ChatApplication.Configurations;
using ChatApplication.Controllers;
using ChatApplication.Hubs;
using ChatApplication.Integration;
using ChatApplication.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;

namespace ChatAppTests
{
    [TestClass]
    public class ChatControllerTests
    {
        private readonly ChatController _controller;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly ILogger<ChatController> _logger;
        private readonly IQueueIntegration _queueIntegration;
        private readonly IChatConfiguration _configuration;

        public ChatControllerTests()
        {
            _hubContext = Mock.Create<IHubContext<ChatHub>>();
            _logger = Mock.Create<ILogger<ChatController>>();
            _queueIntegration = Mock.Create<IQueueIntegration>();
            _configuration = Mock.Create<IChatConfiguration>();

            Mock.Arrange(() => _configuration.AllowedCommands).Returns(
                new System.Collections.Generic.HashSet<string>
                {
                    "/STOCK"
                });

            _controller = new ChatController(_hubContext, _queueIntegration, _configuration, _logger);
        }

        [TestMethod]
        public void OnSendMessage_WhenIsACommonMessage_ShouldNotCallQueueIntegration()
        {
            //Assert
            var expectedNumberOfTimesQueueIntegrationWasCalled = 0;
            var request = new MessageRequest
            {
                User = "test",
                Message = "A message"
            };

            //Call 
            _controller.SendRequest(request);
            var numberOfTimesQueueIntegrationWasCalled = Mock.GetTimesCalled(() => _queueIntegration.PublishMessage(Arg.AnyString));

            Assert.AreEqual(expectedNumberOfTimesQueueIntegrationWasCalled, numberOfTimesQueueIntegrationWasCalled);
        }

        [TestMethod]
        public void OnSendMessage_WhenIsValidCommandMessage_ShouldCallQueueIntegration()
        {
            //Assert
            var expectedNumberOfTimesQueueIntegrationWasCalled = 1;
            var request = new MessageRequest
            {
                User = "test",
                Message = "/stock=t.us"
            };

            //Call 
            _controller.SendRequest(request);
            var numberOfTimesQueueIntegrationWasCalled = Mock.GetTimesCalled(() => _queueIntegration.PublishMessage(Arg.AnyString));

            Assert.AreEqual(expectedNumberOfTimesQueueIntegrationWasCalled, numberOfTimesQueueIntegrationWasCalled);
        }

        [TestMethod]
        public void OnSendMessage_WhenCommandLacksTheEqualsSign_ShouldNotCallQueueIntegration()
        {
            //Assert
            var expectedNumberOfTimesQueueIntegrationWasCalled = 0;
            var request = new MessageRequest
            {
                User = "test",
                Message = "/stockt.us"
            };

            //Call 
            _controller.SendRequest(request);
            var numberOfTimesQueueIntegrationWasCalled = Mock.GetTimesCalled(() => _queueIntegration.PublishMessage(Arg.AnyString));

            Assert.AreEqual(expectedNumberOfTimesQueueIntegrationWasCalled, numberOfTimesQueueIntegrationWasCalled);
        }

        [TestMethod]
        public void OnSendMessage_WhenCommandLacksValue_ShouldNotCallQueueIntegration()
        {
            //Assert
            var expectedNumberOfTimesQueueIntegrationWasCalled = 0;
            var request = new MessageRequest
            {
                User = "test",
                Message = "/stock="
            };

            //Call 
            _controller.SendRequest(request);
            var numberOfTimesQueueIntegrationWasCalled = Mock.GetTimesCalled(() => _queueIntegration.PublishMessage(Arg.AnyString));

            Assert.AreEqual(expectedNumberOfTimesQueueIntegrationWasCalled, numberOfTimesQueueIntegrationWasCalled);
        }

        [TestMethod]
        public void OnSendMessage_WhenCommandIsInvalid_ShouldNotCallQueueIntegration()
        {
            //Assert
            var expectedNumberOfTimesQueueIntegrationWasCalled = 0;
            var request = new MessageRequest
            {
                User = "test",
                Message = "/acommand=somevalue"
            };

            //Call 
            _controller.SendRequest(request);
            var numberOfTimesQueueIntegrationWasCalled = Mock.GetTimesCalled(() => _queueIntegration.PublishMessage(Arg.AnyString));

            Assert.AreEqual(expectedNumberOfTimesQueueIntegrationWasCalled, numberOfTimesQueueIntegrationWasCalled);
        }
    }
}
