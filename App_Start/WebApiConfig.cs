using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Web.Http.Cors;

namespace SwiftEcom
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.EnableCors();

            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Web API routes
            var secretKey = System.Configuration.ConfigurationManager.AppSettings["JwtSecretKey"];
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ValidateIssuer = true,
                ValidIssuer = "SwiftEcom",
                ValidateAudience = true,
                ValidAudience = "SwiftEcom",
                ValidateLifetime = true
            };

            config.Filters.Add(new JwtAuthenticationFilter(tokenValidationParameters));
        }
    }
}
