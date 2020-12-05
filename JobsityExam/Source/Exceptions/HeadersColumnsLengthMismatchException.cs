using System;

namespace StockInfoParserAPI.Exceptions
{
    public class HeadersColumnsLengthMismatchException : Exception
    {
        public HeadersColumnsLengthMismatchException(string message = "The amount of headers does not match the number of columns") : base(message)
        {
        }
    }
}
