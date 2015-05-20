using System.Web.Optimization;

[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(MiniCrm.Web.App_Start.AjaxHelperBundleConfig), "RegisterBundles")]

namespace MiniCrm.Web.App_Start
{
	public class AjaxHelperBundleConfig
	{
		public static void RegisterBundles()
		{
			BundleTable.Bundles.Add(new ScriptBundle("~/bundles/ajaxhelper").Include("~/Scripts/jquery.ajaxhelper.js"));
		}
	}
}