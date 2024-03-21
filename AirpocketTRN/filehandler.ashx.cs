using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Cors;

namespace AirpocketTRN
{
    /// <summary>
    /// Summary description for filehandler
    /// </summary>
    //[EnableCors(origins: "*", headers: "*", methods: "*")]
    public class filehandler : IHttpHandler
    {
        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();
        public static int RandomNumber(int min, int max)
        {
            lock (syncLock)
            { // synchronize
                return random.Next(min, max);
            }
        }
        public void ProcessRequest(HttpContext context)
        {
            string param = context.Request.QueryString["t"];
            if (param == "clientfiles")
            {
                if (context.Request.Files.Count > 0)
                {
                    List<string> fileNames = new List<string>();
                    HttpFileCollection files = context.Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFile file = files[i];
                        var ext = System.IO.Path.GetExtension(file.FileName);
                        var date = DateTime.Now;
                        // Random rnd = new Random();
                        // int rndint = rnd.Next(100000);
                        int rndint = RandomNumber(1, 100000);
                        var key = date.Year.ToString() + date.Month.ToString() + date.Day.ToString() + date.Hour.ToString() + date.Minute.ToString() + date.Second.ToString() +
                            date.Millisecond.ToString() + "_" + i.ToString() + "_" + rndint.ToString() + ext;
                        // var fname = context.Server.MapPath("~/upload/clientsfiles/" + key);
                        var fname = @"C:\inetpub\vhosts\airpocket.online\files.airpocket.online\varesh\clientsfiles\" + key;
                        file.SaveAs(fname);
                        fileNames.Add(key);
                    }


                    //var records = Objs.xls_bill.getJSON("bill.xlsx");
                    context.Response.ContentType = "text/plain";
                    context.Response.Write(string.Join("@", fileNames));
                }
            }


        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}