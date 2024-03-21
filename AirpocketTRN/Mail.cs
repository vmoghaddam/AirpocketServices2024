using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace AirpocketTRN
{
    public class MailHelper
    {
        string dispatchEmail = ConfigurationManager.AppSettings["email_dispatch"];
        string dispatchTitle = ConfigurationManager.AppSettings["email_dispatch_title"];
        string dispatchEmailPassword =  ConfigurationManager.AppSettings["email_dispatch_password"];
        string dispatchEmailHost = ConfigurationManager.AppSettings["email_dispatch_host"];
        string dispatchEmailPort = ConfigurationManager.AppSettings["email_dispatch_port"];
        string caoMSGEmail = ConfigurationManager.AppSettings["email_cao_message"];
        string caoMSGEmailAlt = ConfigurationManager.AppSettings["email_cao_message_alt"];
        string caoMSGEmailAlt2 = ConfigurationManager.AppSettings["email_cao_message_alt2"];
        string caoMSGEmailAlt3 = ConfigurationManager.AppSettings["email_cao_message_alt3"];
        string IsMVTEnabled = ConfigurationManager.AppSettings["mvt_enabled"];

        public string SendTest(string rec,string body, string subject, int port, int ssl)
        {



            try
            {
                var fromAddress = new MailAddress(dispatchEmail, dispatchTitle);
                var toAddress = new MailAddress(caoMSGEmail, "CAO MSG");
                var ccAddress = new MailAddress(caoMSGEmailAlt, "CAO MSG ALT1");
                var ccAddress2 = new MailAddress(caoMSGEmailAlt2, "CAO MSG ALT2");
                var ccAddress3 = new MailAddress(caoMSGEmailAlt3, "CAO MSG ALT3");

                string fromPassword = dispatchEmailPassword;




                var smtp = new SmtpClient
                {
                    //EnableSsl=true,
                    Host = dispatchEmailHost,
                    Port = port, //Convert.ToInt32(dispatchEmailPort),
                    EnableSsl = ssl == 1,
                    TargetName = "STARTTLS/Mail.flypersia.aero",
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword),

                };
                smtp.Timeout = 60000;

                using (var message = new MailMessage(fromAddress, new MailAddress(rec, "RECEIVER"))
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,


                })

                {
                    //smtp.SendCompleted += (s, e) => {
                    //    smtp.Dispose();

                    //};

                    smtp.Send(message);
                    //smtp.Send(new MailMessage(fromAddress, toAddress)
                    //{
                    //    Subject = subject,
                    //    Body = body,
                    //    IsBodyHtml = false,


                    //});
                    //smtp.Send(new MailMessage(dispatchEmail, caoMSGEmailAlt) {
                    //    Subject = subject,
                    //    Body = body,
                    //    IsBodyHtml = false,
                    //});
                    return "OK";
                }


            }
            catch (Exception ex)
            {
                var _msg = ex.Message;
                if (ex.InnerException != null)
                    _msg += "   INNER:  " + ex.InnerException.Message;
                return _msg;
            }








        }

 

    }
}