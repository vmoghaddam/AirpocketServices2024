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
    public class FDMV2EventsController : ApiController
    {
        [Route("api/fdm/V2/events/{dt1}/{dt2}")]
        public async Task<DataResponse> get_events(DateTime dt1, DateTime dt2)
        {
            FDMEntities context = new FDMEntities();
            dt2 = dt2.AddDays(1);
            var events_total = (from x in context.view_fdm_processed
                                where x.std >= dt1 && x.std <= dt2 && x.register != "CNL"
                                select x).ToList();
            var events = (from x in events_total
                          group x by new { x.event_name } into grp
                          select new
                          {
                              grp.Key.event_name,
                              count = grp.Count()
                          }).OrderByDescending(q => q.count).ToList();
            var events_phase = (from x in events_total
                                group x by new { x.event_name, x.phase } into grp
                                select new
                                {
                                    grp.Key.event_name,
                                    count = grp.Count()
                                }).OrderByDescending(q => q.count).ToList();

            var events_type = (from x in events_total
                               group x by new { x.event_name, x.ac_type2 } into grp
                               select new
                               {
                                   grp.Key.event_name,
                                   ac_type = grp.Key.ac_type2,
                                   count = grp.Count()
                               }).OrderByDescending(q => q.count).ToList();
            var events_type_phase = (from x in events_total
                                     group x by new { x.event_name, x.ac_type2, x.phase } into grp
                                     select new
                                     {
                                         grp.Key.event_name,
                                         ac_type = grp.Key.ac_type2,
                                         grp.Key.phase,
                                         count = grp.Count()
                                     }).OrderByDescending(q => q.count).ToList();
            var events_register = (from x in events_total
                                   group x by new { x.event_name, x.ac_type2, x.register, x.register_id } into grp
                                   select new
                                   {
                                       grp.Key.event_name,
                                       ac_type = grp.Key.ac_type2,
                                       grp.Key.register_id,
                                       grp.Key.register,
                                       count = grp.Count()
                                   }).OrderByDescending(q => q.count).ToList();
            var events_register_phase = (from x in events_total
                                         group x by new { x.event_name, x.ac_type2, x.phase, x.register, x.register_id } into grp
                                         select new
                                         {
                                             grp.Key.event_name,
                                             ac_type = grp.Key.ac_type2,
                                             grp.Key.phase,
                                             grp.Key.register_id,
                                             grp.Key.register,
                                             count = grp.Count()
                                         }).OrderByDescending(q => q.count).ToList();




            var events_crew = (from x in events_total
                               join y in context.fdm_crew on x.id equals y.processed_id
                               group x by new { x.event_name, x.ac_type2, y.crew_id, y.first_name, y.last_name, y.name, y.rank, y.rank_code, y.position } into grp
                               select new
                               {
                                   grp.Key.event_name,
                                   ac_type = grp.Key.ac_type2,
                                   grp.Key.crew_id,
                                   grp.Key.first_name,
                                   grp.Key.last_name,
                                   grp.Key.name,
                                   grp.Key.rank,
                                   grp.Key.rank_code,
                                   grp.Key.position,
                                   count = grp.Count()
                               }).OrderByDescending(q => q.count).ToList();
            var events_crew_phase = (from x in events_total
                                     join y in context.fdm_crew on x.id equals y.processed_id
                                     group x by new { x.event_name, x.ac_type2, x.phase, y.crew_id, y.first_name, y.last_name, y.name, y.rank, y.rank_code, y.position } into grp
                                     select new
                                     {
                                         grp.Key.event_name,
                                         ac_type = grp.Key.ac_type2,
                                         grp.Key.crew_id,
                                         grp.Key.first_name,
                                         grp.Key.last_name,
                                         grp.Key.name,
                                         grp.Key.rank,
                                         grp.Key.rank_code,
                                         grp.Key.position,
                                         grp.Key.phase,
                                         count = grp.Count()
                                     }).OrderByDescending(q => q.count).ToList();

            var events_type_route = (from x in events_total
                                     group x by new { x.event_name, x.ac_type2, x.dep_id, x.dep_iata, x.dep_icao, x.arr_id, x.arr_iata, x.arr_icao } into grp
                                     select new
                                     {
                                         grp.Key.event_name,
                                         ac_type = grp.Key.ac_type2,
                                         grp.Key.dep_id,
                                         grp.Key.dep_iata,
                                         grp.Key.dep_icao,
                                         grp.Key.arr_id,
                                         grp.Key.arr_iata,
                                         grp.Key.arr_icao,
                                         route_iata = grp.Key.dep_iata + "-" + grp.Key.arr_iata,
                                         route_icao = grp.Key.dep_icao + "-" + grp.Key.arr_icao,
                                         count = grp.Count()
                                     }).OrderByDescending(q => q.count).ToList();
            var events_type_route_phase = (from x in events_total
                                           group x by new { x.event_name, x.ac_type2, x.phase, x.dep_id, x.dep_iata, x.dep_icao, x.arr_id, x.arr_iata, x.arr_icao } into grp
                                           select new
                                           {
                                               grp.Key.event_name,
                                               ac_type = grp.Key.ac_type2,
                                               grp.Key.dep_id,
                                               grp.Key.dep_iata,
                                               grp.Key.dep_icao,
                                               grp.Key.arr_id,
                                               grp.Key.arr_iata,
                                               grp.Key.arr_icao,
                                               route_iata = grp.Key.dep_iata + "-" + grp.Key.arr_iata,
                                               route_icao = grp.Key.dep_icao + "-" + grp.Key.arr_icao,
                                               grp.Key.phase,
                                               count = grp.Count()
                                           }).OrderByDescending(q => q.count).ToList();


            var events_type_route_crew = (from x in events_total
                                          join y in context.fdm_crew on x.id equals y.processed_id
                                          group x by new { x.event_name, x.ac_type2, x.dep_id, x.dep_iata, x.dep_icao, x.arr_id, x.arr_iata, x.arr_icao, y.crew_id, y.first_name, y.last_name, y.name, y.rank, y.rank_code, y.position } into grp
                                          select new
                                          {
                                              grp.Key.event_name,
                                              ac_type = grp.Key.ac_type2,
                                              grp.Key.dep_id,
                                              grp.Key.dep_iata,
                                              grp.Key.dep_icao,
                                              grp.Key.arr_id,
                                              grp.Key.arr_iata,
                                              grp.Key.arr_icao,
                                              route_iata = grp.Key.dep_iata + "-" + grp.Key.arr_iata,
                                              route_icao = grp.Key.dep_icao + "-" + grp.Key.arr_icao,
                                              grp.Key.crew_id,
                                              grp.Key.first_name,
                                              grp.Key.last_name,
                                              grp.Key.name,
                                              grp.Key.rank,
                                              grp.Key.rank_code,
                                              grp.Key.position,
                                              count = grp.Count()
                                          }).OrderByDescending(q => q.count).ToList();
            var events_type_route_phase_crew = (from x in events_total
                                                join y in context.fdm_crew on x.id equals y.processed_id
                                                group x by new { x.event_name, x.ac_type2, x.phase, x.dep_id, x.dep_iata, x.dep_icao, x.arr_id, x.arr_iata, x.arr_icao, y.crew_id, y.first_name, y.last_name, y.name, y.rank, y.rank_code, y.position } into grp
                                                select new
                                                {
                                                    grp.Key.event_name,
                                                    ac_type = grp.Key.ac_type2,
                                                    grp.Key.dep_id,
                                                    grp.Key.dep_iata,
                                                    grp.Key.dep_icao,
                                                    grp.Key.arr_id,
                                                    grp.Key.arr_iata,
                                                    grp.Key.arr_icao,
                                                    route_iata = grp.Key.dep_iata + "-" + grp.Key.arr_iata,
                                                    route_icao = grp.Key.dep_icao + "-" + grp.Key.arr_icao,
                                                    grp.Key.crew_id,
                                                    grp.Key.first_name,
                                                    grp.Key.last_name,
                                                    grp.Key.name,
                                                    grp.Key.rank,
                                                    grp.Key.rank_code,
                                                    grp.Key.position,
                                                    grp.Key.phase,
                                                    count = grp.Count()
                                                }).OrderByDescending(q => q.count).ToList();



            return new DataResponse()
            {
                Data = new
                {
                    events_total,
                    events,
                    events_phase,
                    events_type,
                    events_type_phase,
                    events_register,
                    events_register_phase,
                    events_crew,
                    events_crew_phase,
                    events_type_route,
                    events_type_route_phase,
                    events_type_route_crew,
                    events_type_route_phase_crew,

                },
                IsSuccess = true
            };


        }
    }
}
