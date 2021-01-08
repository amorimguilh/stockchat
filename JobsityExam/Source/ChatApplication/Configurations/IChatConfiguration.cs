using System.Collections.Generic;

namespace ChatApplication.Configurations
{   
    /// <summary>
    /// Contract to expose the chat api configuration
    /// </summary>
    public interface IChatConfiguration
    {
        /// <summary>
        /// Represents the allowed commands the user can send in the chat
        /// </summary>
        HashSet<string> AllowedCommands { get; }
        
        /// <summary>
        /// Represents the queue name where the user command messages should be published
        /// </summary>
        string QueueName { get; } 
        
        /// <summary>
        /// Represents the method name that should be called to broadcast the chat messages
        /// </summary>
        string BroadCastMethodName { get; }
    }
}
