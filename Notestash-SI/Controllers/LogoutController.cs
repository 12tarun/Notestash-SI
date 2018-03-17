using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Security;

namespace Notestash_SI.Controllers
{
    public class LogoutController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage SignOut()
        {
            FormsAuthentication.SignOut();
            return Request.CreateResponse(HttpStatusCode.OK,"Logged Out!");
        }
    }
}
