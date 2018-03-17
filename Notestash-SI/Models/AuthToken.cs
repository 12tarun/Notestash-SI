using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Notestash_SI.Models
{
    public class AuthToken
    {
        public int user_id { get; set; }
        public string  token { get; set; }
        public DateTime created_at { get; set; }
        public DateTime expired_at { get; set; }
    }
}