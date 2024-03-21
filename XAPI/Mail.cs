using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using XAPI.Models;

namespace XAPI
{
    public class MailRepository
    {
        private readonly string mailServer, login, password;
        private readonly int port;
        private readonly bool ssl;

        public MailRepository(string mailServer, int port, bool ssl, string login, string password)
        {
            this.mailServer = mailServer;
            this.port = port;
            this.ssl = ssl;
            this.login = login;
            this.password = password;
        }

        public IEnumerable<string> GetUnreadMails()
        {
            var messages = new List<string>();

            using (var client = new ImapClient())
            {
                client.Connect(mailServer, port, ssl);

                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                client.Authenticate(login, password);

                // The Inbox folder is always available on all IMAP servers...
                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadOnly);
                var results = inbox.Search(SearchOptions.All, SearchQuery.Not(SearchQuery.Seen));
                foreach (var uniqueId in results.UniqueIds)
                {
                    var message = inbox.GetMessage(uniqueId);

                    messages.Add(message.HtmlBody);

                    //Mark message as read
                    //inbox.AddFlags(uniqueId, MessageFlags.Seen, true);
                }

                client.Disconnect(true);
            }

            return messages;
        }

        public IEnumerable<string> GetAllMails()
        {
            var ctx = new PPAEntities();
            var messages = new List<string>();

            using (var client = new ImapClient())
            {
                client.Connect(mailServer, port, ssl);

                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                client.Authenticate(login, password);

                // The Inbox folder is always available on all IMAP servers...
                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadOnly);
                var results = inbox.Search(SearchQuery.All).ToList();
                //inbox.Search(SearchQuery.)
                foreach (var uniqueId in results)
                {
                    var message = inbox.GetMessage(uniqueId);
                    var subject = message.Subject;
                    var sbjs = subject.Split(new string[] { "Flight card" }, StringSplitOptions.None).ToList();

                    if (subject.ToLower().Contains("flight card") && message.Attachments.Count() > 0)
                    {
                        //Flight card 5819/EP-VBM OIMM-OIII 2022-06-04 16:57Z-2022-06-04 16:57Z
                        subject = "Flight card" + sbjs[1];
                        var pts1 = subject.Split('/')[0];
                        var fltno = pts1.Replace("Flight card ", "");
                        var pts2 = (subject.Split('/')[1]).Split(' ');
                        var reg = pts2[0];
                        var route = pts2[1];
                        var date = pts2[2];
                        var dateparts = date.Split('-').Select(q => Convert.ToInt32(q)).ToList();
                        var key = "FlightCard_" + date + "_" + fltno + ".pdf";
                        var fc = new FlightCard()
                        {
                            DateCreate = DateTime.Now,
                            FltNo = fltno,
                            Reg = reg,
                            Route = route,
                            Key = key,
                        };
                        ctx.FlightCards.Add(fc);
                        var body = message.TextBody;
                        var filePath = ConfigurationManager.AppSettings["skybag"] + key;
                        message.Attachments.ToList().First().WriteTo(filePath);
                        messages.Add(key);
                    }


                    // var filePath = ConfigurationManager.AppSettings["skybag"] + key;
                    //Mark message as read
                    //inbox.AddFlags(uniqueId, MessageFlags.Seen, true);
                }

                client.Disconnect(true);
            }
            ctx.SaveChanges();
            return messages;
        }

        public IEnumerable<string> GetAllMailsByFlight(string no, string dt)
        {
            try
            {
                var ctx = new PPAEntities();
                var messages = new List<string>();

                using (var client = new ImapClient())
                {
                    client.Connect(mailServer, port, ssl);

                    // Note: since we don't have an OAuth2 token, disable
                    // the XOAUTH2 authentication mechanism.
                    client.AuthenticationMechanisms.Remove("XOAUTH2");

                    client.Authenticate(login, password);

                    // The Inbox folder is always available on all IMAP servers...
                    var inbox = client.Inbox;
                    inbox.Open(FolderAccess.ReadOnly);
                    var results = inbox.Search(SearchQuery.SubjectContains(no).And(SearchQuery.SubjectContains(dt))).ToList();
                    //inbox.Search(SearchQuery.)
                    foreach (var uniqueId in results)
                    {
                        var message = inbox.GetMessage(uniqueId);
                        var subject = message.Subject;
                        var sbjs = subject.Split(new string[] { "Flight card" }, StringSplitOptions.None).ToList();

                        if (subject.ToLower().Contains("flight card") && message.Attachments.Count() > 0)
                        {
                            //Flight card 5819/EP-VBM OIMM-OIII 2022-06-04 16:57Z-2022-06-04 16:57Z
                            subject = "Flight card" + sbjs[1];
                            var pts1 = subject.Split('/')[0];
                            var fltno = pts1.Replace("Flight card ", "");
                            var pts2 = (subject.Split('/')[1]).Split(' ');
                            var reg = pts2[0];
                            var route = pts2[1];
                            var date = pts2[2];
                            var dateparts = date.Split('-').Select(q => Convert.ToInt32(q)).ToList();
                            var key = "FlightCard_" + date + "_" + fltno + ".pdf";
                            var fc = new FlightCard()
                            {
                                DateCreate = DateTime.Now,
                                FltNo = fltno,
                                Reg = reg,
                                Route = route,
                                Key = key,
                            };
                            ctx.FlightCards.Add(fc);
                            var body = message.TextBody;
                            var filePath = ConfigurationManager.AppSettings["skybag"] + key;
                            //message.Attachments.ToList().First().WriteTo(filePath);
                            //message.Attachments.ToList().First().WriteTo(filePath, true); 
                            var attachment = message.Attachments.ToList().First();
                            // var mime = (MimeKit.MimePart)client.Inbox.GetBodyPart(uniqueId, attachment);
                            // var fileName = mime.FileName;

                            //if (string.IsNullOrEmpty(fileName))
                            //    fileName = string.Format("unnamed-{0}", ++unnamed);

                            //using (var stream = File.Create(fileName))
                            //    mime.ContentObject.DecodeTo(stream);

                            using (var stream = File.Create(filePath))
                            {
                                if (attachment is MimeKit.MessagePart)
                                {
                                    var part = (MimeKit.MessagePart)attachment;

                                    part.Message.WriteTo(stream);
                                }
                                else
                                {
                                    var part = (MimeKit.MimePart)attachment;

                                    part.Content.DecodeTo(stream);
                                }
                            }
                            messages.Add(key);
                        }


                        // var filePath = ConfigurationManager.AppSettings["skybag"] + key;
                        //Mark message as read
                        //inbox.AddFlags(uniqueId, MessageFlags.Seen, true);
                    }

                    client.Disconnect(true);
                }
                ctx.SaveChanges();
                return messages;
            }
            catch(Exception ex)
            {
                var ds = new List<string>();
                ds.Add(ex.Message);
                if (ex.InnerException != null)
                    ds.Add(ex.InnerException.Message);
                return ds;
            }
           
        }
    }
}