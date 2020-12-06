namespace StockConsumer.Integration
{
    public interface IQueueIntegration
    {
        void PostMessage(string message);
    }
}
