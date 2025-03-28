using AeroTechApiWeather.Models;
using AeroTechApiWeather.ViewModels;
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

namespace AeroTechApiWeather.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class MainController : ApiController
    {


        //[Route("api/fuel/report")]
        //public async Task<IHttpActionResult> GetFuelReport(DateTime dfrom, DateTime dto)
        //{
        //    try
        //    {
        //        dfrom = dfrom.Date;
        //        dto = dto.Date.AddDays(1);
        //        ppa_entities context = new ppa_entities();
        //        var query = from x in context.AppFuels
        //                    where x.STDDay >= dfrom && x.STDDay <= dto
        //                    select x;

        //        var result = await query.OrderBy(q => q.STD).ToListAsync();


        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        var msg = ex.Message;
        //        if (ex.InnerException != null)
        //            msg += "   " + ex.InnerException.Message;
        //        return Ok(msg);
        //    }

        //}


        [Route("api/roster/fdp/nocrew/save")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> saveFDPNoCrew(dynamic dto)
        {
            var context = new Models.dbEntities();

            var userId = Convert.ToInt32(dto.userId);
            var flightId = Convert.ToInt32(dto.flightId);
            var code = Convert.ToString(dto.code);
            var fdp = new FDP()
            {
                IsTemplate = false,
                DutyType = 1165,
                CrewId = userId,
                GUID = Guid.NewGuid(),
                JobGroupId = RosterFDPDto.getRank(code),
                FirstFlightId = flightId,
                LastFlightId = flightId,

                Split = 0,



            };

            fdp.FDPItems.Add(new FDPItem()
            {
                FlightId = flightId,
                IsPositioning = false,
                IsSector = false,
                PositionId = RosterFDPDto.getRank(code),
                RosterPositionId = 1,

            });

            context.FDPs.Add(fdp);
            var saveResult = await context.SaveChangesAsync();




            return Ok(fdp.Id);
        }


    }
}
