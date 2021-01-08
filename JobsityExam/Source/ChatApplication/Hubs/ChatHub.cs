using ChatApplication.Configurations;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ChatApplication.Hubs
{
    /// <summary>
    /// hub to broadcast messages to all chat clients
    /// </summary>
    public class ChatHub : Hub
    {
        private readonly string _broadCastMessageMethodName;
        public ChatHub(IChatConfiguration configuration)
        {
            _broadCastMessageMethodName = configuration.BroadCastMethodName;
        }

        /// <summary>
        /// Broadcast a message to all chat clients
        /// </summary>
        public Task SendMessage(string user, string message)
        {
            return Clients.All.SendAsync(_broadCastMessageMethodName, user, message);
        }
    }
}