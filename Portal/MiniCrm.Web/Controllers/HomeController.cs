using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MiniCrm.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [Authorize(Roles= "admin")]
        public ActionResult Product()
        {
            ViewBag.Message = "Your Product page.";

            return View();
        }
        public ActionResult Employee()
        {
            ViewBag.Message = "Your Employee page.";

            return View();
        }
    }

}