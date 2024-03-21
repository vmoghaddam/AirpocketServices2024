using ApiCAO.Models;
using ApiCAO.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ApiCAO.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ReportController : ApiController
    {

        [Route("api/cao/report/forma/{year}/{month}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetFormA(int year,int month)
        {
            ppa_entities context = new ppa_entities();
            var forma = context.RPTFormAMonthlies.Where(q => q.Year == year && q.Month == month).FirstOrDefault();
            return Ok(forma);
        }
        [Route("api/cao/report/forma/year/{year}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetFormAsByYear(int year )
        {
            ppa_entities context = new ppa_entities();
            var forma = context.RPTFormAMonthlies.Where(q => q.Year == year  ).OrderBy(q=>q.Month).ToList();
            return Ok(forma);
        }



    }
}
