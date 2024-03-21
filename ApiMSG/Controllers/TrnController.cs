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
    public class TrnController : ApiController
    {

        [Route("api/notify/trn/group/expiring/{m}/{n}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetNotifyTrnGroupExpiring(int m, int n)
        {
            try
            {
                var context = new ppa_vareshEntities();
                var query_expiring = from x in context.ViewCertificateHistories
                                     where x.RankOrder == 1 && x.Remain >= m && x.Remain <= n && x.InActive == false //&& x.PersonId== 350
                                     group x by new { x.JobGroup, x.JobGroupMain, x.JobGroupRoot, x.CertificateType, x.CertificateTypeId2 } into grp
                                     select new cer_history()
                                     {
                                         CertificateType = grp.Key.CertificateType,
                                         CertificateTypeId = grp.Key.CertificateTypeId2,
                                         Count = grp.Count(),
                                         JobGroup = grp.Key.JobGroup,
                                         JobGroupMain = grp.Key.JobGroupMain,
                                         JobGroupRoot = grp.Key.JobGroupRoot,
                                         Items=grp.ToList()
                                         

                                     };
                var type = m.ToString() + "-" + n.ToString();
                var expiring = query_expiring.ToList();
                foreach (var x in expiring)
                {
                    x.KeyStr = x.JobGroupMain
                    + "-"
                    + x.JobGroup
                    + "-"
                    + x.CertificateTypeId
                    + "-"
                    + type;
                }

                var db_managers = context.Managers.ToList();
                var db_managers_grps = context.ManagerGroups.ToList();
                var db_man_ids = db_managers.Select(q => q.EmployeeId).ToList();
                var profiles = context.ViewProfiles.Where(q => db_man_ids.Contains(q.Id)).Select(q => new { q.FirstName, q.LastName, q.Mobile ,q.Id}).ToList();

                
                //filter by manager
                List<manager_mail> managers = new List<manager_mail>() {
                     new manager_mail(){ Email="v.moghaddam59@gmail.com",AllGroups=true, Name="AMIN DAVARI", NID="1"},
                      new manager_mail(){ Email="v.moghaddam59@gmail.com",   Name="MARYAM KAKO", NID="2", JobGroupRoot=new List<string>(){ "QA" } },
                    new manager_mail(){ Email="v.moghaddam59@gmail.com",   Name="FARZAD TOOFAN", NID="2", JobGroupRoot=new List<string>(){"Cockpit","Cabin","F/D","GRND","Flight Control"} },
                    new manager_mail(){ Email="v.moghaddam59@gmail.com",   Name="MASOUD KHAMSEH", NID="2", JobGroupRoot=new List<string>(){ "Cabin" } },
                   new manager_mail(){ Email="v.moghaddam59@gmail.com",   Name="H. ASHRAF OGHALAEI ", NID="2", JobGroupRoot=new List<string>(){ "F/D" } },
                   new manager_mail(){ Email="v.moghaddam59@gmail.com",   Name="HOMAYOUN SALIMI NOURI ", NID="2", JobGroupRoot=new List<string>(){ "MAINTENANCE","CAMO" } },
                    new manager_mail(){ Email="v.moghaddam59@gmail.com",   Name="HOSSEIN SHAYAN", NID="2", JobGroupRoot=new List<string>(){ "CAMO" } },
                    new manager_mail(){ Email="v.moghaddam59@gmail.com",   Name="FARHAD GERVEHEI", NID="2", JobGroupRoot=new List<string>(){ "MAINTENANCE" } },
                     new manager_mail(){ Email="v.moghaddam59@gmail.com",   Name="KAMAL ZAREI", NID="2", JobGroupRoot=new List<string>(){ "SECURITY" } },
                     
                       new manager_mail(){ Email="v.moghaddam59@gmail.com",   Name="REZA SADIEH", NID="2", JobGroupRoot=new List<string>(){ "LEGAL" } },
                         new manager_mail(){ Email="v.moghaddam59@gmail.com",   Name="MAHDIEH GHORBANIAN", NID="2", JobGroupRoot=new List<string>(){ "FINANCIAL" } },
                         new manager_mail(){ Email="v.moghaddam59@gmail.com",   Name="JAVAD TANHA", NID="2", JobGroupRoot=new List<string>(){ "IT" } },
                         new manager_mail(){ Email="v.moghaddam59@gmail.com",   Name="SAEID NASSIRI", NID="2", JobGroupRoot=new List<string>(){ "COMM","GRND" } },
                         new manager_mail(){ Email="v.moghaddam59@gmail.com",   Name="REZA HAJISADEGHI", NID="2", JobGroupRoot=new List<string>(){ "GRND" } },
                         new manager_mail(){ Email="v.moghaddam59@gmail.com",   Name="MEYSAM MOHAMMADI ", NID="2", JobGroupRoot=new List<string>(){ "HR" } },
                          new manager_mail(){ Email="v.moghaddam59@gmail.com",   Name="ALI ASKAR HALVAEI  ", NID="2", JobGroupRoot=new List<string>(){ "HR" } },


                   //HOMAYOUN SALIMI NOURI 
                };

                managers = new List<manager_mail>();
                foreach(var _mng in db_managers)
                {
                    var _grps = db_managers_grps.Where(q => q.ManagerId == _mng.EmployeeId).Select(q=>q.ProfileGroup).ToList();
                    var profile = profiles.FirstOrDefault(q => q.Id == _mng.EmployeeId);
                    var mng_email = new manager_mail()
                    {
                        Email =  _mng.Email,
                        Name = profile.FirstName + " " + profile.LastName,
                        JobGroupRoot = _grps,
                        AllGroups = _grps.Contains("ALL"),
                    };
                    managers.Add(mng_email);

                }

                ////////////
                var o_expiring = expiring.ToList();
                var smtp = new SmtpClient
                {
                    //EnableSsl=true,
                    Host = "mail.flypersia.aero",
                    Port = 25, //Convert.ToInt32(dispatchEmailPort),
                    EnableSsl = false,
                    //TargetName = "STARTTLS/Mail.flypersia.aero",
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("airpocket@flypersia.aero", "1234@aA"),

                };
                smtp.Timeout = 60000;



                foreach (var _man in managers)
                {
                    if (_man.AllGroups)
                    {
                         expiring = o_expiring.OrderBy(q => q.JobGroupMain).ThenBy(q => q.JobGroup).ThenBy(q => q.CertificateType).ToList();
                    }
                    else
                    {
                        expiring = o_expiring.Where(q => _man.JobGroupRoot.Contains(q.JobGroupRoot) || _man.JobGroupRoot.Contains(q.JobGroup)).OrderBy(q => q.JobGroupMain).ThenBy(q => q.JobGroup).ThenBy(q => q.CertificateType).ToList();
                    }

                    if (expiring.Count > 0)
                    {
                        var manager = _man.Name;

                        List<string> strs = new List<string>();
                        // strs.Add("Expiry Notification");
                        strs.Add("Dear " + "<b>" + manager + "</b><br/>");
                        strs.Add("The following items will be expired between " + m + " to " + n + " days<br/>");
                        strs.Add("<table style='border:1px solid #ccc'>");
                        foreach (var rec in expiring)
                        {
                            strs.Add("<tr style='border-bottom:0px solid #ccc;'>");
                            strs.Add("<td style='padding:10px;border :1px solid #ccc;background:#99ffcc'>" + rec.JobGroupMain + "</td>");
                            strs.Add("<td style='padding:10px;border :1px solid #ccc;background:#99ffcc'>" + rec.JobGroup + "</td>");
                            strs.Add("<td style='padding:10px;text-align:center;border :1px solid #ccc;background:#99ffcc'>" + rec.CertificateType + "</td>");
                            strs.Add("<td style='padding:10px;text-align:center;border :1px solid #ccc;background:#99ffcc'>" + rec.Count + "</td>");
                            // strs.Add(rec.JobGroupMain + " > " + rec.JobGroup + "       " + rec.CertificateType + "   " + "Count:" + rec.Count);
                            strs.Add("</tr>");
                            foreach(var row in rec.Items.OrderBy(q => q.Remain))
                            {
                                strs.Add("<tr style='border-bottom:0px solid #ccc;'>");
                                strs.Add("<td colspan=2 style='padding:10px;border :1px solid #ccc; '>" + row.Name + "</td>");
                               
                                strs.Add("<td colspan=2 style='padding:10px;text-align:center;border :1px solid #ccc; '>" + ((DateTime)row.DateExpire).ToString("yyyy-MMM-dd") + "</td>");
                                strs.Add("</tr>");
                            }
                        }
                        
                        strs.Add("</table>");
                        var email_body = String.Join("\n", strs);



                        var fromAddress = new MailAddress("airpocket@flypersia.aero", "TRAINING DEPARTMENT");
                        using (var message = new MailMessage(fromAddress, new MailAddress(_man.Email, _man.Name))
                        {
                            Subject = "Expiry Notification",
                            Body = email_body,
                            IsBodyHtml = true,


                        })

                        {

                            message.CC.Add("v.moghaddam59@gmail.com");
                            smtp.Send(message);


                        }
                         System.Threading.Thread.Sleep(15000);
                    }
                   
                }






                

                return Ok(true);

            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                return Ok(msg);
            }



        }


        [Route("api/notify/trn/employess/expiring/{m}/{n}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetNotifyTrnEmployeesExpiring(int m, int n)
        {
            try
            {
                var context = new ppa_vareshEntities();
                var query_expiring = from x in context.ViewCertificateHistories
                                     where x.RankOrder == 1 && x.Remain >= m && x.Remain <= n && x.InActive == false  //&& x.PersonId== 3408
                                     select new cer_history()
                                     {
                                         CertificateType = x.CertificateType,
                                         CertificateTypeId = x.CertificateTypeId2,
                                         DateExpire = x.DateExpire,
                                         DateIssue = x.DateIssue,
                                         ExpireDay = x.ExpireDay,
                                         ExpireMonth = x.ExpireMonth,
                                         ExpireYear = x.ExpireYear,
                                         Identifier = x.Identifier,
                                         Mobile = x.Mobile,
                                         PersonId = x.PersonId,
                                         Remain = x.Remain,
                                         Name = x.FirstName + " " + x.LastName,

                                     };
                var type = m.ToString() + "-" + n.ToString();
                var expiring = query_expiring.ToList();
                foreach (var x in expiring)
                {
                    x.KeyStr = x.ExpireYear
                    + "-"
                    + x.ExpireMonth
                    + "-"
                    + x.ExpireDay
                    + "-"
                    + x.PersonId
                    + "-"
                    + x.CertificateTypeId
                    + "-"
                    + type;
                }

                var keys = expiring.Select(q => q.KeyStr).ToList();

                var history = (from x in context.ViewNotificationTrnCounts
                               where keys.Contains(x.keystr) && x.SentCount > 1
                               select x).ToList();
                var history_keys = history.Select(q => q.keystr).ToList();

                expiring = expiring.Where(q => !history_keys.Contains(q.KeyStr)).ToList();


                //send sms
                //var text = "expiry notification";
                var grps = (from x in expiring
                            group x by new { x.Name, x.PersonId, x.Mobile } into grp
                            select new cer_history_group()
                            {
                                Name = grp.Key.Name,
                                PersonId = grp.Key.PersonId,
                                Mobile = grp.Key.Mobile,
                                Items = grp.OrderBy(q => q.DateExpire).ToList()
                            }).ToList();
                Magfa _magfa = new Magfa();
                foreach (var grp in grps)
                {
                    List<string> strs = new List<string>();
                    strs.Add("Expiry Notification");
                    strs.Add("Dear " + grp.Name);
                    strs.Add("The following items will be expired between " +m+" to " + n + " days");
                    foreach (var rec in grp.Items)
                    {
                        strs.Add(rec.CertificateType + "   " + ((DateTime)rec.DateExpire).ToString("yyyy-MMM-dd"));
                    }

                    grp.Text = String.Join("\n", strs);

                    grp.RefId = (_magfa.enqueue(1, grp.Mobile/*"09124449584"*/, grp.Text)[0]).ToString();
                    _magfa.enqueue(1, "09124449584", grp.Text);


                }
                System.Threading.Thread.Sleep(20000);


                ///////////////////////////
                foreach (var grp in grps)
                {
                    grp.Status = _magfa.getStatus(Convert.ToInt64(grp.RefId));
                    foreach (var x in grp.Items)
                    {
                        var not = new NotificationTrn()
                        {
                            CertificateType = x.CertificateType,
                            CertificateTypeId = x.CertificateTypeId,
                            DateCreate = DateTime.Now,
                            DateExpire = x.DateExpire,
                            DateIssue = x.DateIssue,
                            Mobile = x.Mobile,
                            PersonId = (int)x.PersonId,
                            Remain = x.Remain,
                            Text = grp.Text,
                            Type = type,
                            Ref = grp.RefId,
                            SMSStatus = grp.Status,
                        };
                        context.NotificationTrns.Add(not);
                    }
                }
                //foreach (var x in expiring)
                //{
                //    var not = new NotificationTrn()
                //    {
                //        CertificateType = x.CertificateType,
                //        CertificateTypeId = x.CertificateTypeId,
                //        DateCreate = DateTime.Now,
                //        DateExpire = x.DateExpire,
                //        DateIssue = x.DateIssue,
                //        Mobile = x.Mobile,
                //        PersonId = (int)x.PersonId,
                //        Remain = x.Remain,
                //        Text = "-",
                //        Type = type
                //    };
                //    context.NotificationTrns.Add(not);
                //}
                try
                {
                    context.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {

                        foreach (var ve in eve.ValidationErrors)
                        {
                            var xxx =
                                 ve.PropertyName + " " + ve.ErrorMessage;
                        }
                    }

                }

                // var refids = new List<Int64>() { smsResult };
                // System.Threading.Thread.Sleep(5000);
                // var status = m.getStatus(refids);

                return Ok(true);

            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                return Ok(msg);
            }



        }

        public class cer_history_group
        {
            public string Name { get; set; }
            public int? PersonId { get; set; }
            public string Mobile { get; set; }
            public List<cer_history> Items { get; set; }
            public string Text { get; set; }
            public string RefId { get; set; }
            public string Status { get; set; }
        }

        public class cer_history
        {
            public string Identifier { get; set; }
            public DateTime? DateIssue { get; set; }
            public DateTime? DateExpire { get; set; }
            public int? ExpireYear { get; set; }
            public int? ExpireMonth { get; set; }
            public int? ExpireDay { get; set; }
            public int? PersonId { get; set; }
            public string CertificateType { get; set; }
            public int? CertificateTypeId { get; set; }
            public int? Remain { get; set; }
            public string Mobile { get; set; }
            public string KeyStr { get; set; }
            public string Name { get; set; }
            //x.JobGroup,x.JobGroupMain,x.CertificateType
            public string JobGroup { get; set; }
            public string JobGroupMain { get; set; }
            public string JobGroupRoot { get; set; }
            public int Count { get; set; }
            public List<ViewCertificateHistory> Items { get; set; }

        }


        public class manager_mail
        {
            public string Name { get; set; }
            public string NID { get; set; }
            public string MainGroup { get; set; }
            public string JobGroup { get; set; }
            public string ProfileGroup { get; set; }
            public string Email { get; set; }
            public bool AllGroups { get; set; }
            public   List<string> JobGroupRoot { get; set; }
        }


    }
}
