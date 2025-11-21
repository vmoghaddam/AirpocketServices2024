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


namespace ApiMSG.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class MagfaController : ApiController
    {
        [Route("api/magfa/test")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetMagfaTest()
        {
            MagfaNew m = new MagfaNew();
            var smsResult = m.enqueue(1, "09124449584", "HELO APIMSG")[0];
            var refids = new List<Int64>() { smsResult };
            System.Threading.Thread.Sleep(5000);
            //var status = m.getStatus(refids);

            return Ok(refids);
        }
        [Route("api/mail/test/")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetMailTest()
        {
            MailHelper xxx = new MailHelper();
            xxx.SendTestAirpocket();
            return Ok(true);
        }

        public class response_asr
        {
            public int Id { get; set; }
            public int FlightId { get; set; }
            public DateTime FlightDate { get; set; }
            public string FlightNumber { get; set; }
            public string Route { get; set; }
            public string Register { get; set; }
            public string PIC { get; set; }
            public string P1Name { get; set; }
            public string IPName { get; set; }
            public string SIC { get; set; }
            public string P2Name { get; set; }
            public string Summary { get; set; }
            public int? P1Id { get; set; }
            public int? PICId { get; set; }
            public int? IPId { get; set; }
        }
        [Route("api/qa/notify/status/")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetNotifyStatus()
        {
            var context = new ppa_vareshEntities();
            var query = from x in context.qa_notification_history
                        join v in context.view_qa_notifiction_history on x.id equals v.id
                        where v.status!= "Delivered" && v.passed_hours<120

                        select x;
            var notifications = query.ToList();
            Magfa m = new Magfa();
            foreach (var n in notifications)
            {
                n.status = m.getStatus(Convert.ToInt64(n.ref_id));
            }
            context.SaveChanges();
            return Ok(notifications.Select(q => new { 
             q.id,
             q.status
            }).ToList());
        }

        [Route("api/qa/notify/resend/")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetNotifyResend()
        {
            var context = new ppa_vareshEntities();
            var query = from x in context.qa_notification_history
                        where x.counter < 3 && x.status != "Delivered"
                        orderby x.id descending
                        select x;
            var notifications = query.ToList();
            Magfa m = new Magfa();
            foreach(var n in notifications)
            {
                n.counter++;
                var smsResult = m.enqueue(1, n.rec_mobile, n.message_text)[0];
                n.ref_id = smsResult.ToString();
                n.date_send = DateTime.Now;
            }

            System.Threading.Thread.Sleep(15000);
            foreach(var n in notifications)
            {
                n.status = m.getStatus(Convert.ToInt64(n.ref_id));
            }
            context.SaveChanges();

            return Ok(notifications);
        }

        [Route("api/qa/notify/asr/v2/{id}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetNotifyASR2(int id)
        {
            Magfa m1 = new Magfa();
           var res= m1.enqueue(1, "09124449584", "asr test")[0];
            return Ok(res);

        }

            [Route("api/qa/notify/asr/{id}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetNotifyASR(int id)
        {
            try
            {
                var context = new ppa_vareshEntities();
                List<qa_notification_history> _result = new List<qa_notification_history>();
                var url = "https://fly.apiapsb.myaero.tech/api/asr/view/abs/" + id;
                using (WebClient webClient = new WebClient())
                {
                    webClient.Encoding = Encoding.UTF8;
                    var str = webClient.DownloadString(url);
                    response_asr asr = JsonConvert.DeserializeObject<response_asr>(str);
                    var pic = context.ViewProfiles.Where(q => q.Id == asr.PICId).FirstOrDefault();

                    //var pic_msg1 = "ضمن سپاس از ارسال گزارش، پس از بررسی و اقدامات انجام شده، حصول نتیجه در صورت لزوم به شما ابلاغ خواهد شد. با تشکر، مدیریت ایمنی و تضمین کیفیت هواپیمایی وارش";





                    List<string> prts = new List<string>();
                    prts.Add("New ASR Notification");
                    //prts.Add("Dear ");
                    prts.Add("Dear " + asr.PIC);
                    prts.Add("Please click on the below link to see details.");
                    prts.Add("https://fly.report.myaero.tech/frmreportview.aspx?type=17&fid=" + asr.FlightId);
                    prts.Add("Date: " + asr.FlightDate.ToString("yyyy-MM-dd"));
                    prts.Add("Route: " + asr.Route);
                    prts.Add("Register: " + asr.Register);
                    prts.Add("PIC: " + asr.PIC);
                    prts.Add("FO: " + asr.P2Name);
                    prts.Add("FP: " + asr.SIC);
                    prts.Add("Event Summary:");
                    prts.Add(asr.Summary);
                    

                    var text = String.Join("\n", prts);
                    List<qa_notification_history> nots = new List<qa_notification_history>();

                    var not_receivers = context.qa_notification_receiver.Where(q => q.is_active == true).ToList();

                    MagfaNew m1 = new MagfaNew();
                    foreach (var rec in not_receivers)
                    {
                        List<string> prts2 = new List<string>();
                        prts2.Add("New ASR Notification");
                        prts2.Add("Dear " + rec.rec_name);
                        prts2.Add("Please click on the below link to see details.");

                        prts2.Add("https://fly.report.myaero.tech/frmreportview.aspx?type=17&fid=" + asr.FlightId);
                        prts2.Add("Date: " + asr.FlightDate.ToString("yyyy-MM-dd"));
                        prts2.Add("Route: " + asr.Route);
                        prts2.Add("Register: EP-" + asr.Register);
                        prts2.Add("PIC: " + asr.PIC);
                        prts2.Add("FO: " + asr.P2Name);
                        prts2.Add("FP: " + asr.SIC);
                        prts2.Add("Event Summary:");
                        prts2.Add(asr.Summary);

                        var text2 = String.Join("\n", prts2);



                        List<string> mail_parts = new List<string>();

                        mail_parts.Add("<b>" + "New ASR Notification" + "</b><br/>");
                        mail_parts.Add("<b>" + "Dear " + rec.rec_name + "</b><br/>");
                        mail_parts.Add("Please click on the below link to see details.");

                        mail_parts.Add("https://fly.report.myaero.tech/frmreportview.aspx?type=17&fid=" + asr.FlightId + "<br/>");
                        mail_parts.Add("Date: " + "<b>" + asr.FlightDate.ToString("yyyy-MM-dd") + "</b><br/>");
                        mail_parts.Add("Route: " + "<b>" + asr.Route + "</b><br/>");
                        mail_parts.Add("Register: " + "<b>" + "EP-" + asr.Register + "</b><br/>");
                        mail_parts.Add("PIC: " + "<b>" + asr.PIC + "</b><br/>");
                        mail_parts.Add("FO: " + "<b>" + asr.P2Name + "</b><br/>");
                        mail_parts.Add("FP: " + "<b>" + asr.SIC + "</b><br/>");
                        mail_parts.Add("<b>" + "Event Summary:" + "</b><br/>");
                        mail_parts.Add(asr.Summary);
                        mail_parts.Add("<br/>");
                        mail_parts.Add("<br/>");
                        mail_parts.Add("Sent by AIRPOCKET" + "<br/>");
                        mail_parts.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));


                        var email_body = String.Join("\n", mail_parts);
                        if (!string.IsNullOrEmpty(rec.email))
                        {
                            MailHelper mail_helper = new MailHelper();
                            mail_helper.SendMailByAirpocket(rec.email, rec.rec_name, "ASR NOTIFICATION (FLT NO " + asr.FlightNumber + ", ROUTE " + asr.Route + ", DATE: " + asr.FlightDate.ToString("yyyy-MM-dd") + ")", email_body);

                        }
                        if (!string.IsNullOrEmpty(rec.email2))
                        {
                            MailHelper mail_helper = new MailHelper();
                            mail_helper.SendMailByAirpocket(rec.email2, rec.rec_name, "ASR NOTIFICATION (FLT NO " + asr.FlightNumber + ", ROUTE " + asr.Route + ", DATE: " + asr.FlightDate.ToString("yyyy-MM-dd") + ")", email_body);

                        }


                        var not_history = new qa_notification_history()
                        {
                            date_send = DateTime.Now,
                            entity_id = id,
                            entity_type = 8,
                            message_text = text2,
                            message_type = 1,
                            rec_id = rec.rec_id,
                            rec_mobile = rec.mobile,
                            rec_name = rec.rec_name,
                            counter = 0,

                        };
                        
                        var smsResult1 = m1.enqueue(1, not_history.rec_mobile, text2)[0];
                        not_history.ref_id = smsResult1.ToString();
                        _result.Add(not_history);
                        System.Threading.Thread.Sleep(2000);

                    }
                    //var not_history_pic = new qa_notification_history()
                    //{
                    //    date_send = DateTime.Now,
                    //    entity_id = id,
                    //    entity_type = 8,
                    //    message_text = pic_msg1,
                    //    message_type = 2,
                    //    rec_id = asr.PICId,
                    //    rec_mobile = pic.Mobile,
                    //    rec_name = pic.Name,
                    //    counter = 0,
                    //};
                    var not_history_pic2 = new qa_notification_history()
                    {
                        date_send = DateTime.Now,
                        entity_id = id,
                        entity_type = 8,
                        message_text = text,
                        message_type = 1,
                        rec_id = asr.PICId,
                        rec_mobile = pic.Mobile,
                        rec_name = pic.Name,
                        counter = 0,
                    };

                    //MagfaNew m1_pic = new MagfaNew();
                    //var m1_pic_result = m1_pic.enqueue(1, not_history_pic.rec_mobile, not_history_pic.message_text)[0];
                    //not_history_pic.ref_id = m1_pic_result.ToString();

                    MagfaNew m_pic = new MagfaNew();
                    var m_pic_result = m_pic.enqueue(1, not_history_pic2.rec_mobile, not_history_pic2.message_text)[0];
                    not_history_pic2.ref_id = m_pic_result.ToString();

                    //_result.Add(not_history_pic);
                    _result.Add(not_history_pic2);

                    System.Threading.Thread.Sleep(20000);
                    foreach(var x in _result)
                    {
                        MagfaNew m_status = new MagfaNew();
                        x.status = m_status.getStatus(Convert.ToInt64(x.ref_id));

                        context.qa_notification_history.Add(x);
                    }
                    //not_history.status = m1.getStatus(Convert.ToInt64(not_history.ref_id));

                    //context.qa_notification_history.Add(not_history);

                    //4539
                    //var not_history = new qa_notification_history()
                    //{
                    //    date_send = DateTime.Now,
                    //    entity_id = id,
                    //    entity_type = 8,
                    //    message_text = text,
                    //    message_type = 1,
                    //    rec_id = 4539,
                    //    rec_mobile = "09121246818",
                    //    rec_name = "ZAHRA GANJINEH",
                    //    counter=0,

                    //};
                    // Magfa m1 = new Magfa();
                    // var smsResult1 = m1.enqueue(1, "09121246818", text)[0];

                    //not_history.ref_id = smsResult1.ToString();
                    // Magfa m = new Magfa();
                    // var smsResult = m.enqueue(1, "09124449584", text)[0];



                    
                    // not_history_pic.rec_mobile = "09124449584";

                    //not_history_pic2.rec_mobile = "09124449584";

                  


                    



                    

                   // not_history_pic.status = m1_pic.getStatus(Convert.ToInt64(not_history_pic.ref_id));

                   // not_history_pic2.status = m_pic.getStatus(Convert.ToInt64(not_history_pic2.ref_id));

                    
                    //context.qa_notification_history.Add(not_history_pic);
                    //context.qa_notification_history.Add(not_history_pic2);




                    context.SaveChanges();

                    //_result.Add(not_history);
                   // _result.Add(not_history_pic);
                   // _result.Add(not_history_pic2);






                }

                return Ok(_result);
            }
            catch(Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "      " + ex.InnerException.Message;
                return Ok(msg);
            }
        }




        [Route("api/qa/feedback/first/{no}/{eid}/{type}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetQAFeedbackFirst(string no, string eid, string type)
        {
            var _no = no;

           

            try
            {
                var context = new ppa_vareshEntities();
                List<qa_notification_history> _result = new List<qa_notification_history>();
                

                using (WebClient webClient = new WebClient())
                {
                    webClient.Encoding = Encoding.UTF8;
                    try
                    {
                        if (eid != "-1" && _no == "-1")
                        {
                            var _eid = Convert.ToInt32(eid);
                            _no = context.ViewProfiles.Where(q => q.Id == _eid).FirstOrDefault().Mobile;
                        }
                    } catch(Exception ex) {
                        return Ok(ex);
                    }
                    var msg1 = "ضمن سپاس از ارسال گزارش، پس از بررسی و اقدامات انجام شده، حصول نتیجه در صورت لزوم به شما ابلاغ خواهد شد. با تشکر، مدیریت ایمنی و تضمین کیفیت هواپیمایی وارش";
                    var msg2 = "You have a new " + type + " Report. Please check your AIRPOCKET.";





                    MagfaNew m = new MagfaNew();
                    var smsResult = m.enqueue(1, _no, msg1)[0];
                    var refids = new List<Int64>() { smsResult };
                    
                    
                    var not_receivers = context.qa_notification_receiver.Where(q => q.is_active == true).ToList();

                    foreach (var rec in not_receivers)
                    {
                        var smsResult1 = m.enqueue(1, rec.mobile, msg2)[0];
                        var refids1 = new List<Int64>() { smsResult1 };
                    }
                   

                   
                    context.SaveChanges();

                    return Ok(new { refids, _no, no, eid });
                }

                
            }catch(Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "      " + ex.InnerException.Message;
                return Ok(msg);
            }
        }


        [Route("api/magfa/send/bulk")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetMagfaSendBulk()
        {
            var context = new ppa_vareshEntities();
            var refids = new List<Int64>();
            var rows = context.BulkMsgs.ToList();
            var message = rows.First().Message;
            foreach (var x in rows)
            {
                Magfa m = new Magfa();
                var smsResult = m.enqueue(1, x.Mobile, message)[0];
                x.RefId = smsResult;
                x.Message = message;
                refids.Add(smsResult);
            }

            context.SaveChanges();
            // var refids = new List<Int64>() { smsResult };
            // System.Threading.Thread.Sleep(5000);
            // var status = m.getStatus(refids);

            return Ok(refids);
        }


        [Route("api/magfa/status/bulk/{skip}/{take}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetMagfaStatusBulk(int skip, int take)
        {
            var context = new ppa_vareshEntities();
            Magfa m = new Magfa();
            var rows = context.BulkMsgs.ToList().OrderBy(q => q.Id).Skip(skip).Take(take).ToList();
            var refids = rows.Select(q => (Int64)q.RefId).ToList();
            var reslt = m.getStatus(refids);
            var c = 0;
            foreach (var x in rows)
            {
                x.Status = reslt[c];
                c++;
            }

            context.SaveChanges();
            // var refids = new List<Int64>() { smsResult };
            // System.Threading.Thread.Sleep(5000);
            // var status = m.getStatus(refids);

            return Ok(reslt);
        }
        [Route("api/magfa/bulk/username")]
        [AcceptVerbs("GET")]
        public IHttpActionResult SendUp()
        {
            var context = new ppa_vareshEntities();
            var rows = context.BulkMsgUsernames.ToList();


            foreach (var crew in rows)
            {



                // if (user != null)
                {
                    var strs = new List<string>();
                    strs.Add("Dear " + (crew.FirstName + " " + crew.LastName).ToUpper() + ", ");
                    strs.Add("Please visit your CrewPocket account To see your roster, notifications, etc.");
                    strs.Add("You have to change your password after first login.");
                    strs.Add("https://fms.airpocket.click");
                    strs.Add("https://fms.kishair.aero/cp");

                    strs.Add("Username: " + crew.UserName);
                    // strs.Add("Password: " + /*(string.IsNullOrEmpty(crew.Telegram) ?*/ "1234@aA" /*: crew.Telegram*/));
                    strs.Add("Password: 123456");
                    strs.Add("Flight Operation Department");
                    var text = String.Join("\n", strs);
                    // SendSMS(crew.Mobile, text, crew.Name);
                    // SendSMS(/*crew.Mobile*/"09122007720", text, crew.Name);
                    // SendSMS(/*crew.Mobile*/"09123938451", text, crew.Name);
                    Magfa m = new Magfa();
                    Magfa m2 = new Magfa();

                    var smsResult = m.enqueue(1, crew.Mobile, text)[0];
                    if (crew.Group == "P1")
                    {

                        var smsResult2 = m2.enqueue(1, "09124449584", text + "   MOBILE:" + smsResult.ToString())[0];
                    }



                }
                //else
                //{
                //    Magfa m2 = new Magfa();
                //    var smsResult2 = m2.enqueue(1, "09124449584", crew.PersonId + "  " + crew.Name + "  null")[0];
                //}

            }

            return Ok(true);
        }



    }
}
