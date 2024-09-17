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
using Formatting = Newtonsoft.Json.Formatting;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using ApiAPSB.Models;
using System.Net.Http.Headers;
using System.Drawing;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Threading;
using System.Reflection;

namespace ApiAPSB.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class QAController : ApiController
    {
        [HttpGet]
        [Route("api/qa/sf/mor/{id}")]
        public async Task<IHttpActionResult> GetQaSF_MOR(int id)
        {
            var context = new Models.dbEntities();
            var form = await context.ViewQAMaintenances.Where(q => q.Id == id).FirstOrDefaultAsync();
            return Ok(form);

             

        }
        [HttpGet]
        [Route("api/qa/sf/csr/{id}")]
        public async Task<IHttpActionResult> GetQaSF_CSR(int id)
        {
            var context = new Models.dbEntities();
            var form = await context.ViewQACSRs.Where(q => q.Id == id).FirstOrDefaultAsync();
            return Ok(form);



        }

    }
}
