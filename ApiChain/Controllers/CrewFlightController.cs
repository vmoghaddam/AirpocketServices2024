using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ApiChain.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CrewFlightController : ApiController
    {

        [Route("api/test")]
        public   IHttpActionResult GetTest()
        {
            var web = new WebUtils();
            var result = web.GET("https://fleet.flypersia.aero/apiv2/odata/crew/flights/app/297?from=2022-11-05T14:10:43&to=2022-11-05T14:10:43");

            return Ok(result);
        }



    }


    public class WebUtils
    {
        public string GET(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                    return reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                WebResponse errorResponse = ex.Response;
                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.GetEncoding("utf-8"));
                    String errorText = reader.ReadToEnd();
                    // log errorText
                }
                throw;
            }
        }
    }




}
