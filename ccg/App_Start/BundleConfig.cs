﻿using System.Web;
using System.Web.Optimization;

namespace ccg
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/fa/font-awesome.min.css",
                      "~/Content/site.css",
                      "~/Content/slick.css",
                      "~/Content/slick-theme.css",
                      "~/Content/loading-bar.css",
                      "~/Content/angular-material.min.css",
                      "~/Content/main.css"));

            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                "~/Scripts/vendor/angular.min.js",
                "~/Scripts/vendor/angular-aria.min.js",
                "~/Scripts/vendor/angular-animate.min.js",
                "~/Scripts/vendor/angular-messages.min.js",
                "~/Scripts/vendor/angular-filter.min.js",
                "~/Scripts/vendor/angular-material.min.js",
                "~/Scripts/vendor/ng-map.min.js",
                "~/Scripts/vendor/ng-google-chart.min.js",
                "~/Scripts/vendor/loading-bar.js",
                "~/Scripts/vendor/slick.min.js",
                "~/Scripts/vendor/angular-slick.min.js",
                "~/Scripts/app.js"));
        }
    }
}
