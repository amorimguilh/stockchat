using System;

namespace ChatApplication.Exceptions
{
    public class CommandParameterNotFoundException : Exception
    {
        public CommandParameterNotFoundException(string errorMessage) : base(errorMessage)
        {

        }
    }
}
