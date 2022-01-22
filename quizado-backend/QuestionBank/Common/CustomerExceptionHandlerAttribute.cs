using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using QuestionBank.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuestionBank.Common
{
    public class CustomerExceptionHandlerAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var exceptionType = context.Exception.GetType();
            var message = context.Exception.Message;
            if (exceptionType == typeof(CategoryNotFoundException) || exceptionType == typeof(QuestionNotFoundException) || exceptionType == typeof(DifficultyNotFoundException))
            {
                context.Result = new NotFoundObjectResult(message);
            }
            else if(exceptionType == typeof(InvalidRequestBodyException) || exceptionType == typeof(QuestionNotAttempedException))
            {
                context.Result = new BadRequestObjectResult(message);
            }
            else if (exceptionType == typeof(CategoryAlreadyExistsException) || exceptionType == typeof(QuestionAlreadyExistsException))
            {
                context.Result = new ConflictObjectResult(message);
            }
            //else if(exceptionType == typeof(Exception))
            //{
            //    context.Result = new ConflictObjectResult(message);
            //}
            else
            {
                context.Result = new StatusCodeResult(500);
            }
        }
    }
}
