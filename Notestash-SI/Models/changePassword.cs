using NotestashUserDataAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Notestash_SI.Models
{
    public class changePassword
    {
        [Required]
        [DataType(DataType.Password)]
        [StringLength(15, MinimumLength = 6, ErrorMessage = "Password should be between 6 to 15 characters!")]
        public string newPassword { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [StringLength(15, MinimumLength = 6, ErrorMessage = "Password should be between 6 to 15 characters!")]
        [Compare("Password", ErrorMessage = "Passwords do not match!")]
        public string confirmNewPassword { get; set; }
    }
}