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
using XAPI.Models;
using System.Net.Http.Headers;
using System.Drawing;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Threading;
using System.Web.Script.Serialization;
using System.Collections.Specialized;

namespace XAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class FlightRadarController : ApiController
    {


        [Route("api/fr/search")]
        [AcceptVerbs("GET")]
        public async Task< IHttpActionResult> GetFlt(string query)
        {
            SearchResponse result = null;
            using (HttpClient client = new HttpClient())
            {
                var url = "https://www.flightradar24.com/v1/search/web/find?query="+query+"&limit=50";
                var str=await client.GetStringAsync(url);
                result= JsonConvert.DeserializeObject< SearchResponse >(str);
            }

            return Ok(result);

        }


        [Route("api/fr/live/flights")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetLiveFlts(string query)
        {
            SearchResponse result = null;
            List<FrFlight> ds = new List<FrFlight>();
            using (HttpClient client = new HttpClient())
            {
                var url = "https://www.flightradar24.com/v1/search/web/find?query=" + query + "&limit=50";
                var str = await client.GetStringAsync(url);
                result = JsonConvert.DeserializeObject<SearchResponse>(str);

                  ds = result.results.Where(q => q.type == "live").Select(q => new FrFlight() { id = q.id, no = q.detail.flight }).ToList();
                   
            }

            return Ok(ds);

        }

        // var url = $"https://data-live.flightradar24.com/zones/fcgi/feed.js?faa=1&mlat=1&flarm=1&adsb=1&gnd=1&air=1&vehicles=1&estimated=1&maxage=60&gliders=1&time={now}&filter_info=1&selected={id}&ems=1&limit=0";

        [Route("api/fr/live/data")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetLiveData(string fid)
        {

            string str = "";
            using (HttpClient client = new HttpClient())
            {
                var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                var url = "https://data-live.flightradar24.com/zones/fcgi/feed.js?faa=1&mlat=1&flarm=1&adsb=1&gnd=1&air=1&vehicles=1&estimated=1&maxage=60&gliders=1&time="+now+"&filter_info=1&selected="+fid+"&ems=1&limit=0";
                 str = await client.GetStringAsync(url);
                //result = JsonConvert.DeserializeObject<SearchResponse>(str);

                //ds = result.results.Where(q => q.type == "live").Select(q => new FrFlight() { id = q.id, no = q.detail.flight }).ToList();

            }

            return Ok(str);

        }

        [Route("api/fr/data")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetFlightData(string fid)
        {
            FlightDetailsResponse result = null;
            string str = "";
            using (HttpClient client = new HttpClient())
            {
                var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                var url = "https://data-live.flightradar24.com/clickhandler/?version=1.5&flight="+fid;
                str = await client.GetStringAsync(url);
                 result = JsonConvert.DeserializeObject<FlightDetailsResponse>(str);
                
                 
                //ds = result.results.Where(q => q.type == "live").Select(q => new FrFlight() { id = q.id, no = q.detail.flight }).ToList();

            }

            return Ok(result);

        }
        //public async Task<FlightDetailsResponse> Details(string flightId)
        //{
        //    var url = $"https://data-live.flightradar24.com/clickhandler/?version=1.5&flight={flightId}";

        //    return JsonConvert.DeserializeObject<FlightDetailsResponse>(await httpClient.GetStringAsync(url));
        //}






    }



    public class SearchResponse
    {
        public Result[] results { get; set; }
        public Stats stats { get; set; }
    }

    public class Stats
    {
        public Total total { get; set; }
        public Count count { get; set; }
    }

    public class Total
    {
        public int all { get; set; }
        public int airport { get; set; }
        public int _operator { get; set; }
        public int live { get; set; }
        public int schedule { get; set; }
        public int aircraft { get; set; }
    }

    public class Count
    {
        public int airport { get; set; }
        public int _operator { get; set; }
        public int live { get; set; }
        public int schedule { get; set; }
        public int aircraft { get; set; }
    }

    public class Result
    {
        public string id { get; set; }
        public string label { get; set; }
        public Detail detail { get; set; }
        public string type { get; set; }
        public string match { get; set; }
        public string name { get; set; }
    }

    public class Detail
    {
        public string image { get; set; }
        public string iata { get; set; }
        public float lat { get; set; }
        public float lon { get; set; }
        public string reg { get; set; }
        public string callsign { get; set; }
        public string flight { get; set; }
    }

    public class FrFlight
    {
        public string id { get; set; }
        public string no { get; set; }
    }
    public class FlightDetailsResponse
    {
        public Identification identification { get; set; }
        public Status status { get; set; }
        public string level { get; set; }
        public Aircraft aircraft { get; set; }
        public Airline airline { get; set; }
        public Owner owner { get; set; }
        public object airspace { get; set; }
        public Airport airport { get; set; }
        public Flighthistory flightHistory { get; set; }
        public object ems { get; set; }
        public string[] availability { get; set; }
        public Time time { get; set; }
        public Trail[] trail { get; set; }
        public int firstTimestamp { get; set; }
        public string s { get; set; }
    }

    public class Identification
    {
        public string id { get; set; }
        public long row { get; set; }
        public Number number { get; set; }
        public string callsign { get; set; }
    }

    public class Number
    {
        public string @default { get; set; }
        public object alternative { get; set; }
    }

    public class Status
    {
        public bool live { get; set; }
        public string text { get; set; }
        public string icon { get; set; }
        public object estimated { get; set; }
        public bool ambiguous { get; set; }
        public Generic generic { get; set; }
    }

    public class Generic
    {
        public StatusDetails status { get; set; }
        public Eventtime eventTime { get; set; }
    }

    public class StatusDetails
    {
        public string text { get; set; }
        public string color { get; set; }
        public string type { get; set; }
    }

    public class Eventtime
    {
        public int utc { get; set; }
        public int local { get; set; }
    }

    public class Aircraft
    {
        public Model model { get; set; }
        public string registration { get; set; }
        public string hex { get; set; }
        public object age { get; set; }
        public object msn { get; set; }
        public Images images { get; set; }
    }

    public class Model
    {
        public string code { get; set; }
        public string text { get; set; }
    }

    public class Images
    {
        public Thumbnail[] thumbnails { get; set; }
        public Medium[] medium { get; set; }
        public Large[] large { get; set; }
    }

    public class Thumbnail
    {
        public string src { get; set; }
        public string link { get; set; }
        public string copyright { get; set; }
        public string source { get; set; }
    }

    public class Medium
    {
        public string src { get; set; }
        public string link { get; set; }
        public string copyright { get; set; }
        public string source { get; set; }
    }

    public class Large
    {
        public string src { get; set; }
        public string link { get; set; }
        public string copyright { get; set; }
        public string source { get; set; }
    }

    public class Airline
    {
        public string name { get; set; }
        public string _short { get; set; }
        public Code code { get; set; }
        public string url { get; set; }
    }

    public class Code
    {
        public string iata { get; set; }
        public string icao { get; set; }
    }

    public class Owner
    {
        public string name { get; set; }
        public Code code { get; set; }
        public string url { get; set; }
    }

    public class Airport
    {
        public AirportDetails origin { get; set; }
        public AirportDetails destination { get; set; }
        public object real { get; set; }
    }

    public class AirportDetails
    {
        public string name { get; set; }
        public Code code { get; set; }
        public Position position { get; set; }
        public Timezone timezone { get; set; }
        public bool visible { get; set; }
        public string website { get; set; }
        public Info info { get; set; }
    }


    public class Position
    {
        public float latitude { get; set; }
        public float longitude { get; set; }
        public string altitude { get; set; }
        public Country country { get; set; }
        public Region region { get; set; }
    }

    public class Country
    {
        public string name { get; set; }
        public string code { get; set; }
    }

    public class Region
    {
        public string city { get; set; }
    }

    public class Timezone
    {
        public string name { get; set; }
        public int offset { get; set; }
        public string offsetHours { get; set; }
        public string abbr { get; set; }
        public string abbrName { get; set; }
        public bool isDst { get; set; }
    }

    public class Info
    {
        public string terminal { get; set; }
        public object baggage { get; set; }
        public object gate { get; set; }
    }
    public class Flighthistory
    {
        public Aircraft[] aircraft { get; set; }
        public Flight[] flight { get; set; }
    }

    public class Time
    {
        public TimeDetails scheduled { get; set; }
        public TimeDetails real { get; set; }
        public TimeDetails estimated { get; set; }
        public Other other { get; set; }
        public Historical historical { get; set; }
    }

    public class TimeDetails
    {
        public int? departure { get; set; }
        public int? arrival { get; set; }
        public DateTime? dt_departure { get { return this.arrival == null ? null : (Nullable<DateTime>)new DateTime(1970, 1, 1).Add(TimeSpan.FromSeconds((int)this.arrival)); } }
        public DateTime? dt_arrival { get { return this.departure==null?null:(Nullable<DateTime>) new DateTime(1970, 1, 1).Add(TimeSpan.FromSeconds((int)this.departure)); } }
    }

    public class Flight
    {
        public Identification identification { get; set; }
        public Aircraft aircraft { get; set; }
        public Airport airport { get; set; }
        public Time time { get; set; }
    }








    public class Historical
    {
        public int flighttime { get; set; }

        public string delay { get; set; }
    }



    public class Scheduled
    {
        public int departure { get; set; }
        public int arrival { get; set; }
    }


    public class Estimated
    {
        public int departure { get; set; }
        public int arrival { get; set; }
    }

    public class Other
    {
        public int eta { get; set; }
        public int updated { get; set; }
    }

    public class Trail
    {
        public float lat { get; set; }
        public float lng { get; set; }
        public int alt { get; set; }
        public int spd { get; set; }
        public int ts { get; set; }
        public int hd { get; set; }
        public DateTime dt { get { return  new DateTime(1970, 1, 1).Add(TimeSpan.FromSeconds((int)this.ts)); } }
    }


}
