using Registration.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Registration.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration([Bind(Exclude = "ActivationCode,isEmailVerified")] UsersDB user)
        {
            bool status = false;
            string message = " ";
            //model validation
            if (ModelState.IsValid)
            {
                
                #region//email is already exist
                var isExist = isEmailExist(user.EmailID);
                if (isExist)
                {
                    ModelState.AddModelError("EmailExist", "Email is already used");
                    ViewBag.Message = " Email Already in use... ";
                    ViewBag.status = status;
                    return View(user);
                }
                #endregion
                

                #region ////generate activation code
                user.ActivationCode = Guid.NewGuid();
                #endregion

                #region//password  hashing
                user.Password = Crypto.Hash(user.Password);
                user.Confirm_Password = Crypto.Hash(user.Confirm_Password);
                #endregion

                int returnValue = 66;
                #region save to database
                using (MyDatabasesEntities dc = new MyDatabasesEntities())
                {
                    try
                    {
                        dc.UsersDBs.Add(user);
                        returnValue = dc.SaveChanges();
                    }
                    catch (DbEntityValidationException dbEx)
                    {
                        foreach (var validationErrors in dbEx.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                System.Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                            }
                        }
                        ViewBag.Message = ".... DataBase Failure ...";
                        ViewBag.status = status;
                        return View(user);
                    }

                    //send email to user
                    
                    if (returnValue == 1)
                    {
                        SendVerificationLinkEmail(user.EmailID, user.ActivationCode.ToString());
                        message = "Registration successfully done. Account Activation Link has been sent to your email id: " + user.EmailID;
                        status = true;
                    } 
                }
                #endregion
            }
            else //model invalid case
            {
                message = "Invaid Request";
            }
            ViewBag.Message = message;
            ViewBag.status = status;
            return View();
        }

        [NonAction]
        public void SendVerificationLinkEmail(string emailIDD, string activationCOde,string emailFor = "VerifyAccount")
        {
            var verifyUrl = "/User/"+emailFor+"/" + activationCOde;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("practise.saroj@gmail.com", "Registration Testing Web Application");
            var toEmail = new MailAddress(emailIDD);
            var fromEmailPassword = ""; //replace wth actual password

            string subject="", body = "";
            if (emailFor == "VerifyAccount")
            {
                subject = "Your account is Successfully created";
                body = "<br/>We are excited to tell you that your account is Successfully created.<br/> Please click on below link to verify Your account" + "<br/>Click in the link below <br/><br/> <a" + " href='" + link + "'>" + link + "</a>";
            }else if (emailFor == "ResetPassword")
            {
                subject = "Reset Password";
                body = "Hi,<br/>We got request to reset your account password. Please click on the below link to reset your password" + " <br/> <a href="+link+">Reset Password</a>";
            }

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };
            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }

        [NonAction]
        public bool isEmailExist(string emailID)
        {
            using (MyDatabasesEntities dc = new MyDatabasesEntities())
            {
                //var v = dc.UsersDBs.Where(user => user.EmailID == emailID).FirstOrDefault();
                var vk = dc.UsersDBs.Any(userr => userr.EmailID == emailID);
                return vk;
            }
        }

        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            bool status = false;
            using (MyDatabasesEntities dc = new MyDatabasesEntities())
            {
                dc.Configuration.ValidateOnSaveEnabled = false; // this line  i have added here to avoid
                //confirm password doesnot match issue on save changes
                var v = dc.UsersDBs.Where(userr => userr.ActivationCode == new Guid(id)).FirstOrDefault();
                if (v != null)
                {
                    v.isEmailVerified = true;
                    dc.SaveChanges();
                    status = true;
                }
                else
                {
                    ViewBag.Message = "Invalid Request";
                }
            }
            ViewBag.status = status;
            return View();
        }

        public JsonResult isUserNamefound(string userName)
        {
            using (MyDatabasesEntities dc = new MyDatabasesEntities())
            {
                var vk = dc.UsersDBs.Any(userr => userr.UserName == userName);
                return Json(vk, JsonRequestBehavior.DenyGet);
            }
            return Json(false, JsonRequestBehavior.DenyGet);
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserLoginModel Login_Attemped_User,string ReturnUrl="")
        {
         string ErrorMessage = "";
            if (ModelState.IsValid)
            {
                using(MyDatabasesEntities dc = new MyDatabasesEntities())
                {
                    var v = dc.UsersDBs.Where(userr => userr.UserName ==Login_Attemped_User.UserName).FirstOrDefault();
                    if (v != null)
                    {
                        if (string.Compare(Crypto.Hash(Login_Attemped_User.Password), v.Password) == 0)
                        {
                            int timeout = Login_Attemped_User.RememberMe ? 525600 : 20; //525600min = 1year
                            var ticket = new FormsAuthenticationTicket(Login_Attemped_User.UserName, Login_Attemped_User.RememberMe, timeout);
                            string encrypt = FormsAuthentication.Encrypt(ticket);
                            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypt);
                            cookie.Expires = DateTime.Now.AddMinutes(timeout);
                            cookie.HttpOnly = true;
                            Response.Cookies.Add(cookie);

                            if (Url.IsLocalUrl(ReturnUrl))
                            {
                                return Redirect(ReturnUrl);
                            }
                            else
                            {
                                return RedirectToAction("Index", "Home");
                            }
                        }
                        else
                        {
                            ErrorMessage = "Invalid UserName/Password";
                        }
                    }
                    else
                    {
                        ErrorMessage = "Invalid UserName";
                    }    
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
            ViewBag.Message = ErrorMessage;
            return View(Login_Attemped_User);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login","User");
        }


        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(string EmailID)
        {
            //verify email id
            //generate reset passowrd link
            //send email

            string Message = "";
            bool status = false;

            using(MyDatabasesEntities dc = new MyDatabasesEntities())
            {
                var account = dc.UsersDBs.Where(usee => usee.EmailID == EmailID).FirstOrDefault();
                if (account != null)
                {
                    //send email for reset password
                    string resetCode = Guid.NewGuid().ToString();
                    SendVerificationLinkEmail(EmailID, resetCode, "ResetPassword");
                    account.ResetPasswordCode = resetCode;
                    //
                    dc.Configuration.ValidateOnSaveEnabled = false;
                    int retvval = dc.SaveChanges();
                    if (retvval == 1)
                    {
                        Message = "Account reset code successfully sent to " + EmailID;
                        status = true;
                    }
                }
                else
                {
                    Message = "Account not found";
                }
            }
            ViewBag.Message = Message;
            ViewBag.status = status;
            return View();
        }

        public ActionResult ResetPassword(string id)
        {
            //verify the reset password link
            //find account associated with this link
            //redirect to reset password page
            using(MyDatabasesEntities dc= new MyDatabasesEntities())
            {
                var user = dc.UsersDBs.Where(a => a.ResetPasswordCode == id).FirstOrDefault();
                if (user != null)
                {
                    ResetPasswordModel model = new ResetPasswordModel();
                    model.ResetPasswordCode = id;
                    ViewBag.Message = "You can add function to reset password";
                    return View("Login");
                }
                else
                {
                    return HttpNotFound();
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel ResetModel)
        {
            var message = "";
            if (ModelState.IsValid)
            {
                using(MyDatabasesEntities dc = new MyDatabasesEntities())
                {
                    var user = dc.UsersDBs.Where(a => a.ResetPasswordCode == ResetModel.ResetPasswordCode).FirstOrDefault();
                    if (user != null)
                    {
                        user.Password = Crypto.Hash(ResetModel.NewPassword);
                        user.ResetPasswordCode = "";
                        dc.Configuration.ValidateOnSaveEnabled = false;
                        int i = dc.SaveChanges();
                        if (i == 1)
                        {
                            message = "New Password Updated successfull";
                        }
                        else
                        {

                            message = "New Password Updated Failed";
                        }
                    }
                }
            }
            else
            {
                message = "Invalid Actions";
            }

            ViewBag.Message = message;
            return View(ResetModel);
        }
    }
}