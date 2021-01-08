using System.Collections.Generic;

namespace ChatApplication.Configurations
{
    /// <summary>
    /// Implements the IChatConfiguration interface 
    /// and sets the basic configuration in order to run the projet
    /// </summary>
    public class ChatConfiguration : IChatConfiguration
    {
        private readonly static HashSet<string> _allowedCommands = new HashSet<string> { "/STOCK" };
        private readonly static string _get_stock_quote_queue = "get_stock_quote_queue";
        private readonly static string _broadCastMessageMethod = "ReceiveOne";

        /// <summary>
        /// Represents the allowed commands the user can send in the chat
        /// </summary>
        public HashSet<string> AllowedCommands 
        { 
            get
            {
                return _allowedCommands;
            }
        }

        /// <summary>
        /// Represents the queue name where the user command messages should be published
        /// </summary>
        public string QueueName 
        {
            get
            {
                return _get_stock_quote_queue;
            }
        }

        /// <summary>
        /// Represents the method name that should be called to broadcast the chat messages
        /// </summary>
        public string BroadCastMethodName
        {
            get
            {
                return _broadCastMessageMethod;
            }
        }
    }
}
