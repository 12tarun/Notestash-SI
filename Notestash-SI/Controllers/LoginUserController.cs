//using System;
//using System.Collections.Generic;
//using System.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
//using System.IO;
//using System.Net;
//using System.Net.Http;
//using System.Security.Claims;
//using System.Web;
//using System.Web.Http;
//using Notestash_SI.Models;
//using NotestashUserDataAccess;
//using System.Linq;
//using System.Web.Mvc;

//namespace Notestash_SI.Controllers
//{
//    public class LoginUserController : ApiController
//    {
//        [HttpPost]
//        public ActionResult Authenticate([FromBody] LoginModel login)
//        {
//            using (Notestash_DatabaseEntities db = new Notestash_DatabaseEntities())
//            {
//                //   var loginResponse = new LoginResponse { };
//                LoginModel loginrequest = new LoginModel { };
//                loginrequest.Email = login.Email.ToLower();
//                loginrequest.Password = login.Password;

//            //    IHttpActionResult response;
//                HttpResponseMessage responseMsg = new HttpResponseMessage();

//                var user = db.tblUsers.FirstOrDefault(e => e.Email.Equals(loginrequest.Email));
//                //  bool isUsernamePasswordValid = false;

//            //    if (user != null)
//            //     //   isUsernamePasswordValid = loginrequest.Password == "admin" ? true : false;
//            //    // if credentials are valid
//            //  //  if (isUsernamePasswordValid)
//            //    {
//            //        string token = createToken(loginrequest.Email);
//            //        //return the token
//            //        return Ok<string>(token);
//            //    }
//            //    else
//            //    {
//            //        // if credentials are not valid send unauthorized status code in response
//            //        //     loginResponse.responseMsg.StatusCode = HttpStatusCode.Unauthorized;
//            //        //     response = ResponseMessage(loginResponse.responseMsg);
                    
//            //        return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Unauthorized Access!"); 
//            //    }
//            //}
//        }

//        private string createToken(string username)
//        {
//            //Set issued at date
//            DateTime issuedAt = DateTime.UtcNow;
//            //set the time when it expires
//            DateTime expires = DateTime.UtcNow.AddDays(7);

//            //http://stackoverflow.com/questions/18223868/how-to-encrypt-jwt-security-token
//            var tokenHandler = new JwtSecurityTokenHandler();

//            //create a identity and add claims to the user which we want to log in
//            ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[]
//            {
//                new Claim(ClaimTypes.Name, username)
//            });

//            const string sec = "401b09eab3c013d4ca54922bb802bec8fd5318192b0a75f201d8b3727429090fb337591abd3e44453b954555b7a0812e1081c39b740293f765eae731f5a65ed1";
//            var now = DateTime.UtcNow;
//            var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(sec));
//            var signingCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256Signature);


//            //create the jwt
//            var token =
//                (JwtSecurityToken)
//                    tokenHandler.CreateJwtSecurityToken(issuer: "http://localhost:17285", audience: "http://localhost:17285",
//                        subject: claimsIdentity, notBefore: issuedAt, expires: expires, signingCredentials: signingCredentials);
//            var tokenString = tokenHandler.WriteToken(token);

//            return tokenString;
//        }
//    }
//}
