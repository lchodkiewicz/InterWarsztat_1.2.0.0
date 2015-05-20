using EnzoMiniCrm.Model;
using log4net;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MiniCrm.Web.APIControllers
{
    public class api_CustomerController : ApiController
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(api_CustomerController));
        public List<Customer> Get()
        {
            using (var db = new Database("DefaultConnection"))
            {
                try
                {
                    var result = db.Fetch<Customer>("Select * from dbo.Customers");
                    return result;
                }
                catch (Exception ex)
                {
                    _log.Error(ex.ToString());
                    throw;
          
                }
            }
        }
        public void Post(Customer entity)
        {
            using(var db = new Database("DefaultConnection"))
            {

            }
        }
    }
}
