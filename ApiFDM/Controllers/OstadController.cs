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

namespace ApiFDM.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class OstadController : ApiController
    {
        dbEntities context = new dbEntities();
        [HttpGet]
        [Route("api/phase/cockpit/{ym1}/{ym2}")]
        public async Task<DataResponse> get_phase_cockpit(int ym1,int ym2)
        {

            //var d1 = new DateTime(year, month, 1);
            //var dates = new List<int?>();
            //dates.Add(Convert.ToInt32(d1.ToString("yyyyMM")));
            //for (var i = 0; i <= 11; i++)
            //{
            //    d1 = d1.AddMonths(-1);
            //    dates.Add(Convert.ToInt32(d1.ToString("yyyyMM")));
            //}
           /* if (m2==12)
            {
                m2 = 1;
                y2 += 1;
            }
            var dt1 = new DateTime(y1, m1, 1);
            var dt2= new DateTime(y2, m2, 1);
           */

            try
            {
                var query=from x in context.FDMPhaseDetailByEvents
                              // where x.Date>=dt1 && x.Date<dt2
                         where x.YearMonth>=ym1 && x.YearMonth<=ym2
                          select x;
                var query_crew= from x in context.FDMPhaseDetailCrewByEvents
                                     // where x.Date>=dt1 && x.Date<dt2
                                 where x.YearMonth >= ym1 && x.YearMonth <= ym2
                                 select x;
                var main_source=await query.ToListAsync();
                var main_source_crew = await query_crew.ToListAsync();
                var grp_phase = (from x in main_source
                                    group x by new {  x.Phase  } into grp
                                    select new
                                    {
                                       
                                        grp.Key.Phase,
                                        EventCount = grp.Sum(q => q.EventCount),
                                        HighCount = grp.Sum(q => q.HighCount),
                                        MediumCount = grp.Sum(q => q.MediumCount),
                                        LowCount = grp.Sum(q => q.LowCount),
                                        Score = grp.Sum(q => q.Score),
                                        HighScore = grp.Sum(q => q.HighScore),
                                        MediumScore = grp.Sum(q => q.MediumScore),
                                        LowScore = grp.Sum(q => q.LowScore)
                                    }).OrderByDescending(q => q.Score).ToList();
                var grp_phase_route = (from x in main_source
                                 group x by new { x.Phase , x.RouteICAO } into grp
                                 select new
                                 {

                                     grp.Key.Phase,
                                     grp.Key.RouteICAO,
                                     EventCount = grp.Sum(q => q.EventCount),
                                     HighCount = grp.Sum(q => q.HighCount),
                                     MediumCount = grp.Sum(q => q.MediumCount),
                                     LowCount = grp.Sum(q => q.LowCount),
                                     Score = grp.Sum(q => q.Score),
                                     HighScore = grp.Sum(q => q.HighScore),
                                     MediumScore = grp.Sum(q => q.MediumScore),
                                     LowScore = grp.Sum(q => q.LowScore),
                                     Items=grp.OrderByDescending(q => q.Score).ToList()
                                 }).OrderBy(q=>q.Phase).OrderByDescending(q => q.Score).ToList();
                var grp_phase_crew = (from x in main_source_crew
                                      group x by new { x.Phase, x.Id, x.Name,x.CrewId } into grp
                                       select new
                                       {

                                           grp.Key.Phase,
                                           grp.Key.Name,
                                           grp.Key.CrewId,
                                           EventCount = grp.Sum(q => q.EventCount),
                                           HighCount = grp.Sum(q => q.HighCount),
                                           MediumCount = grp.Sum(q => q.MediumCount),
                                           LowCount = grp.Sum(q => q.LowCount),
                                           Score = grp.Sum(q => q.Score),
                                           HighScore = grp.Sum(q => q.HighScore),
                                           MediumScore = grp.Sum(q => q.MediumScore),
                                           LowScore = grp.Sum(q => q.LowScore),
                                           Items = grp.OrderByDescending(q => q.Score).ToList()
                                       }).OrderBy(q => q.Phase).OrderByDescending(q => q.Score).ToList();
                var grp_phase_event = (from x in main_source
                                       group x by new { x.Phase, x.EventName } into grp
                                       select new
                                       {

                                           grp.Key.Phase,
                                           grp.Key.EventName,
                                           EventCount = grp.Sum(q => q.EventCount),
                                           HighCount = grp.Sum(q => q.HighCount),
                                           MediumCount = grp.Sum(q => q.MediumCount),
                                           LowCount = grp.Sum(q => q.LowCount),
                                           Score = grp.Sum(q => q.Score),
                                           HighScore = grp.Sum(q => q.HighScore),
                                           MediumScore = grp.Sum(q => q.MediumScore),
                                           LowScore = grp.Sum(q => q.LowScore),
                                           Items = grp.OrderByDescending(q => q.Score).ToList()
                                       }).OrderBy(q => q.Phase).OrderByDescending(q => q.Score).ToList();
                var grp_phase_event2 = (from x in grp_phase_event
                                        group x by new { x.Phase } into grp
                                        select new
                                        {
                                            grp.Key.Phase,
                                            Items = grp.ToList()
                                        }).ToList();
                var grp_phase_ym = (from x in main_source
                                    group x by new { x.Year, x.Month, x.YearMonth, x.Phase } into grp
                                    select new
                                    {
                                        grp.Key.Year,
                                        grp.Key.Month,
                                        grp.Key.YearMonth,
                                        grp.Key.Phase,
                                        EventCount = grp.Sum(q => q.EventCount),
                                        HighCount = grp.Sum(q => q.HighCount),
                                        MediumCount = grp.Sum(q => q.MediumCount),
                                        LowCount = grp.Sum(q => q.LowCount),
                                        Score = grp.Sum(q => q.Score),
                                        HighScore = grp.Sum(q => q.HighScore),
                                        MediumScore = grp.Sum(q => q.MediumScore),
                                        LowScore = grp.Sum(q => q.LowScore),
                                        Items=grp.OrderByDescending(q=>q.Score).ToList()
                                    }).OrderBy(q => q.Year).ThenBy(q => q.Month).ThenBy(q=>q.Score).ToList();
                var grp_phase_route_ym = (from x in main_source
                                    group x by new { x.Year, x.Month, x.YearMonth, x.Phase,x.RouteICAO } into grp
                                    select new
                                    {
                                        grp.Key.Year,
                                        grp.Key.Month,
                                        grp.Key.YearMonth,
                                        grp.Key.Phase,
                                        grp.Key.RouteICAO,
                                        EventCount = grp.Sum(q => q.EventCount),
                                        HighCount = grp.Sum(q => q.HighCount),
                                        MediumCount = grp.Sum(q => q.MediumCount),
                                        LowCount = grp.Sum(q => q.LowCount),
                                        Score = grp.Sum(q => q.Score),
                                        HighScore = grp.Sum(q => q.HighScore),
                                        MediumScore = grp.Sum(q => q.MediumScore),
                                        LowScore = grp.Sum(q => q.LowScore),
                                        Items = grp.OrderByDescending(q => q.Score).ToList()
                                    }).OrderBy(q => q.Year).ThenBy(q => q.Month).ThenBy(q=>q.Phase).ThenBy(q=>q.Score).ToList();
                var grp_phase_event_ym = (from x in main_source
                                          group x by new { x.Year, x.Month, x.YearMonth, x.Phase, x.EventName } into grp
                                          select new
                                          {
                                              grp.Key.Year,
                                              grp.Key.Month,
                                              grp.Key.YearMonth,
                                              grp.Key.Phase,
                                              grp.Key.EventName,
                                              EventCount = grp.Sum(q => q.EventCount),
                                              HighCount = grp.Sum(q => q.HighCount),
                                              MediumCount = grp.Sum(q => q.MediumCount),
                                              LowCount = grp.Sum(q => q.LowCount),
                                              Score = grp.Sum(q => q.Score),
                                              HighScore = grp.Sum(q => q.HighScore),
                                              MediumScore = grp.Sum(q => q.MediumScore),
                                              LowScore = grp.Sum(q => q.LowScore),
                                              Items = grp.OrderByDescending(q => q.Score).ToList()
                                          }).OrderBy(q => q.Year).ThenBy(q => q.Month).ThenBy(q => q.Phase).ThenBy(q => q.Score).ToList();
                var grp_phase_crew_ym = (from x in main_source_crew
                                         group x by new { x.Year, x.Month, x.YearMonth, x.Phase, x.CrewId ,x.Name} into grp
                                          select new
                                          {
                                              grp.Key.Year,
                                              grp.Key.Month,
                                              grp.Key.YearMonth,
                                              grp.Key.Phase,
                                              grp.Key.Name,
                                              grp.Key.CrewId,
                                              EventCount = grp.Sum(q => q.EventCount),
                                              HighCount = grp.Sum(q => q.HighCount),
                                              MediumCount = grp.Sum(q => q.MediumCount),
                                              LowCount = grp.Sum(q => q.LowCount),
                                              Score = grp.Sum(q => q.Score),
                                              HighScore = grp.Sum(q => q.HighScore),
                                              MediumScore = grp.Sum(q => q.MediumScore),
                                              LowScore = grp.Sum(q => q.LowScore),
                                              Items = grp.OrderByDescending(q => q.Score).ToList()
                                          }).OrderBy(q => q.Year).ThenBy(q => q.Month).ThenBy(q => q.Phase).ThenBy(q => q.Score).ToList();
                var event_names = main_source.Select(q => q.EventName).Distinct().OrderBy(q=>q).ToList();

                var result = new
                {
                    grp_phase,
                    grp_phase_crew,
                    grp_phase_route,
                    grp_phase_ym,
                    grp_phase_crew_ym,
                    grp_phase_route_ym,
                    grp_phase_event,
                    grp_phase_event_ym,
                    event_names,
                    grp_phase_event2
                };
                return new DataResponse()
                {
                    IsSuccess = true,
                    Data = result

                };
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg+="   INNER:"+ex.InnerException.Message;
                return new DataResponse()
                {
                    IsSuccess = false,
                    Data = ex

                };
            }
        }
    }
}
