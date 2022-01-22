using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authentication.Exceptions
{
    public class UserNotVerifiedException : ApplicationException
    {
        public UserNotVerifiedException() { }
        public UserNotVerifiedException(string message) : base(message) { }
    }
}
