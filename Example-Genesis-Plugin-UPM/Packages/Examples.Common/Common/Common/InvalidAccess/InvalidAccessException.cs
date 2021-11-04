using System;

namespace Examples.Common
{
    public class InvalidAccessException : InvalidCastException
    {
        public InvalidAccessException()
            : base()
        { }

        public InvalidAccessException(string message)
            : base(message)
        { }

        public InvalidAccessException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}