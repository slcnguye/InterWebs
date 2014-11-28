﻿using System.Web.Optimization;

namespace InterWebs
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/site")
                .IncludeDirectory("~/Scripts/site/knockout-models", "*.js", true)
                .IncludeDirectory("~/Scripts/site/knockout-extenders", "*.js", true)
                .IncludeDirectory("~/Scripts/site/knockout-bindings", "*.js", true));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js").Include(
                        "~/Scripts/jquery.signalR-2.1.2.js"));

            bundles.Add(new ScriptBundle("~/bundles/knockout").Include(
            "~/Scripts/knockout-{version}.js",
            "~/Scripts/namespace.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/i18n").Include(
                "~/Scripts/i18next-{version}.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/Styles/bootstrap.css",
                      "~/Content/Styles/site.less",
                      "~/Content/Styles/font-awesome.css"));
        }
    }
}
