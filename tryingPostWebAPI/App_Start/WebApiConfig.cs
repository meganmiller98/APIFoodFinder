using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using MySql.Data.MySqlClient;

namespace tryingPostWebAPI
{
    public static class WebApiConfig
    {
        public static MySqlConnection conn()
        {
            string conn_string = "server=silva.computing.dundee.ac.uk;port=3306;database=foodfinderdb;username=foodfinder;password=8343.mm.3438";

            MySqlConnection conn = new MySqlConnection(conn_string);

            return conn;
        }


        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );


            var appXnlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXnlType);
        }
    }
}
