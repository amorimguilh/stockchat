using ChatApplication.Hubs;
using ChatApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace ChatApplication.Controllers
{
    [ApiController]
    [Route("api/chat")]
    public class ChatController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly ILogger<ChatController> _logger;

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
            return Ok();
        }
    }
}
