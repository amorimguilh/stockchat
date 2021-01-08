namespace ChatApplication.Models
{
    /// <summary>
    /// Represents a chat message in the application
    /// </summary>
    public class MessageRequest
    {
        /// <summary>
        /// Username of the sender
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// The message the user sent
        /// </summary>
        public string Message { get; set; }
    }
}
