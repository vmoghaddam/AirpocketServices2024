using AirpocketAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

using System.Data.Entity;
using System.Data.Entity.Infrastructure;


using System.Web.Http.Description;

using System.Data.Entity.Validation;

using System.Web.Http.ModelBinding;

using System.Text;
using System.Configuration;
using Newtonsoft.Json;
using System.Web.Http.Cors;
using System.IO;
using System.Xml;
using System.Web;
using System.Text.RegularExpressions;

namespace AirpocketAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class FileUploaderController : ApiController
    {


        [HttpGet]
        [Route("api/get/test")]
        public IHttpActionResult test()
        {
            string test = "test test test";
            return Ok(test);
        }

    }

}
