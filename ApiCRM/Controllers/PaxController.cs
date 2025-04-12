using ApiCRM.Models;
using ApiCRM.ViewModels;
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

namespace ApiCRM.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PaxController : ApiController
    {
        public class temp_resul
        {
            public int id { get; set; }
            public string value { get; set; }
        }
        [Route("api/pax/flight/notify/{id}")]
        public async Task<IHttpActionResult> GetPaxFlightNotify(int id)
        {
            ppa_entities context = new ppa_entities();
            var pax_flight = await context.crm_pax_flight.Where(q => q.flight_id == id).ToListAsync();
            var pax_flight_view = await context.view_crm_pax_flight.Where(q => q.flight_id == id).ToListAsync();
            MelliPayamak sender = new MelliPayamak();
            List<string> refs = new List<string>();
            foreach (var p in pax_flight_view)
            {
                List<string> strs = new List<string>();
                strs.Add("مسافر گرامی "+p.first_name2+" "+p.last_name2+" "+"عزیز،");
                strs.Add("مفتخریم که میزبان شما بودیم. خواهشمندیم با شرکت در نظر سنجی شرکت هواپیمایی آوا، ما را در ارائه خدمات بهتر یاری کنید.");
                strs.Add("https://crm.airpocket.app/?id="+p.id);
                strs.Add("لینک ارسال شده تا 24 ساعت اعتبار دارد");
                var message= string.Join("\r\n", strs);
                sms_obj dto = new sms_obj() { 
                 message=message,
                  mobile=p.mobile,
                   name= p.first_name2 + " " + p.last_name2
                };
               // if (p.passenger_id == 86) 
               // {
                    var result = sender.send(dto);
                    var item = pax_flight.Where(q => q.id == p.id).FirstOrDefault();
                    item.status = "";
                    item.ref_id = result;
                    item.date_send = DateTime.Now;
                    refs.Add(result);
                //}
               
            }
             await context.SaveChangesAsync();
            return Ok(refs);


        }

        [Route("api/pax/flight/view/{id}")]
        public async Task<IHttpActionResult> GetPaxFlightView(int id)
        {
            ppa_entities context = new ppa_entities();
            var result = await context.view_crm_pax_flight.Where(q => q.id == id).FirstOrDefaultAsync();
            var questions = await context.crm_question_result.Where(q => q.flight_pax_id == id).Select(q=>new { q.score,q.question_id }).ToListAsync();
            var positions = new List<string>() { "CCM", "SCCM","ISCCM","CCI","CCE" };

            var crews = await context.view_crew_score.Where(q => q.flight_id == result.flight_id && q.flight_pax_id==result.id && positions.Contains(q.Position)).OrderBy(q => q.GroupOrder)

                .ToListAsync();

            return Ok(new
            {
                result,
                std_local = ((DateTime)result.std_local).ToString("HH:mm"),
                crews,
                questions

            });
        }

        public class score_dto
        {
            public int obj_id { get; set; }
            public int score { get; set; }
        }
        public class fp_dto
        {
            public int id { get; set; }
            public string comments { get; set; }
            public List<score_dto> crew_scores { get; set; }
            public List<score_dto> flight_scores { get; set; }
        }

        [Route("api/pax/flight/save")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostPaxFlightScore(fp_dto dto)
        {
            ppa_entities context = new ppa_entities();
            var flight_pax = await context.crm_pax_flight.Where(q => q.id == dto.id).FirstOrDefaultAsync();

            var questions = await context.crm_question_result.Where(q => q.flight_pax_id == dto.id).ToListAsync();
            var crews = await context.crm_crew_score.Where(q => q.flight_pax_id == dto.id).ToListAsync();

            context.crm_crew_score.RemoveRange(crews);
            context.crm_question_result.RemoveRange(questions);

            flight_pax.comments = dto.comments;
            flight_pax.date_result = DateTime.Now;

            foreach (var x in dto.crew_scores)
            {
                context.crm_crew_score.Add(new crm_crew_score()
                {
                    crew_id = x.obj_id,
                    flight_pax_id = dto.id,
                    score = x.score
                });
            }

            foreach (var x in dto.flight_scores)
            {
                context.crm_question_result.Add(new crm_question_result()
                {
                    flight_pax_id = dto.id,
                    question_id = x.obj_id,
                    score = x.score,
                });
            }

            await context.SaveChangesAsync();

            return Ok(true);
        }




    }
}
