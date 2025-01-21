using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.UI.WebControls;

namespace ApiChain.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CrewFlightController : ApiController
    {

        //[Route("api/test")]
        //public   IHttpActionResult GetTest()
        //{
        //    var web = new WebUtils();
        //    var result = web.GET("https://fleet.flypersia.aero/apiv2/odata/crew/flights/app/297?from=2022-11-05T14:10:43&to=2022-11-05T14:10:43");

        //    return Ok(result);
        //}

        [Route("api/get/fixtime")]
        [HttpGet]
        public async Task<IHttpActionResult> GetFixTime()
        {
            try
            {
                var context = new Models.dbEntities();
                var result = context.FixTimes.ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += " INNER: " + ex.InnerException.Message;
                return Ok(msg);
            }
        }

        public class FixTime
        {
            public string Route { get; set; }
            public int Duration { get; set; }
            public string Remark { get; set; }
        }

        [Route("api/save/fixtime")]
        [HttpPost]
        public async Task<IHttpActionResult> SaveFixTime(FixTime fixTime)
        {
            try { 
            var context = new Models.dbEntities();
            var route = Regex.Replace(fixTime.Route, @"\s+", "");
            var entity = context.FixTimes.FirstOrDefault(q => q.Route == route.ToUpper());
            if (entity == null)
            {
                entity = new Models.FixTime();
                context.FixTimes.Add(entity);
            }

            entity.Route = route.ToUpper();
            entity.Duration = fixTime.Duration;
            entity.remark = fixTime.Remark;

            await context.SaveChangesAsync();
            return Ok();
            } catch (Exception ex)
            {
                return BadRequest(ex.Message + " InnerException: " + ex.InnerException);
            }
        }
    }


    //public class WebUtils
    //{
    //    public string GET(string url)
    //    {
    //        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
    //        try
    //        {
    //            WebResponse response = request.GetResponse();
    //            using (Stream responseStream = response.GetResponseStream())
    //            {
    //                StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
    //                return reader.ReadToEnd();
    //            }
    //        }
    //        catch (WebException ex)
    //        {
    //            WebResponse errorResponse = ex.Response;
    //            using (Stream responseStream = errorResponse.GetResponseStream())
    //            {
    //                StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.GetEncoding("utf-8"));
    //                String errorText = reader.ReadToEnd();
    //                // log errorText
    //            }
    //            throw;
    //        }
    //    }
    //}




}
