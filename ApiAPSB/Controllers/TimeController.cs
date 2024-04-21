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
using ApiAPSB.Models;
using System.Net.Http.Headers;
using System.Drawing;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Threading;

namespace ApiAPSB.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class TimeController : ApiController
    {
        [Route("api/geo/sunrise/")]

        public async Task<IHttpActionResult> GetSunriseSave(DateTime df, DateTime dt)
        {
            try
            {
                var df_str = df.ToString("yyyy-MM-dd");
                var dt_str = dt.ToString("yyyy-MM-dd");
                List<geo_apt_sunrise> sun_times = new List<geo_apt_sunrise>();

                var context = new Models.dbEntities();
                var stations = await context.Airports.Where(q => q.ICAO.StartsWith("OI")
               //|| q.ICAO.StartsWith("OR")
               // || q.ICAO.StartsWith("UT")
               //|| q.ICAO.StartsWith("UG")
               // || q.ICAO.StartsWith("OK")
               // || q.ICAO.StartsWith("LT")
               // || q.ICAO.StartsWith("UDYZ")
               ).ToListAsync();

                var station_ids = stations.Select(q => (Nullable<int>)q.Id).ToList();

                var exist = context.geo_apt_sunrise.Where(q => station_ids.Contains(q.airport_id) && (q.date >= df && q.date <= dt)).ToList();
                context.geo_apt_sunrise.RemoveRange(exist);

                //https://api.sunrisesunset.io/json?lat=38.907192&lng=-77.036873&timezone=UTC&date_start=2024-04-18&date_end=2024-04-18
                using (WebClient webClient = new WebClient())
                {
                    foreach (var stn in stations)
                    {
                        var lat = stn.Latitude;
                        var lng = stn.Longitude;
                        var url = "https://api.sunrisesunset.io/json?lat=" + lat + "&lng=" + lng + "&timezone=UTC&date_start=" + df_str + "&date_end=" + dt_str;
                        var str = webClient.DownloadString(url);
                        //sunrise_sunset_io_root
                        //11:48:07 PM
                        var objs = JsonConvert.DeserializeObject<sunrise_sunset_io_root>(str);
                        foreach (var obj in objs.results)
                        {
                            var sunrise = new geo_apt_sunrise()
                            {
                                airport_id = stn.Id,
                                iata = stn.IATA,
                                icao = stn.ICAO,
                                lt = stn.Latitude,
                                lng = stn.Longitude,
                                date = to_date(obj.date),
                                day_length = obj.day_length,

                            };
                            sunrise.sunrise = to_date_time(obj.sunrise, sunrise.date);
                            sunrise.sunset = to_date_time(obj.sunset, sunrise.date);
                            sunrise.dawn = to_date_time(obj.dawn, sunrise.date);
                            sunrise.dusk = to_date_time(obj.dusk, sunrise.date);
                            sunrise.first_light = to_date_time(obj.first_light, sunrise.date);
                            sunrise.last_light = to_date_time(obj.last_light, sunrise.date);
                            sunrise.solar_noon = to_date_time(obj.solar_noon, sunrise.date);
                            sunrise.golden_hour = to_date_time(obj.golden_hour, sunrise.date);

                            sun_times.Add(sunrise);
                            context.geo_apt_sunrise.Add(sunrise);

                        }
                    }
                    // str_metar = webClient.DownloadString(url);
                    // obj_metar = JsonConvert.DeserializeObject<List<WeatherMetar>>(str_metar);



                }

                context.SaveChanges();
                var _result = sun_times.Select(q => new { q.iata, q.date }).ToList();
                return Ok(_result);
            }
            catch(Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                return Ok(msg);
            }
          

        }

        public DateTime to_date(string str)
        {
            var prts = str.Split('-').Select(q => Convert.ToInt32(q)).ToList();
            var dt = new DateTime(prts[0], prts[1], prts[2]);
            return dt;
        }
        public DateTime to_date_time(string str, DateTime? _dt)
        {
            //10:29:15 AM
            var dt = (DateTime)_dt;
            var p1 = str.Split(' ');
            var p2 = p1[0].Split(':').Select(q => Convert.ToInt32(q)).ToList();
            if (p1[1] == "PM")
                p2[0] += 12;
            var result = dt.Date;
            result = result.AddHours(p2[0]).AddMinutes(p2[1]).AddSeconds(p2[2]);
            return result;
        }


    }



    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class sunrise_sunset_io_result
    {
        public string date { get; set; }
        public string sunrise { get; set; }
        public string sunset { get; set; }
        public string first_light { get; set; }
        public string last_light { get; set; }
        public string dawn { get; set; }
        public string dusk { get; set; }
        public string solar_noon { get; set; }
        public string golden_hour { get; set; }
        public string day_length { get; set; }
        public string timezone { get; set; }
        public int utc_offset { get; set; }
    }

    public class sunrise_sunset_io_root
    {
        public List<sunrise_sunset_io_result> results { get; set; }
        public string status { get; set; }
    }




}
