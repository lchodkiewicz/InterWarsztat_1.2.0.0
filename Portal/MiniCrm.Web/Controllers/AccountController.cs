using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Owin;
using MiniCrm.Web.Models;
using System.Net.Mail;
using System.Net.Mime;
using System.Web.Security;
using PetaPoco;
using EnzoMiniCrm.Model;
using log4net;

namespace MiniCrm.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        protected static readonly ILog _log = LogManager.GetLogger("Controller");
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            //ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await UserManager.FindAsync(model.Email, model.Password);
                    if (user != null)
                    {
                        await SignInAsync(user, model.RememberMe);
                        
                        if (user.Roles.First().RoleId == "2")
                        {
                            return RedirectToAction("index", "ClientPanel");
                        }
                        else
                        {
                            return RedirectToLocal(returnUrl);
                        }

                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid username or password.");
                    }
                }

                // If we got this far, something failed, redisplay form
                return View(model);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                throw;
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<JsonResult> LoginAndroid(string Email, string Password)
        {
            
            try
            {

                
                if (ModelState.IsValid)
                {
                    var user = await UserManager.FindAsync(Email, Password);
                    if (user != null)
                    {

                        
                        switch (user.Roles.First().RoleId)
                        {
                            case "1":
                                return Json((new { Result = "OK", Rola = "employee" }), JsonRequestBehavior.AllowGet);
                                break;
                            case "2":
                                return Json((new { Result = "OK", Rola = "customer" }), JsonRequestBehavior.AllowGet);
                                break;
                            case "3":
                                return Json((new { Result = "OK", Rola = "admin" }), JsonRequestBehavior.AllowGet);
                                break;
                        }
                        
                    }
                    else
                    {
                        return Json((new { Result = "Error", Rola = "Invalid username or password." }), JsonRequestBehavior.AllowGet);
                    }
                }
               
             
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                throw;
            }
            return Json(new { Result = "Fatal Error"});
        }

        //
        // GET: /Account/Register
        [Authorize(Roles = "admin, employee")]
        public ActionResult RegisterCustomer()
        {
            return View(new MyRegisterViewModelForCustomer());

        }

        public ActionResult DisplayEmail()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [Authorize(Roles = "admin, employee")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterCustomer(MyRegisterViewModelForCustomer model)
        {
            string password = Membership.GeneratePassword(12, 1);
            model.LoginData.UserType = "customer";

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser() { UserName = model.LoginData.Email, Email = model.LoginData.Email };
                IdentityResult result = await UserManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    UserManager.AddToRole(user.Id, model.LoginData.UserType);
                    //await SignInAsync(user, isPersistent: false);

                    //// For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    //// Send an email with this link
                    //// string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    //// var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    //// await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                    var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action(
                       "ConfirmEmail", "Account",
                       new { userId = user.Id, code = code },
                       protocol: Request.Url.Scheme);

                    //await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>");
                    SendEmail(user.Email, callbackUrl, "Registration", "Please confirm your account by clicking this link: " + callbackUrl);
                    // ViewBag.Link = callbackUrl;   // Used only for initial demo.

                    var code2 = UserManager.GeneratePasswordResetToken(user.Id);
                    var callbackUrl2 = Url.Action(
                       "ResetPassword", "Account",
                       new { userId = user.Id, code = code2 },
                       protocol: Request.Url.Scheme);
                    SendEmail(user.Email, callbackUrl, "Password Reset", "Please reset your password by clicking this link: " + callbackUrl2);
                    //await UserManager.ResetPasswordAsync(user.Id, "Please reset your passwort", "Please confirm your account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>");
                    //ForgotPasswordViewModel tmpEntity = new ForgotPasswordViewModel();
                    //tmpEntity.Email = model.LoginData.Email;
                    //var asd = ForgotPassword(tmpEntity);


                    try
                    {
                        using (var db = new Database("DefaultConnection"))
                        {
                            db.Insert(new Customer
                            {
                                CustomerID = model.LoginData.Email,
                                Address = model.CustomerData.Address,
                                City = model.CustomerData.City,
                                Country = model.CustomerData.Country,
                                Fax = model.CustomerData.Fax,
                                FirstName = model.CustomerData.FirstName,
                                LastName = model.CustomerData.LastName,
                                Phone = model.CustomerData.Phone,
                                PostalCode = model.CustomerData.PostalCode,
                                Region = model.CustomerData.Region

                            });

                        }
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex);
                        throw;
                    }
                    return View("DisplayEmail");


                }
                else
                {
                    AddErrors(result);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        [Authorize(Roles = "admin")]
        public ActionResult RegisterEmployee()
        {
            return View(new MyRegisterViewModelForEmployee());

        }
        [HttpPost]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterEmployee(MyRegisterViewModelForEmployee model)
        {
            string password = Membership.GeneratePassword(12, 1);
            model.LoginData.UserType = "employee";
           

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser() { UserName = model.LoginData.Email, Email = model.LoginData.Email };
                IdentityResult result = await UserManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    UserManager.AddToRole(user.Id, model.LoginData.UserType);
                    //await SignInAsync(user, isPersistent: false);

                    //// For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    //// Send an email with this link
                    //// string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    //// var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    //// await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                    var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action(
                       "ConfirmEmail", "Account",
                       new { userId = user.Id, code = code },
                       protocol: Request.Url.Scheme);

                    //await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>");
                    SendEmail(user.Email, callbackUrl, "Registration", "Please confirm your account by clicking this link: " + callbackUrl);
                    // ViewBag.Link = callbackUrl;   // Used only for initial demo.

                    var code2 = UserManager.GeneratePasswordResetToken(user.Id);
                    var callbackUrl2 = Url.Action(
                       "ResetPassword", "Account",
                       new { userId = user.Id, code = code2 },
                       protocol: Request.Url.Scheme);
                    SendEmail(user.Email, callbackUrl, "Password Reset", "Please reset your password by clicking this link: " + callbackUrl2);
                    //await UserManager.ResetPasswordAsync(user.Id, "Please reset your passwort", "Please confirm your account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>");
                    //ForgotPasswordViewModel tmpEntity = new ForgotPasswordViewModel();
                    //tmpEntity.Email = model.LoginData.Email;
                    //var asd = ForgotPassword(tmpEntity);


                    try
                    {
                        using (var db = new Database("DefaultConnection"))
                        {
                            db.Insert(new Employee
                            {
                                EmployeeID = model.LoginData.Email,
                                Address = model.EmployeeData.Address,
                                City = model.EmployeeData.City,
                                Country = model.EmployeeData.Country,
                                Notes = model.EmployeeData.Notes,
                                FirstName = model.EmployeeData.FirstName,
                                LastName = model.EmployeeData.LastName,
                                BirthDate = model.EmployeeData.BirthDate,
                                HireDate = model.EmployeeData.HirehDate,
                                HomePhone = model.EmployeeData.HomePhone,
                                PostalCode = model.EmployeeData.PostalCode,
                                Region = model.EmployeeData.Region,
                                PhotoPath = "/Content/images/default.png"
                            });

                        }
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex);

                        throw;
                    }
                    return View("DisplayEmail");


                }
                else
                {
                    AddErrors(result);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            IdentityResult result;
            try
            {
                result = await UserManager.ConfirmEmailAsync(userId, code);
                //Todo dodać faktycznie opcję potwierdzania e-mailem, w chwili obecnej nie trzeba kliknąć żeby się zalogować

            }
            catch (InvalidOperationException ioe)
            {
                _log.Error(ioe);
                // ConfirmEmailAsync throws when the userId is not found.
                ViewBag.errorMessage = ioe.Message;
                return View("Error");
            }

            if (result.Succeeded)
            {
                return View();
            }

            // If we got this far, something failed.
            AddErrors(result);
            ViewBag.errorMessage = "ConfirmEmail failed";
            return View("Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "The user either does not exist");
                    return View();
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            if (code == null)
            {
                return View("Error");
            }
            return View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "No user found.");
                    return View();
                }
                IdentityResult result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("ResetPasswordConfirmation", "Account");
                }
                else
                {
                    AddErrors(result);
                    return View();
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            ManageMessageId? message = null;
            IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                await SignInAsync(user, isPersistent: false);
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                        await SignInAsync(user, isPersistent: false);
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var user = await UserManager.FindAsync(loginInfo.Login);
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // If the user does not have an account, then prompt the user to create an account
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        }

        //
        // GET: /Account/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
            IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            if (result.Succeeded)
            {
                return RedirectToAction("Manage");
            }
            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };
                IdentityResult result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInAsync(user, isPersistent: false);

                        // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                        // Send an email with this link
                        string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                        var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                        SendEmail(user.Email, callbackUrl, "Confirm your account", "Please confirm your account by clicking this link");

                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);

            //TODO ŁCH Usówanie konta z bazy
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, await user.GenerateUserIdentityAsync(UserManager));
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private void SendEmail(string email, string callbackUrl, string subject, string message)
        {
            // For information on sending mail, please visit http://go.microsoft.com/fwlink/?LinkID=320771

            #region formatter
            string text = string.Format("Please click on this link to {0}: {1}", subject, message);
            string html = "" + message;

            //html += HttpUtility.HtmlEncode(@"Or click on the copy the following link on the browser:" + message);
            #endregion

            try
            {
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress("enzoitsollutions@gmail.com");
                msg.To.Add(new MailAddress(email));
                msg.Subject = subject;
                msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
                msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));

                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", Convert.ToInt32(587));
                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("enzoitsollutions@gmail.com", "EnzoAdmin123@1");
                smtpClient.Port = 587;
                smtpClient.Credentials = credentials;
                smtpClient.EnableSsl = true;
                smtpClient.Send(msg);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                throw;
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}