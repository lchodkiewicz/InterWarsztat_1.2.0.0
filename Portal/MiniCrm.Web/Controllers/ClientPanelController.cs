using EnzoMiniCrm.Model;
using log4net;
using MiniCrm.Web.Models;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;

namespace MiniCrm.Web.Controllers
{
    public class ClientPanelController : Controller
    {
        protected string _message = "Wystąpił problem z Twoim rządaniem, skontaktuj się z administratorem";
        protected static readonly ILog _log = LogManager.GetLogger("Controller");
        // GET: ClientPanel
        [Authorize(Roles = "customer")]
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "customer")]
        public ActionResult MyVehicles()
        {
            return View();
        }
        [Authorize(Roles = "customer")]
        public ActionResult MyPermission()
        {
            return View();
        }
        [Authorize(Roles = "customer")]
        public ActionResult MyStatusRapair()
        {
            return View();
        }
        [Authorize(Roles = "customer")]
        public ActionResult MyRepair()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "customer")]
        public JsonResult ShowRepairsStatusList(string customerID)
        {
            using (var db = new Database("DefaultConnection"))
            {
                try
                {
                    var result = db.Fetch<RepairsStatus>("Select * from dbo.RepairsStatus where CustomerId = @0", customerID);
                    return Json(new { Result = "OK", Records = result });
                }
                catch (Exception ex)
                {
                    _log.Error(ex);
                    return Json(new { Result = "ERROR", Message = _message });
                }
            }
        }

        [HttpPost]
        [Authorize(Roles = "customer")]
        public JsonResult ShowRepairsStatusListToAccept(string customerID)
        {
            using (var db = new Database("DefaultConnection"))
            {
                try
                {
                    var result = db.Fetch<RepairsStatus>("Select * from dbo.RepairsStatus where CustomerId = @0 and Permission= 4", customerID);
                    return Json(new { Result = "OK", Records = result });
                }
                catch (Exception ex)
                {
                    _log.Error(ex);
                    return Json(new { Result = "ERROR", Message = _message });
                }
            }
        }
        [HttpPost]
        public JsonResult UpdateMyPermission(RepairsStatus repair)
        {
            try
            {
             
                using (var db = new Database("DefaultConnection"))
                {

                    db.Update(repair);

                }
                return Json(new { Result = "OK" });
            }


            catch (Exception ex)
            {
                _log.Error(ex);
                return Json(new { Result = "ERROR", Message = _message });
            }
        }
        [HttpPost]
        [Authorize(Roles = "customer")]
        public JsonResult ShowRepairsList(string customerID)
        {
            using (var db = new Database("DefaultConnection"))
            {
                try
                {
                    var result = db.Fetch<Repair>("Select * from dbo.Repairs where CustomerId = @0", customerID);
                    return Json(new { Result = "OK", Records = result });
                }
                catch (Exception ex)
                {
                    _log.Error(ex);
                    return Json(new { Result = "ERROR", Message = _message });
                }
            }
        }
        //When I wrote this, only God and I understood what I was doing
        [HttpGet]
        [Authorize(Roles = "customer")]
        public JsonResult CountOfNotification()
        {
            using (var db = new Database("DefaultConnection"))
            {
                try
                {
                    var _curentuser = User.Identity.Name;
                    var result = db.Fetch<int>("select count (*) from dbo.RepairsStatus as rs where rs.CustomerID = @0 And rs.Status = 4", _curentuser); //Status 4 -> Waiting for permission from Cusomer
                    return Json(new { Result = "OK", Records = result });
                }
                catch (Exception ex)
                {
                    _log.Error(ex);
                    return Json(new { Result = "ERROR", Message = _message });
                }
            }
        }

        [HttpGet]
        [Authorize(Roles = "customer")]
        public JsonResult NotificationForClient()
        {
            UserNotification userNotification = new UserNotification();
            using (var db = new Database("DefaultConnection"))
            {
                try
                {
                    //To dało sie napisać lepiej, ale jest 01:20 
                    var _curentuser = User.Identity.Name;
                    var countNotification = db.Fetch<int>("select count (*) from dbo.RepairsStatus as rs where rs.CustomerID = @0 And rs.Permission = 4", _curentuser); //Status 4 -> Waiting for permission from Cusomer
                    var describeNotification = db.Fetch<string>("select rs.DescribeRepair from dbo.RepairsStatus as rs where rs.CustomerID = @0 And rs.Permission = 4", _curentuser);
                    userNotification.Count = countNotification.ToList<int>();
                    userNotification.Describe = describeNotification.ToList<string>();
                    return Json(new { Result = "OK", Records = userNotification }, JsonRequestBehavior.AllowGet);

                    //ok, zgoda... tego nie dało się napisać gorzej.
                }
                catch (Exception ex)
                {
                    _log.Error(ex);
                    return Json(new { Result = "ERROR", Message = _message });
                }
            }
        }
        //Now, God only knows
    }


}