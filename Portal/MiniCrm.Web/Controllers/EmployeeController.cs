using EnzoMiniCrm.Model;
using log4net;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MiniCrm.Web.Controllers
{
    public class EmployeeController : Controller
    {
        protected string _message = "Wystąpił problem z Twoim rządaniem, skontaktuj się z administratorem";
        protected static readonly ILog _log = LogManager.GetLogger("Controller");
        // GET: Employee
        
        public ActionResult Index()
        {
            return View();
        }
         [Authorize(Roles = "admin")]
        public ActionResult Employee()
        {
            ViewBag.Message = "Your Page to manage employees.";

            return View();
        }
        [HttpPost]
        [Authorize(Roles="admin")]
         public JsonResult ShowEmployeeList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            using (var db = new Database("DefaultConnection"))
            {
                try
                {
                    var result = db.Fetch<Employee>("Select TOP 10 * from dbo.Employees");
                    return Json(new { Result = "OK", Records = result });
                }
                catch (Exception ex)
                {
                    _log.Error(ex);
                    return Json(new { Result = "ERROR", Message = _message });

                    throw;
                }
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public JsonResult EmployeeGetById(string employeeID)
        {
            try
            {
                using (var db = new Database("DefaultConnection"))
                {
                    var result = db.Fetch<Employee>("Select * from dbo.Employees where EmployeeID = @0", employeeID);
                    return Json(new { Result = "OK", Records = result });
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                return Json(new { Result = "ERROR", Message = _message });
            }
        }


        [Authorize(Roles = "admin")]
        public JsonResult DeleteEmployee(string employeeID)
        {
            try
            {
               
                using (var db = new Database("DefaultConnection"))
                {
                    Employee employee = db.Single<Employee>("Select * from Employees where EmployeeID = @0", employeeID);
                    db.Delete(employee);
                    return Json(new { Result = "OK"});
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                return Json(new { Result = "ERROR", Message = _message });
            }
        }
    }
}