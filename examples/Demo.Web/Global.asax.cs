using Coldairarrow.Util;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Demo.Web
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            new IdHelperBootstrapper()
                    .UseZookeeper("127.0.0.1:2181", 200, "Purchase")
                    .Boot();
        }
    }
}
