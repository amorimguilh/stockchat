namespace StockInfoParserAPI.Integration
{
    public interface IQueueIntegration
    {
        void PostMessage(string message);
    }
}
