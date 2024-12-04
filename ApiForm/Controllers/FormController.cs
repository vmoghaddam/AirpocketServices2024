using ApiForm.Models;
using ApiForm.ViewModels;
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

namespace ApiForm.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class FormController : ApiController
    {

        public class VacationFormViewModel
        {
            public int Id { get; set; }
            public int UserId { get; set; }
            public string DateFrom { get; set; }
            public string DateTo { get; set; }
            public string ReasonStr { get; set; }
            public int Reason { get; set; }

            public string Remark { get; set; }
            public string OperationRemak { get; set; }
            public string SchedulingRemark { get; set; }
            public string Status { get; set; }
            public int OperatorId { get; set; }

            public int? FDPId { get; set; }
        }
        public DateTime convert_to_date(string str)
        {
            var prts = str.Split('-');
            return new DateTime(Convert.ToInt32(prts[0]), Convert.ToInt32(prts[1]), Convert.ToInt32(prts[2]));
        }
        [Route("api/vacation/save")]
        [AcceptVerbs("POST")]
        public  IHttpActionResult SaveVacation(VacationFormViewModel log)
        {
            ppa_entities context = new ppa_entities();

            var requester= context.ViewProfiles.Where(q=>q.Id==log.UserId).FirstOrDefault();
            

            var form = new FormVacation()
            {
                UserId = log.UserId,
                DateCreate = DateTime.Now,
                DateFrom = convert_to_date( log.DateFrom),
                DateTo = convert_to_date( log.DateTo),
                ReasonStr = log.ReasonStr,
                Reason = log.Reason,
                Remark = log.Remark,
            };
            var cockpit_grps = new List<string>() {"TRE","TRI","LTC","NC","P1","P2" };
            //4811
            if (cockpit_grps.IndexOf(requester.JobGroup)!=-1)
                form.ResponsibleId = 4758;
            else
                form.ResponsibleId = 4811;




            context.FormVacations.Add(form);
            context.SaveChanges();
            var view = context.FormVacations.Where(q => q.Id == form.Id).FirstOrDefault();
            return Ok(view);
        }
        [Route("api/vacation/delete")]
        [AcceptVerbs("POST")]
        public IHttpActionResult DeleteVacation(dynamic log)
        {
            ppa_entities context = new ppa_entities();
            int id = Convert.ToInt32(log.id);
            var obj = context.FormVacations.Where(q => q.Id == id).FirstOrDefault();
            context.FormVacations.Remove(obj);

            context.SaveChanges();

            return Ok(true);
        }
        [Route("api/vacation/update")]
        [AcceptVerbs("POST")]
        public IHttpActionResult UpdateVacation(VacationFormViewModel log)
        {
            ppa_entities context = new ppa_entities();
            var form = context.FormVacations.Where(q => q.Id == log.Id).FirstOrDefault();
           // form.OperationRemak = log.OperationRemak;
            form.OperatorId = log.OperatorId;
          //  form.Reason = log.Reason;
          //  form.ReasonStr = log.ReasonStr;
        //    form.Remark = log.Remark;
            form.SchedulingRemark = log.SchedulingRemark;
            if (!string.IsNullOrEmpty(log.Status) && log.Status != form.Status)
                form.DateStatus = DateTime.Now;
            form.Status = log.Status;
            if (form.Status == "Accepted")
            {
                form.FDPId = log.FDPId;
            }
            else
            {
                if (form.FDPId != null)
                {
                    var _fdp = context.FDPs.FirstOrDefault(q => q.Id == form.FDPId);
                    if (_fdp != null)
                        context.FDPs.Remove(_fdp);
                }
                
            }
         //   form.UserId = log.UserId;



            context.SaveChanges();
            var view = context.ViewFormVacations.Where(q => q.Id == form.Id).FirstOrDefault();
            return Ok(view);
        }
        [Route("api/vacation/forms/crew/{id}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetVacationForms(int id)
        {
            ppa_entities context = new ppa_entities();
            var forms = context.ViewFormVacations.Where(q => q.UserId == id).OrderByDescending(q => q.DateCreate).ToList();
            return Ok(forms);
        }



        [Route("api/vacation/forms/all")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetVacationFormsAll()
        {
            ppa_entities context = new ppa_entities();
            var forms = context.ViewFormVacations.OrderByDescending(q => q.DateCreate).ToList();
            return Ok(forms);
        }

       //2024-11-26
        [Route("api/vacation/forms/responsible/all/{id}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetVacationFormsResponsibleAll(int id)
        {
            ppa_entities context = new ppa_entities();
            var forms = context.ViewFormVacations.Where(q=>q.ResponsibleId==id).OrderByDescending(q => q.DateCreate).ToList();
            return Ok(forms);
        }

        //2024-11-26
        [Route("api/vacation/forms/approved")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetVacationFormsApproved()
        {
            ppa_entities context = new ppa_entities();
            var forms = context.ViewFormVacations.Where(q=>q.ResponsibleActionId==1).OrderByDescending(q => q.DateCreate).ToList();
            return Ok(forms);
        }


        [Route("api/vacation/forms/new")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetVacationFormsNew()
        {
            ppa_entities context = new ppa_entities();
            var forms = context.ViewFormVacations.Where(q => string.IsNullOrEmpty(q.Status)).OrderByDescending(q => q.DateCreate).ToList();
            return Ok(forms);
        }
        [Route("api/vacation/forms/acc")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetVacationFormsAcc()
        {
            ppa_entities context = new ppa_entities();
            var forms = context.ViewFormVacations.Where(q => q.Status == "Accepted").OrderByDescending(q => q.DateCreate).ToList();
            return Ok(forms);
        }
        [Route("api/vacation/forms/rej")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetVacationFormsRej()
        {
            ppa_entities context = new ppa_entities();
            var forms = context.ViewFormVacations.Where(q => q.Status == "Rejected").OrderByDescending(q => q.DateCreate).ToList();
            return Ok(forms);
        }

        public class request_visit_dto
        {
            public int employee_id { get; set; }
            public int form_id { get; set; }
        }
        //2024-11-26
        [Route("api/request/visit")]
        [AcceptVerbs("POST")]
        public IHttpActionResult VisitRequest(request_visit_dto dto)
        {
            ppa_entities context = new ppa_entities();

            var request  = context.FormVacations.Where(q => q.Id == dto.form_id).FirstOrDefault();
           
            if (request == null)
                return Ok(-1);
            if (request.ResponsibleId != dto.employee_id)
                return Ok(-2);
            request.ResponsibleDateVisit=DateTime.Now;
 
            context.SaveChanges();
            
            return Ok(dto.form_id);
        }


        public class request_action_dto
        {
            public int employee_id { get; set; }
            public int form_id { get; set; }
            /// <summary>
            /// 1: approved 0:rejected
            /// </summary>
            public int action_id { get; set; }

            public string remark { get; set; }
        }
        //2024-11-26
        [Route("api/request/action")]
        [AcceptVerbs("POST")]
        public IHttpActionResult ActionRequest(request_action_dto dto)
        {
            ppa_entities context = new ppa_entities();

            var request = context.FormVacations.Where(q => q.Id == dto.form_id).FirstOrDefault();
            if (request == null)
                return Ok(-1);
            if (request.ResponsibleId != dto.employee_id)
                return Ok(-2);
            request.ResponsibleActionDate = DateTime.Now;
            request.ResponsibleActionId= dto.action_id;
            request.ResponsibleRemark = dto.remark;

            context.SaveChanges();
            var view=context.ViewFormVacations.Where(q=>q.Id==dto.form_id).FirstOrDefault();

            return Ok(view);
        }


        [Route("api/forms/employee/timeline/{form_id}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetEmployeeTimeline(int form_id)
        {
            ppa_entities context = new ppa_entities();
            var form = context.ViewFormVacations.Where(q => q.Id == form_id).FirstOrDefault();
            var dt1 = ((DateTime)form.DateFrom).AddDays(-3).Date;
            var dt2 = ((DateTime)form.DateTo).AddDays(4).Date;
            var timeline = context.ViewCrewDutyTimeLineNews.Where(q => q.CrewId == form.EmployeeId && q.DateStartLocal >= dt1 && q.DateStartLocal < dt2).OrderBy(q => q.DateStartLocal).ToList();

            return Ok(timeline);
        }





    }
}
