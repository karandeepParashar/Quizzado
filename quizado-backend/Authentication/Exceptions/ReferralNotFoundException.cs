using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authentication.Exceptions
{
    public class ReferralNotFoundException : ApplicationException
    {
        public ReferralNotFoundException() { }
        public ReferralNotFoundException(string message) : base(message) { }
    }
}
