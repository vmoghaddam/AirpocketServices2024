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

namespace ApiFDM.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class FDMV2Controller : ApiController
    {
        [Route("api/fdm/V2/events/{dt1}/{dt2}")]
        public async Task<DataResponse> get_events(DateTime dt1, DateTime dt2)
        {
            FDMEntities context = new FDMEntities();
            try
            {
                var xxxx = context.ViewMSNs.ToList();
            }
            catch(Exception ex)
            {
                int yyyyyy = 0;
            }
        
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
            var result_register1 = await query.ToListAsync();
            var _xx = await query_flights.ToListAsync();
            var result_register = (from q in _xx
                                   let matchedResult = result_register1.FirstOrDefault(w => w.register_id == q.register_id)
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

            var result_type = (from x in result_register
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


            return new DataResponse()
            {
                Data = new
                {
                    result_register,
                    result_type,

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
            var result_register_crew = (from q in _xx
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

            var result_type_crew = (from x in result_register_crew
                                    where x.crew_position == "CPT"
                                    group x by new { x.ac_type, x.crew_id, x.crew_name } into grp
                                    select new
                                    {
                                        grp.Key.ac_type,
                                        grp.Key.crew_id,
                                        grp.Key.crew_name,
                                        count = grp.Sum(q => q.event_count),
                                        high_count = grp.Sum(q => q.high_count),
                                        medium_count = grp.Sum(q => q.medium_count),
                                        low_count = grp.Sum(q => q.low_count),
                                        high_score = grp.Sum(q => q.high_score),
                                        medium_score = grp.Sum(q => q.medium_score),
                                        low_score = grp.Sum(q => q.low_score),
                                        total_score = grp.Sum(q => q.total_score),
                                        flight_count= grp.Sum(q => q.flight_count),
                                        score_per_event = Math.Round(grp.Sum(q => q.total_score) * 1.0 / grp.Sum(q => q.event_count), 1),
                                        score_per_flight = Math.Round(grp.Sum(q => q.total_score) * 1.0 / grp.Sum(q => q.flight_count), 1)
                                    }).ToList();


            return new DataResponse()
            {
                Data = new
                {
                    result_register_crew,
                    result_type_crew,

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
            var result_register_crew_route = (from q in _xx
                                              where q.crew_position == "CPT"
                                              let machedResult = result_register.FirstOrDefault(w => w.register_id == q.register_id && w.crew_id == q.crew_id && w.position == q.crew_position && w.dep_id == q.dep_id && w.arr_id == q.arr_id)
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
                                                  event_count = machedResult?.count ?? 0,
                                                  high_count = machedResult?.high_count ?? 0,
                                                  medium_count = machedResult?.medium_count ?? 0,
                                                  low_count = machedResult?.low_count ?? 0,
                                                  high_score = machedResult?.high_count * 4 ?? 0,
                                                  medium_score = machedResult?.medium_count * 2 ?? 0,
                                                  low_score = machedResult?.low_count * 1 ?? 0,
                                                  total_score = machedResult?.total_score ?? 0
                                              }).ToList();

            var result_type_crew_route = (from x in result_register_crew_route
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
                    result_register_crew_route,
                    result_type_crew_route,

                },
                IsSuccess = true
            };

        }

        [Route("api/fdm/V2/eventsRoute/{dt1}/{dt2}")]
        public async Task<DataResponse> get_events_route(DateTime dt1, DateTime dt2)
        {
            FDMEntities context = new FDMEntities();
            dt2 = dt2.AddDays(1);

            var query = from x in context.view_fdm_processed
                        where x.std >= dt1 && x.std <= dt2
                        group x by new { x.register_id, x.register, x.ac_type2, x.dep_id, x.dep_iata, x.dep_icao, x.arr_id, x.arr_iata, x.arr_icao } into grp
                        select new
                        {
                            grp.Key.register_id,
                            grp.Key.register,
                            ac_type = grp.Key.ac_type2,
                            grp.Key.dep_id,
                            grp.Key.dep_iata,
                            grp.Key.dep_icao,
                            grp.Key.arr_id,
                            grp.Key.arr_iata,
                            grp.Key.arr_icao,
                            route = grp.Key.dep_iata + '_' + grp.Key.arr_iata,
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
                                group x by new { x.RegisterID, y.Register, y.AircraftType2, x.FromAirportId, x.ToAirportId } into grp
                                select new
                                {
                                    register_id = grp.Key.RegisterID,
                                    register = grp.Key.Register,
                                    ac_type = grp.Key.AircraftType2,
                                    grp.Key.FromAirportId,
                                    grp.Key.ToAirportId,
                                    count = grp.Count()
                                };
            var result_register1 = await query.ToListAsync();
            var _xx = await query_flights.ToListAsync();
            var result_register_route = (from q in _xx
                                         let matchedResult = result_register1.FirstOrDefault(w => w.register_id == q.register_id && w.dep_id == q.FromAirportId && w.arr_id == q.ToAirportId)
                                         select new
                                         {
                                             q.register_id,
                                             q.register,
                                             q.ac_type,
                                             matchedResult.dep_id,
                                             matchedResult.dep_icao,
                                             matchedResult.dep_iata,
                                             matchedResult.arr_id,
                                             matchedResult.arr_iata,
                                             matchedResult.arr_icao,
                                             route = matchedResult.dep_iata + '-' + matchedResult.arr_iata,
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


            var result_type_route = (from x in result_register_route
                                     group x by new { x.ac_type, x.arr_iata, x.dep_iata, x.route } into grp
                                     select new
                                     {
                                         grp.Key.ac_type,
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
                    result_register_route,
                    result_type_route,

                },
                IsSuccess = true
            };

        }


        [Route("api/fdm/V2/eventsPhase/{dt1}/{dt2}")]
        public async Task<DataResponse> get_events_phase(DateTime dt1, DateTime dt2)
        {
            FDMEntities context = new FDMEntities();
            dt2 = dt2.AddDays(1);

            var query = from x in context.view_fdm_processed
                        where x.std >= dt1 && x.std <= dt2
                        group x by new { x.register_id, x.register, x.ac_type2, x.phase } into grp
                        select new
                        {
                            grp.Key.register_id,
                            grp.Key.register,
                            ac_type = grp.Key.ac_type2,
                            grp.Key.phase,
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
            var result_register1 = await query.ToListAsync();
            var _xx = await query_flights.ToListAsync();
            var result_register_phase = (from q in result_register1
                                         let matchedResult = _xx.FirstOrDefault(w => w.register_id == q.register_id)
                                         select new
                                         {
                                             q.register_id,
                                             q.register,
                                             q.ac_type,
                                             q.phase,
                                             flight_count = matchedResult.count,
                                             event_count = q?.count ?? 0,
                                             high_count = q?.high_count ?? 0,
                                             medium_count = q?.medium_count ?? 0,
                                             low_count = q?.low_count ?? 0,
                                             high_score = (q?.high_count ?? 0) * 4,
                                             medium_score = (q?.medium_count ?? 0) * 2,
                                             low_score = (q?.low_count ?? 0) * 1,
                                             total_score = q?.total_score ?? 0,
                                             score_per_event = q != null && q.count > 0
                                                                  ? Math.Round((double)q.total_score / q.count, 1)
                                                                  : 0,
                                             score_per_flight = matchedResult.count > 0
                                                    ? Math.Round((double)(q?.total_score ?? 0) / matchedResult.count, 1)
                                                    : 0
                                         }).ToList();


            var result_type_phase = (from x in result_register_phase
                                     group x by new { x.ac_type, x.phase } into grp
                                     select new
                                     {
                                         grp.Key.ac_type,
                                         grp.Key.phase,
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
                    result_register_phase,
                    result_type_phase,

                },
                IsSuccess = true
            };

        }
        [Route("api/fdm/V2/eventsCrewPhase/{dt1}/{dt2}")]
        public async Task<DataResponse> get_events_captain_phase(DateTime dt1, DateTime dt2)
        {
            FDMEntities context = new FDMEntities();
            dt2 = dt2.AddDays(1);
            var query = from x in context.view_fdm_processed
                        join y in context.fdm_crew on x.id equals y.processed_id
                        where x.std >= dt1 && x.std <= dt2
                        group x by new { x.register_id, x.register, x.ac_type2, y.crew_id, y.first_name, y.last_name, y.name, y.rank, y.rank_code, y.position, x.phase } into grp
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
                            grp.Key.phase,
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
            var _ee = await query.ToListAsync();
            var _ff = await query_flights.ToListAsync();
            var result_register_crew_phase = (from q in _ee
                                              where q.position == "CPT"
                                              let matchedResult = _ff.FirstOrDefault(w => w.register_id == q.register_id && w.crew_id == q.crew_id && w.crew_position == q.position)
                                              select new
                                              {
                                                  q.register_id,
                                                  q.register,
                                                  q.ac_type,
                                                  q.crew_id,
                                                  crew_name = q.name,
                                                  crew_job_group = matchedResult.crew_job_group,
                                                  crew_job_group_code = matchedResult.crew_job_group_code,
                                                  crew_position = q.position,
                                                  q.phase,
                                                  flight_count = matchedResult.count,
                                                  event_count = q?.count ?? 0,
                                                  high_count = q?.high_count ?? 0,
                                                  medium_count = q?.medium_count ?? 0,
                                                  low_count = q?.low_count ?? 0,
                                                  high_score = q?.high_count * 4 ?? 0,
                                                  medium_score = q?.medium_count * 2 ?? 0,
                                                  low_score = q?.low_count * 1 ?? 0,
                                                  total_score = q?.total_score ?? 0
                                              }).ToList();

            var result_type_crew_phase = (from x in result_register_crew_phase
                                          where x.crew_position == "CPT"
                                          group x by new { x.ac_type, x.crew_name, x.phase ,x.crew_id} into grp
                                          select new
                                          {
                                              
                                              grp.Key.crew_id,
                                              grp.Key.ac_type,
                                              grp.Key.crew_name,
                                              grp.Key.phase,
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
                    result_register_crew_phase,
                    result_type_crew_phase,

                },
                IsSuccess = true
            };

        }
        [Route("api/fdm/V2/eventsCrewRoutePhase/{dt1}/{dt2}")]
        public async Task<DataResponse> get_events_captain_route_phase(DateTime dt1, DateTime dt2)
        {
            FDMEntities context = new FDMEntities();
            dt2 = dt2.AddDays(1);
            var query = from x in context.view_fdm_processed
                        join y in context.fdm_crew on x.id equals y.processed_id
                        where x.std >= dt1 && x.std <= dt2
                        group x by new { x.register_id, x.register, x.ac_type2, x.phase, y.crew_id, y.first_name, y.last_name, y.name, y.rank, y.rank_code, y.position, x.dep_id, x.dep_iata, x.dep_icao, x.arr_id, x.arr_iata, x.arr_icao } into grp
                        select new
                        {
                            grp.Key.register_id,
                            grp.Key.register,
                            ac_type = grp.Key.ac_type2,
                            grp.Key.phase,
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
            var _ee = await query.ToListAsync();
            var _ff = await query_flights.ToListAsync();
            var result_register_crew_route_phase = (from q in _ee
                                                    where q.position == "CPT"
                                                    let matchedResult = _ff.FirstOrDefault(w => w.register_id == q.register_id && w.crew_id == q.crew_id && w.crew_position == q.position && w.arr_id == q.arr_id && w.dep_id == q.dep_id)

                                                    select new
                                                    {
                                                        q.register_id,
                                                        q.register,
                                                        q.ac_type,
                                                        q.phase,
                                                        q.crew_id,
                                                        crew_name = q.name,
                                                        crew_job_group = matchedResult.crew_job_group,
                                                        crew_job_group_code = matchedResult.crew_job_group_code,
                                                        crew_position = q.position,
                                                        q.dep_id,
                                                        q.dep_iata,
                                                        q.dep_icao,
                                                        q.arr_id,
                                                        q.arr_iata,
                                                        q.arr_icao,
                                                        route = q.dep_iata + '_' + q.arr_iata,
                                                        flight_count = matchedResult.count,
                                                        event_count = q?.count ?? 0,
                                                        high_count = q?.high_count ?? 0,
                                                        medium_count = q?.medium_count ?? 0,
                                                        low_count = q?.low_count ?? 0,
                                                        high_score = q?.high_count * 4 ?? 0,
                                                        medium_score = q?.medium_count * 2 ?? 0,
                                                        low_score = q?.low_count * 1 ?? 0,
                                                        total_score = q?.total_score ?? 0
                                                    }).ToList();

            var result_type_crew_route_phase = (from x in result_register_crew_route_phase
                                                where x.crew_position == "CPT"
                                                group x by new { x.ac_type, x.crew_name, x.arr_iata, x.dep_iata, x.route, x.phase } into grp
                                                select new
                                                {
                                                    grp.Key.ac_type,
                                                    grp.Key.phase,
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
                    result_register_crew_route_phase,
                    result_type_crew_route_phase,

                },
                IsSuccess = true
            };

        }


    }
}
