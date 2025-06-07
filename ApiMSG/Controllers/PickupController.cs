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
    public class PickupController : ApiController
    {

        [Route("api/msg/pickup/notify/raimon")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostNotifyPickupRaimon(NotificationX dto)
        {
            try
            {
                var context = new ppa_vareshEntities();

                var employees = await context.ViewEmployeeSimples.Where(q => dto.Employees.Contains(q.Id)).ToListAsync();

                var empids = employees.Select(q => q.Id).ToList();
                var pickups = await context.CrewPickupSMS.Where(q => q.FlightId == dto.FlightId && empids.Contains(q.CrewId)).ToListAsync();

                List<IdDel> result = new List<IdDel>();
                //kiro
                int c1 = 0;
                foreach (var x in dto.Employees)
                {
                    if (dto.Names != null && dto.Names.Count > 0)
                    {
                        var name = dto.Names[c1];
                        var dtime = dto.Dates[c1];

                        // if (dtime == null && dto.TypeId > 10011)
                        //    dtime = (new DateTime()).ToUniversalTime();
                        string message = string.Empty;
                        switch (dto.TypeId)
                        {
                            case 10010:
                            case 10011:
                            case 10017:
                                if (dtime != null)
                                {
                                    var tzoffset = GetTimeOffset((DateTime)dtime);
                                    var _time2 = ((DateTime)dtime).AddMinutes(tzoffset);
                                    var _time = (_time2).Hour.ToString().PadLeft(2, '0') + ":" + (_time2).Minute.ToString().PadLeft(2, '0');
                                    message = dto.Message.Replace("<br/>", "\n").Replace("<br />", "\n").Replace("#Time", _time).Replace("()", _time);
                                }
                                break;
                            case 10012:
                            case 10013:
                                message = dto.Message.Replace("<br/>", "\n").Replace("<br />", "\n");
                                break;
                            default:
                                break;
                        }
                        var emplo = employees.Where(q => q.Id == x).FirstOrDefault();
                        if (emplo != null && !string.IsNullOrEmpty(message))
                        {
                            message = message.Replace("Flt.Crew", emplo.Name);

                            var _post_result = await PostDataAsyncRAIMON( emplo.Mobile , message);
                            await PostDataAsyncRAIMON("09124449584", message);
                            await PostDataAsyncRAIMON("09127065193", message);
                            context.SMSHistories.Add(new SMSHistory()
                            {
                                DateSent = DateTime.Now,
                                RecMobile = emplo.Mobile,
                                RecName = name,
                                Ref = _post_result.data.messageId.ToString(),
                                Text = message,
                                TypeId = dto.TypeId,
                                FlightId = dto.FlightId,
                                Sender = dto.SenderName,
                            });
                          
                            result.Add(new IdDel()
                            {
                                Id = emplo.Id,
                                Ref = _post_result.data.messageId.ToString(),
                                Status = _post_result.message,
                                Message = message,
                                TypeStr = getMessageTypeStr(dto.TypeId),

                            });
                            var his = pickups.FirstOrDefault(q => q.CrewId == emplo.Id);
                            if (his != null)
                                context.CrewPickupSMS.Remove(his);
                            context.CrewPickupSMS.Add(new CrewPickupSM()
                            {
                                CrewId = emplo.Id,
                                DateSent = DateTime.Now,
                                DateStatus = DateTime.Now,
                                FlightId = (int)dto.FlightId,
                                Message = message,
                                Pickup = (dto.TypeId == 10012 || dto.TypeId == 10013) ? null : dtime,
                                RefId = _post_result.data.messageId.ToString(),
                                Status = _post_result.message,
                                Type = dto.TypeId

                            });

                            //SendSMSNoSave("09124449584", message, emplo.Name);
                        }



                    }
                    c1++;
                }

               context.SaveChanges();
                return Ok(result);

            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                return Ok(msg);
            }



        }

        //        {
        //    "data": {
        //        "messageId": 189380327,
        //        "cost": 2.400
        //    },
        //    "status": 1,
        //    "message": "موفق"
        //}

        public class post_result
        {
            public int status { get; set; }
            public string message { get; set; }
            public post_result_data data { get; set; }

        }
        public class post_result_data
        {
            public Int64 messageId { get; set; }
            public double cost { get; set; }
        }

        string getMessageTypeStr(int id)
        {
            switch (id)
            {
                case 10010:
                    return "Pickup Time Notification";
                case 10011:
                    return "New Pickup Time Notification";
                case 10012:
                    return "Pickup Stand by Notification";
                case 10013:
                    return "اعلام تاخیر";
                case 10014:
                    return "Cancelling Notification";
                case 10015:
                    return "Delay Notification";
                case 10016:
                    return "Operation Notification";
                default:
                    return "-";
            }
        }
        public static int GetTimeOffset(DateTime dt)
        {

            try
            {
                return Convert.ToInt32(Math.Round(TimeZoneInfo.Local.GetUtcOffset(dt).TotalMinutes));
            }
            catch (Exception ex)
            {
                return 210;
            }

            //return 210;
            //if (dt >= new DateTime(2019, 1, 1) && dt < new DateTime(2019, 3, 22))
            //    return 210;
            //else if (dt >= new DateTime(2019, 3, 22) && dt < new DateTime(2019, 9, 22))
            //    return 270;
            //else if (dt >= new DateTime(2019, 9, 22) && dt < new DateTime(2020, 1, 1))
            //    return 210;


            //else if (dt >= new DateTime(2020, 1, 1) && dt < new DateTime(2020, 3, 21))
            //    return 210;
            //else if (dt >= new DateTime(2020, 3, 21) && dt < new DateTime(2020, 9, 21))
            //    return 270;
            //else if (dt >= new DateTime(2020, 9, 21) && dt < new DateTime(2021, 1, 1))
            //    return 210;
            //else
            //    return 270;


        }

        //https://api.sms.ir/v1/send?username=Raimon&password=2qeM5q8ze1M3j8aWy6iBfO6qpZds7g6V0w0OVSkoB2BXWxtq&line=30003472004992&mobile=09124449584&text=%22MESSAGE_TEXT%22

        public static async Task<post_result> PostDataAsyncRAIMON(string mobile, string text)
        {
            string url = "https://api.sms.ir/v1/send?username=Raimon&password=2qeM5q8ze1M3j8aWy6iBfO6qpZds7g6V0w0OVSkoB2BXWxtq&line=30003472004992&mobile=" + mobile + "&text='" + text + "'";

            var values = new Dictionary<string, string>
            {

            };

            var content = new FormUrlEncodedContent(values);


            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.PostAsync(url, content);
                    response.EnsureSuccessStatusCode(); // اگر وضعیت HTTP موفق نباشد، خطا می‌دهد
                    var response_text = await response.Content.ReadAsStringAsync();
                    var json = JsonConvert.DeserializeObject<post_result>(response_text);
                    return json;
                }
                catch (HttpRequestException ex)
                {
                    return new post_result()
                    {
                        message = "error in sending "+ex.Message,
                        status = -1000,
                    };
                }
            }
        }


    }



    public class NotificationX
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? CustomerId { get; set; }
        public string Message { get; set; }
        public DateTime DateSent { get; set; }
        public int? SenderId { get; set; }
        public string SenderName { get; set; }
        public bool SMS { get; set; }
        public bool Email { get; set; }
        public bool App { get; set; }
        public DateTime? DateSMSSent { get; set; }
        public DateTime? DateEmailSent { get; set; }
        public DateTime? DateAppSent { get; set; }
        public string SMSIssue { get; set; }
        public string EmailIssue { get; set; }
        public string AppIssue { get; set; }
        public DateTime? DateAppVisited { get; set; }
        public int TypeId { get; set; }
        public string Subject { get; set; }
        public int? ModuleId { get; set; }
        public int? FlightId { get; set; }

        List<int> employees;
        public List<int> Employees
        {
            get
            {
                if (employees == null)
                    employees = new List<int>();
                return employees;
            }
            set
            {
                employees = value;
            }
        }

        List<int?> fdps;
        public List<int?> FDPs
        {
            get
            {
                if (fdps == null)
                    fdps = new List<int?>();
                return fdps;
            }
            set
            {
                fdps = value;
            }
        }


        List<string> names;
        public List<string> Names
        {
            get
            {
                if (names == null)
                    names = new List<string>();
                return names;
            }
            set
            {
                names = value;
            }
        }

        List<string> names2;
        public List<string> Names2
        {
            get
            {
                if (names2 == null)
                    names2 = new List<string>();
                return names2;
            }
            set
            {
                names2 = value;
            }
        }

        List<string> mobiles2;
        public List<string> Mobiles2
        {
            get
            {
                if (mobiles2 == null)
                    mobiles2 = new List<string>();
                return mobiles2;
            }
            set
            {
                mobiles2 = value;
            }
        }

        List<string> messages2;
        public List<string> Messages2
        {
            get
            {
                if (messages2 == null)
                    messages2 = new List<string>();
                return messages2;
            }
            set
            {
                messages2 = value;
            }
        }


        List<DateTime?> dates;
        public List<DateTime?> Dates
        {
            get
            {
                if (dates == null)
                    dates = new List<DateTime?>();
                return dates;
            }
            set
            {
                dates = value;
            }
        }

    }
}
