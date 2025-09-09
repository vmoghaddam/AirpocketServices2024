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

        public  List<view_trn_exam_question_person> ShuffleQuestionsBasedOnId(List<view_trn_exam_question_person> questions, int client_id)
        {
            Random random = new Random(client_id);
            return questions.OrderBy(q => random.Next()).ToList();
        }
        [Route("api/ato/client/exam/{id}/{client_id}")]
        public async Task<IHttpActionResult> GetClientExamById(int id, int client_id)
        {
            try
            {

                ppa_entities context = new ppa_entities();
                var profile=await context.view_trn_profile.Where(q=>q.Id==client_id).FirstOrDefaultAsync();
                // var exam=await context.view_trn_exam.Where(q=>q.id==id).FirstOrDefaultAsync();

                var exam = await context.view_trn_person_exam.Where(q => q.exam_id == id).FirstOrDefaultAsync();
                List<view_trn_exam_question_person> _questions = new List<view_trn_exam_question_person>();


                if (exam.status_id != 0)
                {
                    _questions = await context.view_trn_exam_question_person.Where(q => q.exam_id == id && q.person_id == client_id).OrderBy(q => q.category_id).ThenBy(q => q.question_id).ToListAsync();

                    _questions = ShuffleQuestionsBasedOnId(_questions, client_id);
                }
             
              //  var _qqq = _questions.Select(q => q.question_id).ToList();

                var qids=_questions.Select(q=>(Nullable<int>) q.question_id).ToList();

                var qids2=_questions.Select(q => (Nullable<int>)q.id).ToList();

                var questions= _questions.Select(q => JsonConvert.DeserializeObject<view_trn_exam_question_person_dto>(JsonConvert.SerializeObject(q))).ToList();
                var answers = await context.trn_answers.Where(q => qids.Contains(q.quesion_id)).ToListAsync();
                //var xxxxx= await context.view_trm_exam_student_answer.Where(q => q.person_id == client_id ).ToListAsync();
                var client_answers=await context.view_trm_exam_student_answer.Where(q=>q.person_id==client_id && qids2.Contains(q.question_id)).ToListAsync();

                var out_answers = new List<dto_answer_out>();
                foreach(var ans in answers)
                {
                    var _ans=new dto_answer_out();
                    _ans.id=ans.id;
                    _ans.answer_id = ans.id;
                    _ans.quesion_id = ans.quesion_id;
                   
                    _ans.is_rtl=ans.is_rtl;
                    _ans.persian_title=ans.persian_title;
                    _ans.english_title=ans.english_title;
                    _ans.is_selected = client_answers.FirstOrDefault(q => q.main_question_id == ans.quesion_id && q.answer_id==ans.id) != null;
                    if (exam.status_id == 2)
                        _ans.is_answer = ans.is_answer;
                    out_answers.Add(_ans);

                }
                foreach(var q in questions)
                {
                    q.answers = out_answers.Where(x=>x.quesion_id==q.question_id).ToList();
                    foreach (var x in q.answers)
                        x.exam_quesion_id = q.id;
                }


                var result = new DataResponse()
                {
                    Data = new
                    {
                        exam,
                        questions,
                        profile,
                        toal_questions = questions.Count,
                        total_answerd = client_answers.Count(),
                        total_remained= questions.Count- client_answers.Count(),
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


        [Route("api/ato/client/exam/admin/{id}/{client_id}")]
        public async Task<IHttpActionResult> GetClientExamByIdAdmin(int id, int client_id)
        {
            try
            {

                ppa_entities context = new ppa_entities();
                var profile = await context.view_trn_profile.Where(q => q.Id == client_id).FirstOrDefaultAsync();
                // var exam=await context.view_trn_exam.Where(q=>q.id==id).FirstOrDefaultAsync();

                var exam = await context.view_trn_person_exam.Where(q => q.exam_id == id).FirstOrDefaultAsync();
                List<view_trn_exam_question_person> _questions = new List<view_trn_exam_question_person>();


                if (exam.status_id != 0)
                {
                    _questions = await context.view_trn_exam_question_person.Where(q => q.exam_id == id && q.person_id == client_id).OrderBy(q => q.category_id).ThenBy(q => q.question_id).ToListAsync();

                    _questions = ShuffleQuestionsBasedOnId(_questions, client_id);
                }

                //  var _qqq = _questions.Select(q => q.question_id).ToList();

                var qids = _questions.Select(q => (Nullable<int>)q.question_id).ToList();

                var qids2 = _questions.Select(q => (Nullable<int>)q.id).ToList();

                var questions = _questions.Select(q => JsonConvert.DeserializeObject<view_trn_exam_question_person_dto>(JsonConvert.SerializeObject(q))).ToList();
                var answers = await context.trn_answers.Where(q => qids.Contains(q.quesion_id)).ToListAsync();
                //var xxxxx= await context.view_trm_exam_student_answer.Where(q => q.person_id == client_id ).ToListAsync();
                var client_answers = await context.view_trm_exam_student_answer.Where(q => q.person_id == client_id && qids2.Contains(q.question_id)).ToListAsync();

                var out_answers = new List<dto_answer_out>();
                foreach (var ans in answers)
                {
                    var _ans = new dto_answer_out();
                    _ans.id = ans.id;
                    _ans.answer_id = ans.id;
                    _ans.quesion_id = ans.quesion_id;

                    _ans.is_rtl = ans.is_rtl;
                    _ans.persian_title = ans.persian_title;
                    _ans.english_title = ans.english_title;
                    _ans.is_selected = client_answers.FirstOrDefault(q => q.main_question_id == ans.quesion_id && q.answer_id == ans.id) != null;
                    //if (exam.status_id == 2)
                        _ans.is_answer = ans.is_answer;
                    out_answers.Add(_ans);

                }
                foreach (var q in questions)
                {
                    q.answers = out_answers.Where(x => x.quesion_id == q.question_id).ToList();
                    foreach (var x in q.answers)
                        x.exam_quesion_id = q.id;
                }


                var result = new DataResponse()
                {
                    Data = new
                    {
                        exam,
                        questions,
                        profile,
                        toal_questions = questions.Count,
                        total_answerd = client_answers.Count(),
                        total_remained = questions.Count - client_answers.Count(),
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


        [Route("api/ato/client/exam/summary/{exam_id}/{client_id}")]
        public async Task<IHttpActionResult> GetClientExamSummary(int exam_id, int client_id)
        {
            try
            {

                ppa_entities context = new ppa_entities();
                var sum=await context.view_trn_exam_summary.FirstOrDefaultAsync(q=>q.person_id==client_id && q.exam_id==exam_id);


                var result = new DataResponse()
                {
                    Data = sum,
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
        public async Task<IHttpActionResult> PostAnser(  int client_id,int question_id,int answer_id )
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

        public class dto_profile
        {
            public string user_id { get; set; }
            public int person_id { get; set; }
        }
        [Route("api/ato/client/profile")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostProfile(dto_profile dto)
        {
            ppa_entities context = new ppa_entities();
            var profile = await context.view_trn_profile.FirstOrDefaultAsync(q => q.Id==dto.person_id);
            
            var result = new DataResponse()
            {
                IsSuccess = true,
                Data= profile

            };
            return Ok(result);
        }


        [Route("api/ato/client/exams/scheduled/{client_id}")]
        public async Task<IHttpActionResult> GetClientExamsById(  int client_id)
        {
            try
            {

                ppa_entities context = new ppa_entities();
                var exams = await context.view_trn_person_exam.Where(q => q.person_id==client_id && (q.status_id==0 || q.status_id == 1)).OrderBy(q=>q.exam_status_id).ThenByDescending(q=>q.exam_date_actual).ToListAsync();

                 

                var result = new DataResponse()
                {
                    Data = exams,
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





    }
}
