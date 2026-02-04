using AirpocketTRN.Models;
using AirpocketTRN.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using PdfiumViewer;
using System.Drawing;
using System.Drawing.Imaging;
using System.Configuration;

namespace AirpocketTRN.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class FormsController : ApiController
    {
        [Route("api/forms/items/{form_id}/{crew_id}/{flight_id}")]
        [AcceptVerbs("Get")]

        public async Task<IHttpActionResult> get_items(int form_id, int crew_id, int flight_id)
        {
            var context = new FLYEntities();


            var cats = context.view_frms_checklist_category.Where(q => q.form_type_id == form_id).OrderBy(q => q.path).ToList();

            var query = context.view_frms_checklist_item_result.Where(q => q.crew_id == crew_id).ToList();


            var result = (from x in cats
                         select new dto_category()
                         {
                             form_type_id = x.form_type_id,
                             code = x.code,
                             id = x.id,
                             level = x.level,
                             parent_id = x.parent_id,
                             path = x.path,
                             sort_order = x.sort_order,
                             title = x.title,
                             items=query.Where(q=>q.category_id==x.id).OrderBy(q=>q.sort_order).ToList(),

                         }).ToList();
            return Ok(result);
        }


        [Route("api/forms/cabin/linecheck/{crew_id}/{ins_id}/{flight_id}")]
        [AcceptVerbs("Get")]

        public async Task<IHttpActionResult> get_cabin_linecheck(int ins_id, int crew_id, int flight_id)
        {
            var context = new FLYEntities();

            var result=context.view_frms_cabin_line_check.Where(q=>q.flight_id==flight_id && q.crew_id==crew_id && q.instructor_id==ins_id).FirstOrDefault();
            if (result == null)
            {
                var frm = new frms_cabin_line_check()
                {
                    flight_id = flight_id,
                    crew_id = crew_id,
                    instructor_id = ins_id,

                };
                context.frms_cabin_line_check.Add(frm);
                context.SaveChanges();
            }

            result = context.view_frms_cabin_line_check.Where(q => q.flight_id == flight_id && q.crew_id == crew_id && q.instructor_id == ins_id).FirstOrDefault();

            return Ok(result);
        }




        public class dto_category
        {


            public int id { get; set; }
            public Nullable<int> parent_id { get; set; }
            public Nullable<int> form_type_id { get; set; }
            public string code { get; set; }
            public string title { get; set; }
            public Nullable<int> level { get; set; }
            public string path { get; set; }
            public Nullable<int> sort_order { get; set; }

            public List<view_frms_checklist_item_result> items { get; set; }
        }








    }
}
