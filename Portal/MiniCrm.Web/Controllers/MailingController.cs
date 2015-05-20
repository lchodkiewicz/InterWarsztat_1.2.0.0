using ActiveUp.Net.Mail;
using EnzoMiniCrm.Model;
using log4net;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace MiniCrm.Web.Controllers
{
    public class MailingController : Controller
    {
        protected string _message = "Wystąpił problem z Twoim rządaniem, skontaktuj się z administratorem";
        protected static readonly ILog _log = LogManager.GetLogger("Controller");
        // GET: Mailing
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Mailing()
        {

            return View();
        }
        public ActionResult GetSendMailList()
        {

            return View();
        }


        [HttpPost]
        public JsonResult ShowMailingList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            int TotalRecordCount;
            using (var db = new Database("DefaultConnection"))
            {
                try
                {
                    var totalRecords = db.Fetch<Mail>("Select * from dbo.Mails");
                    var result = db.Fetch<Mail>("Select * from dbo.Mails ORDER BY MailID OFFSET @0 ROWS FETCH NEXT @1 ROWS ONLY", jtStartIndex, jtPageSize);
                    TotalRecordCount = result.Count;
                    return Json(new { Result = "OK", Records = result, TotalRecordCount = totalRecords.Count });
                }
                catch (Exception ex)
                {
                    return Json(new { Result = "ERROR", Message = ex.Message});
                    //TODO add log4net_log.Error(ex.ToString());

                    throw;
                }
            }
        }
        [HttpPost]
        public JsonResult CreateMail(Mail mail)
        {
            try
            {     
                using (var db = new Database("DefaultConnection"))
                {
                    db.Insert(mail);
                    SendEmail(mail);
                    return Json(new { Result = "OK", Record = mail });
                }
                
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                return Json(new { Result = "ERROR", Message = _message });
            }
        }
        public JsonResult MailingGetById(int mailID)
        {
            try
            {
                using (var db = new Database("DefaultConnection"))
                {
                    var result = db.Fetch<Mail>("Select * from dbo.Mails where MailId = @0", mailID);
                    return Json(new { Result = "OK", Records = result });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetReciveMailList()
        {
            List<Mail> result = new List<Mail>();
            try
            {
                var mailRepository = new MiniCrm.Web.Helpers.MailRepositorycs.MailRepository(
                            "imap.gmail.com",
                            993,
                            true,
                            "enzoitsollutions@gmail.com",
                            "EnzoAdmin123@1"
                        );

                var emailList = mailRepository.GetAllMails("inbox");
                int i=1;
                foreach (Message email in emailList)
                {
                    result.Add(new Mail { From = email.From.Email, Subject=email.Subject, Body = email.BodyText.TextStripped, MailID = i, SendingDate = email.Date });
                        i++;
                    if (email.Attachments.Count > 0)
                    {
                        foreach (MimePart attachment in email.Attachments)
                        {
                            
                        }
                        
                    }
                }
                return Json(new { Result = "OK", Records = result });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
        [HttpPost]
        public JsonResult GetUnreadeMailList()
        {
            List<Mail> result = new List<Mail>();
            try
            {
                var mailRepository = new MiniCrm.Web.Helpers.MailRepositorycs.MailRepository(
                            "imap.gmail.com",
                            993,
                            true,
                            "enzoitsollutions@gmail.com",
                            "EnzoAdmin123@1"
                        );

                var emailList = mailRepository.GetAllMails("unseen");
                int i = 1;
                foreach (Message email in emailList)
                {
                    result.Add(new Mail { From = email.From.Email, Subject = email.Subject, Body = email.BodyText.TextStripped, MailID = i, SendingDate = email.Date });
                    i++;
                    if (email.Attachments.Count > 0)
                    {
                        foreach (MimePart attachment in email.Attachments)
                        {

                        }

                    }
                }
                return Json(new { Result = "OK", Records = result });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
        [HttpPost]
        public JsonResult DeleteMailById()
        {
            try
            {
          
                using (var db = new Database("DefaultConnection"))
                {
                    db.Delete("Mails", "MailID", null);
                    return Json(new { Result = "OK" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
        [HttpPost]
        [Authorize(Roles="admin, employee")]
         private void SendEmail(Mail mail)
        {
           
            // For information on sending mail, please visit http://go.microsoft.com/fwlink/?LinkID=320771
            try
            {
            #region formatter
            string text = string.Format("Please click on this link to {0}: {1}", mail.Body, mail.Body);
            string html = "" + mail.Body;

            //html += HttpUtility.HtmlEncode(@"Or click on the copy the following link on the browser:" + message);
            #endregion

           
            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("enzoitsollutions@gmail.com");
            msg.To.Add(new MailAddress(mail.CustomerID));
            msg.Subject = mail.Subject;
            msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
            msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));

            System.Net.Mail.SmtpClient smtpClient = new System.Net.Mail.SmtpClient("smtp.gmail.com", Convert.ToInt32(587));
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("enzoitsollutions@gmail.com", "EnzoAdmin123@1");
            smtpClient.Port = 587;
            smtpClient.Credentials = credentials;
            smtpClient.EnableSsl = true;
            smtpClient.Send(msg);
            mail.ErrorRaport = "OK";
                //Update the record mail
            
                

            }
            catch (Exception ex)
            {
                _log.Error(ex);
                mail.ErrorRaport = ex.Message;
                
            }
            finally
            {
                mail.SendingDate = DateTime.UtcNow;
                using(var db = new Database("DefaultConnection"))
                {
                    db.Update(mail);

                }
              
            }
     
        }
    }
}