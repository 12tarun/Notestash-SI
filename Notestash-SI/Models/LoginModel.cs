using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Security.Claims;
using NotestashUserDataAccess;
using SecurityDriven.Inferno.Kdf;
using PBKDF2 = SecurityDriven.Inferno.Kdf.PBKDF2;
using SecurityDriven.Inferno.Extensions;
using static SecurityDriven.Inferno.SuiteB;
using static SecurityDriven.Inferno.Utils;
using System.Web.Security;
using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace Notestash_SI.Models
{
    public class LoginModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

        public string Check(LoginModel objUser)
        {
            try
            {
                using (Notestash_DatabaseEntities db = new Notestash_DatabaseEntities())
                {
                    var user = db.tblUsers.FirstOrDefault(e => e.Email.Equals(objUser.Email));

                    var sha384Factory = HmacFactory;

                    byte[] derivedKey;
                    string hashedPassword = null;
                    string suppliedPassword = objUser.Password;

                    byte[] passwordBytes = SafeUTF8.GetBytes(suppliedPassword);

                    using (var pbkdf2 = new PBKDF2(sha384Factory, passwordBytes, user.Salt, 256 * 1000))
                        derivedKey = pbkdf2.GetBytes(384 / 8);


                    using (var hmac = sha384Factory())
                    {
                        hmac.Key = derivedKey;
                        hashedPassword = hmac.ComputeHash(passwordBytes).ToBase16();
                    }



                    var userCredentials =
                        db.tblUsers.FirstOrDefault(e => e.Email.Equals(objUser.Email) && e.Password.Equals(hashedPassword));

                    if (userCredentials != null)
                    {
                      //  FormsAuthentication.SetAuthCookie(objUser.Email, false);
                        //string accessToken = "";
                        //string time = DateTime.Now.ToString("h:mm:ss tt");
                        //string token = objUser.Email + hashedPassword + time;
                        //using (MD5 md5Hash = MD5.Create())
                        //{
                        //    byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(token));
                        //    StringBuilder sBuilder = new StringBuilder();
                        //    for (int i = 0; i < data.Length; i++)
                        //    {
                        //        sBuilder.Append(data[i].ToString("x2"));
                        //    }
                        //    accessToken = sBuilder.ToString();
                        //}

                        //DateTime createdToken = DateTime.Now;
                        //DateTime expiredToken = createdToken.AddDays(10);
                        string token = createToken(objUser.Email);
                        //return the token
                        return token;
                    }
                    else
                    {
                        return "invalid";
                    }
                }
            }
            catch (Exception ex)
            {
                string message = ex.ToString();
                return "error";
            }
        }

        private string createToken(string username)
        {
            //Set issued at date
            DateTime issuedAt = DateTime.UtcNow;
            //set the time when it expires
            DateTime expires = DateTime.UtcNow.AddDays(7);

            //http://stackoverflow.com/questions/18223868/how-to-encrypt-jwt-security-token
            var tokenHandler = new JwtSecurityTokenHandler();

            //create a identity and add claims to the user which we want to log in
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, username)
            });

            const string sec = "401b09eab3c013d4ca54922bb802bec8fd5318192b0a75f201d8b3727429090fb337591abd3e44453b954555b7a0812e1081c39b740293f765eae731f5a65ed1";
            var now = DateTime.UtcNow;
            var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(sec));
            var signingCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256Signature);


            //create the jwt
            var token =
                (JwtSecurityToken)
                    tokenHandler.CreateJwtSecurityToken(issuer: "http://localhost:17285", audience: "http://localhost:17285",
                        subject: claimsIdentity, notBefore: issuedAt, expires: expires, signingCredentials: signingCredentials);
            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }
    }
}