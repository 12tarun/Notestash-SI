using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Notestash_SI.Controllers
{
    [Authorize]
    public class DashBoardController : ApiController
    {
        [HttpGet]
        public string dashBoard()
        {
            var username = User.Identity.Name;
            return username.ToString();
        }
    }
}
