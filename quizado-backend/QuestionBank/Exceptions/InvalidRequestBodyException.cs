using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuestionBank.Exceptions
{
    public class InvalidRequestBodyException : ApplicationException
    {
        public InvalidRequestBodyException() { }
        public InvalidRequestBodyException(string message) : base(message) { }
    }
}
