﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Notestash_SI.Models;
using System.Net.Mail;
using NotestashUserDataAccess;
using System.Web.Http.Cors;

namespace Notestash_SI.Controllers
{
    public class UserController : ApiController
    {
        [EnableCors("*", "*", "*")]
        [HttpPost]
        public HttpResponseMessage Create(UserModel objUser)
        {
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Model State Invalid");

            UserModel ob = new UserModel();
            // we can use out for exceptions.
            string created = ob.Create(objUser);

            if (created == "exists")
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Email Already Exists!");
            }
            else if (created == "error")
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Something Went Wrong!");
            }
            else
            {
                int i = created.IndexOf(" ");
                int sizeOfCode = created.Length - i - 1;
                string passedEmail = created.Substring(0, i);
                string passedCode = created.Substring((i + 1), sizeOfCode);
                SendVerificationLink(passedEmail, passedCode);
                return Request.CreateResponse(HttpStatusCode.OK, "Successful Registration! Activation Code has been sent to your email id. Link expires in 1 day.");
            }
        }

        // Send email verification link.
        [NonAction]
        public void SendVerificationLink(string Email, string ActivationCode)
        {
            var verifyUrl = "/api/User/VerifyAccount/" + ActivationCode;
            Uri link = new Uri(Request.RequestUri.AbsoluteUri.Replace(Request.RequestUri.PathAndQuery, verifyUrl));
            var fromEmail = new MailAddress("suryakant.rocky@gmail.com", "Notestash");
            var toEmail = new MailAddress(Email);
            var fromEmailPassword = "suryasharma";
            string subject = "Change Password";
            string body = "<br/><br/>Please click on the link below to activate your Notestash-Admin account." + "<br/><br/><a href='" + link + "'>" + link + "</a>";

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

        // GET: Verify account on clicking on activation code sent in email.
        [EnableCors("*", "*", "*")]
        [HttpGet]
        [Route("api/User/VerifyAccount/{id}")]
        public HttpResponseMessage VerifyAccount(string id)
        {
            using (Notestash_DatabaseEntities db = new Notestash_DatabaseEntities())
            {
                var activate = db.tblUsers.Where(e => e.ActivationCode == new Guid(id)).FirstOrDefault();
                if(activate == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Account deleted! Create Again!");
                }
                else
                {
                    DateTime expire = activate.Created_at.Value.AddDays(1);
                    DateTime present = DateTime.Now;
                    if (present >= expire)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Link Expired! Register Again!");
                    }
                    else
                    {
                        activate.IsEmailVerified = 1;
                        db.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK, "Your Notestash account is activated!");
                    }
                }
            }
        }
    }
}
