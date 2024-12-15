using ApiReportFlight.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ApiReportFlight.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ReportController : ApiController
    {
        int IsHoliday(DateTime? dt, string dayname)
        {
            if (dayname == "FRI")
                return 1;
            if (dayname == "THU")
                return 1;
            return 0;
        }
        [Route("api/crew/flight/summary")]
        public IHttpActionResult GetCrewFlightSummary(DateTime df, DateTime dt, string grps = "All", string actype = "All", string cid = "-1")
        {
            var total_days = (dt.Date - df.Date).Days;
            var result = new List<CrewSummaryDto>();

            var context = new ppa_Entities();

            df = df.Date;
            dt = dt.Date.AddDays(1);
            // dataSource: ['All', 'Cockpit', 'Cabin', 'IP', 'P1', 'P2', 'SCCM', 'CCM', 'ISCCM'],
            var crew_grps = new List<string>() { "TRE", "TRI", "P1", "P2", "ISCCM", "SCCM", "CCM" ,"LTC"}; 
            if (grps == "Cockpit")
                crew_grps = new List<string>() { "TRE", "TRI", "P1", "P2" ,"LTC"};
            else if (grps == "Cabin")
                crew_grps = new List<string>() { "ISCCM", "SCCM", "CCM" };
            else if (grps == "IP")
                crew_grps = new List<string>() { "TRE", "TRI","LTC" };
            else if (grps == "P1")
                crew_grps = new List<string>() { "P1" };
            else if (grps == "P2")
                crew_grps = new List<string>() { "P2" };
            else if (grps == "ISCCM")
                crew_grps = new List<string>() { "ISCCM" };
            else if (grps == "SCCM")
                crew_grps = new List<string>() { "SCCM" };
            else if (grps == "CCM")
                crew_grps = new List<string>() { "CCM" };

            string crew_types = "";// "21,22,26" ;
            if (actype == "AIRBUS")
                crew_types = "26";
            if (actype == "MD")
                crew_types = "21";
            if (actype == "FOKKER")
                crew_types = "22";


            var qry_crew = from x in context.ViewCrews
                           where crew_grps.Contains(x.JobGroup) 
                           select x;
            if (!string.IsNullOrEmpty(crew_types))
            {
                qry_crew = qry_crew.Where(q => q.ValidTypes.Contains(crew_types));
            }
            var _cid = Convert.ToInt32(cid);
            if (_cid != -1)
                qry_crew = qry_crew.Where(q => q.Id == _cid);
            var ds_crew = qry_crew.Select(q => new RptFDPGRP()
            {
                CrewId = q.Id,
                ScheduleName = q.ScheduleName,
                FirstName = q.FirstName,
                LastName = q.LastName,
                JobGroup = q.JobGroup,
                JobGroupCode = q.JobGroupCode,
                //GroupOrder = getOrder(q.JobGroup),

            }).ToList();

            var crew_ids = ds_crew.Select(q => q.CrewId).ToList();

             

            var crew_flights = (from x in context.ViewCrewFlightApps
                                where crew_ids.Contains(x.CrewId) && x.STDDay >= df && x.STDDay < dt && x.FlightStatusId!=4
                                select new { x.CrewId, x.IsPositioning, x.Position ,x.BlockTime,x.FlightTime}
                             ).ToList();



            var qry_fdps = from x in context.RptFDP2
                           where x.STDDay >= df && x.STDDay < dt && crew_ids.Contains(x.CrewId)
                           select x;

            var final_crew_ids=qry_fdps.Select(q=>q.CrewId).ToList();

            var ds_fdps = qry_fdps.ToList();

            var ds_fdps_total = (from x in ds_fdps
                                 group x by new { x.CrewId, x.Name, x.ScheduleName, x.FirstName, x.LastName, x.JobGroup, x.JobGroupCode, x.JobGroupRoot } into grp
                                 select new RptFDPGRP()
                                 {

                                     CrewId = grp.Key.CrewId,
                                     ScheduleName = grp.Key.ScheduleName,
                                     Name = grp.Key.Name,
                                     FirstName = grp.Key.FirstName,
                                     LastName = grp.Key.LastName,
                                     JobGroup = grp.Key.JobGroup,
                                     JobGroupCode = grp.Key.JobGroupCode,
                                     JobGroupRoot = grp.Key.JobGroupRoot,
                                     BlockTime = grp.Sum(q => q.BlockTime) ?? 0,
                                     IndexF = Math.Round((grp.Sum(q => q.BlockTime * 1.0 / 60) ?? 0.0) / total_days, 2),
                                     FlightTime = grp.Sum(q => q.FlightTime) ?? 0,
                                     FixedFlightTime = grp.Sum(q => q.FixedFlightTime) ?? 0,
                                     ScheduledTime = grp.Sum(q => q.ScheduledTime) ?? 0,
                                     Legs = grp.Sum(q => q.Legs) ?? 0,
                                     Leg1 = grp.Sum(q => q.Leg1),
                                     Leg2 = grp.Sum(q => q.Leg2),
                                     Leg3 = grp.Sum(q => q.Leg3),
                                     Leg4 = grp.Sum(q => q.Leg4),
                                     Leg5 = grp.Sum(q => q.Leg5),
                                     Leg6 = grp.Sum(q => q.Leg6),
                                     Leg7 = grp.Sum(q => q.Leg7),
                                     Leg8 = grp.Sum(q => q.Leg8),
                                     Leg2X = (grp.Sum(q => q.Leg1)) + (grp.Sum(q => q.Leg2)) + (grp.Sum(q => q.Leg3)),
                                     Leg4X = grp.Sum(q => q.Leg4) + grp.Sum(q => q.Leg5) + grp.Sum(q => q.Leg6) + grp.Sum(q => q.Leg7) + grp.Sum(q => q.Leg8),
                                     Positioning = 0, //grp.Sum(q => q.Positioning) ?? 0,
                                     Canceled = 0, //grp.Sum(q => q.Canceled) ?? 0,
                                     PositioningFixTime = 0, //grp.Sum(q => q.PositioningFixTime) ?? 0,
                                     CanceledFixTime = 0, //grp.Sum(q => q.CanceledFixTime) ?? 0,


                                     //DeadHead=grp.Sum(q=>q.)
                                     EarlyDeparture = grp.Sum(q => q.EarlyDeparture),
                                     LateArrival = grp.Sum(q => q.LateArrival),
                                     HolidayDeparture = grp.Sum(q => IsHoliday(q.STDDay, q.DayName)),
                                     WOCLDuration = grp.Sum(q => q.WOCLDuration) ?? 0,
                                     WOCLLND = grp.Sum(q => q.WOCLLND) ?? 0,
                                     WOCLTO = grp.Sum(q => q.WOCLTO) ?? 0,
                                     XAirportLND = grp.Sum(q => q.XAirportLND) ?? 0,
                                     XAirportTO = grp.Sum(q => q.XAirportTO) ?? 0,
                                     FDPs = grp.OrderBy(q => q.STD).ToList(),
                                     DH= crew_flights.Where (q=>q.CrewId== grp.Key.CrewId && q.IsPositioning==true).Count(),
                                     Instructor= crew_flights.Where(q => q.CrewId == grp.Key.CrewId && q.IsPositioning != true 
                                       && (q.Position=="IP" || q.Position=="TRE" || q.Position=="TRI" || q.Position=="LTC" || q.Position=="ISCCM")  ).Count(),
                                     InstructorBlock = crew_flights.Where(q => q.CrewId == grp.Key.CrewId && q.IsPositioning != true
                                        && (q.Position == "IP" || q.Position == "TRE" || q.Position == "TRI" || q.Position == "LTC" || q.Position == "ISCCM")).Sum(q=>q.BlockTime),

                                     OBS = crew_flights.Where(q => q.CrewId == grp.Key.CrewId && q.IsPositioning != true
                                       && (q.Position == "OBS")).Count(),
                                     OBSBlock = crew_flights.Where(q => q.CrewId == grp.Key.CrewId && q.IsPositioning != true
                                        && (q.Position == "OBS")).Sum(q => q.BlockTime),

                                     Check = crew_flights.Where(q => q.CrewId == grp.Key.CrewId && q.IsPositioning != true
                                       && (q.Position == "Check")).Count(),
                                     CheckBlock = crew_flights.Where(q => q.CrewId == grp.Key.CrewId && q.IsPositioning != true
                                        && (q.Position == "Check")).Sum(q => q.BlockTime),

                                 }).OrderBy(q => q.GroupOrder).ThenByDescending(q => q.FixedFlightTime).ThenBy(q => q.LastName).ToList();


            //safety
            var qry_safety = from x in context.RptFDPItems
                             where x.PositionId == 1162 && x.STDDay >= df && x.STDDay < dt && crew_ids.Contains(x.CrewId)
                             select x;
            var ds_safety = qry_safety.ToList();
            var ds_safety_total = (from x in ds_safety
                                   group x by new { x.CrewId } into grp
                                   select new
                                   {
                                       grp.Key.CrewId,
                                       count = grp.Count(),
                                       blocktime = grp.Sum(q => q.BlockTime),
                                   }).ToList();
                               



            var qry_nofdp = from x in context.RptNoFDPs
                            where x.Date >= df && x.Date < dt && crew_ids.Contains(x.CrewId)
                            select x;
            var ds_nofdp = qry_nofdp.ToList();
            final_crew_ids=final_crew_ids.Concat(ds_nofdp.Select(q=>q.CrewId)).ToList();

            var ds_nofdp_total = (from x in ds_nofdp
                                  group x by new { x.CrewId, x.DutyTypeTitle, x.DutyType } into grp
                                  select new
                                  {
                                      grp.Key.CrewId,
                                      grp.Key.DutyType,
                                      grp.Key.DutyTypeTitle,
                                      Duration = grp.Sum(q => q.Duration ?? 0),
                                      Count = grp.Count(),
                                      FX = grp.Sum(q => q.FX ?? 0),

                                  }).ToList();


            var qry_refuse = from x in context.RptRefuses
                             where x.Date >= df && x.Date < dt && crew_ids.Contains(x.CrewId) && (x.Sick == null || x.Sick == 0)
                             select x;
            var ds_refuse = qry_refuse.ToList();

            var ds_refuse_total = (from x in ds_refuse
                                   group x by new { x.CrewId, x.DutyTypeTitle, x.DutyType } into grp
                                   select new
                                   {
                                       grp.Key.CrewId,
                                       grp.Key.DutyType,
                                       grp.Key.DutyTypeTitle,
                                       Duration = 0,
                                       Count = grp.Count(),
                                       FX = 0

                                   }).ToList();

            foreach (var crew in ds_crew)
            {
                crew.GroupOrder = getOrder(crew.JobGroup);
                var nofdps = ds_nofdp_total.Where(q => q.CrewId == crew.CrewId).ToList();
                var refs = ds_refuse_total.Where(q => q.CrewId == crew.CrewId).ToList();

                var safety = ds_safety_total.FirstOrDefault(q => q.CrewId == crew.CrewId);
                if (safety != null)
                {
                    crew.Safety = safety.count;
                    crew.SafetyBlock = safety.blocktime!=null?(int)safety.blocktime:0;
                }
                foreach (var rec in refs)
                {
                    crew.Refuse += rec.Count;
                }
                foreach (var rec in nofdps)
                {
                    if (rec.DutyTypeTitle == "StandBy")
                    {
                        crew.Standby += rec.Count;
                        crew.StandbyFixTime += rec.Count * 120;
                       
                    }
                    if (rec.DutyTypeTitle == "STBY-PM")
                        crew.StandbyPM += rec.Count;
                    if (rec.DutyTypeTitle == "STBY-AM")
                        crew.StandbyAM += rec.Count;
                    if (rec.DutyTypeTitle.StartsWith("Reserve"))
                        crew.Reserve += rec.Count;
                    if (rec.DutyTypeTitle == "Mission") { crew.Mission += rec.Duration; }
                    if (rec.DutyType == 300002) { crew.FX300002 += rec.Duration; }
                    if (rec.DutyType == 300003) { crew.FX300003 += rec.Duration; }
                    if (rec.DutyType == 300004) { crew.FX300004 += rec.Duration; }
                    if (rec.DutyType == 300005) { crew.FX300005 += rec.Duration; }
                    if (rec.DutyType == 300006) { crew.FX300006 += rec.Duration; }
                    if (rec.DutyType == 300007) { crew.FX300007 += rec.Duration; }


                }

                var fdp = ds_fdps_total.Where(q => q.CrewId == crew.CrewId).FirstOrDefault();
                if (fdp != null)
                {
                    crew.BlockTime = fdp.BlockTime;
                    crew.IndexF = fdp.IndexF;
                    crew.FlightTime = fdp.FlightTime;
                    crew.FixedFlightTime = fdp.FixedFlightTime;
                    crew.ScheduledTime = fdp.ScheduledTime;
                    crew.Leg1 = fdp.Leg1;
                    crew.Leg2 = fdp.Leg2;
                    crew.Leg3 = fdp.Leg3;
                    crew.Leg4 = fdp.Leg4;
                    crew.Leg5 = fdp.Leg5;
                    crew.Leg6 = fdp.Leg6;
                    crew.Leg7 = fdp.Leg7;
                    crew.Leg8 = fdp.Leg8;
                    crew.Legs = fdp.Legs;
                    crew.Leg2X = fdp.Leg2X;
                    crew.Leg4X = fdp.Leg4X;
                    crew.Positioning = fdp.Positioning;
                    crew.Canceled = fdp.Canceled;
                    crew.PositioningFixTime = fdp.PositioningFixTime;
                    crew.CanceledFixTime = fdp.CanceledFixTime;
                    crew.EarlyDeparture = fdp.EarlyDeparture;
                    crew.LateArrival = fdp.LateArrival;
                    crew.HolidayDeparture = fdp.HolidayDeparture;
                    crew.WOCLDuration = fdp.WOCLDuration;
                    crew.WOCLLND = fdp.WOCLLND;
                    crew.WOCLTO = fdp.WOCLTO;
                    crew.XAirportLND = fdp.XAirportLND;
                    crew.XAirportTO = fdp.XAirportTO + fdp.XAirportLND;
                    crew.FDPs = fdp.FDPs.ToList();
                    crew.Instructor = fdp.Instructor;
                    crew.InstructorBlock = fdp.InstructorBlock;
                    crew.OBS = fdp.OBS;
                    crew.OBSBlock = fdp.OBSBlock;
                    crew.Check = fdp.Check;
                    crew.CheckBlock = fdp.CheckBlock;
                    crew.DH = fdp.DH;

                    //crew.FixTimeTotal += fdp.FixedFlightTime;
                }
            }

            //var qry_flights = from x in context.ViewLegCrews
            //                  where x.STDLocal >= df && x.STDLocal < dt && x.FlightStatusID != 4
            //                  select x;

            //var ds_flights =(from x in qry_flights

            //               group x by new { x.CrewId, x.ScheduleName, x.JobGroup, x.JobGroupCode, x.Name, x.PID } into _grp
            //               select new CrewSummaryDto()
            //               {
            //                   CrewId = _grp.Key.CrewId,
            //                   ScheduleName = _grp.Key.ScheduleName,
            //                   JobGroup = _grp.Key.JobGroup,
            //                   JobGropCode = _grp.Key.JobGroupCode,
            //                   Name = _grp.Key.Name,
            //                   PID = _grp.Key.PID,

            //                   Legs = _grp.Where(q => q.IsPositioning == false).Count(),
            //                   DH = _grp.Where(q => q.IsPositioning == true).Count(),
            //                   FlightTime = _grp.Sum(q => q.FlightTime),
            //                   BlockTime = _grp.Sum(q => q.BlockTime),
            //                   JLFlightTime = _grp.Sum(q => q.JL_FlightTime),
            //                   JLBlockTime = _grp.Sum(q => q.JL_BlockTime),
            //                   FixTime = _grp.Sum(q => q.FixTime),
            //               }
            //              ).ToList();

            //foreach(var c in ds_crew)
            //{
            //    var obj = new CrewSummaryDto() {CrewId=c.Id,ScheduleName=c.ScheduleName,JobGropCode=c.JobGroupCode,JobGroup=c.JobGroup,Name=c.Name,PID=c.PID };
            //    var flt = ds_flights.Where(q => q.CrewId == c.Id).FirstOrDefault();
            //    if (flt != null)
            //    {
            //        obj.Legs = flt.Legs;
            //        obj.DH = flt.DH;
            //        obj.FlightTime=flt.FlightTime;
            //        obj.BlockTime=flt.BlockTime;
            //        obj.JLFlightTime = flt.FlightTime;
            //        obj.JLBlockTime = flt.BlockTime;
            //        obj.FixTime = flt.FixTime;
            //    }

            //    result.Add(obj);
            //}

            var _result=ds_crew.Where(q=> final_crew_ids.Contains(q.CrewId)).ToList();
             
            return Ok(_result.OrderBy(q => q.GroupOrder).ThenByDescending(q => q.FixTimeTotal).ThenBy(q => q.LastName).ToList());


        }



        public partial class RptFDPGRP
        {
            //  public int FDPId { get; set; }
            //  public int FDPId { get; set; }
            public Nullable<int> CrewId { get; set; }
            public string Name { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string ScheduleName { get; set; }
            public string JobGroup { get; set; }
            public string JobGroupCode { get; set; }
            public string JobGroupRoot { get; set; }
            // public Nullable<System.DateTime> STDDay { get; set; }
            // public Nullable<System.DateTime> STDDayUTC { get; set; }
            // public Nullable<System.DateTime> STDLocal { get; set; }
            // public Nullable<System.DateTime> STD { get; set; }
            // public Nullable<System.DateTime> STALocal { get; set; }
            // public Nullable<System.DateTime> STA { get; set; }
            public int EarlyDeparture { get; set; }
            public int LateArrival { get; set; }
            public int WOCLTO { get; set; }
            public int WOCLLND { get; set; }
            public int WOCLDuration { get; set; }
            public int XAirportTO { get; set; }
            public int XAirportLND { get; set; }
            public int HolidayDeparture { get; set; }
            // public string Flights { get; set; }
            // public string Route { get; set; }
            public int Legs { get; set; }
            public int FlightTime { get; set; }
            public int FixedFlightTime { get; set; }
            public int BlockTime { get; set; }
            public int ScheduledTime { get; set; }
            public int Leg1 { get; set; }
            public int Leg2 { get; set; }
            public int Leg3 { get; set; }
            public int Leg4 { get; set; }
            public int Leg5 { get; set; }
            public int Leg6 { get; set; }
            public int Leg7 { get; set; }
            public int Leg8 { get; set; }

            public int Leg2X { get; set; }
            public int Leg4X { get; set; }
            public int DH { get; set; }
            public int Instructor { get; set; }
            public int? InstructorBlock { get; set; }

            public int OBS { get; set; }
            public int? OBSBlock { get; set; }


            public int Check { get; set; }
            public int? CheckBlock { get; set; }

            public int Safety { get; set; }
            public int SafetyBlock { get; set; }

            public int IntFDP { get; set; }
            public int IntFlt { get; set; }

            public int Index { get; set; }
            public double IndexF { get; set; }

            public int Standby { get; set; }
            public int StandbyFixTime { get; set; }

            public int StandbyAM { get; set; }
            public int StandbyAMFixTime { get; set; }

            public int StandbyPM { get; set; }
            public int StandbyPMFixTime { get; set; }

            public int Reserve { get; set; }
            public int ReserveFixTime { get; set; }

            public int Refuse { get; set; }

            public int Canceled { get; set; }
            public int CanceledFixTime { get; set; }

            public int Positioning { get; set; }
            public int PositioningFixTime { get; set; }



            public int Mission { get; set; }

            public int FX300002 { get; set; }
            public int FX300003 { get; set; }
            public int FX300004 { get; set; }
            public int FX300005 { get; set; }
            public int FX300006 { get; set; }
            public int FX300007 { get; set; }
            public int FixTimeTotal
            {
                get
                {
                    return this.FX300002 + this.FX300003 + this.FX300004 + this.FX300005 + this.FX300006 + this.FX300007
                           + this.Mission
                           + this.FixedFlightTime
                           + this.StandbyFixTime
                           + this.CanceledFixTime
                           + this.PositioningFixTime;
                }
            }
            // public string DayName { get; set; }
            // public Nullable<int> Year { get; set; }
            // public string MonthName { get; set; }
            // public Nullable<int> Month { get; set; }
            public List<RptFDP2> FDPs { get; set; }

            public int GroupOrder { get; set; }
        }
        //api/flight/daily
        [Route("api/flight/daily")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetFlightsDaily(DateTime df, DateTime dt, string regs, string routes, string from, string to, string no
            , string status
            , string type2
            , string idx
            , string chr
            , string dateref
            )
        {
            var cmd = "select * from viewflightdaily ";
            try
            {
                var context = new ppa_Entities();
                var dayprm = dateref == "std" ? "STDDayLocal" : "TakeOffDayLocal";

                // var cmd = "select * from viewflightdaily ";
                string whr = "  (" + dayprm + ">= '" + df.ToString("yyyy-MM-dd") + "' and " + dayprm + "<='" + dt.ToString("yyyy-MM-dd") + "')";

                if (!string.IsNullOrEmpty(status) && status != "-1")
                {
                    //all
                    if (status == "-2")
                    {

                    }
                    //all excludes cnl
                    else if (status == "-3")
                    {

                        var _whr = "status <> 4";
                        whr += " AND " + _whr;
                    }
                    else
                    {
                        var _regs = status.Split('_').ToList();
                        var col = _regs.Select(q => "status=" + q).ToList();
                        var _whr = "(" + string.Join(" OR ", col) + ")";
                        whr += " AND " + _whr;
                    }

                }
                if (!string.IsNullOrEmpty(type2) && type2 != "-1")
                {
                    var _regs = type2.Split('_').ToList();
                    var col = _regs.Select(q => "FlightType2=N'" + q + "'").ToList();
                    var _whr = "(" + string.Join(" OR ", col) + ")";
                    whr += " AND " + _whr;
                }

                if (!string.IsNullOrEmpty(idx) && idx != "-1")
                {
                    var _regs = idx.Split('_').ToList();
                    var col = _regs.Select(q => "FlightIndex=N'" + q + "'").ToList();
                    var _whr = "(" + string.Join(" OR ", col) + ")";
                    whr += " AND " + _whr;
                }

                if (!string.IsNullOrEmpty(chr) && chr != "-1")
                {
                    var _regs = chr.Split('_').ToList();
                    var col = _regs.Select(q => "ChrTitle=N'" + q + "'").ToList();
                    var _whr = "(" + string.Join(" OR ", col) + ")";
                    whr += " AND " + _whr;
                }

                if (!string.IsNullOrEmpty(regs) && regs != "-1")
                {
                    var _regs = regs.Split('_').ToList();
                    var col = _regs.Select(q => "Register='" + q + "'").ToList();
                    var _whr = "(" + string.Join(" OR ", col) + ")";
                    whr += " AND " + _whr;
                }

                if (!string.IsNullOrEmpty(from) && from != "-1")
                {
                    var _regs = from.Split('_').ToList();
                    var _whr = "(" + string.Join(" OR ", _regs.Select(q => "FromAirportIATA='" + q + "'").ToList()) + ")";
                    whr += " AND " + _whr;
                }

                if (!string.IsNullOrEmpty(to) && to != "-1")
                {
                    var _regs = to.Split('_').ToList();
                    var _whr = "(" + string.Join(" OR ", _regs.Select(q => "ToAirportIATA='" + q + "'").ToList()) + ")";
                    whr += " AND " + _whr;
                }

                if (!string.IsNullOrEmpty(no) && no != "-1")
                {
                    var _regs = no.Split('_').ToList();
                    var _whr = "(" + string.Join(" OR ", _regs.Select(q => "FlightNumber='" + q + "'").ToList()) + ")";
                    whr += " AND " + _whr;
                }

                if (!string.IsNullOrEmpty(routes) && routes != "-1")
                {
                    var _regs = routes.Split('_').ToList();
                    var _whr = "(" + string.Join(" OR ", _regs.Select(q => "Route like '%" + q + "%'").ToList()) + ")";
                    whr += " AND " + _whr;
                }

                cmd = cmd + " WHERE " + whr + " ORDER BY STD,Register";

                var flts = context.ViewFlightDailies
                            .SqlQuery(cmd)
                            .ToList<ViewFlightDaily>();

                //var result = await courseService.GetEmployeeCertificates(id);

                return Ok(flts);
            }
            catch (Exception ex)
            {
                return Ok(cmd);
            }

        }


        [Route("api/flight/daily/int")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetFlightsDailyINT(DateTime df, DateTime dt, string regs, string routes, string from, string to, string no
            , string status
            , string type2
            , string idx
            , string chr
            , string dateref
            )
        {
            var cmd = "select * from viewflightdaily ";
            try
            {
                var context = new ppa_Entities();
                var dayprm = dateref == "std" ? "STDDayLocal" : "TakeOffDayLocal";

                // var cmd = "select * from viewflightdaily ";
                string whr = "  (" + dayprm + ">= '" + df.ToString("yyyy-MM-dd") + "' and " + dayprm + "<='" + dt.ToString("yyyy-MM-dd") + "')";

                if (!string.IsNullOrEmpty(status) && status != "-1")
                {
                    //all
                    if (status == "-2")
                    {

                    }
                    //all excludes cnl
                    else if (status == "-3")
                    {

                        var _whr = "status <> 4";
                        whr += " AND " + _whr;
                    }
                    else
                    {
                        var _regs = status.Split('_').ToList();
                        var col = _regs.Select(q => "status=" + q).ToList();
                        var _whr = "(" + string.Join(" OR ", col) + ")";
                        whr += " AND " + _whr;
                    }

                }
                if (!string.IsNullOrEmpty(type2) && type2 != "-1")
                {
                    var _regs = type2.Split('_').ToList();
                    var col = _regs.Select(q => "FlightType2='" + q + "'").ToList();
                    var _whr = "(" + string.Join(" OR ", col) + ")";
                    whr += " AND " + _whr;
                }

                if (!string.IsNullOrEmpty(idx) && idx != "-1")
                {
                    var _regs = idx.Split('_').ToList();
                    var col = _regs.Select(q => "FlightIndex='" + q + "'").ToList();
                    var _whr = "(" + string.Join(" OR ", col) + ")";
                    whr += " AND " + _whr;
                }

                if (!string.IsNullOrEmpty(chr) && chr != "-1")
                {
                    var _regs = chr.Split('_').ToList();
                    var col = _regs.Select(q => "ChrTitle='" + q + "'").ToList();
                    var _whr = "(" + string.Join(" OR ", col) + ")";
                    whr += " AND " + _whr;
                }

                if (!string.IsNullOrEmpty(regs) && regs != "-1")
                {
                    var _regs = regs.Split('_').ToList();
                    var col = _regs.Select(q => "Register='" + q + "'").ToList();
                    var _whr = "(" + string.Join(" OR ", col) + ")";
                    whr += " AND " + _whr;
                }

                if (!string.IsNullOrEmpty(from) && from != "-1")
                {
                    var _regs = from.Split('_').ToList();
                    var _whr = "(" + string.Join(" OR ", _regs.Select(q => "FromAirportIATA='" + q + "'").ToList()) + ")";
                    whr += " AND " + _whr;
                }

                if (!string.IsNullOrEmpty(to) && to != "-1")
                {
                    var _regs = to.Split('_').ToList();
                    var _whr = "(" + string.Join(" OR ", _regs.Select(q => "ToAirportIATA='" + q + "'").ToList()) + ")";
                    whr += " AND " + _whr;
                }

                if (!string.IsNullOrEmpty(no) && no != "-1")
                {
                    var _regs = no.Split('_').ToList();
                    var _whr = "(" + string.Join(" OR ", _regs.Select(q => "FlightNumber='" + q + "'").ToList()) + ")";
                    whr += " AND " + _whr;
                }

                if (!string.IsNullOrEmpty(routes) && routes != "-1")
                {
                    var _regs = routes.Split('_').ToList();
                    var _whr = "(" + string.Join(" OR ", _regs.Select(q => "Route like '%" + q + "%'").ToList()) + ")";
                    whr += " AND " + _whr;
                }

                cmd = cmd + " WHERE " + whr + " ORDER BY STD,Register";

                var flts = context.ViewFlightDailies
                            .SqlQuery(cmd)
                            .ToList<ViewFlightDaily>();

                //var result = await courseService.GetEmployeeCertificates(id);

                return Ok(flts);
            }
            catch (Exception ex)
            {
                return Ok(cmd);
            }

        }

        [Route("api/delay/items/{flt}")]
        public IHttpActionResult GetDelayItems(int flt)
        {
            var context = new ppa_Entities();
            var result = context.ViewFlightDelays.Where(q => q.FlightId == flt).OrderByDescending(q => q.Delay).ThenBy(q => q.Code).ToList();
            return Ok(result);
        }
        [Route("api/flight/delayed")]

        // [Authorize]
        public IHttpActionResult GetDelayedFlight(DateTime df, DateTime dt, string route = "", string regs = "", string types = "", string flts = "", string cats = "", int range = 1)
        {
            try
            {


                var context = new ppa_Entities();
                var _df = df.Date;
                var _dt = dt.Date;//.AddHours(24);
                var query = from x in context.ViewDelayedFlights
                            where x.STDDayLocal >= _df && x.STDDayLocal <= _dt
                            select x;
                ////if (!string.IsNullOrEmpty(cats))
                ////{
                ////    var cts = cats.Split('_').ToList();
                ////    query = query.Where(q => cts.Contains(q.MapTitle2));
                ////}
                if (!string.IsNullOrEmpty(route))
                {
                    var rids = route.Split('_').ToList();
                    query = query.Where(q => rids.Contains(q.Route));
                }



                if (!string.IsNullOrEmpty(regs))
                {
                    var regids = regs.Split('_').Select(q => (Nullable<int>)Convert.ToInt32(q)).ToList();
                    query = query.Where(q => regids.Contains(q.RegisterID));
                }

                if (!string.IsNullOrEmpty(types))
                {
                    var typeids = types.Split('_').Select(q => (Nullable<int>)Convert.ToInt32(q)).ToList();
                    query = query.Where(q => typeids.Contains(q.TypeId));
                }
                //malakh
                if (!string.IsNullOrEmpty(flts))
                {
                    var fltids = flts.Split(',').Select(q => q.Trim().Replace(" ", "")).ToList();
                    query = query.Where(q => fltids.Contains(q.FlightNumber));
                }

                switch (range)
                {
                    case 1:

                        break;
                    case 2:
                        query = query.Where(q => q.Delay <= 30);
                        break;
                    case 3:
                        query = query.Where(q => q.Delay > 30);
                        break;
                    case 4:
                        query = query.Where(q => q.Delay >= 31 && q.Delay <= 60);
                        break;
                    case 5:
                        query = query.Where(q => q.Delay >= 61 && q.Delay <= 120);
                        break;
                    case 6:
                        query = query.Where(q => q.Delay >= 121 && q.Delay <= 180);
                        break;
                    case 7:
                        query = query.Where(q => q.Delay >= 181);
                        break;
                    case 8:
                        // query = query.Where(q => q.Delay <= 15);
                        query = query.Where(q => q.Delay <= 0);
                        break;
                    case 9:
                        query = query.Where(q => q.Delay <= 15);
                        break;
                    case 10:
                        query = query.Where(q => q.Delay > 15);
                        break;
                    case 11:
                        // query = query.Where(q => q.Delay <= 15);
                        query = query.Where(q => q.Delay > 0);
                        break;
                    default: break;
                }





                var result = query.OrderBy(q => q.STDDay).ThenBy(q => q.AircraftType).ThenBy(q => q.Register).ThenBy(q => q.STD).ToList();

                return Ok(result);


            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "    " + ex.InnerException.Message;
                return Ok(msg);
            }



        }




        [Route("api/flight/daily/twoway")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetFlightsDailyTwoWay(DateTime df, DateTime dt, string regs, string routes, string from, string to, string no
           , string status
           , string type2
           , string idx
           , string chr
            , int cnl
           )
        {
            var cmd = "select * from viewflightdaily ";
            try
            {
                var context = new ppa_Entities();


                // var cmd = "select * from viewflightdaily ";
                string whr = "  (STDDayLocal>='" + df.ToString("yyyy-MM-dd") + "' and STDDayLocal<='" + dt.ToString("yyyy-MM-dd") + "')";

                if (!string.IsNullOrEmpty(status) && status != "-1")
                {
                    var _regs = status.Split('_').ToList();
                    var col = _regs.Select(q => "status=" + q).ToList();
                    var _whr = "(" + string.Join(" OR ", col) + ")";
                    whr += " AND " + _whr;
                }
                if (!string.IsNullOrEmpty(type2) && type2 != "-1")
                {
                    var _regs = type2.Split('_').ToList();
                    var col = _regs.Select(q => "FlightType2=N'" + q + "'").ToList();
                    var _whr = "(" + string.Join(" OR ", col) + ")";
                    whr += " AND " + _whr;
                }

                if (!string.IsNullOrEmpty(idx) && idx != "-1")
                {
                    var _regs = idx.Split('_').ToList();
                    var col = _regs.Select(q => "FlightIndex=N'" + q + "'").ToList();
                    var _whr = "(" + string.Join(" OR ", col) + ")";
                    whr += " AND " + _whr;
                }

                if (!string.IsNullOrEmpty(chr) && chr != "-1")
                {
                    var _regs = chr.Split('_').ToList();
                    var col = _regs.Select(q => "ChrTitle=N'" + q + "'").ToList();
                    var _whr = "(" + string.Join(" OR ", col) + ")";
                    whr += " AND " + _whr;
                }

                if (!string.IsNullOrEmpty(regs) && regs != "-1")
                {
                    var _regs = regs.Split('_').ToList();
                    var col = _regs.Select(q => "Register='" + q + "'").ToList();
                    var _whr = "(" + string.Join(" OR ", col) + ")";
                    whr += " AND " + _whr;
                }

                if (!string.IsNullOrEmpty(from) && from != "-1")
                {
                    var _regs = from.Split('_').ToList();
                    var _whr = "(" + string.Join(" OR ", _regs.Select(q => "FromAirportIATA='" + q + "'").ToList()) + ")";
                    whr += " AND " + _whr;
                }

                if (!string.IsNullOrEmpty(to) && to != "-1")
                {
                    var _regs = to.Split('_').ToList();
                    var _whr = "(" + string.Join(" OR ", _regs.Select(q => "ToAirportIATA='" + q + "'").ToList()) + ")";
                    whr += " AND " + _whr;
                }

                if (!string.IsNullOrEmpty(no) && no != "-1")
                {
                    var _regs = no.Split('_').ToList();
                    var _whr = "(" + string.Join(" OR ", _regs.Select(q => "FlightNumber='" + q + "'").ToList()) + ")";
                    whr += " AND " + _whr;
                }

                if (!string.IsNullOrEmpty(routes) && routes != "-1")
                {
                    var _regs = routes.Split('_').ToList();
                    var _whr = "(" + string.Join(" OR ", _regs.Select(q => "Route like '%" + q + "%'").ToList()) + ")";
                    whr += " AND " + _whr;
                }

                if (cnl == 0)
                    whr += " AND status<>4";

                cmd = cmd + " WHERE " + whr + " ORDER BY STD,Register";

                var flts = context.ViewFlightDailies
                            .SqlQuery(cmd)
                            .ToList<ViewFlightDaily>();

                var tempflts = flts.ToList();

                var grps = (from x in flts
                            group x by new { x.Register, x.RegisterID, x.STDDayLocal } into grp
                            select new
                            {
                                grp.Key.Register,
                                grp.Key.RegisterID,
                                grp.Key.STDDayLocal,
                                Items = grp.OrderBy(q => q.STD).ToList()



                            }).ToList();
                var output = new List<TwoWayResult>();

                foreach (var g in grps)
                {
                    var rowflts = g.Items.OrderBy(q => q.STD).ToList();
                    while (rowflts.Count > 0)
                    {
                        var flt = rowflts.First();
                        var rec = new TwoWayResult()
                        {
                            Register = g.Register,
                            RegisterID = g.RegisterID,
                            STDDayLocal = g.STDDayLocal,
                            FlightNumber = flt.FlightNumber,
                            STD = flt.STD,
                            STDLocal = flt.STD
                        };
                        output.Add(rec);

                        var xflt = rowflts.Where(q => q.FlightId != flt.ID && reverseRoute(q.Route) == flt.Route).FirstOrDefault();
                        if (xflt != null)
                        {
                            //var recx = new TwoWayResult()
                            //{
                            //    Register = g.Register,
                            //    RegisterID = g.RegisterID,
                            //    STDDayLocal = g.STDDayLocal,
                            //    FlightNumber = xflt.FlightNumber,
                            //};
                            //output.Add(recx);
                            rec.FlightNumber2 = xflt.FlightNumber;
                            rowflts.Remove(xflt);
                        }
                        rowflts.Remove(flt);

                    }
                }

                //var grouped = (from x in flts
                //              group x by new { x.Register, x.RegisterID, x.STDDayLocal, x.PDate, x.PMonth, x.PDayName, x.FlightType2, x.XRoute } into grp
                //              select new TwoWayResult()
                //              {
                //                  Register= grp.Key.Register,
                //                  RegisterID=grp.Key.RegisterID,
                //                  STDDayLocal=grp.Key.STDDayLocal,
                //                  PDate=grp.Key.PDate,
                //                  PMonth=grp.Key.PMonth,
                //                  PDayName=grp.Key.PDayName,
                //                  FlightType2=grp.Key.FlightType2,
                //                  XRoute=grp.Key.XRoute,
                //                  Items=grp.OrderBy(q=>q.STD).ToList()



                //              }).ToList();
                //foreach(var grp in grouped)
                //{
                //    if (grp.Items.Count > 1)
                //    {
                //        grp.Route = grp.XRoute + "-" + grp.XRoute.Split('-')[0];
                //    }
                //    grp.STD = grp.Items.First().STD;
                //    grp.STA = grp.Items.Last().STA;
                //    grp.BlockOff = grp.Items.First().BlockOff;
                //    grp.BlockOn = grp.Items.Last().BlockOn;
                //    grp.TakeOff = grp.Items.First().TakeOff;
                //    grp.Landing = grp.Items.Last().Landing;

                //    grp.STDLocal = grp.Items.First().STDLocal;
                //    grp.STALocal = grp.Items.Last().STALocal;
                //    grp.BlockOffLocal = grp.Items.First().BlockOffLocal;
                //    grp.BlockOnLocal = grp.Items.Last().BlockOnLocal;
                //    grp.TakeOffLocal = grp.Items.First().TakeOffLocal;
                //    grp.LandingLocal = grp.Items.Last().LandingLocal;

                //}

                var result = output.Select(q => new
                {
                    //legs=q.Items.Count(),
                    q.Register,
                    q.RegisterID,
                    q.STDDayLocal,
                    q.FlightNumber,
                    q.FlightNumber2,
                    //q.PDate,
                    //q.PMonth,
                    //q.PDayName,
                    //q.FlightType2,
                    //q.Route,
                    //q.XRoute,
                    q.STD,
                    //q.STA,
                    //q.BlockOff,
                    //q.BlockOn,
                    //q.TakeOff,
                    //q.Landing,
                    //q.STDLocal,
                    //q.STALocal,
                    //q.BlockOffLocal,
                    //q.BlockOnLocal,
                    //q.TakeOffLocal,
                    //q.LandingLocal,
                }).OrderBy(q => q.STDDayLocal).ThenBy(q => q.Register).ThenBy(q => q.STD).ToList();

                // var oneway = result.Where(q => q.legs == 1).ToList();
                //var twoway = result.Where(q => q.legs == 2).ToList();



                //var result = await courseService.GetEmployeeCertificates(id);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(cmd);
            }

        }


        [Route("api/flight/daily/station")]
        [AcceptVerbs("GET")]
        //public IHttpActionResult GetFlightsDailyTwoWay(DateTime df, DateTime dt, string regs, string routes, string from, string to, string no
        //  , string status
        //  , string type2
        //  , string idx
        //  , string chr
        //   , int cnl
        //  )
        public IHttpActionResult GetFlightsDailyStation(DateTime df, DateTime dt
            , string regs
              // , string routes
              , string from
            //, string to, string no
            , string status
            , string type2
             //, string idx
             //, string chr
             , int cnl
          )
        {
            var cmd = "select * from viewflightdaily ";
            try
            {
                var context = new ppa_Entities();


                // var cmd = "select * from viewflightdaily ";
                string whr = "  (STDDayLocal>='" + df.ToString("yyyy-MM-dd") + "' and STDDayLocal<='" + dt.ToString("yyyy-MM-dd") + "')";

                //if (!string.IsNullOrEmpty(status) && status != "-1")
                //{
                //    var _regs = status.Split('_').ToList();
                //    var col = _regs.Select(q => "status=" + q).ToList();
                //    var _whr = "(" + string.Join(" OR ", col) + ")";
                //    whr += " AND " + _whr;
                //}
                //if (!string.IsNullOrEmpty(type2) && type2 != "-1")
                //{
                //    var _regs = type2.Split('_').ToList();
                //    var col = _regs.Select(q => "FlightType2=N'" + q + "'").ToList();
                //    var _whr = "(" + string.Join(" OR ", col) + ")";
                //    whr += " AND " + _whr;
                //}

                //if (!string.IsNullOrEmpty(idx) && idx != "-1")
                //{
                //    var _regs = idx.Split('_').ToList();
                //    var col = _regs.Select(q => "FlightIndex=N'" + q + "'").ToList();
                //    var _whr = "(" + string.Join(" OR ", col) + ")";
                //    whr += " AND " + _whr;
                //}

                //if (!string.IsNullOrEmpty(chr) && chr != "-1")
                //{
                //    var _regs = chr.Split('_').ToList();
                //    var col = _regs.Select(q => "ChrTitle=N'" + q + "'").ToList();
                //    var _whr = "(" + string.Join(" OR ", col) + ")";
                //    whr += " AND " + _whr;
                //}

                if (!string.IsNullOrEmpty(regs) && regs != "-1")
                {
                    var _regs = regs.Split('_').ToList();
                    var col = _regs.Select(q => "Register='" + q + "'").ToList();
                    var _whr = "(" + string.Join(" OR ", col) + ")";
                    whr += " AND " + _whr;
                }

                //if (!string.IsNullOrEmpty(from) && from != "-1")
                //{
                //    var _regs = from.Split('_').ToList();
                //    var _whr = "(" + string.Join(" OR ", _regs.Select(q => "FromAirportIATA='" + q + "'").ToList()) + ")";
                //    whr += " AND " + _whr;
                //}

                //if (!string.IsNullOrEmpty(to) && to != "-1")
                //{
                //    var _regs = to.Split('_').ToList();
                //    var _whr = "(" + string.Join(" OR ", _regs.Select(q => "ToAirportIATA='" + q + "'").ToList()) + ")";
                //    whr += " AND " + _whr;
                //}

                //if (!string.IsNullOrEmpty(no) && no != "-1")
                //{
                //    var _regs = no.Split('_').ToList();
                //    var _whr = "(" + string.Join(" OR ", _regs.Select(q => "FlightNumber='" + q + "'").ToList()) + ")";
                //    whr += " AND " + _whr;
                //}

                //if (!string.IsNullOrEmpty(routes) && routes != "-1")
                //{
                //    var _regs = routes.Split('_').ToList();
                //    var _whr = "(" + string.Join(" OR ", _regs.Select(q => "Route like '%" + q + "%'").ToList()) + ")";
                //    whr += " AND " + _whr;
                //}

                if (cnl == 0)
                    whr += " AND status<>4";

                cmd = cmd + " WHERE " + whr + " ORDER BY STD,Register";

                var flts = context.ViewFlightDailies
                            .SqlQuery(cmd)
                            .ToList<ViewFlightDaily>();

                var stations = flts.Select(q => q.FromAirportIATA).ToList().Concat(flts.Select(q => q.ToAirportIATA).ToList()).Distinct().ToList();
                var output2 = new List<TwoWayResult>();
                foreach (var stn in stations)
                {
                    var stnflights = flts.Where(q => q.FromAirportIATA == stn || q.ToAirportIATA == stn).ToList();
                    var regGroups = (from x in stnflights
                                     group x by new { x.Register, x.RegisterID, x.STDDayLocal } into grp
                                     select new
                                     {
                                         Station = stn,
                                         grp.Key.Register,
                                         grp.Key.RegisterID,
                                         grp.Key.STDDayLocal,
                                         Items = grp.OrderBy(q => q.STD).ToList()



                                     }).ToList();
                    foreach (var grp in regGroups)
                    {
                        var _flts = grp.Items.ToList();
                        while (_flts.Count() > 0)
                        {
                            var _flt = _flts.First();

                            if (_flt.FromAirportIATA == grp.Station)
                            {
                                //out
                                var rec = new TwoWayResult()
                                {
                                    Station = grp.Station,
                                    FlightDate = _flt.FlightDate,
                                    Date = _flt.Date,
                                    DateLocal = _flt.DateLocal,
                                    PDate = _flt.PDate,
                                    PDayName = _flt.PDayName,
                                    PMonth = _flt.PMonth,
                                    PMonthName = _flt.PMonthName,
                                    STDDay = _flt.STDDay,
                                    STDDayLocal = _flt.STDDayLocal,
                                    Register = grp.Register,
                                    RegisterID = grp.RegisterID,
                                    FromAirportIATA2 = _flt.FromAirportIATA,
                                    FromAirportICAO2 = _flt.FromAirportICAO,
                                    ToAirportIATA2 = _flt.ToAirportIATA,
                                    ToAirportICAO2 = _flt.ToAirportICAO,
                                    FlightNumber2 = _flt.FlightNumber,
                                    STD2 = _flt.STD,
                                    STA2 = _flt.STA,
                                    BlockOff2 = _flt.BlockOff,
                                    BlockOn2 = _flt.BlockOn,
                                    TakeOff2 = _flt.TakeOff,
                                    Landing2 = _flt.Landing,
                                    STDLocal2 = _flt.STDLocal,
                                    STALocal2 = _flt.STALocal,
                                    TakeOffLocal2 = _flt.TakeOffLocal,
                                    LandingLocal2 = _flt.LandingLocal,
                                    BlockOffLocal2 = _flt.BlockOffLocal,
                                    BlockOnLocal2 = _flt.BlockOnLocal,
                                    PaxAdult2 = _flt.PaxAdult,
                                    PaxChild2 = _flt.PaxChild,
                                    PaxInfant2 = _flt.PaxInfant,
                                    RevPax2 = _flt.RevPax,
                                    TotalPax2 = _flt.TotalPax,
                                    FlightStatus2 = _flt.FlightStatus,
                                    IsArrInt2 = _flt.IsArrInt,
                                    IsDepInt2 = _flt.IsDepInt,
                                    STDX = _flt.STD,
                                    RegFlightCount = grp.Items.Count(),
                                };

                                output2.Add(rec);
                                _flts.Remove(_flt);
                            }
                            else
                            {
                                //in
                                var rec = new TwoWayResult()
                                {
                                    Station = grp.Station,
                                    Register = grp.Register,
                                    RegisterID = grp.RegisterID,
                                    FromAirportIATA = _flt.FromAirportIATA,
                                    ToAirportIATA = _flt.ToAirportIATA,
                                    FromAirportICAO = _flt.FromAirportICAO,
                                    ToAirportICAO = _flt.ToAirportICAO,
                                    FlightNumber = _flt.FlightNumber,
                                    FlightDate = _flt.FlightDate,
                                    Date = _flt.Date,
                                    DateLocal = _flt.DateLocal,
                                    PDate = _flt.PDate,
                                    PDayName = _flt.PDayName,
                                    PMonth = _flt.PMonth,
                                    PMonthName = _flt.PMonthName,
                                    STDDay = _flt.STDDay,
                                    STDDayLocal = _flt.STDDayLocal,
                                    STD = _flt.STD,
                                    STA = _flt.STA,
                                    BlockOff = _flt.BlockOff,
                                    BlockOn = _flt.BlockOn,
                                    TakeOff = _flt.TakeOff,
                                    Landing = _flt.Landing,
                                    STDLocal = _flt.STDLocal,
                                    STALocal = _flt.STALocal,
                                    TakeOffLocal = _flt.TakeOffLocal,
                                    LandingLocal = _flt.LandingLocal,
                                    BlockOffLocal = _flt.BlockOffLocal,
                                    BlockOnLocal = _flt.BlockOnLocal,
                                    PaxAdult = _flt.PaxAdult,
                                    PaxChild = _flt.PaxChild,
                                    PaxInfant = _flt.PaxInfant,
                                    RevPax = _flt.RevPax,
                                    TotalPax = _flt.TotalPax,
                                    FlightStatus = _flt.FlightStatus,
                                    IsArrInt = _flt.IsArrInt,
                                    IsDepInt = _flt.IsDepInt,
                                    STDX = _flt.STD,
                                    RegFlightCount = grp.Items.Count(),

                                };
                                _flts.Remove(_flt);
                                if (_flts.Count() > 0)
                                {
                                    _flt = _flts.First();
                                    rec.FromAirportIATA2 = _flt.FromAirportIATA;
                                    rec.ToAirportIATA2 = _flt.ToAirportIATA;
                                    rec.ToAirportIATA2 = _flt.ToAirportIATA;
                                    rec.ToAirportICAO2 = _flt.ToAirportICAO;
                                    rec.FlightNumber2 = _flt.FlightNumber;
                                    rec.FromAirportICAO2 = _flt.FromAirportICAO;
                                    rec.STD2 = _flt.STD;
                                    rec.STA2 = _flt.STA;
                                    rec.BlockOff2 = _flt.BlockOff;
                                    rec.BlockOn2 = _flt.BlockOn;
                                    rec.TakeOff2 = _flt.TakeOff;
                                    rec.Landing2 = _flt.Landing;
                                    rec.STDLocal2 = _flt.STDLocal;
                                    rec.STALocal2 = _flt.STALocal;
                                    rec.TakeOffLocal2 = _flt.TakeOffLocal;
                                    rec.LandingLocal2 = _flt.LandingLocal;
                                    rec.BlockOffLocal2 = _flt.BlockOffLocal;
                                    rec.BlockOnLocal2 = _flt.BlockOnLocal;
                                    rec.PaxAdult2 = _flt.PaxAdult;
                                    rec.PaxChild2 = _flt.PaxChild;
                                    rec.PaxInfant2 = _flt.PaxInfant;
                                    rec.RevPax2 = _flt.RevPax;
                                    rec.TotalPax2 = _flt.TotalPax;
                                    rec.FlightStatus2 = _flt.FlightStatus;
                                    rec.IsArrInt2 = _flt.IsArrInt;
                                    rec.IsDepInt2 = _flt.IsDepInt;
                                    _flts.Remove(_flt);
                                }

                                output2.Add(rec);

                            }
                        }
                    }


                }

                foreach (var x in output2)
                {
                    x.FlightType2 = (x.IsDepInt == 1 || x.IsDepInt2 == 1 || x.IsArrInt == 1 || x.IsArrInt2 == 1) ? "INT" : "DOM";
                }

                if (from != "-1")
                {
                    var _regs = from.Split('_').ToList();
                    output2 = output2.Where(q => _regs.Contains(q.Station)).ToList();
                }
                if (!string.IsNullOrEmpty(type2) && type2 != "-1")
                {
                    var _regs = type2.Split('_').ToList();
                    output2 = output2.Where(q => _regs.Contains(q.FlightType2)).ToList();
                }
                //var grps = (from x in flts
                //            group x by new { x.Register, x.RegisterID, x.STDDayLocal } into grp
                //            select new
                //            {
                //                grp.Key.Register,
                //                grp.Key.RegisterID,
                //                grp.Key.STDDayLocal,
                //                Items = grp.OrderBy(q => q.STD).ToList()



                //            }).ToList();
                //var output = new List<TwoWayResult>();

                //foreach (var g in grps)
                //{
                //    var rowflts = g.Items.OrderBy(q => q.STD).ToList();
                //    while (rowflts.Count > 0)
                //    {
                //        var flt = rowflts.First();
                //        var rec = new TwoWayResult()
                //        {
                //            Register = g.Register,
                //            RegisterID = g.RegisterID,
                //            STDDayLocal = g.STDDayLocal,
                //            FlightNumber = flt.FlightNumber,
                //            STD = flt.STD,
                //            STDLocal = flt.STD
                //        };
                //        output.Add(rec);

                //        var xflt = rowflts.Where(q => q.FlightId != flt.ID && reverseRoute(q.Route) == flt.Route).FirstOrDefault();
                //        if (xflt != null)
                //        {
                //            //var recx = new TwoWayResult()
                //            //{
                //            //    Register = g.Register,
                //            //    RegisterID = g.RegisterID,
                //            //    STDDayLocal = g.STDDayLocal,
                //            //    FlightNumber = xflt.FlightNumber,
                //            //};
                //            //output.Add(recx);
                //            rec.FlightNumber2 = xflt.FlightNumber;
                //            rowflts.Remove(xflt);
                //        }
                //        rowflts.Remove(flt);

                //    }
                //}



                /////////////////////////
                /////////////////////////

                //var grouped = (from x in flts
                //              group x by new { x.Register, x.RegisterID, x.STDDayLocal, x.PDate, x.PMonth, x.PDayName, x.FlightType2, x.XRoute } into grp
                //              select new TwoWayResult()
                //              {
                //                  Register= grp.Key.Register,
                //                  RegisterID=grp.Key.RegisterID,
                //                  STDDayLocal=grp.Key.STDDayLocal,
                //                  PDate=grp.Key.PDate,
                //                  PMonth=grp.Key.PMonth,
                //                  PDayName=grp.Key.PDayName,
                //                  FlightType2=grp.Key.FlightType2,
                //                  XRoute=grp.Key.XRoute,
                //                  Items=grp.OrderBy(q=>q.STD).ToList()



                //              }).ToList();
                //foreach(var grp in grouped)
                //{
                //    if (grp.Items.Count > 1)
                //    {
                //        grp.Route = grp.XRoute + "-" + grp.XRoute.Split('-')[0];
                //    }
                //    grp.STD = grp.Items.First().STD;
                //    grp.STA = grp.Items.Last().STA;
                //    grp.BlockOff = grp.Items.First().BlockOff;
                //    grp.BlockOn = grp.Items.Last().BlockOn;
                //    grp.TakeOff = grp.Items.First().TakeOff;
                //    grp.Landing = grp.Items.Last().Landing;

                //    grp.STDLocal = grp.Items.First().STDLocal;
                //    grp.STALocal = grp.Items.Last().STALocal;
                //    grp.BlockOffLocal = grp.Items.First().BlockOffLocal;
                //    grp.BlockOnLocal = grp.Items.Last().BlockOnLocal;
                //    grp.TakeOffLocal = grp.Items.First().TakeOffLocal;
                //    grp.LandingLocal = grp.Items.Last().LandingLocal;

                //}

                var fc = (from x in output2 group x by new { x.Station } into grp select new { grp.Key.Station, cnt = grp.Count() }).ToList();
                foreach (var x in output2)
                {
                    var _fc = fc.FirstOrDefault(q => q.Station == x.Station);
                    x.FlightCount = _fc == null ? 0 : _fc.cnt;
                }


                var result = new
                {
                    flts = output2.OrderBy(q => q.STDDayLocal).ThenByDescending(q => q.FlightCount).ThenBy(q => q.Station).ThenBy(q => q.Register).ThenBy(q => q.STDX).ToList(),
                    stations = fc
                };
                /*.Select(q => new
            {
                //legs=q.Items.Count(),
                q.Register,
                q.RegisterID,
                q.STDDayLocal,
                q.FlightNumber,
                q.FromAirportIATA,
                q.ToAirportIATA,
                q.FlightNumber2,
                q.FromAirportIATA2,
                q.ToAirportIATA2,
                q.Station,
                //q.PDate,
                //q.PMonth,
                //q.PDayName,
                //q.FlightType2,
                //q.Route,
                //q.XRoute,
                 q.STDX,
                //q.STA,
                //q.BlockOff,
                //q.BlockOn,
                //q.TakeOff,
                //q.Landing,
                //q.STDLocal,
                //q.STALocal,
                //q.BlockOffLocal,
                //q.BlockOnLocal,
                //q.TakeOffLocal,
                //q.LandingLocal,
            })*/

                //.OrderBy(q => q.STDDayLocal).ThenBy(q => q.Register).ThenBy(q => q.STD).ToList();

                // var oneway = result.Where(q => q.legs == 1).ToList();
                //var twoway = result.Where(q => q.legs == 2).ToList();



                //var result = await courseService.GetEmployeeCertificates(id);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(cmd);
            }

        }

        public class TwoWayResult
        {
            public int ID { get; set; }
            public int FlightId { get; set; }
            public int FlightId2 { get; set; }
            public string Station { get; set; }
            public int FlightCount { get; set; }
            public int RegFlightCount { get; set; }
            public Nullable<int> FlightPlanId { get; set; }
            public Nullable<System.DateTime> STD { get; set; }
            public Nullable<System.DateTime> STD2 { get; set; }
            public Nullable<System.DateTime> STDX { get; set; }
            public Nullable<System.DateTime> STA { get; set; }
            public Nullable<System.DateTime> STA2 { get; set; }
            public Nullable<System.DateTime> STDLocal { get; set; }
            public Nullable<System.DateTime> STALocal { get; set; }
            public Nullable<System.DateTime> STDLocal2 { get; set; }
            public Nullable<System.DateTime> STALocal2 { get; set; }
            public Nullable<System.DateTime> Date { get; set; }
            public Nullable<System.DateTime> DateLocal { get; set; }
            public Nullable<int> FlightStatusID { get; set; }
            public Nullable<int> FlightStatusID2 { get; set; }
            public Nullable<int> RegisterID { get; set; }
            public Nullable<int> FlightTypeID { get; set; }
            public string AircraftType { get; set; }
            public Nullable<int> TypeId { get; set; }
            public string FlightNumber { get; set; }
            public string FlightNumber2 { get; set; }
            public Nullable<int> FromAirport { get; set; }
            public string FromAirportICAO { get; set; }
            public Nullable<int> ToAirport { get; set; }
            public string ToAirportICAO { get; set; }
            public Nullable<int> FromAirport2 { get; set; }
            public string FromAirportICAO2 { get; set; }
            public Nullable<int> ToAirport2 { get; set; }
            public string ToAirportICAO2 { get; set; }
            public Nullable<int> CustomerId { get; set; }
            public string FromAirportIATA { get; set; }
            public string FromAirportIATA2 { get; set; }
            public string ToAirportIATA { get; set; }
            public string ToAirportIATA2 { get; set; }
            public string Register { get; set; }
            public string FlightStatus { get; set; }
            public string FlightStatus2 { get; set; }
            public string ArrivalRemark { get; set; }
            public string DepartureRemark { get; set; }
            public Nullable<System.DateTime> STDDay { get; set; }
            public Nullable<System.DateTime> STDDayLocal { get; set; }
            public Nullable<System.DateTime> STADay { get; set; }
            public Nullable<int> DelayOffBlock { get; set; }
            public Nullable<int> DelayTakeoff { get; set; }
            public Nullable<System.DateTime> OSTA { get; set; }
            public Nullable<int> OToAirportId { get; set; }
            public string OToAirportIATA { get; set; }
            public Nullable<int> FPFlightHH { get; set; }
            public Nullable<int> FPFlightMM { get; set; }
            public Nullable<System.DateTime> Departure { get; set; }
            public Nullable<System.DateTime> Arrival { get; set; }
            public Nullable<System.DateTime> Departure2 { get; set; }
            public Nullable<System.DateTime> Arrival2 { get; set; }
            public Nullable<System.DateTime> BlockOff { get; set; }
            public Nullable<System.DateTime> BlockOn { get; set; }
            public Nullable<System.DateTime> TakeOff { get; set; }
            public Nullable<System.DateTime> Landing { get; set; }
            public Nullable<System.DateTime> BlockOff2 { get; set; }
            public Nullable<System.DateTime> BlockOn2 { get; set; }
            public Nullable<System.DateTime> TakeOff2 { get; set; }
            public Nullable<System.DateTime> Landing2 { get; set; }
            public Nullable<System.DateTime> BlockOffLocal { get; set; }
            public Nullable<System.DateTime> BlockOnLocal { get; set; }
            public Nullable<System.DateTime> TakeOffLocal { get; set; }
            public Nullable<System.DateTime> LandingLocal { get; set; }
            public Nullable<System.DateTime> DepartureLocal { get; set; }
            public Nullable<System.DateTime> ArrivalLocal { get; set; }
            public Nullable<System.DateTime> BlockOffLocal2 { get; set; }
            public Nullable<System.DateTime> BlockOnLocal2 { get; set; }
            public Nullable<System.DateTime> TakeOffLocal2 { get; set; }
            public Nullable<System.DateTime> LandingLocal2 { get; set; }
            public Nullable<System.DateTime> DepartureLocal2 { get; set; }
            public Nullable<System.DateTime> ArrivalLocal2 { get; set; }
            public Nullable<int> BlockTime { get; set; }
            public Nullable<int> ScheduledTime { get; set; }
            public Nullable<int> FlightTime { get; set; }
            public Nullable<int> status { get; set; }
            public Nullable<System.DateTime> JLOffBlock { get; set; }
            public Nullable<System.DateTime> JLOnBlock { get; set; }
            public Nullable<System.DateTime> JLTakeOff { get; set; }
            public Nullable<System.DateTime> JLLanding { get; set; }
            public Nullable<int> PFLR { get; set; }
            public int PaxChild { get; set; }
            public int PaxInfant { get; set; }
            public int PaxAdult { get; set; }
            public Nullable<int> RevPax { get; set; }
            public Nullable<int> TotalPax { get; set; }
            public int PaxChild2 { get; set; }
            public int PaxInfant2 { get; set; }
            public int PaxAdult2 { get; set; }
            public Nullable<int> RevPax2 { get; set; }
            public Nullable<int> TotalPax2 { get; set; }
            public Nullable<int> FuelUnitID { get; set; }
            public Nullable<decimal> FuelArrival { get; set; }
            public Nullable<decimal> FuelDeparture { get; set; }
            public Nullable<double> UpliftLtr { get; set; }
            public Nullable<double> UpliftLbs { get; set; }
            public Nullable<double> UpliftKg { get; set; }
            public Nullable<decimal> UsedFuel { get; set; }
            public Nullable<int> TotalSeat { get; set; }
            public int BaggageWeight { get; set; }
            public int CargoWeight { get; set; }
            public int BaggageWeight2 { get; set; }
            public int CargoWeight2 { get; set; }
            public Nullable<int> Freight { get; set; }
            public Nullable<double> BaggageWeightLbs { get; set; }
            public Nullable<double> BaggageWeightKg { get; set; }
            public Nullable<double> CargoWeightLbs { get; set; }
            public Nullable<double> CargoWeightKg { get; set; }
            public Nullable<double> FreightLbs { get; set; }
            public Nullable<double> FreightKg { get; set; }
            public Nullable<System.DateTime> FlightDate { get; set; }
            public Nullable<int> CargoCount { get; set; }
            public Nullable<int> BaggageCount { get; set; }
            public Nullable<int> CargoCount2 { get; set; }
            public Nullable<int> BaggageCount2 { get; set; }
            public Nullable<int> JLBlockTime { get; set; }
            public Nullable<int> JLFlightTime { get; set; }
            public Nullable<decimal> FPFuel { get; set; }
            public Nullable<decimal> FPTripFuel { get; set; }
            public Nullable<int> MaxWeightTO { get; set; }
            public Nullable<int> MaxWeightLND { get; set; }
            public string MaxWeighUnit { get; set; }
            public string ChrCode { get; set; }
            public string ChrTitle { get; set; }
            public Nullable<int> ChrCapacity { get; set; }
            public Nullable<int> ChrAdult { get; set; }
            public Nullable<int> ChrChild { get; set; }
            public Nullable<int> ChrInfant { get; set; }
            public Nullable<int> PMonth { get; set; }
            public string PMonthName { get; set; }
            public string PDayName { get; set; }
            public string FlightType2 { get; set; }
            public string FlightIndex { get; set; }
            public Nullable<int> AirlineSold { get; set; }
            public Nullable<int> CherterSold { get; set; }
            public Nullable<int> OverSeat { get; set; }
            public Nullable<int> EmptySeat { get; set; }
            public string DelayReason { get; set; }
            public Nullable<double> Distance { get; set; }
            public Nullable<double> StationIncome { get; set; }
            public string TotalRemark { get; set; }
            public string Route { get; set; }
            public string PDate { get; set; }
            public int IsDepInt { get; set; }
            public int IsArrInt { get; set; }
            public int IsDepInt2 { get; set; }
            public int IsArrInt2 { get; set; }
            public string XRoute { get; set; }
            public List<ViewFlightDaily> Items { get; set; }
        }

        public class _filter
        {
            public string FromAirport { get; set; }
            public string ToAirport { get; set; }
            public string Register { get; set; }
        }

        [Route("api/flight/daily/filters")]
        public IHttpActionResult GetFlightsDailyFilters(DateTime df, DateTime dt)
        {
            var context = new ppa_Entities();
            df = df.Date;
            dt = dt.Date;
            var qry = from x in context.ViewFlightDailies

                      where x.STDDay >= df && x.STDDay <= dt
                      select new _filter() { FromAirport = x.FromAirportIATA, ToAirport = x.ToAirportIATA, Register = x.Register };
            var ds = qry.ToList();
            var origins = qry.Select(q => q.FromAirport).Distinct().ToList();
            var destinations = qry.Select(q => q.ToAirport).Distinct().ToList();
            var registers = qry.Select(q => q.Register).Distinct().ToList();
            return Ok(new { origins, destinations, registers });
        }

        [Route("api/fuel")]
        public IHttpActionResult GetFuelOFP(DateTime df, DateTime dt)
        {
            var context = new ppa_Entities();
            df = df.Date;
            dt = dt.Date;
            var qry = from x in context.RptFuelOFPs

                      where x.STDDay >= df && x.STDDay <= dt
                      select x;
            var ds = qry.OrderBy(q => q.STDDay).ThenBy(q => q.Register).ThenBy(q => q.STD).ToList();

            return Ok(ds);
        }

        [Route("api/rvsm")]
        public IHttpActionResult GetFlightRvsm(DateTime df, DateTime dt)
        {
            var context = new ppa_Entities();
            df = df.Date;
            dt = dt.Date;
            var qry = from x in context.RptFlightRVSMs

                      where x.STDDay >= df && x.STDDay <= dt
                      select x;
            var ds = qry.OrderBy(q => q.STDDay).ThenBy(q => q.Register).ThenBy(q => q.STD).ToList();

            return Ok(ds);
        }

        [Route("api/formb/report")]

        // [Authorize]
        public IHttpActionResult GetFormBReport(int year, int qrt)
        {
            var context = new ppa_Entities();
            var query = (from x in context.ViewFormBs
                         where x.Year == year && x.Quarter == qrt
                         select x).ToList();




            return Ok(query);
        }

        [Route("api/formc/report")]

        // [Authorize]
        public IHttpActionResult GetFormCReport(int year, int month)
        {
            var context = new ppa_Entities();
            var query = (from x in context.ViewFormCs
                         where x.Year == year && x.Month == month
                         select x).ToList();




            return Ok(query);
        }

        string reverseRoute(string rt)
        {
            var prts = rt.Split('-');
            return prts[1] + "-" + prts[0];
        }
        int getRouteOrder(string r)
        {
            if (r.StartsWith("THR"))
                return 1;
            if (r.StartsWith("IKA"))
                return 2;
            if (r.StartsWith("SRY"))
                return 3;
            return 10;

        }
        [Route("api/citypair/report")]

        // [Authorize]
        public IHttpActionResult GetCityPairReport(int year, int month, int? dom = -1)
        {
            var context = new ppa_Entities();


            var query = from x in context.ViewFinMonthlyRoutes
                        where x.Year == year && x.Month == month
                        select x;

            if (dom == 1)
                query = query.Where(q => q.IsDom == true);
            if (dom == 0)
                query = query.Where(q => q.IsDom == false);

            var ds = query.ToList().OrderByDescending(q => q.Legs).ThenBy(q => getRouteOrder(q.Route)).ToList();
            var routes = ds.Select(q => q.Route).OrderBy(q => getRouteOrder(q)).ToList();
            var result = new List<ViewFinMonthlyRoute>();
            while (ds.Count > 0)
            {
                var data = ds.First();
                var rev = reverseRoute(data.Route);
                var data_rev = ds.Where(q => q.Route == rev).FirstOrDefault();
                result.Add(data);
                ds.Remove(data);
                if (data_rev != null && rev != data.Route)
                {
                    result.Add(data_rev);
                    ds.Remove(data_rev);
                }
            }
            /* while (routes.Count > 0)
             { 
                 var rt = routes.First(); 

                 var data = ds.Where(q => q.Route == rt).FirstOrDefault();

                 var rec = new ViewFinMonthlyRoute()
                 {
                     Adult = data.Adult,
                     BaggageWeight = data.BaggageWeight,
                     Route = data.Route,
                     CargoWeight = data.CargoWeight,
                     Child = data.Child,
                     Delay = data.Delay,
                     Freight = data.Freight,
                     FromAirportIATA = data.FromAirportIATA,
                     Infant = data.Infant,
                     IsDom = data.IsDom,
                     Legs = data.Legs,
                     Month = data.Month,
                     MonthName = data.MonthName,
                     ToAirportIATA = data.ToAirportIATA,
                     TotalPax = data.TotalPax,
                     TotalSeat = data.TotalSeat,
                     UpliftFuel = data.UpliftFuel,
                     UsedFuel = data.UsedFuel,
                     Year = data.Year,
                     YearMonth = data.YearMonth,
                     YearName = data.YearName,
                 };
                 var rev = reverseRoute(rt);
                 var data2 = ds.Where(q => q.Route == rev).FirstOrDefault();
                 if (data2 != null)
                 {
                     rec.Route = rec.Route +"-"+ (rec.Route.Split('-')[0]);
                     rec.Adult += data2.Adult;
                     rec.BaggageWeight += data2.BaggageWeight;
                     rec.CargoWeight += data2.CargoWeight;
                     rec.Child += data2.Child;
                     rec.Delay += data2.Delay;
                     rec.Freight += data2.Freight;
                     rec.Infant += data2.Infant;
                     rec.Legs += data2.Legs;
                     rec.TotalPax += data2.TotalPax;
                     rec.TotalSeat += data2.TotalSeat;
                     rec.UsedFuel += data2.UsedFuel;
                     rec.UpliftFuel += data2.UpliftFuel;
                 }

                 routes.Remove(rt);
                 routes.Remove(reverseRoute(rt));

                 result.Add(rec);

             }*/



            //var query = (from x in context.ViewFormCs
            //             where x.Year == year && x.Month == month
            //             select x).ToList();




            //return Ok(query);
            return Ok(result);
        }



        string toHHMM(int? mm)
        {
            if (mm == null || mm <= 0)
                return "00:00";
            // TimeSpan ts = TimeSpan.FromMinutes(Convert.ToDouble(mm));
            // var result = ts.Hours.ToString().PadLeft(2, '0') + ":" + ts.Minutes.ToString().PadLeft(2, '0');
            var hh = mm / 60;
            var min = mm % 60;
            var result = hh.ToString().PadLeft(2, '0') + ":" + min.ToString().PadLeft(2, '0');
            return result;

        }

        public class FlightTimeDto
        {
            public int? CrewId { get; set; }
            public string ScheduleName { get; set; }
            public string JobGroup { get; set; }
            public string JobGropCode { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public string HomeBase { get; set; }
            public int GroupOrder { get; set; }
            public int Legs { get; set; }
            public int DH { get; set; }
            public int? FlightTime { get; set; }
            public int? BlockTime { get; set; }
            public int? JLFlightTime { get; set; }
            public int? JLBlockTime { get; set; }
            public int? FixTime { get; set; }
            public string PID { get; set; }
            public string ValidTypes { get; set; }
            public int? OA { get; set; }
        }

        public class CrewSummaryDto
        {
            public int? CrewId { get; set; }
            public string ScheduleName { get; set; }
            public string JobGroup { get; set; }
            public string JobGropCode { get; set; }
            public string Name { get; set; }
            public int GroupOrder { get; set; }
            public int Legs { get; set; }
            public int DH { get; set; }
            public int? FlightTime { get; set; }
            public int? BlockTime { get; set; }
            public int? JLFlightTime { get; set; }
            public int? JLBlockTime { get; set; }
            public int? FixTime { get; set; }
            public string PID { get; set; }
        }

        int getOrder(string group)
        {
            switch (group)
            {
                case "TRE":
                    return 1;
                case "TRI":
                    return 2;
                case "LTC":
                    return 3;
                case "P1":
                    return 4;
                case "P2":
                    return 5;
                case "ISCCM":
                    return 10;
                case "SCCM":
                    return 11;
                case "CCM":
                    return 12;
                default:
                    return 1000;
            }
        }
        string GETUrl(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                    return reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                WebResponse errorResponse = ex.Response;
                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.GetEncoding("utf-8"));
                    String errorText = reader.ReadToEnd();
                    // log errorText
                }
                throw;
            }
        }

        [Route("api/report/flights")]

        //nookp
        public IHttpActionResult GetFlightCockpit(DateTime? df, DateTime? dt, int? ip, int? cpt, int? status)
        {
            //nooz
            //this.context.Database.CommandTimeout = 160;
            var ctx = new ppa_Entities();
            df = df != null ? ((DateTime)df).Date : DateTime.MinValue.Date;
            dt = dt != null ? ((DateTime)dt).Date : DateTime.MaxValue.Date;
            var query = from x in ctx.ViewFlightCockpits
                            // where x.FlightStatusID != 1 && x.FlightStatusID != 4
                        select x;
            query = query.Where(q => q.STDDayLocal >= df && q.STDDayLocal <= dt);
            if (ip != null)
                query = query.Where(q => q.IPId == ip);
            if (cpt != null)
                query = query.Where(q => q.CaptainId == cpt);
            if (status != null)
            {
                //       { Id: 1, Title: 'Done' },
                //{ Id: 2, Title: 'Scheduled' },
                //{ Id: 3, Title: 'Canceled' },
                //{ Id: 4, Title: 'Starting' },
                // { Id: 5, Title: 'All' },
                List<int> sts = new List<int>();
                switch ((int)status)
                {
                    case 1:
                        sts.Add(15);
                        sts.Add(3);
                        query = query.Where(q => sts.Contains(q.FlightStatusID));
                        break;
                    case 2:
                        sts.Add(1);
                        query = query.Where(q => sts.Contains(q.FlightStatusID));
                        break;
                    case 3:
                        sts.Add(4);
                        query = query.Where(q => sts.Contains(q.FlightStatusID));
                        break;
                    case 4:
                        sts.Add(20);
                        sts.Add(21);
                        sts.Add(22);
                        sts.Add(4);
                        sts.Add(2);
                        sts.Add(23);
                        sts.Add(24);
                        sts.Add(25);
                        query = query.Where(q => sts.Contains(q.FlightStatusID));

                        break;
                    case 5:
                        break;
                    default:
                        break;
                }
            }
            var result = query.OrderBy(q => q.STD).ToList();

            // return result.OrderBy(q => q.STD);
            return Ok(result);

        }





        [Route("api/crew/flights/{grp}/{type}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetCrewFlightTimes(string grp, string type, DateTime df, DateTime dt)
        {
            try
            {
                List<FlightTimeDto> external_list = new List<FlightTimeDto>();
                //try
                //{
                //    var ids = "-4325_-4287_-4289";
                //    var dfStr = df.ToString("yyyy-MM-dd");
                //    var dtStr = dt.ToString("yyyy-MM-dd");
                //    var extStr = GETUrl("https://apireportflight.apvaresh.com/api/crew/flights/ids/" + ids + "?df=" + dfStr + "&dt=" + dtStr);
                //    external_list = JsonConvert.DeserializeObject<List<FlightTimeDto>>(extStr);
                //}
                //catch (Exception ex)
                //{

                //}

                var ctx = new ppa_Entities();
                var _df = df.Date;
                var _dt = dt.Date.AddDays(1);
                var _query_x = from x in ctx.ViewLegCrews
                               where x.STDLocal >= _df && x.STDLocal < _dt && x.FlightStatusID != 4
                               select x;
                if (grp != "ALL")
                {
                    var ds_cockpit = new List<string>() { "TRE", "TRI", "LTC", "P1", "P2" };
                    var ds_ip = new List<string>() { "TRE", "TRI", "LTC" };
                    var ds_cabin = new List<string>() { "ISCCM", "SCCM", "CCM", "CC", "CCE", "CCI" };
                    switch (grp)
                    {
                        case "COCKPIT":
                            _query_x = _query_x.Where(q => ds_cockpit.Contains(q.JobGroup));
                            break;
                        case "IP":
                            _query_x = _query_x.Where(q => ds_ip.Contains(q.JobGroup));
                            break;
                        case "P1":
                            _query_x = _query_x.Where(q => q.JobGroup == "P1");
                            break;
                        case "P2":
                            _query_x = _query_x.Where(q => q.JobGroup == "P2");
                            break;
                        case "ISCCM":
                            _query_x = _query_x.Where(q => q.JobGroup == "ISCCM");
                            break;
                        case "SCCM":
                            _query_x = _query_x.Where(q => q.JobGroup == "SCCM");
                            break;
                        case "CCM":
                            _query_x = _query_x.Where(q => q.JobGroup == "CCM");
                            break;
                        case "CABIN":
                            _query_x = _query_x.Where(q => ds_cabin.Contains(q.JobGroup));
                            break;
                        case "ALL":
                            //_query_x = _query_x.Where(q => ds_cockpit.IndexOf(q.JobGroup) != -1);
                            break;
                        default:
                            break;
                    }
                }
                if (type != "ALL")
                {
                    _query_x = _query_x.Where(q => q.ValidTypes.Contains(type));
                }
                var _query = (
                               //from x in ctx.ViewLegCrews
                               //where x.STDLocal >= df && x.STDLocal < dt && x.FlightStatusID != 4
                               from x in _query_x
                               group x by new { x.CrewId, x.ScheduleName, x.JobGroup, x.JobGroupCode, x.Name, x.PID, x.ValidTypes, x.BaseAirport, x.BaseAirportId, x.FlightPlanId } into _grp
                               select new FlightTimeDto()
                               {
                                   CrewId = _grp.Key.CrewId,
                                   ScheduleName = _grp.Key.ScheduleName,
                                   JobGroup = _grp.Key.JobGroup,
                                   JobGropCode = _grp.Key.JobGroupCode,
                                   Name = _grp.Key.Name,
                                   PID = _grp.Key.PID,
                                   Type = _grp.Key.ValidTypes,
                                   HomeBase = _grp.Key.BaseAirport,
                                   ValidTypes = _grp.Key.ValidTypes,

                                   Legs = _grp.Where(q => q.IsPositioning == false).Count(),
                                   DH = _grp.Where(q => q.IsPositioning == true).Count(),
                                   FlightTime = _grp.Sum(q => q.FlightTime),
                                   BlockTime = _grp.Sum(q => q.BlockTime),
                                   JLFlightTime = _grp.Sum(q => q.JL_FlightTime),
                                   JLBlockTime = _grp.Sum(q => q.JL_BlockTime),
                                   FixTime = _grp.Sum(q => q.FixTime),
                                   OA = _grp.Key.FlightPlanId
                               }
                              ).ToList();
                foreach (var x in _query)
                    x.GroupOrder = getOrder(x.JobGroup);
                if (external_list.Count > 0)
                    _query = _query.Concat(external_list).ToList();

                _query = _query.OrderBy(q => q.GroupOrder).ThenByDescending(q => q.FixTime).ToList();
                return Ok(_query);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                return Ok(msg);
            }


        }



        [Route("api/crew/flights/rank/{rank}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetCrewFlightTimesRank(string rank, DateTime df, DateTime dt)
        {
            try
            {
                List<FlightTimeDto> external_list = new List<FlightTimeDto>();


                var ctx = new ppa_Entities();
                var _df = df.Date;
                var _dt = dt.Date.AddDays(1);

                var _baseQ = from x in ctx.ViewLegCrews
                             where x.STDLocal >= _df && x.STDLocal < _dt && x.FlightStatusID != 4
                             select x;

                if (rank != "-1")
                {
                    switch (rank)
                    {
                        case "1":
                            _baseQ = _baseQ.Where(q => q.JobGroup == "TRI" || q.JobGroup == "TRE" || q.JobGroup == "LTC" || q.JobGroup == "P1");
                            break;
                        case "10":
                            _baseQ = _baseQ.Where(q => q.JobGroup == "TRI" || q.JobGroup == "TRE" || q.JobGroup == "LTC" || q.JobGroup == "P1" || q.JobGroup == "P2");
                            break;
                        case "2":
                            _baseQ = _baseQ.Where(q => q.JobGroup == "P1");
                            break;
                        case "3":
                            _baseQ = _baseQ.Where(q => q.JobGroup == "P2");
                            break;
                        case "4":
                            _baseQ = _baseQ.Where(q => q.JobGroup == "TRE");
                            break;
                        case "5":
                            _baseQ = _baseQ.Where(q => q.JobGroup == "TRI");
                            break;
                        case "6":
                            _baseQ = _baseQ.Where(q => q.JobGroup == "ISCCM" || q.JobGroup == "SCCM");
                            break;
                        case "11":
                            _baseQ = _baseQ.Where(q => q.JobGroup == "ISCCM" || q.JobGroup == "SCCM" || q.JobGroup == "CCM");
                            break;
                        case "7":
                            _baseQ = _baseQ.Where(q => q.JobGroup == "ISCCM");
                            break;
                        case "8":
                            _baseQ = _baseQ.Where(q => q.JobGroup == "SCCM");
                            break;
                        case "9":
                            _baseQ = _baseQ.Where(q => q.JobGroup == "CCM");
                            break;
                        default:
                            break;
                    }
                }




                var _query = (
                               from x in _baseQ

                               group x by new { x.CrewId, x.ScheduleName, x.JobGroup, x.JobGroupCode, x.Name, x.PID } into _grp
                               select new FlightTimeDto()
                               {
                                   CrewId = _grp.Key.CrewId,
                                   ScheduleName = _grp.Key.ScheduleName,
                                   JobGroup = _grp.Key.JobGroup,
                                   JobGropCode = _grp.Key.JobGroupCode,
                                   Name = _grp.Key.Name,
                                   PID = _grp.Key.PID,

                                   Legs = _grp.Where(q => q.IsPositioning == false).Count(),
                                   DH = _grp.Where(q => q.IsPositioning == true).Count(),
                                   FlightTime = _grp.Sum(q => q.FlightTime),
                                   BlockTime = _grp.Sum(q => q.BlockTime),
                                   JLFlightTime = _grp.Sum(q => q.JL_FlightTime),
                                   JLBlockTime = _grp.Sum(q => q.JL_BlockTime),
                                   FixTime = _grp.Sum(q => q.FixTime),
                               }
                              ).ToList();
                foreach (var x in _query)
                    x.GroupOrder = getOrder(x.JobGroup);
                if (external_list.Count > 0)
                    _query = _query.Concat(external_list).ToList();

                _query = _query.OrderBy(q => q.GroupOrder).ThenByDescending(q => q.FixTime).ToList();
                return Ok(_query);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                return Ok(msg);
            }


        }


        [Route("api/crew/flights/grp/{id}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetCrewFlightTimesByCrew(int id, DateTime df, DateTime dt)
        {
            try
            {
                List<FlightTimeDto> external_list = new List<FlightTimeDto>();
                //try
                //{
                //    var ids = "-4325_-4287_-4289";
                //    var dfStr = df.ToString("yyyy-MM-dd");
                //    var dtStr = dt.ToString("yyyy-MM-dd");
                //    var extStr = GETUrl("https://apireportflight.apvaresh.com/api/crew/flights/ids/" + ids + "?df=" + dfStr + "&dt=" + dtStr);
                //    external_list = JsonConvert.DeserializeObject<List<FlightTimeDto>>(extStr);
                //}
                //catch (Exception ex)
                //{

                //}

                var ctx = new ppa_Entities();
                var _df = df.Date;
                var _dt = dt.Date.AddDays(1);
                var _query = (
                               from x in ctx.ViewLegCrews
                               where x.CrewId == id && x.STDLocal >= _df && x.STDLocal < _dt && x.FlightStatusID != 4
                               group x by new { x.CrewId, x.ScheduleName, x.JobGroup, x.JobGroupCode, x.Name, x.PID } into _grp
                               select new FlightTimeDto()
                               {
                                   CrewId = _grp.Key.CrewId,
                                   ScheduleName = _grp.Key.ScheduleName,
                                   JobGroup = _grp.Key.JobGroup,
                                   JobGropCode = _grp.Key.JobGroupCode,
                                   Name = _grp.Key.Name,
                                   PID = _grp.Key.PID,

                                   Legs = _grp.Where(q => q.IsPositioning == false).Count(),
                                   DH = _grp.Where(q => q.IsPositioning == true).Count(),
                                   FlightTime = _grp.Sum(q => q.FlightTime),
                                   BlockTime = _grp.Sum(q => q.BlockTime),
                                   JLFlightTime = _grp.Sum(q => q.JL_FlightTime),
                                   JLBlockTime = _grp.Sum(q => q.JL_BlockTime),
                                   FixTime = _grp.Sum(q => q.FixTime),
                               }
                              ).ToList();
                // foreach (var x in _query)
                //     x.GroupOrder = getOrder(x.JobGroup);
                // if (external_list.Count > 0)
                //     _query = _query.Concat(external_list).ToList();

                // _query = _query.OrderBy(q => q.GroupOrder).ThenByDescending(q => q.FixTime);
                var result = _query.FirstOrDefault();
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

        [Route("api/crew/flights/ids/{ids}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetCrewFlightTimesByIds(string ids, DateTime df, DateTime dt)
        {

            var _ids = ids.Split('_').Select(q => (Nullable<int>)Convert.ToInt32(q)).ToList();

            var ctx = new ppa_Entities();
            var _df = df.Date;
            var _dt = dt.Date.AddDays(1);
            var _query = (
                           from x in ctx.ViewLegCrews
                           where x.STDLocal >= _df && x.STDLocal < _dt && x.FlightStatusID != 4 && _ids.Contains(x.CrewId)
                           group x by new { x.CrewId, x.ScheduleName, x.JobGroup, x.JobGroupCode, x.Name } into _grp
                           select new FlightTimeDto()
                           {
                               CrewId = _grp.Key.CrewId,
                               ScheduleName = _grp.Key.ScheduleName,
                               JobGroup = _grp.Key.JobGroup,
                               JobGropCode = _grp.Key.JobGroupCode,
                               Name = _grp.Key.Name,

                               Legs = _grp.Where(q => q.IsPositioning == false).Count(),
                               DH = _grp.Where(q => q.IsPositioning == true).Count(),
                               FlightTime = _grp.Sum(q => q.FlightTime),
                               BlockTime = _grp.Sum(q => q.BlockTime),
                               JLFlightTime = _grp.Sum(q => q.JL_FlightTime),
                               JLBlockTime = _grp.Sum(q => q.JL_BlockTime),
                               FixTime = _grp.Sum(q => q.FixTime),
                           }
                          ).ToList();
            foreach (var x in _query)
                x.GroupOrder = getOrder(x.JobGroup);
            _query = _query.OrderBy(q => q.GroupOrder).ThenByDescending(q => q.FixTime).ToList();
            return Ok(_query);
        }

        [Route("api/crew/flights/detail/{id}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetCrewFlightTimes(int id, DateTime df, DateTime dt)
        {
            var ctx = new ppa_Entities();
            var _df = df.Date;
            var _dt = dt.Date.AddDays(1);
            var _query = (
                           from x in ctx.ViewLegCrews
                           where x.STDLocal >= _df && x.STDLocal < _dt && x.FlightStatusID != 4 && x.CrewId == id
                           orderby x.STD
                           select x

                          ).ToList();

            List<ViewLegCrew> external_list = new List<ViewLegCrew>();
            var _ids = new List<int>() { -4325, -4287, -4289 };
            if (_ids.IndexOf(id) != -1)
            {
                try
                {
                    var ids = "-4325_-4287_-4289";
                    var dfStr = df.ToString("yyyy-MM-dd");
                    var dtStr = dt.ToString("yyyy-MM-dd");
                    var extStr = GETUrl("https://apireportflight.apvaresh.com/api/crew/flights/detail/" + id + "?df=" + dfStr + "&dt=" + dtStr);
                    external_list = JsonConvert.DeserializeObject<List<ViewLegCrew>>(extStr);
                }
                catch (Exception ex)
                {

                }
                if (external_list.Count > 0)
                    _query = _query.Concat(external_list).OrderBy(q => q.STD).ToList();
            }


            return Ok(_query);
        }



        public string getDutyAbbr(int t)
        {
            switch (t)
            {
                case 100002:
                    return "SCK";
                    break;
                case 1167:
                    return "STBP";
                    break;
                case 1168:
                    return "STBA";
                    break;
                case 5001:
                    return "OFC";
                    break;
                case 5000:
                    return "TRN";
                    break;
                case 300008:
                    return "DTY";
                    break;
                case 300009:
                    return "RST";
                    break;
                case 1169:
                    return "VAC";
                    break;
                case 1170:
                    return "RES";
                    break;
                default:
                    return "DTY";
                    break;
            }
        }

        [Route("api/crew/calendar/{id}/{year}/{month}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetCrewCalendar(int id, int year, int month)
        {
            var ctx = new ppa_Entities();

            //if (year)
            var query = from x in ctx.FDPs
                        where x.CrewId == id
                        select x;
            var query_result = query.ToList();

            var grps = from x in query_result
                       group x by new { Date = ((DateTime)x.DateStart).AddMinutes(210).Date, x.DutyType } into grp
                       select new
                       {
                           Start = grp.Key.Date,
                           End = grp.Key.Date.AddHours(23).AddMinutes(59),
                           Type = grp.Key.DutyType,
                           Total = grp.Key.DutyType != 1165 ? 0 : grp.Sum(q => string.IsNullOrEmpty(q.InitNo) ? 0 : q.InitNo.Split('_').Count()),
                           TypeStr = getDutyAbbr(grp.Key.DutyType),
                           ItemIds = grp.OrderBy(q => q.DateStart).Select(q => q.Id).ToList()

                       };
            var grps_result = grps.ToList();
            return Ok(grps_result);
        }


        [Route("api/crew/calendar/day/{id}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetCrewCalendar(int id, DateTime dt)
        {
            var ctx = new ppa_Entities();
            var dt2 = dt.Date.AddDays(1).AddMinutes(-210);
            dt = dt.Date.AddMinutes(-210);

            //if (year)
            var query = from x in ctx.FDPs
                        where x.CrewId == id && x.DateStart >= dt && x.DateStart <= dt2
                        select x;
            var query_result = query.ToList();

            var flights = (from x in ctx.ViewCrewFlightApps
                           where x.CrewId == id && x.STD >= dt && x.STD <= dt2
                           orderby x.STD
                           select x).ToList();
            var trnIds = (from x in query_result
                          where x.DutyType == 5000
                          select x.Id).ToList();
            var trns = (from x in ctx.ViewCourseFDPs
                        where trnIds.Contains(x.FDPId)
                        group x by new { x.CourseId, x.Title, x.Organization, x.Location, x.HoldingType, x.Instructor, x.No, x.DateEnd, x.DateStart } into grp
                        select new
                        {
                            grp.Key.CourseId,
                            grp.Key.Title,
                            grp.Key.Organization,
                            grp.Key.Location,
                            grp.Key.HoldingType,
                            grp.Key.Instructor,
                            grp.Key.No,
                            grp.Key.DateEnd,
                            grp.Key.DateStart,
                            Sessions = grp.OrderBy(q => q.SessionStart).ToList()
                        }).OrderBy(q => q.DateStart).ToList();


            var others = query_result.Where(q => q.DutyType != 5000 && q.DutyType != 1165).Select(q => new
            {
                Id = q.Id,
                Start = ((DateTime)q.DateStart).AddMinutes(210),
                End = ((DateTime)q.DateEnd).AddMinutes(210),
                TypeStr = getDutyAbbr(q.DutyType),
                RestTo = ((DateTime)q.InitRestTo).AddMinutes(210),
            }).ToList();
            var total_result = new
            {
                flights,
                trns,
                others
            };


            return Ok(total_result);
        }


        [Route("api/flightpax/daily")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetFlightsPaxDaily(DateTime df, DateTime dt)
        {
            var cmd = "select * from viewflightpax ";
            try
            {
                var context = new ppa_Entities();
                var dayprm = "Date";

                string whr = "  (" + dayprm + ">= '" + df.ToString("yyyy-MM-dd") + "' and " + dayprm + "<='" + dt.ToString("yyyy-MM-dd") + "')";

                cmd = cmd + " WHERE " + whr + " ORDER BY STD,Register";

                var flts = context.ViewFlightPaxes
                            .SqlQuery(cmd)
                            .ToList<ViewFlightPax>();

                return Ok(flts);
            }
            catch (Exception ex)
            {
                return Ok(cmd);
            }

        }


        [Route("api/stat/flight/{year}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetStatFlight(int year)
        {
            var ctx = new ppa_Entities();

            var totalQuery = ctx.ViewRegHistoryYearlies;
            var total = totalQuery.OrderBy(q => q.Year)
                .ThenBy(q => q.Month)

                .ToList();


            var pax_route_total = ctx.rpt_pax_total.OrderByDescending(q => q.TotalPax).ToList();

            var result = new
            {
                total
              ,
                pax_route_total
            };
            return Ok(result);
        }
        public partial class dto_summary_flight_pax
        {
            public int season { get; set; }
            public string season_title { get; set; }
            public string pyear_month_name { get; set; }
            public int pyear { get; set; }
            public int pmonth { get; set; }
            public string pmonth_name { get; set; }
            public string register { get; set; }
            public int register_id { get; set; }
            public string aircraft_type { get; set; }
            public string origin_icao { get; set; }
            public string destination_icao { get; set; }
            public string origin { get; set; }
            public string destination { get; set; }
            public Nullable<int> pax_adult { get; set; }
            public Nullable<int> pax_child { get; set; }
            public Nullable<int> pax_infant { get; set; }
            public Nullable<int> pax_total { get; set; }
            public Nullable<int> pax_rev { get; set; }
            public Nullable<int> baggage_weight { get; set; }
            public Nullable<int> cargo_weight { get; set; }
            public Nullable<decimal> fuel_uplift { get; set; }
            public Nullable<decimal> fuel_used { get; set; }
            public Nullable<int> block_time { get; set; }
            public Nullable<int> flight_time { get; set; }
            public Nullable<int> flights_count { get; set; }
            public Nullable<int> std_hour_0004 { get; set; }
            public Nullable<int> std_hour_0408 { get; set; }
            public Nullable<int> std_hour_0812 { get; set; }
            public Nullable<int> std_hour_1216 { get; set; }
            public Nullable<int> std_hour_1620 { get; set; }
            public Nullable<int> std_hour_2024 { get; set; }


            public string route { get; set; }
            public int? grp_total_pax { get; set; }
            public int? grp_total_pax_reg { get; set; }

            public decimal? grp_total_fuel { get; set; }
            public decimal? grp_total_fuel_reg { get; set; }

            public int? grp_total_block { get; set; }
            public int? grp_total_block_reg { get; set; }

            public int? grp_total_flight { get; set; }
            public int? grp_total_flight_reg { get; set; }

            public int? grp_total_count { get; set; }
            public int? grp_total_count_reg { get; set; }
        }
        [Route("api/summary/pax/monthly/{y1}/{y2}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetSummaryPaxMonthly(int y1,int y2)
        {
             
            try
            {
                var context = new ppa_Entities();
                var query = from x in context.summary_flight_pax
                            select x;
                if (y1 != -1)
                    query = query.Where(q => q.pyear >= y1);
                if (y2 != -1)
                    query = query.Where(q => q.pyear <= y2);

                var query_result = query.OrderBy(q => q.pyear).ThenBy(q => q.pmonth).ThenBy(q => q.aircraft_type).ThenBy(q => q.register).ToList();
                var result = new List<dto_summary_flight_pax>();
                foreach (var x in query_result)
                {
                    var item =JsonConvert.DeserializeObject<dto_summary_flight_pax>( JsonConvert.SerializeObject(x));
                    item.route = item.origin + "-" + item.destination;
                    result.Add(item);
                }
                var grp_total = (from x in result
                                 group x by new { x.route } into grp
                                 select new
                                 {
                                     grp.Key.route,
                                     total_pax = grp.Sum(q => q.pax_total),
                                     total_fuel = grp.Sum(q => q.fuel_used),
                                     total_block = grp.Sum(q => q.block_time),
                                     total_flight = grp.Sum(q => q.flight_time),
                                     total_count = grp.Sum(q => q.flights_count),

                                 }).ToList();
                var grp_total_month = (from x in result
                                 group x by new { x.route,x.pyear,x.pmonth,x.pmonth_name } into grp
                                 select new
                                 {
                                     grp.Key.pyear,
                                     grp.Key.pmonth,
                                     grp.Key.pmonth_name,
                                     grp.Key.route,

                                     total_pax = grp.Sum(q => q.pax_total),
                                     total_fuel = grp.Sum(q => q.fuel_used),
                                     total_block = grp.Sum(q => q.block_time),
                                     total_flight = grp.Sum(q => q.flight_time),
                                     total_count = grp.Sum(q => q.flights_count),

                                 }).ToList();
                var grp_total_reg = (from x in result
                                 group x by new { x.route,x.register_id,x.register,x.aircraft_type } into grp
                                 select new
                                 {
                                     grp.Key.route,
                                     grp.Key.register_id,
                                     grp.Key.register,
                                     total_pax = grp.Sum(q => q.pax_total),
                                     total_fuel = grp.Sum(q => q.fuel_used),
                                     total_block = grp.Sum(q => q.block_time),
                                     total_flight = grp.Sum(q => q.flight_time),
                                     total_count = grp.Sum(q => q.flights_count),

                                 }).ToList();
                var grp_total_reg_month = (from x in result
                                     group x by new { x.route, x.pyear, x.pmonth, x.pmonth_name,x.aircraft_type,x.register, x.register_id } into grp
                                     select new
                                     {
                                         grp.Key.pyear,
                                         grp.Key.pmonth,
                                         grp.Key.pmonth_name,
                                         grp.Key.route,
                                         grp.Key.register_id,
                                         grp.Key.register,
                                         total_pax = grp.Sum(q => q.pax_total),
                                         total_fuel = grp.Sum(q => q.fuel_used),
                                         total_block = grp.Sum(q => q.block_time),
                                         total_flight = grp.Sum(q => q.flight_time),
                                         total_count = grp.Sum(q => q.flights_count),

                                     }).ToList();

                foreach (var x in result)
                {
                    var obj = grp_total.FirstOrDefault(q => q.route == x.route);
                    var obj_reg = grp_total_reg.FirstOrDefault(q => q.route == x.route && q.register_id == x.register_id);
                    if (obj != null)
                    {
                        x.grp_total_block = obj.total_block;
                        x.grp_total_count = obj.total_count;
                        x.grp_total_flight = obj.total_flight;
                        x.grp_total_fuel = obj.total_fuel;
                        x.grp_total_pax = obj.total_pax;
                    }
                    if (obj_reg != null)
                    {
                        x.grp_total_block_reg = obj_reg.total_block;
                        x.grp_total_count_reg = obj_reg.total_count;
                        x.grp_total_flight_reg = obj_reg.total_flight;
                        x.grp_total_fuel_reg = obj_reg.total_fuel;
                        x.grp_total_pax_reg = obj_reg.total_pax;
                    }
                }


                return Ok(new { 
                    result,
                    grp_total,
                    grp_total_month,
                    grp_total_reg,
                    grp_total_reg_month

                });
            }
            catch (Exception ex)
            {
                return Ok(false);
            }

        }




    }
}
