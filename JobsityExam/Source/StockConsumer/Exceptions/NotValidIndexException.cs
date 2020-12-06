using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockConsumer.Exceptions
{
    public class NotValidIndexException : Exception
    {
        public NotValidIndexException(string message = "Invalid indexes on csv file") : base(message)
        {
        }
    }

    public class HeadersColumnsLengthMismatchException : Exception
    {
        public HeadersColumnsLengthMismatchException(string message = "The amount of headers does not match the number of columns") : base(message)
        {
        }
    }
}
