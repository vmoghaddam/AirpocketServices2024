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
using System.Text.RegularExpressions;
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
            load_sheet_history history = new load_sheet_history()
            {
                airlines = entity.AL,
                from_city = entity.FromCity,
                to_city = entity.ToCity,
                flight_no = entity.FLNB,
                flight_id = entity.TripInfo.RefID,
                flight_date = entity.FlightDate,
                cabin = entity.TripInfo.DOWData.CabinCrew,
                cockpit = entity.TripInfo.DOWData.CockpitCrew,
                pantry_code = entity.TripInfo.DOWData.Pantry,
                fsg = entity.TripInfo.DOWData.FSGCount,
                mos = entity.TripInfo.DOWData.MOS,
                pic_name = entity.TripInfo.DOWData.CaptainName,
                density = entity.TripInfo.FuelData.Density.ToString(),
                total_fuel = entity.TripInfo.FuelData.TotalFuel.ToString(),
                taxi_fuel = entity.TripInfo.FuelData.TaxiFuel.ToString(),
                trip_fuel = entity.TripInfo.FuelData.TripFuel.ToString(),
                rtow = entity.TripInfo.WeightLimitData.RTOW.ToString(),


            };

            context.load_sheet_history.Add(history);
            await context.SaveChangesAsync();

            var url = "https://external.fdcs.ir/Account/GetAuthorizeToken";

            var auth = new AuthRequest
            {
                Username = "WB.IV.API",
                Password = "1234@",
                AL = "CPN"
            };

            history.auth_airline = auth.AL;
            history.auth_password = auth.Password;
            history.auth_user = auth.Username;


            var json = JsonConvert.SerializeObject(auth);

            using (var client = new HttpClient())
            using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
            {

                try
                {
                    history.auth_date = DateTime.Now;
                    var response = await client.PostAsync(url, content);
                    var responseBody = await response.Content.ReadAsStringAsync();
                    history.auth_response = responseBody;
                    var auth_res = Newtonsoft.Json.JsonConvert.DeserializeObject<AuthorizeTokenResponse>(responseBody);
                    try
                    {
                        var url2 = "https://external.fdcs.ir/api/wb/AddFlightScheduleTripInfo";


                        using (var client2 = new HttpClient())
                        {
                            client2.DefaultRequestHeaders.Authorization =
                                new AuthenticationHeaderValue("Bearer", auth_res.AccessToken);


                            client2.DefaultRequestHeaders.Add("Subscription-Key", auth_res.SubscriptionKey);
                          //  entity.TripInfo.RefID = history.Id;
                            history.request_date = DateTime.Now;

                            var json2 = JsonConvert.SerializeObject(entity);
                            using (var content2 = new StringContent(json2, Encoding.UTF8, "application/json"))
                            {
                                var resp = await client2.PostAsync(url2, content2);
                                var body = await resp.Content.ReadAsStringAsync();

                                var body_object = JsonConvert.DeserializeObject<RequestResult>(body);
                                history.request_result_code = body_object.ResultCode.ToString();
                                history.reques_result_text = body_object.ResultText;


                            }
                        }
                    }
                    catch (Exception ex_req)
                    {
                        var ex_req_msg = ex_req.Message;
                        if (ex_req.InnerException != null)
                            ex_req_msg += ex_req.InnerException.Message;
                        history.error_message = ex_req_msg;
                    }





                }
                catch (Exception ex_auth)
                {
                    var ex_auth_meg = ex_auth.Message;
                    if (ex_auth.InnerException != null)
                        ex_auth_meg += ex_auth.InnerException.Message;
                    history.auth_error_message = ex_auth_meg;
                }





            }

            await context.SaveChangesAsync();

            return Ok(true);
        }



        public class load_sheet_dto
        {
            public string content { get; set; }
            public int? RefId { get; set; }
            public string Edition { get; set; }
            public string RowRefID { get; set; }
            public string MessageType { get; set; }

            public string key { get; set; }
        }


        public class old_load_sheet_dto
        {
            public string content { get; set; }
            // public int? RefId { get; set; }


            public string key { get; set; }
        }


        [Route("api/loadsheet")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostLoadSheet(load_sheet_dto dto)
        {
            var ctx = new ppa_entities();
            var entity = new load_sheet_raw()
            {
                date_create = DateTime.Now,



            };
            try
            {

                ctx.load_sheet_raw.Add(entity);
                old_load_sheet_dto old_dto = new old_load_sheet_dto()
                {
                    key = dto.key,
                    content = dto.content,
                };

                var json = JsonConvert.SerializeObject(old_dto);
                HttpClient _http = new HttpClient();
                entity.airline_server_request_date = DateTime.Now;
                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                using (var request = new HttpRequestMessage(HttpMethod.Post, "https://fleet.caspianairlines.com/zxapi/api/loadsheet") { Content = content })
                {

                    // request.Headers.Add("X-Api-Key", "....");

                    using (var response = await _http.SendAsync(request))
                    {
                        var body = await response.Content.ReadAsStringAsync();

                        entity.airline_server_response = body;


                    }
                }





            }
            catch (Exception exxxx)
            {
                var msg = exxxx.Message;
                if (exxxx.InnerException != null)
                    msg += "   " + exxxx.InnerException.Message;
                entity.airline_server_error_message = msg;
            }

            try
            {


                if (string.IsNullOrEmpty(dto.key))
                {
                    entity.error_message = "Authorization key not found.";
                    await ctx.SaveChangesAsync();
                    return Ok(new
                    {
                        result = false,
                        error_message = "Authorization key not found.",
                        ref_id = -1,
                    });
                }

                if (string.IsNullOrEmpty(dto.content))
                {
                    entity.error_message = "Plan cannot be empty.";
                    await ctx.SaveChangesAsync();
                    return Ok(new
                    {
                        result = false,
                        error_message = "Plan cannot be empty.",
                        ref_id = -1,
                    });
                }


                if (dto.key != "FaraNegar@1359#")
                {
                    entity.error_message = "Authorization key is wrong.";
                    await ctx.SaveChangesAsync();
                    return Ok(new
                    {
                        result = false,
                        error_message = "Authorization key is wrong.",
                        ref_id = -1,
                    });
                }



                ctx.Database.CommandTimeout = 1000;

                FlightInformation flight = null;


                if (dto.RefId != null)
                {
                    var _refid = Convert.ToInt32(dto.RefId);
                    flight = ctx.FlightInformations.Where(q => q.ID == _refid).FirstOrDefault();
                }

                string flightNo = "";
                var m = Regex.Match(dto.content, @"\bCPN(?<flightNo>\d{3,4})/(?<day>\d{2})\b");
                if (m.Success)
                {
                    flightNo = m.Groups["flightNo"].Value;
                    //string day = m.Groups["day"].Value;
                    int targetDay = int.Parse(m.Groups["day"].Value);

                    DateTime now = DateTime.Now;

                    int maxDay = DateTime.DaysInMonth(now.Year, now.Month);
                    //if (targetDay < 1 || targetDay > maxDay)
                    //    throw new ArgumentOutOfRangeException(nameof(targetDay), $"Day must be 1..{maxDay} for {now:yyyy-MM}.");

                    DateTime adjusted_date = new DateTime(
                        now.Year, now.Month, targetDay,
                        00, 00, 00, 00,
                        00
                    );
                    var d = adjusted_date.Date;
                    var next = d.AddDays(1);
                    if (flight == null)
                        flight = ctx.FlightInformations.SingleOrDefault(q => q.FlightNumber == flightNo && q.STD >= d && q.STD < next);
                }

                entity.content = dto.content;
                entity.flight_id = flight == null ? -1 : flight.ID;
                entity.RefId = dto.RefId;
                entity.RowRefID = dto.RowRefID;
                entity.Edition = dto.Edition;
                entity.MessageType = dto.MessageType;
                entity.airline = "CASPIAN";
                entity.flight_no = flightNo;

                var history = ctx.load_sheet_history.Where(q => q.Id == dto.RefId).FirstOrDefault();
                if (history != null)
                    entity.history_id = history.Id;

                //var entity = new load_sheet_raw()
                //{
                //    content = dto.content,
                //    flight_id = flight == null ? -1 : flight.ID,

                //    RefId = dto.RefId,
                //    RowRefID = dto.RowRefID,
                //    Edition = dto.Edition,
                //    MessageType = dto.MessageType,
                //    airline = "CASPIAN",
                //    flight_no = flightNo


                //};

                await ctx.SaveChangesAsync();

                return Ok(new
                {
                    result = true,
                    error_message = string.Empty,
                    ref_id = entity.id,
                });
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += " " + ex.InnerException.Message;

                entity.error_message = msg;
                await ctx.SaveChangesAsync();
                return Ok(new
                {
                    result = false,
                    error_message = msg,
                    ref_id = -1,
                });
            }


        }


        [Route("api/loadsheet/flight/{id}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetLoadSheet(int id)
        {
            var ctx = new ppa_entities();
            var loadsheet = ctx.view_load_sheet_raw.FirstOrDefault(q => q.RefId == id && q.rank_last==1);

            return Ok(loadsheet);



        }


        [Route("api/loadsheet/new/{flt_id}/{cid}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetNewLoadSheets(string flt_id, int cid)
        {
            var ctx = new ppa_entities();
            var flt_ids = flt_id.Split('_').Select(x => (Nullable<int>)Convert.ToInt32(x)).ToList();
            var loadsheets = ctx.view_load_sheet_raw.Where(q => flt_ids.Contains(q.RefId) && q.rank_last==1 ).ToList();
            var ls_ids = loadsheets.Select(q => q.id).ToList();
            var history = ctx.load_sheet_visit_history.Where(q => q.crew_id == cid &&  ls_ids.Contains(q.load_sheet_id)).ToList();
            var h_ids = history.Select(q => q.load_sheet_id).ToList();
            var result = loadsheets.Where(q => !h_ids.Contains(q.id)).Select(q => new
            {
                q.flight_no,
                q.Edition,
                q.id,
                flight_id=q.flight_id==-1?q.RefId:q.flight_id
            }).ToList();

            return Ok(result);



        }


        [Route("api/loadsheet/view/{id}/{cid}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetViewLoadSheets(int id, int cid)
        {
            var ctx = new ppa_entities();
            var vh = ctx.load_sheet_visit_history.Where(q => q.load_sheet_id == id && q.crew_id == cid).FirstOrDefault();
            if (vh == null)
            {
                vh = new load_sheet_visit_history()
                {
                    load_sheet_id = id,
                    crew_id = cid,
                    
                };
                ctx.load_sheet_visit_history.Add(vh);
            }
            vh.date_visit= DateTime.Now;
            ctx.SaveChanges();
            return Ok(new
            {
                vh.id,
                vh.load_sheet_id,
                
                vh.date_visit
            });



        }


        [Route("api/loadsheet/sign/{id}/{cid}/{lic}/{pic}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetSignLoadSheets(int id, int cid,string lic,string pic)
        {
            var ctx = new ppa_entities();
            var load_sheet=ctx.load_sheet_raw.Where(q=>q.id==id).FirstOrDefault();
            if (load_sheet==null)
                return Ok(new { IsSuccess = false, message = "the load sheet was not found" });
            load_sheet.date_sign = DateTime.Now;
            load_sheet.pic = pic;
            load_sheet.lic_no= lic;
            load_sheet.signed_by_id = cid;
            ctx.SaveChanges();
            return Ok(new { IsSuccess = true, message = "succeeded" });




        }




        public class RegisterUserDto
        {
            public string UserName { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public string Station { get; set; }
            public string Password { get; set; }
            public string[] Roles { get; set; }   // null هم می‌تونه باشه
            public int? PersonId { get; set; }
        }

        [Route("api/resgister")]
        [HttpGet]
        public async Task<IHttpActionResult> register_user()
        {
            ppa_entities context = new ppa_entities();
            var items = context.atoes.ToList();

            foreach (var item in items)
            {
                var dto = new RegisterUserDto()
                {
                    FirstName = item.FIRSTNAME,
                    LastName = item.LASTNAME,
                    Email = item.LASTNAME.Replace(" ", "").ToUpper() + "." + item.FIRSTNAME.Replace(" ", "").ToUpper() + "@aerotech.app",
                    PersonId = -1,
                    Roles = null,
                    PhoneNumber = item.MOBILE,
                    Station = "",
                    Password = item.MOBILE,
                    UserName = item.NID,
                };
                HttpClient _http = new HttpClient
                {
                    Timeout = TimeSpan.FromSeconds(300)
                };

                var url = "https://ato.airpocket.app/apinet/odata/users/register";

                var json = JsonConvert.SerializeObject(dto);
                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                using (var resp = await _http.PostAsync(url, content).ConfigureAwait(false))
                {
                    var body = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);


                    //  if (!resp.IsSuccessStatusCode)
                    //      throw new HttpRequestException($"HTTP {(int)resp.StatusCode} {resp.ReasonPhrase}\n{body}");

                    //  return body;
                }
            }



            return Ok(true);
        }







    }
}
