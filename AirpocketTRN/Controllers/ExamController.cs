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


namespace AirpocketTRN.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ExamController : ApiController
    {

        public List<trn_questions> TakeRandomRows(List<trn_questions> list, int n)
        {
            // Create a new instance of Random
            Random random = new Random();

            // Order the list randomly and take n rows
            return list.OrderBy(x => random.Next()).Take(n).ToList();
        }


        [Route("api/trn/exam/questions/generate")]
        [AcceptVerbs("Post")]

        public async Task<IHttpActionResult> get_crm_assessment(dto_exam_questions_generate dto)
        {
            FLYEntities context = new FLYEntities();
            context.Configuration.LazyLoadingEnabled = false;
            var templates = await context.trn_exam_question_template.Where(q => q.exam_id == dto.exam_id).ToListAsync();
            var generated_questions = new List<trn_exam_question>();
            var cat_ids = templates.Select(q => q.question_category_id).ToList();
            var questions = await context.trn_questions.Where(q => cat_ids.Contains(q.category_id)).ToListAsync();
            foreach (var template in templates)
            {
                var qs = questions.Where(q => q.category_id == template.question_category_id).OrderBy(q => q.id).ToList();
                var selected_qs = TakeRandomRows(qs, (int)template.total);
                foreach (var q in selected_qs)
                {
                    generated_questions.Add(new trn_exam_question()
                    {
                        exam_id = dto.exam_id,
                        question_id = q.id,
                        remark = "date: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm")
                    });
                }

            }
            var exists=await context.trn_exam_question.Where(q=>q.exam_id==dto.exam_id).ToListAsync();
            if (exists != null && exists.Count > 0)
                context.trn_exam_question.RemoveRange(exists);
            foreach(var q in generated_questions)
                context.trn_exam_question.Add(q);
            await context.SaveAsync();
            var result = new DataResponse()
            {
                IsSuccess = true,
                Data= generated_questions,

            };
            return Ok(result);
        }


        public class dto_exam_questions_generate
        {
            public int exam_id { get; set; }
        }

    }
}
