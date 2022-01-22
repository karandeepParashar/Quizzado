using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuestionBank.Exceptions
{
    public class QuestionAlreadyExistsException : ApplicationException
    {
        public QuestionAlreadyExistsException () { }
        public QuestionAlreadyExistsException(string message) : base(message) { }

    }
}
