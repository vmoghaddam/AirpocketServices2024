using ApiMnt.Models;
using ApiMnt.ViewModels;
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

namespace ApiMnt.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class FuelController : ApiController
    {
        [Route("api/fuel/report")]
        public async Task<IHttpActionResult> GetFuelReport(DateTime dfrom, DateTime dto)
        {
            dfrom = dfrom.Date;
            dto = dto.Date.AddDays(1);
            ppa_entities context = new ppa_entities();



            return Ok(true);
        }

    }
}
