using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuestionBank.Exceptions
{
    public class DifficultyNotFoundException :  ApplicationException
    {
        public DifficultyNotFoundException() { }
        public DifficultyNotFoundException(string message) : base(message) { }
    }
}
