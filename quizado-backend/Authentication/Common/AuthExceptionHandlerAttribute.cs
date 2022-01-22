using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Authentication.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authentication.Common
{
    public class AuthExceptionHandlerAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var exceptionType = context.Exception.GetType();
            var message = context.Exception.Message;
            if (exceptionType == typeof(UserNotFoundException))
            {
                context.Result = new NotFoundObjectResult(message);
            }
            else if (exceptionType == typeof(ReferralNotFoundException))
            {
                context.Result = new NotFoundObjectResult(message);
            }
            else if (exceptionType == typeof(UserNotVerifiedException))
            {
                context.Result = new UnauthorizedObjectResult(message);
            }
            else if (exceptionType == typeof(UserAlreadyExistsException))
            {
                context.Result = new ConflictObjectResult(message);
            }
            else if (exceptionType == typeof(OtpIncorrectException))
            {
                context.Result = new ConflictObjectResult(message);
            }
            else if (exceptionType == typeof(Exception))
            {
                context.Result = new ConflictObjectResult(message);
            }
            else
            {
                context.Result = new StatusCodeResult(500);
            }
        }
    }
}
