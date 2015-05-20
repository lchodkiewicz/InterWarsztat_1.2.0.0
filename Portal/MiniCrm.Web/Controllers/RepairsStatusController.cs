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
    public class RepairsStatusController : Controller
    {
        protected string _message = "Wystąpił problem z Twoim rządaniem, skontaktuj się z administratorem";
        protected static readonly ILog _log = LogManager.GetLogger("Controller");
        // GET: RepairsStatussStatus
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "admin, employee")]
        public JsonResult ShowRepairsStatusList()
        {
            using (var db = new Database("DefaultConnection"))
            {
                try
                {
                    var result = db.Fetch<RepairsStatus>("Select * from dbo.RepairsStatus");
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
        public JsonResult CreateRepairsStatus(RepairsStatus repairStatus)
        {
            try
            {   // set the date of change and current user
                repairStatus.DateOfChange = DateTime.UtcNow;
                repairStatus.EmployeeID = HttpContext.User.Identity.Name;

                using (var db = new Database("DefaultConnection"))
                {
                    using (var scope = db.GetTransaction())
                    {
                        Repair entityRepair = db.SingleOrDefault<Repair>("Select * from Repairs where RepairID = @0", repairStatus.RepairID); // search record where we need to update status
                        entityRepair.Status = repairStatus.Status;
                        db.Update(entityRepair); //update status naprawy

                        db.Insert(repairStatus); // new record for RepairStatus

                        scope.Complete();
                        return Json(new { Result = "OK", Record = repairStatus });
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                return Json(new { Result = "ERROR", Message = _message });
            }
        }
        [HttpPost]
        public JsonResult RepairsStatusGetById(string RepairID)
        {
            try
            {
                using (var db = new Database("DefaultConnection"))
                {
                    var result = db.Fetch<RepairsStatus>("Select * from RepairsStatus where RepairID = @0", RepairID);
                    return Json(new { Result = "OK", Records = result });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
        [HttpPost]
        [Authorize(Roles = "admin, employee")]
        public JsonResult DeleteRepairsStatus(string repairStatusID)
        {
            try
            {

                using (var db = new Database("DefaultConnection"))
                {
                    using (var scope = db.GetTransaction())
                    {
                        //search and delete repair
                      RepairsStatus repair = db.Single<RepairsStatus>("Select * from RepairsStatus where RepairStatusID = @0", repairStatusID);
                        db.Delete(repair);

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
        public JsonResult UpdateRepairsStatus(RepairsStatus repairStatus)
        {
            
            try
            {   // set the date of change and current user
                repairStatus.DateOfChange = DateTime.UtcNow;
                repairStatus.EmployeeID = HttpContext.User.Identity.Name;

                using (var db = new Database("DefaultConnection"))
                {
                    using (var scope = db.GetTransaction())
                    {
                        Repair entityRepair = db.SingleOrDefault<Repair>("Select * from Repairs where RepairID = @0", repairStatus.RepairID); // search record where we need to update status
                        entityRepair.Status = repairStatus.Status;
                        db.Update(entityRepair); //update status naprawy

                        db.Update(repairStatus); // update record for RepairStatus

                        scope.Complete();
                        return Json(new { Result = "OK", Record = repairStatus });
                    }
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