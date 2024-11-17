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
            public DateTime DateFrom { get; set; }
            public DateTime DateTo { get; set; }
            public string ReasonStr { get; set; }
            public int Reason { get; set; }

            public string Remark { get; set; }
            public string OperationRemak { get; set; }
            public string SchedulingRemark { get; set; }
            public string Status { get; set; }
            public int OperatorId { get; set; }
        }

        [Route("api/vacation/save")]
        [AcceptVerbs("POST")]
        public IHttpActionResult SaveVacation(VacationFormViewModel log)
        {
            ppa_entities context = new ppa_entities();
            var form = new FormVacation()
            {
                UserId = log.UserId,
                DateCreate = DateTime.Now,
                DateFrom = log.DateFrom,
                DateTo = log.DateTo,
                ReasonStr = log.ReasonStr,
                Reason = log.Reason,
                Remark = log.Remark,
            };
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
            form.OperationRemak = log.OperationRemak;
            form.OperatorId = log.OperatorId;
            form.Reason = log.Reason;
            form.ReasonStr = log.ReasonStr;
            form.Remark = log.Remark;
            form.SchedulingRemark = log.SchedulingRemark;
            if (!string.IsNullOrEmpty(log.Status) && log.Status != form.Status)
                form.DateStatus = DateTime.Now;
            form.Status = log.Status;
            form.UserId = log.UserId;



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


    }
}
