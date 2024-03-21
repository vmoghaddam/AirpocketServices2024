using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ApiMap.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]

    public class ValuesController : ApiController
    {
        [Route("api/windy/test")]
        [AcceptVerbs("GET")]
        public IHttpActionResult Gettest()
        {

            //HttpContext.Current.Response.Headers.Add("MaxRecords", "1000");
            return Ok(true);

        }
    }
}
