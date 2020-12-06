using System;

namespace StockConsumer.Exceptions
{
    public class NotValidIndexException : Exception
    {
        public NotValidIndexException(string message = "Invalid indexes on csv file") : base(message)
        {
        }
    }
}
