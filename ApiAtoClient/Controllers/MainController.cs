using ApiAtoClient.Models;
using ApiAtoClient.ViewModels;
using Newtonsoft.Json;
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

namespace ApiAtoClient.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class MainController : ApiController
    {

        [Route("api/ato/client/exam/{id}/{client_id}")]
        public async Task<IHttpActionResult> GetClientExamById(int id, int client_id)
        {
            try
            {

                ppa_entities context = new ppa_entities();
                var exam=await context.view_trn_exam.Where(q=>q.id==id).FirstOrDefaultAsync();
                var _questions = await context.view_trn_exam_question_person.Where(q => q.exam_id == id && q.person_id == client_id).ToListAsync();

                var qids=_questions.Select(q=>(Nullable<int>) q.id).ToList();

                var questions= _questions.Select(q => JsonConvert.DeserializeObject<view_trn_exam_question_person_dto>(JsonConvert.SerializeObject(q))).ToList();
                var answers = await context.trn_answers.Where(q => qids.Contains(q.quesion_id)).ToListAsync();
                foreach(var q in questions)
                {
                    q.answers = answers.Where(x=>x.quesion_id==q.id).ToList();
                }


                var result = new DataResponse()
                {
                    Data = new
                    {
                         exam,
                         questions
                    },
                    IsSuccess = true,
                };


                return Ok(result);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                var result = new DataResponse()
                {
                    IsSuccess = false,
                    Errors = new List<string>() { msg }
                };


                return Ok(result);

            }

        }


        [Route("api/ato/client/exam/anwser/save/{client_id}/{question_id}/{answer_id}")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostAnser(  int client_id,int question_id,int answer_id)
        {
            ppa_entities context = new ppa_entities();
            var answer = await context.trn_exam_student_answer.FirstOrDefaultAsync(q => q.question_id == question_id && q.person_id == client_id);
            if (answer == null)
            {
                answer=new trn_exam_student_answer() { person_id = client_id , question_id= question_id };
                context.trn_exam_student_answer.Add(answer);
            }
            answer.answer_id = answer_id;
            answer.date_sent = DateTime.Now;
            await context.SaveChangesAsync();
            var result = new DataResponse()
            {
                IsSuccess = true,
                 
            };
            return Ok(result);
        }







    }
}
