﻿using ApiFuel.Models;
using ApiFuel.ViewModels;
using Newtonsoft.Json;
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

namespace ApiFuel.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class FuelController : ApiController
    {
        [Route("api/fuel/report")]
        public async Task<IHttpActionResult> GetFuelReport(DateTime dfrom, DateTime dto)
        {
            try
            {
                dfrom = dfrom.Date;
                dto = dto.Date.AddDays(1);
                ppa_entities context = new ppa_entities();
                var query = from x in context.AppFuels
                            where x.STDDay >= dfrom && x.STDDay <= dto
                            select x;

                var result = await query.OrderBy(q => q.STD).ToListAsync();


                return Ok(result);
            }
            catch(Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                return Ok(msg);
            }


           
        }

         

        [Route("api/fuel/report/{register}")]
        public async Task<IHttpActionResult> GetFuelReportRegisterX(DateTime dfrom, DateTime dto,string register)
        {
            try
            {
                dfrom = dfrom.Date;
                dto = dto.Date.AddDays(1);
                ppa_entities context = new ppa_entities();
                var query = from x in context.AppFuels
                            where x.STDDay >= dfrom && x.STDDay <= dto && x.Register==register 
                            select x;

                var result = await query.OrderBy(q => q.STD).ToListAsync();


                return Ok(result);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                return Ok(msg);
            }



        }


        [Route("api/fuel/parse/route")]
        public async Task<IHttpActionResult> get_parse_route()
        {
            ppa_entities context = new ppa_entities();
            int flight_id = 595707;
            var ofp=await context.OFPImports.Where(q=>q.FlightId == flight_id).FirstOrDefaultAsync();
            var main_plan_text = ofp.JPlan;
            //var main_route= FlightPlanParser.ParseRoute(main_plan_text);
            List<_Waypoint> waypoints = JsonConvert.DeserializeObject<List<_Waypoint>>(main_plan_text);
            return Ok(waypoints);

        }


    }
}
