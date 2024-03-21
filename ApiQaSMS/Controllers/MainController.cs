using ApiQaSMS.Models;
using ApiQaSMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ApiQaSMS.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class MainController : ApiController
    {
        [Route("qasms/event/categories/{pid}")]
        public async Task<IHttpActionResult> get_event_categories(int pid)
        {
           
            ppa_entities context = new ppa_entities();
            var query = from x in context.view_qa_event_category select x;
            if (pid == 0)
                query = query.Where(q => q.parent_id == null);
            else if (pid > 0)
                query = query.Where(q => q.parent_id == pid);

            var result = await query.OrderBy(q => q.full_code).ToListAsync();
             

            return Ok(result);
        }



    }
}
