using AirpocketTRN.Models;
using AirpocketTRN.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

using Antlr.Runtime;


using System.Data.Entity;

using System.Management.Instrumentation;
using System.Net.Http.Formatting;
using System.Security.Permissions;

using System.Web.Helpers;
using Newtonsoft.Json;

namespace AirpocketTRN.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ExamController : ApiController
    {
        CourseService courseService = null;

        public ExamController()
        {
            courseService = new CourseService();
        }

        public List<trn_questions> TakeRandomRows(List<trn_questions> list, int n)
        {
            // Create a new instance of Random
            Random random = new Random();

            // Order the list randomly and take n rows
            return list.OrderBy(x => random.Next()).Take(n).ToList();
        }
        [Route("api/trn/exam/status/")]
        [AcceptVerbs("Post")]
        public async Task<IHttpActionResult> post_exam_status(dynamic dto)
        {
            int exam_id = Convert.ToInt32(dto.exam_id);
            int status = Convert.ToInt32(dto.status);
            FLYEntities context = new FLYEntities();
            context.Configuration.LazyLoadingEnabled = false;
            var exam = await context.trn_exam.FirstOrDefaultAsync(q => q.id == exam_id);
            var old_status_id = exam.status_id;
            exam.status_id = status;
            switch (status)
            {
                case 0:
                    exam.date_start = null;
                    exam.date_end_actual = null;
                    exam.date_end_scheduled = ((DateTime)exam.exam_date).AddMinutes((int)exam.duration);
                    exam.is_running = false;
                    break;
                case 1:

                    exam.date_start = DateTime.Now;
                    exam.date_end_scheduled = ((DateTime)exam.date_start).AddMinutes((int)exam.duration);
                    exam.date_end_actual = null;
                    exam.is_running = true;


                    break;
                case 2:
                    exam.date_end_actual = DateTime.Now;
                    exam.is_running = false;
                    break;

                default:
                    break;

            }

            var sub_exams = await context.trn_person_exam.Where(q => q.main_exam_id == exam_id).ToListAsync();
            foreach (var x in sub_exams)
                x.status_id = exam.status_id;


            await context.SaveChangesAsync();
            var result = new DataResponse()
            {
                IsSuccess = true,
                Data = status,

            };
            return Ok(result);
        }

        [Route("api/trn/exam/pause/")]
        [AcceptVerbs("Post")]
        public async Task<IHttpActionResult> post_exam_pause(dynamic dto)
        {
            int exam_id = Convert.ToInt32(dto.exam_id);
             
            FLYEntities context = new FLYEntities();
            context.Configuration.LazyLoadingEnabled = false;
            var exam = await context.trn_exam.FirstOrDefaultAsync(q => q.id == exam_id);
            exam.is_running = false;
            exam.passed =Convert.ToInt32( Math.Round( (DateTime.Now - ((DateTime)exam.date_start)).TotalMinutes));
           

            var sub_exams = await context.trn_person_exam.Where(q => q.main_exam_id == exam_id).ToListAsync();
            foreach (var x in sub_exams)
            { 
                x.is_running = exam.is_running;
                x.passed = exam.passed;
            }


            await context.SaveChangesAsync();
            var result = new DataResponse()
            {
                IsSuccess = true,
                Data = exam.is_running,

            };
            return Ok(result);
        }

        [Route("api/trn/exam/resume/")]
        [AcceptVerbs("Post")]
        public async Task<IHttpActionResult> post_exam_resume(dynamic dto)
        {
            int exam_id = Convert.ToInt32(dto.exam_id);

            FLYEntities context = new FLYEntities();
            context.Configuration.LazyLoadingEnabled = false;
            var exam = await context.trn_exam.FirstOrDefaultAsync(q => q.id == exam_id);
            exam.is_running = true;
            var dif =(int)( exam.duration - exam.passed);
            exam.date_end_scheduled= (DateTime.Now).AddMinutes(dif);


            var sub_exams = await context.trn_person_exam.Where(q => q.main_exam_id == exam_id).ToListAsync();
            foreach (var x in sub_exams)
            {
                x.is_running = exam.is_running;
                x.date_end_scheduled = exam.date_end_scheduled;
            }


            await context.SaveChangesAsync();
            var result = new DataResponse()
            {
                IsSuccess = true,
                Data = exam.is_running,

            };
            return Ok(result);
        }




        public class dto_exam_person_pause
        {
            public List<int> exam_ids { get; set; }
        }

        [Route("api/trn/exam/person/pause/")]
        [AcceptVerbs("Post")]
        public async Task<IHttpActionResult> post_exam_person_pause(dto_exam_person_pause dto)
        {
             

            FLYEntities context = new FLYEntities();
            context.Configuration.LazyLoadingEnabled = false;
            var exams = await context.trn_person_exam.Where(q => dto.exam_ids.Contains(q.id)).ToListAsync();
            foreach (var exam   in exams)
            {
                exam.is_running = false;
                exam.passed = Convert.ToInt32(Math.Round((DateTime.Now - ((DateTime)exam.date_start)).TotalMinutes));
            }
           


            


            await context.SaveChangesAsync();
            var result = new DataResponse()
            {
                IsSuccess = true,
                Data = dto.exam_ids,

            };
            return Ok(result);
        }

        [Route("api/trn/exam/person/resume/")]
        [AcceptVerbs("Post")]
        public async Task<IHttpActionResult> post_exam_person_resume(dto_exam_person_pause dto)
        {
            

            FLYEntities context = new FLYEntities();
            context.Configuration.LazyLoadingEnabled = false;
            var exams = await context.trn_person_exam.Where(q => dto.exam_ids.Contains(q.id)).ToListAsync();
            foreach (var exam in exams)
            {
                exam.is_running = true;
                var dif = (int)(exam.duration - exam.passed);
                exam.date_end_scheduled = (DateTime.Now).AddMinutes(dif);
            }
            
            


             


            await context.SaveChangesAsync();
            var result = new DataResponse()
            {
                IsSuccess = true,
                Data = dto.exam_ids,

            };
            return Ok(result);
        }

        [Route("api/trn/exam/person/generate/{exam_id}")]
        [AcceptVerbs("Get")]
        public async Task<IHttpActionResult> exam_person_generate(int exam_id)
        {
            FLYEntities context = new FLYEntities();
            context.Configuration.LazyLoadingEnabled = false;
            var main_exam = await context.trn_exam.FirstOrDefaultAsync(q => q.id == exam_id);
            var people = await context.CoursePeoples.Where(q => q.CourseId == main_exam.course_id).ToListAsync();
            foreach (var p in people)
            {
                var pexam = new trn_person_exam()
                {
                    confirmed_by = main_exam.confirmed_by,
                    confirmed_date = main_exam.confirmed_date,
                    course_id = main_exam.course_id,
                    created_by = main_exam.created_by,
                    created_date = main_exam.created_date,
                    date_end_actual = main_exam.date_end_actual,
                    date_end_scheduled = main_exam.date_end_scheduled,
                    date_start = main_exam.date_start,
                    date_start_scheduled = main_exam.date_start_scheduled,
                    exam_date = main_exam.exam_date,
                    duration = main_exam.duration,
                    exam_date_persian = main_exam.exam_date_persian,
                    exam_type_id = main_exam.exam_type_id,
                    location_address = main_exam.location_address,
                    location_phone = main_exam.location_phone,
                    location_title = main_exam.location_title,
                    remark = main_exam.remark,
                    signed_by_director_date = main_exam.signed_by_director_date,
                    signed_by_ins1_date = main_exam.signed_by_ins1_date,
                    signed_by_ins2_date = main_exam.signed_by_ins2_date,
                    signed_by_staff_date = main_exam.signed_by_staff_date,
                    status_id = main_exam.status_id,
                    main_exam_id = main_exam.id,
                    person_id = p.PersonId


                };
                context.trn_person_exam.Add(pexam);
            }

            await context.SaveChangesAsync();
            var result = new DataResponse()
            {
                IsSuccess = true,
                Data = 1,

            };
            return Ok(result);
        }

        [Route("api/trn/exam/questions/generate")]
        [AcceptVerbs("Post")]

        public async Task<IHttpActionResult> get_crm_assessment(dto_exam_questions_generate dto)
        {
            try
            {
                FLYEntities context = new FLYEntities();
                context.Configuration.LazyLoadingEnabled = false;
                var templates = await context.trn_exam_question_template.Where(q => q.exam_id == dto.exam_id).ToListAsync();
                var generated_questions = new List<trn_exam_question>();
                var person_generated_questions = new List<trn_person_exam_question>();
                var cat_ids = templates.Select(q => q.question_category_id).ToList();
                var questions = await context.trn_questions.Where(q => cat_ids.Contains(q.category_id)).ToListAsync();
                foreach (var template in templates)
                {
                    var qs = questions.Where(q => q.category_id == template.question_category_id).OrderBy(q => q.id).ToList();
                    var selected_qs = TakeRandomRows(qs, (int)template.total);
                    if (selected_qs != null && selected_qs.Count() > 0)
                        foreach (var q in selected_qs)
                        {
                            context.trn_exam_question.Add(new trn_exam_question()
                            {
                                exam_id = dto.exam_id,
                                question_id = q.id,

                            });
                            person_generated_questions.Add(
                             new trn_person_exam_question()
                             {
                                 exam_id = dto.exam_id,
                                 question_id = q.id,
                                 remark = "date: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm"),

                             }
                           );
                        }

                }
                var exists = await context.trn_exam_question.Where(q => q.exam_id == dto.exam_id).ToListAsync();
                if (exists != null && exists.Count > 0)
                    context.trn_exam_question.RemoveRange(exists);
                foreach (var q in generated_questions)
                    context.trn_exam_question.Add(q);



                var person_exams = await context.trn_person_exam.Where(q => q.main_exam_id == dto.exam_id ).ToListAsync();
                foreach (var person_exam in person_exams)
                {
                    var pexists = await context.trn_person_exam_question.Where(q => q.exam_id == person_exam.id).ToListAsync();
                    if (pexists != null && pexists.Count > 0)
                        context.trn_person_exam_question.RemoveRange(pexists);

                    foreach (var q in person_generated_questions)
                        context.trn_person_exam_question.Add(new trn_person_exam_question()
                        {
                            person_id = person_exam.person_id,
                            question_id = q.question_id,
                            remark = q.remark,
                            exam_id = person_exam.id
                        });

                }


                await context.SaveAsync();
                var result = new DataResponse()
                {
                    IsSuccess = true,
                    Data = generated_questions.Select(q => new { q.exam_id, q.id, q.question_id, q.remark }),

                };
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }


        [Route("api/trn/exam/person/questions/generate")]
        [AcceptVerbs("Post")]

        public async Task<IHttpActionResult> generate_person_exam_question(dto_exam_questions_generate dto)
        {
            FLYEntities context = new FLYEntities();
            context.Configuration.LazyLoadingEnabled = false;
            int main_exam_id = Convert.ToInt32(dto.main_exam_id);

            var _pids = dto.person_ids.Select(q => (Nullable<int>)q).ToList();
            List<trn_person_exam> person_exams = await context.trn_person_exam.Where(q => _pids.Contains(q.person_id) && q.main_exam_id == main_exam_id).ToListAsync();
            var main_exam = await context.trn_exam.FirstOrDefaultAsync(q => q.id == main_exam_id);

            var templates = await context.trn_exam_question_template.Where(q => q.exam_id == main_exam.id).ToListAsync();

            var generated_questions = new List<trn_person_exam_question>();

            var cat_ids = templates.Select(q => q.question_category_id).ToList();
            var questions = await context.trn_questions.Where(q => cat_ids.Contains(q.category_id)).ToListAsync();

            foreach (var template in templates)
            {
                var qs = questions.Where(q => q.category_id == template.question_category_id).OrderBy(q => q.id).ToList();
                var selected_qs = TakeRandomRows(qs, (int)template.total);
                foreach (var q in selected_qs)
                {
                    generated_questions.Add(new trn_person_exam_question()
                    {
                        exam_id = -1,
                        question_id = q.id,
                        remark = "date: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                        person_id = -1,
                    });


                }

            }
            // List<trn_person_exam_question> dddddd = new List<trn_person_exam_question>();
            foreach (var person_exam in person_exams)
            {
                var exists = await context.trn_person_exam_question.Where(q => q.exam_id == person_exam.id).ToListAsync();
                if (exists != null && exists.Count > 0)
                    context.trn_person_exam_question.RemoveRange(exists);
                foreach (var q in generated_questions)
                {
                    var _q = new trn_person_exam_question()
                    {
                        person_id = person_exam.person_id,
                        exam_id = person_exam.id,
                        question_id = q.question_id,
                        remark = q.remark,
                    };

                    context.trn_person_exam_question.Add(_q);
                }
            }








            await context.SaveAsync();
            var result = new DataResponse()
            {
                IsSuccess = true,
                Data = generated_questions.Select(q => new { q.exam_id, q.id, q.question_id, q.remark }),

            };
            return Ok(result);
        }

        public class dto_exam_done
        {
            public int exam_id { get; set; }
            public int person_id { get; set; }

        }
        [Route("api/trn/exam/sign/client")]
        [AcceptVerbs("Post")]

        public async Task<IHttpActionResult> sign_exam_client(dto_exam_done dto)
        {
            FLYEntities context = new FLYEntities();
            context.Configuration.LazyLoadingEnabled = false;
            var exam = await context.trn_person_exam.FirstOrDefaultAsync(q => q.id == dto.exam_id && q.person_id == dto.person_id);
            if (exam != null && exam.status_id != 2)
            {
                exam.status_id = 2;
                exam.person_sign_date = DateTime.Now;
            }


            await context.SaveAsync();
            var result = new DataResponse()
            {
                IsSuccess = true,
                Data = dto.exam_id,

            };
            return Ok(result);
        }

        [Route("api/exam/summary/{exam_id}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetCoursePeopleSessions(int exam_id)
        {
            var result = await courseService.GetExamSummary(exam_id);

            return Ok(result);
        }

        [Route("api/exam/results/{exam_id}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetExamResults(int exam_id)
        {
            var result = await courseService.GetExamPeopleAnswers(exam_id);

            return Ok(result);
        }

        [Route("api/exam/person/results/{exam_id}/{person_id}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetExamPersonResults(int exam_id, int person_id)
        {
            var result = await courseService.GetExamPeopleAnswersByPerson(exam_id, person_id);
            //GetExamPeopleAnswersByPerson

            return Ok(result);
        }

        public List<view_trn_exam_question_person> ShuffleQuestionsBasedOnId(List<view_trn_exam_question_person> questions, int client_id)
        {
            Random random = new Random(client_id);
            return questions.OrderBy(q => random.Next()).ToList();
        }

        public class dto_answer_out
        {
            public int id { get; set; }
            public int answer_id { get; set; }
            public Nullable<int> quesion_id { get; set; }
            public Nullable<int> exam_quesion_id { get; set; }
            public string english_title { get; set; }
            public string persian_title { get; set; }

            public Nullable<bool> is_rtl { get; set; }
            public bool is_selected { get; set; }

            public int? is_answer { get; set; }
        }
        public partial class view_trn_exam_question_person_dto
        {
            public int id { get; set; }
            public int question_id { get; set; }
            public int exam_id { get; set; }
            public Nullable<int> response_time { get; set; }
            public string english_title { get; set; }
            public string persian_title { get; set; }
            public Nullable<int> category_id { get; set; }
            public string category { get; set; }
            public int course_id { get; set; }
            public string Title { get; set; }
            public Nullable<int> person_id { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public Nullable<int> answer_id { get; set; }
            public Nullable<System.DateTime> date_sent { get; set; }
            public List<trn_answers> db_answers { get; set; }
            public List<dto_answer_out> answers { get; set; }
        }
        [Route("api/client/exam/{id}/{client_id}")]
        public async Task<IHttpActionResult> GetClientExamById(int id, int client_id)
        {
            try
            {
                FLYEntities context = new FLYEntities();

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
                    if (exam.status_id == 2)
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

        public class dto_exam_questions_generate
        {
            public int exam_id { get; set; }
            public List<int> person_ids { get; set; }
            public int? main_exam_id { get; set; }
        }


        public class question_dto
        {
            public int exam_id { get; set; }
            public int exam_question_id { get; set; }
            public int? question_id { get; set; }
            public string english_title { get; set; }
            public int? category_id { get; set; }
            public bool is_rtl { get; set; }
            public int hardness { get; set; }
            public string category { get; set; }
            public string correct_answer_title { get; set; }
            public int correct_answer_id { get; set; }

            public List<answers_dto> answers { get; set; }
        }


        public class answers_dto
        {
            public int id { get; set; }
            public int? quesion_id { get; set; }
            public string english_title { get; set; }
            public string persian_title { get; set; }
            public int? is_answer { get; set; }
            public bool? is_rtl { get; set; }

        }

        [Route("api/exam/questions/{exam_id}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetExamQuestions(int exam_id)
        {
            try
            {
                FLYEntities context = new FLYEntities();
                var questions = await context.view_trn_exam_question
     .Where(q => q.exam_id == exam_id)
     .Select(q => new question_dto
     {
         exam_id = q.exam_id,
         exam_question_id = q.exam_question_id,
         question_id = q.question_id,
         english_title = q.english_title,
         category_id = q.category_id,
         is_rtl = q.is_rtl,
         hardness = q.hardness,
         category = q.category,
         correct_answer_title = q.correct_answer_title,
         correct_answer_id = q.correct_answer_id
     })
     .ToListAsync();

                var questions_id = questions.Select(q => q.question_id).ToList();
                var answersGrouped = context.trn_answers
      .Where(a => questions_id.Contains(a.quesion_id))
      .Select(a => new answers_dto
      {
          id = a.id,
          quesion_id = a.quesion_id,
          english_title = a.english_title,
          persian_title = a.persian_title,
          is_answer = a.is_answer,
          is_rtl = a.is_rtl
      })
      .ToList()
      .GroupBy(a => a.quesion_id)
      .ToDictionary(g => g.Key, g => g.ToList());

                foreach (var question in questions)
                {
                    question.answers = answersGrouped.ContainsKey(question.question_id)
                                       ? answersGrouped[question.question_id]
                                       : new List<answers_dto>();
                }

                var result = new { questions };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Route("api/question/templates/")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetQuestionTemplates()
        {
            try
            {
                FLYEntities context = new FLYEntities();
                var temps = await context.view_trn_exam_question_template.OrderBy(q => q.category).ToListAsync();

                var result = new DataResponse()
                {
                    Data = temps
                     ,
                    IsSuccess = true,
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Route("api/exams/")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetExams()
        {
            try
            {
                FLYEntities context = new FLYEntities();
                var exams = await context.view_trn_exam.Where(q=>q.exam_date!=null).OrderByDescending(q => q.exam_date).ToListAsync();

                var result = new DataResponse()
                {
                    Data = exams
                     ,
                    IsSuccess = true,
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }




    }
}
