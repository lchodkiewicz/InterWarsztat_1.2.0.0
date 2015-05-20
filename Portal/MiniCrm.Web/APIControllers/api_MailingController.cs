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
    public class api_MailingController : ApiController
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(api_MailingController));
        public object Get()
        {
            using (var db = new Database("DefaultConnection"))
            {
                try
                {
                    var result = db.Fetch<Mail>("Select * from dbo.Mails");
                    return new { Result = "OK", Records = result };
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

