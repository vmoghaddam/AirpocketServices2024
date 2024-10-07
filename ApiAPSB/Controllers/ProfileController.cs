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
    public class ProfileController : ApiController
    {
        public class tel_dto
        {
            public string eid { get; set; }
            public string tel { get; set; }
        }
        [HttpPost]
        [Route("api/profile/telegram")]
        public async Task<IHttpActionResult> PostTel(tel_dto dto)
        {
            var context = new Models.dbEntities();

            try
            {
                var _eid = Convert.ToInt32(dto.eid);
                var pid = await context.ViewEmployees.Where(q => q.Id == _eid).Select(q => q.PersonId).FirstOrDefaultAsync();
                var person = await context.People.FirstOrDefaultAsync(q => q.Id == pid);
                person.Telegram = dto.tel;
                await context.SaveChangesAsync();
                return Ok(true);
            }
            catch(Exception ex)
            {
                return Ok(true);
            }


           


        }
    }
}
