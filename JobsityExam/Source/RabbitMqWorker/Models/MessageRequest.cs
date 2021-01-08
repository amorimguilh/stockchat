namespace RabbitMqWorker.Models
{
    /// <summary>
    /// Posts a given message in the chat application
    /// </summary>
    public class MessageRequest
    {
        public string User { get; set; }
        public string Message { get; set; }
    }
}
