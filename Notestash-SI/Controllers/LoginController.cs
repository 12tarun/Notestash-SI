using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Notestash_SI.Models;
using NotestashUserDataAccess;
using System.Net.Mail;
using SecurityDriven.Inferno;
using SecurityDriven.Inferno.Kdf;
using PBKDF2 = SecurityDriven.Inferno.Kdf.PBKDF2;
using SecurityDriven.Inferno.Extensions;
using static SecurityDriven.Inferno.SuiteB;
using static SecurityDriven.Inferno.Utils;

namespace Notestash_SI.Controllers
{
    public class LoginController : ApiController
    {
        [HttpPost]
        [Route("api/Login/Check")]
        public HttpResponseMessage Check(LoginModel objUser)
        {
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Model State Invalid");

            LoginModel ob = new LoginModel();
            string check = ob.Check(objUser);

            if (check == "invalid")
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Invalid Credentials!");
            }
            else if (check == "error")
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Something Went Wrong!");
            }
            else if (check == "inactive")
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Please verify your email accoun. Verification link has been sent to your email id.");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, check);
            }
        }

        [HttpPost]
        [Route("api/Login/forgotPassword")]
        public HttpResponseMessage forgotPassword(forgotPassword User)
        {
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Model State Invalid");
            try
            {
                using (Notestash_DatabaseEntities db = new Notestash_DatabaseEntities())
                {
                    var emailId = db.tblUsers.FirstOrDefault(e=>e.Email == User.Email);
                    emailId.forgotPasswordCode = Guid.NewGuid();
                    db.SaveChanges();

                    changePasswordEmail(emailId.Email, emailId.forgotPasswordCode.ToString());
                    return Request.CreateResponse(HttpStatusCode.OK, "Link to change password has been sent to your email id.");
                }
            }
            catch(Exception ex)
            {
                string s = ex.Message;
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error occurred, please try again!");
            }
        }

        [NonAction]
        public void changePasswordEmail(string Email, string passwordChangeKey)
        {
            var verifyUrl = "/api/Login/changePassword/" + passwordChangeKey; 
            Uri link = new Uri(Request.RequestUri.AbsoluteUri.Replace(Request.RequestUri.PathAndQuery, verifyUrl));
            var fromEmail = new MailAddress("suryakant.rocky@gmail.com", "Notestash");
            var toEmail = new MailAddress(Email);
            var fromEmailPassword = "suryasharma";
            string subject = "Your account is successfully created!";
            string body = "<br/><br/>Please click on the link below to change your password." + "<br/><br/><a href='" + link + "'>" + link + "</a>";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword),
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }

        [HttpPost]
        [Route("api/Login/changePassword/{id}")]
        public HttpResponseMessage changePassword(string id, changePassword pass)
        {
            try
            {
                using (Notestash_DatabaseEntities db = new Notestash_DatabaseEntities())
                {
                    var passwordChanged = db.tblUsers.Where(e => e.forgotPasswordCode == new Guid(id)).FirstOrDefault();
                    string newPass = pass.newPassword;

                    var sha384Factory = HmacFactory;
                    var random = new CryptoRandom();

                    byte[] derivedKey;
                    string hashedPassword = null;
                    string passwordText = newPass;

                    byte[] passwordBytes = SafeUTF8.GetBytes(passwordText);
                    var salt = random.NextBytes(384 / 8);

                    using (var pbkdf2 = new PBKDF2(sha384Factory, passwordBytes, salt, 256 * 1000))
                        derivedKey = pbkdf2.GetBytes(384 / 8);


                    using (var hmac = sha384Factory())
                    {
                        hmac.Key = derivedKey;
                        hashedPassword = hmac.ComputeHash(passwordBytes).ToBase16();
                    }

                    passwordChanged.Password = hashedPassword;
                    passwordChanged.Salt = salt;
                    passwordChanged.forgotPasswordCode = null;
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, "Password changed successfully!");
                }
            }
            catch(Exception ex)
            {
                string s = ex.Message;
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error occurred, please try again!");
            }
        }
    }
}
