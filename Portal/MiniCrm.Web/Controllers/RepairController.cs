using EnzoMiniCrm.Model;
using log4net;
using Mechanic.BL.Constans;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MiniCrm.Web.Controllers
{
    public class RepairController : Controller
    {
        protected string _message = "Wystąpił problem z Twoim rządaniem, skontaktuj się z administratorem";
        protected static readonly ILog _log = LogManager.GetLogger("Controller");
        private DictionaryStatusesRepair dictionaryStatusesRepair = new DictionaryStatusesRepair();

        // GET: Rapair
        public ActionResult Index()
        {
            return View();
        }

   
        public ActionResult Repair()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin, employee")]
        public JsonResult ShowRepairList(int StatusId = 0, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            
            using (var db = new Database("DefaultConnection"))
            {
                try
                {
                    var totalRecords = db.Fetch<Mail>("Select * from dbo.Repairs");
                    var sql = PetaPoco.Sql.Builder.Append("Select * from dbo.Repairs ");
                    if (dictionaryStatusesRepair.HasKey(StatusId))
                    {
                        sql.Append("Where Status = @0", StatusId);
                    }
                    sql.Append(" ORDER BY RepairID OFFSET @0 ROWS FETCH NEXT @1 ROWS ONLY", jtStartIndex, jtPageSize);
                    var result = db.Query<Repair>(sql);
                    return Json(new { Result = "OK", Records = result, TotalRecordCount = result.Count() });
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
        public JsonResult CreateRepair(Repair repair)
        {
            try
            {
                using (var db = new Database("DefaultConnection"))
                {
                    db.Insert(repair);
                    return Json(new { Result = "OK", Record = repair });
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                return Json(new { Result = "ERROR", Message = _message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin, employee")]
        public JsonResult GetEmployeeID()
        {
            try
            {
                using (var db = new Database("DefaultConnection"))
                {
                    var result = db.Fetch<Employee>("Select * from Employees").Select(c => new { DisplayText = c.EmployeeID, Value = c.EmployeeID });
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
        [Authorize(Roles = "admin, employee")]
        public JsonResult DeleteRepair(string RepairID)
        {
            try
            {

                using (var db = new Database("DefaultConnection"))
                {

                    using (var scope = db.GetTransaction())
                    {
                       
                        //search and delete repair
                        Repair repair = db.Single<Repair>("Select * from Repairs where RepairID = @0", RepairID);
                        db.Delete(repair);


                        //search and delete all status from before deleteing repair
                        var statuses = db.Fetch<RepairsStatus>("Select * from RepairsStatus where RepairID = @0", RepairID);
                        foreach (var status in statuses)
                        {
                            db.Delete(status);
                        }
                        scope.Complete();


                    }
                    return Json(new { Result = "OK" });
                }
            }
            catch (Exception ex)
            {

                _log.Error(ex);
                return Json(new { Result = "ERROR", Message = _message });
            }
        }
        [HttpPost]
        public JsonResult UpdateRepair(Repair repair)
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
        /// <summary>
        /// Get Repair for Employee
        /// </summary>
        /// <param name="customerID"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetRepairForEmployeeById(string employeeID)
        {
            try
            {
                using (var db = new Database("DefaultConnection"))
                {
                    var result = db.Fetch<Repair>("Select * from dbo.Repairs where EmployeeID = @0", employeeID);
                    return Json(new { Result = "OK", Records = result });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
    }
}