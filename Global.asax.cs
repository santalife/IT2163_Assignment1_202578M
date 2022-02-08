using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace IT2163_Assignment1_202578M
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        //protected void Application_BeginRequest(object sender, EventArgs e)
        //{
        //    HttpContext context = base.Context;
        //    HttpRequest request = context.Request;
        //    string pageName = System.IO.Path.GetFileNameWithoutExtension(request.RawUrl);
        //    if (pageName != "2FAPage")
        //    {
        //        if (context.Session["ToAuthenticate"] == null) { 
                    
        //        }
        //        else
        //        {
        //            context.Session.Remove("ToAuthenticate");
        //        }
        //    }
        //}

    }
}