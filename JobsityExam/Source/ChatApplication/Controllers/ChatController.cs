using ChatApplication.Hubs;
using ChatApplication.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace ChatApplication.Controllers
{
    [ApiController]
    [Route("api/chat")]
    public class ChatController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly ILogger<ChatController> _logger;
        private static HashSet<string> _availableCommands = new HashSet<string> { "/STOCK" };

        public ChatController(IHubContext<ChatHub> hubContext, ILogger<ChatController> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        [Route("send")]
        [HttpPost]
        public IActionResult SendRequest([FromBody] MessageRequest chatMessage)
        {
            _hubContext.Clients.All.SendAsync("ReceiveOne", chatMessage.User, chatMessage.Message);

            if (chatMessage.Message.StartsWith('/') && chatMessage.Message.Contains('='))
            {
                var commandParts = chatMessage.Message.Split('=');
                if(_availableCommands.Contains(commandParts[0].ToUpper()))
                {
                    // valid command, check the second position of the array.
                    if(string.IsNullOrWhiteSpace(commandParts[1]))
                    {
                        // send a message informing the user how to use the command.
                    }
                }
                else
                {
                    // invalid command
                }
            }

            
            return Ok();
        }
    }
}
