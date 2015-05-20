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
        public JsonResult ShowVehicleList(string customerID)
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