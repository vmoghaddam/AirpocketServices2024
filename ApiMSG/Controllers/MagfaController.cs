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
            var smsResult= m.enqueue(1, "09124449584", "HELO APIMSG")[0];
            var refids = new List<Int64>() { smsResult };
            System.Threading.Thread.Sleep(5000);
            //var status = m.getStatus(refids);

            return Ok(refids);
        }

        [Route("api/qa/feedback/first/{no}/{eid}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetQAFeedbackFirst(string no,string eid)
        {
            var _no = no;
            
            if (eid != "-1" && _no=="-1")
            {
                try
                {
                    var _eid = Convert.ToInt32(eid);
                    var context = new ppa_vareshEntities();
                    _no = context.ViewProfiles.Where(q => q.Id == _eid).FirstOrDefault().Mobile;
                }
                catch(Exception ex)
                {

                }
               
            }
            var msg = "ضمن سپاس از ارسال گزارش، پس از بررسی و اقدامات انجام شده، حصول نتیجه در صورت لزوم به شما ابلاغ خواهد شد. با تشکر، مدیریت ایمنی و تضمین کیفیت هواپیمایی وارش";
            MagfaNew m = new MagfaNew();
            var smsResult = m.enqueue(1, _no, msg)[0];
            var refids = new List<Int64>() { smsResult };
            System.Threading.Thread.Sleep(5000);
            //var status = m.getStatus(refids);

            return Ok(new { refids ,_no,no,eid});
        }


        [Route("api/magfa/send/bulk")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetMagfaSendBulk()
        {
            var context = new ppa_vareshEntities();
            var refids = new List<Int64>() ;
            var rows = context.BulkMsgs.ToList();
            var message = rows.First().Message;
            foreach(var x in rows)
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
        public IHttpActionResult GetMagfaStatusBulk(int skip,int take)
        {
            var context = new ppa_vareshEntities();
            Magfa m = new Magfa();
            var rows = context.BulkMsgs.ToList().OrderBy(q=>q.Id).Skip(skip).Take(take).ToList();
            var refids = rows.Select(q =>(Int64) q.RefId).ToList();
            var reslt=m.getStatus(refids);
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
            var rows = context.BulkMsgUsernames .ToList();
             

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
