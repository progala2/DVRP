using System;

namespace _15pl04.Ucc.Commons.Exceptions
{
    public class RegisterException : Exception
    {
        public RegisterException()
            : base()
        { }

        public RegisterException(string message)
            : base(message)
        { }

        public RegisterException(string message, Exception inner)
            : base(message, inner)
        { }
    }
}
