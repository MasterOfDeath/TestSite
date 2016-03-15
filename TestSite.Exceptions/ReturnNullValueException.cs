namespace TestSite.Exceptions
{
    using System;

    public class ReturnNullValueException : Exception
    {
        public ReturnNullValueException(string message)
            : base(message)
        {
        }

        public ReturnNullValueException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
