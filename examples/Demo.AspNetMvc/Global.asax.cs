using Coldairarrow.Util;
using System.Web.Mvc;
using System.Web.Routing;

namespace Demo.AspNetMvc
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            new IdHelperBootstrapper()
                .UseZookeeper("127.0.0.1:2181", 5000, "Test")
                .Boot();
        }
    }
}
