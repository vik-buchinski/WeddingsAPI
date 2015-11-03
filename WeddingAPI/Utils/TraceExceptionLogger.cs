using System;
using System.Diagnostics;
using System.Web.Http.ExceptionHandling;

namespace WeddingAPI.Utils
{
    public class TraceExceptionLogger : ExceptionLogger
    {
        public override void Log(ExceptionLoggerContext context)
        {
            Trace.TraceError(context.ExceptionContext.Exception.ToString());
        }
        public static void LogException(Exception e)
        {
            Trace.TraceError(e.ToString());
        }
    }
}