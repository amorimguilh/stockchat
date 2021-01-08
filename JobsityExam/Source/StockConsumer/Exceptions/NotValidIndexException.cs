using System;

namespace StockConsumer.Exceptions
{
    public class NotValidIndexException : Exception
    {
        public NotValidIndexException(string message = "Invalid index on csv file") : base(message)
        {
        }
    }
}
