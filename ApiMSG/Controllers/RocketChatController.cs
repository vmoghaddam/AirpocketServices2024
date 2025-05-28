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
    public class RocketChatController : ApiController
    {
        [Route("api/msg/rocketchat/notify/duties")]
        [AcceptVerbs("POST")]
        public async  Task<IHttpActionResult> PostNotifyDuties(RosterSMSDto dto)
        {
            try
            {
                var context = new ppa_vareshEntities();

                var Ids = dto.Ids;
                var date = dto.Date.Date;
                var username = dto.UserName;

                var fdps = context.FDPs.Where(q => Ids.Contains(q.Id) /*&& q.DutyType != 5000*/).ToList();
                foreach (var f in fdps)
                {
                    if (f.DateConfirmed == null)
                    {
                        f.DateConfirmed = DateTime.Now;
                        f.ConfirmedBy = username;
                    }

                }

                var query = (from x in context.ViewDutiyRocketChats
                             where Ids.Contains(x.Id) /*&& x.DutyType != 5000*/
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
                                 ,x.user_id
                                 ,x.user_name
                             }

                                   ).Distinct().ToList();
                var _fids = query.Select(q => (Nullable<int>)q.Id).ToList();
                var histories = context.SMSHistories.Where(q => _fids.Contains(q.ResId)).ToList();

                List<IdDel> iddels = new List<IdDel>();
                var offs = new List<int>() { 100009, 100020, 100021, 100022, 100023 };
                foreach (var x in query)
                {

                    List<string> strs = new List<string>();
                    List<string> token2s = new List<string>();
                    var token = "";
                    var token3 = "";

                    strs.Add(ConfigurationManager.AppSettings["airline"] + " Airlines");
                    strs.Add("Dear " + x.Name + ",");
                    token = x.Name;

                    var day = (DateTime)x.DateLocal;
                    var _start = (DateTime)x.Start;
                    var _end = (DateTime)x.End;
                    if (x.DutyType == 1165)
                    {
                        _start = (DateTime)x.STDLocal;
                        _end = (DateTime)x.STALocal;
                    }
                    var dayStr = day.ToString("ddd") + " " + day.Year + "-" + day.Month + "-" + day.Day;
                    var datesent = DateTime.Now.Year + "/" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "/" + DateTime.Now.Day.ToString().PadLeft(2, '0') + " " + DateTime.Now.Hour.ToString().PadLeft(2, '0') + ":" + DateTime.Now.Minute.ToString().PadLeft(2, '0');
                    token3 = datesent;

                    if (!offs.Contains(x.DutyType))
                    {
                        strs.Add(x.DutyTypeTitle);
                        strs.Add(dayStr);

                        token2s.Add(x.DutyTypeTitle);
                        token2s.Add(dayStr);

                        if (x.DutyType == 1165)
                        {
                            strs.Add(x.Route);
                            strs.Add(x.FltNo);

                            token2s.Add(x.Route);
                            token2s.Add(x.FltNo);
                        }
                        else
                        {
                            if (!String.IsNullOrEmpty(x.Remark))
                            {
                                strs.Add(x.Remark);
                                token2s.Add(x.Remark);
                            }
                        }
                        strs.Add("Start " + _start.ToString("ddd yy-MMM-dd HH:mm"));
                        strs.Add("End " + _end.ToString("ddd yy-MMM-dd HH:mm"));

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

                    var text = String.Join("\n", strs);
                    /// var result9 = m.enqueue(1, x.Mobile, text)[0];
                    var result9 = await PostDataAsync(x.user_name, text);


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
                        Ref = "ROCKET CHAT "+ result9.ToString(),
                        Text = text,
                        TypeId = 10,
                        ResId = x.Id,
                        ResFlts = !string.IsNullOrEmpty(x.FltNo) ? x.FltNo : x.Remark,
                        ResDate = date,
                        ResStr = "ROCKET CHAT ",



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
                        RefId = "ROCKET CHAT "+ result9.ToString(),
                        Status = "ROCKET CHAT ",
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
                    iddels.Add(new IdDel() { Id = x.Id, Ref = result9.ToString(), Username=x.user_name });


                }

               //context.SaveChanges();

                return Ok(iddels);

            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                return Ok(msg);
            }



        }

        [Route("api/msg/rocketchat/test")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetTest()
        {
            var context = new ppa_vareshEntities();
            var users=context.AspNetUsers.Select(q=>q.UserName).ToList();
            List<IdDel> result=new List<IdDel>();
            foreach (var user in users)
            {
                var result9 = await PostDataAsync(user, "TEST");
                result.Add(new IdDel() { Username=user, Message=result9});
            }
            result=result.Where(q=>q.Message.Contains("Bad Request")).OrderBy(q=>q.Username).ToList();
            return Ok(result.Select(q=>q.Username).ToList());
        }
        public static async Task<string> PostDataAsync(  string username, string text)
        {
            string url = "https://chat.avaair.ir/hooks/68248e66e4b014b555ce42c7/XPMrY5PnJmsQgaZJMFGCCapZpmhKJfZHkveWHMir7Hs8Fmpb";
            var values = new Dictionary<string, string>
            {
              { "username", username.ToLower() },
              { "text", text }
            };

            var content = new FormUrlEncodedContent(values);

            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.PostAsync(url, content);
                    response.EnsureSuccessStatusCode(); // اگر وضعیت HTTP موفق نباشد، خطا می‌دهد
                    return await response.Content.ReadAsStringAsync();
                }
                catch (HttpRequestException ex)
                {
                    return ex.Message ;
                }
            }
        }
    }



}
