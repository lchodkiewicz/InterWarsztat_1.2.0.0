using EnzoMiniCrm.Model;
using log4net;
using Mechanik.Repository;
using MiniCrm.Web.Models;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MiniCrm.Web.Controllers
{

    public class CustomerController : Controller
    {
        protected string _message = "Wystąpił problem z Twoim rządaniem, skontaktuj się z administratorem";

        protected static readonly ILog _log = LogManager.GetLogger("CustomerController");
        // GET: Customer
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Customer()
        {
            ViewBag.Message = "Your Page to manage customers.";

            return View();
        }
        public JsonResult ShowCustomerList()
        {
            using (var db = new Database("DefaultConnection"))
            {
                try
                {
                    var result = db.Fetch<Customer>("Select * from dbo.Customers");
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
        [Authorize(Roles = "admin, employee")]
        public JsonResult GetCustomerID()
        {
            try
            {
                using (var db = new Database("DefaultConnection"))
                {
                    var result = db.Fetch<Customer>("Select * from Customers").Select(c => new { DisplayText = c.CustomerID, Value = c.CustomerID });
                    return Json(new { Result = "OK", Options = result });
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                return Json(new { Result = "ERROR", Message = _message });
            }
        }
        [HttpPost]
        public JsonResult NewGetCustomerID()
        {
            try
            {

                var result = new CustomerRepository(new Database("DefaultConnection")).GetById("tyborrafal@gmail.com");
                    return Json(new { Result = "OK", Options = result });
     
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                return Json(new { Result = "ERROR", Message = _message });
            }
        }
    }
}
