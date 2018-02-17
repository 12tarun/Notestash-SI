using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;

namespace Notestash_SI.Controllers
{
    public class AuthenticationController : ApiController
    {
        public HttpResponseMessage Post()
        {
            // ... do the job

            // now redirect
            var response = Request.CreateResponse(HttpStatusCode.Moved);
            response.Headers.Location = new Uri("http://localhost:17285/api/Account/ExternalLogin?provider=Google&response_type=token&client_id=self&redirect_uri=http%3A%2F%2Flocalhost%3A17285%2FLogin.html%2F&state=H7TaCf8vAz8qR10ypr9HBdkDZSaVXiIlB9VqnzC6Ihs1");
            return response;
        }
    }
}
