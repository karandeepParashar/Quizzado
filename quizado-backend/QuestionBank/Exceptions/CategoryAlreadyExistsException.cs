using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuestionBank.Exceptions
{
    public class CategoryAlreadyExistsException : ApplicationException
    {
        public CategoryAlreadyExistsException() { }
        public CategoryAlreadyExistsException(string message) : base(message) { }
    }
}
