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
using ApiPlanning.Models;
using System.Net.Http.Headers;
using System.Drawing;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Threading;
using ApiPlanning.ViewModels;

namespace ApiPlanning.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PlanningController : ApiController
    {

        [Route("api/plan/newtime")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostNewTime(SimpleDto dto)
        {
            var _context = new Models.dbEntities();
            var flights = await _context.FlightInformations.Where(q => dto.ids.Contains(q.ID)).ToListAsync();
            foreach (var f in flights)
                if (f.NewTime == null || f.NewTime == 0)
                    f.NewTime = 1;
                else
                    f.NewTime = 0;
            var result = await _context.SaveChangesAsync();
            return Ok(flights);



        }
        [Route("api/plan/newregister")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostNewRegister(SimpleDto dto)
        {
            var _context = new Models.dbEntities();
            var flights = await _context.FlightInformations.Where(q => dto.ids.Contains(q.ID)).ToListAsync();
            foreach (var f in flights)
                if (f.NewReg == null || f.NewReg == 0)
                    f.NewReg = 1;
                else
                    f.NewReg = 0;
            var result = await _context.SaveChangesAsync();
            return Ok(flights);



        }

        public DateTime parseDate(string str)
        {
            var prts = str.Split('-').Select(q => Convert.ToInt32(q)).ToList();
            return new DateTime(prts[0], prts[1], prts[2], prts[3], prts[4], 0);

        }
        public List<DateTime> GetInvervalDates(int type, DateTime start, DateTime end, List<int> days = null)
        {
            List<DateTime> result = new List<DateTime>();
            var minDate = start.Date;
            //var maxDate = end.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            var maxDate = end.Date.AddDays(1);
            //  while (minDate <= maxDate)
            while (minDate < maxDate)
            {
                switch (type)
                {
                    case 1:
                        result.Add(minDate);
                        break;
                    case 2:
                        var d = (int)minDate.DayOfWeek;
                        if (days.IndexOf(d) != -1)
                            result.Add(minDate);
                        break;
                    default:
                        break;
                }
                minDate = minDate.AddDays(1);
            }
            return result;
        }

        public DateTime copy_to_date(string str)
        {
            var prts = str.Split('-').Select(q => Convert.ToInt32(q)).ToList();
            return new DateTime(prts[0], prts[1], prts[2]);
        }

        public class copy_flt_obj
        {
            public DateTime std { get; set; }
            public DateTime sta { get; set; }
            public DateTime flt_date { get { return std.AddMinutes(210).Date; } }
            public string flt_no { get; set; }
            public int register_id { get; set; }
            public string key { get; set; }
            public bool is_exit { get; set; }
            public FlightInformation source { get; set; }
        }
        [Route("api/plan/copy")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostFlightCopy(ViewModels.copy_dto dto)
        {
            var _context = new Models.dbEntities();
            var flt = _context.FlightInformations.OrderByDescending(q => q.ID).FirstOrDefault();
            var source_flts = _context.FlightInformations.Where(q => dto.FlightIds.Contains(q.ID)).ToList();
            var interval_from = copy_to_date(dto.IntervalFromRAW);
            var interval_to = copy_to_date(dto.IntervalToRAW);

            var interval_days = GetInvervalDates(2, interval_from, interval_to, dto.Days);

            var ref_day = copy_to_date(dto.RefDayRAW);
            List<copy_flt_obj> objs = new List<copy_flt_obj>();
            foreach (var s_slt in source_flts)
            {
                var utc_diff_std = ((DateTime)s_slt.STD).Date < ref_day.Date ? -1 : 0; //((DateTime)s_slt.STD).Date.Subtract(ref_day).TotalDays>0?1:0;
                var utc_diff_sta = ((DateTime)s_slt.STA).Date < ref_day.Date ? -1 : 0; //((DateTime)s_slt.STA).Date.Subtract(ref_day).TotalDays>0;
                var std_hh = ((DateTime)s_slt.STD).Hour;
                var std_mm = ((DateTime)s_slt.STD).Minute;
                var sta_hh = ((DateTime)s_slt.STA).Hour;
                var sta_mm = ((DateTime)s_slt.STA).Minute;
                foreach (var dt in interval_days)
                {
                    var obj = new copy_flt_obj()
                    {
                        source = s_slt,
                        flt_no = s_slt.FlightNumber,
                        std = dt.Date.AddDays(utc_diff_std).AddHours(std_hh).AddMinutes(std_mm),
                        sta = dt.Date.AddDays(utc_diff_sta).AddHours(sta_hh).AddMinutes(sta_mm),
                        is_exit = false


                    };
                    obj.key = obj.std.ToString("yyyyMMdd") + "_" + obj.flt_no;
                    objs.Add(obj);
                }
            }

            var new_keys = objs.Select(q => q.key).ToList();
            var _s = interval_from.AddDays(-1);
            var _e = interval_to.AddDays(1);
            var exist_flts = (from x in _context.HelperFlightKeys
                              where x.STD >= _s && x.STD <= _e && new_keys.Contains(x.KeyUtc)
                              select x.KeyUtc).ToList();
            var _new_flights = new List<FlightInformation>();
            foreach (var obj in objs)
            {
                obj.is_exit = exist_flts.FirstOrDefault(q => q == obj.key) != null;
                if (!obj.is_exit)
                {
                    var new_flight = new FlightInformation()
                    {
                        STD = obj.std,
                        STA = obj.sta,
                        Takeoff = obj.std,
                        Landing = obj.sta,
                        ChocksIn = obj.sta,
                        ChocksOut = obj.std,

                        RegisterID=obj.source.RegisterID,
                        FlightNumber=obj.source.FlightNumber,
                        FlightStatusID=1,
                        FlightTypeID=obj.source.FlightTypeID,
                        FromAirportId=obj.source.FromAirportId,
                        ToAirportId=obj.source.ToAirportId,
                        FlightH=obj.source.FlightH,
                        FlightM=obj.source.FlightM,
                        CustomerId=obj.source.CustomerId,

                          

                    };
                    _new_flights.Add(new_flight);
                    _context.FlightInformations.Add(new_flight);
                }
            }

            var saveResult = await _context.SaveChangesAsync();

            var new_ids = _new_flights.Select(q => q.ID).ToList();
            var _from = copy_to_date(dto.FromRAW).AddDays(-1);
            var _to = copy_to_date(dto.ToRAW).AddDays(1);
            var fg = await _context.ViewFlightsGantts.Where(q => new_ids.Contains(q.ID)
               && q.STDDay >= _from && q.STDDay <= _to
               ).ToListAsync();

            var odtos = new List<ViewFlightsGanttDto>();
            foreach (var f in fg)
            {
                ViewModels.ViewFlightsGanttDto odto = new ViewFlightsGanttDto();

                ViewModels.ViewFlightsGanttDto.FillDto(f, odto, 0, 1);
                odto.resourceId.Add((int)odto.RegisterID);
                odtos.Add(odto);
            }

            var resgroups = from x in fg
                            group x by new { x.AircraftType, AircraftTypeId = x.TypeId }
                           into grp
                            select new { groupId = grp.Key.AircraftTypeId, Title = grp.Key.AircraftType };
            var ressq = (from x in fg
                         group x by new { x.RegisterID, x.Register, x.TypeId }
                     into grp

                         orderby getOrderIndex(grp.Key.Register, new List<string>())
                         select new { resourceId = grp.Key.RegisterID, resourceName = grp.Key.Register, groupId = grp.Key.TypeId }).ToList();


            var result = new
            {
                duplicated = objs.Where(q => q.is_exit).Select(q => new { q.flt_date, q.flt_no }).ToList(),
                new_flts_count = objs.Where(q => !q.is_exit).Count(),
                flights = odtos,
                resgroups,
                ressq,
                
            };

            return Ok(result);




        }
        public class dto_hide
        {
            public string from_raw { get; set; }
            public string to_raw { get; set; }
            public List<int> ids { get; set; }
            public bool is_hidden { get; set; }
        }
        [Route("api/plan/hide")]
        //2023-11-23
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostFlightHide(dto_hide dto)
        {
            var context = new Models.dbEntities();
            if (!string.IsNullOrEmpty(dto.from_raw))
            {
                var dt_from = copy_to_date(dto.from_raw);
                var dt_to = copy_to_date(dto.to_raw);
               
                var cmd = "update flightinformation set CPDH="+(dto.is_hidden?"1":"0")+ " where FlightStatusID=1 and  cast(std as date)>='" + dt_from.ToString("yyyy-MM-dd")+"' and cast(std as date)<='"+ dt_to.ToString("yyyy-MM-dd") + "'";
                context.Database.ExecuteSqlCommand(cmd);
                context.SaveChanges();
            }
            else
            {
                var flts = context.FlightInformations.Where(q => dto.ids.Contains(q.ID) && q.FlightStatusID==1).ToList();
                foreach (var x in flts)
                    x.CPDH = dto.is_hidden;
                context.SaveChanges();
            }

            return Ok(true);


        }

        [Route("api/plan/group/save/utc")]
        //2023-11-23
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostFlightGroupUTC(ViewModels.FlightDto dto)
        {
            try
            {
                List<string> messages = new List<string>();

                var time_mode = dto.time_mode;
                var apts = apt_info.get_apts();
                var apt_from = apts.FirstOrDefault(q => q.Id == dto.FromAirportId);
                var apt_to = apts.FirstOrDefault(q => q.Id == dto.ToAirportId);

                var _context = new Models.dbEntities();
                bool isUtc = true;



                //var validate = unitOfWork.FlightRepository.ValidateFlight(dto);
                //if (validate.Code != HttpStatusCode.OK)
                //    return validate;
                var nowOffset = isUtc ? 0 : TimeZoneInfo.Local.GetUtcOffset(DateTime.Now).TotalMinutes;
                

                dto.STD = parseDate(dto.STDRAW);
                dto.STA = parseDate(dto.STARAW);
                DateTime dto_std =(DateTime) dto.STD;
                if (time_mode == "lcb")
                {
                    var _offset = -210; //TimeZoneInfo.Local.GetUtcOffset(DateTime.Now).TotalMinutes;
                    dto.STD = ((DateTime)dto.STD).AddMinutes(_offset);
                    dto.STA = ((DateTime)dto.STA).AddMinutes(_offset);
                    dto_std = (DateTime)dto.STD;

                }
                else if (time_mode == "lcl")
                {
                    var from_offset = apt_from == null ? -210 : -1 * apt_from.UTC;
                    var to_offset = apt_to == null ? -210 : -1 * apt_to.UTC;
                    dto.STD = ((DateTime)dto.STD).AddMinutes(from_offset);
                    dto.STA = ((DateTime)dto.STA).AddMinutes(to_offset);
                    dto_std = (DateTime)dto.STD;
                }


                dto.RefDate = parseDate(dto.RefDateRAW);
                dto.IntervalFrom = parseDate(dto.IntervalFromRAW);
                dto.IntervalTo = parseDate(dto.IntervalToRAW);
                dto.Days = isUtc ? dto.DaysUTC.ToList() : dto.Days;




                var stdOffset = isUtc ? 0 : TimeZoneInfo.Local.GetUtcOffset((DateTime)dto.STD).TotalMinutes;
                var localSTD = ((DateTime)dto.STD).AddMinutes(stdOffset);
                var _addDay = localSTD.Day == ((DateTime)dto.STD).Day ? 0 : 1;

                var stdHours = ((DateTime)dto.STD).Hour;
                var stdMinutes = ((DateTime)dto.STD).Minute;
                var staHours = ((DateTime)dto.STA).Hour;
                var staMinutes = ((DateTime)dto.STA).Minute;
                var duration = (((DateTime)dto.STA) - ((DateTime)dto.STD)).TotalMinutes;


                var intervalDays = GetInvervalDates((int)dto.Interval, (DateTime)dto.IntervalFrom, (DateTime)dto.IntervalTo, dto.Days);

                var i_dates = intervalDays.Select(q => (Nullable<DateTime>)q).ToList();

                var _exist_flights = await (from x in _context.ViewFlightsGantts
                                            where i_dates.Contains(x.Date) && x.FlightNumber == dto.FlightNumber
                                            select new { x.Date, x.FlightNumber, x.STD }).ToListAsync();
                var exist_flights = _exist_flights.Select(q => q.Date).ToList();

                foreach (var x in _exist_flights)
                {
                    messages.Add(((DateTime)x.STD).ToString("yyyy-MMM-dd HH:mm") + "(utc) " + x.FlightNumber + ": Duplicated Flight Number Error.");
                }
                intervalDays = intervalDays.Where(q => !exist_flights.Contains(q)).ToList();


                FlightInformation entity = null;
                // FlightChangeHistory changeLog = null;

                List<FlightInformation> flights = new List<FlightInformation>();
                var str = DateTime.Now.ToString("MMddmmss");
                var flightGroup = Convert.ToInt32(str);
                var _dec_day = false;
                foreach (var dt in intervalDays)
                {

                    entity = new FlightInformation();
                    _context.FlightInformations.Add(entity);
                    flights.Add(entity);
                   
                    //if (entity.STD != null)
                    //{
                    //    var oldSTD = ((DateTime)entity.STD).AddMinutes(270).Date;
                    //    var newSTD = ((DateTime)dto.STD).AddMinutes(270).Date;
                    //    if (oldSTD != newSTD)
                    //    {
                    //        entity.FlightDate = oldSTD;
                    //    }
                    //}


                    ViewModels.FlightDto.Fill(entity, dto);
                    var _fltDate = new DateTime(dt.Year, dt.Month, dt.Day, 1, 0, 0);
                    var fltOffset = isUtc ? 0 : -1 * TimeZoneInfo.Local.GetUtcOffset(_fltDate).TotalMinutes;
                    entity.FlightGroupID = flightGroup;


                    // var _std = new DateTime(dt.Year, dt.Month, dt.Day, (int)dto.STDHH, (int)dto.STDMM, 0);
                    // _std = _std.AddDays(_addDay).AddMinutes(fltOffset);


                    // entity.STD = _std;
                  //  var temp_std = new DateTime(dt.Year, dt.Month, dt.Day, stdHours, stdMinutes, 0);
                  //  if ((stdHours >= 0 && stdHours <= 3) && staMinutes < 30)
                  //      temp_std = temp_std.AddDays(-1);

                    entity.STD = new DateTime(dt.Year, dt.Month, dt.Day, stdHours, stdMinutes, 0);
                    if (dt == intervalDays.First())
                    {
                        _dec_day = dto_std.Day != ((DateTime)entity.STD).Day;
                    }
                    //if (dto_std.Day != ((DateTime)entity.STD).Day)
                    if (_dec_day)
                        entity.STD = ((DateTime)entity.STD).AddDays(-1);


                   entity.STA = ((DateTime)entity.STD).AddMinutes(duration);
                    entity.ChocksOut = entity.STD;
                    entity.ChocksIn = entity.STA;
                    entity.Takeoff = entity.STD;
                    entity.Landing = entity.STA;
                    entity.BoxId = dto.BoxId;
                }


                var saveResult = await _context.SaveChangesAsync();
                // if (saveResult.Code != HttpStatusCode.OK)
                //     return saveResult;

                //if (dto.SMSNira == 1)
                // {
                //     foreach (var x in flights)
                //         await unitOfWork.FlightRepository.NotifyNira(x.ID, dto.UserName);
                // }

                //bip
                var flightIds = flights.Select(q => q.ID).ToList();
                var beginDate = ((DateTime)dto.RefDate).Date;
                var endDate = ((DateTime)dto.RefDate).Date.AddDays((int)dto.RefDays).Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                var fg = await _context.ViewFlightsGantts.Where(q => flightIds.Contains(q.ID)
                && q.STDDay >= beginDate && q.STDDay <= endDate
                ).ToListAsync();



                var odtos = new List<ViewFlightsGanttDto>();
                foreach (var f in fg)
                {
                    ViewModels.ViewFlightsGanttDto odto = new ViewFlightsGanttDto();

                    ViewModels.ViewFlightsGanttDto.FillDto(f, odto, 0, 1);
                    odto.resourceId.Add((int)odto.RegisterID);
                    odtos.Add(odto);
                }

                var resgroups = from x in fg
                                group x by new { x.AircraftType, AircraftTypeId = x.TypeId }
                               into grp
                                select new { groupId = grp.Key.AircraftTypeId, Title = grp.Key.AircraftType };
                var ressq = (from x in fg
                             group x by new { x.RegisterID, x.Register, x.TypeId }
                         into grp

                             orderby getOrderIndex(grp.Key.Register, new List<string>())
                             select new { resourceId = grp.Key.RegisterID, resourceName = grp.Key.Register, groupId = grp.Key.TypeId }).ToList();




                var oresult = new
                {
                    flights = odtos,
                    resgroups,
                    ressq,
                    messages
                };

                return Ok(/*entity*/oresult);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += ex.InnerException.Message;
                return Ok(msg);
            }

        }

        [Route("api/plan/group/update/utc")]
        //2024-02-16
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostFlightGroupUpdate(ViewModels.FlightDto dto)
        {
            try
            {
                List<string> messages = new List<string>();


                var time_mode = dto.time_mode;
                var apts = apt_info.get_apts();
                var apt_from = apts.FirstOrDefault(q => q.Id == dto.FromAirportId);
                var apt_to = apts.FirstOrDefault(q => q.Id == dto.ToAirportId);

                var _context = new Models.dbEntities();

                //var validate = unitOfWork.FlightRepository.ValidateFlight(dto);
                //if (validate.Code != HttpStatusCode.OK)
                //    return validate;
                bool isUtc = true;
                var nowOffset = isUtc ? 0 : TimeZoneInfo.Local.GetUtcOffset(DateTime.Now).TotalMinutes;
                dto.STD = parseDate(dto.STDRAW);
                dto.STA = parseDate(dto.STARAW);

                if (time_mode == "lcb")
                {
                    var _offset = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now).TotalMinutes;
                    dto.STD = ((DateTime)dto.STD).AddMinutes(_offset);
                    dto.STA = ((DateTime)dto.STA).AddMinutes(_offset);
                }
                else if (time_mode == "lcl")
                {
                    var from_offset = apt_from == null ? -210 : -1 * apt_from.UTC;
                    var to_offset = apt_to == null ? -210 : -1 * apt_to.UTC;
                    dto.STD = ((DateTime)dto.STD).AddMinutes(from_offset);
                    dto.STA = ((DateTime)dto.STA).AddMinutes(to_offset);
                }



                dto.RefDate = parseDate(dto.RefDateRAW);
                dto.IntervalFrom = parseDate(dto.IntervalFromRAW);
                dto.IntervalTo = parseDate(dto.IntervalToRAW);
                dto.Days = isUtc ? dto.DaysUTC.ToList() : dto.Days;


                if (((DateTime)dto.IntervalFrom).Day!=((DateTime)dto.STD).Day)
                {
                    dto.IntervalFrom = ((DateTime)dto.IntervalFrom).AddDays(-1);
                    dto.IntervalTo = ((DateTime)dto.IntervalTo).AddDays(-1);
                    var _days = dto.Days.ToList();
                    dto.Days =new List<int>();
                    foreach (var d in _days)
                    {
                        if (d == 6)
                            dto.Days.Add(5);
                        else
                            dto.Days.Add(d-1);
                    }
                }

                var intervalDays = GetInvervalDates((int)dto.Interval, (DateTime)dto.IntervalFrom, (DateTime)dto.IntervalTo, dto.Days).Select(q => (Nullable<DateTime>)q).ToList();
                var i_dates = intervalDays.Select(q => (Nullable<DateTime>)q).ToList();

                var exist_flights = await (from x in _context.ViewFlightsGantts
                                           where i_dates.Contains(x.Date) && x.FlightNumber == dto.FlightNumber
                                           select new { x.ID, x.Date }).ToListAsync();
                //intervalDays = intervalDays.Where(q => !exist_flights.Contains(q)).ToList();

                var baseFlight = await _context.FlightInformations.Where(q => q.ID == dto.ID).FirstOrDefaultAsync();
                if (baseFlight == null)
                    return BadRequest();

                var stdHoursBF = ((DateTime)baseFlight.STD).Hour;
                var stdMinutesBF = ((DateTime)baseFlight.STD).Minute;
                var staHoursBF = ((DateTime)baseFlight.STA).Hour;
                var staMinutesBF = ((DateTime)baseFlight.STA).Minute;

                var flightIds = await (from x in _context.ViewLegTimes
                                       where x.FlightNumber == baseFlight.FlightNumber /*&& x.FlightPlanId==baseFlight.FlightGroupID*/ && intervalDays.Contains(x.STDDay)
                                       select x.ID).ToListAsync();
                //var flights = await unitOfWork.FlightRepository.GetFlights().Where(q => flightIds.Contains(q.ID)).ToListAsync();
                List<FlightInformation> flights = new List<FlightInformation>();
                if ((int)dto.CheckTime == 0)
                    flights = await _context.FlightInformations.Where(q => flightIds.Contains(q.ID)).OrderBy(q => q.STD).ToListAsync();
                else
                    flights = await _context.FlightInformations.Where(q => flightIds.Contains(q.ID)

                    ).OrderBy(q => q.STD).ToListAsync();

                var stdOffset = isUtc ? 0 : TimeZoneInfo.Local.GetUtcOffset((DateTime)dto.STD).TotalMinutes;
                var localSTD = ((DateTime)dto.STD).AddMinutes(stdOffset);
                var _addDay = localSTD.Day == ((DateTime)dto.STD).Day ? 0 : 1;

                var stdHours = ((DateTime)dto.STD).Hour;
                var stdMinutes = ((DateTime)dto.STD).Minute;
                var staHours = ((DateTime)dto.STA).Hour;
                var staMinutes = ((DateTime)dto.STA).Minute;
                var duration = (((DateTime)dto.STA) - ((DateTime)dto.STD)).TotalMinutes;

                int utcDiff = 0;
                //if (flights.Count > 0)
                //{
                //    var firstFlight = flights.First();
                //    var __d1 = ((DateTime)firstFlight.STD).Date;
                //    var __d2 = ((DateTime)dto.STD).Date;
                //    if (__d1 > __d2)
                //        utcDiff = -1;
                //    if (((DateTime)firstFlight.STD).Date < ((DateTime)dto.STD).Date)
                //        utcDiff = 1;

                //}

                var nflts = flightIds.Select(q => (Nullable<int>)q).ToList();
                //var fdpitems = await _context.FDPItems.Where(q => nflts.Contains(q.FlightId)).Select(q => q.FlightId).ToListAsync();

                int check_crew = Convert.ToInt32(ConfigurationManager.AppSettings["check_crew"]);
                List<int?> fdpitems = new List<int?>();
                if (check_crew==1)
                  fdpitems = await (from fi in _context.FDPItems
                                      join f in _context.FDPs on fi.FDPId equals f.Id
                                      where nflts.Contains(fi.FlightId) && f.IsTemplate == false
                                      select fi.FlightId).ToListAsync();



                foreach (var entity in flights)
                {
                    var check_exist = exist_flights.FirstOrDefault(q => q.Date == ((DateTime)entity.STD).Date && q.ID != entity.ID);
                    if (check_exist != null)
                    {
                        messages.Add(((DateTime)entity.STD).ToString("yyyy-MMM-dd HH:mm") + "(utc) " + entity.FlightNumber + ": Duplicated Flight Number Error.");
                        continue;
                    }
                    var fdpitem = fdpitems.FirstOrDefault(q => q == entity.ID);
                    if (fdpitem != null)
                    {
                        messages.Add(((DateTime)entity.STD).ToString("yyyy-MMM-dd HH:mm") + "(utc) " + entity.FlightNumber + ": Flight Crew(s) Error.");
                        continue;
                    }

                    var flt_stdHours = ((DateTime)entity.STD).Hour;
                    var flt_stdMinutes = ((DateTime)entity.STD).Minute;
                    var flt_staHours = ((DateTime)entity.STA).Hour;
                    var flt_staMinutes = ((DateTime)entity.STA).Minute;
                    bool exec = true;
                    if (((int)dto.CheckTime) == 1)
                    {
                        exec = flt_stdHours == stdHoursBF && flt_stdMinutes == stdMinutesBF && flt_staHours == staHoursBF && flt_staMinutes == staMinutesBF;
                    }
                    if (entity.FlightStatusID == 1 && exec)
                    {

                        var changeLog = new FlightChangeHistory()
                        {
                            Date = DateTime.Now,
                            FlightId = entity.ID,

                        };

                        _context.FlightChangeHistories.Add(changeLog);
                        changeLog.OldFlightNumer = entity.FlightNumber;
                        changeLog.OldFromAirportId = entity.FromAirportId;
                        changeLog.OldToAirportId = entity.ToAirportId;
                        changeLog.OldSTD = entity.STD;
                        changeLog.OldSTA = entity.STA;
                        changeLog.OldStatusId = entity.FlightStatusID;
                        changeLog.OldRegister = entity.RegisterID;
                        changeLog.OldOffBlock = entity.ChocksOut;
                        changeLog.OldOnBlock = entity.ChocksIn;
                        changeLog.OldTakeOff = entity.Takeoff;
                        changeLog.OldLanding = entity.Landing;
                        changeLog.User = dto.UserName;

                        var oldSTD = (DateTime)entity.STD;



                        var newSTD = new DateTime(oldSTD.Year, oldSTD.Month, oldSTD.Day, stdHours, stdMinutes, 0);
                        var newSTA = newSTD.AddMinutes(duration);
                        // if (oldSTD.AddMinutes(270).Date != newSTD.AddMinutes(270).Date)
                        //     entity.FlightDate = oldSTD.AddDays(utcDiff);
                        ViewModels.FlightDto.FillForGroupUpdate(entity, dto);


                        var _fltDate = new DateTime(oldSTD.Year, oldSTD.Month, oldSTD.Day, 1, 0, 0);


                        //var fltOffset = isUtc ? 0 : -1 * TimeZoneInfo.Local.GetUtcOffset(_fltDate).TotalMinutes;
                        //var _std = new DateTime(oldSTD.Year, oldSTD.Month, oldSTD.Day, (int)dto.STDHH, (int)dto.STDMM, 0);

                        // _std = _std.AddMinutes(fltOffset);
                        //_std = _std.AddDays(_addDay).AddDays(utcDiff).AddMinutes(fltOffset);

                        var _std = newSTD;

                        entity.STD = _std; //newSTD;
                        entity.STA = _std.AddMinutes(duration); //newSTA;
                        entity.ChocksOut = entity.STD;
                        entity.ChocksIn = entity.STA;
                        entity.Takeoff = entity.STD;
                        entity.Landing = entity.STA;
                        entity.BoxId = dto.BoxId;

                        changeLog.NewFlightNumber = entity.FlightNumber;
                        changeLog.NewFromAirportId = entity.FromAirportId;
                        changeLog.NewToAirportId = entity.ToAirportId;
                        changeLog.NewSTD = entity.STD;
                        changeLog.NewSTA = entity.STA;
                        changeLog.NewStatusId = entity.FlightStatusID;
                        changeLog.NewRegister = entity.RegisterID;
                        changeLog.NewOffBlock = entity.ChocksOut;
                        changeLog.NewOnBlock = entity.ChocksIn;
                        changeLog.NewTakeOff = entity.Takeoff;
                        changeLog.NewLanding = entity.Landing;

                        //var state = unitOfWork.context.Entry(entity).State; //= EntityState.Modified;

                    }
                    else if (exec)
                    {
                        entity.ChrCode = dto.ChrCode;
                        entity.ChrTitle = dto.ChrTitle;
                    }


                }



                var saveResult = await _context.SaveChangesAsync();


                //if (dto.SMSNira == 1)
                //{
                //    foreach (var x in flights)
                //        await unitOfWork.FlightRepository.NotifyNira(x.ID, dto.UserName);
                //}

                //bip
                // var flightIds = flights.Select(q => q.ID).ToList();
                var beginDate = ((DateTime)dto.RefDate).Date;
                var endDate = ((DateTime)dto.RefDate).Date.AddDays((int)dto.RefDays).Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                var fg = await _context.ViewFlightsGantts.Where(q => flightIds.Contains(q.ID)
                  && q.STDDay >= beginDate && q.STDDay <= endDate
                ).ToListAsync();


                var odtos = new List<ViewFlightsGanttDto>();
                foreach (var f in fg)
                {
                    ViewModels.ViewFlightsGanttDto odto = new ViewFlightsGanttDto();

                    ViewModels.ViewFlightsGanttDto.FillDto(f, odto, 0, 1);
                    odto.resourceId.Add((int)odto.RegisterID);
                    odtos.Add(odto);
                }

                var resgroups = from x in fg
                                group x by new { x.AircraftType, AircraftTypeId = x.TypeId }
                               into grp
                                select new { groupId = grp.Key.AircraftTypeId, Title = grp.Key.AircraftType };
                var ressq = (from x in fg
                             group x by new { x.RegisterID, x.Register, x.TypeId }
                         into grp

                             orderby getOrderIndex(grp.Key.Register, new List<string>())
                             select new { resourceId = grp.Key.RegisterID, resourceName = grp.Key.Register, groupId = grp.Key.TypeId }).ToList();


                var oflight = odtos.FirstOrDefault(q => q.ID == dto.ID);

                var oresult = new
                {
                    flights = odtos,
                    flight = oflight,
                    resgroups,
                    ressq,
                    messages
                };

                return Ok(/*entity*/oresult);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "  " + ex.InnerException.Message;
                return Ok(msg);
            }
        }

        [Route("api/plan/delete/group")]

        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> DeleteFlightGroup(dynamic dto)
        {
            var _context = new Models.dbEntities();
            int interval = Convert.ToInt32(dto.Interval);
            DateTime intervalFrom = Convert.ToDateTime(dto.IntervalFrom);
            DateTime intervalTo = Convert.ToDateTime(dto.IntervalTo);
            string _days = Convert.ToString(dto.Days);
            var days = _days.Split('_').Select(q => Convert.ToInt32(q)).ToList();
            int checkTime = (int)dto.CheckTime;

            int flightId = Convert.ToInt32(dto.Id);

            //var result = await unitOfWork.FlightRepository.DeleteFlightGroup(intervalFrom, intervalTo, days, flightId, interval, checkTime);
            /////////////////////////////////
            List<int> result = new List<int>();
            var intervalDays = GetInvervalDates(interval, intervalFrom, intervalTo, days).Select(q => (Nullable<DateTime>)q).ToList();
            var baseFlight = await _context.FlightInformations.Where(q => q.ID == flightId).FirstOrDefaultAsync();

            var stdHoursBF = ((DateTime)baseFlight.STD).Hour;
            var stdMinutesBF = ((DateTime)baseFlight.STD).Minute;
            var staHoursBF = ((DateTime)baseFlight.STA).Hour;
            var staMinutesBF = ((DateTime)baseFlight.STA).Minute;

            var flightIds = await (from x in _context.ViewLegTimes
                                   where
                                     x.FlightNumber == baseFlight.FlightNumber
                                     //&&  x.FlightPlanId==baseFlight.FlightGroupID
                                     && intervalDays.Contains(x.STDDay)
                                   select x.ID).ToListAsync();
            var nflts = flightIds.Select(q => (Nullable<int>)q).ToList();

            int check_crew = Convert.ToInt32(ConfigurationManager.AppSettings["check_crew"]);
            List<int?> fdpitems = new List<int?>();
            if (check_crew == 1)
                fdpitems = await _context.FDPItems.Where(q => nflts.Contains(q.FlightId)).Select(q => q.FlightId).ToListAsync();

            var finalIds = nflts.Where(q => !fdpitems.Contains(q)).Select(q => (int)q).Distinct().ToList();
            var finalFlights = await _context.FlightInformations.Where(q => finalIds.Contains(q.ID)).ToListAsync();
            foreach (var entity in finalFlights)
            {
                if (entity.FlightStatusID == 1 || entity.FlightStatusID == 4)
                {
                    var flt_stdHours = ((DateTime)entity.STD).Hour;
                    var flt_stdMinutes = ((DateTime)entity.STD).Minute;
                    var flt_staHours = ((DateTime)entity.STA).Hour;
                    var flt_staMinutes = ((DateTime)entity.STA).Minute;
                    bool exec = true;
                    if (checkTime == 1)
                    {
                        exec = flt_stdHours == stdHoursBF && flt_stdMinutes == stdMinutesBF && flt_staHours == staHoursBF && flt_staMinutes == staMinutesBF;
                    }
                    if (exec)
                    {
                        result.Add(entity.ID);
                        _context.FlightInformations.Remove(entity);
                    }

                }
            }


            ///////////////////////////////////



            var saveResult = await _context.SaveChangesAsync();



            return Ok(result);
        }
        public class ShiftFlightsDto
        {
            public List<int> ids { get; set; }
            public int hour { get; set; }
            public int minute { get; set; }
            public int register { get; set; }
            public string userName { get; set; }
            public int nira { get; set; }
            public int sign { get; set; }
        }

        [Route("api/plan/shift")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostFlightsShift(ShiftFlightsDto dto)
        {

            var _context = new Models.dbEntities();
            var flights = await _context.FlightInformations.Where(q => dto.ids.Contains(q.ID)).ToListAsync();
            var legs = await _context.ViewLegTimes.Where(q => dto.ids.Contains(q.ID)).ToListAsync();
            foreach (var flight in flights)
            {
                var leg = legs.FirstOrDefault(q => q.ID == flight.ID);
                if (leg != null && leg.FlightStatusID == 1)
                {
                    var changeLog = new FlightChangeHistory()
                    {
                        Date = DateTime.Now,
                        FlightId = flight.ID,

                    };
                    changeLog.User = dto.userName + " " + (dto.nira == 1 ? "1" : "0");
                    changeLog.OldFlightNumer = leg.FlightNumber;
                    changeLog.OldFromAirportId = leg.FromAirport;
                    changeLog.OldToAirportId = leg.ToAirport;
                    changeLog.OldSTD = leg.STD;
                    changeLog.OldSTA = leg.STA;
                    changeLog.OldStatusId = leg.FlightStatusID;
                    changeLog.OldRegister = leg.RegisterID;
                    changeLog.OldOffBlock = leg.ChocksOut;
                    changeLog.OldOnBlock = leg.ChocksIn;
                    changeLog.OldTakeOff = leg.Takeoff;
                    changeLog.OldLanding = leg.Landing;

                    var mm = (dto.sign) * (dto.hour * 60 + dto.minute);
                    var dtoSTD = ((DateTime)flight.STD).AddMinutes(mm);
                    var dtoSTA = ((DateTime)flight.STA).AddMinutes(mm);
                    var tzoffset = Helper.GetTimeOffset((DateTime)dtoSTD);
                    var oldSTD = ((DateTime)flight.STD).AddMinutes(tzoffset).Date;
                    var newSTD = ((DateTime)dtoSTD).AddMinutes(tzoffset).Date;
                    if (oldSTD != newSTD)
                    {
                        flight.FlightDate = oldSTD;
                    }
                    flight.STD = dtoSTD;
                    flight.STA = dtoSTA;
                    flight.ChocksIn = dtoSTA;
                    flight.ChocksOut = dtoSTD;
                    flight.Takeoff = dtoSTD;
                    flight.Landing = dtoSTA;
                    changeLog.NewFlightNumber = flight.FlightNumber;
                    changeLog.NewFromAirportId = flight.FromAirportId;
                    changeLog.NewToAirportId = flight.ToAirportId;
                    changeLog.NewSTD = flight.STD;
                    changeLog.NewSTA = flight.STA;
                    changeLog.NewStatusId = flight.FlightStatusID;
                    changeLog.NewRegister = flight.RegisterID;
                    changeLog.NewOffBlock = flight.ChocksOut;
                    changeLog.NewOnBlock = flight.ChocksIn;
                    changeLog.NewTakeOff = flight.Takeoff;
                    changeLog.NewLanding = flight.Landing;

                    _context.FlightChangeHistories.Add(changeLog);


                }

            }

            var saveResult = await _context.SaveChangesAsync();
            //////////////////
            /////////////////


            if (dto.nira == 1)
            {
                // foreach (var id in dto.ids)
                //    await unitOfWork.FlightRepository.NotifyNira(id, obj.userName);
            }

            /////////////////////
            // var fg = await unitOfWork.FlightRepository.GetViewFlightGantts().Where(q => obj.ids.Contains(q.ID)).ToListAsync();
            var fg = await _context.ViewFlightsGantts.Where(q => dto.ids.Contains(q.ID)).ToListAsync();
            var xflights = new List<ViewFlightsGanttDto>();
            foreach (var x in fg)
            {
                ViewModels.ViewFlightsGanttDto odto = new ViewFlightsGanttDto();
                ViewModels.ViewFlightsGanttDto.FillDto(x, odto, 0, 1);
                xflights.Add(odto);
            }



            var resgroups = from x in fg
                            group x by new { x.AircraftType, AircraftTypeId = x.TypeId }
                               into grp
                            select new { groupId = grp.Key.AircraftTypeId, Title = grp.Key.AircraftType };
            var ressq = (from x in fg
                         group x by new { x.RegisterID, x.Register, x.TypeId }
                     into grp

                         orderby getOrderIndex(grp.Key.Register, new List<string>())
                         select new { resourceId = grp.Key.RegisterID, resourceName = grp.Key.Register, groupId = grp.Key.TypeId }).ToList();

            //odto.resourceId.Add((int)odto.RegisterID);


            var oresult = new
            {
                flights = xflights,
                resgroups,
                ressq
            };
            //////////////////////
            return Ok(oresult);

        }


        public class cnlregs
        {
            public List<int> fids { get; set; }
            public int reason { get; set; }
            public string userName { get; set; }
            public int? userId { get; set; }
            public string remark { get; set; }
            public int? reg { get; set; }
            public DateTime? intervalFrom { get; set; }
            public DateTime? intervalTo { get; set; }
            public List<int> days { get; set; }
            public int? interval { get; set; }
            public DateTime? RefDate { get; set; }
            public int? RefDays { get; set; }
        }
        public class updateLogResult
        {
            public bool sendNira { get; set; }
            public int flight { get; set; }
            public List<int?> offIds { get; set; }
            public List<int> fltIds { get; set; }
            public List<offcrew> offcrews { get; set; }
        }
        public class offcrew
        {
            public int? flightId { get; set; }
            public List<int?> crews { get; set; }
        }

        [Route("api/plan/active/groupx")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostFlightActiveX(cnlregs dto)
        {
            var _context = new Models.dbEntities();


            ///////////////////////////////////
            //var result = await unitOfWork.FlightRepository.ActiveFlights(dto);
            var flights = await _context.FlightInformations.Where(q => dto.fids.Contains(q.ID)).ToListAsync();
            var legs = await _context.ViewLegTimes.Where(q => dto.fids.Contains(q.ID)).ToListAsync();
            foreach (var fid in dto.fids)
            {
                var flight = flights.FirstOrDefault(q => q.ID == fid);
                var leg = legs.FirstOrDefault(q => q.ID == fid);

                var changeLog = new FlightChangeHistory()
                {
                    Date = DateTime.Now,
                    FlightId = flight.ID,
                    User = dto.userName,

                };
                changeLog.OldFlightNumer = leg.FlightNumber;
                changeLog.OldFromAirportId = leg.FromAirport;
                changeLog.OldToAirportId = leg.ToAirport;
                changeLog.OldSTD = flight.STD;
                changeLog.OldSTA = flight.STA;
                changeLog.OldStatusId = flight.FlightStatusID;
                changeLog.OldRegister = leg.RegisterID;
                changeLog.OldOffBlock = flight.ChocksOut;
                changeLog.OldOnBlock = flight.ChocksIn;
                changeLog.OldTakeOff = flight.Takeoff;
                changeLog.OldLanding = flight.Landing;

                //////////////////////////////////////////////////////////////

                flight.DateCreate = DateTime.Now.ToUniversalTime();
                flight.FlightStatusUserId = dto.userId;


                flight.FlightStatusID = 1;

                //var cnlMsn = await this.context.Ac_MSN.Where(q => q.Register == "CNL").Select(q => q.ID).FirstOrDefaultAsync();
                flight.RegisterID = dto.reg;
                flight.CancelDate = null;
                flight.CancelReasonId = null;
                flight.DepartureRemark += (!string.IsNullOrEmpty(flight.DepartureRemark) ? "\r\n" : "") + dto.remark + "(ACTV REMARK BY:" + dto.userName + ")";
                //2020-11-24




                if (flight.FlightStatusID != null && /*dto.UserId != null*/ !string.IsNullOrEmpty(dto.userName))
                    _context.FlightStatusLogs.Add(new FlightStatusLog()
                    {
                        FlightID = flight.ID,

                        Date = DateTime.Now.ToUniversalTime(),
                        FlightStatusID = (int)flight.FlightStatusID,

                        UserId = dto.userId != null ? (int)dto.userId : 128000,
                        Remark = dto.userName,
                    });

                //kak4

                ////////////////////////////////////////
                changeLog.NewFlightNumber = leg.FlightNumber;
                changeLog.NewFromAirportId = leg.FromAirport;
                changeLog.NewToAirportId = flight.ToAirportId;
                changeLog.NewSTD = flight.STD;
                changeLog.NewSTA = flight.STA;
                changeLog.NewStatusId = flight.FlightStatusID;
                changeLog.NewRegister = leg.RegisterID;
                changeLog.NewOffBlock = flight.ChocksOut;
                changeLog.NewOnBlock = flight.ChocksIn;
                changeLog.NewTakeOff = flight.Takeoff;
                changeLog.NewLanding = flight.Landing;

                _context.FlightChangeHistories.Add(changeLog);
            }




            bool sendNira = true;
            var nullfids = dto.fids.Select(q => (Nullable<int>)q).ToList();




            var result = new updateLogResult()
            {
                sendNira = sendNira,
                flight = -1, //flight.ID,
                             //offIds = offCrewIds
                offIds = nullfids


            };



            //////////////////////////////////////////////////////




            var saveResult = await _context.SaveChangesAsync();


            var fresult = result as updateLogResult;
            //if (fresult.sendNira)
            //{
            //    foreach (var x in fresult.offIds)
            //        await unitOfWork.FlightRepository.NotifyNira((int)x, dto.userName);
            //}



            var fg = await _context.ViewFlightsGantts.Where(q => dto.fids.Contains(q.ID)).ToListAsync();
            var xflights = new List<ViewFlightsGanttDto>();
            foreach (var x in fg)
            {
                ViewModels.ViewFlightsGanttDto odto = new ViewFlightsGanttDto();
                ViewModels.ViewFlightsGanttDto.FillDto(x, odto, 0, 1);
                xflights.Add(odto);
            }



            var resgroups = from x in fg
                            group x by new { x.AircraftType, AircraftTypeId = x.TypeId }
                           into grp
                            select new { groupId = grp.Key.AircraftTypeId, Title = grp.Key.AircraftType };
            var ressq = (from x in fg
                         group x by new { x.RegisterID, x.Register, x.TypeId }
                     into grp

                         orderby getOrderIndex(grp.Key.Register, new List<string>())
                         select new { resourceId = grp.Key.RegisterID, resourceName = grp.Key.Register, groupId = grp.Key.TypeId }).ToList();

            //odto.resourceId.Add((int)odto.RegisterID);


            var oresult = new
            {
                flights = xflights,
                resgroups,
                ressq
            };
            //////////////////////
            return Ok(oresult);
        }


        [Route("api/plan/active/group")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostFlightActive(cnlregs dto)
        {
            var _context = new Models.dbEntities();


            var intervalDays = GetInvervalDates((int)dto.interval, (DateTime)dto.intervalFrom, (DateTime)dto.intervalTo, dto.days).Select(q => (Nullable<DateTime>)q).ToList();
            var baseFlights = await _context.FlightInformations.Where(q => dto.fids.Contains(q.ID)).ToListAsync();
            var fltNumbers = baseFlights.Select(q => q.FlightNumber).ToList();
            var fltIds = new List<int>();
            fltIds = baseFlights.Select(q => q.ID).ToList();
            var groupIds = baseFlights.Select(q => q.FlightGroupID).ToList();

            var _flightIds = await (from x in _context.ViewLegTimes
                                    where fltNumbers.Contains(x.FlightNumber) && groupIds.Contains(x.FlightPlanId) && intervalDays.Contains(x.STDDay)
                                    select x.ID).ToListAsync();
            fltIds = fltIds.Concat(_flightIds).Distinct().ToList();

            var flights = await _context.FlightInformations.Where(q => fltIds.Contains(q.ID)).ToListAsync();
            var legs = await _context.ViewLegTimes.Where(q => fltIds.Contains(q.ID)).ToListAsync();
            /////////////////////////////////
            //var flights = await this.context.FlightInformations.Where(q => dto.fids.Contains(q.ID)).ToListAsync();
            // var legs = await this.context.ViewLegTimes.Where(q => dto.fids.Contains(q.ID)).ToListAsync();
            foreach (var fid in fltIds)
            {
                var flight = flights.FirstOrDefault(q => q.ID == fid);
                var leg = legs.FirstOrDefault(q => q.ID == fid);

                var changeLog = new FlightChangeHistory()
                {
                    Date = DateTime.Now,
                    FlightId = flight.ID,
                    User = dto.userName,

                };
                changeLog.OldFlightNumer = leg.FlightNumber;
                changeLog.OldFromAirportId = leg.FromAirport;
                changeLog.OldToAirportId = leg.ToAirport;
                changeLog.OldSTD = flight.STD;
                changeLog.OldSTA = flight.STA;
                changeLog.OldStatusId = flight.FlightStatusID;
                changeLog.OldRegister = leg.RegisterID;
                changeLog.OldOffBlock = flight.ChocksOut;
                changeLog.OldOnBlock = flight.ChocksIn;
                changeLog.OldTakeOff = flight.Takeoff;
                changeLog.OldLanding = flight.Landing;

                //////////////////////////////////////////////////////////////

                flight.DateCreate = DateTime.Now.ToUniversalTime();
                flight.FlightStatusUserId = dto.userId;


                flight.FlightStatusID = 1;

                //var cnlMsn = await this.context.Ac_MSN.Where(q => q.Register == "CNL").Select(q => q.ID).FirstOrDefaultAsync();
                flight.RegisterID = dto.reg;
                flight.CancelDate = null;
                flight.CancelReasonId = null;
                flight.DepartureRemark += (!string.IsNullOrEmpty(flight.DepartureRemark) ? "\r\n" : "") + dto.remark + "(ACTV REMARK BY:" + dto.userName + ")";
                //2020-11-24




                if (flight.FlightStatusID != null && /*dto.UserId != null*/ !string.IsNullOrEmpty(dto.userName))
                    _context.FlightStatusLogs.Add(new FlightStatusLog()
                    {
                        FlightID = flight.ID,

                        Date = DateTime.Now.ToUniversalTime(),
                        FlightStatusID = (int)flight.FlightStatusID,

                        UserId = dto.userId != null ? (int)dto.userId : 128000,
                        Remark = dto.userName,
                    });

                //kak4

                ////////////////////////////////////////
                changeLog.NewFlightNumber = leg.FlightNumber;
                changeLog.NewFromAirportId = leg.FromAirport;
                changeLog.NewToAirportId = flight.ToAirportId;
                changeLog.NewSTD = flight.STD;
                changeLog.NewSTA = flight.STA;
                changeLog.NewStatusId = flight.FlightStatusID;
                changeLog.NewRegister = leg.RegisterID;
                changeLog.NewOffBlock = flight.ChocksOut;
                changeLog.NewOnBlock = flight.ChocksIn;
                changeLog.NewTakeOff = flight.Takeoff;
                changeLog.NewLanding = flight.Landing;

                _context.FlightChangeHistories.Add(changeLog);
            }




            bool sendNira = true;
            //var nullfids = dto.fids.Select(q => (Nullable<int>)q).ToList();

            var nullfids = fltIds.Select(q => (Nullable<int>)q).ToList();




            var result = new updateLogResult()
            {
                sendNira = sendNira,
                flight = -1, //flight.ID,
                             //offIds = offCrewIds
                offIds = nullfids,
                fltIds = fltIds

            };



            //////////////////////////////////////////////////////




            var saveResult = await _context.SaveChangesAsync();


            var fresult = result as updateLogResult;
            //if (fresult.sendNira)
            //{
            //    foreach (var x in fresult.offIds)
            //        await unitOfWork.FlightRepository.NotifyNira((int)x, dto.userName);
            //}



            // var fg = await _context.ViewFlightsGantts.Where(q => dto.fids.Contains(q.ID)).ToListAsync();
            var beginDate = ((DateTime)dto.RefDate).Date;
            var endDate = ((DateTime)dto.RefDate).Date.AddDays((int)dto.RefDays).Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            var fg = await _context.ViewFlightsGantts.Where(q => fresult.fltIds.Contains(q.ID)
             && q.STDDay >= beginDate && q.STDDay <= endDate
            ).ToListAsync();
            var xflights = new List<ViewFlightsGanttDto>();
            foreach (var x in fg)
            {
                ViewModels.ViewFlightsGanttDto odto = new ViewFlightsGanttDto();
                ViewModels.ViewFlightsGanttDto.FillDto(x, odto, 0, 1);
                xflights.Add(odto);
            }



            var resgroups = from x in fg
                            group x by new { x.AircraftType, AircraftTypeId = x.TypeId }
                           into grp
                            select new { groupId = grp.Key.AircraftTypeId, Title = grp.Key.AircraftType };
            var ressq = (from x in fg
                         group x by new { x.RegisterID, x.Register, x.TypeId }
                     into grp

                         orderby getOrderIndex(grp.Key.Register, new List<string>())
                         select new { resourceId = grp.Key.RegisterID, resourceName = grp.Key.Register, groupId = grp.Key.TypeId }).ToList();

            //odto.resourceId.Add((int)odto.RegisterID);


            var oresult = new
            {
                flights = xflights,
                resgroups,
                ressq
            };
            //////////////////////
            return Ok(oresult);
        }



        [Route("api/plan/cancel/group")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostFlightCancelGroup(cnlregs dto)
        {
            var _context = new Models.dbEntities();

            ///////////
            ///////////
            var intervalDays = GetInvervalDates((int)dto.interval, (DateTime)dto.intervalFrom, (DateTime)dto.intervalTo, dto.days).Select(q => (Nullable<DateTime>)q).ToList();
            var baseFlights = await _context.FlightInformations.Where(q => dto.fids.Contains(q.ID)).ToListAsync();
            var fltNumbers = baseFlights.Select(q => q.FlightNumber).ToList();
            var groupIds = baseFlights.Select(q => q.FlightGroupID).ToList();
            var fltIds = new List<int>();
            fltIds = baseFlights.Select(q => q.ID).ToList();

            var _flightIds = await (from x in _context.ViewLegTimes
                                    where fltNumbers.Contains(x.FlightNumber) && groupIds.Contains(x.FlightPlanId) && intervalDays.Contains(x.STDDay)
                                    select x.ID).ToListAsync();
            fltIds = fltIds.Concat(_flightIds).Distinct().ToList();

            var flights = await _context.FlightInformations.Where(q => fltIds.Contains(q.ID)).ToListAsync();
            var legs = await _context.ViewLegTimes.Where(q => fltIds.Contains(q.ID)).ToListAsync();
            foreach (var fid in fltIds)
            {
                var flight = flights.FirstOrDefault(q => q.ID == fid);
                var leg = legs.FirstOrDefault(q => q.ID == fid);

                var changeLog = new FlightChangeHistory()
                {
                    Date = DateTime.Now,
                    FlightId = flight.ID,
                    User = dto.userName,

                };
                changeLog.OldFlightNumer = leg.FlightNumber;
                changeLog.OldFromAirportId = leg.FromAirport;
                changeLog.OldToAirportId = leg.ToAirport;
                changeLog.OldSTD = flight.STD;
                changeLog.OldSTA = flight.STA;
                changeLog.OldStatusId = flight.FlightStatusID;
                changeLog.OldRegister = leg.RegisterID;
                changeLog.OldOffBlock = flight.ChocksOut;
                changeLog.OldOnBlock = flight.ChocksIn;
                changeLog.OldTakeOff = flight.Takeoff;
                changeLog.OldLanding = flight.Landing;

                //////////////////////////////////////////////////////////////

                flight.DateCreate = DateTime.Now.ToUniversalTime();
                flight.FlightStatusUserId = dto.userId;


                flight.FlightStatusID = 4;

                var cnlMsn = 84; //await _context .Ac_MSN.Where(q => q.Register == "CNL").Select(q => q.ID).FirstOrDefaultAsync();
                // flight.JLBLHH = flight.RegisterID;
                flight.CPRegister = leg.Register;
                flight.RegisterID = cnlMsn;
                flight.CancelDate = DateTime.Now;
                flight.CancelReasonId = dto.reason;
                flight.DepartureRemark += (!string.IsNullOrEmpty(flight.DepartureRemark) ? "\r\n" : "") + dto.remark + "(CNL REMARK BY:" + dto.userName + ")";
                //2020-11-24




                if (flight.FlightStatusID != null && /*dto.UserId != null*/ !string.IsNullOrEmpty(dto.userName))
                    _context.FlightStatusLogs.Add(new FlightStatusLog()
                    {
                        FlightID = flight.ID,

                        Date = DateTime.Now.ToUniversalTime(),
                        FlightStatusID = (int)flight.FlightStatusID,

                        UserId = dto.userId != null ? (int)dto.userId : 128000,
                        Remark = dto.userName,
                    });


                UpdateFirstLastFlights(flight.ID, _context);



                ////////////////////////////////////////
                changeLog.NewFlightNumber = leg.FlightNumber;
                changeLog.NewFromAirportId = leg.FromAirport;
                changeLog.NewToAirportId = flight.ToAirportId;
                changeLog.NewSTD = flight.STD;
                changeLog.NewSTA = flight.STA;
                changeLog.NewStatusId = flight.FlightStatusID;
                changeLog.NewRegister = leg.RegisterID;
                changeLog.NewOffBlock = flight.ChocksOut;
                changeLog.NewOnBlock = flight.ChocksIn;
                changeLog.NewTakeOff = flight.Takeoff;
                changeLog.NewLanding = flight.Landing;

                _context.FlightChangeHistories.Add(changeLog);
            }




            bool sendNira = false;
            var nullfids = dto.fids.Select(q => (Nullable<int>)q).ToList();

            var offCrewIds = (from q in _context.ViewFlightCrewNews
                              where nullfids.Contains(q.FlightId)
                              group q by q.FlightId into grp
                              select new offcrew() { flightId = grp.Key, crews = grp.Select(w => w.CrewId).ToList() }

                             ).ToList();
            var result = new updateLogResult()
            {
                sendNira = sendNira,
                flight = -1, //flight.ID,
                //offIds = offCrewIds
                offcrews = offCrewIds,
                fltIds = fltIds

            };

            /////
            ///********************
            ///*******************
            // var result = await unitOfWork.FlightRepository.CancelFlightsGroup(dto);

            ////////////////////
            ///////////////////
            ///




            var saveResult = await _context.SaveChangesAsync();



            var fresult = result as updateLogResult;
            if (fresult.offcrews != null && fresult.offcrews.Count > 0)
            {
                foreach (var rec in fresult.offcrews)
                {
                    foreach (var crewid in rec.crews)
                    {
                        await RemoveItemsFromFDP(rec.flightId.ToString(), (int)crewid, 2, "Flight Cancellation - Removed by AirPocket.", 0, 0);
                    }
                }

            }


            var beginDate = ((DateTime)dto.RefDate).Date;
            var endDate = ((DateTime)dto.RefDate).Date.AddDays((int)dto.RefDays).Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            var fg = await _context.ViewFlightsGantts.Where(q => fresult.fltIds.Contains(q.ID)
             && q.STDDay >= beginDate && q.STDDay <= endDate
            ).ToListAsync();
            var xflights = new List<ViewFlightsGanttDto>();
            foreach (var x in fg)
            {
                ViewModels.ViewFlightsGanttDto odto = new ViewFlightsGanttDto();
                ViewModels.ViewFlightsGanttDto.FillDto(x, odto, 0, 1);
                xflights.Add(odto);
            }



            var resgroups = from x in fg
                            group x by new { x.AircraftType, AircraftTypeId = x.TypeId }
                           into grp
                            select new { groupId = grp.Key.AircraftTypeId, Title = grp.Key.AircraftType };
            var ressq = (from x in fg
                         group x by new { x.RegisterID, x.Register, x.TypeId }
                     into grp

                         orderby getOrderIndex(grp.Key.Register, new List<string>())
                         select new { resourceId = grp.Key.RegisterID, resourceName = grp.Key.Register, groupId = grp.Key.TypeId }).ToList();

            //odto.resourceId.Add((int)odto.RegisterID);


            var oresult = new
            {
                flights = xflights,
                resgroups,
                ressq
            };
            //////////////////////
            return Ok(oresult);

        }


        internal async Task<IHttpActionResult> RemoveItemsFromFDP(string strItems, int crewId, int reason, string remark, int notify, int noflight, string username = "")
        {
            var context = new Models.dbEntities();
            double default_reporting = 75;

            var flightIds = strItems.Split('*').Select(q => (Nullable<int>)Convert.ToInt32(q)).Distinct().ToList();
            var allRemovedItems = await (from x in context.FDPItems
                                         join y in context.FlightInformations on x.FlightId equals y.ID
                                         where flightIds.Contains(x.FlightId)
                                         orderby y.ChocksOut
                                         select x).ToListAsync();
            var _fdpItemIds = allRemovedItems.Select(q => q.Id).ToList();
            // var _fdpItemIds = await  context.ViewFDPItem2.Where(q => q.CrewId == crewId && flightIds.Contains(q.FlightId)).OrderBy(q => q.STD).Select(q => q.Id).ToListAsync();



            //var allRemovedItems = await  context.FDPItems.Where(q => _fdpItemIds.Contains(q.Id)).ToListAsync();
            var _fdpIds = allRemovedItems.Select(q => q.FDPId).ToList();
            var fdps = await context.FDPs.Where(q => _fdpIds.Contains(q.Id)).ToListAsync();
            var fdpItems = await context.FDPItems.Where(q => _fdpIds.Contains(q.FDPId)).ToListAsync();


            var allFlightIds = fdpItems.Select(q => q.FlightId).ToList();
            //var allFlights = await context.ViewLegTimes.Where(q => allFlightIds.Contains(q.ID)).OrderBy(q => q.STD).ToListAsync();
            var allFlights = await context.SchFlights.Where(q => allFlightIds.Contains(q.ID)).OrderBy(q => q.STD).ToListAsync();
            var crews = await context.ViewEmployeeLights.Where(q => q.Id == crewId).ToListAsync();
            var allRemovedFlights = allFlights.Where(q => flightIds.Contains(q.ID)).OrderBy(q => q.STD).ToList();
            FDP offFDP = null;
            string offSMS = string.Empty;
            List<string> sms = new List<string>();
            List<string> nos = new List<string>();
            List<CrewPickupSM> csms = new List<CrewPickupSM>();
            if (reason != -1)
            {


                offFDP = new FDP()
                {
                    CrewId = crewId,
                    DateStart = allRemovedFlights.First().STD,
                    DateEnd = allRemovedFlights.Last().STA,
                    InitStart = allRemovedFlights.First().STD,
                    InitEnd = allRemovedFlights.Last().STA,

                    InitRestTo = allRemovedFlights.Last().STA,
                    InitKey = allRemovedFlights.First().ID.ToString(),
                    DutyType = 0,
                    GUID = Guid.NewGuid(),
                    IsTemplate = false,
                    Remark = remark,
                    UPD = 1,
                    UserName = username


                };
                offFDP.CanceledNo = string.Join(",", allRemovedFlights.Select(q => q.FlightNumber));
                offFDP.CanceledRoute = string.Join(",", allRemovedFlights.Select(q => q.FromAirportIATA)) + "," + allRemovedFlights.Last().ToAirportIATA;
                switch (reason)
                {
                    case 1:
                        offFDP.DutyType = 100009;
                        offFDP.Remark2 = "Refused by Crew";
                        break;
                    case 5:
                        offFDP.DutyType = 100020;
                        offFDP.Remark2 = "Cenceled due to Rescheduling";
                        break;
                    case 2:
                        offFDP.DutyType = 100021;
                        offFDP.Remark2 = "Cenceled due to Flight(s) Cancellation";
                        break;
                    case 3:
                        offFDP.DutyType = 100022;
                        offFDP.Remark2 = "Cenceled due to Change of A/C Type";
                        break;
                    case 4:
                        offFDP.DutyType = 100023;
                        offFDP.Remark2 = "Cenceled due to Flight/Duty Limitations";
                        break;
                    case 6:
                        offFDP.DutyType = 100024;
                        offFDP.Remark2 = "Cenceled due to Not using Split Duty";
                        break;


                    case 7:
                        offFDP.DutyType = 200000;
                        offFDP.Remark2 = "Refused-Not Home";
                        break;
                    case 8:
                        offFDP.DutyType = 200001;
                        offFDP.Remark2 = "Refused-Family Problem";
                        break;
                    case 9:
                        offFDP.DutyType = 200002;
                        offFDP.Remark2 = "Canceled - Training";
                        break;
                    case 10:
                        offFDP.DutyType = 200003;
                        offFDP.Remark2 = "Ground - Operation";
                        break;
                    case 11:
                        offFDP.DutyType = 200004;
                        offFDP.Remark2 = "Ground - Expired License";
                        break;
                    case 12:
                        offFDP.DutyType = 200005;
                        offFDP.Remark2 = "Ground - Medical";
                        break;
                    case 13:
                        offFDP.DutyType = 200005;
                        offFDP.Remark2 = "Refused - Not Acceptable";
                        break;
                    case 14:
                        offFDP.DutyType = 200005;
                        offFDP.Remark2 = "Refused - Acceptable";
                        break;
                    default:
                        break;
                }
                foreach (var x in allRemovedFlights)
                {
                    var _ofdpitem = fdpItems.FirstOrDefault(q => q.FlightId == x.ID);
                    string _oremark = string.Empty;
                    if (_ofdpitem != null)
                    {
                        var _ofdp = fdps.Where(q => q.Id == _ofdpitem.FDPId).FirstOrDefault();
                        if (_ofdp != null)
                            _oremark = _ofdp.InitRank;
                    }
                    offFDP.OffItems.Add(new OffItem() { FDP = offFDP, FlightId = x.ID, Remark = _oremark });
                }

                context.FDPs.Add(offFDP);



                var strs = new List<string>();
                strs.Add(ConfigurationManager.AppSettings["airline"] + " Airlines");
                strs.Add("Dear " + crews.FirstOrDefault(q => q.Id == crewId).Name + ", ");
                strs.Add("Canceling Notification");
                var day = ((DateTime)allRemovedFlights.First().STDLocal).Date;
                var dayStr = day.ToString("ddd") + " " + day.Year + "-" + day.Month + "-" + day.Day;
                strs.Add(dayStr);
                strs.Add(offFDP.CanceledNo);
                strs.Add(offFDP.CanceledRoute);
                strs.Add(offFDP.Remark2);
                strs.Add(remark);
                strs.Add("Date sent: " + DateTime.Now.ToLocalTime().ToString("yyyy/MM/dd HH:mm"));
                strs.Add("Crew Scheduling Department");
                offSMS = String.Join("\n", strs);
                sms.Add(offSMS);
                nos.Add(crews.FirstOrDefault(q => q.Id == crewId).Mobile);

                var csm = new CrewPickupSM()
                {
                    CrewId = (int)crewId,
                    DateSent = DateTime.Now,
                    DateStatus = DateTime.Now,
                    FlightId = -1,
                    Message = offSMS,
                    Pickup = null,
                    RefId = "",
                    Status = "",
                    Type = offFDP.DutyType,
                    FDP = offFDP,
                    DutyType = offFDP.Remark2,
                    DutyDate = ((DateTime)offFDP.InitStart).ToLocalTime().Date,
                    Flts = offFDP.CanceledNo,
                    Routes = offFDP.CanceledRoute
                };
                csms.Add(csm);
                if (notify == 1)
                    context.CrewPickupSMS.Add(csm);
            }

            /////////////////////////
            //var fdps = await this.context.FDPs.Where(q => _fdpIds.Contains(q.Id)).ToListAsync();
            //var fdpIds = fdps.Select(q => q.Id).ToList();
            //var crewIds = fdps.Select(q => q.CrewId).ToList();
            //var fdpItems = await this.context.FDPItems.Where(q => fdpIds.Contains(q.FDPId)).ToListAsync();
            //var allFlightIds = fdpItems.Select(q => q.FlightId).ToList();
            //var allFlights = await this.context.ViewLegTimes.Where(q => allFlightIds.Contains(q.ID)).ToListAsync();
            //var crews = await this.context.ViewEmployeeLights.Where(q => crewIds.Contains(q.Id)).ToListAsync();
            //var allRemovedItems = fdpItems.Where(q => _fdpItemIds.Contains(q.Id)).ToList();
            //////////////////////////
            //////////////////////////
            ///////////////////

            foreach (var x in allRemovedItems)
            {
                var xfdp = fdps.FirstOrDefault(q => q.Id == x.FDPId);
                var xcrew = crews.FirstOrDefault(q => q.Id == xfdp.CrewId);
                var xleg = allFlights.FirstOrDefault(q => q.ID == x.FlightId);


            }

            var updatedIds = new List<int>();
            var updated = new List<FDP>();
            var removed = new List<int>();


            //  List<FDP> deleted = new List<FDP>();
            foreach (var fdp in fdps)
            {
                fdp.Split = 0;
                var allitems = fdpItems.Where(q => q.FDPId == fdp.Id).ToList();
                var removedItems = allitems.Where(q => _fdpItemIds.Contains(q.Id)).ToList();
                var remainItems = allitems.Where(q => !_fdpItemIds.Contains(q.Id)).ToList();
                var remainFlightIds = remainItems.Select(q => q.FlightId).ToList();
                if (allitems.Count == removedItems.Count)
                {
                    removed.Add(fdp.Id);
                    context.FDPItems.RemoveRange(removedItems);

                    context.FDPs.Remove(fdp);
                    context.Database.ExecuteSqlCommand("Delete from TableDutyFDP where FDPId=" + fdp.Id);
                    context.Database.ExecuteSqlCommand("Delete from TableFlightFDP where FDPId=" + fdp.Id);
                }
                else
                {
                    //Update FDP

                    context.FDPItems.RemoveRange(removedItems);
                    var items = (from x in remainItems
                                 join y in allFlights on x.FlightId equals y.ID
                                 orderby y.STD
                                 select new { fi = x, flt = y }).ToList();
                    fdp.FirstFlightId = items.First().flt.ID;
                    fdp.LastFlightId = items.Last().flt.ID;
                    fdp.InitStart = ((DateTime)items.First().flt.ChocksOut).AddMinutes(-default_reporting);
                    fdp.InitEnd = ((DateTime)items.Last().flt.ChocksIn).AddMinutes(30);

                    fdp.DateStart = ((DateTime)items.First().flt.ChocksOut).AddMinutes(-default_reporting);
                    fdp.DateEnd = ((DateTime)items.Last().flt.ChocksIn).AddMinutes(30);

                    var rst = 12;
                    if (fdp.InitHomeBase != null && fdp.InitHomeBase != items.Last().flt.ToAirportId)
                        rst = 10;
                    fdp.InitRestTo = ((DateTime)items.Last().flt.ChocksIn).AddMinutes(30).AddHours(rst);
                    fdp.InitFlts = string.Join(",", items.Select(q => q.flt).Select(q => q.FlightNumber).ToList());
                    fdp.InitRoute = string.Join(",", items.Select(q => q.flt).Select(q => q.FromAirportIATA).ToList());
                    fdp.InitRoute += "," + items.Last().flt.ToAirportIATA;
                    fdp.InitFromIATA = items.First().flt.FromAirportId.ToString();
                    fdp.InitToIATA = items.Last().flt.ToAirportId.ToString();
                    fdp.InitNo = string.Join("-", items.Select(q => q.flt).Select(q => q.FlightNumber).ToList());
                    fdp.InitKey = string.Join("-", items.Select(q => q.flt).Select(q => q.ID).ToList());
                    fdp.InitFlights = string.Join("*", items.Select(q => q.flt.ID + "_" + (q.fi.IsPositioning == true ? "1" : "0") + "_" + ((DateTime)q.flt.ChocksOutLocal).ToString("yyyyMMddHHmm")
                      + "_" + ((DateTime)q.flt.ChocksInLocal).ToString("yyyyMMddHHmm")
                      + "_" + q.flt.FlightNumber + "_" + q.flt.FromAirportIATA + "_" + q.flt.ToAirportIATA).ToList()
                    );

                    var keyParts = new List<string>();
                    keyParts.Add(items[0].flt.ID + "*" + (items[0].fi.IsPositioning == true ? "1" : "0"));
                    var breakGreaterThan10Hours = string.Empty;
                    for (int i = 1; i < items.Count; i++)
                    {

                        keyParts.Add(items[i].flt.ID + "*" + (items[i].fi.IsPositioning == true ? "1" : "0"));
                        var dt = (DateTime)items[i].flt.ChocksOut - (DateTime)items[i - 1].flt.ChocksIn;
                        var minuts = dt.TotalMinutes;
                        // – (0:30 + 0:15 + 0:45)
                        var brk = minuts - 30 - 60; //30:travel time, post flight duty:15, pre flight duty:30
                        if (brk >= 600)
                        {
                            //var tfi = tflights.FirstOrDefault(q => q.ID == flights[i].ID);
                            // var tfi1 = tflights.FirstOrDefault(q => q.ID == flights[i - 1].ID);
                            breakGreaterThan10Hours = "The break is greater than 10 hours.";
                        }
                        else
                        if (brk >= 180)
                        {
                            var xfdpitem = allitems.FirstOrDefault(q => q.Id == items[i].fi.Id);
                            xfdpitem.SplitDuty = true;
                            var pair = allitems.FirstOrDefault(q => q.Id == items[i - 1].fi.Id);
                            pair.SplitDuty = true;
                            xfdpitem.SplitDutyPairId = pair.FlightId;
                            fdp.Split += 0.5 * (brk);

                        }

                    }
                    fdp.UPD = fdp.UPD == null ? 1 : ((int)fdp.UPD) + 1;
                    fdp.Key = string.Join("_", keyParts);
                    fdp.UserName = username;
                    //var flights = allFlights.Where(q => remainFlightIds.Contains(q.ID)).OrderBy(q=>q.STD).ToList();
                    updatedIds.Add(fdp.Id);
                    updated.Add(fdp);

                }
            }





            var saveResult = await context.SaveChangesAsync();
            //if (saveResult.Code != HttpStatusCode.OK)
            //    return saveResult;

            var fdpsIds = fdps.Select(q => q.Id).ToList();
            var maxfdps = await context.HelperMaxFDPs.Where(q => fdpsIds.Contains(q.Id)).ToListAsync();
            var fdpExtras = await context.FDPExtras.Where(q => fdpsIds.Contains(q.FDPId)).ToListAsync();
            context.FDPExtras.RemoveRange(fdpExtras);
            foreach (var x in maxfdps)
            {
                context.FDPExtras.Add(new FDPExtra()
                {
                    FDPId = x.Id,
                    MaxFDP = Convert.ToDecimal(x.MaxFDPExtended),
                });
            }
            saveResult = await context.SaveChangesAsync();

            /////TableFlight-DUTY FDP
            ///

            ////////////////////
            // if (saveResult.Code != HttpStatusCode.OK)
            //     return saveResult;
            //if (notify == 1)
            //{
            //    Magfa m = new Magfa();
            //    int c = 0;
            //    foreach (var x in sms)
            //    {
            //        var txt = sms[c];
            //        var no = nos[c];

            //        var smsResult = m.enqueue(1, no, txt)[0];
            //        c++;

            //    }
            //}

            //updated = await this.context.ViewFDPKeys.Where(q => updatedIds.Contains(q.Id)).ToListAsync();

            var result = new
            {
                removed,
                updatedId = updated.Select(q => q.Id).ToList(),
                updated = getRosterFDPDtos(updated)
            };

            return Ok(result);
        }
        void AddToCumDuty(FDP fdp, Models.dbEntities context = null)
        {
            try
            {
                bool do_save = context == null;
                if (context == null)
                    context = new Models.dbEntities();
                context.Database.ExecuteSqlCommand("Delete from TableDutyFDP where FDPId=" + fdp.Id);
                var utcdiff = Convert.ToInt32(ConfigurationManager.AppSettings["utcdiff"]);

                if (fdp.DutyType == 1165)
                {
                    var startLocal = ((DateTime)fdp.DateStart).AddMinutes(utcdiff);
                    var endLocal = ((DateTime)fdp.DateEnd).AddMinutes(utcdiff);
                    var startDate = startLocal.Date;
                    var endDate = endLocal.Date;
                    if (startDate == endDate)
                    {
                        var duration = (endLocal - startLocal).TotalMinutes;
                        context.TableDutyFDPs.Add(new TableDutyFDP()
                        {
                            CDate = startDate,
                            CrewId = fdp.CrewId,
                            Duration = duration,
                            DurationLocal = duration,
                            DutyEnd = fdp.DateEnd,
                            DutyEndLocal = endLocal,
                            DutyStart = fdp.DateStart,
                            DutyStartLocal = fdp.DateStart,
                            FDPId = fdp.Id,
                            GUID = Guid.NewGuid()
                        });

                    }
                    else
                    {
                        var duration1 = (endDate - startLocal).TotalMinutes;
                        var duration2 = (endLocal - endDate).TotalMinutes;
                        context.TableDutyFDPs.Add(new TableDutyFDP()
                        {
                            CDate = startDate,
                            CrewId = fdp.CrewId,
                            Duration = duration1,
                            DurationLocal = duration1,
                            DutyEnd = fdp.DateEnd,
                            DutyEndLocal = endLocal,
                            DutyStart = fdp.DateStart,
                            DutyStartLocal = fdp.DateStart,
                            FDPId = fdp.Id,
                            GUID = Guid.NewGuid()
                        });
                        context.TableDutyFDPs.Add(new TableDutyFDP()
                        {
                            CDate = endDate,
                            CrewId = fdp.CrewId,
                            Duration = duration2,
                            DurationLocal = duration2,
                            DutyEnd = fdp.DateEnd,
                            DutyEndLocal = endLocal,
                            DutyStart = fdp.DateStart,
                            DutyStartLocal = fdp.DateStart,
                            FDPId = fdp.Id,
                            GUID = Guid.NewGuid()
                        });
                    }




                }
                if (do_save)
                    context.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }
        private void AddToCumFlight(FDP fdp, List<SchFlight> flights, List<FDPItem> fdpitems, Models.dbEntities context = null)
        {
            try
            {
                //02-05
                bool do_save = context == null;
                if (context == null)
                    context = new Models.dbEntities();
                context.Database.ExecuteSqlCommand("Delete from TableFlightFDP where FDPId=" + fdp.Id);
                var utcdiff = Convert.ToInt32(ConfigurationManager.AppSettings["utcdiff"]);
                // var fdpItems = fdp.FDPItems.ToList();
                foreach (var flight in flights)
                {
                    var fdp_item = fdpitems.FirstOrDefault(q => q.FlightId == flight.ID);
                    var offblock = ((DateTime)flight.ChocksOut).AddMinutes(utcdiff);
                    var onblock = ((DateTime)flight.ChocksIn).AddMinutes(utcdiff);
                    if (offblock.Date == onblock.Date)
                    {
                        var duration = (onblock - offblock).TotalMinutes;
                        context.TableFlightFDPs.Add(new TableFlightFDP()
                        {
                            CDate = offblock.Date,
                            CrewId = fdp.CrewId,
                            Duration = duration,
                            DurationLocal = duration,
                            DutyEnd = flight.ChocksIn,
                            DutyEndLocal = onblock,
                            DutyStart = flight.ChocksOut,
                            DutyStartLocal = offblock,
                            FDPItemId = fdp_item.Id,
                            FlightId = flight.ID,
                            FDPId = fdp.Id,
                            GUID = Guid.NewGuid()
                        });
                    }
                    else
                    {
                        var duration1 = (onblock.Date - offblock).TotalMinutes;
                        var duration2 = (onblock - onblock.Date).TotalMinutes;
                        context.TableFlightFDPs.Add(new TableFlightFDP()
                        {
                            CDate = offblock.Date,
                            CrewId = fdp.CrewId,
                            Duration = duration1,
                            DurationLocal = duration1,
                            DutyEnd = flight.ChocksIn,
                            DutyEndLocal = onblock,
                            DutyStart = flight.ChocksOut,
                            DutyStartLocal = offblock,
                            FDPItemId = fdp_item.Id,
                            FlightId = flight.ID,
                            FDPId = fdp.Id,
                            GUID = Guid.NewGuid()
                        });
                        context.TableFlightFDPs.Add(new TableFlightFDP()
                        {
                            CDate = onblock.Date,
                            CrewId = fdp.CrewId,
                            Duration = duration2,
                            DurationLocal = duration2,
                            DutyEnd = flight.ChocksIn,
                            DutyEndLocal = onblock,
                            DutyStart = flight.ChocksOut,
                            DutyStartLocal = offblock,
                            FDPItemId = fdp_item.Id,
                            FlightId = flight.ID,
                            FDPId = fdp.Id,
                            GUID = Guid.NewGuid()
                        });


                    }
                }
                if (do_save)
                    context.SaveChanges();
            }
            catch (Exception ex)
            {

            }


        }



        internal List<RosterFDPDto> getRosterFDPDtos(List<FDP> fdps)
        {

            var result = new List<RosterFDPDto>();
            foreach (var x in fdps)
            {
                var item = new RosterFDPDto()
                {
                    Id = x.Id,
                    crewId = (int)x.CrewId,
                    flts = x.InitFlts,
                    from = Convert.ToInt32(x.InitFromIATA),
                    group = x.InitGroup,
                    homeBase = (int)x.InitHomeBase,
                    index = (int)x.InitIndex,
                    key = x.Key.Replace("*0", ""),
                    no = x.InitNo,
                    rank = x.InitRank,
                    route = x.InitRoute,
                    scheduleName = x.InitScheduleName,
                    to = Convert.ToInt32(x.InitToIATA),
                    flights = x.InitFlights.Split('*').ToList(),



                };
                item.ids = new List<RosterFDPId>();
                foreach (var f in item.flights)
                {
                    var prts = f.Split('_').ToList();
                    item.ids.Add(new RosterFDPId() { id = Convert.ToInt32(prts[0]), dh = Convert.ToInt32(prts[1]) });
                }
                result.Add(item);

            }
            return result;
        }

        internal string UpdateFirstLastFlights(int flightId, dbEntities _context)
        //(int fdpId,int fdpItemId,bool off)
        {
            //var viewFdpItems = this.context.ViewFDPItems.AsNoTracking().Where(q => q.FlightId == flightId).ToList();
            // var fdpItems = this.context.FDPItems.Where(q => q.FlightId == flightId).ToList();
            //var fdpIds = viewFdpItems.Select(q => q.FDPId).Distinct().ToList();
            var fdps = (from x in _context.FDPs
                        join y in _context.FDPItems on x.Id equals y.FDPId
                        where y.FlightId == flightId
                        select x).ToList();
            var fdpIds = fdps.Select(q => q.Id).Distinct().ToList();
            var viewFdpItems = (from x in _context.ViewFDPItems.AsNoTracking()
                                where fdpIds.Contains(x.FDPId)
                                select x).ToList();


            foreach (var f in fdps)
            {
                var fdpId = f.Id;
                var viewItems = viewFdpItems.Where(q => q.FDPId == fdpId).ToList();
                var firstItem = viewFdpItems.Where(q => (q.IsOff == null || q.IsOff == false) && q.FlightId != flightId).OrderBy(q => q.STD).FirstOrDefault();
                var lastItem = viewFdpItems.Where(q => (q.IsOff == null || q.IsOff == false) && q.FlightId != flightId).OrderByDescending(q => q.STD).FirstOrDefault();
                if (firstItem != null)
                    f.FirstFlightId = firstItem.FlightId;
                if (lastItem != null)
                    f.LastFlightId = lastItem.FlightId;
                if (f.UPD == null)
                    f.UPD = 1;
                else
                    f.UPD++;
            }


            return string.Empty;


        }


        //[Route("api/plan/flight/save")]

        //[AcceptVerbs("POST")]
        //public async Task<IHttpActionResult> PostFlight(ViewModels.FlightDto dto)
        //{

        //    var _context = new Models.dbEntities();
        //    //var validate = unitOfWork.FlightRepository.ValidateFlight(dto);
        //    //if (validate.Code != HttpStatusCode.OK)
        //    //    return validate;

        //    FlightInformation entity = null;
        //    FlightChangeHistory changeLog = null;
        //    if (dto.ID == -1)
        //    {
        //        entity = new FlightInformation();
        //        _context.FlightInformations.Add(entity);
        //    }

        //    else
        //    {
        //        entity = await unitOfWork.FlightRepository.GetFlightById(dto.ID);
        //        if (entity == null)
        //            return Exceptions.getNotFoundException();
        //        unitOfWork.FlightRepository.RemoveFlightLink(dto.ID);

        //        changeLog = new FlightChangeHistory()
        //        {
        //            Date = DateTime.Now,
        //            FlightId = entity.ID,

        //        };
        //        unitOfWork.FlightRepository.Insert(changeLog);
        //        changeLog.OldFlightNumer = entity.FlightNumber;
        //        changeLog.OldFromAirportId = entity.FromAirportId;
        //        changeLog.OldToAirportId = entity.ToAirportId;
        //        changeLog.OldSTD = entity.STD;
        //        changeLog.OldSTA = entity.STA;
        //        changeLog.OldStatusId = entity.FlightStatusID;
        //        changeLog.OldRegister = entity.RegisterID;
        //        changeLog.OldOffBlock = entity.ChocksOut;
        //        changeLog.OldOnBlock = entity.ChocksIn;
        //        changeLog.OldTakeOff = entity.Takeoff;
        //        changeLog.OldLanding = entity.Landing;
        //        changeLog.User = dto.UserName;

        //    }


        //    if (entity.STD != null)
        //    {
        //        var oldSTD = ((DateTime)entity.STD).AddMinutes(270).Date;
        //        var newSTD = ((DateTime)dto.STD).AddMinutes(270).Date;
        //        if (oldSTD != newSTD)
        //        {
        //            entity.FlightDate = oldSTD;
        //        }
        //    }


        //    ViewModels.FlightDto.Fill(entity, dto);
        //    if (dto.ID != -1 && changeLog != null)
        //    {
        //        entity.RegisterID = changeLog.OldRegister;
        //        changeLog.NewFlightNumber = entity.FlightNumber;
        //        changeLog.NewFromAirportId = entity.FromAirportId;
        //        changeLog.NewToAirportId = entity.ToAirportId;
        //        changeLog.NewSTD = entity.STD;
        //        changeLog.NewSTA = entity.STA;
        //        changeLog.NewStatusId = entity.FlightStatusID;
        //        changeLog.NewRegister = entity.RegisterID;
        //        changeLog.NewOffBlock = entity.ChocksOut;
        //        changeLog.NewOnBlock = entity.ChocksIn;
        //        changeLog.NewTakeOff = entity.Takeoff;
        //        changeLog.NewLanding = entity.Landing;
        //    }
        //    entity.BoxId = dto.BoxId;

        //    if (dto.LinkedFlight != null)
        //    {
        //        var link = new FlightLink()
        //        {
        //            FlightInformation = entity,
        //            Flight2Id = (int)dto.LinkedFlight,
        //            ReasonId = (int)dto.LinkedReason,
        //            Remark = dto.LinkedRemark

        //        };
        //        unitOfWork.FlightRepository.Insert(link);
        //    }

        //    var saveResult = await unitOfWork.SaveAsync();
        //    if (saveResult.Code != HttpStatusCode.OK)
        //        return saveResult;

        //    if (dto.SMSNira == 1)
        //    {
        //        await unitOfWork.FlightRepository.NotifyNira(entity.ID, dto.UserName);
        //    }

        //    //bip
        //    var fg = await unitOfWork.FlightRepository.GetViewFlightGantts().Where(q => q.ID == entity.ID).ToListAsync();
        //    ViewModels.ViewFlightsGanttDto odto = new ViewFlightsGanttDto();
        //    ViewModels.ViewFlightsGanttDto.FillDto(fg.First(), odto, 0, 1);


        //    var resgroups = from x in fg
        //                    group x by new { x.AircraftType, AircraftTypeId = x.TypeId }
        //                   into grp
        //                    select new { groupId = grp.Key.AircraftTypeId, Title = grp.Key.AircraftType };
        //    var ressq = (from x in fg
        //                 group x by new { x.RegisterID, x.Register, x.TypeId }
        //             into grp

        //                 orderby unitOfWork.FlightRepository.getOrderIndex(grp.Key.Register, new List<string>())
        //                 select new { resourceId = grp.Key.RegisterID, resourceName = grp.Key.Register, groupId = grp.Key.TypeId }).ToList();

        //    odto.resourceId.Add((int)odto.RegisterID);


        //    var oresult = new
        //    {
        //        flight = odto,
        //        resgroups,
        //        ressq
        //    };

        //    return Ok(/*entity*/oresult);

        //}

        public string getOrderIndex(string reg, List<string> grounds)
        {
            var str = "";
            //orderby grp.Key.Register.Contains("CNL") ? "ZZZZ" :( grp.Key.Register[grp.Key.Register.Length - 1].ToString())
            if (reg.Contains("CNL"))
                str = "ZZZZZZ";
            else if (reg.Contains("RBC"))
                str = "ZZZZZY";
            else
           //str = 1000000;
           // if (grounds.Contains(reg))
           if (reg.Contains("."))
            {
                str = "ZZZZ" + reg[reg.Length - 2];
                //str = 100000;
            }
            // str= reg[reg.Length - 1].ToString();
            else
                str = reg[reg.Length - 1].ToString();

            return str;

        }



    }
    public class SimpleDto
    {
        public string username { get; set; }
        public List<int> ids { get; set; }
    }
    public class RosterFDPDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public List<RosterFDPId> ids { get; set; }
        public int crewId { get; set; }
        public string rank { get; set; }
        public int index { get; set; }
        public List<string> flights { get; set; }
        public int from { get; set; }
        public int to { get; set; }
        public int homeBase { get; set; }
        public string flts { get; set; }
        public string route { get; set; }
        public string key { get; set; }
        public string group { get; set; }
        public string scheduleName { get; set; }
        public string no { get; set; }
        public int? extension { get; set; }
        public decimal? maxFDP { get; set; }

        public bool split { get; set; }

        public bool? IsSplitDuty { get; set; }
        public int? SplitValue { get; set; }

        public int? IsAdmin { get; set; }

        public int? DeletedFDPId { get; set; }

        public List<RosterFDPDtoItem> items { get; set; }

        public double getDuty(double? default_reporting = 60)
        {
            var def = (double)default_reporting;
            return (this.items.Last().sta.AddMinutes(30) - this.items.First().std.AddMinutes(-1 * def)).TotalMinutes;
        }
        public double getFlight()
        {
            double flt = 0;
            foreach (var x in this.items)
                flt += (x.sta - x.std).TotalMinutes;
            return flt;
        }
        public static List<RosterFDPDtoItem> getItems(List<string> flts)
        {
            List<RosterFDPDtoItem> result = new List<RosterFDPDtoItem>();
            foreach (var x in flts)
            {
                var parts = x.Split('_');
                var item = new RosterFDPDtoItem();
                item.flightId = Convert.ToInt32(parts[0]);
                item.dh = Convert.ToInt32(parts[1]);
                var stdStr = parts[2];
                var staStr = parts[3];
                item.std = new DateTime(Convert.ToInt32(stdStr.Substring(0, 4)), Convert.ToInt32(stdStr.Substring(4, 2)), Convert.ToInt32(stdStr.Substring(6, 2))
                    , Convert.ToInt32(stdStr.Substring(8, 2))
                    , Convert.ToInt32(stdStr.Substring(10, 2))
                    , 0
                    ).ToUniversalTime();
                item.sta = new DateTime(Convert.ToInt32(staStr.Substring(0, 4)), Convert.ToInt32(staStr.Substring(4, 2)), Convert.ToInt32(staStr.Substring(6, 2))
                   , Convert.ToInt32(staStr.Substring(8, 2))
                   , Convert.ToInt32(staStr.Substring(10, 2))
                   , 0
                   ).ToUniversalTime();
                item.no = parts[4];
                item.from = parts[5];
                item.to = parts[6];

                result.Add(item);
            }

            return result;
        }
        public static List<string> getFlightsStrs(List<Models.SchFlight> flts, List<RosterFDPId> ids)
        {
            var result = new List<string>();
            //fdp.flights.push(flt.ID + '_' + _d.dh + '_' + $scope.DatetoStr(new Date(flt.ChocksOut)) + '_' + $scope.DatetoStr(new Date(flt.ChocksIn))
            //+ '_' + flt.FlightNumber + '_' + flt.FromAirportIATA + '_' + flt.ToAirportIATA);
            // \"flights\":[\"542235_0_202301200735_202301200905_5822_THR_KIH\",\"542237_0_202301200950_202301201135_5823_KIH_THR\"],
            //\"key2\":\"542235*0_542237*0\",
            foreach (var x in flts)
            {
                var id_item = ids.FirstOrDefault(q => q.id == x.ID);
                var prts = new List<string>();
                prts.Add(x.ID.ToString());
                prts.Add(id_item.dh.ToString());
                prts.Add(((DateTime)x.ChocksOutLocal).ToString("yyyyMMddHHmm"));
                prts.Add(((DateTime)x.ChocksInLocal).ToString("yyyyMMddHHmm"));
                prts.Add(x.FlightNumber);
                prts.Add(x.FromAirportIATA);
                prts.Add(x.ToAirportIATA);
                result.Add(string.Join("_", prts));
            }
            return result;
        }

        public static List<string> getFlightsStrs(List<Models.SchFlight> flts, List<int?> dhs)
        {
            var result = new List<string>();
            //fdp.flights.push(flt.ID + '_' + _d.dh + '_' + $scope.DatetoStr(new Date(flt.ChocksOut)) + '_' + $scope.DatetoStr(new Date(flt.ChocksIn))
            //+ '_' + flt.FlightNumber + '_' + flt.FromAirportIATA + '_' + flt.ToAirportIATA);
            // \"flights\":[\"542235_0_202301200735_202301200905_5822_THR_KIH\",\"542237_0_202301200950_202301201135_5823_KIH_THR\"],
            //\"key2\":\"542235*0_542237*0\",
            foreach (var x in flts)
            {
                var dh = dhs.FirstOrDefault(q => q == x.ID);
                var prts = new List<string>();
                prts.Add(x.ID.ToString());
                prts.Add(dh == null ? "0" : "1");
                prts.Add(((DateTime)x.ChocksOutLocal).ToString("yyyyMMddHHmm"));
                prts.Add(((DateTime)x.ChocksInLocal).ToString("yyyyMMddHHmm"));
                prts.Add(x.FlightNumber);
                prts.Add(x.FromAirportIATA);
                prts.Add(x.ToAirportIATA);
                result.Add(string.Join("_", prts));
            }
            return result;
        }
        public static List<RosterFDPDtoItem> getItemsX(List<Models.SchFlight> flts, List<RosterFDPId> ids)
        {
            //oooo
            List<RosterFDPDtoItem> result = new List<RosterFDPDtoItem>();
            foreach (var x in flts)
            {
                var id_item = ids.FirstOrDefault(q => q.id == x.ID);
                var item = new RosterFDPDtoItem();
                item.flightId = x.ID;
                item.dh = id_item.dh;
                item.pos = id_item.pos;
                item.std = (DateTime)x.STD;
                item.sta = (DateTime)x.STA;
                item.offblock = (DateTime)x.ChocksOut;
                item.onblock = (DateTime)x.ChocksIn;

                item.no = x.FlightNumber;
                item.from = x.FromAirportIATA;
                item.to = x.ToAirportIATA;

                result.Add(item);

            }


            return result;
        }


        public static int getRank(string rank)
        {
            if (rank.StartsWith("IP"))
                return 12000;
            if (rank.StartsWith("P1"))
                return 1160;
            if (rank.StartsWith("P2"))
                return 1161;
            if (rank.ToUpper().StartsWith("SAFETY"))
                return 1162;
            if (rank.ToUpper().StartsWith("FE"))
                return 1165;
            if (rank.StartsWith("ISCCM"))
                return 10002;
            if (rank.StartsWith("SCCM"))
                return 1157;
            if (rank.StartsWith("CCM"))
                return 1158;
            if (rank.StartsWith("OBS"))
                return 1153;
            if (rank.StartsWith("CHECK"))
                return 1154;
            if (rank.StartsWith("00103"))
                return 12001;
            if (rank.StartsWith("004"))
                return 12002;
            if (rank.StartsWith("005"))
                return 12003;

            return -1;

        }
        public static string getRankStr(int rank)
        {
            if (rank == 12000)
                return "IP";
            if (rank == 1160)
                return "P1";
            if (rank == 1161)
                return "P2";
            if (rank == 1162)
                return "SAFETY";
            if (rank == 10002)
                return "ISCCM";
            if (rank == 1157)
                return "SCCM";
            if (rank == 1158)
                return "CCM";
            if (rank == 1153)
                return "OBS";
            if (rank == 1154)
                return "CHECK";
            return "";
        }


    }



    public class RosterFDPId
    {
        public int id { get; set; }
        public int dh { get; set; }
        public string pos { get; set; }
    }

    public class RosterFDPDtoItem
    {
        public int flightId { get; set; }
        public int dh { get; set; }
        public DateTime std { get; set; }
        public DateTime sta { get; set; }
        public DateTime offblock { get; set; }
        public DateTime onblock { get; set; }
        public int index { get; set; }
        public int rankId { get; set; }
        public string no { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public string pos { get; set; }




    }
}
