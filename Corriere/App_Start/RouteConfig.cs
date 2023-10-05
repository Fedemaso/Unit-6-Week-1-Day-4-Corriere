using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Corriere
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Home",
                url: "Home/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "AggiornaSpedizione",
                url: "Spedizioni/AggiornaSpedizione/{id}",
                defaults: new { controller = "Spedizioni", action = "AggiornaSpedizione", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Spedizioni",
                url: "Spedizioni/{action}/{id}",
                defaults: new { controller = "Spedizioni", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Clienti",
                url: "Clienti/{action}/{id}",
                defaults: new { controller = "Clienti", action = "Index", id = UrlParameter.Optional }
            );


            routes.MapRoute(
                name: "SpedizioniInAttesa",
                url: "Spedizioni/SpedizioniInAttesa",
                defaults: new { controller = "Spedizioni", action = "SpedizioniInAttesaView", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "SpedizioniPerCitta",
                url: "SpedizioniPerCittaView",
                defaults: new { controller = "Spedizioni", action = "SpedizioniPerCittaView", id = UrlParameter.Optional }
            );


            routes.MapRoute(
                name: "UserRegistration",
                url: "Registration/UserRegistration",
                defaults: new { controller = "Registration", action = "UserRegistration", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Login",
                url: "Registration/Login",
                defaults: new { controller = "Registration", action = "Login", id = UrlParameter.Optional }
            );


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}

