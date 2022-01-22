using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaderBoard.Exceptions
{
    public class UserNotFoundException : ApplicationException
    {
        public UserNotFoundException(){}
        public UserNotFoundException(string message) : base(message){}
    }
}
