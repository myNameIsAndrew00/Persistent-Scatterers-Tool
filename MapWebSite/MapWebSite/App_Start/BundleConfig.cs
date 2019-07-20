﻿using BundleTransformer.Core.Bundles;
using BundleTransformer.Core.Orderers;
using System;
using System.Web;
using System.Web.Optimization;

namespace MapWebSite
{
    public class BundleConfig
    { 
        public static void RegisterBundles(BundleCollection bundles)
        {
            loadScriptBundles(bundles);
            loadBootstrapBundles(bundles);
            loadSassBundles(bundles);
            loadOpenLayersBundles(bundles);

        }




        #region Private

        private static void loadSassBundles(BundleCollection bundles)
        {
            var loginStyleBundle = new CustomStyleBundle("~/Content/login_sass").Include(
                "~/Resources/css/login.scss");  
            loginStyleBundle.Orderer = new NullOrderer();
            
            var homeStyleBundle = new CustomStyleBundle("~/Content/home_sass").Include(
                "~/Resources/css/home.scss");
            homeStyleBundle.Orderer = new NullOrderer();

            bundles.Add(loginStyleBundle);
            bundles.Add(homeStyleBundle);
        }

        private static void loadBootstrapBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/FrameworkContent/css").Include(
                         "~/FrameworkContent/bootstrap.css"));
        }

        private static void loadScriptBundles(BundleCollection bundles)
        {
            /**Framework scripts bellow **/
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                      "~/FrameworkContent/Scripts/jquery-3.3.1.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/FrameworkContent/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/FrameworkContent/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/FrameworkContent/Scripts/bootstrap.js"));


            /**Custom scripts bellow**/
            bundles.Add(new ScriptBundle("~/scripts/login").Include(
                "~/Resources/js/login.js"));

            bundles.Add(new ScriptBundle("~/scripts/home").Include(
                "~/Resources/js/home.js",
                "~/Resources/js/map.js"));

            bundles.Add(new ScriptBundle("~/scripts/menu").Include(
                "~/Resources/js/menu.js"));
        }

        private static void loadOpenLayersBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/scripts/openlayers").Include(
                "~/FrameworkContent/OpenLayers/ol.js"));

            bundles.Add(new StyleBundle("~/css/openlayers").Include(
                "~/FrameworkContent/OpenLayers/ol.css")); 
        }

        #endregion
    }
}