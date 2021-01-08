using ChatApplication.Configurations;
using ChatApplication.Exceptions;
using ChatApplication.Hubs;
using ChatApplication.Integration;
using ChatApplication.Models;
using Microsoft.AspNetCore.Cors;
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
        private readonly HashSet<string> _allowedCommands;
        private readonly string _broadCastMessageMethodName;

        public ChatController(
            IHubContext<ChatHub> hubContext,
            IQueueIntegration queueIntegration,
            IChatConfiguration configuration,
            ILogger<ChatController> logger)
        {
            _queueIntegration = queueIntegration;
            _allowedCommands = configuration.AllowedCommands;
            _broadCastMessageMethodName = configuration.BroadCastMethodName;
            _hubContext = hubContext;
            _logger = logger;
        }

        /// <summary>
        /// Receives the http request with the message content and redirects
        /// it to the signal R hub to be broadcasted to all subscribers
        /// </summary>
        [Route("send")]
        [HttpPost]
        public IActionResult SendRequest([FromBody] MessageRequest chatMessage)
        {
            try
            {
                _hubContext.Clients.All.SendAsync(_broadCastMessageMethodName, chatMessage.User, chatMessage.Message);
                
                HandleCommandMessage(chatMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error - User:{chatMessage.User} Message: {chatMessage.Message}");
                _hubContext.Clients.All.SendAsync(_broadCastMessageMethodName, "bot", ex.Message);
            }

            return Accepted();
        }

        /// <summary>
        /// Handles the commands the users post in the chat
        /// </summary>
        private void HandleCommandMessage(MessageRequest chatMessage)
        {
            if (chatMessage.Message.StartsWith('/'))
            {
                ValidateCommandValueAttribuition(chatMessage);

                var commandParts = chatMessage.Message.Split('=');
                if (IsValidCommand(commandParts))
                {
                    _queueIntegration.PublishMessage(commandParts[1]);
                }
            }
        }

        /// <summary>
        /// Verifies if the command sent by the user is a system known command 
        /// If not, throws an exception
        /// </summary>
        private bool IsValidCommand(string[] commandParts)
        {
            if (_allowedCommands.Contains(commandParts[0].ToUpper()))
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

        /// <summary>
        /// Validates if a given command passed by the user 
        /// contains the attribution sign.
        /// </summary>
        private void ValidateCommandValueAttribuition(MessageRequest chatMessage)
        {
            if (!chatMessage.Message.Contains('='))
            {
                throw new InvalidCommandException("Missing the attribution sign. Values are setted using the '=' sign following the pattern: '/<command>=<value>'");
            }
        }
    }
}      