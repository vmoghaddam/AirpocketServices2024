using ApiQATemp.Models;
using ApiQATemp.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
//using static ApiQATemp.Controllers.QaController;

namespace ApiQATemp.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class LogController : ApiController
    {
        ppa_entities context = new ppa_entities();

        [HttpGet]
        [Route("api/qa/log/main")]
        public async Task<DataResponse> GetFlightLogMain(DateTime df, DateTime dt)
        {
            try
            {
                df = df.Date;
                dt = dt.Date.AddDays(1);
                var result = await context.ViewFlightLogMains.Where(q => q.STDLocal >= df && q.STDLocal < dt).OrderBy(q => q.STDLocal).ToListAsync();
                return new DataResponse()
                {
                    Data = result,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   Inner: " + ex.InnerException.Message;
                return new DataResponse()
                {
                    Data = msg,
                    IsSuccess = false
                };
            }
        }


        [HttpGet]
        [Route("api/qa/log/detail/{fid}")]
        public async Task<DataResponse> GetFlightLogDetail(int fid)
        {
            try
            {

                var flight_log = await context.ViewFlightLogs.Where(q => q.FlightId == fid).OrderBy(q => q.DateCreate).ToListAsync();
                var crew_log = await context.ViewDutyLogs.Where(q => q.FlightId == fid).OrderBy(q => q.DateCreate).ToListAsync();
                return new DataResponse()
                {
                    Data = new { flight_log, crew_log },
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   Inner: " + ex.InnerException.Message;
                return new DataResponse()
                {
                    Data = msg,
                    IsSuccess = false
                };
            }
        }


        [HttpGet]
        [Route("api/qa/log/profile/")]
        public async Task<DataResponse> GetFlightLogProfile(int id/*,DateTime df, DateTime dt*/)
        {
            try
            {
                //df = df.Date;
                //dt = dt.Date.AddDays(1);
                var result = await context.ViewProfileLogs.Where(q => q.PersonId == id).OrderByDescending(q => q.DateCreate).ThenBy(q => q.CertificateType).ToListAsync();
                return new DataResponse()
                {
                    Data = result,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   Inner: " + ex.InnerException.Message;
                return new DataResponse()
                {
                    Data = msg,
                    IsSuccess = false
                };
            }
        }


        //[HttpGet]
        //[Route("api/qa/log/profile/duty/")]
        //public async Task<DataResponse> GetFlightLogProfileDuty(int id/*,DateTime df, DateTime dt*/)
        //{
        //    try
        //    {
        //        //df = df.Date;
        //        //dt = dt.Date.AddDays(1);
        //        var result = await context.ViewDutyLogGroups.Where(q => q.CrewId == id).OrderByDescending(q => q.DateCreate).ToListAsync();
        //        return new DataResponse()
        //        {
        //            Data = result,
        //            IsSuccess = true
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        var msg = ex.Message;
        //        if (ex.InnerException != null)
        //            msg += "   Inner: " + ex.InnerException.Message;
        //        return new DataResponse()
        //        {
        //            Data = msg,
        //            IsSuccess = false
        //        };
        //    }
        //}


        [HttpGet]
        [Route("api/qa/profiles/")]
        public async Task<DataResponse> GetProfiles(string grp)
        {
            try
            {
                //df = df.Date;
                //dt = dt.Date.AddDays(1);
                var query = from x in context.ViewProfiles
                            select x;
                if (grp != "-1")
                    query = query.Where(q => q.JobGroupRoot == grp);
                var result = await query.OrderBy(q => q.LastName).ThenBy(q => q.FirstName).ToListAsync();
                return new DataResponse()
                {
                    Data = result,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   Inner: " + ex.InnerException.Message;
                return new DataResponse()
                {
                    Data = msg,
                    IsSuccess = false
                };
            }
        }


        [HttpGet]
        [Route("api/qa/profiles/crew/")]
        public async Task<DataResponse> GetProfilesCrew(string grp)
        {
            try
            {
                //df = df.Date;
                //dt = dt.Date.AddDays(1);
                var grps = new List<string>() { "TRE", "TRI", "P1", "P2" };
                var query = from x in context.ViewProfiles
                            where grps.Contains(x.JobGroup)
                            select x;
                if (grp != "-1")
                    query = query.Where(q => q.JobGroup == grp);
                var result = await query.OrderBy(q => q.LastName).ThenBy(q => q.FirstName).ToListAsync();
                return new DataResponse()
                {
                    Data = result,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   Inner: " + ex.InnerException.Message;
                return new DataResponse()
                {
                    Data = msg,
                    IsSuccess = false
                };
            }
        }


        public class DataResponse
        {
            public bool IsSuccess { get; set; }
            public object Data { get; set; }
            public List<string> Errors { get; set; }
        }


    }
}
