using ApiQA.Models;
using ApiQA.ViewModels;
using System;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Net.Http.Headers;

namespace ApiQA.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class surveyController : ApiController
    {
        ppa_entities context = new ppa_entities();
        public class DataResponse
        {
            public bool IsSuccess { get; set; }
            public object Data { get; set; }
            public List<string> Errors { get; set; }
        }
        public class dto_survey
        {
            public int id { get; set; }
            public string participant_name { get; set; }
            public string email { get; set; }
            public string phone { get; set; }
            public Nullable<int> participant_id { get; set; }
            public string participant_key { get; set; }
            public Nullable<System.DateTime> submitted_at { get; set; }

            public string driver_name { get; set; }
            public string journey_route { get; set; }
            public string journey_subject { get; set; }
            public string journey_title { get; set; }
            public string journey_time { get; set; }
            public string vehicle_no { get; set; }
            public string remark { get; set; }
            public Nullable<int> date_year { get; set; }
            public Nullable<int> date_month { get; set; }
            public Nullable<int> date_day { get; set; }
            public Dictionary<string,string> selected_statuses { get; set; }


        }
        
        [HttpPost]
        [Route("api/qa/survey/driver")]
        public async Task<DataResponse> SaveSurvey(dto_survey dto)
        {
            try
            {
                var entity = await context.survey_participant.FirstOrDefaultAsync(q => dto.participant_key == dto.participant_key);
                if (entity == null)
                {
                    entity = new survey_participant()
                    {
                        participant_key = dto.participant_key,
                        participant_id = dto.participant_id
                    };
                    context.survey_participant.Add(entity);
                }
                entity.participant_name = dto.participant_name;
                entity.phone = dto.phone;
                entity.email = dto.email;
                entity.submitted_at = DateTime.Now;

                var driver = await context.survey_driver.FirstOrDefaultAsync(q => q.survey_id == entity.id);
                if (driver == null)
                {
                    driver = new survey_driver();
                    entity.survey_driver.Add(driver);
                }

                driver.journey_route = dto.journey_route;
                driver.journey_subject = dto.journey_subject;
                driver.journey_time = dto.journey_time;
                driver.driver_name = dto.driver_name;
                driver.remark = dto.remark;
                driver.date_day = dto.date_day;
                driver.date_month = dto.date_month;
                driver.date_year = dto.date_year;

                var _result = await context.survey_result.Where(q => q.participant_id == entity.id).ToListAsync();
                if (_result != null && _result.Count > 0)
                    context.survey_result.RemoveRange(_result);


                foreach (var x in dto.selected_statuses)
                {
                    var key = x.Key;
                    var value = x.Value;
                    var question_id = Convert.ToInt32(key.Split('_')[1]);
                    entity.survey_result.Add(new survey_result()
                    {
                        answer = string.IsNullOrEmpty(value) ? null : (Nullable<int>)Convert.ToInt32(value),
                        question_id = question_id

                    });
                }








               await context.SaveChangesAsync();

                return new DataResponse() { Data = entity.id, IsSuccess = true };

            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                return new DataResponse()
                {
                    Data = msg,
                    IsSuccess = true
                };
            }
        }




    }
}
