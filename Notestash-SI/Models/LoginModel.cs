using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NotestashUserDataAccess;
using SecurityDriven.Inferno.Kdf;
using PBKDF2 = SecurityDriven.Inferno.Kdf.PBKDF2;
using SecurityDriven.Inferno.Extensions;
using static SecurityDriven.Inferno.SuiteB;
using static SecurityDriven.Inferno.Utils;

namespace Notestash_SI.Models
{
    public class LoginModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

        public bool Check(LoginModel objUser)
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
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}