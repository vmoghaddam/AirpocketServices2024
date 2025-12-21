using ApiWorld.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ApiWorld.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class FaraNegarController : ApiController
    {

        public class AuthRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string AL { get; set; }
        }
        public class AuthorizeTokenResponse
        {
            public string Type { get; set; }
            public string AccessToken { get; set; }
            public string SubscriptionKey { get; set; }
            public int TokenExpiredMinutes { get; set; }
            public string ServerDate { get; set; }
            public int ResultCode { get; set; }
            public string ResultText { get; set; }
        }


        /// ///////////////////
        public class TripInfoRequest
        {
            public string AL { get; set; }
            public string FLNB { get; set; }
            public string FlightDate { get; set; }   // "2025-12-01"
            public string FromCity { get; set; }
            public string ToCity { get; set; }
            public TripInfo TripInfo { get; set; }
        }

        public class TripInfo
        {
            public DOWData DOWData { get; set; }
            public FuelData FuelData { get; set; }
            public WeightLimitData WeightLimitData { get; set; }
            public int RefID { get; set; }
        }

        public class DOWData
        {
            public int CockpitCrew { get; set; }
            public int CabinCrew { get; set; }
            public string Pantry { get; set; }
            public int FSGCount { get; set; }
            public bool IsStandard { get; set; }
            public int MOS { get; set; }
            public string CaptainName { get; set; }
        }

        public class FuelData
        {
            public double Density { get; set; }
            public int TotalFuel { get; set; }
            public int TaxiFuel { get; set; }
            public int TripFuel { get; set; }
        }

        public class WeightLimitData
        {
            public int RTOW { get; set; }

        }
        public class RequestResult
        {
            public int ResultCode { get; set; }
            public string ResultText { get; set; }
            public bool IsShowResultCode { get; set; }
            public object MessageJson { get; set; }
        }



        [Route("api/faranegar/send/request/{fltid}")]
        [HttpPost]
        public async Task<IHttpActionResult> send_request(TripInfoRequest entity)
        {
            ppa_entities context = new ppa_entities();
            var url = "https://external.fdcs.ir/Account/GetAuthorizeToken";

            var auth = new AuthRequest
            {
                Username = "WB.IV.API",
                Password = "1234@",
                AL = "CPN"
            };

            var json = JsonConvert.SerializeObject(auth);

            using (var client = new HttpClient())
            using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
            {


                var response = await client.PostAsync(url, content);
                var responseBody = await response.Content.ReadAsStringAsync();

                var auth_res = Newtonsoft.Json.JsonConvert.DeserializeObject<AuthorizeTokenResponse>(responseBody);



                var url2 = "https://external.fdcs.ir/api/wb/AddFlightScheduleTripInfo";

                using (var client2 = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", auth_res.AccessToken);

                    client.DefaultRequestHeaders.Add("Subscription-Key", auth_res.SubscriptionKey);

                    var json2 = JsonConvert.SerializeObject(entity);
                    using (var content2 = new StringContent(json2, Encoding.UTF8, "application/json"))
                    {
                        var resp = await client2.PostAsync(url2, content2);
                        var body = await resp.Content.ReadAsStringAsync();

                        var body_object = JsonConvert.DeserializeObject<RequestResult>(body);

                    }
                }




            }
            return Ok(true);
        }



    }
}
