using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EnzoMiniCrm.Model;
using log4net;

namespace MiniCrm.Web.Controllers
{
    public class api_EmployeeController : ApiController
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(api_EmployeeController));
        public List<Employee> Get()
        {
            using (var db = new Database("DefaultConnection"))
            {
                try
                {
                    var result = db.Fetch<Employee>("Select * from dbo.Employees");
                    return result;
                }
                catch (Exception ex)
                {
                    _log.Error(ex.ToString());

                    throw;
                }
            }
        }
    }
}
