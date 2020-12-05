using System;

namespace StockInfoParserAPI.Exceptions
{
    public class NotValidIndexException : Exception
    {
        public NotValidIndexException(string message = "Invalid indexes on csv file") : base(message)
        {
        }
    }
}
