using System;

namespace ChatApplication.Exceptions
{
    public class InvalidCommandException : Exception
    {
        public InvalidCommandException(string errorMessage) : base(errorMessage)
        {

        }
    }
}
