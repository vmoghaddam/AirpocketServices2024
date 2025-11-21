using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

using System.Data.Entity;
using System.Data.Entity.Infrastructure;


using System.Web.Http.Description;

using System.Data.Entity.Validation;

using System.Web.Http.ModelBinding;

using System.Text;
using System.Configuration;
using Newtonsoft.Json;
using System.Web.Http.Cors;
using System.IO;
using System.Xml;
using System.Web;
using System.Text.RegularExpressions;
using Formatting = Newtonsoft.Json.Formatting;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using ApiMSG.Models;
using System.Net.Http.Headers;
using System.Drawing;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Threading;
using System.Net.Mail;

namespace ApiMSG.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class OpsController : ApiController
    {

        [Route("api/notify/ops/scheduling/duties")]
        [AcceptVerbs("POST")]
       // public IHttpActionResult GetNotifyTrnGroupExpiring(int m, int n)
        public IHttpActionResult SMSDuties(/*List<int> Ids, DateTime date, string username = ""*/RosterSMSDto dto)
        {
            try
            {
                var ids = dto.Ids;
                var date = dto.Date.Date;
                var username = dto.UserName;
                var context = new ppa_vareshEntities();
                var fdps =context.FDPs.Where(q => ids.Contains(q.Id) /*&& q.DutyType != 5000*/).ToList();
                foreach (var f in fdps)
                {
                    if (f.DateConfirmed == null)
                    {
                        f.DateConfirmed = DateTime.Now;
                        f.ConfirmedBy = username;
                    }

                }

                var query =  (from x in context.ViewCrewDuties
                                   where ids.Contains(x.Id) /*&& x.DutyType != 5000*/
                                   select new
                                   {
                                       x.DateLocal
,
                                       x.Start
,
                                       x.End
,
                                       x.DutyType
,
                                       x.STDLocal
,
                                       x.STALocal
,
                                       x.DutyTypeTitle
,
                                       x.Route
,
                                       x.FltNo
,
                                       x.Remark
,
                                       x.CanceledNo
,
                                       x.CanceledRoute
,
                                       x.Remark2

,
                                       x.Mobile
,
                                       x.Name
,
                                       x.Id
,
                                       x.CrewId
                                       ,x.Email
                                   }

                                   ).Distinct().ToList();
                var _fids = query.Select(q => (Nullable<int>)q.Id).ToList();
                var histories = context.SMSHistories.Where(q => _fids.Contains(q.ResId)).ToList();
                Magfa m = new Magfa();
                Magfa m2 = new Magfa();
                List<IdDel> iddels = new List<IdDel>();
                var offs = new List<int>() { 100009, 100020, 100021, 100022, 100023 };

                MailHelper mailHelper = new MailHelper();

                //var smtp = new SmtpClient
                //{
                //    //EnableSsl=true,
                //    Host = "aerok.tech",
                //    Port = 25, //Convert.ToInt32(dispatchEmailPort),
                //    EnableSsl = false,
                //    //TargetName = "STARTTLS/Mail.flypersia.aero",
                //    DeliveryMethod = SmtpDeliveryMethod.Network,
                //    UseDefaultCredentials = false,
                //    Credentials = new NetworkCredential("airpocket@flypersiaairlines.ir", "1234@aA"),

                //};
                //smtp.Timeout = 60000;



                foreach (var x in query)
                {

                    List<string> strs = new List<string>();
                    List<string> strs_email = new List<string>();
                    List<string> token2s = new List<string>();
                    var token = "";
                    var token3 = "";

                    strs.Add(ConfigurationManager.AppSettings["airline"] );
                    strs.Add("Dear " + x.Name + ",");

                    strs_email.Add("<b>"+ConfigurationManager.AppSettings["airline"] +"</b>"+"<br/>");
                    strs_email.Add("Dear " +"<b>"+ x.Name+"</b>" + "," + "<br/>");


                    token = x.Name;

                    var day = (DateTime)x.DateLocal;
                    var _start = (DateTime)x.Start;
                    var _end = (DateTime)x.End;
                    if (x.DutyType == 1165)
                    {
                        _start =( (DateTime)x.STDLocal).AddMinutes(-45);
                        _end = (DateTime)x.STALocal;
                    }
                    var dayStr = day.ToString("ddd") + " " + day.Year + "-" + day.Month + "-" + day.Day;
                    var datesent = DateTime.Now.Year + "/" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "/" + DateTime.Now.Day.ToString().PadLeft(2, '0') + " " + DateTime.Now.Hour.ToString().PadLeft(2, '0') + ":" + DateTime.Now.Minute.ToString().PadLeft(2, '0');
                    token3 = datesent;

                    if (!offs.Contains(x.DutyType))
                    {
                        strs.Add(x.DutyTypeTitle);
                        strs.Add(dayStr);

                        strs_email.Add("<b>"+x.DutyTypeTitle+"</b>"+"<br/>");
                        strs_email.Add(dayStr + "<br/>");

                        token2s.Add(x.DutyTypeTitle);
                        token2s.Add(dayStr);

                        if (x.DutyType == 1165)
                        {
                            strs.Add(x.Route);
                            strs.Add(x.FltNo);

                            strs_email.Add(x.Route + "<br/>");
                            strs_email.Add(x.FltNo + "<br/>");


                            token2s.Add(x.Route);
                            token2s.Add(x.FltNo);
                        }
                        else
                        {
                            if (!String.IsNullOrEmpty(x.Remark))
                            {
                                strs.Add(x.Remark);
                                token2s.Add(x.Remark);

                                strs_email.Add(x.Remark+"<br/>");
                            }
                        }
                        strs.Add("Start " + _start.ToString("ddd yy-MMM-dd HH:mm"));
                        strs.Add("End " + _end.ToString("ddd yy-MMM-dd HH:mm"));

                        strs_email.Add("Start " + _start.ToString("ddd yy-MMM-dd HH:mm") + "<br/>");
                        strs_email.Add("End " + _end.ToString("ddd yy-MMM-dd HH:mm") + "<br/>");

                        token2s.Add("Start " + _start.ToString("ddd yy-MMM-dd HH:mm"));
                        token2s.Add("End " + _end.ToString("ddd yy-MMM-dd HH:mm"));
                    }
                    else
                    {
                        strs.Add("Canceling Notification");
                        strs.Add(dayStr);
                        strs.Add(x.CanceledNo);
                        strs.Add(x.CanceledRoute);
                        strs.Add(x.Remark2);
                        strs.Add(x.Remark);


                        strs_email.Add("Canceling Notification" + "<br/>");
                        strs_email.Add(dayStr + "<br/>");
                        strs_email.Add(x.CanceledNo + "<br/>");
                        strs_email.Add(x.CanceledRoute + "<br/>");
                        strs_email.Add(x.Remark2 + "<br/>");
                        strs_email.Add(x.Remark + "<br/>");
                        ////////////////
                        /// strs.Add("Canceling Notification");
                        token2s.Add(dayStr);
                        token2s.Add(x.CanceledNo);
                        token2s.Add(x.CanceledRoute);
                        token2s.Add(x.Remark2);
                        token2s.Add(x.Remark);
                        ////////////////

                    }




                    strs.Add("Date Sent: " + datesent);
                    strs.Add("Crew Scheduling Department");

                    strs_email.Add("Date Sent: " + datesent + "<br/>");
                    strs_email.Add("<b>"+"Crew Scheduling Department"+"</b>");

                    var text = String.Join("\n", strs);


                    long result9 = -1000;

                    if (1 == 2)
                        result9 = m.enqueue(1, x.Mobile, text)[0];


                    var email_status = "";
                    if (3==3 )
                    {
                        var email_body = String.Join("\n", strs_email);
                       // var fromAddress = new MailAddress("airpocket@flypersiaairlines.ir", "TRAINING DEPARTMENT");
                       email_status= mailHelper.SendEmail(email_body, "shahraeinisepehr@gmail.com", x.Name, "Crew Scheduling Department", "Crew Scheduling Notification", ConfigurationManager.AppSettings["smtp_address"]);
                        //using (var message = new MailMessage(fromAddress, new MailAddress(_man.Email, _man.Name))
                        //{
                        //    Subject = "Crew Scheduling Notification",
                        //    Body = email_body,
                        //    IsBodyHtml = true,


                        //})

                        //{

                        //    // message.CC.Add("v.moghaddam59@gmail.com");
                        //    message.CC.Add("itmng@flypersiaairlines.ir");
                        //    smtp.Send(message);


                        //}
                    }

                   
                    var exist = histories.Where(q => q.ResId == x.Id).ToList();
                    if (exist != null && exist.Count > 0)
                    {
                        // this.context.SMSHistories.Remove(exist);
                        context.SMSHistories.RemoveRange(exist);
                    }

                    context.SMSHistories.Add(new SMSHistory()
                    {
                        DateSent = DateTime.Now,
                        RecMobile = x.Mobile,
                        RecName = x.Name,
                        Ref = result9.ToString(),
                        Text = text,
                        TypeId = 1,
                        ResId = x.Id,
                        ResFlts = !string.IsNullOrEmpty(x.FltNo) ? x.FltNo : x.Remark,
                        ResDate = date,
                        ResStr =email_status, //"Queue",



                    });
                    //var existcps = await this.context.CrewPickupSMS.FirstOrDefaultAsync(q => q.FDPId == x.Id && q.CrewId == x.CrewId);
                    var existcps = context.CrewPickupSMS.Where(q => q.FDPId == x.Id && q.CrewId == x.CrewId).ToList();
                    if (existcps != null && existcps.Count() > 0)
                        //  this.context.CrewPickupSMS.Remove(existcps);
                        context.CrewPickupSMS.RemoveRange(existcps);
                    var cps = new CrewPickupSM()
                    {
                        CrewId = (int)x.CrewId,
                        DateSent = DateTime.Now,
                        DateStatus = DateTime.Now,
                        FlightId = -1,
                        Message = text,
                        Pickup = null,
                        RefId = result9.ToString(),
                        Status =email_status, //"Queue",
                        Type = x.DutyType,
                        FDPId = x.Id,
                        DutyType = x.DutyTypeTitle,
                        DutyDate = x.DateLocal,


                    };

                    if (x.DutyType == 1165)
                    {
                        cps.Flts = x.FltNo;
                        cps.Routes = x.Route;
                    }
                    else if (!string.IsNullOrEmpty(x.CanceledNo))
                    {
                        cps.Flts = x.CanceledNo;
                        cps.Routes = x.CanceledRoute;
                    }
                    context.CrewPickupSMS.Add(cps);
                    iddels.Add(new IdDel() { Id = x.Id, Ref = result9.ToString() });


                }
                context.SaveChanges();
                return Ok( iddels);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "    IN:" + ex.InnerException.Message;
                return Ok( msg);
            }

        }


    }

    public class RosterSMSDto
    {
        public List<int> Ids { get; set; }
        public DateTime Date { get; set; }
        public string UserName { get; set; }
    }
    public class IdDel
    {
        public int Id { get; set; }
        public string Ref { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public string TypeStr { get; set; }
        public string Username { get; set; }

    }
}
