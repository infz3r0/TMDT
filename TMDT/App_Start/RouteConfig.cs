using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TMDT
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Detail San pham",
                url: "r/{loaiAlias}/{nsxAlias}/{spAlias}",
                defaults: new { controller = "KhongTen", action = "Details" }
            );

            routes.MapRoute(
                name: "List San pham by NSX",
                url: "r/{loaiAlias}/{nsxAlias}",
                defaults: new { controller = "KhongTen", action = "SanPhamTheoNSX" }
            );

            routes.MapRoute(
                name: "List NSX by Loai",
                url: "r/{loaiAlias}",
                defaults: new { controller = "KhongTen", action = "NSXtheoLoai" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "KhongTen", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
