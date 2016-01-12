using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace PloomesCRMAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "ActionApiId",
                routeTemplate: "{controller}/{id}/{action}",
                defaults: new { },
                constraints: new { action = @"((?!(Get|Post|Put|Delete|\d+)).)*", id = @"\d+" }
            );

            config.Routes.MapHttpRoute(
                name: "ActionApi",
                routeTemplate: "{controller}/{action}",
                defaults: new { },
                constraints: new { action = @"((?!(Get|Post|Put|Delete|\d+)).)*" }
            );

            config.Routes.MapHttpRoute(
                name: "SubresourceApi",
                routeTemplate: "{controller}/{action}/{id}",
                defaults: new { },
                constraints: new { action = @"((?!(Get|Post|Put|Delete|\d+)).)*", id = @"\d+" }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApiId",
                routeTemplate: "{controller}/{id}",
                defaults: new { action = "" },
                constraints: new { id = @"\d+" }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}",
                defaults: new { action = "" }
            );

            config.Formatters.JsonFormatter.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Local;
        }
    }
}
