using System.Web;
using System.Web.Optimization;

namespace TigerTodoAp
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js",
                      "~/Scripts/ui-bootstrap-tpls.js"));
            bundles.Add(new ScriptBundle("~/bundles/angularMain").Include(                    
                      "~/Scripts/angular.js"));
            bundles.Add(new ScriptBundle("~/bundles/angularModules").Include(
                      "~/Scripts/angular-route.js",
                      "~/Scripts/ng-table.js", 
                      "~/Scripts/restangular.js"));
            bundles.Add(new ScriptBundle("~/bundles/utilityScripts").Include(
                     "~/Scripts/lodash.underscore.js"));
            bundles.Add(new ScriptBundle("~/bundles/app").Include(
                     "~/Scripts/App/app.js").IncludeDirectory("~/Scripts/App/Controllers", "*.js"));
            
            //css bundle
            bundles.Add(new StyleBundle("~/Content/css").Include(
                       "~/Content/normalize.css",
                       "~/Content/bootstrap.css",
                       "~/Content/main.css",
                       "~/Content/ng-table.css",
                       "~/Content/app.css"));

        }
    }
}
