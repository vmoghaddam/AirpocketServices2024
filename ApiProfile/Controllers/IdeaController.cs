using ApiProfile.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace ApiProfile.Controllers
{
    public class IdeaController : ApiController
    {
        [Route("api/idea/unique/sync/{year}/{user}/{password}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetIdeaUniqueSync(string year, string user, string password)
        {
            if (user != "vahid")
                return BadRequest("Not Authenticated");
            if (password != "Chico1359")
                return BadRequest("Not Authenticated");

            // string apiUrl = "http://fleet.caspianairlines.com/airpocketexternal/api/idea/alt/unique/obj/"+year;
            string apiUrl = "http://172.16.103.37/airpocketexternal/api/idea/alt/unique/obj/" + year;
            var input = new
            {

            };
            string inputJson = JsonConvert.SerializeObject(input);
            WebClient client = new WebClient();
            client.Headers["Content-type"] = "application/json";
            client.Encoding = Encoding.UTF8;
            string json = client.DownloadString(apiUrl);
            json = json.Replace("?xml", "d_xml").Replace("soap:Envelope", "d_envelope").Replace("soap:Body", "d_body")
                .Replace("@xmlns:soap", "d_xmlns_soap").Replace("@xmlns:xsi", "d_xmlns_xsi").Replace("@xmlns:xsd", "d_xmlns_xsd")
                .Replace("@xmlns", "d_xmlns")
                .Replace("GetTotalDataJsonResponse", "response")
                .Replace("GetTotalDataJsonResult", "result");
            var obj = JsonConvert.DeserializeObject<IdeaResultUnique>(json);

            var response = obj.d_envelope.d_body.response.result;
            var responseJson = JsonConvert.DeserializeObject<List<IdeaUniqueX>>(response);
            using (var context = new Models.dbEntities())
            {
                context.Database.CommandTimeout = 180;
                context.Configuration.AutoDetectChangesEnabled = false;
                context.Configuration.ValidateOnSaveEnabled = false;
                // context.IdeaUniques.RemoveRange(context.IdeaUniques);
                // context.SaveChanges();
                context.Database.ExecuteSqlCommand("TRUNCATE TABLE [IdeaUnique]");
            }

            using (var context = new Models.dbEntities())
            {
                context.Database.CommandTimeout = 180;
                context.Configuration.AutoDetectChangesEnabled = false;
                context.Configuration.ValidateOnSaveEnabled = false;
                //context.IdeaUniques.RemoveRange(context.IdeaUniques);
                foreach (var item in responseJson)
                {
                    var dbItem = new IdeaUnique()
                    {
                        ClassID = item.ClassID,
                        CourseCode = item.courseCode,
                        CourseTitle = item.CourseTitle,
                        DateBegin = item.BeginDate2,
                        DateCreate = DateTime.Now,
                        DateEnd = item.EndDate2,
                        IdeaId = item.ID,
                        NID = item.nid,
                        PID = "",
                        DateExpire = item.ExpireDate2

                    };

                    context.IdeaUniques.Add(dbItem);

                }

                context.SaveChanges();
            }



            return Ok(responseJson);
        }

        DateTime getExp2 (DateTime? _issue1,DateTime? _exp1,DateTime? _exp_old)
        {
            DateTime issue1 = (DateTime)_issue1;
            DateTime exp1 = (DateTime)_exp1;
            DateTime exp_old = (DateTime)_exp_old;
            var interval = (exp1 - issue1).TotalDays ;
            var exp2 = exp_old.AddDays(interval);
            if (exp2 > exp1)
                return exp2;
            else
                return exp1;

        }

        [Route("api/trn/airpocket/certificates/update2/{user}/{password}/{type}/{grp}/{nid}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetUpdateAirpocketCertificates(string user, string password, string type, string grp, string nid)
        {
            if (user != "vahid")
                return BadRequest("Not Authenticated");
            if (password != "Chico1359")
                return BadRequest("Not Authenticated");

            using (var context = new Models.dbEntities())
            {
                context.Database.CommandTimeout = 1000;
                var people = context.People.ToList();
                var employees = context.ViewEmployeeLights.ToList();


                var ideaRecords = context.ViewIdeaUniqueLasts.ToList();
                if (type != "-1")
                    ideaRecords = ideaRecords.Where(q => q.CourseType == type).ToList();
                if (grp != "-1")
                {
                    var _emps = employees.Where(q => q.JobGroup == grp).Select(q => q.NID.Replace("-", "")).ToList();
                    ideaRecords = ideaRecords.Where(q => _emps.Contains(q.NID)).ToList();
                }
                if (nid != "-1")
                {

                    ideaRecords = ideaRecords.Where(q => q.NID == nid).ToList();
                }
                foreach (var rec in ideaRecords)
                {
                    if (rec.CrewId != null)
                    {
                        if (rec.CourseType == "CRM")
                        {
                            var person = people.FirstOrDefault(q => q.NID == rec.NID);
                            if (person != null && (person.UpsetRecoveryTrainingExpireDate == null || rec.DateExpire > person.UpsetRecoveryTrainingExpireDate))
                            {
                                person.UpsetRecoveryTrainingIssueDate = rec.DateEnd;
                                person.UpsetRecoveryTrainingExpireDate = getExp2(rec.DateEnd, rec.DateExpire, person.UpsetRecoveryTrainingExpireDate);
                            }
                        }
                        if (rec.CourseType == "CCRM")
                        {
                            var person = people.FirstOrDefault(q => q.NID == rec.NID);
                            if (person != null && (person.CCRMExpireDate == null || rec.DateExpire > person.CCRMExpireDate))
                            {
                                person.CCRMIssueDate = rec.DateEnd;
                                person.CCRMExpireDate = getExp2(rec.DateEnd, rec.DateExpire, person.CCRMExpireDate);
                            }
                        }
                        if (rec.CourseType == "SMS")
                        {
                            var person = people.FirstOrDefault(q => q.NID == rec.NID);
                            if (person != null && (person.SMSExpireDate == null || rec.DateExpire > person.SMSExpireDate))
                            {
                                person.SMSIssueDate = rec.DateEnd;
                                person.SMSExpireDate = getExp2(rec.DateEnd, rec.DateExpire, person.SMSExpireDate);
                            }
                        }
                        if (rec.CourseType == "FIRSTAID")
                        {
                            var person = people.FirstOrDefault(q => q.NID == rec.NID);
                            if (person != null && (person.FirstAidExpireDate == null || rec.DateExpire > person.FirstAidExpireDate))
                            {
                                person.FirstAidIssueDate = rec.DateEnd;
                                person.FirstAidExpireDate = getExp2(rec.DateEnd, rec.DateExpire, person.FirstAidExpireDate);
                            }
                        }

                        if (rec.CourseType == "HOT-WX")
                        {
                            var person = people.FirstOrDefault(q => q.NID == rec.NID);
                            if (person != null && (person.HotWeatherOperationExpireDate == null || rec.DateExpire > person.HotWeatherOperationExpireDate))
                            {
                                person.HotWeatherOperationIssueDate = rec.DateEnd;
                                person.HotWeatherOperationExpireDate = getExp2(rec.DateEnd, rec.DateExpire, person.HotWeatherOperationExpireDate);
                            }
                        }

                        if (rec.CourseType == "COLD-WX")
                        {
                            var person = people.FirstOrDefault(q => q.NID == rec.NID);
                            if (person != null && (person.ColdWeatherOperationExpireDate == null || rec.DateExpire > person.ColdWeatherOperationExpireDate))
                            {
                                person.ColdWeatherOperationIssueDate = rec.DateEnd;
                                person.ColdWeatherOperationExpireDate = getExp2(rec.DateEnd, rec.DateExpire, person.ColdWeatherOperationExpireDate);
                            }
                        }


                        if (rec.CourseType == "SEPT")
                        {
                            var person = people.FirstOrDefault(q => q.NID == rec.NID);
                            if (person != null && (person.SEPTPExpireDate == null || rec.DateExpire > person.SEPTPExpireDate))
                            {
                                person.SEPTPIssueDate = rec.DateEnd;
                                person.SEPTPExpireDate = getExp2(rec.DateEnd, rec.DateExpire, person.SEPTPExpireDate);
                            }
                        }
                        if (rec.CourseType == "SEPT-TEORY")
                        {
                            var person = people.FirstOrDefault(q => q.NID == rec.NID);
                            if (person != null && (person.SEPTExpireDate == null || rec.DateExpire > person.SEPTExpireDate))
                            {
                                person.SEPTIssueDate = rec.DateEnd;
                                person.SEPTExpireDate = getExp2(rec.DateEnd, rec.DateExpire, person.SEPTExpireDate);
                            }
                        }
                        if (rec.CourseType == "AVSEC")
                        {
                            var person = people.FirstOrDefault(q => q.NID == rec.NID);
                            if (person != null && (person.AviationSecurityExpireDate == null || rec.DateExpire > person.AviationSecurityExpireDate))
                            {
                                person.AviationSecurityIssueDate = rec.DateEnd;
                                person.AviationSecurityExpireDate = getExp2(rec.DateEnd, rec.DateExpire, person.AviationSecurityExpireDate);
                            }
                        }
                        if (rec.CourseType == "DG")
                        {
                            var person = people.FirstOrDefault(q => q.NID == rec.NID);
                            if (person != null && (person.DangerousGoodsExpireDate == null || rec.DateExpire > person.DangerousGoodsExpireDate))
                            {
                                person.DangerousGoodsIssueDate = rec.DateEnd;
                                person.DangerousGoodsExpireDate = getExp2(rec.DateEnd, rec.DateExpire, person.DangerousGoodsExpireDate);
                            }
                        }


                        if (rec.CourseType == "RE-TRAIN")
                        {
                            var person = people.FirstOrDefault(q => q.NID == rec.NID);
                            if (person != null && (person.RecurrentExpireDate == null || rec.DateExpire > person.RecurrentExpireDate))
                            {

                                person.RecurrentIssueDate = rec.DateBegin != null ? rec.DateBegin : rec.DateEnd;
                                person.RecurrentExpireDate = getExp2(rec.DateEnd, rec.DateExpire, person.RecurrentExpireDate);

                                person.FirstAidIssueDate = rec.DateBegin != null ? rec.DateBegin : rec.DateEnd;
                                person.FirstAidExpireDate = getExp2(rec.DateEnd, rec.DateExpire, person.FirstAidExpireDate);

                                person.UpsetRecoveryTrainingIssueDate = rec.DateBegin != null ? rec.DateBegin : rec.DateEnd;
                                person.UpsetRecoveryTrainingExpireDate = getExp2(rec.DateEnd, rec.DateExpire, person.UpsetRecoveryTrainingExpireDate);

                                person.SEPTIssueDate = rec.DateBegin != null ? rec.DateBegin : rec.DateEnd;
                                person.SEPTExpireDate = getExp2(rec.DateEnd, rec.DateExpire, person.SEPTExpireDate);



                            }
                        }

                        if (rec.CourseType == "FMT" || rec.CourseType == "FRM")
                        {
                            var person = people.FirstOrDefault(q => q.NID == rec.NID);
                            if (person != null && (person.DateIssueNDT==null || rec.DateEnd > person.DateIssueNDT))
                            {
                                person.DateIssueNDT = rec.DateEnd;
                                person.DateExpireNDT = getExp2(rec.DateEnd, rec.DateExpire, person.DateExpireNDT);
                            }
                        }

                        if (rec.CourseType == "GRT" || rec.CourseType == "G&RT")
                        {
                            var person = people.FirstOrDefault(q => q.NID == rec.NID);
                            if (person != null && (person.DateCaoCardIssue==null || rec.DateEnd > person.DateCaoCardIssue))
                            {
                                person.DateCaoCardIssue = rec.DateEnd;
                                person.DateCaoCardExpire = getExp2(rec.DateEnd, rec.DateExpire, person.DateCaoCardExpire);
                            }
                        }


                    }

                }

                var history = new ThirdPartySyncHistory()
                {
                    App = "IDEA",
                    DateSync = DateTime.Now,
                    Remark = ideaRecords.Count + " Records Proccessed.",
                };
                context.ThirdPartySyncHistories.Add(history);
                context.SaveChanges();
                return Ok(history);
            }



        }




    }

    public class Session
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        public DateTime? DateFromUtc { get; set; }
        public DateTime? DateToUtc { get; set; }

        public string Remark { get; set; }

    }




    public class IdeaResultUnique
    {
        public object d_xml { get; set; }
        public IdeaEnvelopeUnique d_envelope { get; set; }

    }
    public class IdeaEnvelopeUnique
    {
        public string d_xmlns_soap { get; set; }
        public string d_xmlns_xsi { get; set; }
        public string d_xmlns_xsd { get; set; }
        public IdeaBodyUnique d_body { get; set; }


    }
    public class IdeaBodyUnique
    {
        public IdeaResponseUnique response { get; set; }
    }
    public class IdeaResponseUnique
    {
        public string d_xmlns { get; set; }
        //public List<IdeaSession> result { get; set; }
        public string result { get; set; }
    }

    //{\"BeginDate\":\"22/01/2020 12:00:00 ق.ظ\",\"EndDate\":\"22/01/2020 12:00:00 ق.ظ\",\"expire\":\"21/01/2021 12:00:00 ق.ظ\"}
    public class IdeaUniqueX
    {
        public string ID { get; set; }
        public string ClassID { get; set; }
        public string nid { get; set; }
        public string courseCode { get; set; }
        public string CourseTitle { get; set; }
        public string BeginDate { get; set; }
        public string EndDate { get; set; }
        public string expire { get; set; }

        public DateTime? BeginDate2
        {
            //19/06/2021 12:00:00 ق.ظ
            get
            {
                if (string.IsNullOrEmpty(this.BeginDate))
                    return null;
                var _date = this.BeginDate;
                try
                {
                    var dd = Convert.ToInt32(_date.Substring(0, 2));
                    var mm = Convert.ToInt32(_date.Substring(3, 2));
                    var yy = Convert.ToInt32(_date.Substring(6, 4));
                    return (new DateTime(yy, mm, dd)).Date;

                }
                catch (Exception ex)
                {
                    return null;
                }


            }
        }
        public DateTime? EndDate2
        {
            get
            {
                if (string.IsNullOrEmpty(this.EndDate))
                    return null;
                var _date = this.EndDate;
                try
                {
                    var dd = Convert.ToInt32(_date.Substring(0, 2));
                    var mm = Convert.ToInt32(_date.Substring(3, 2));
                    var yy = Convert.ToInt32(_date.Substring(6, 4));
                    return (new DateTime(yy, mm, dd)).Date;

                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }
        public DateTime? ExpireDate2
        {
            //19/06/2021 12:00:00 ق.ظ
            get
            {
                if (string.IsNullOrEmpty(this.expire))
                    return null;
                var _date = this.expire;
                try
                {
                    var dd = Convert.ToInt32(_date.Substring(0, 2));
                    var mm = Convert.ToInt32(_date.Substring(3, 2));
                    var yy = Convert.ToInt32(_date.Substring(6, 4));
                    return (new DateTime(yy, mm, dd)).Date;

                }
                catch (Exception ex)
                {
                    return null;
                }


            }
        }

    }


    public class IdeaResultAll
    {
        public object d_xml { get; set; }
        public IdeaEnvelopeAll d_envelope { get; set; }

    }
    public class IdeaEnvelopeAll
    {
        public string d_xmlns_soap { get; set; }
        public string d_xmlns_xsi { get; set; }
        public string d_xmlns_xsd { get; set; }
        public IdeaBodyUnique d_body { get; set; }


    }
    public class IdeaBodyAll
    {
        public IdeaResponseUnique response { get; set; }
    }
    public class IdeaResponseAll
    {
        public string d_xmlns { get; set; }
        //public List<IdeaSession> result { get; set; }
        public string result { get; set; }
    }

    //{\"BeginDate\":\"22/01/2020 12:00:00 ق.ظ\",\"EndDate\":\"22/01/2020 12:00:00 ق.ظ\",\"expire\":\"21/01/2021 12:00:00 ق.ظ\"}
    public class IdeaAllX
    {
        public string id { get; set; }
        public string ClassID { get; set; }
        public string nid { get; set; }
        public string courseCode { get; set; }
        public string CourseTitle { get; set; }
        public string BeginDate { get; set; }
        public string EndDate { get; set; }
        public string expire { get; set; }

        public DateTime? BeginDate2
        {
            //19/06/2021 12:00:00 ق.ظ
            get
            {
                if (string.IsNullOrEmpty(this.BeginDate))
                    return null;
                var _date = this.BeginDate;
                try
                {
                    var dd = Convert.ToInt32(_date.Substring(0, 2));
                    var mm = Convert.ToInt32(_date.Substring(3, 2));
                    var yy = Convert.ToInt32(_date.Substring(6, 4));
                    return (new DateTime(yy, mm, dd)).Date;

                }
                catch (Exception ex)
                {
                    return null;
                }


            }
        }
        public DateTime? EndDate2
        {
            get
            {
                if (string.IsNullOrEmpty(this.EndDate))
                    return null;
                var _date = this.EndDate;
                try
                {
                    var dd = Convert.ToInt32(_date.Substring(0, 2));
                    var mm = Convert.ToInt32(_date.Substring(3, 2));
                    var yy = Convert.ToInt32(_date.Substring(6, 4));
                    return (new DateTime(yy, mm, dd)).Date;

                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public DateTime? ExpireDate2
        {
            //19/06/2021 12:00:00 ق.ظ
            get
            {
                if (string.IsNullOrEmpty(this.expire))
                    return null;
                var _date = this.expire;
                try
                {
                    var dd = Convert.ToInt32(_date.Substring(0, 2));
                    var mm = Convert.ToInt32(_date.Substring(3, 2));
                    var yy = Convert.ToInt32(_date.Substring(6, 4));
                    return (new DateTime(yy, mm, dd)).Date;

                }
                catch (Exception ex)
                {
                    return null;
                }


            }
        }

    }

}
