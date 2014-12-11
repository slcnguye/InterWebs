namespace InterWebs
{
    using System.Web.Optimization;

    public class BundleConfig
    {
        public const string BundlesImageSlider = "~/bundles/jssorslider";
        public const string BundlesSite = "~/bundles/site";
        public const string BundlesJquery = "~/bundles/jquery";
        public const string BundlesKnockout = "~/bundles/knockout";
        public const string BundlesModernizr = "~/bundles/modernizr";
        public const string BundlesBootstrap = "~/bundles/bootstrap";
        public const string BundlesI18N = "~/bundles/i18n";
        public const string ContentStyles = "~/Content/styles";

        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle(BundlesSite)
                .IncludeDirectory("~/Scripts/site/knockout-models", "*.js", true)
                .IncludeDirectory("~/Scripts/site/knockout-extenders", "*.js", true)
                .IncludeDirectory("~/Scripts/site/knockout-bindings", "*.js", true));

            bundles.Add(new ScriptBundle(BundlesJquery)
                .Include("~/Scripts/jquery-{version}.js")
                .Include("~/Scripts/jquery.signalR-2.1.2.js")
                .Include("~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle(BundlesKnockout)
                .Include("~/Scripts/knockout-{version}.js")
                .Include("~/Scripts/namespace.js"));

            bundles.Add(new ScriptBundle(BundlesModernizr)
                .Include("~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle(BundlesBootstrap)
                .Include("~/Scripts/bootstrap.js")
                .Include("~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle(BundlesI18N)
                .Include("~/Scripts/i18next-{version}.js"));

            bundles.Add(new ScriptBundle(BundlesImageSlider)
                .Include("~/Scripts/jquery.flexslider-min.js"));

            bundles.Add(new StyleBundle(ContentStyles)
                .Include("~/Content/Styles/bootstrap.css")
                .Include("~/Content/Styles/font-awesome.css")
                .Include("~/Content/Styles/flexslider.css")
                .IncludeDirectory("~/Content/Styles", "*.less", true));
        }
    }
}
