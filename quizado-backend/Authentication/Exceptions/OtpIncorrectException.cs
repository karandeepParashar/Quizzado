using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authentication.Exceptions
{
    public class OtpIncorrectException : ApplicationException
    {
        public OtpIncorrectException() { }
        public OtpIncorrectException(string message) : base(message) { }
    }
}
