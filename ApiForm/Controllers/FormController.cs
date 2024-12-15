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
using System.Threading;
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
        public IHttpActionResult SaveVacation(VacationFormViewModel log)
        {
            ppa_entities context = new ppa_entities();

            var requester = context.ViewProfiles.Where(q => q.Id == log.UserId).FirstOrDefault();


            var form = new FormVacation()
            {
                UserId = log.UserId,
                DateCreate = DateTime.Now,
                DateFrom = convert_to_date(log.DateFrom),
                DateTo = convert_to_date(log.DateTo).AddHours(23),
                ReasonStr = log.ReasonStr,
                Reason = log.Reason,
                Remark = log.Remark,
            };
            var cockpit_grps = new List<string>() { "TRE", "TRI", "LTC", "NC", "P1", "P2" };
            //4811
            if (cockpit_grps.IndexOf(requester.JobGroup) != -1)
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
            var forms = context.ViewFormVacations.Where(q => q.ResponsibleId == id).OrderByDescending(q => q.DateCreate).ToList();
            return Ok(forms);
        }

        //2024-11-26
        [Route("api/vacation/forms/approved")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetVacationFormsApproved()
        {
            ppa_entities context = new ppa_entities();
            var forms = context.ViewFormVacations.Where(q => q.ResponsibleActionId == 1).OrderByDescending(q => q.DateCreate).ToList();
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

            var request = context.FormVacations.Where(q => q.Id == dto.form_id).FirstOrDefault();

            if (request == null)
                return Ok(-1);
            if (request.ResponsibleId != dto.employee_id)
                return Ok(-2);
            request.ResponsibleDateVisit = DateTime.Now;

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
            request.ResponsibleActionId = dto.action_id;
            request.ResponsibleRemark = dto.remark;

            context.SaveChanges();
            var view = context.ViewFormVacations.Where(q => q.Id == dto.form_id).FirstOrDefault();

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

        public void send_vacation_notification(FormVacation vacation, ViewEmployee employee)
        {
            new Thread(async () =>
            {
                try
                {
                    ppa_entities context = new ppa_entities();
                    var pic = employee;
                    var applicant = context.ViewEmployees.FirstOrDefault(q => q.PersonId == vacation.UserId);

                    List<string> prts = new List<string>();
                    prts.Add("New Vacation Request Notification");
                    prts.Add("Dear " + applicant.Name);
                    prts.Add("Date: " + ((DateTime)vacation.DateCreate).ToString("yyyy-MM-dd"));
                    prts.Add("Request Summary:");
                    prts.Add(vacation.Remark);


                    var text = String.Join("\n", prts);
                    List<request_receiver> nots = new List<request_receiver>();

                    var not_receivers = context.request_receiver.Where(q => q.is_active == true).ToList();
                    var _result = new List<qa_notification_history>();
                    MelliPayamac m1 = new MelliPayamac();
                    foreach (var rec in not_receivers)
                    {
                        List<string> prts2 = new List<string>();
                        prts2.Add("New ASR Notification");
                        prts2.Add("Dear " + rec.receiver_name);
                        prts2.Add("Event Summary:");
                        prts2.Add(vacation.Remark);

                        var text2 = String.Join("\n", prts2);

                        var not_history = new qa_notification_history();

                        if (applicant.JobGroupRoot == "Cockpit" && rec.group == "Cockpit_737")
                        {
                            not_history = new qa_notification_history()
                            {
                                date_send = DateTime.Now,
                                entity_id = vacation.Id,
                                entity_type = 8,
                                message_text = text2,
                                message_type = 1,
                                rec_id = rec.receiver_id,
                                rec_mobile = rec.receiver_mobile.ToString(),
                                rec_name = rec.receiver_name,
                                counter = 0,

                            };
                        }
                        else if (applicant.JobGroupRoot == "Cockpit" && rec.group == "Cockpit_MD")
                        {
                            not_history = new qa_notification_history()
                            {
                                date_send = DateTime.Now,
                                entity_id = vacation.Id,
                                entity_type = 8,
                                message_text = text2,
                                message_type = 1,
                                rec_id = rec.receiver_id,
                                rec_mobile = rec.receiver_mobile.ToString(),
                                rec_name = rec.receiver_name,
                                counter = 0,

                            };
                        }
                        else if (applicant.JobGroupRoot == "Cockpit" && rec.group == "Cockpit_Airbus")
                        {
                            not_history = new qa_notification_history()
                            {
                                date_send = DateTime.Now,
                                entity_id = vacation.Id,
                                entity_type = 8,
                                message_text = text2,
                                message_type = 1,
                                rec_id = rec.receiver_id,
                                rec_mobile = rec.receiver_mobile.ToString(),
                                rec_name = rec.receiver_name,
                                counter = 0,

                            };
                        }
                        else if (applicant.JobGroupRoot == "Cabin" && rec.group == "Cabin")
                        {

                            not_history = new qa_notification_history()
                            {
                                date_send = DateTime.Now,
                                entity_id = vacation.Id,
                                entity_type = 8,
                                message_text = text2,
                                message_type = 1,
                                rec_id = rec.receiver_id,
                                rec_mobile = rec.receiver_mobile.ToString(),
                                rec_name = rec.receiver_name,
                                counter = 0,

                            };

                        }

                        var smsResult1 = m1.send(not_history.rec_mobile, null, text2)[0];
                        not_history.ref_id = smsResult1.ToString();
                        _result.Add(not_history);
                        System.Threading.Thread.Sleep(2000);
                    }


                    var not_history_pic = new qa_notification_history()
                    {
                        date_send = DateTime.Now,
                        entity_id = vacation.Id,
                        entity_type = 8,
                        message_text = text,
                        message_type = 2,
                        rec_id = applicant.PersonId,
                        rec_mobile = pic.Mobile,
                        rec_name = pic.Name,
                        counter = 0,
                    };

                    var not_history_pic2 = new qa_notification_history()
                    {
                        date_send = DateTime.Now,
                        entity_id = vacation.Id,
                        entity_type = 8,
                        message_text = text,
                        message_type = 1,
                        rec_id = applicant.PersonId,
                        rec_mobile = "09124449584",
                        rec_name = pic.Name,
                        counter = 0,
                    };

                    MelliPayamac m1_pic = new MelliPayamac();
                    var m1_pic_result = m1_pic.send(not_history_pic.rec_mobile, null, not_history_pic.message_text)[0];
                    not_history_pic.ref_id = m1_pic_result.ToString();

                    MelliPayamac m_pic = new MelliPayamac();
                    var m_pic_result = m_pic.send(not_history_pic2.rec_mobile, null, not_history_pic2.message_text)[0];
                    not_history_pic2.ref_id = m_pic_result.ToString();

                    _result.Add(not_history_pic);
                    _result.Add(not_history_pic2);

                    System.Threading.Thread.Sleep(20000);
                    foreach (var x in _result)
                    {
                        MelliPayamac m_status = new MelliPayamac();
                        x.status = m_status.get_delivery(x.ref_id);

                        context.qa_notification_history.Add(x);
                    }

                    context.SaveChanges();
                }
                catch (Exception ex)
                {

                }

            }).Start();
            /////////////////////

        }






    }
}
