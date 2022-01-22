using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace QuestionBank.Aspect
{
    public class LoggerAspect : ActionFilterAttribute
    {
        static Stopwatch stopWatch;
        string logFilePath;
        static string time;

        public LoggerAspect(IHostingEnvironment environment)
        {
            stopWatch = new Stopwatch();
            logFilePath = environment.ContentRootPath + @"/LogFile.txt";
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            stopWatch.Start();
            time = string.Format("{0:HH:mm:ss tt}", DateTime.Now);
            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            Log(context.RouteData);
            base.OnActionExecuted(context);
        }

        private void Log(RouteData routeData)
        {
            stopWatch.Stop();
            String ts = stopWatch.Elapsed.ToString();
            var controllerName = routeData.Values["controller"];
            var actionName = routeData.Values["action"];
            string message = "Controller:" + controllerName + " Action:" + actionName + "Request Time: " + time + " Request Processing Time: " + ts + Environment.NewLine;

            using (FileStream fs = File.Open(logFilePath, FileMode.Append, FileAccess.Write))//logFilePath, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(message);
                }
            }

        }
    }
}
