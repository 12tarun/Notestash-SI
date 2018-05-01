using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Notestash_SI.Controllers
{
    [Authorize]
    public class NotesController : ApiController
    {
        [EnableCors("*", "*", "*")]
        [HttpPost]
        public string Create()
        {
            var username = User.Identity.Name;

            return "xc";
        }
    }
}
