using System;
using System.Collections.Generic;
using System.Linq;
using ApiFDM.Objects;
using ApiFDM.Models;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Data;
using System.Data.Entity;

using System.Data.SqlClient;
using System.Web.Http.Results;
using OfficeOpenXml;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;

using Novacode;

namespace ApiFDM.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class FDMV2Controller : ApiController
    {
        [Route("api/fdm/V2/events/{dt1}/{dt2}")]
        public async Task<DataResponse> get_events(DateTime dt1, DateTime dt2)
        {
            FDMEntities context = new FDMEntities();
            dt2 = dt2.AddDays(1);
            var query = from x in context.view_fdm_processed
                        where x.std >= dt1 && x.std <= dt2
                        group x by new { x.register_id, x.register, x.ac_type2 } into grp
                        select new
                        {
                            grp.Key.register_id,

                            grp.Key.register,
                            ac_type = grp.Key.ac_type2,
                            count = grp.Count(),
                            high_count = grp.Sum(g => g.severity == "High" ? 1 : 0),
                            medium_count = grp.Sum(g => g.severity == "Medium" ? 1 : 0),
                            low_count = grp.Sum(g => g.severity == "Low" ? 1 : 0),
                            total_score = grp.Sum(g => g.severity == "High" ? 1 : 0) * 4 +
                                     grp.Sum(g => g.severity == "Medium" ? 1 : 0) * 2 +
                                     grp.Sum(g => g.severity == "Low" ? 1 : 0)
                        };

            var query_flights = from x in context.FlightInformations
                                join y in context.ViewMSNs on x.RegisterID equals y.ID
                                where x.STD >= dt1 && x.STD <= dt2 && (x.FlightStatusID == 3 || x.FlightStatusID == 15 || x.FlightStatusID == 7 || x.FlightStatusID == 17)
                                group x by new { x.RegisterID, y.Register, y.AircraftType2 } into grp
                                select new
                                {
                                    register_id = grp.Key.RegisterID,
                                    register = grp.Key.Register,
                                    ac_type = grp.Key.AircraftType2,
                                    count = grp.Count()
                                };
            var result_register = await query.ToListAsync();
            var _xx = await query_flights.ToListAsync();
            var result_register_flight = (from q in _xx
                                          let matchedResult = result_register.FirstOrDefault(w => w.register_id == q.register_id)
                                          select new
                                          {
                                              q.register_id,
                                              q.register,
                                              q.ac_type,
                                              flight_count = q.count,
                                              event_count = matchedResult?.count ?? 0,
                                              high_count = matchedResult?.high_count ?? 0,
                                              medium_count = matchedResult?.medium_count ?? 0,
                                              low_count = matchedResult?.low_count ?? 0,
                                              high_score = (matchedResult?.high_count ?? 0) * 4,
                                              medium_score = (matchedResult?.medium_count ?? 0) * 2,
                                              low_score = (matchedResult?.low_count ?? 0) * 1,
                                              total_score = matchedResult?.total_score ?? 0,
                                              score_per_event = matchedResult != null && matchedResult.count > 0
                                                                   ? Math.Round((double)matchedResult.total_score / matchedResult.count, 1)
                                                                   : 0,
                                              score_per_flight = q.count > 0
                                                     ? Math.Round((double)(matchedResult?.total_score ?? 0) / q.count, 1)
                                                     : 0
                                          }).ToList();

            var result_type = (from x in result_register_flight
                               group x by new { x.ac_type } into grp
                               select new
                               {
                                   grp.Key.ac_type,
                                   count = grp.Sum(q => q.event_count),
                                   flight_count = grp.Sum(q => q.flight_count),
                                   high_count = grp.Sum(q => q.high_count),
                                   medium_count = grp.Sum(q => q.medium_count),
                                   low_count = grp.Sum(q => q.low_count),
                                   high_score = grp.Sum(q => q.high_score),
                                   medium_score = grp.Sum(q => q.medium_score),
                                   low_score = grp.Sum(q => q.low_score),
                                   total_score = grp.Sum(q => q.total_score),
                                   score_per_event = Math.Round(grp.Sum(q => q.total_score) * 1.0 / grp.Sum(q => q.event_count), 1),
                                   score_per_flight = Math.Round(grp.Sum(q => q.total_score) * 1.0 / grp.Sum(q => q.flight_count), 1)
                               }).ToList();


            var query_route = from x in context.view_fdm_processed
                              where x.std >= dt1 && x.std <= dt2
                              group x by new { x.ac_type2, x.dep_iata, x.arr_iata } into grp
                              select new
                              {
                                  grp.Key.dep_iata,

                                  grp.Key.arr_iata,
                                  route = grp.Key.dep_iata + "-" + grp.Key.arr_iata,
                                  ac_type = grp.Key.ac_type2,
                                  count = grp.Count(),
                                  high_count = grp.Sum(g => g.severity == "High" ? 1 : 0),
                                  medium_count = grp.Sum(g => g.severity == "Medium" ? 1 : 0),
                                  low_count = grp.Sum(g => g.severity == "Low" ? 1 : 0),
                                  total_score = grp.Sum(g => g.severity == "High" ? 1 : 0) * 4 +
                                           grp.Sum(g => g.severity == "Medium" ? 1 : 0) * 2 +
                                           grp.Sum(g => g.severity == "Low" ? 1 : 0)
                              };


            var query_flights_route = from x in context.view_fdm_flight_crew
                                      where x.STD >= dt1 && x.STD <= dt2
                                      group x by new { x.ac_type2, x.dep_id, x.dep_iata, x.dep_icao, x.arr_id, x.arr_iata, x.arr_icao } into grp
                                      select new
                                      {

                                          ac_type = grp.Key.ac_type2,

                                          grp.Key.dep_id,
                                          grp.Key.dep_iata,
                                          grp.Key.dep_icao,
                                          grp.Key.arr_id,
                                          grp.Key.arr_iata,
                                          grp.Key.arr_icao,
                                          count = grp.Count()
                                      };

            var result_route = await query_route.ToListAsync();
            var _yy = await query_flights_route.ToListAsync();
            var result_route_flight = (from q in _yy
                                       let matchedResult = result_route.FirstOrDefault(w => w.ac_type == q.ac_type && w.dep_iata == q.dep_iata && w.arr_iata == q.arr_iata)
                                       select new
                                       {


                                           q.ac_type,
                                           q.dep_iata,
                                           q.arr_iata,
                                           route = q.dep_iata + "-" + q.arr_iata,
                                           flight_count = q.count,
                                           event_count = matchedResult?.count ?? 0,
                                           high_count = matchedResult?.high_count ?? 0,
                                           medium_count = matchedResult?.medium_count ?? 0,
                                           low_count = matchedResult?.low_count ?? 0,
                                           high_score = (matchedResult?.high_count ?? 0) * 4,
                                           medium_score = (matchedResult?.medium_count ?? 0) * 2,
                                           low_score = (matchedResult?.low_count ?? 0) * 1,
                                           total_score = matchedResult?.total_score ?? 0,
                                           score_per_event = matchedResult != null && matchedResult.count > 0
                                                                ? Math.Round((double)matchedResult.total_score / matchedResult.count, 1)
                                                                : 0,
                                           score_per_flight = q.count > 0
                                                  ? Math.Round((double)(matchedResult?.total_score ?? 0) / q.count, 1)
                                                  : 0
                                       }).ToList();




            return new DataResponse()
            {
                Data = new
                {
                    result_register_flight,
                    result_type,
                    result_route_flight

                },
                IsSuccess = true
            };

        }
        [Route("api/fdm/V2/eventsCrew/{dt1}/{dt2}")]
        public async Task<DataResponse> get_events_captain(DateTime dt1, DateTime dt2)
        {
            FDMEntities context = new FDMEntities();
            dt2 = dt2.AddDays(1);
            var query = from x in context.view_fdm_processed
                        join y in context.fdm_crew on x.id equals y.processed_id
                        where x.std >= dt1 && x.std <= dt2
                        group x by new { x.register_id, x.register, x.ac_type2, y.crew_id, y.first_name, y.last_name, y.name, y.rank, y.rank_code, y.position } into grp
                        select new
                        {
                            grp.Key.register_id,
                            grp.Key.register,
                            ac_type = grp.Key.ac_type2,
                            grp.Key.crew_id,
                            grp.Key.first_name,
                            grp.Key.last_name,
                            grp.Key.name,
                            grp.Key.rank,
                            grp.Key.rank_code,
                            grp.Key.position,
                            count = grp.Count(),
                            high_count = grp.Sum(g => g.severity == "High" ? 1 : 0),
                            medium_count = grp.Sum(g => g.severity == "Medium" ? 1 : 0),
                            low_count = grp.Sum(g => g.severity == "Low" ? 1 : 0),
                            total_score = grp.Sum(g => g.severity == "High" ? 1 : 0) * 4 +
                                     grp.Sum(g => g.severity == "Medium" ? 1 : 0) * 2 +
                                     grp.Sum(g => g.severity == "Low" ? 1 : 0)
                        };

            var query_flights = from x in context.view_fdm_flight_crew
                                where x.STD >= dt1 && x.STD <= dt2
                                group x by new { x.RegisterID, x.Register, x.ac_type2, x.CrewId, x.Name, x.JobGroup, x.JobGroupCode, x.Position } into grp
                                select new
                                {
                                    register_id = grp.Key.RegisterID,
                                    register = grp.Key.Register,
                                    ac_type = grp.Key.ac_type2,
                                    crew_id = grp.Key.CrewId,
                                    crew_name = grp.Key.Name,
                                    crew_job_group = grp.Key.JobGroup,
                                    crew_job_group_code = grp.Key.JobGroupCode,
                                    crew_position = grp.Key.Position,
                                    count = grp.Count()
                                };
            var result_register = await query.ToListAsync();
            var _xx = await query_flights.ToListAsync();
            var result_register_flight_crew = (from q in _xx
                                               where q.crew_position == "CPT"
                                               select new
                                               {
                                                   q.register_id,
                                                   q.register,
                                                   q.ac_type,
                                                   q.crew_id,
                                                   q.crew_name,
                                                   q.crew_job_group,
                                                   q.crew_job_group_code,
                                                   q.crew_position,
                                                   flight_count = q.count,
                                                   event_count = result_register.FirstOrDefault(w => w.register_id == q.register_id && w.crew_id == q.crew_id && w.position == q.crew_position)?.count ?? 0,
                                                   high_count = result_register.FirstOrDefault(w => w.register_id == q.register_id && w.crew_id == q.crew_id && w.position == q.crew_position)?.high_count ?? 0,
                                                   medium_count = result_register.FirstOrDefault(w => w.register_id == q.register_id && w.crew_id == q.crew_id && w.position == q.crew_position)?.medium_count ?? 0,
                                                   low_count = result_register.FirstOrDefault(w => w.register_id == q.register_id && w.crew_id == q.crew_id && w.position == q.crew_position)?.low_count ?? 0,
                                                   high_score = result_register.FirstOrDefault(w => w.register_id == q.register_id && w.crew_id == q.crew_id && w.position == q.crew_position)?.high_count * 4 ?? 0,
                                                   medium_score = result_register.FirstOrDefault(w => w.register_id == q.register_id && w.crew_id == q.crew_id && w.position == q.crew_position)?.medium_count * 2 ?? 0,
                                                   low_score = result_register.FirstOrDefault(w => w.register_id == q.register_id && w.crew_id == q.crew_id && w.position == q.crew_position)?.low_count * 1 ?? 0,
                                                   total_score = result_register.FirstOrDefault(w => w.register_id == q.register_id && w.crew_id == q.crew_id && w.position == q.crew_position)?.total_score ?? 0
                                               }).ToList();

            var result_type = (from x in result_register_flight_crew
                               where x.crew_position == "CPT"
                               group x by new { x.ac_type, x.crew_id } into grp
                               select new
                               {
                                   grp.Key.ac_type,
                                   grp.Key.crew_id,
                                   count = grp.Sum(q => q.event_count),
                                   high_count = grp.Sum(q => q.high_count),
                                   medium_count = grp.Sum(q => q.medium_count),
                                   low_count = grp.Sum(q => q.low_count),
                                   high_score = grp.Sum(q => q.high_score),
                                   medium_score = grp.Sum(q => q.medium_score),
                                   low_score = grp.Sum(q => q.low_score),
                                   total_score = grp.Sum(q => q.total_score),
                                   score_per_event = Math.Round(grp.Sum(q => q.total_score) * 1.0 / grp.Sum(q => q.event_count), 1),
                                   score_per_flight = Math.Round(grp.Sum(q => q.total_score) * 1.0 / grp.Sum(q => q.flight_count), 1)
                               }).ToList();


            return new DataResponse()
            {
                Data = new
                {
                    result_register_flight_crew,
                    result_type,

                },
                IsSuccess = true
            };

        }

        [Route("api/fdm/V2/eventsCrewRoute/{dt1}/{dt2}")]
        public async Task<DataResponse> get_events_captain_route(DateTime dt1, DateTime dt2)
        {
            FDMEntities context = new FDMEntities();
            dt2 = dt2.AddDays(1);
            var query = from x in context.view_fdm_processed
                        join y in context.fdm_crew on x.id equals y.processed_id
                        where x.std >= dt1 && x.std <= dt2
                        group x by new { x.register_id, x.register, x.ac_type2, y.crew_id, y.first_name, y.last_name, y.name, y.rank, y.rank_code, y.position, x.dep_id, x.dep_iata, x.dep_icao, x.arr_id, x.arr_iata, x.arr_icao } into grp
                        select new
                        {
                            grp.Key.register_id,
                            grp.Key.register,
                            ac_type = grp.Key.ac_type2,
                            grp.Key.crew_id,
                            grp.Key.first_name,
                            grp.Key.last_name,
                            grp.Key.name,
                            grp.Key.rank,
                            grp.Key.rank_code,
                            grp.Key.position,
                            grp.Key.dep_id,
                            grp.Key.dep_iata,
                            grp.Key.dep_icao,
                            grp.Key.arr_id,
                            grp.Key.arr_iata,
                            grp.Key.arr_icao,
                            count = grp.Count(),
                            high_count = grp.Sum(g => g.severity == "High" ? 1 : 0),
                            medium_count = grp.Sum(g => g.severity == "Medium" ? 1 : 0),
                            low_count = grp.Sum(g => g.severity == "Low" ? 1 : 0),
                            total_score = grp.Sum(g => g.severity == "High" ? 1 : 0) * 4 +
                                     grp.Sum(g => g.severity == "Medium" ? 1 : 0) * 2 +
                                     grp.Sum(g => g.severity == "Low" ? 1 : 0)
                        };

            var query_flights = from x in context.view_fdm_flight_crew
                                where x.STD >= dt1 && x.STD <= dt2
                                group x by new { x.RegisterID, x.Register, x.ac_type2, x.CrewId, x.Name, x.JobGroup, x.JobGroupCode, x.Position, x.dep_id, x.dep_iata, x.dep_icao, x.arr_id, x.arr_iata, x.arr_icao } into grp
                                select new
                                {
                                    register_id = grp.Key.RegisterID,
                                    register = grp.Key.Register,
                                    ac_type = grp.Key.ac_type2,
                                    crew_id = grp.Key.CrewId,
                                    crew_name = grp.Key.Name,
                                    crew_job_group = grp.Key.JobGroup,
                                    crew_job_group_code = grp.Key.JobGroupCode,
                                    crew_position = grp.Key.Position,
                                    grp.Key.dep_id,
                                    grp.Key.dep_iata,
                                    grp.Key.dep_icao,
                                    grp.Key.arr_id,
                                    grp.Key.arr_iata,
                                    grp.Key.arr_icao,
                                    count = grp.Count()
                                };
            var result_register = await query.ToListAsync();
            var _xx = await query_flights.ToListAsync();
            var result_register_flight_crew = (from q in _xx
                                               where q.crew_position == "CPT"
                                               select new
                                               {
                                                   q.register_id,
                                                   q.register,
                                                   q.ac_type,
                                                   q.crew_id,
                                                   q.crew_name,
                                                   q.crew_job_group,
                                                   q.crew_job_group_code,
                                                   q.crew_position,
                                                   q.dep_id,
                                                   q.dep_iata,
                                                   q.dep_icao,
                                                   q.arr_id,
                                                   q.arr_iata,
                                                   q.arr_icao,
                                                   route = q.dep_iata + '_' + q.arr_iata,
                                                   flight_count = q.count,
                                                   event_count = result_register.FirstOrDefault(w => w.register_id == q.register_id && w.crew_id == q.crew_id && w.position == q.crew_position)?.count ?? 0,
                                                   high_count = result_register.FirstOrDefault(w => w.register_id == q.register_id && w.crew_id == q.crew_id && w.position == q.crew_position)?.high_count ?? 0,
                                                   medium_count = result_register.FirstOrDefault(w => w.register_id == q.register_id && w.crew_id == q.crew_id && w.position == q.crew_position)?.medium_count ?? 0,
                                                   low_count = result_register.FirstOrDefault(w => w.register_id == q.register_id && w.crew_id == q.crew_id && w.position == q.crew_position)?.low_count ?? 0,
                                                   high_score = result_register.FirstOrDefault(w => w.register_id == q.register_id && w.crew_id == q.crew_id && w.position == q.crew_position)?.high_count * 4 ?? 0,
                                                   medium_score = result_register.FirstOrDefault(w => w.register_id == q.register_id && w.crew_id == q.crew_id && w.position == q.crew_position)?.medium_count * 2 ?? 0,
                                                   low_score = result_register.FirstOrDefault(w => w.register_id == q.register_id && w.crew_id == q.crew_id && w.position == q.crew_position)?.low_count * 1 ?? 0,
                                                   total_score = result_register.FirstOrDefault(w => w.register_id == q.register_id && w.crew_id == q.crew_id && w.position == q.crew_position)?.total_score ?? 0
                                               }).ToList();

            var result_type = (from x in result_register_flight_crew
                               where x.crew_position == "CPT"
                               group x by new { x.ac_type, x.crew_name, x.arr_iata, x.dep_iata, x.route } into grp
                               select new
                               {
                                   grp.Key.ac_type,
                                   grp.Key.crew_name,
                                   grp.Key.arr_iata,
                                   grp.Key.dep_iata,
                                   grp.Key.route,
                                   count = grp.Sum(q => q.event_count),
                                   high_count = grp.Sum(q => q.high_count),
                                   medium_count = grp.Sum(q => q.medium_count),
                                   low_count = grp.Sum(q => q.low_count),
                                   high_score = grp.Sum(q => q.high_score),
                                   medium_score = grp.Sum(q => q.medium_score),
                                   low_score = grp.Sum(q => q.low_score),
                                   total_score = grp.Sum(q => q.total_score),
                                   score_per_event = Math.Round(grp.Sum(q => q.total_score) * 1.0 / grp.Sum(q => q.event_count), 1),
                                   score_per_flight = Math.Round(grp.Sum(q => q.total_score) * 1.0 / grp.Sum(q => q.flight_count), 1)
                               }).ToList();


            var result_route = (from x in result_register_flight_crew
                                where x.crew_position == "CPT"
                                group x by new { x.ac_type, x.crew_name, x.arr_iata, x.dep_iata, x.route } into grp
                                select new
                                {
                                    grp.Key.ac_type,
                                    grp.Key.crew_name,
                                    grp.Key.arr_iata,
                                    grp.Key.dep_iata,
                                    grp.Key.route,
                                    count = grp.Sum(q => q.event_count),
                                    high_count = grp.Sum(q => q.high_count),
                                    medium_count = grp.Sum(q => q.medium_count),
                                    low_count = grp.Sum(q => q.low_count),
                                    high_score = grp.Sum(q => q.high_score),
                                    medium_score = grp.Sum(q => q.medium_score),
                                    low_score = grp.Sum(q => q.low_score),
                                    total_score = grp.Sum(q => q.total_score),
                                    score_per_event = Math.Round(grp.Sum(q => q.total_score) * 1.0 / grp.Sum(q => q.event_count), 1),
                                    score_per_flight = Math.Round(grp.Sum(q => q.total_score) * 1.0 / grp.Sum(q => q.flight_count), 1)
                                }).ToList();


            return new DataResponse()
            {
                Data = new
                {
                    result_register_flight_crew,
                    result_type,

                },
                IsSuccess = true
            };

        }
        //using OfficeOpenXml;


        public class Course
        {
            public string Title { get; set; }
            public string Instructor { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public List<Student> Students { get; set; }
        }

        public class Student
        {
            public string FullName { get; set; }
            public List<SessionAttendance> Attendances { get; set; }
        }

        public class SessionAttendance
        {
            public DateTime Date { get; set; }
            public TimeSpan StartTime { get; set; }
            public TimeSpan EndTime { get; set; }
            public bool IsPresent { get; set; }
        }
        public static DateTime? ConvertPersianDateToGregorian(string input)
        {
            // استفاده از Regex برای پیدا کردن تاریخ شمسی در فرمت yyyy/MM/dd
            var match = Regex.Match(input, @"\d{4}/\d{2}/\d{2}");
            if (match.Success)
            {
                string persianDate = match.Value;
                string[] parts = persianDate.Split('/');
                int year = int.Parse(parts[0]);
                int month = int.Parse(parts[1]);
                int day = int.Parse(parts[2]);

                // استفاده از PersianCalendar برای تبدیل
                PersianCalendar pc = new PersianCalendar();
                return pc.ToDateTime(year, month, day, 0, 0, 0, 0);
            }

            // اگر تاریخ پیدا نشد، مقدار null برمی‌گردد
            return null;
        }

        public static DateTime? ConvertPersianDateToGregorian2(string input)
        {
            // استفاده از Regex برای پیدا کردن تاریخ شمسی در فرمت yyyy/MM/dd
            var match = Regex.Match(input, @"\d{4}-\d{2}-\d{2}");
            if (match.Success)
            {
                string persianDate = match.Value;
                string[] parts = persianDate.Split('-');
                int year = int.Parse(parts[0]);
                int month = int.Parse(parts[1]);
                int day = int.Parse(parts[2]);

                // استفاده از PersianCalendar برای تبدیل
                PersianCalendar pc = new PersianCalendar();
                return pc.ToDateTime(year, month, day, 0, 0, 0, 0);
            }

            // اگر تاریخ پیدا نشد، مقدار null برمی‌گردد
            return null;
        }
        public class ExtractedData
        {
            public string PersianDate { get; set; }
            public DateTime? GDate { get; set; }
            public int Hours { get; set; }
            public int Days { get; set; }
        }
        public static ExtractedData ExtractFromText(string input)
        {
            var result = new ExtractedData();

            // استخراج تاریخ (مثلاً: 1403/12/01)
            var dateMatch = Regex.Match(input, @"\d{4}/\d{2}/\d{2}");
            result.PersianDate = dateMatch.Success ? dateMatch.Value : null;
            if (result.PersianDate != null)
                result.GDate = ConvertPersianDateToGregorian(result.PersianDate);

            // استخراج همه اعداد
            var numberMatches = Regex.Matches(input, @"\d+");

            // فرض بر این است که اولین عدد بعد از تاریخ، ساعت و دومین عدد روز است
            if (numberMatches.Count >= 3)
            {
                result.Hours = int.Parse(numberMatches[3].Value);
                result.Days = int.Parse(numberMatches[4].Value);
            }

            return result;
        }
        public class AttendanceExcelReader
        {
            public Course ReadCourseFromExcel(string filePath)
            {
                // ExcelPackage.License = new OfficeOpenXml.License.LicenseProvider().GetLicense(LicenseContext.NonCommercial);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                var package = new ExcelPackage(new FileInfo(filePath));
                var ws = package.Workbook.Worksheets["Table 1"];

                // مرحله 1: خواندن اطلاعات متادیتا دوره (فرض بر این است که در ردیف‌های بالا هستند)
                string courseTitle = ws.Cells[3, 3].Text?.Trim();          // فرضاً عنوان دوره در A1
                string instructor = (ws.Cells[5, 3].Text?.Trim()).Split(':')[1];
                instructor = instructor.Trim();// نام مدرس در B3
                string startDateStr = ws.Cells[4, 3].Text?.Trim();
                var _date_start = ConvertPersianDateToGregorian(startDateStr);

                string endDateStr = ws.Cells[4, 11].Text?.Trim();// تاریخ شروع در B2
                var extracted = ExtractFromText(endDateStr);
                //DateTime.TryParseExact(startDateStr, "yyyy/MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var startDate);

                var course = new Course
                {
                    Title = courseTitle,
                    Instructor = instructor,
                    StartDate = _date_start,
                };

                var studentsDict = new Dictionary<string, Student>();

                for (int row = 9; ws.Cells[row, 3].Value != null; row++) // شروع از ردیف 9
                {
                    string firstName = ws.Cells[row, 5].Text?.Trim();
                    string lastName = ws.Cells[row, 7].Text?.Trim();
                    string fullName = $"{firstName} {lastName}".Trim();

                    if (!studentsDict.ContainsKey(fullName))
                        studentsDict[fullName] = new Student { FullName = fullName };

                    for (int col = 9; col <= ws.Dimension.End.Column; col++)
                    {
                        string dateText = ws.Cells[6, col].Text?.Trim();
                        string timeText = ws.Cells[7, col].Text?.Replace("\n", "").Trim();
                        string symbol = ws.Cells[row, col].Text?.Trim();

                        if (string.IsNullOrWhiteSpace(dateText) || string.IsNullOrWhiteSpace(timeText))
                            continue;

                        if (!DateTime.TryParseExact(dateText, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var sessionDate))
                            continue;

                        var times = timeText.Split('-');
                        if (times.Length != 2 ||
                            !TimeSpan.TryParse(times[0], out var start) ||
                            !TimeSpan.TryParse(times[1], out var end))
                            continue;

                        studentsDict[fullName].Attendances.Add(new SessionAttendance
                        {
                            Date = sessionDate,
                            StartTime = start,
                            EndTime = end,
                            IsPresent = symbol == "" || symbol == "✓"
                        });

                        // تنظیم تاریخ پایان دوره بر اساس آخرین تاریخ جلسه
                        if (sessionDate > course.EndDate)
                            course.EndDate = sessionDate;
                    }
                }

                course.Students = studentsDict.Values.ToList();
                return course;
            }
        }

        [Route("api/xls")]
        public async Task<DataResponse> get_xls()
        {

            //1-avsec.xlsx
            var reader = new AttendanceExcelReader();
            var course = reader.ReadCourseFromExcel(@"C:\Users\vahid\Desktop\ava\flykish\1-avsec.xlsx");

            return new DataResponse()
            {
                Data = new
                {
                    xls = 1,

                },
                IsSuccess = true
            };
        }


        public class Student2
        {
            public int No { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }

            public string key { get; set; }
        }
        public class Session
        {
            public string Date { get; set; }
            public string Start { get; set; }
            public string End { get; set; }

            public DateTime? DateStart { get; set; }
            public DateTime? DateEnd { get; set; }
            public string key { get; set; }

            public override string ToString()
            {
                return $"{{ date = \"{Date}\", start = \"{Start}\", end = \"{End}\" }}";
            }
        }

        static List<List<string>> SplitListBySig(List<string> input)
        {
            List<List<string>> result = new List<List<string>>();
            List<string> current = new List<string>();

            foreach (var item in input)
            {
                if (item == "Signature")
                {
                    if (current.Count > 0)
                    {
                        result.Add(new List<string>(current));
                        current.Clear();
                    }
                }
                else
                {
                    current.Add(item);
                }
            }

            if (current.Count > 0)
            {
                result.Add(current);
            }

            return result;
        }

        public class fly_course
        {
            public string title { get; set; }
            public DateTime? date_start { get; set; }
            public DateTime? date_end { get; set; }

            public int? duration { get; set; }
            public int? days { get; set; }
            public string instructors { get; set; }
            public List<Session> sessions { get; set; }
            public List<Student2> students { get; set; }
            public string key { get; set; }
        }
        [Route("api/doc")]
        public async Task<DataResponse> get_doc()
        {
            string folderPath = @"C:\Users\vahid\Desktop\ava\flykish";
            string[] docxFiles = Directory.GetFiles(folderPath, "*.docx");
            List<fly_course> courses = new List<fly_course>();
            Console.WriteLine("فایل‌های موجود در پوشه:");
            List<string> errors = new List<string>();
            foreach (string file in docxFiles)
            {
                // Console.WriteLine(Path.GetFileName(file)); // فقط نام فایل بدون مسیر
                //string filePath = @"C:\Users\vahid\Desktop\ava\flykish\" + "فرم  حضورغیاب fly kish Annoucment" + ".docx";
                string filePath = (file);
                List<Student2> students = new List<Student2>();
                // List<Session> sessions = new List<Session>();
                List<string> days = new List<string>();
                List<string> sessions = new List<string>();
                List<string> day_sessions = new List<string>();
                List<Session> c_sessions = new List<Session>();
                fly_course course = new fly_course() { key = filePath };
                try
                {
                    using (DocX document = DocX.Load(filePath))
                    {
                        string text = document.Text;

                        string courseTitle = ExtractField(text, @"Course Title:\s*(.+?)\s*Department");
                        string startingDate = ExtractField(text, @"Starting Date:\s*([^\s]+)");
                        string endingDate = ExtractField(text, @"Ending Date:\s*([^\s]+)");
                        string duration = ExtractField(text, @"Duration:\s*(.+?)\s*Hrs");  //……20……/……3……
                        string instructor = ExtractField(text, @"Instructor's Name:\s*(.+)");

                        course.date_start = ConvertPersianDateToGregorian(startingDate);
                        course.date_end = ConvertPersianDateToGregorian(endingDate);
                        var d_prts = duration.Replace(".", "").Replace(" ", "").Split('/');
                        course.duration = Convert.ToInt32(duration.Replace("…", "").Replace(".", "").Replace(" ", "").Split('/')[0]);
                        course.days = Convert.ToInt32(duration.Replace("…", "").Replace(".", "").Split('/')[1]);
                        course.instructors = String.Join(", ", instructor.Split('&').Select(q => q.ToUpper()).ToList());

                        Console.WriteLine("Course Title: " + courseTitle);
                        Console.WriteLine("Starting Date: " + startingDate);
                        Console.WriteLine("Ending Date: " + endingDate);
                        Console.WriteLine("Duration: " + duration);
                        Console.WriteLine("Instructor's Name: " + instructor);
                        foreach (var table in document.Tables)
                        {
                            // بررسی اینکه جدول حداقل 3 ستون دارد (No, Name (per), Name (eng))
                            int _r = 0;
                            foreach (var row in table.Rows)
                            {
                                if (row.Cells.Count >= 3 &&
                                    int.TryParse(row.Cells[0].Paragraphs[0].Text.Trim(), out int no))
                                {
                                    string namePer = row.Cells[1].Paragraphs[0].Text.Trim();
                                    string nameEng = row.Cells[2].Paragraphs[0].Text.Trim();

                                    students.Add(new Student2
                                    {
                                        No = no,
                                        first_name = namePer,
                                        last_name = nameEng,
                                        key = filePath
                                    });

                                    var hrow = table.Rows[_r - 2];
                                    var srow = table.Rows[_r - 1];
                                    foreach (var c in hrow.Cells)
                                    {
                                        string str = c.Paragraphs[0].Text;
                                        if (str != "No" && str != "Name")
                                            days.Add(str);
                                    }
                                    if (day_sessions.Count == 0)
                                    {
                                        foreach (var c in srow.Cells)
                                        {
                                            string str = c.Paragraphs[0].Text;
                                            if (!string.IsNullOrEmpty(str) && str != "Signature")
                                            {
                                                var strs = str.Split('-');
                                                if (!strs[0].Contains(":"))
                                                    strs[0] += ":00";
                                                if (strs[0].Length < 5)
                                                    strs[0] = "0" + strs[0];

                                                if (!strs[1].Contains(":"))
                                                    strs[1] += ":00";
                                                if (strs[1].Length < 5)
                                                    strs[1] = "0" + strs[1];

                                                sessions.Add(strs[0] + "-" + strs[1]);
                                            }
                                        }
                                        // List<List<string>> result_sessions = SplitListBySig(sessions);

                                        int _di = 0;
                                        int _si = 0;
                                        Int64 _cs = -1;
                                        foreach (var session in sessions)
                                        {
                                            var _i6 = Convert.ToInt64(session.Replace("-", "").Replace(":", ""));
                                            if (_cs >= _i6)
                                                _di++;
                                            var _day = days[_di];
                                            day_sessions.Add(_day + " " + session);
                                            var c_session = (new Session()
                                            {
                                                DateStart = ConvertPersianDateToGregorian(_day),
                                                DateEnd = ConvertPersianDateToGregorian(_day),
                                            });

                                            if (c_session.DateStart == null)
                                            {
                                                c_session.DateStart = ConvertPersianDateToGregorian2(_day);
                                                c_session.DateEnd = ConvertPersianDateToGregorian2(_day);
                                            }
                                            if (c_session.DateStart != null)
                                            {
                                                var _ss = session.Split('-')[0];
                                                var _se = session.Split('-')[1];
                                                c_session.DateStart = ((DateTime)c_session.DateStart).AddHours(Convert.ToInt32(_ss.Split(':')[0]))
                                                    .AddMinutes(Convert.ToInt32(_ss.Split(':')[1]));

                                                c_session.DateEnd = ((DateTime)c_session.DateEnd).AddHours(Convert.ToInt32(_se.Split(':')[0]))
                                                    .AddMinutes(Convert.ToInt32(_se.Split(':')[1]));
                                                c_session.key = filePath;
                                                c_sessions.Add(c_session);

                                            }
                                            _cs = Convert.ToInt64(session.Replace("-", "").Replace(":", ""));

                                        }
                                    }


                                }
                                _r++;
                            }
                        }

                        course.sessions = c_sessions;
                        course.students = students;
                        courses.Add(course);

                        //foreach (var table in document.Tables)
                        //{
                        //    // فرض: ردیف اول شامل تاریخ‌ها است، ردیف دوم شامل ساعت‌ها
                        //    if (table.RowCount >= 3)
                        //    {
                        //        var dateRow = table.Rows[0]; // یا 1 اگر سطر اول عنوان است
                        //        var timeRow = table.Rows[1];

                        //        string currentDate = null;

                        //        for (int i = 0; i < dateRow.Cells.Count; i++)
                        //        {
                        //            var dateText = dateRow.Cells[i].Paragraphs[0].Text.Trim();
                        //            if (Regex.IsMatch(dateText, @"\d{4}-\d{2}-\d{2}")) // تشخیص تاریخ
                        //            {
                        //                currentDate = dateText;
                        //            }

                        //            var timeText = timeRow.Cells[i].Paragraphs[0].Text.Trim();
                        //            var match = Regex.Match(timeText, @"(\d{2}:\d{2})-(\d{2}:\d{2})");
                        //            if (currentDate != null && match.Success)
                        //            {
                        //                sessions.Add(new Session
                        //                {
                        //                    Date = currentDate,
                        //                    Start = match.Groups[1].Value,
                        //                    End = match.Groups[2].Value
                        //                });
                        //            }
                        //        }

                        //        break; // اولین جدول کافی است
                        //    }
                        //}




                    }
                }
                catch(Exception ex)
                {
                    var msg =filePath+"    "+ ex.Message;
                    if (ex.InnerException != null)
                        msg += "    " + ex.InnerException.Message;
                    errors.Add(msg);
                }
               
            }
            FDMEntities context = new FDMEntities();
            foreach (var c in courses)
            {
                context.fly_course.Add(new Models.fly_course()
                {
                    date_end = c.date_end,
                    date_start = c.date_start,
                    days = c.days,
                    duration = c.duration,
                    instructors = c.instructors,
                    key = c.key,
                    title = c.title,
                });
                foreach (var s in c.sessions)
                {
                    context.fly_course_session.Add(new fly_course_session()
                    {
                        date_end = s.DateEnd,
                        date_start = s.DateStart,
                        key = s.key,
                    });
                }
                foreach (var s in c.students)
                {
                    context.fly_course_student.Add(new fly_course_student()
                    {
                         key = s.key,
                          first_name = s.first_name,
                           last_name = s.last_name,
                            row_no =   s.No.ToString(),
                    });
                }
            }
            context.SaveChanges();

            return new DataResponse()
            {
                Data = new
                {
                    courses,
                    errors
                },
                IsSuccess = true
            };
        }

        static string ExtractField(string text, string pattern)
        {
            Match match = Regex.Match(text, pattern);
            return match.Success ? match.Groups[1].Value.Trim() : "Not Found";
        }







    }
}
