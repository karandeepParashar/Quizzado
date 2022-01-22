using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuestionBank.Exceptions
{
    public class QuestionNotAttempedException : ApplicationException
    {
        public QuestionNotAttempedException() { }
        public QuestionNotAttempedException(string message) : base(message) { }
    }
}
