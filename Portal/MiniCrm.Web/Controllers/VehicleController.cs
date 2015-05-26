using EnzoMiniCrm.Model;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MiniCrm.Web.Controllers
{
    public class VehicleController : Controller
    {
        // GET: Vehicle
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult VehicleGetById(string customerID)
        {
            try
            {
                using (var db = new Database("DefaultConnection"))
                {
                    var result = db.Fetch<Vehicle>("Select * from dbo.Vehicles where CustomerID = @0", customerID);
                    return Json(new { Result = "OK", Records = result });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
        [HttpPost]
        public JsonResult ShowVehicleList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            int totalRecords = 0;
            string customerId = User.Identity.Name;
            using (var db = new Database("DefaultConnection"))
            {
                try
                {
                    var sql = PetaPoco.Sql.Builder.Append("Select * from dbo.Vehicles WHERE CustomerID = @0", customerId);
                    totalRecords = (db.Query<Vehicle>(sql)).Count();


                    sql.Append("ORDER BY VehicleID OFFSET @0 ROWS FETCH NEXT @1 ROWS ONLY", jtStartIndex, jtPageSize);
                    var result = db.Query<Vehicle>(sql);

                    return Json(new { Result = "OK", Records = result, TotalRecordCount = totalRecords });
                }

                catch (Exception ex)
                {
                    return Json(new { Result = "ERROR", Message = ex.Message });
                }
            }
        }
        
        [HttpPost]
        public JsonResult CreateVehicle(Vehicle vehicle)
        {
            try
            {
                using (var db = new Database("DefaultConnection"))
                {
                    db.Insert(vehicle);
                    return Json(new { Result = "OK", Record = vehicle });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
        [HttpPost]
        public JsonResult UpdateVehicle(Vehicle vehicle)
        {
            try
            {
                using (var db = new Database("DefaultConnection"))
                {
                    db.Update(vehicle);
                    return Json(new { Result = "OK"});
                }
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
        public JsonResult GetVehicleOptions(string customerID = null)
        {
            try
            {
                using (var db = new Database("DefaultConnection"))
                {
                    var vehicles = db.Fetch<Vehicle>("Select * from Vehicles where CustomerID = @0", customerID).Select(c => new { DisplayText = c.VehicleID, Value = c.VehicleID });
                    return Json(new { Result = "OK", Options = vehicles });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
    }
}