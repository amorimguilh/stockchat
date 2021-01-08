namespace ChatApplication.Integration
{
    /// <summary>
    /// Interface responsible to expose the contrat to all queue integration implementations
    /// such as Kafka and RabbitMq
    /// </summary>
    public interface IQueueIntegration
    {
        /// <summary>
        /// Publishes a message in a queue
        /// </summary>
        void PublishMessage(string message);
    }
}
