using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuestionBank.Exceptions
{
    public class QuestionNotFoundException: ApplicationException
    {
        public QuestionNotFoundException() { }
        public QuestionNotFoundException(string message) : base(message) { }
    }
}
