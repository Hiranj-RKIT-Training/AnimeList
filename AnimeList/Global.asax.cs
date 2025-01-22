using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace AnimeList
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
            var dbFactory = new OrmLiteConnectionFactory(connectionString, MySqlDialect.Provider);

            Application["DbFactory"] = dbFactory;
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
