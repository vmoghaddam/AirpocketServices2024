using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
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
using System.Net.Http.Headers;
using System.Drawing;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Threading;

using System.Collections.Specialized;
using ApiMap.Models;

using System.Data.Entity;

namespace ApiMap.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class FlightRadarController : ApiController
    {


        [Route("api/test")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetLivsdfsdfeFlts(string query)
        {
            ppa_mainEntities ctx = new ppa_mainEntities();
            var ds = await ctx.Options.ToListAsync();
            return Ok(ds);


        }

        [Route("api/metar")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetMetar()
        {
            WebClient client = new WebClient();
            string reply = client.DownloadString("https://metar-taf.com/live/OIII?zoom=70");
            return Ok(reply);


        }

        [Route("api/test2")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetTest2()
        {
            FlightDetailsResponse result = null;
            string str = "";
            using (HttpClient client = new HttpClient())
            {
                var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                var url = "https://data-feed.flightradar24.com/fr24.feed.api.v1.Feed/LiveFeed";
                var obj = await client.GetByteArrayAsync(url);
                //result = JsonConvert.DeserializeObject<FlightDetailsResponse>(str);


                //ds = result.results.Where(q => q.type == "live").Select(q => new FrFlight() { id = q.id, no = q.detail.flight }).ToList();

            }

            return Ok(true);

        }



        [Route("api/fr/live/flights")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetLiveFlts()
        {
            // List<string> airlines = new List<string>() { "VRH", "CPN" };
            List<Airline> airlines = new List<Airline>();
            airlines.Add(new Airline() { name = "MAHAN", code = new Code() { iata = "W5", icao = "IRM" } });
            airlines.Add(new Airline() { name = "CASPIAN", code = new Code() { iata = "RV", icao = "CPN" } });
            airlines.Add(new Airline() { name = "VARESH", code = new Code() { iata = "", icao = "VRH" } });
            airlines.Add(new Airline() { name = "ATA", code = new Code() { iata = "I3", icao = "TBZ" } });
            airlines.Add(new Airline() { name = "KISHAIR", code = new Code() { iata = "", icao = "KIS" } });
            airlines.Add(new Airline() { name = "KARUN", code = new Code() { iata = "", icao = "KRU" } });
            List<FR_Flight> live_flights = new List<FR_Flight>();
            List<Result> ds = new List<Result>();

            foreach (var airline in airlines)
            {
                using (HttpClient client = new HttpClient())
                {
                    var url = "https://www.flightradar24.com/v1/search/web/find?query=" + airline.code.icao + "&limit=50";
                    var str = await client.GetStringAsync(url);
                    var result = JsonConvert.DeserializeObject<SearchResponse>(str);
                    var live = result.results.Where(q => q.type == "live").ToList();
                    foreach (var x in live)
                    {
                        x.iata = airline.code.iata;
                        x.icao = airline.code.icao;
                        x.airline = airline.name;
                    }

                    ds = ds.Concat(live).ToList();
                }
            }

            var live_ids = ds.Select(q => q.id).ToList();
            ppa_mainEntities context = new ppa_mainEntities();
            var exists = await context.FR_Flight.Where(q => live_ids.Contains(q.FlightId)).Select(q => q.FlightId).ToListAsync();
            var new_flights = ds.Where(q => !exists.Contains(q.id)).ToList();




            foreach (var x in new_flights)
            {
                var flt = new FR_Flight()
                {
                    FlightId = x.id,
                    FlightNo = x.detail.callsign,
                    Register = x.detail.reg,
                    Model = x.detail.ac_type,
                    Destination = x.detail.schd_to,
                    Origin = x.detail.schd_from,
                    AirlineIATA = x.iata,
                    AirlineICAO = x.icao,
                    Airline = x.airline,
                    DateCreate = DateTime.UtcNow,
                    Status = "live",

                };
                context.FR_Flight.Add(flt);
                live_flights.Add(flt);
            }

            await context.SaveChangesAsync();


            //SearchResponse result = null;
            //List<FrFlight> ds = new List<FrFlight>();
            //using (HttpClient client = new HttpClient())
            //{
            //    var url = "https://www.flightradar24.com/v1/search/web/find?query=" + query + "&limit=50";
            //    var str = await client.GetStringAsync(url);
            //    result = JsonConvert.DeserializeObject<SearchResponse>(str);

            //    ds = result.results.Where(q => q.type == "live").Select(q => new FrFlight() { id = q.id, no = q.detail.flight }).ToList();

            //}

            return Ok(live_flights);

        }
        public class ap_flight
        {
            public int Id { get; set; }
            public string FlightNo { get; set; }
            public string DateStr { get; set; }
            public DateTime Date { get; set; }
            public string FlightStatus { get; set; }
            public int FlightStatusId { get; set; }
            public string Origin { get; set; }
            public string Destination { get; set; }
            public string Register { get; set; }
            public DateTime Dep { get; set; }
            public DateTime Arr { get; set; }
            public DateTime DepLocal { get; set; }
            public DateTime ArrLocal { get; set; }
        }

        [Route("api/fr/live/flights/icao")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetLiveFltsIcao(string icao, string no)
        {
            ////// List<string> airlines = new List<string>() { "VRH", "CPN" };
            ////List<Airline> airlines = new List<Airline>();
            ////airlines.Add(new Airline() { name = "MAHAN", code = new Code() { iata = "W5", icao = "IRM" } });
            ////airlines.Add(new Airline() { name = "CASPIAN", code = new Code() { iata = "RV", icao = "CPN" } });
            ////airlines.Add(new Airline() { name = "VARESH", code = new Code() { iata = "", icao = "VRH" } });
            ////airlines.Add(new Airline() { name = "ATA", code = new Code() { iata = "I3", icao = "TBZ" } });
            ////airlines.Add(new Airline() { name = "KISHAIR", code = new Code() { iata = "", icao = "KIS" } });
            ////airlines.Add(new Airline() { name = "KARUN", code = new Code() { iata = "", icao = "KRU" } });
            ////List<FR_Flight> live_flights = new List<FR_Flight>();
            ////List<Result> ds = new List<Result>();

            ////foreach (var airline in airlines)
            ////{
            ////    using (HttpClient client = new HttpClient())
            ////    {
            ////        var url = "https://www.flightradar24.com/v1/search/web/find?query=" + airline.code.icao + "&limit=50";
            ////        var str = await client.GetStringAsync(url);
            ////        var result = JsonConvert.DeserializeObject<SearchResponse>(str);
            ////        var live = result.results.Where(q => q.type == "live").ToList();
            ////        foreach (var x in live)
            ////        {
            ////            x.iata = airline.code.iata;
            ////            x.icao = airline.code.icao;
            ////            x.airline = airline.name;
            ////        }

            ////        ds = ds.Concat(live).ToList();
            ////    }
            ////}

            ////var live_ids = ds.Select(q => q.id).ToList();
            ppa_mainEntities context = new ppa_mainEntities();
            var query = from x in context.VFR_Flight
                        where x.Status == "live"
                        select x;
            if (icao != "-1")
                query = query.Where(q => q.AirlineICAO == icao);
            if (no != "-1")
                query = query.Where(q => q.FlightNo == no);


            var lives = await query.ToListAsync();


            var lives2 = new List<VFR_Flight>();
            using (HttpClient client = new HttpClient())
            {
                var url = "https://zapi.apvaresh.com/api/flight/departed";
                switch (icao)
                {
                    case "AXV":
                        url = "https://ava.api.airpocket.app/api/flight/departed";
                        break;
                    default:
                        break;
                }
                var str = await client.GetStringAsync(url);
                var obj = JsonConvert.DeserializeObject<List<ap_flight>>(str);
                if (no != "-1")
                    obj = obj.Where(q => q.FlightNo == no).ToList();


                foreach (var x in obj)
                {
                    lives2.Add(new VFR_Flight()
                    {
                        Id = x.Id,
                        Airline = icao,
                        AirlineICAO = icao,
                        DateFlight = x.Date,
                        FlightNo = icao.ToUpper() + x.FlightNo,
                        FlightStatus = x.FlightStatus,
                        FlightStatusId = x.FlightStatusId,
                        APFlightId = x.Id,
                         Register=x.Register,
                         Dep=x.Dep,
                         DepLocal=x.DepLocal,
                         Arr=x.Arr,
                         ArrLocal=x.ArrLocal,
                         Origin=x.Origin,
                         Destination=x.Destination,
                         


                    });
                }

            }

            var _l1 = lives2.Where(q => !lives.Select(z => z.FlightNo).Contains(q.FlightNo)).ToList();

            foreach(var x in lives)
            {
                var _f = lives2.FirstOrDefault(q => q.FlightNo == x.FlightNo);
                if (_f != null)
                {
                    x.APFlightId = _f.APFlightId;
                    x.FlightStatus = _f.FlightStatus;
                    x.FlightStatusId = _f.FlightStatusId;
                    x.Register = _f.Register;
                    x.Destination = _f.Destination;
                    x.Origin = _f.Origin;
                    x.Dep = _f.Dep;
                    x.DepLocal = _f.DepLocal;
                    x.Arr = _f.Arr;
                    x.ArrLocal = _f.ArrLocal;
                }
            }

            lives = lives.Concat(_l1).ToList();


            return Ok(lives);

        }


        [Route("api/fr/flight")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetFlight(string fid)
        {
            FlightDetailsResponse result = null;
            string str = "";
            dynamic obj;
            dynamic _flight;
            using (HttpClient client = new HttpClient())
            {
                var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                var url = "https://api.flightradar24.com/common/v1/flight-playback.json?flightId=329db747";
                str = await client.GetStringAsync(url);
                obj = JsonConvert.DeserializeObject<dynamic>(str);
                // result = JsonConvert.DeserializeObject<FlightDetailsResponse>(str);
                _flight = obj.result.response.data.flight;
                result = _flight as FlightDetailsResponse;
                //ds = result.results.Where(q => q.type == "live").Select(q => new FrFlight() { id = q.id, no = q.detail.flight }).ToList();

            }

            return Ok(_flight);

        }


        public class flight_track_altitude
        {
            public double feet { get; set; }
            public double meters { get; set; }

        }
        public class flight_track_speed
        {
            public double kmh { get; set; }
            public double kts { get; set; }
            public double mph { get; set; }
        }
        public class flight_track_vertical_speed
        {
            public double fpm { get; set; }
            public double ms { get; set; }

        }
        public class flight_track
        {
            public decimal latitude { get; set; }
            public decimal longitude { get; set; }
            public double heading { get; set; }
            public double squawk { get; set; }
            public Int64 timestamp { get; set; }
            public string ems { get; set; }

            public flight_track_altitude altitude { get; set; }
            public flight_track_speed speed { get; set; }
            public flight_track_vertical_speed verticalSpeed { get; set; }

            public DateTime time
            {
                get
                {
                    return new DateTime(1970, 1, 1).Add(TimeSpan.FromSeconds((int)this.timestamp));
                }
            }


        }


        [Route("api/fr/live/flights/track/{search}/{icao}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetLiveFltsTrack(int search, string icao)
        {
            ppa_mainEntities context = new ppa_mainEntities();
            if (search == 1)
            {
                List<Airline> airlines = new List<Airline>();
                // airlines.Add(new Airline() { name = "MAHAN", code = new Code() { iata = "W5", icao = "IRM" } });
                // airlines.Add(new Airline() { name = "CASPIAN", code = new Code() { iata = "RV", icao = "CPN" } });
                // airlines.Add(new Airline() { name = "VARESH", code = new Code() { iata = "", icao = "VRH" } });
                //  airlines.Add(new Airline() { name = "ATA", code = new Code() { iata = "I3", icao = "TBZ" } });
                // airlines.Add(new Airline() { name = "KISHAIR", code = new Code() { iata = "", icao = "KIS" } });
                //  airlines.Add(new Airline() { name = "KARUN", code = new Code() { iata = "", icao = "KRU" } });
                airlines.Add(new Airline() { name = "AVA", code = new Code() { iata = "", icao = "AXV" } });
                airlines.Add(new Airline() { name = "VARESH", code = new Code() { iata = "", icao = "VRH" } });
                if (icao != "-1")
                    airlines = airlines.Where(q => q.code.icao == icao).ToList();
                List<FR_Flight> live_flights = new List<FR_Flight>();
                List<Result> ds = new List<Result>();

                foreach (var airline in airlines)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        var url = "https://www.flightradar24.com/v1/search/web/find?query=" + airline.code.icao + "&limit=50";
                        var str = await client.GetStringAsync(url);
                        var result = JsonConvert.DeserializeObject<SearchResponse>(str);
                        var live = result.results.Where(q => q.type == "live").ToList();
                        foreach (var x in live)
                        {
                            x.iata = airline.code.iata;
                            x.icao = airline.code.icao;
                            x.airline = airline.name;
                        }

                        ds = ds.Concat(live).ToList();
                        var _live_ids = live.Select(q => q.id).ToList();
                        var cmd = "update fr_flight set status='done' where AirlineICAO='" + airline.code.icao + "'";
                        if (_live_ids.Count > 0)
                        {
                            cmd += " AND flightid not in (" + string.Join(",", _live_ids.Select(q => "'" + q + "'").ToList()) + ")";
                        }
                        context.Database.ExecuteSqlCommand(cmd);

                    }




                }

                var live_ids = ds.Select(q => q.id).ToList();
                var _dt = DateTime.Now.Date;
                //ppa_mainEntities context = new ppa_mainEntities();
                var exists = await context.VFR_Flight.Where(q => live_ids.Contains(q.FlightId) && q.DateFlight == _dt).Select(q => q.FlightId).ToListAsync();
                var new_flights = ds.Where(q => !exists.Contains(q.id)).ToList();




                foreach (var x in new_flights)
                {
                    var flt = new FR_Flight()
                    {
                        FlightId = x.id,
                        FlightNo = x.detail.callsign,
                        Register = x.detail.reg,
                        Model = x.detail.ac_type,
                        Destination = x.detail.schd_to,
                        Origin = x.detail.schd_from,
                        AirlineIATA = x.iata,
                        AirlineICAO = x.icao,
                        Airline = x.airline,
                        DateCreate = DateTime.UtcNow,
                        DateFlight = _dt,
                        Status = "live",

                    };
                    context.FR_Flight.Add(flt);
                    live_flights.Add(flt);
                }




                await context.SaveChangesAsync();
            }
            DataTable tbl = new DataTable();
            tbl.Columns.Add(new DataColumn("Id", typeof(int)));
            tbl.Columns.Add(new DataColumn("FlightId", typeof(int)));
            tbl.Columns.Add(new DataColumn("Latitude", typeof(decimal)));
            tbl.Columns.Add(new DataColumn("Longitude", typeof(decimal)));

            tbl.Columns.Add(new DataColumn("Squawk", typeof(double)));

            tbl.Columns.Add(new DataColumn("TimeStamp", typeof(Int64)));
            tbl.Columns.Add(new DataColumn("EMS", typeof(string)));
            tbl.Columns.Add(new DataColumn("Altitude_F", typeof(double)));
            tbl.Columns.Add(new DataColumn("Altitude_M", typeof(double)));
            tbl.Columns.Add(new DataColumn("Speed_KMH", typeof(double)));
            tbl.Columns.Add(new DataColumn("Speed_KTS", typeof(double)));


            tbl.Columns.Add(new DataColumn("Speed_MPH", typeof(double)));
            tbl.Columns.Add(new DataColumn("VSpeed_FPM", typeof(double)));
            tbl.Columns.Add(new DataColumn("VSpeed_MS", typeof(double)));
            tbl.Columns.Add(new DataColumn("DateTrack", typeof(DateTime)));
            tbl.Columns.Add(new DataColumn("Heading", typeof(double)));







            var qry = from x in context.FR_Flight
                      where x.Status == "live"
                      select x;
            if (icao != "-1")
                qry = qry.Where(q => q.AirlineICAO == icao);
            var flights = await qry.ToListAsync();
            foreach (var flt in flights)
            {
                using (HttpClient client = new HttpClient())
                {
                    var url = "https://api.flightradar24.com/common/v1/flight-playback.json?flightId=" + flt.FlightId;
                    var str = await client.GetStringAsync(url);
                    var obj = JsonConvert.DeserializeObject<dynamic>(str);
                    var status = obj.result.response.data.flight.status.live;
                    // if (status == "false")
                    //     flt.Status = "done";
                    var obj_track = obj.result.response.data.flight.track;
                    var obj_track_str = JsonConvert.SerializeObject(obj_track);
                    List<flight_track> tracks = JsonConvert.DeserializeObject<List<flight_track>>(obj_track_str);
                    tracks = tracks.OrderBy(q => q.time).ToList();
                    if (tracks.Count > 0)
                        flt.DateFlight = tracks.First().time;
                    foreach (var t in tracks)
                    {
                        DataRow dr = tbl.NewRow();
                        dr["Id"] = -1;
                        dr["FlightId"] = flt.Id;
                        dr["Latitude"] = t.latitude;
                        dr["Longitude"] = t.longitude;
                        dr["Squawk"] = t.squawk;
                        dr["TimeStamp"] = t.timestamp;
                        dr["EMS"] = t.ems;
                        dr["Altitude_F"] = t.altitude.feet;
                        dr["Altitude_M"] = t.altitude.meters;
                        dr["Speed_KMH"] = t.speed.kmh;
                        dr["Speed_KTS"] = t.speed.kts;
                        dr["Speed_MPH"] = t.speed.mph;
                        dr["VSpeed_FPM"] = t.verticalSpeed.fpm;
                        dr["VSpeed_MS"] = t.verticalSpeed.ms;
                        dr["DateTrack"] = t.time;
                        dr["Heading"] = t.heading;


                        tbl.Rows.Add(dr);
                    }

                    //context.Database.ExecuteSqlCommand("Delete from FR_Flight_Track where FlightId=" + flt.Id);
                    //foreach (var t in tracks)
                    //{
                    //    context.FR_Flight_Track.Add(new FR_Flight_Track()
                    //    {
                    //        FlightId = flt.Id,
                    //        DateTrack = t.time,
                    //        Latitude = t.latitude,
                    //        Longitude = t.longitude,
                    //        Squawk = t.squawk,
                    //        EMS = t.ems,
                    //        TimeStamp = t.timestamp,
                    //        Altitude_M = t.altitude.meters,
                    //        Altitude_F = t.altitude.feet,
                    //        Speed_KMH = t.speed.kmh,
                    //        Speed_KTS = t.speed.kts,
                    //        Speed_MPH = t.speed.mph,
                    //        VSpeed_FPM = t.verticalSpeed.fpm,
                    //        VSpeed_MS = t.verticalSpeed.ms,

                    //    });
                    //}

                }
            }
            await context.SaveChangesAsync();


            var flt_ids = string.Join(",", flights.Select(q => q.Id).ToList());
            var ap_cnn_string = "Data Source=5.182.44.132;Initial Catalog=ppa_main;User ID=sa;Password=Atrina1359@aA";
            SqlConnection cnnAP = new SqlConnection(ap_cnn_string);
            cnnAP.Open();
            if (flights.Count > 0)
                using (SqlCommand command = new SqlCommand("delete from fr_flight_track where flightid in (" + flt_ids + ")", cnnAP))
                {
                    //  command.CommandTimeout = 1000000;
                    var rr = command.ExecuteNonQuery();
                }

            SqlBulkCopy objbulk = new SqlBulkCopy(cnnAP);
            objbulk.DestinationTableName = "FR_FLIGHT_TRACK";
            objbulk.WriteToServer(tbl);
            cnnAP.Close();


            return Ok(true);
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
                var url = "https://data-live.flightradar24.com/clickhandler/?version=1.5&flight=" + fid;
                str = await client.GetStringAsync(url);
                result = JsonConvert.DeserializeObject<FlightDetailsResponse>(str);


                //ds = result.results.Where(q => q.type == "live").Select(q => new FrFlight() { id = q.id, no = q.detail.flight }).ToList();

            }

            return Ok(result);

        }



        public class ext_flight_data
        {
            public VFR_Flight flight { get; set; }
            public List<VFR_Flight_Track> track { get; set; }
        }
        [Route("api/ext/flight/data")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetEXTFlightData(string icao, string no, DateTime date)
        {
            date = date.Date;

            if (!no.ToLower().Contains(icao.ToLower()))
                no = icao.ToUpper() + no.ToUpper();
            else
                no = no.ToUpper();

            ppa_mainEntities context = new ppa_mainEntities();
            var flight = await context.VFR_Flight.Where(q => q.DateFlight == date && q.FlightNo == no).FirstOrDefaultAsync();
            if (flight != null)
            {
                var track = await context.VFR_Flight_Track.Where(q => q.FlightId == flight.Id).OrderBy(q => q.DateTrack).ToListAsync();
                return Ok(new ext_flight_data()
                {
                    flight = flight,
                    track = track,
                });
            }
            else
                return Ok(new ext_flight_data()
                {
                    flight = new VFR_Flight()
                    {
                        Id = -1
                    },
                    track = new List<VFR_Flight_Track>(),
                });


        }
        public int get_alt_level(int alt)
        {
            if (alt <= 1000)
                return 0;
            if (alt > 1000 && alt <= 5000)
                return 1;



            if (alt > 5000 && alt <= 10000)
                return 2;


            if (alt > 10000 && alt <= 18000)
                return 3;


            if (alt > 18000 && alt <= 20000)
                return 4;


            if (alt > 20000 && alt <= 22000)
                return 5;


            if (alt > 22000 && alt <= 24000)
                return 6;


            if (alt > 24000 && alt <= 26000)
                return 7;


            if (alt > 26000 && alt <= 28000)
                return 8;



            if (alt > 28000 && alt <= 30000)
                return 9;


            if (alt > 30000 && alt <= 32000)
                return 10;


            if (alt > 32000 && alt <= 34000)
                return 11;

            return 12;
        }

        //[Route("api/ext/flight/data/v2")]
        //[AcceptVerbs("GET")]
        //public async Task<IHttpActionResult> GetEXTFlightDataV2(string fid)
        //{
        //    FlightDetailsResponse result = null;
        //    string str = "";
        //    var flight = new VFR_Flight();
        //    var track = new List<VFR_Flight_Track>();

        //    using (HttpClient client = new HttpClient())
        //    {

        //        var url = "https://data-live.flightradar24.com/clickhandler/?version=1.5&flight=" + fid;
        //        str = await client.GetStringAsync(url);
        //        result = JsonConvert.DeserializeObject<FlightDetailsResponse>(str);

        //        flight = new VFR_Flight()
        //        {
        //            FlightId = result.identification.id,
        //            FlightNo = result.identification.callsign,
        //            Airline = result.airline.name,
        //            AirlineIATA = result.airline.code.iata,
        //            AirlineICAO = result.airline.code.icao,
        //            Origin = result.airport.origin.code.iata,
        //            Destination = result.airport.destination.code.iata,
        //            Register = result.aircraft.registration,

        //        };
        //        var trail = result.trail.OrderBy(q => q.dt).ToList();
        //        int c = 1;
        //        foreach (var x in trail)
        //        {
        //            track.Add(new VFR_Flight_Track()
        //            {
        //                Id = c,
        //                FlightId = -1,
        //                Altitude_F = x.alt,
        //                DateTrack = x.dt,
        //                Heading = x.hd,
        //                Latitude = Convert.ToDecimal(x.lat),
        //                Longitude = Convert.ToDecimal(x.lng),
        //                Speed_MPH = x.spd,
        //                AltitudeLvl = get_alt_level(x.alt),


        //            });
        //            c++;
        //        }


        //        //ds = result.results.Where(q => q.type == "live").Select(q => new FrFlight() { id = q.id, no = q.detail.flight }).ToList();

        //    }

        //    return Ok(new ext_flight_data()
        //    {
        //        flight = flight,
        //        track = track,
        //    });




        //    //return Ok(new ext_flight_data()
        //    //{
        //    //    flight = new VFR_Flight()
        //    //    {
        //    //        Id = -1
        //    //    },
        //    //    track = new List<VFR_Flight_Track>(),
        //    //});


        //}
        [Route("api/ext/flight/ofp/points")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetEXTFlightOfpPoints(string no, DateTime date)
        {
            ppa_mainEntities context = new ppa_mainEntities();
            date = date.Date;
            var points = await context.VFR_OFPPoint.Where(q => q.DateFlight == date && q.FlightNo == no).OrderBy(q => q.Id).ToListAsync();
            return Ok(points);
        }




        [Route("api/ext/flight/data/v2")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetEXTFlightDataV2(string icao, string no, DateTime date, string mode)
        {

            var _no = no.Split('*').ToList();

            ppa_mainEntities context = new ppa_mainEntities();
            date = date.Date;
            var out_result = new List<ext_flight_data>();

            foreach (var fn in _no)
            {
                if (mode == "online")
                {
                    var existing_flight = await context.VFR_Flight.Where(q => q.DateFlight == date && q.FlightNo == fn).FirstOrDefaultAsync();
                    if (existing_flight == null)
                    {
                        out_result.Add(new ext_flight_data()
                        {
                            flight = new VFR_Flight()
                            {
                                Id = -1,
                                FlightNo = fn
                            },
                            track = new List<VFR_Flight_Track>(),
                        });

                    }
                    else
                    {
                        var fid = existing_flight.FlightId;


                        FlightDetailsResponse result = null;
                        string str = "";
                        var flight = new VFR_Flight();
                        var track = new List<VFR_Flight_Track>();

                        using (HttpClient client = new HttpClient())
                        {

                            var url = "https://data-live.flightradar24.com/clickhandler/?version=1.5&flight=" + fid;
                            str = await client.GetStringAsync(url);
                            result = JsonConvert.DeserializeObject<FlightDetailsResponse>(str);

                            flight = new VFR_Flight()
                            {
                                FlightId = result.identification.id,
                                FlightNo = result.identification.callsign,
                                Airline = result.airline.name,
                                // AirlineIATA = result.airline.code.iata,
                                //  AirlineICAO = result.airline.code.icao,
                                // Origin = result.airport.origin.code.iata,
                                // Destination = result.airport.destination.code.iata,
                                Register = result.aircraft.registration,

                            };
                            var trail = result.trail.OrderBy(q => q.dt).ToList();
                            int c = 1;
                            foreach (var x in trail)
                            {
                                track.Add(new VFR_Flight_Track()
                                {
                                    Id = c,
                                    FlightId = -1,
                                    Altitude_F = x.alt,
                                    DateTrack = x.dt,
                                    Heading = x.hd,
                                    Latitude = Convert.ToDecimal(x.lat),
                                    Longitude = Convert.ToDecimal(x.lng),
                                    Speed_MPH = x.spd,
                                    AltitudeLvl = get_alt_level(x.alt),


                                });
                                c++;
                            }


                            //ds = result.results.Where(q => q.type == "live").Select(q => new FrFlight() { id = q.id, no = q.detail.flight }).ToList();

                        }


                        out_result.Add(
                            new ext_flight_data()
                            {
                                flight = flight,
                                track = track,
                            });
                    }

                }
                else
                {
                    var existing_flight = await context.VFR_Flight.Where(q => q.DateFlight == date && q.FlightNo == fn).FirstOrDefaultAsync();
                    if (existing_flight == null)
                    {
                        out_result.Add(new ext_flight_data()
                        {
                            flight = new VFR_Flight()
                            {
                                Id = -1,
                                FlightNo = fn
                            },
                            track = new List<VFR_Flight_Track>(),
                        });

                    }
                    else
                    {
                        var track = await context.VFR_Flight_Track.Where(q => q.FlightId == existing_flight.Id).OrderBy(q => q.Id).ToListAsync();
                        out_result.Add(
                           new ext_flight_data()
                           {
                               flight = existing_flight,
                               track = track,
                           });
                    }

                }





            }



            return Ok(out_result);




            //return Ok(new ext_flight_data()
            //{
            //    flight = new VFR_Flight()
            //    {
            //        Id = -1
            //    },
            //    track = new List<VFR_Flight_Track>(),
            //});


        }









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
        public string id
        {
            get; set;
        }
        public string icao { get; set; }
        public string iata { get; set; }
        public string airline { get; set; }

        public string label { get; set; }
        public Detail detail { get; set; }
        public string type { get; set; }
        public string match { get; set; }
        public string name { get; set; }
        public string key
        {
            get { return this.id + "_" + (detail == null ? id : detail.callsign); }
        }
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
        public string ac_type { get; set; }
        public string schd_from { get; set; }
        public string schd_to { get; set; }

    }

    public class FrFlight
    {
        public string id { get; set; }
        public string no { get; set; }
        public string iata { get; set; }
        public string icao { get; set; }
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
        public long? row { get; set; }
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
        public DateTime? dt_arrival { get { return this.departure == null ? null : (Nullable<DateTime>)new DateTime(1970, 1, 1).Add(TimeSpan.FromSeconds((int)this.departure)); } }
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
        public int? eta { get; set; }
        public int? updated { get; set; }
    }

    public class Trail
    {
        public float lat { get; set; }
        public float lng { get; set; }
        public int alt { get; set; }
        public int spd { get; set; }
        public int ts { get; set; }
        public int hd { get; set; }
        public DateTime dt { get { return new DateTime(1970, 1, 1).Add(TimeSpan.FromSeconds((int)this.ts)); } }
    }




}
