using ChatApplication.Configurations;
using ChatApplication.Exceptions;
using ChatApplication.Hubs;
using ChatApplication.Integration;
using ChatApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace ChatApplication.Controllers
{
    [ApiController]
    [Route("api/chat")]
    public class ChatController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly ILogger<ChatController> _logger;
        private readonly IQueueIntegration _queueIntegration;
        private readonly IChatConfiguration _configuration;

        public ChatController(
            IHubContext<ChatHub> hubContext,
            IQueueIntegration queueIntegration,
            IChatConfiguration configuration,
            ILogger<ChatController> logger)
        {
            _queueIntegration = queueIntegration;
            _configuration = configuration;
            _hubContext = hubContext;
            _logger = logger;
        }

        [Route("send")]
        [HttpPost]
        public IActionResult SendRequest([FromBody] MessageRequest chatMessage)
        {
            try
            {
                _hubContext.Clients.All.SendAsync("ReceiveOne", chatMessage.User, chatMessage.Message);

                if (chatMessage.Message.StartsWith('/'))
                {
                    ValidateCommandValueAttribuition(chatMessage);
                    
                    var commandParts = chatMessage.Message.Split('=');
                    if (IsValidCommandPattern(commandParts, _configuration))
                    {
                        _queueIntegration.PublishMessage(chatMessage.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error - User:{chatMessage.User} Message: {chatMessage.Message}");
                _hubContext.Clients.All.SendAsync("ReceiveOne", "bot", ex.Message);
            }

            return Accepted();
        }

        private static bool IsValidCommandPattern(string[] commandParts, IChatConfiguration configuration)
        {
            if (configuration.AllowedCommands.Contains(commandParts[0].ToUpper()))
            {
                // valid command, check the second position of the array.
                if (string.IsNullOrWhiteSpace(commandParts[1]))
                {
                    throw new InvalidCommandException("A command (message starting with '/') requires a value to it. Please follow the pattern '/<command>=<value>'");
                }

                return true;
            }
            else
            {
                throw new InvalidCommandException($"The command{commandParts[0]} is not recognized by the system");
            }
        }

        private static void ValidateCommandValueAttribuition(MessageRequest chatMessage)
        {
            if (!chatMessage.Message.Contains('='))
            {
                throw new InvalidCommandException("Missing the value of the command. Values are setted using the '=' sign. Following the pattern: '/<command>=<value>'");
            }
        }
    }
}
      