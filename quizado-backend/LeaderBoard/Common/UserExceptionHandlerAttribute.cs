using LeaderBoard.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaderBoard.Common
{
    public class UserExceptionHandlerAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var exceptionType = context.Exception.GetType();
            var message = context.Exception.Message;
            if (exceptionType == typeof(CategoryNotFoundException) || exceptionType == typeof(UserNotFoundException))
            {
                context.Result = new NotFoundObjectResult(message);
            }
            else if (exceptionType == typeof(CategoryAlreadyExistsException) || exceptionType == typeof(UserAlreadyExistsException))
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
