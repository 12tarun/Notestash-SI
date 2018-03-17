using System;
using System.Collections.Generic;
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
            return "HEY!";
        }
    }
}
