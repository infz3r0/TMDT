using System.Web;
using System.Web.Optimization;

namespace NPL
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-3.3.1.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap-confirmation").Include(
                        "~/Scripts/bootstrap-confirmation.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/popper").Include(
                        "~/Scripts/umd/popper.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/chartjs").Include(
                        "~/Scripts/Chart.js"));
            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/otherscript").Include(
                      "~/Scripts/shared/superfish.js",
                      "~/Scripts/shared/jquery.scrolltotop.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/Content/style.css",
                      "~/Content/etalage.css",
                      "~/Content/style_user.css",
                      "~/Content/PagedList.css",
                      "~/Content/styleAdmin.css",
                      "~/Content/shared/bootstrap-responsive.min.css",
                      "~/Content/shared/bootstrappage.css",
                      "~/Content/shared/main.css",
                      "~/Content/shared/flexslider.css"));

            bundles.Add(new StyleBundle("~/Content/admincss").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/PagedList.css",
                      "~/Content/styleAdmin.css"));
        }
    }
}
