using System.Web;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using WeddingAPI.Utils;

namespace WeddingAPI
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configuration.Services.Add(typeof(IExceptionLogger), new TraceExceptionLogger());
        }
    }
}
