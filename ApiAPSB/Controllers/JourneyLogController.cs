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
using Spire.Xls;

namespace ApiAPSB.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class JourneyLogController : ApiController
    {
        [Route("api/jl/{fid}")]

        //nookp
        public async Task<IHttpActionResult> GetJourneyLog(int fid)
        {
            //nooz 
            //this.context.Database.CommandTimeout = 160;

            var context = new Models.dbEntities();
            var appleg = context.AppLegs.FirstOrDefault(q => q.FlightId == fid);
            var pid = appleg.PICId;
            var appFlight = context.AppCrewFlightJLs.Where(q => q.FlightId == fid && q.CrewId == pid).FirstOrDefault();
            var crewlegs = context.AppCrewFlightJLs.Where(q => q.FDPId == appFlight.FDPId).ToList();
            var clegs = crewlegs.Select(q => (int)q.FlightId).ToList();
            var legs = context.AppLegJLs.Where(q => clegs.Contains(q.FlightId)).OrderBy(q => q.STD).ToList();
            var fdp = context.ViewFDPRests.FirstOrDefault(q => q.Id == appFlight.FDPId);
            var asr = legs.FirstOrDefault(q => q.AttASR == true) != null;
            var vr = legs.FirstOrDefault(q => q.AttVoyageReport == true) != null;
            var pos1 = legs.FirstOrDefault(q => q.AttRepositioning1 == true) != null;
            var pos2 = legs.FirstOrDefault(q => q.AttRepositioning2 == true) != null;


            // var flight = legs.Sum(q => (((DateTime)(q.Landing!=null?q.Landing:q.STA) - (DateTime)(q.TakeOff!=null?q.TakeOff:q.STD))).TotalMinutes);
            // var block = legs.Sum(q => (((DateTime)(q.BlockOn!=null?q.BlockOn:q.STA) - (DateTime)(q.BlockOff!=null?q.BlockOff:q.STD))).TotalMinutes);
            var flight = legs.Sum(q => (q.Landing != null && q.TakeOff != null) ? ((DateTime)q.Landing - (DateTime)q.TakeOff).TotalMinutes : 0);
            var block = legs.Sum(q => (q.BlockOn != null && q.BlockOff != null) ? ((DateTime)q.BlockOn - (DateTime)q.BlockOff).TotalMinutes : 0);
            var fids = legs.Select(q => (Nullable<int>)q.FlightId).ToList();
            var _crews2 = (from x in context.ViewFlightCrewNews
                               //where x.FlightId == flightId

                           where fids.Contains(x.FlightId) //&& x.IsPositioning != true
                           orderby x.IsPositioning, x.GroupOrder

                           select new CLJLData()
                           {
                               CrewId = x.CrewId,
                               IsPositioning = x.IsPositioning,
                               PositionId = x.PositionId,
                               Position = x.Position,
                               Name = x.Name,
                               GroupId = x.GroupId,
                               JobGroup = x.JobGroup,
                               JobGroupCode = x.JobGroupCode,
                               GroupOrder = x.GroupOrder,
                               IsCockpit = x.IsCockpit,
                               FlightId = x.FlightId,

                           }).ToList();

            var _gcrews = (from x in _crews2
                           group x by new
                           {
                               x.CrewId,
                               x.IsPositioning,
                               x.PositionId,
                               x.Position,
                               x.Name,
                               x.GroupId,
                               x.JobGroup,
                               x.JobGroupCode,
                               x.GroupOrder,
                               x.IsCockpit,
                           } into grp
                           select grp).ToList();
            var query = (from x in _gcrews
                         let xfids = x.Select(q => Convert.ToInt32(q.FlightId)).ToList()
                         select new CLJLData()
                         {
                             CrewId = x.Key.CrewId,
                             IsPositioning = x.Key.IsPositioning,
                             PositionId = x.Key.PositionId,
                             Position = x.Key.Position,
                             Name = x.Key.Name,
                             GroupId = x.Key.GroupId,
                             JobGroup = x.Key.JobGroup,
                             JobGroupCode = x.Key.JobGroupCode,
                             GroupOrder = x.Key.GroupOrder,
                             IsCockpit = x.Key.IsCockpit,
                             Legs = legs.Where(q => xfids.Contains((int)q.FlightId)).OrderBy(q => q.STD).Select(q => q.FlightNumber).Distinct().ToList(),
                             LegsStr = string.Join("-", legs.Where(q => xfids.Contains((int)q.FlightId)).OrderBy(q => q.STD).Select(q => q.FlightNumber).Distinct().ToList()),

                         }).ToList();


            foreach (var x in query)
            {
                if (x.Legs.Count == fids.Count)
                    x.LegsStr = "";
            }

            var result = new
            {
                legs,
                AcType = legs.First().AircraftType,
                Reg = legs.First().Register,
                Date = legs.First().STDDay,
                STD = legs.First().STD,
                fdp.ReportingTime,
                fdp.DutyEnd,
                FDPId = fdp.Id,
                // PIC = legs.First().PIC,
                PIC = appleg.PIC,

                MaxFDP = fdp.MaxFDPExtended,
                fdp.FDP,
                fdp.Duty,
                flight,
                block,
                asr,
                vr,
                pos1,
                pos2,
                crew = query


            };





            // return result.OrderBy(q => q.STD);
            return Ok(result);

        }



        [Route("api/jl/v2/{fid}")]

        //nookp
        //karun,
        public async Task<IHttpActionResult> GetJourneyLogV2(int fid)
        {
            //nooz 
            //this.context.Database.CommandTimeout = 160;

            var context = new Models.dbEntities();
            var appleg = context.AppLegs.FirstOrDefault(q => q.FlightId == fid);
            var pid = appleg.PICId;
            var appFlight = context.AppCrewFlightJLs.Where(q => q.FlightId == fid && q.CrewId == pid).FirstOrDefault();
            var crewlegs = context.AppCrewFlightJLs.Where(q => q.FDPId == appFlight.FDPId).ToList();
            var clegs = crewlegs.Select(q => (int)q.FlightId).ToList();
            var legs = context.AppLegJLs.Where(q => clegs.Contains(q.FlightId)).OrderBy(q => q.STD).ToList();
            var fdp = context.ViewFDPRests.FirstOrDefault(q => q.Id == appFlight.FDPId);
            foreach (var x in legs)
            {
                if (x.BlockOn != null)
                {
                    x.RemDuty = Convert.ToInt32(Math.Round((double)fdp.MaxFDPExtended)) - Convert.ToInt32(Math.Round(((DateTime)x.BlockOn - (DateTime)fdp.ReportingTime).TotalMinutes));
                    x.ElapsedDuty = Convert.ToInt32(Math.Round(((DateTime)x.BlockOn - (DateTime)fdp.ReportingTime).TotalMinutes));
                }
                else
                {
                    x.RemDuty = null;
                    x.ElapsedDuty = null;
                }
            }
            //var over = legs.Last().BlockOn!=null?
            //    Convert.ToInt32(Math.Round((double)fdp.MaxFDPExtended)) - Convert.ToInt32(Math.Round(((DateTime)legs.Last().BlockOn - (DateTime)fdp.ReportingTime).TotalMinutes))
            //    : 0;
            var over_leg = legs.Where(q => q.RemDuty < 0).OrderBy(q => q.RemDuty).FirstOrDefault();
            var over = over_leg == null ? 0 : (int)over_leg.RemDuty;
            if (over > 0)
                over = 0;
            over = Math.Abs(over);

            var elapsed_leg = legs.Where(q => q.ElapsedDuty > 0).OrderByDescending(q => q.ElapsedDuty).FirstOrDefault();



            var actual_fdp = elapsed_leg == null ? 0 : elapsed_leg.ElapsedDuty;
            //var actual_duty = Convert.ToInt32(Math.Round(((DateTime)legs.Last().BlockOn - (DateTime)fdp.ReportingTime).TotalMinutes)) + 30;



            var asr = legs.FirstOrDefault(q => q.AttASR == true) != null;
            var vr = legs.FirstOrDefault(q => q.AttVoyageReport == true) != null;
            var pos1 = legs.FirstOrDefault(q => q.AttRepositioning1 == true) != null;
            var pos2 = legs.FirstOrDefault(q => q.AttRepositioning2 == true) != null;


            // var flight = legs.Sum(q => (((DateTime)(q.Landing!=null?q.Landing:q.STA) - (DateTime)(q.TakeOff!=null?q.TakeOff:q.STD))).TotalMinutes);
            // var block = legs.Sum(q => (((DateTime)(q.BlockOn!=null?q.BlockOn:q.STA) - (DateTime)(q.BlockOff!=null?q.BlockOff:q.STD))).TotalMinutes);
            var flight = legs.Sum(q => (q.Landing != null && q.TakeOff != null) ? ((DateTime)q.Landing - (DateTime)q.TakeOff).TotalMinutes : 0);
            var block = legs.Sum(q => (q.BlockOn != null && q.BlockOff != null) ? ((DateTime)q.BlockOn - (DateTime)q.BlockOff).TotalMinutes : 0);
            var night = legs.Sum(q => q.NightTime);
            var fids = legs.Select(q => (Nullable<int>)q.FlightId).ToList();
            var _crews2 = (from x in context.ViewFlightCrewNews
                               //where x.FlightId == flightId

                           where fids.Contains(x.FlightId) //&& x.IsPositioning != true
                           orderby x.IsPositioning, x.GroupOrder

                           select new CLJLData()
                           {
                               CrewId = x.CrewId,
                               IsPositioning = x.IsPositioning,
                               PositionId = x.PositionId,
                               Position = x.Position,
                               Name = x.Name,
                               GroupId = x.GroupId,
                               JobGroup = x.JobGroup,
                               JobGroupCode = x.JobGroupCode,
                               GroupOrder = x.GroupOrder,
                               IsCockpit = x.IsCockpit,
                               FlightId = x.FlightId,


                           }).ToList();

            var _gcrews = (from x in _crews2
                           group x by new
                           {
                               x.CrewId,
                               x.IsPositioning,
                               x.PositionId,
                               x.Position,
                               x.Name,
                               x.GroupId,
                               x.JobGroup,
                               x.JobGroupCode,
                               x.GroupOrder,
                               x.IsCockpit,
                           } into grp
                           select grp).ToList();
            var query = (from x in _gcrews
                         let xfids = x.Select(q => Convert.ToInt32(q.FlightId)).ToList()
                         select new CLJLData()
                         {
                             CrewId = x.Key.CrewId,
                             IsPositioning = x.Key.IsPositioning,
                             PositionId = x.Key.PositionId,
                             Position = x.Key.Position,
                             Name = x.Key.Name,
                             GroupId = x.Key.GroupId,
                             JobGroup = x.Key.JobGroup,
                             JobGroupCode = x.Key.JobGroupCode,
                             GroupOrder = x.Key.GroupOrder,
                             IsCockpit = x.Key.IsCockpit,
                             Legs = legs.Where(q => xfids.Contains((int)q.FlightId)).OrderBy(q => q.STD).Select(q => q.FlightNumber).Distinct().ToList(),
                             LegsStr = string.Join("-", legs.Where(q => xfids.Contains((int)q.FlightId)).OrderBy(q => q.STD).Select(q => q.FlightNumber).Distinct().ToList()),
                             TotalBlockTime = legs.Where(q => xfids.Contains((int)q.FlightId)).Sum(q => q.BlockOff != null && q.BlockOn != null
                                  ? ((DateTime)q.BlockOn - (DateTime)q.BlockOff).TotalMinutes
                                  : (Nullable<double>)null),

                         }).ToList();


            foreach (var x in query)
            {
                if (x.Legs.Count == fids.Count)
                    x.LegsStr = "";
            }

            var result = new
            {
                legs,
                AcType = legs.First().AircraftType,
                Reg = legs.First().Register,
                Date = legs.First().STDDay,
                STD = legs.First().STD,
                fdp.ReportingTime,
                fdp.DutyEnd,
                FDPId = fdp.Id,
                // PIC = legs.First().PIC,
                PIC = appleg.PIC,

                MaxFDP = fdp.MaxFDPExtended,
                fdp.FDP,
                fdp.Duty,
                flight,
                block,
                night,
                over,
                actual_fdp,
                asr,
                vr,
                pos1,
                pos2,
                crew = query


            };





            // return result.OrderBy(q => q.STD);
            return Ok(result);

        }



        public string format_to_time(double? v)
        {
            if (v == null)
                return "";
            var x = Convert.ToInt32(Math.Round((double)v));
            var sgn = 1;
            if (x < 0)
                sgn = -1;
            x = Math.Abs(x);
            var hh = x / 60;
            var mm = x % 60;
            return hh.ToString().PadLeft(2, '0') + ":" + mm.ToString().PadLeft(2, '0');
        }

        public string format_to_time(int? v)
        {
            if (v == null)
                return "";
            var x = (int)v;
            var sgn = 1;
            if (x < 0)
                sgn = -1;
            x = Math.Abs(x);
            var hh = x / 60;
            var mm = x % 60;
            return hh.ToString().PadLeft(2, '0') + ":" + mm.ToString().PadLeft(2, '0');
        }


        public string get_dh(List<CLJLData> ds, int flt)
        {
            var rec = ds.Where(q => q.FlightId == flt && q.IsPositioning == true).FirstOrDefault();
            if (rec != null)
                return " dh";
            return "";
        }

        [Route("api/jl/v2/xls/{fid}")]

        //nookp
        //karun,
        public HttpResponseMessage GetJourneyLogV2XLS(int fid)
        {
            string err_code = "1";
            try
            {
                //nooz 
                //this.context.Database.CommandTimeout = 160;

                var context = new Models.dbEntities();
                var appleg = context.AppLegs.FirstOrDefault(q => q.FlightId == fid);
                var pid = appleg.PICId;
                var appFlight = context.AppCrewFlightJLs.Where(q => q.FlightId == fid && q.CrewId == pid).FirstOrDefault();
                var crewlegs = context.AppCrewFlightJLs.Where(q => q.FDPId == appFlight.FDPId).ToList();
                var clegs = crewlegs.Select(q => (int)q.FlightId).ToList();
                var legs = context.AppLegJLs.Where(q => clegs.Contains(q.FlightId)).OrderBy(q => q.STD).ToList();
                var fdp = context.ViewFDPRests.FirstOrDefault(q => q.Id == appFlight.FDPId);
                foreach (var x in legs)
                {
                    if (x.BlockOn != null)
                    {
                        x.RemDuty = Convert.ToInt32(Math.Round((double)fdp.MaxFDPExtended)) - Convert.ToInt32(Math.Round(((DateTime)x.BlockOn - (DateTime)fdp.ReportingTime).TotalMinutes));
                        x.ElapsedDuty = Convert.ToInt32(Math.Round(((DateTime)x.BlockOn - (DateTime)fdp.ReportingTime).TotalMinutes));
                    }
                    else
                    {
                        x.RemDuty = null;
                        x.ElapsedDuty = null;
                    }
                    if (x.BlockOff == null || x.BlockOn == null)
                        x.BlockTime = null;
                    if (x.TakeOff == null || x.Landing == null)
                        x.FlightTime = null;
                }
                //var over = legs.Last().BlockOn!=null?
                //    Convert.ToInt32(Math.Round((double)fdp.MaxFDPExtended)) - Convert.ToInt32(Math.Round(((DateTime)legs.Last().BlockOn - (DateTime)fdp.ReportingTime).TotalMinutes))
                //    : 0;
                var over_leg = legs.Where(q => q.RemDuty < 0).OrderBy(q => q.RemDuty).FirstOrDefault();
                var over = over_leg == null ? 0 : (int)over_leg.RemDuty;
                if (over > 0)
                    over = 0;
                over = Math.Abs(over);

                var elapsed_leg = legs.Where(q => q.ElapsedDuty > 0).OrderByDescending(q => q.ElapsedDuty).FirstOrDefault();



                var actual_fdp = elapsed_leg == null ? 0 : elapsed_leg.ElapsedDuty;
                //var actual_duty = Convert.ToInt32(Math.Round(((DateTime)legs.Last().BlockOn - (DateTime)fdp.ReportingTime).TotalMinutes)) + 30;



                var asr = legs.FirstOrDefault(q => q.AttASR == true) != null;
                var vr = legs.FirstOrDefault(q => q.AttVoyageReport == true) != null;
                var pos1 = legs.FirstOrDefault(q => q.AttRepositioning1 == true) != null;
                var pos2 = legs.FirstOrDefault(q => q.AttRepositioning2 == true) != null;


                // var flight = legs.Sum(q => (((DateTime)(q.Landing!=null?q.Landing:q.STA) - (DateTime)(q.TakeOff!=null?q.TakeOff:q.STD))).TotalMinutes);
                // var block = legs.Sum(q => (((DateTime)(q.BlockOn!=null?q.BlockOn:q.STA) - (DateTime)(q.BlockOff!=null?q.BlockOff:q.STD))).TotalMinutes);
                var flight = legs.Sum(q => (q.Landing != null && q.TakeOff != null) ? ((DateTime)q.Landing - (DateTime)q.TakeOff).TotalMinutes : 0);
                var block = legs.Sum(q => (q.BlockOn != null && q.BlockOff != null) ? ((DateTime)q.BlockOn - (DateTime)q.BlockOff).TotalMinutes : 0);
                var night = legs.Sum(q => q.NightTime);
                var fids = legs.Select(q => (Nullable<int>)q.FlightId).ToList();
                var _crews2 = (from x in context.ViewFlightCrewNews
                                   //where x.FlightId == flightId

                               where fids.Contains(x.FlightId) //&& x.IsPositioning != true
                               orderby x.IsPositioning, x.GroupOrder

                               select new CLJLData()
                               {
                                   CrewId = x.CrewId,
                                   IsPositioning = x.IsPositioning,
                                   PositionId = x.PositionId,
                                   Position = x.Position,
                                   Name = x.Name,
                                   GroupId = x.GroupId,
                                   JobGroup = x.JobGroup,
                                   JobGroupCode = x.JobGroupCode,
                                   GroupOrder = x.GroupOrder,
                                   IsCockpit = x.IsCockpit,
                                   FlightId = x.FlightId,


                               }).ToList();

                var _gcrews = (from x in _crews2
                               group x by new
                               {
                                   x.CrewId,
                                   x.IsPositioning,
                                   x.PositionId,
                                   x.Position,
                                   x.Name,
                                   x.GroupId,
                                   x.JobGroup,
                                   x.JobGroupCode,
                                   x.GroupOrder,
                                   x.IsCockpit,
                               } into grp
                               select grp).ToList();
                var query = (from x in _gcrews
                             let xfids = x.Select(q => Convert.ToInt32(q.FlightId)).ToList()
                             //let _items=x.ToList()
                             select new CLJLData()
                             {
                                 CrewId = x.Key.CrewId,
                                 IsPositioning = x.Key.IsPositioning,
                                 PositionId = x.Key.PositionId,
                                 Position = x.Key.Position,
                                 Name = x.Key.Name,
                                 GroupId = x.Key.GroupId,
                                 JobGroup = x.Key.JobGroup,
                                 JobGroupCode = x.Key.JobGroupCode,
                                 GroupOrder = x.Key.GroupOrder,
                                 IsCockpit = x.Key.IsCockpit,
                                 Legs = legs.Where(q => xfids.Contains((int)q.FlightId)).OrderBy(q => q.STD).Select(q => q.FlightNumber).Distinct().ToList(),

                                 LegsStr = string.Join("-", legs.Where(q => xfids.Contains((int)q.FlightId)).OrderBy(q => q.STD)
                                 .Select(q => q.FlightNumber + get_dh(x.ToList(), q.FlightId))
                                 .Distinct().ToList()),
                                 TotalBlockTime = legs.Where(q => xfids.Contains((int)q.FlightId)).Sum(q => q.BlockOff != null && q.BlockOn != null
                                      ? ((DateTime)q.BlockOn - (DateTime)q.BlockOff).TotalMinutes
                                      : (Nullable<double>)null),

                             }).ToList();


                foreach (var x in query)
                {
                    if (x.Legs.Count == fids.Count)
                        x.LegsStr = "";
                }

                var result = new
                {
                    legs,
                    AcType = legs.First().AircraftType,
                    Reg = legs.First().Register,
                    Date = legs.First().STDDay,
                    STD = legs.First().STD,
                    fdp.ReportingTime,
                    fdp.DutyEnd,
                    FDPId = fdp.Id,
                    // PIC = legs.First().PIC,
                    PIC = appleg.PIC,

                    MaxFDP = fdp.MaxFDPExtended,
                    fdp.FDP,
                    fdp.Duty,
                    flight,
                    block,
                    night,
                    over,
                    actual_fdp,
                    asr,
                    vr,
                    pos1,
                    pos2,
                    crew = query


                };


                string LData = "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiIHN0YW5kYWxvbmU9InllcyI/Pgo8TGljZW5zZSBLZXk9ImNyOERaN1hKMkR5MUs2UUJBTkRPSVRLdlpjTzZkelVod2lsSHBnVlluQ3k0cXlHV2V6TFZubFJGeFAxNU1mSWZnUmdNWm1XaEdOQWRFNFRqZWZnQ1ovbFR2b1BkSXRIbDZXdDVBNWk1TVhFbnFkQnVPMUthRnovRFFzYUdWTGhzdjlySG1ybnRxSElFRGxJeGRxYUpNcGtLb0Frd1A3d1N6T01KMVkrbUNmVTVVRmV6REwvTjd1enJ4M0Y0d2I1SGErd0E2VFQ5VFJ3SzAzejlFS01aRmwzU1lSL3o0YVU3TE0wZFNYWTlqU0ZKZ2dqZlZzRFVLaUJyVm5td1ljaXVyOUVrYmw5Q3RaWTAzdG1yZm01QlplKzZnaHRFTm4wb2gzMzh0WlJleWpjcjc0QWs3MWhnWWtuTE9CQzE1VllmalhzcXVBVW13MlI2TWNWMlBPT2JyY1RSYlhBZ3pvUWJPeWQ4U2JFWmN3aE43NktQd1dzUVFTMUowdGlZSFVLeE9tMnQ0ZkJWMGhQVmhhOUI4Y0swNHFKUVp0MDBaMWNKRGEwd2I4VWx6RWs5QkhVVzJlbk9mVDE0UnlIQ2krWUdlbVBLY2RDUXJoMXpyWVRGN0ltb0x4N3h1NGV2RFRZc2xzV0JrbFFJb3g4NnJWckVVa1N0dXErQUNTWS9xVTM5L1Zhd3Y5S0FmUjVUZUVicGt3RGhTYjBOQkFqVDhBeXRsRFZkR2ZpZzBxS0czVllpVHBYRnc1cHRMVmgrYmtkK2RnN3Z4dHZyNDVaVVdKZXlyekdOR0g3YUZZZDZwLzJNRy9YSlRsR3ovU05RbzJDUExraU83SlhuOU5HZXhaN3BIbTBkZ3pNWmJHRVhxVmR2bG04MTJhL1hMMVNxeEdVWStvNVpsVUM3WTV4Z2dhRCtGZVA5enpoeUpxSUVwcDk3My9ScTRteG1wQWZMcVNzTzJSeHlTcStpdjFDc3AwQ3JvMDc4OEhybDFteWt4dVQweWRSWVpDNkRTeDhNMi9MWTNkOXNud3U3NkFmYjVDOVF1ZE9Zc0wzREh2aGZncmNVSWUvcUhmVFo5QWF6Y3pUanlyM2RPQkFjczBLZk12Y0xVUzRSeHZDdW1NNDVyNDJnMXJ3UGluN2JBcmYvZnNMTzZtS3g0WWRoSURNWlF6V3RjbkhFSTF5TXJ6aU9pdXhMdE8xalRBV25uU2VLVDJ0cXI3Tm42Qmg5TURHNjZZK2lJaW4xV05TUCtMdDFYdXRkajNKTyt4b1FNUVB5ZFpoZkJYZXpVMEhRMnd0eEdwdzRNczRTMTVJbFg1TEdXR3dXeUdYTWNjVWd3b1RDeFRGYmgyZFo0Vkg3OVZHTEVFR1JRWEZrNTRBdlFLdFBpdUcxY0w4RFo3WEoyRHkxSzZUUWVORE9YeFl2NFNveitCMHNBS0VwTVRrNCtTYWpYNksrSjlUOFhZVXRTOE8wWWZGUFZqZkhIYTZORWQyODdVcUlqMnJnQlF1bjVDV3hCczFHUm5BYmd1Z3MyL2ZQakcwZmdQemdSYzR5Q3ZObFg4V2pKUnloc3U5VFRKTjd1R3NOdnprU2IyZWlyQmhEaG1vQ0Jqa0wyYnMzT3I2d2pnNnBUNVpmNGhEdDF0STBJNXo1aytxQXVSZnRhd1lmamhXYmpMS0xKOTlUVk1kRDZaTCtTenNtQkNWN05lYm96V0RUTWgrRnJPT292R09ZbUk1bWp4Smd1MVRXNnI1V0JUK2oxSjBFNmJIb2tEMWo0Wm1DWUQreVBPUW1PMm1yUTNGdC9jVmZwQWlJdzliRkgwZ1FIbXQ4QnNuZnQ2MVV3c1h6cSs2akNvY1hOOUMvRXZPblhTczZuVlNGSkVBL3l1QmNIazZxOWdqanBnRG1NTEcrNlpxR1VjRWMzZEp2THpuK3pNT0p3TDI4WUQxN3BLSXBUNnd6WFBFVFJwWS9qNHhoMkQvaFhJRVNHcTk1eTVmZE9MNmx1QT09IiBWZXJzaW9uPSI5LjkiPgogICAgPFR5cGU+UnVudGltZTwvVHlwZT4KICAgIDxVc2VybmFtZT5Vc2VyTmFtZTwvVXNlcm5hbWU+CiAgICA8RW1haWw+ZU1haWxAaG9zdC5jb208L0VtYWlsPgogICAgPE9yZ2FuaXphdGlvbj5Pcmdhbml6YXRpb248L09yZ2FuaXphdGlvbj4KICAgIDxMaWNlbnNlZERhdGU+MjAxNi0wMS0wMVQxMjowMDowMFo8L0xpY2Vuc2VkRGF0ZT4KICAgIDxFeHBpcmVkRGF0ZT4yMDk5LTEyLTMxVDEyOjAwOjAwWjwvRXhwaXJlZERhdGU+CiAgICA8UHJvZHVjdHM+CiAgICAgICAgPFByb2R1Y3Q+CiAgICAgICAgICAgIDxOYW1lPlNwaXJlLk9mZmljZSBQbGF0aW51bTwvTmFtZT4KICAgICAgICAgICAgPFZlcnNpb24+OS45OTwvVmVyc2lvbj4KICAgICAgICAgICAgPFN1YnNjcmlwdGlvbj4KICAgICAgICAgICAgICAgIDxOdW1iZXJPZlBlcm1pdHRlZERldmVsb3Blcj45OTk5OTwvTnVtYmVyT2ZQZXJtaXR0ZWREZXZlbG9wZXI+CiAgICAgICAgICAgICAgICA8TnVtYmVyT2ZQZXJtaXR0ZWRTaXRlPjk5OTk5PC9OdW1iZXJPZlBlcm1pdHRlZFNpdGU+CiAgICAgICAgICAgIDwvU3Vic2NyaXB0aW9uPgogICAgICAgIDwvUHJvZHVjdD4KICAgIDwvUHJvZHVjdHM+CiAgICA8SXNzdWVyPgogICAgICAgIDxOYW1lPklzc3VlcjwvTmFtZT4KICAgICAgICA8RW1haWw+aXNzdWVyQGlzc3Vlci5jb208L0VtYWlsPgogICAgICAgIDxVcmw+aHR0cDovL3d3dy5pc3N1ZXIuY29tPC9Vcmw+CiAgICA8L0lzc3Vlcj4KPC9MaWNlbnNlPg==";

                Spire.License.LicenseProvider.SetLicenseKey(LData);
                Workbook workbook = new Workbook();
                var mappedPathSource = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + "jlv2" + ".xlsx");
                workbook.LoadFromFile(mappedPathSource);
                Worksheet sheet = workbook.Worksheets[0];

                err_code = "2";
                sheet.Range[1, 17].Text = fdp.Id.ToString();

                sheet.Range[2, 2].Text = ((DateTime)result.Date).ToString("MMMM dd yyyy");
                sheet.Range[3, 2].Text = string.IsNullOrEmpty(result.AcType) ? "" : result.AcType;
                sheet.Range[4, 2].Text = string.IsNullOrEmpty(result.Reg) ? "" : "EP-" + result.Reg;
                sheet.Range[5, 2].Text = ""; //ATLNO
                sheet.Range[6, 2].Text = ((DateTime)result.STD).ToString("HH:mm");
                var ln_crew = 8;
                foreach (var c in result.crew)
                {
                    sheet.Range[ln_crew, 1].Text = string.IsNullOrEmpty(c.Position) ? "" : c.Position;
                    sheet.Range[ln_crew, 2].Text = string.IsNullOrEmpty(c.Name) ? "" : c.Name + (string.IsNullOrEmpty(c.LegsStr) ? "" : " " + c.LegsStr);
                    sheet.Range[ln_crew, 3].Text = format_to_time(c.TotalBlockTime);
                    ln_crew++;
                }

                var ln_leg = 4;
                foreach (var leg in result.legs)
                {
                    sheet.Range[ln_leg, 5].Text = leg.ID.ToString(); //CODE
                    sheet.Range[ln_leg, 6].Text = leg.FlightNumber;
                    sheet.Range[ln_leg, 7].Text = leg.FromAirportIATA;
                    sheet.Range[ln_leg, 8].Text = leg.ToAirportIATA;
                    sheet.Range[ln_leg, 9].Text = string.IsNullOrEmpty(leg.PF) ? "" : leg.PF;
                    sheet.Range[ln_leg, 10].Text = leg.BlockOff == null ? "" : ((DateTime)leg.BlockOff).ToString("HH:mm");
                    sheet.Range[ln_leg, 11].Text = leg.TakeOff == null ? "" : ((DateTime)leg.TakeOff).ToString("HH:mm");
                    sheet.Range[ln_leg, 12].Text = leg.Landing == null ? "" : ((DateTime)leg.Landing).ToString("HH:mm");
                    sheet.Range[ln_leg, 13].Text = leg.BlockOn == null ? "" : ((DateTime)leg.BlockOn).ToString("HH:mm");
                    sheet.Range[ln_leg, 14].Text = leg.FlightTime == null ? "" : format_to_time(leg.FlightTime);
                    sheet.Range[ln_leg, 15].Text = leg.BlockTime == null ? "" : format_to_time(leg.BlockTime);
                    sheet.Range[ln_leg, 16].Text = leg.BlockOff == null || leg.BlockOn == null ? "" : format_to_time(leg.NightTime);


                    sheet.Range[ln_leg, 17].Text = leg.RemDuty == null ? "" : format_to_time(leg.RemDuty);
                    sheet.Range[ln_leg, 18].Text = string.IsNullOrEmpty(leg.DelayCode) ? "" : leg.DelayCode;
                    ln_leg++;


                }
                var nots = new List<string>();
                ln_leg = 14;
                foreach (var leg in result.legs)
                {
                    sheet.Range[ln_leg, 5].Text = leg.FuelRemaining == null ? "" : Convert.ToInt32(leg.FuelRemaining).ToString();
                    sheet.Range[ln_leg, 6].Text = leg.FuelUplift == null ? "" : Convert.ToInt32(leg.FuelUplift).ToString();
                    sheet.Range[ln_leg, 7].Text = leg.FuelTotal == null ? "" : Convert.ToInt32(leg.FuelTotal).ToString();
                    sheet.Range[ln_leg, 8].Text = leg.FuelUsed == null ? "" : Convert.ToInt32(leg.FuelUsed).ToString();
                    sheet.Range[ln_leg, 9].Text = string.IsNullOrEmpty(leg.SerialNo) ? "" : leg.SerialNo;

                    sheet.Range[ln_leg, 11].Text = leg.PaxAdult == null ? "" : leg.PaxAdult.ToString();
                    sheet.Range[ln_leg, 12].Text = leg.PaxChild == null ? "" : leg.PaxChild.ToString();
                    sheet.Range[ln_leg, 13].Text = leg.PaxInfant == null ? "" : leg.PaxInfant.ToString();
                    sheet.Range[ln_leg, 14].Text = ((leg.PaxInfant == null ? 0 : leg.PaxInfant)
                        + (leg.PaxChild == null ? 0 : leg.PaxChild)
                        + (leg.PaxAdult == null ? 0 : leg.PaxAdult)).ToString();
                    //(leg.PaxInfant ?? 0 + leg.PaxAdult ?? 0 + leg.PaxChild ?? 0).ToString();

                    sheet.Range[ln_leg, 15].Text = leg.BaggageWeight.ToString();
                    sheet.Range[ln_leg, 16].Text = leg.CargoWeight.ToString();

                    sheet.Range[ln_leg, 17].Text = string.IsNullOrEmpty(leg.FlightType) ? "SCHEDULED" : leg.FlightType.ToString().ToUpper();

                    //CommanderNote
                    if (!string.IsNullOrEmpty(leg.CommanderNote))
                        nots.Add(leg.CommanderNote);
                    ln_leg++;

                }

                sheet.Range[11, 6].Text = format_to_time(result.MaxFDP);
                sheet.Range[11, 8].Text = ((DateTime)result.ReportingTime).ToString("HH:mm");
                sheet.Range[11, 10].Text = (((DateTime)result.DutyEnd).AddMinutes(-30)).ToString("HH:mm");
                sheet.Range[11, 12].Text = format_to_time(result.FDP);
                sheet.Range[11, 14].Text = format_to_time(result.flight); //flt
                sheet.Range[11, 15].Text = format_to_time(result.block); //blk
                sheet.Range[11, 16].Text = format_to_time(result.night); //night
                sheet.Range[11, 17].Text = format_to_time(result.over); //rem



                sheet.Range[22, 4].Text = String.Join("\n", nots); //commander note

                if (!string.IsNullOrEmpty(result.legs.First().JLSignedBy))
                {
                    sheet.Range[22, 15].Text = string.IsNullOrEmpty(result.legs.First().JLSignedBy) ? "" : result.legs.First().JLSignedBy;  //pic name
                    sheet.Range[23, 15].Text = ((DateTime)result.legs.First().JLDatePICApproved).ToString("yyyy-MMM-dd HH:mm");  //pic sign date
                }


                var forms = new List<string>();
                if (result.asr)
                    forms.Add("ASR");
                if (result.vr)
                    forms.Add("VOYAGE REPORT");
                if (forms.Count > 0)
                    sheet.Range[24, 7].Text = string.Join(", ", forms);  //attached forms
                else
                    sheet.Range[24, 7].Text = "NONE";




                var name = "JourneyLog-" + fdp.Id;
                var mappedPath = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + name + ".xlsx");



                workbook.SaveToFile(mappedPath, ExcelVersion.Version2016);

                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StreamContent(new FileStream(mappedPath, FileMode.Open, FileAccess.Read));
                response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = name + ".xlsx";
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");


                return response;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, msg);
                return response;
            }

        }





        public string get_position(string str)
        {
            switch (str)
            {
                case "TRE":
                case "TRI":
                case "IP":
                    return "IP";
                case "Captain":
                    return "PIC";
                case "SAFETY":
                case "SO":
                case "Safety":
                    return "SP";
                case "SCCM":
                    return "FP";
                case "CCM":
                case "CCE":
                    return "FA";
                case "ISCCM":
                case "CCI":
                    return "IFP";
                default:
                    return str;
            }
        }
        [Route("api/jl/v3/xls/{fid}")]

        //nookp
        //karun,
        public HttpResponseMessage GetJourneyLogV3XLS(int fid)
        {
            string err_code = "1";
            try
            {
                //nooz 
                //this.context.Database.CommandTimeout = 160;

                var context = new Models.dbEntities();
                var appleg = context.AppLegs.FirstOrDefault(q => q.FlightId == fid);
                var pid = appleg.PICId;
                var appFlight = context.AppCrewFlightJLs.Where(q => q.FlightId == fid && q.CrewId == pid).FirstOrDefault();
                var crewlegs = context.AppCrewFlightJLs.Where(q => q.FDPId == appFlight.FDPId).ToList();
                var clegs = crewlegs.Select(q => (int)q.FlightId).ToList();
                var legs = context.AppLegJLs.Where(q => clegs.Contains(q.FlightId)).OrderBy(q => q.STD).ToList();
                var fdp = context.ViewFDPRests.FirstOrDefault(q => q.Id == appFlight.FDPId);
                var reporting_time_local = legs[0].STDLocal.Value.AddMinutes(-60);
                var reporting_time_utc = legs[0].STD.Value.AddMinutes(-60);
                foreach (var x in legs)
                {
                    if (x.BlockOn != null)
                    {
                        //fdp reporting time => reporting time utc
                        x.RemDuty = Convert.ToInt32(Math.Round((double)fdp.MaxFDPExtended)) - Convert.ToInt32(Math.Round(((DateTime)x.BlockOn - (DateTime)reporting_time_utc).TotalMinutes));
                        //fdp reporting time => reporting time utc
                        x.ElapsedDuty = Convert.ToInt32(Math.Round(((DateTime)x.BlockOn - (DateTime)reporting_time_utc).TotalMinutes));
                    }
                    else
                    {
                        x.RemDuty = null;
                        x.ElapsedDuty = null;
                    }
                    if (x.BlockOff == null || x.BlockOn == null)
                        x.BlockTime = null;
                    if (x.TakeOff == null || x.Landing == null)
                        x.FlightTime = null;
                }
                //var over = legs.Last().BlockOn!=null?
                //    Convert.ToInt32(Math.Round((double)fdp.MaxFDPExtended)) - Convert.ToInt32(Math.Round(((DateTime)legs.Last().BlockOn - (DateTime)fdp.ReportingTime).TotalMinutes))
                //    : 0;
                var over_leg = legs.Where(q => q.RemDuty < 0).OrderBy(q => q.RemDuty).FirstOrDefault();
                var over = over_leg == null ? 0 : (int)over_leg.RemDuty;
                if (over > 0)
                    over = 0;
                over = Math.Abs(over);

                var elapsed_leg = legs.Where(q => q.ElapsedDuty > 0).OrderByDescending(q => q.ElapsedDuty).FirstOrDefault();



                var actual_fdp = elapsed_leg == null ? 0 : elapsed_leg.ElapsedDuty;
                //var actual_duty = Convert.ToInt32(Math.Round(((DateTime)legs.Last().BlockOn - (DateTime)fdp.ReportingTime).TotalMinutes)) + 30;



                var asr = legs.FirstOrDefault(q => q.AttASR == true) != null;
                var vr = legs.FirstOrDefault(q => q.AttVoyageReport == true) != null;
                var pos1 = legs.FirstOrDefault(q => q.AttRepositioning1 == true) != null;
                var pos2 = legs.FirstOrDefault(q => q.AttRepositioning2 == true) != null;


                // var flight = legs.Sum(q => (((DateTime)(q.Landing!=null?q.Landing:q.STA) - (DateTime)(q.TakeOff!=null?q.TakeOff:q.STD))).TotalMinutes);
                // var block = legs.Sum(q => (((DateTime)(q.BlockOn!=null?q.BlockOn:q.STA) - (DateTime)(q.BlockOff!=null?q.BlockOff:q.STD))).TotalMinutes);
                var flight = legs.Sum(q => (q.Landing != null && q.TakeOff != null) ? ((DateTime)q.Landing - (DateTime)q.TakeOff).TotalMinutes : 0);
                var block = legs.Sum(q => (q.BlockOn != null && q.BlockOff != null) ? ((DateTime)q.BlockOn - (DateTime)q.BlockOff).TotalMinutes : 0);
                var night = legs.Sum(q => q.NightTime);
                var fids = legs.Select(q => (Nullable<int>)q.FlightId).ToList();
                var _crews2 = (from x in context.ViewFlightCrewNews
                                   //where x.FlightId == flightId

                               where fids.Contains(x.FlightId) //&& x.IsPositioning != true
                               orderby x.IsPositioning, x.GroupOrder

                               select new CLJLData()
                               {
                                   CrewId = x.CrewId,
                                   IsPositioning = x.IsPositioning,
                                   PositionId = x.PositionId,
                                   Position = x.Position,
                                   Name = x.Name,
                                   GroupId = x.GroupId,
                                   JobGroup = x.JobGroup,
                                   JobGroupCode = x.JobGroupCode,
                                   GroupOrder = x.GroupOrder,
                                   IsCockpit = x.IsCockpit,
                                   FlightId = x.FlightId,


                               }).ToList();

                var _gcrews = (from x in _crews2
                               group x by new
                               {
                                   x.CrewId,
                                   x.IsPositioning,
                                   x.PositionId,
                                   x.Position,
                                   x.Name,
                                   x.GroupId,
                                   x.JobGroup,
                                   x.JobGroupCode,
                                   x.GroupOrder,
                                   x.IsCockpit,
                               } into grp
                               select grp).ToList();
                var query = (from x in _gcrews
                             let xfids = x.Select(q => Convert.ToInt32(q.FlightId)).ToList()
                             select new CLJLData()
                             {
                                 CrewId = x.Key.CrewId,
                                 IsPositioning = x.Key.IsPositioning,
                                 PositionId = x.Key.PositionId,
                                 Position = x.Key.Position,
                                 Name = x.Key.Name,
                                 GroupId = x.Key.GroupId,
                                 JobGroup = x.Key.JobGroup,
                                 JobGroupCode = x.Key.JobGroupCode,
                                 GroupOrder = x.Key.GroupOrder,
                                 IsCockpit = x.Key.IsCockpit,
                                 Legs = legs.Where(q => xfids.Contains((int)q.FlightId)).OrderBy(q => q.STD).Select(q => q.FlightNumber).Distinct().ToList(),
                                 LegsStr = string.Join("-", legs.Where(q => xfids.Contains((int)q.FlightId)).OrderBy(q => q.STD).Select(q => q.FlightNumber).Distinct().ToList()),
                                 TotalBlockTime = legs.Where(q => xfids.Contains((int)q.FlightId)).Sum(q => q.BlockOff != null && q.BlockOn != null
                                      ? ((DateTime)q.BlockOn - (DateTime)q.BlockOff).TotalMinutes
                                      : (Nullable<double>)null),

                             }).ToList();


                foreach (var x in query)
                {
                    if (x.Legs.Count == fids.Count)
                        x.LegsStr = "";
                }

                var result = new
                {
                    legs,
                    AcType = legs.First().AircraftType,
                    Reg = legs.First().Register,
                    Date = legs.First().STDDay,
                    STD = legs.First().STD,
                    STDLocal = legs.First().STDLocal,
                    STA = legs.Last().STA,
                    STALocal = legs.Last().STALocal,
                    OffBlockLocal = legs.First().BlockOffLocal,
                    OnBlockLocal = legs.Last().BlockOnLocal,
                    fdp.ReportingTime,
                    fdp.ReportingTimeLocal,
                    fdp.DutyEnd,
                    DutyEndLoccal = ((DateTime)fdp.DutyEnd).AddMinutes(210),
                    //DutyEnd30Local =((DateTime)fdp.DutyEnd).AddMinutes(210).AddMinutes(30),
                    FDPId = fdp.Id,
                    // PIC = legs.First().PIC,
                    PIC = appleg.PIC,

                    MaxFDP = fdp.MaxFDPExtended,
                    fdp.FDP,
                    fdp.Duty,
                    flight,
                    block,
                    night,
                    over,
                    actual_fdp,
                    asr,
                    vr,
                    pos1,
                    pos2,
                    crew = query


                };


                string LData = "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiIHN0YW5kYWxvbmU9InllcyI/Pgo8TGljZW5zZSBLZXk9ImNyOERaN1hKMkR5MUs2UUJBTkRPSVRLdlpjTzZkelVod2lsSHBnVlluQ3k0cXlHV2V6TFZubFJGeFAxNU1mSWZnUmdNWm1XaEdOQWRFNFRqZWZnQ1ovbFR2b1BkSXRIbDZXdDVBNWk1TVhFbnFkQnVPMUthRnovRFFzYUdWTGhzdjlySG1ybnRxSElFRGxJeGRxYUpNcGtLb0Frd1A3d1N6T01KMVkrbUNmVTVVRmV6REwvTjd1enJ4M0Y0d2I1SGErd0E2VFQ5VFJ3SzAzejlFS01aRmwzU1lSL3o0YVU3TE0wZFNYWTlqU0ZKZ2dqZlZzRFVLaUJyVm5td1ljaXVyOUVrYmw5Q3RaWTAzdG1yZm01QlplKzZnaHRFTm4wb2gzMzh0WlJleWpjcjc0QWs3MWhnWWtuTE9CQzE1VllmalhzcXVBVW13MlI2TWNWMlBPT2JyY1RSYlhBZ3pvUWJPeWQ4U2JFWmN3aE43NktQd1dzUVFTMUowdGlZSFVLeE9tMnQ0ZkJWMGhQVmhhOUI4Y0swNHFKUVp0MDBaMWNKRGEwd2I4VWx6RWs5QkhVVzJlbk9mVDE0UnlIQ2krWUdlbVBLY2RDUXJoMXpyWVRGN0ltb0x4N3h1NGV2RFRZc2xzV0JrbFFJb3g4NnJWckVVa1N0dXErQUNTWS9xVTM5L1Zhd3Y5S0FmUjVUZUVicGt3RGhTYjBOQkFqVDhBeXRsRFZkR2ZpZzBxS0czVllpVHBYRnc1cHRMVmgrYmtkK2RnN3Z4dHZyNDVaVVdKZXlyekdOR0g3YUZZZDZwLzJNRy9YSlRsR3ovU05RbzJDUExraU83SlhuOU5HZXhaN3BIbTBkZ3pNWmJHRVhxVmR2bG04MTJhL1hMMVNxeEdVWStvNVpsVUM3WTV4Z2dhRCtGZVA5enpoeUpxSUVwcDk3My9ScTRteG1wQWZMcVNzTzJSeHlTcStpdjFDc3AwQ3JvMDc4OEhybDFteWt4dVQweWRSWVpDNkRTeDhNMi9MWTNkOXNud3U3NkFmYjVDOVF1ZE9Zc0wzREh2aGZncmNVSWUvcUhmVFo5QWF6Y3pUanlyM2RPQkFjczBLZk12Y0xVUzRSeHZDdW1NNDVyNDJnMXJ3UGluN2JBcmYvZnNMTzZtS3g0WWRoSURNWlF6V3RjbkhFSTF5TXJ6aU9pdXhMdE8xalRBV25uU2VLVDJ0cXI3Tm42Qmg5TURHNjZZK2lJaW4xV05TUCtMdDFYdXRkajNKTyt4b1FNUVB5ZFpoZkJYZXpVMEhRMnd0eEdwdzRNczRTMTVJbFg1TEdXR3dXeUdYTWNjVWd3b1RDeFRGYmgyZFo0Vkg3OVZHTEVFR1JRWEZrNTRBdlFLdFBpdUcxY0w4RFo3WEoyRHkxSzZUUWVORE9YeFl2NFNveitCMHNBS0VwTVRrNCtTYWpYNksrSjlUOFhZVXRTOE8wWWZGUFZqZkhIYTZORWQyODdVcUlqMnJnQlF1bjVDV3hCczFHUm5BYmd1Z3MyL2ZQakcwZmdQemdSYzR5Q3ZObFg4V2pKUnloc3U5VFRKTjd1R3NOdnprU2IyZWlyQmhEaG1vQ0Jqa0wyYnMzT3I2d2pnNnBUNVpmNGhEdDF0STBJNXo1aytxQXVSZnRhd1lmamhXYmpMS0xKOTlUVk1kRDZaTCtTenNtQkNWN05lYm96V0RUTWgrRnJPT292R09ZbUk1bWp4Smd1MVRXNnI1V0JUK2oxSjBFNmJIb2tEMWo0Wm1DWUQreVBPUW1PMm1yUTNGdC9jVmZwQWlJdzliRkgwZ1FIbXQ4QnNuZnQ2MVV3c1h6cSs2akNvY1hOOUMvRXZPblhTczZuVlNGSkVBL3l1QmNIazZxOWdqanBnRG1NTEcrNlpxR1VjRWMzZEp2THpuK3pNT0p3TDI4WUQxN3BLSXBUNnd6WFBFVFJwWS9qNHhoMkQvaFhJRVNHcTk1eTVmZE9MNmx1QT09IiBWZXJzaW9uPSI5LjkiPgogICAgPFR5cGU+UnVudGltZTwvVHlwZT4KICAgIDxVc2VybmFtZT5Vc2VyTmFtZTwvVXNlcm5hbWU+CiAgICA8RW1haWw+ZU1haWxAaG9zdC5jb208L0VtYWlsPgogICAgPE9yZ2FuaXphdGlvbj5Pcmdhbml6YXRpb248L09yZ2FuaXphdGlvbj4KICAgIDxMaWNlbnNlZERhdGU+MjAxNi0wMS0wMVQxMjowMDowMFo8L0xpY2Vuc2VkRGF0ZT4KICAgIDxFeHBpcmVkRGF0ZT4yMDk5LTEyLTMxVDEyOjAwOjAwWjwvRXhwaXJlZERhdGU+CiAgICA8UHJvZHVjdHM+CiAgICAgICAgPFByb2R1Y3Q+CiAgICAgICAgICAgIDxOYW1lPlNwaXJlLk9mZmljZSBQbGF0aW51bTwvTmFtZT4KICAgICAgICAgICAgPFZlcnNpb24+OS45OTwvVmVyc2lvbj4KICAgICAgICAgICAgPFN1YnNjcmlwdGlvbj4KICAgICAgICAgICAgICAgIDxOdW1iZXJPZlBlcm1pdHRlZERldmVsb3Blcj45OTk5OTwvTnVtYmVyT2ZQZXJtaXR0ZWREZXZlbG9wZXI+CiAgICAgICAgICAgICAgICA8TnVtYmVyT2ZQZXJtaXR0ZWRTaXRlPjk5OTk5PC9OdW1iZXJPZlBlcm1pdHRlZFNpdGU+CiAgICAgICAgICAgIDwvU3Vic2NyaXB0aW9uPgogICAgICAgIDwvUHJvZHVjdD4KICAgIDwvUHJvZHVjdHM+CiAgICA8SXNzdWVyPgogICAgICAgIDxOYW1lPklzc3VlcjwvTmFtZT4KICAgICAgICA8RW1haWw+aXNzdWVyQGlzc3Vlci5jb208L0VtYWlsPgogICAgICAgIDxVcmw+aHR0cDovL3d3dy5pc3N1ZXIuY29tPC9Vcmw+CiAgICA8L0lzc3Vlcj4KPC9MaWNlbnNlPg==";

                Spire.License.LicenseProvider.SetLicenseKey(LData);
                Workbook workbook = new Workbook();
                var mappedPathSource = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + "jlv3" + ".xlsx");
                workbook.LoadFromFile(mappedPathSource);
                Worksheet sheet = workbook.Worksheets[0];

                err_code = "2";
                // sheet.Range[1, 17].Text = fdp.Id.ToString();

                sheet.Range[14, 9].Text = "FUEL INFO (kg)";

                sheet.Range[8, 5].Text = ((DateTime)result.Date).ToString("MM/dd/yyyy");
                sheet.Range[4, 5].Text = string.IsNullOrEmpty(result.AcType) ? "" : result.AcType;
                sheet.Range[6, 5].Text = string.IsNullOrEmpty(result.Reg) ? "" : result.Reg;
                //offblock local => stdlocal
                sheet.Range[11, 7].Text = ((DateTime)result.STDLocal).ToString("HH:mm");
                sheet.Range[12, 7].Text = result.OnBlockLocal != null ? ((DateTime)result.OnBlockLocal).ToString("HH:mm") : ((DateTime)result.STALocal).ToString("HH:mm");

                //reporting time local => stdlocal - 60 min
                sheet.Range[11, 14].Text = ((DateTime)reporting_time_local).ToString("HH:mm");
                // var _start =
                sheet.Range[12, 14].Text = ((result.OnBlockLocal == null ? (DateTime)result.STALocal : (DateTime)result.OnBlockLocal)).AddMinutes(30).ToString("HH:mm");

                // ((DateTime)result.DutyEndLoccal).ToString("HH:mm");


                var _duty = Convert.ToInt32(Math.Round((

                       (result.OnBlockLocal != null ? ((DateTime)result.OnBlockLocal) : ((DateTime)result.STALocal))

                        //- (result.OffBlockLocal != null ? ((DateTime)result.OffBlockLocal)  : ((DateTime)result.STDLocal) )
                        - ((DateTime)result.ReportingTimeLocal)
                        ).TotalMinutes));

                sheet.Range[11, 20].Text = format_to_time(result.MaxFDP);
                //var scheduled_fdp =Convert.ToInt32( Math.Round( (((DateTime)result.STALocal) - ((DateTime)result.ReportingTimeLocal)).TotalMinutes));
                sheet.Range[12, 20].Text = format_to_time(_duty + 30); //format_to_time(result.FDP+30);

                sheet.Range[12, 23].Text = format_to_time(_duty);  //format_to_time(result.FDP  );

                TimeSpan flightTime = TimeSpan.FromMinutes(result.flight);
                sheet.Range[23, 24].Text = flightTime.ToString(@"hh\:mm");
                //sheet.Range[23, 24].Text = result.OnBlockLocal != null
                //    ? flightTime.ToString(@"hh\:mm")
                //    : ((DateTime)result.STALocal).ToString("HH:mm");

                TimeSpan blockTime = TimeSpan.FromMinutes(result.block);
                sheet.Range[23, 25].Text = blockTime.ToString(@"hh\:mm");
                //sheet.Range[23, 25].Text = result.OnBlockLocal != null
                //    ? blockTime.ToString(@"hh\:mm")
                //    : ((DateTime)result.STALocal).ToString("HH:mm");


                //sheet.Range[23, 24].Text = result.OnBlockLocal != null ? ((DateTime)result.flight).ToString("HH:mm") : ((DateTime)result.STALocal).ToString("HH:mm");
                //sheet.Range[23, 25].Text = result.OnBlockLocal != null ? ((DateTime)result.block).ToString("HH:mm") : ((DateTime)result.STALocal).ToString("HH:mm");





                //  sheet.Range[11, 14].Text = format_to_time(result.flight); //flt
                // sheet.Range[11, 15].Text = format_to_time(result.block); //blk
                // sheet.Range[11, 16].Text = format_to_time(result.night); //night
                //sheet.Range[11, 17].Text = format_to_time(result.over); //rem


                var ln_crew = 5;
                var col_crew = 8; //17
                var cn_crew = 1;
                foreach (var c in result.crew)
                {
                    sheet.Range[ln_crew, col_crew].Text = string.IsNullOrEmpty(c.Position) ? "" : get_position(c.Position);
                    sheet.Range[ln_crew, col_crew + 2].Text = string.IsNullOrEmpty(c.Name) ? "" : c.Name + " (" + c.JobGroup + ")";
                    //sheet.Range[ln_crew, 3].Text = format_to_time(c.TotalBlockTime);
                    ln_crew++;
                    cn_crew++;
                    if (cn_crew > 5)
                    {
                        col_crew = 17;
                        ln_crew = 5;
                    }

                }

                var ln_leg = 18;  //18,2
                foreach (var leg in result.legs)
                {
                    sheet.Range[ln_leg, 2].Text = ((DateTime)leg.STD).ToString("MM");
                    sheet.Range[ln_leg, 3].Text = ((DateTime)leg.STD).ToString("dd");
                    sheet.Range[ln_leg, 4].Text = leg.FromAirportIATA;
                    sheet.Range[ln_leg, 5].Text = leg.ToAirportIATA;
                    sheet.Range[ln_leg, 6].Text = leg.FlightNumber;
                    sheet.Range[ln_leg, 7].Text = string.IsNullOrEmpty(leg.FlightType) ? "S" : (leg.FlightType.StartsWith("S") ? "S" : "N");
                    sheet.Range[ln_leg, 8].Text = string.IsNullOrEmpty(leg.PF) ? "" : leg.PF;

                    sheet.Range[ln_leg, 9].Text = leg.FuelRemaining == null ? "" : Convert.ToInt32(leg.FuelRemaining).ToString();
                    sheet.Range[ln_leg, 10].Text = leg.FuelUplift == null ? "" : Convert.ToInt32(leg.FuelUplift).ToString();
                    sheet.Range[ln_leg, 11].Text = leg.FuelDensity == null ? "0.79" : Math.Round((decimal)leg.FuelDensity, 2).ToString();
                    sheet.Range[ln_leg, 12].Text = leg.FuelTotal == null ? "" : Convert.ToInt32(leg.FuelTotal).ToString();
                    sheet.Range[ln_leg, 13].Text = leg.FuelUsed == null ? "" : Convert.ToInt32(leg.FuelUsed).ToString();

                    //sheet.Range[ln_leg, 14].Text = leg.PaxAdult == null ? "" : leg.PaxAdult.ToString();
                    sheet.Range[ln_leg, 14].Text = leg.PaxMale == null ? "" : leg.PaxMale.ToString();
                    sheet.Range[ln_leg, 15].Text = leg.PaxFemale == null ? "" : leg.PaxFemale.ToString();
                    sheet.Range[ln_leg, 16].Text = leg.PaxChild == null ? "" : leg.PaxChild.ToString();
                    sheet.Range[ln_leg, 17].Text = leg.PaxInfant == null ? "" : leg.PaxInfant.ToString();
                    sheet.Range[ln_leg, 18].Text = ((leg.PaxInfant == null ? 0 : leg.PaxInfant)
                        + (leg.PaxChild == null ? 0 : leg.PaxChild)
                        + (leg.PaxAdult == null ? 0 : leg.PaxAdult)).ToString();

                    sheet.Range[ln_leg, 19].Text = (leg.BaggageWeight + leg.CargoWeight).ToString();



                    sheet.Range[ln_leg, 20].Text = leg.BlockOff == null ? "" : ((DateTime)leg.BlockOff).ToString("HH:mm");
                    sheet.Range[ln_leg, 21].Text = leg.TakeOff == null ? "" : ((DateTime)leg.TakeOff).ToString("HH:mm");
                    sheet.Range[ln_leg, 22].Text = leg.Landing == null ? "" : ((DateTime)leg.Landing).ToString("HH:mm");
                    sheet.Range[ln_leg, 23].Text = leg.BlockOn == null ? "" : ((DateTime)leg.BlockOn).ToString("HH:mm");
                    sheet.Range[ln_leg, 24].Text = leg.FlightTime == null ? "" : format_to_time(leg.FlightTime);
                    sheet.Range[ln_leg, 25].Text = leg.BlockTime == null ? "" : format_to_time(leg.BlockTime);
                    //sheet.Range[ln_leg, 26].Text = leg.RemDuty == null ? "" : ((DateTime)leg.RemDuty).ToString("HH:mm");
                    sheet.Range[ln_leg, 26].Text = leg.RemDuty == null ? "" : TimeSpan.FromMinutes((int)leg.RemDuty).ToString(@"hh\:mm");


                    //sheet.Range[ln_leg, 16].Text = leg.BlockOff == null || leg.BlockOn == null ? "" : format_to_time(leg.NightTime);


                    //sheet.Range[ln_leg, 17].Text = leg.RemDuty == null ? "" : format_to_time(leg.RemDuty);
                    //sheet.Range[ln_leg, 18].Text = string.IsNullOrEmpty(leg.DelayCode) ? "" : leg.DelayCode;
                    ln_leg++;


                }
                var nots = new List<string>();
                ln_leg = 27;
                foreach (var leg in result.legs)
                {

                    sheet.Range[ln_leg, 2].Text = string.IsNullOrEmpty(leg.SerialNo) ? "" : leg.SerialNo;
                    sheet.Range[ln_leg, 5].Text = string.IsNullOrEmpty(leg.LTR) ? "" : leg.LTR;


                    //(leg.PaxInfant ?? 0 + leg.PaxAdult ?? 0 + leg.PaxChild ?? 0).ToString();





                    //CommanderNote
                    if (!string.IsNullOrEmpty(leg.CommanderNote))
                        nots.Add(leg.CommanderNote);
                    ln_leg++;

                }





                sheet.Range[27, 8].Text = String.Join("\n", nots); //commander note

                if (!string.IsNullOrEmpty(result.legs.First().JLSignedBy))
                {
                    sheet.Range[27, 19].Text = (string.IsNullOrEmpty(result.legs.First().JLSignedBy) ? "" : result.legs.First().JLSignedBy)
                        + "\n"
                        + ((DateTime)result.legs.First().JLDatePICApproved).ToString("yyyy-MMM-dd HH:mm");  //pic name

                }


                var forms = new List<string>();
                if (result.asr)
                {
                    forms.Add("ASR");
                    sheet.Range[32, 14].Text = "*";
                }
                if (result.vr)
                {
                    forms.Add("VOYAGE REPORT");
                    sheet.Range[32, 17].Text = "*";
                }
                //if (forms.Count > 0)
                //    sheet.Range[24, 7].Text = string.Join(", ", forms);  //attached forms
                // else
                //    sheet.Range[24, 7].Text = "NONE";




                var name = "JourneyLog-" + fdp.Id;
                var mappedPath = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + name + ".xlsx");



                workbook.SaveToFile(mappedPath, ExcelVersion.Version2016);

                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StreamContent(new FileStream(mappedPath, FileMode.Open, FileAccess.Read));
                response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = name + ".xlsx";
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");


                return response;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, msg);
                return response;
            }

        }

        [Route("api/jl/v4/xls/{fid}")]

        //nookp
        //karun,
        public HttpResponseMessage GetJourneyLogV4XLS(int fid)
        {
            string err_code = "1";
            try
            {
                //nooz 
                //this.context.Database.CommandTimeout = 160;

                var context = new Models.dbEntities();
                var appleg = context.AppLegs.FirstOrDefault(q => q.FlightId == fid);
                var pid = appleg.PICId;
                var appFlight = context.AppCrewFlightJLs.Where(q => q.FlightId == fid && q.CrewId == pid).FirstOrDefault();
                var crewlegs = context.AppCrewFlightJLs.Where(q => q.FDPId == appFlight.FDPId).ToList();
                var clegs = crewlegs.Select(q => (int)q.FlightId).ToList();
                var legs = context.AppLegJLs.Where(q => clegs.Contains(q.FlightId)).OrderBy(q => q.STD).ToList();
                var fdp = context.ViewFDPRests.FirstOrDefault(q => q.Id == appFlight.FDPId);
                var reporting_time_local = legs[0].STDLocal.Value.AddMinutes(-60);
                var reporting_time_utc = legs[0].STD.Value.AddMinutes(-60);
                foreach (var x in legs)
                {
                    if (x.BlockOn != null)
                    {
                        //fdp reporting time => reporting time utc
                        x.RemDuty = Convert.ToInt32(Math.Round((double)fdp.MaxFDPExtended)) - Convert.ToInt32(Math.Round(((DateTime)x.BlockOn - (DateTime)reporting_time_utc).TotalMinutes));
                        //fdp reporting time => reporting time utc
                        x.ElapsedDuty = Convert.ToInt32(Math.Round(((DateTime)x.BlockOn - (DateTime)reporting_time_utc).TotalMinutes));
                    }
                    else
                    {
                        x.RemDuty = null;
                        x.ElapsedDuty = null;
                    }
                    if (x.BlockOff == null || x.BlockOn == null)
                        x.BlockTime = null;
                    if (x.TakeOff == null || x.Landing == null)
                        x.FlightTime = null;
                }
                //var over = legs.Last().BlockOn!=null?
                //    Convert.ToInt32(Math.Round((double)fdp.MaxFDPExtended)) - Convert.ToInt32(Math.Round(((DateTime)legs.Last().BlockOn - (DateTime)fdp.ReportingTime).TotalMinutes))
                //    : 0;
                var over_leg = legs.Where(q => q.RemDuty < 0).OrderBy(q => q.RemDuty).FirstOrDefault();
                var over = over_leg == null ? 0 : (int)over_leg.RemDuty;
                if (over > 0)
                    over = 0;
                over = Math.Abs(over);

                var elapsed_leg = legs.Where(q => q.ElapsedDuty > 0).OrderByDescending(q => q.ElapsedDuty).FirstOrDefault();



                var actual_fdp = elapsed_leg == null ? 0 : elapsed_leg.ElapsedDuty;
                //var actual_duty = Convert.ToInt32(Math.Round(((DateTime)legs.Last().BlockOn - (DateTime)fdp.ReportingTime).TotalMinutes)) + 30;



                var asr = legs.FirstOrDefault(q => q.AttASR == true) != null;
                var vr = legs.FirstOrDefault(q => q.AttVoyageReport == true) != null;
                var pos1 = legs.FirstOrDefault(q => q.AttRepositioning1 == true) != null;
                var pos2 = legs.FirstOrDefault(q => q.AttRepositioning2 == true) != null;


                // var flight = legs.Sum(q => (((DateTime)(q.Landing!=null?q.Landing:q.STA) - (DateTime)(q.TakeOff!=null?q.TakeOff:q.STD))).TotalMinutes);
                // var block = legs.Sum(q => (((DateTime)(q.BlockOn!=null?q.BlockOn:q.STA) - (DateTime)(q.BlockOff!=null?q.BlockOff:q.STD))).TotalMinutes);
                var flight = legs.Sum(q => (q.Landing != null && q.TakeOff != null) ? ((DateTime)q.Landing - (DateTime)q.TakeOff).TotalMinutes : 0);
                var block = legs.Sum(q => (q.BlockOn != null && q.BlockOff != null) ? ((DateTime)q.BlockOn - (DateTime)q.BlockOff).TotalMinutes : 0);
                var night = legs.Sum(q => q.NightTime);
                var fids = legs.Select(q => (Nullable<int>)q.FlightId).ToList();
                var _crews2 = (from x in context.ViewFlightCrewNews
                                   //where x.FlightId == flightId

                               where fids.Contains(x.FlightId) //&& x.IsPositioning != true
                               orderby x.IsPositioning, x.GroupOrder

                               select new CLJLData()
                               {
                                   CrewId = x.CrewId,
                                   IsPositioning = x.IsPositioning,
                                   PositionId = x.PositionId,
                                   Position = x.Position,
                                   Name = x.Name,
                                   GroupId = x.GroupId,
                                   JobGroup = x.JobGroup,
                                   JobGroupCode = x.JobGroupCode,
                                   GroupOrder = x.GroupOrder,
                                   IsCockpit = x.IsCockpit,
                                   FlightId = x.FlightId,


                               }).ToList();

                var _gcrews = (from x in _crews2
                               group x by new
                               {
                                   x.CrewId,
                                   x.IsPositioning,
                                   x.PositionId,
                                   x.Position,
                                   x.Name,
                                   x.GroupId,
                                   x.JobGroup,
                                   x.JobGroupCode,
                                   x.GroupOrder,
                                   x.IsCockpit,
                               } into grp
                               select grp).ToList();
                var query = (from x in _gcrews
                             let xfids = x.Select(q => Convert.ToInt32(q.FlightId)).ToList()
                             select new CLJLData()
                             {
                                 CrewId = x.Key.CrewId,
                                 IsPositioning = x.Key.IsPositioning,
                                 PositionId = x.Key.PositionId,
                                 Position = x.Key.Position,
                                 Name = x.Key.Name,
                                 GroupId = x.Key.GroupId,
                                 JobGroup = x.Key.JobGroup,
                                 JobGroupCode = x.Key.JobGroupCode,
                                 GroupOrder = x.Key.GroupOrder,
                                 IsCockpit = x.Key.IsCockpit,
                                 Legs = legs.Where(q => xfids.Contains((int)q.FlightId)).OrderBy(q => q.STD).Select(q => q.FlightNumber).Distinct().ToList(),
                                 LegsStr = string.Join("-", legs.Where(q => xfids.Contains((int)q.FlightId)).OrderBy(q => q.STD).Select(q => q.FlightNumber).Distinct().ToList()),
                                 TotalBlockTime = legs.Where(q => xfids.Contains((int)q.FlightId)).Sum(q => q.BlockOff != null && q.BlockOn != null
                                      ? ((DateTime)q.BlockOn - (DateTime)q.BlockOff).TotalMinutes
                                      : (Nullable<double>)null),

                             }).ToList();


                foreach (var x in query)
                {
                    if (x.Legs.Count == fids.Count)
                        x.LegsStr = "";
                }

                var result = new
                {
                    legs,
                    AcType = legs.First().AircraftType,
                    Reg = legs.First().Register,
                    Date = legs.First().STDDay,
                    STD = legs.First().STD,
                    STDLocal = legs.First().STDLocal,
                    STA = legs.Last().STA,
                    STALocal = legs.Last().STALocal,
                    OffBlockLocal = legs.First().BlockOffLocal,
                    OnBlockLocal = legs.Last().BlockOnLocal,
                    fdp.ReportingTime,
                    fdp.ReportingTimeLocal,
                    fdp.DutyEnd,
                    DutyEndLoccal = ((DateTime)fdp.DutyEnd).AddMinutes(210),
                    //DutyEnd30Local =((DateTime)fdp.DutyEnd).AddMinutes(210).AddMinutes(30),
                    FDPId = fdp.Id,
                    // PIC = legs.First().PIC,
                    PIC = appleg.PIC,

                    MaxFDP = fdp.MaxFDPExtended,
                    fdp.FDP,
                    fdp.Duty,
                    flight,
                    block,
                    night,
                    over,
                    actual_fdp,
                    asr,
                    vr,
                    pos1,
                    pos2,
                    crew = query


                };


                string LData = "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiIHN0YW5kYWxvbmU9InllcyI/Pgo8TGljZW5zZSBLZXk9ImNyOERaN1hKMkR5MUs2UUJBTkRPSVRLdlpjTzZkelVod2lsSHBnVlluQ3k0cXlHV2V6TFZubFJGeFAxNU1mSWZnUmdNWm1XaEdOQWRFNFRqZWZnQ1ovbFR2b1BkSXRIbDZXdDVBNWk1TVhFbnFkQnVPMUthRnovRFFzYUdWTGhzdjlySG1ybnRxSElFRGxJeGRxYUpNcGtLb0Frd1A3d1N6T01KMVkrbUNmVTVVRmV6REwvTjd1enJ4M0Y0d2I1SGErd0E2VFQ5VFJ3SzAzejlFS01aRmwzU1lSL3o0YVU3TE0wZFNYWTlqU0ZKZ2dqZlZzRFVLaUJyVm5td1ljaXVyOUVrYmw5Q3RaWTAzdG1yZm01QlplKzZnaHRFTm4wb2gzMzh0WlJleWpjcjc0QWs3MWhnWWtuTE9CQzE1VllmalhzcXVBVW13MlI2TWNWMlBPT2JyY1RSYlhBZ3pvUWJPeWQ4U2JFWmN3aE43NktQd1dzUVFTMUowdGlZSFVLeE9tMnQ0ZkJWMGhQVmhhOUI4Y0swNHFKUVp0MDBaMWNKRGEwd2I4VWx6RWs5QkhVVzJlbk9mVDE0UnlIQ2krWUdlbVBLY2RDUXJoMXpyWVRGN0ltb0x4N3h1NGV2RFRZc2xzV0JrbFFJb3g4NnJWckVVa1N0dXErQUNTWS9xVTM5L1Zhd3Y5S0FmUjVUZUVicGt3RGhTYjBOQkFqVDhBeXRsRFZkR2ZpZzBxS0czVllpVHBYRnc1cHRMVmgrYmtkK2RnN3Z4dHZyNDVaVVdKZXlyekdOR0g3YUZZZDZwLzJNRy9YSlRsR3ovU05RbzJDUExraU83SlhuOU5HZXhaN3BIbTBkZ3pNWmJHRVhxVmR2bG04MTJhL1hMMVNxeEdVWStvNVpsVUM3WTV4Z2dhRCtGZVA5enpoeUpxSUVwcDk3My9ScTRteG1wQWZMcVNzTzJSeHlTcStpdjFDc3AwQ3JvMDc4OEhybDFteWt4dVQweWRSWVpDNkRTeDhNMi9MWTNkOXNud3U3NkFmYjVDOVF1ZE9Zc0wzREh2aGZncmNVSWUvcUhmVFo5QWF6Y3pUanlyM2RPQkFjczBLZk12Y0xVUzRSeHZDdW1NNDVyNDJnMXJ3UGluN2JBcmYvZnNMTzZtS3g0WWRoSURNWlF6V3RjbkhFSTF5TXJ6aU9pdXhMdE8xalRBV25uU2VLVDJ0cXI3Tm42Qmg5TURHNjZZK2lJaW4xV05TUCtMdDFYdXRkajNKTyt4b1FNUVB5ZFpoZkJYZXpVMEhRMnd0eEdwdzRNczRTMTVJbFg1TEdXR3dXeUdYTWNjVWd3b1RDeFRGYmgyZFo0Vkg3OVZHTEVFR1JRWEZrNTRBdlFLdFBpdUcxY0w4RFo3WEoyRHkxSzZUUWVORE9YeFl2NFNveitCMHNBS0VwTVRrNCtTYWpYNksrSjlUOFhZVXRTOE8wWWZGUFZqZkhIYTZORWQyODdVcUlqMnJnQlF1bjVDV3hCczFHUm5BYmd1Z3MyL2ZQakcwZmdQemdSYzR5Q3ZObFg4V2pKUnloc3U5VFRKTjd1R3NOdnprU2IyZWlyQmhEaG1vQ0Jqa0wyYnMzT3I2d2pnNnBUNVpmNGhEdDF0STBJNXo1aytxQXVSZnRhd1lmamhXYmpMS0xKOTlUVk1kRDZaTCtTenNtQkNWN05lYm96V0RUTWgrRnJPT292R09ZbUk1bWp4Smd1MVRXNnI1V0JUK2oxSjBFNmJIb2tEMWo0Wm1DWUQreVBPUW1PMm1yUTNGdC9jVmZwQWlJdzliRkgwZ1FIbXQ4QnNuZnQ2MVV3c1h6cSs2akNvY1hOOUMvRXZPblhTczZuVlNGSkVBL3l1QmNIazZxOWdqanBnRG1NTEcrNlpxR1VjRWMzZEp2THpuK3pNT0p3TDI4WUQxN3BLSXBUNnd6WFBFVFJwWS9qNHhoMkQvaFhJRVNHcTk1eTVmZE9MNmx1QT09IiBWZXJzaW9uPSI5LjkiPgogICAgPFR5cGU+UnVudGltZTwvVHlwZT4KICAgIDxVc2VybmFtZT5Vc2VyTmFtZTwvVXNlcm5hbWU+CiAgICA8RW1haWw+ZU1haWxAaG9zdC5jb208L0VtYWlsPgogICAgPE9yZ2FuaXphdGlvbj5Pcmdhbml6YXRpb248L09yZ2FuaXphdGlvbj4KICAgIDxMaWNlbnNlZERhdGU+MjAxNi0wMS0wMVQxMjowMDowMFo8L0xpY2Vuc2VkRGF0ZT4KICAgIDxFeHBpcmVkRGF0ZT4yMDk5LTEyLTMxVDEyOjAwOjAwWjwvRXhwaXJlZERhdGU+CiAgICA8UHJvZHVjdHM+CiAgICAgICAgPFByb2R1Y3Q+CiAgICAgICAgICAgIDxOYW1lPlNwaXJlLk9mZmljZSBQbGF0aW51bTwvTmFtZT4KICAgICAgICAgICAgPFZlcnNpb24+OS45OTwvVmVyc2lvbj4KICAgICAgICAgICAgPFN1YnNjcmlwdGlvbj4KICAgICAgICAgICAgICAgIDxOdW1iZXJPZlBlcm1pdHRlZERldmVsb3Blcj45OTk5OTwvTnVtYmVyT2ZQZXJtaXR0ZWREZXZlbG9wZXI+CiAgICAgICAgICAgICAgICA8TnVtYmVyT2ZQZXJtaXR0ZWRTaXRlPjk5OTk5PC9OdW1iZXJPZlBlcm1pdHRlZFNpdGU+CiAgICAgICAgICAgIDwvU3Vic2NyaXB0aW9uPgogICAgICAgIDwvUHJvZHVjdD4KICAgIDwvUHJvZHVjdHM+CiAgICA8SXNzdWVyPgogICAgICAgIDxOYW1lPklzc3VlcjwvTmFtZT4KICAgICAgICA8RW1haWw+aXNzdWVyQGlzc3Vlci5jb208L0VtYWlsPgogICAgICAgIDxVcmw+aHR0cDovL3d3dy5pc3N1ZXIuY29tPC9Vcmw+CiAgICA8L0lzc3Vlcj4KPC9MaWNlbnNlPg==";

                Spire.License.LicenseProvider.SetLicenseKey(LData);
                Workbook workbook = new Workbook();
                var mappedPathSource = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + "jlv3" + ".xlsx");
                workbook.LoadFromFile(mappedPathSource);
                Worksheet sheet = workbook.Worksheets[0];

                err_code = "2";
                // sheet.Range[1, 17].Text = fdp.Id.ToString();

                //sheet.Range[14, 9].Text = "FUEL INFO (kg)";

                sheet.Range[3, 2].Text = ((DateTime)result.Date).ToString("MM/dd/yyyy");
                sheet.Range[3, 5].Text = string.IsNullOrEmpty(result.AcType) ? "" : result.AcType;
                sheet.Range[4, 5].Text = string.IsNullOrEmpty(result.Reg) ? "" : result.Reg;
                //offblock local => stdlocal
                //sheet.Range[11, 7].Text = ((DateTime)result.STDLocal).ToString("HH:mm");
                //sheet.Range[12, 7].Text = result.OnBlockLocal != null ? ((DateTime)result.OnBlockLocal).ToString("HH:mm") : ((DateTime)result.STALocal).ToString("HH:mm");

                //reporting time local => stdlocal - 60 min
                sheet.Range[5, 4].Text = ((DateTime)reporting_time_local).ToString("HH:mm");
                // var _start =
                //sheet.Range[12, 14].Text = ((result.OnBlockLocal == null ? (DateTime)result.STALocal : (DateTime)result.OnBlockLocal)).AddMinutes(30).ToString("HH:mm");

                // ((DateTime)result.DutyEndLoccal).ToString("HH:mm");


                var _duty = Convert.ToInt32(Math.Round((

                       (result.OnBlockLocal != null ? ((DateTime)result.OnBlockLocal) : ((DateTime)result.STALocal))

                        //- (result.OffBlockLocal != null ? ((DateTime)result.OffBlockLocal)  : ((DateTime)result.STDLocal) )
                        - ((DateTime)result.ReportingTimeLocal)
                        ).TotalMinutes));

                //sheet.Range[11, 20].Text = format_to_time(result.MaxFDP);
                //var scheduled_fdp =Convert.ToInt32( Math.Round( (((DateTime)result.STALocal) - ((DateTime)result.ReportingTimeLocal)).TotalMinutes));
                sheet.Range[5, 8].Text = format_to_time(_duty + 30); //format_to_time(result.FDP+30);

                sheet.Range[5, 6].Text = format_to_time(_duty);  //format_to_time(result.FDP  );

                //TimeSpan flightTime = TimeSpan.FromMinutes(result.flight);
                //sheet.Range[23, 24].Text = result.OnBlockLocal != null
                //    ? flightTime.ToString(@"hh\:mm")
                //    : ((DateTime)result.STALocal).ToString("HH:mm");

                //TimeSpan blockTime = TimeSpan.FromMinutes(result.block);
                //sheet.Range[23, 25].Text = result.OnBlockLocal != null
                //    ? blockTime.ToString(@"hh\:mm")
                //    : ((DateTime)result.STALocal).ToString("HH:mm");


                //sheet.Range[23, 24].Text = result.OnBlockLocal != null ? ((DateTime)result.flight).ToString("HH:mm") : ((DateTime)result.STALocal).ToString("HH:mm");
                //sheet.Range[23, 25].Text = result.OnBlockLocal != null ? ((DateTime)result.block).ToString("HH:mm") : ((DateTime)result.STALocal).ToString("HH:mm");





                //  sheet.Range[11, 14].Text = format_to_time(result.flight); //flt
                // sheet.Range[11, 15].Text = format_to_time(result.block); //blk
                // sheet.Range[11, 16].Text = format_to_time(result.night); //night
                //sheet.Range[11, 17].Text = format_to_time(result.over); //rem


                int ln_cockpit = 15;
                int ln_cabin = 21;
                int ln_other = 15;

                var cockpit = new List<string>() { "TRI", "TRE", "P1", "P2" };
                var cabin = new List<string>() { "ISCCM", "SCCM", "CCM" };

                foreach (var c in result.crew)
                {
                    if (cockpit.Contains(c.JobGroup))
                    {
                        sheet.Range[ln_cockpit, 2].Text = string.IsNullOrEmpty(c.Position) ? "" : get_position(c.Position);
                        sheet.Range[ln_cockpit, 3].Text = string.IsNullOrEmpty(c.Name) ? "" : c.Name + " (" + c.JobGroup + ")";
                        ln_cockpit++;
                    }
                    else if (cabin.Contains(c.JobGroup))
                    {
                        sheet.Range[ln_cabin, 2].Text = string.IsNullOrEmpty(c.Position) ? "" : get_position(c.Position);
                        sheet.Range[ln_cabin, 3].Text = string.IsNullOrEmpty(c.Name) ? "" : c.Name + " (" + c.JobGroup + ")";
                        ln_cabin++;
                    }
                    else
                    {
                        sheet.Range[ln_other, 7].Text = string.IsNullOrEmpty(c.Position) ? "" : get_position(c.Position);
                        sheet.Range[ln_other, 8].Text = string.IsNullOrEmpty(c.Name) ? "" : c.Name + " (" + c.JobGroup + ")";
                        ln_other++;
                    }
                }

                var ln_leg = 9;  //18,2
                foreach (var leg in result.legs)
                {
                    //sheet.Range[ln_leg, 2].Text = ((DateTime)leg.STD).ToString("MM");
                    //sheet.Range[ln_leg, 3].Text = ((DateTime)leg.STD).ToString("dd");
                    sheet.Range[ln_leg, 2].Text = leg.FlightNumber;
                    sheet.Range[ln_leg, 3].Text = leg.FromAirportIATA;
                    sheet.Range[ln_leg, 4].Text = leg.ToAirportIATA;
                    //sheet.Range[ln_leg, 7].Text = string.IsNullOrEmpty(leg.FlightType) ? "S" : (leg.FlightType.StartsWith("S") ? "S" : "N");
                    //sheet.Range[ln_leg, 8].Text = string.IsNullOrEmpty(leg.PF) ? "" : leg.PF;

                    //sheet.Range[ln_leg, 9].Text = leg.FuelRemaining == null ? "" : Convert.ToInt32(leg.FuelRemaining).ToString();
                    //sheet.Range[ln_leg, 10].Text = leg.FuelUplift == null ? "" : Convert.ToInt32(leg.FuelUplift).ToString();
                    //sheet.Range[ln_leg, 11].Text = leg.FuelDensity == null ? "0.79" : Math.Round((decimal)leg.FuelDensity, 2).ToString();
                    //sheet.Range[ln_leg, 12].Text = leg.FuelTotal == null ? "" : Convert.ToInt32(leg.FuelTotal).ToString();
                    //sheet.Range[ln_leg, 13].Text = leg.FuelUsed == null ? "" : Convert.ToInt32(leg.FuelUsed).ToString();

                    //sheet.Range[ln_leg, 14].Text = leg.PaxAdult == null ? "" : leg.PaxAdult.ToString();
                    //sheet.Range[ln_leg, 14].Text = leg.PaxMale == null ? "" : leg.PaxMale.ToString();
                    //heet.Range[ln_leg, 15].Text = leg.PaxFemale == null ? "" : leg.PaxFemale.ToString();
                    //sheet.Range[ln_leg, 16].Text = leg.PaxChild == null ? "" : leg.PaxChild.ToString();
                    //sheet.Range[ln_leg, 17].Text = leg.PaxInfant == null ? "" : leg.PaxInfant.ToString();
                    //sheet.Range[ln_leg, 18].Text = ((leg.PaxInfant == null ? 0 : leg.PaxInfant)
                    //+(leg.PaxChild == null ? 0 : leg.PaxChild)
                    //+ (leg.PaxAdult == null ? 0 : leg.PaxAdult)).ToString();

                    //sheet.Range[ln_leg, 19].Text = (leg.BaggageWeight + leg.CargoWeight).ToString();



                    sheet.Range[ln_leg, 5].Text = leg.BlockOff == null ? "" : ((DateTime)leg.BlockOff).ToString("HH:mm");
                    sheet.Range[ln_leg, 6].Text = leg.BlockOn == null ? "" : ((DateTime)leg.BlockOn).ToString("HH:mm");
                    sheet.Range[ln_leg, 7].Text = leg.BlockTime == null ? "" : format_to_time(leg.BlockTime);
                    sheet.Range[ln_leg, 8].Text = leg.TakeOff == null ? "" : ((DateTime)leg.TakeOff).ToString("HH:mm");
                    sheet.Range[ln_leg, 9].Text = leg.Landing == null ? "" : ((DateTime)leg.Landing).ToString("HH:mm");
                    sheet.Range[ln_leg, 10].Text = leg.FlightTime == null ? "" : format_to_time(leg.FlightTime);

                    //sheet.Range[ln_leg, 26].Text = leg.RemDuty == null ? "" : ((DateTime)leg.RemDuty).ToString("HH:mm");
                    //sheet.Range[ln_leg, 26].Text = leg.RemDuty == null ? "" : TimeSpan.FromMinutes((int)leg.RemDuty).ToString(@"hh\:mm");


                    //sheet.Range[ln_leg, 16].Text = leg.BlockOff == null || leg.BlockOn == null ? "" : format_to_time(leg.NightTime);


                    //sheet.Range[ln_leg, 17].Text = leg.RemDuty == null ? "" : format_to_time(leg.RemDuty);
                    //sheet.Range[ln_leg, 18].Text = string.IsNullOrEmpty(leg.DelayCode) ? "" : leg.DelayCode;
                    ln_leg++;


                }

                TimeSpan flightTime = TimeSpan.FromMinutes(result.flight);
                sheet.Range[13, 10].Text = result.OnBlockLocal != null
                    ? flightTime.ToString(@"hh\:mm")
                    : ((DateTime)result.STALocal).ToString("HH:mm");

                TimeSpan blockTime = TimeSpan.FromMinutes(result.block);
                sheet.Range[13, 7].Text = result.OnBlockLocal != null
                    ? blockTime.ToString(@"hh\:mm")
                    : ((DateTime)result.STALocal).ToString("HH:mm");
                //var nots = new List<string>();
                //ln_leg = 27;
                //foreach (var leg in result.legs)
                //{

                //    //sheet.Range[ln_leg, 2].Text = string.IsNullOrEmpty(leg.SerialNo) ? "" : leg.SerialNo;
                //    //sheet.Range[ln_leg, 5].Text = string.IsNullOrEmpty(leg.LTR) ? "" : leg.LTR;


                //    //(leg.PaxInfant ?? 0 + leg.PaxAdult ?? 0 + leg.PaxChild ?? 0).ToString();





                //    //CommanderNote
                //    if (!string.IsNullOrEmpty(leg.CommanderNote))
                //        nots.Add(leg.CommanderNote);
                //    ln_leg++;

                //}





                //sheet.Range[27, 8].Text = String.Join("\n", nots); //commander note

                if (!string.IsNullOrEmpty(result.legs.First().JLSignedBy))
                {
                    sheet.Range[38, 1].Text = (string.IsNullOrEmpty(result.legs.First().JLSignedBy) ? "" : result.legs.First().JLSignedBy)
                        + "\n"
                        + ((DateTime)result.legs.First().JLDatePICApproved).ToString("yyyy-MMM-dd HH:mm");  //pic name

                }


                //var forms = new List<string>();
                //if (result.asr)
                //{
                //    forms.Add("ASR");
                //    sheet.Range[32, 14].Text = "*";
                //}
                //if (result.vr)
                //{
                //    forms.Add("VOYAGE REPORT");
                //    sheet.Range[32, 17].Text = "*";
                //}
                //if (forms.Count > 0)
                //    sheet.Range[24, 7].Text = string.Join(", ", forms);  //attached forms
                // else
                //    sheet.Range[24, 7].Text = "NONE";




                var name = "JourneyLog-" + fdp.Id;
                var mappedPath = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + name + ".xlsx");



                workbook.SaveToFile(mappedPath, ExcelVersion.Version2016);

                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StreamContent(new FileStream(mappedPath, FileMode.Open, FileAccess.Read));
                response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = name + ".xlsx";
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");


                return response;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, msg);
                return response;
            }

        }
        public class CLJLData
        {
            public int? CrewId { get; set; }
            public bool? IsPositioning { get; set; }
            public int? PositionId { get; set; }
            public string Position { get; set; }
            public string Name { get; set; }
            public int? GroupId { get; set; }
            public string JobGroup { get; set; }
            public string JobGroupCode { get; set; }
            public int? GroupOrder { get; set; }
            public int IsCockpit { get; set; }
            public string PassportNo { get; set; }

            public List<string> Legs { get; set; }
            public string LegsStr { get; set; }

            public int? FlightId { get; set; }
            public string PID { get; set; }

            public string Mobile { get; set; }
            public string Address { get; set; }
            public string NID { get; set; }


            //public string ATLNO { get; set; }
            //public DateTime Date { get; set; }
            //public int ScheduledTime { get; set; }
            //public int STD { get; set; }
            //public int MaxFDP { get; set; }
            //public DateTime ReportingTime { get; set; }
            //public DateTime EndFDP { get; set; }
            //public int FDP { get; set; }

            public double? TotalFlightTime { get; set; }
            public double? TotalBlockTime { get; set; }
            //public int TotalNight { get; set; }
            //public DateTime Night { get; set; }


        }




    }
}
