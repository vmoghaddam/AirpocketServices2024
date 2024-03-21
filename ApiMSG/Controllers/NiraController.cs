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
using ApiMSG.Models;
using System.Net.Http.Headers;
using System.Drawing;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Threading;

namespace ApiMSG.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class NiraController : ApiController
    {
        [Route("api/nira/sync/")]
        [AcceptVerbs("POST")]
        public IHttpActionResult PostSyncNira(NiraDto dto)
        {
            var histories = new List<NiraHistory>();
            var context = new ppa_vareshEntities();
            var legs = context.ViewNiraFlights.Where(q => q.FlightStatusID == 1 && dto.ids.Contains(q.ID)).OrderBy(q => q.STDLocal).ToList();
            foreach (var leg in legs)
            {
                var _std = (DateTime)leg.STDLocal;
                string airline = "VR";
                var date = (_std).Year + "-" + (_std).Month.ToString().PadLeft(2, '0') + "-" + (_std).Day.ToString().PadLeft(2, '0');

                var dep = ((DateTime)leg.DepartureLocal).ToString("HH:mm");
                var depDate = ((DateTime)leg.DepartureLocal).ToString("yyyy-MM-dd");
                var arr = ((DateTime)leg.ArrivalLocal).ToString("HH:mm");

                var status = "Scheduled";
                if (leg.FlightStatusID == 4)
                    status = "Canceled";
                else if (leg.FlightStatusID == 2)
                    status = "TookOff";
                else if (leg.FlightStatusID == 3)
                    status = "Landed";


                var url = "http://vr.nirasoft.ir:882/NRSFlightInfo.jsp?Airline=" + airline + "&FlightNo=" + leg.FlightNumber
                  + "&Origin=" + leg.FromAirportIATA + "&Destination=" + leg.ToAirportIATA + "&FlightDate="
                  + date
                  + "&NewDepartureTime=" + dep + "&NewDepartureDate=" + depDate + "&NewArrivalTime=" + arr + "&NewFlightStatus=" + status + "&NewACT=EP-" + leg.Register
                  + "&Comment=lorem_ipsum&FleetWatchKey=" + leg.ID + "&SendSMS=false&OfficeUser=" + "THR059.AIRPOCKET&OfficePass=nira123";

                var result = new NiraHistory()
                {
                    FlightId = leg.ID,
                    Arrival = leg.ArrivalLocal,
                    Departure = leg.DepartureLocal,
                    FlightStatusId = leg.FlightStatusID,
                    Register = leg.Register,
                    DateSend = DateTime.Now,
                    Remark = url
                };


                try
                {
                    // var _client = new HttpClient();

                    // var _result = await _client.GetStringAsync(url);

                    WebRequest request = WebRequest.Create(url);

                    request.Credentials = CredentialCache.DefaultCredentials;

                    WebResponse response = request.GetResponse();

                    Stream dataStream = response.GetResponseStream();

                    StreamReader reader = new StreamReader(dataStream);

                    string responseFromServer = reader.ReadToEnd();

                    reader.Close();
                    response.Close();

                    result.DateReplied = DateTime.Now;


                    dynamic myObject = JsonConvert.DeserializeObject<dynamic>(responseFromServer);
                    result.CHTIME = Convert.ToString(myObject.CHTIME);
                    result.FLIGHT = Convert.ToString(myObject.FLIGHT);
                    result.NEWAIRCRAFT = Convert.ToString(myObject.NEWAIRCRAFT);
                    result.NEWSTATUS = Convert.ToString(myObject.NEWSTATUS);
                    // result.NEWSTATUS = _result; //Convert.ToString(myObject.NEWSTATUS);

                    context.NiraHistories.Add(result);

                    histories.Add(result);

                }
                catch (Exception ex)
                {
                    var msg = "ERROR: " + ex.Message;
                    if (ex.InnerException != null)
                        msg += " INNER:" + ex.InnerException.Message;
                    result.NEWSTATUS = msg;
                    //var saveResult = context.SaveChanges();
                    histories.Add(result);
                }



            }

            var saveResult = context.SaveChanges();
            return Ok(histories);
        }


        [Route("api/nira/sync/id/{id}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetSyncNira(int id)
        {
            var ids = new List<int>() { id };
            var histories = new List<NiraHistory>();
            var context = new ppa_vareshEntities();
            var legs = context.ViewNiraFlights.Where(q => q.FlightStatusID == 1 && ids.Contains(q.ID)).OrderBy(q => q.STDLocal).ToList();
            foreach (var leg in legs)
            {
                var _std = (DateTime)leg.STDLocal;
                string airline = "VR";
                var date = (_std).Year + "-" + (_std).Month.ToString().PadLeft(2, '0') + "-" + (_std).Day.ToString().PadLeft(2, '0');

                var dep = ((DateTime)leg.DepartureLocal).ToString("HH:mm");
                var depDate = ((DateTime)leg.DepartureLocal).ToString("yyyy-MM-dd");
                var arr = ((DateTime)leg.ArrivalLocal).ToString("HH:mm");

                var status = "Scheduled";
                if (leg.FlightStatusID == 4)
                    status = "Canceled";
                else if (leg.FlightStatusID == 2)
                    status = "TookOff";
                else if (leg.FlightStatusID == 3)
                    status = "Landed";


                var url = "http://vr.nirasoft.ir:882/NRSFlightInfo.jsp?Airline=" + airline + "&FlightNo=" + leg.FlightNumber
                  + "&Origin=" + leg.FromAirportIATA + "&Destination=" + leg.ToAirportIATA + "&FlightDate="
                  + date
                  + "&NewDepartureTime=" + dep + "&NewDepartureDate=" + depDate + "&NewArrivalTime=" + arr + "&NewFlightStatus=" + status + "&NewACT=EP-" + leg.Register
                  + "&Comment=lorem_ipsum&FleetWatchKey=" + leg.ID + "&SendSMS=false&OfficeUser=" + "THR059.AIRPOCKET&OfficePass=nira123";

                var result = new NiraHistory()
                {
                    FlightId = leg.ID,
                    Arrival = leg.ArrivalLocal,
                    Departure = leg.DepartureLocal,
                    FlightStatusId = leg.FlightStatusID,
                    Register = leg.Register,
                    DateSend = DateTime.Now,
                    Remark = url
                };


                try
                {
                    // var _client = new HttpClient();

                    // var _result = await _client.GetStringAsync(url);

                    WebRequest request = WebRequest.Create(url);

                    request.Credentials = CredentialCache.DefaultCredentials;

                    WebResponse response = request.GetResponse();

                    Stream dataStream = response.GetResponseStream();

                    StreamReader reader = new StreamReader(dataStream);

                    string responseFromServer = reader.ReadToEnd();

                    reader.Close();
                    response.Close();

                    result.DateReplied = DateTime.Now;


                    dynamic myObject = JsonConvert.DeserializeObject<dynamic>(responseFromServer);
                    result.CHTIME = Convert.ToString(myObject.CHTIME);
                    result.FLIGHT = Convert.ToString(myObject.FLIGHT);
                    result.NEWAIRCRAFT = Convert.ToString(myObject.NEWAIRCRAFT);
                    result.NEWSTATUS = Convert.ToString(myObject.NEWSTATUS);
                    // result.NEWSTATUS = _result; //Convert.ToString(myObject.NEWSTATUS);

                    context.NiraHistories.Add(result);

                    histories.Add(result);

                }
                catch (Exception ex)
                {
                    var msg = "ERROR: " + ex.Message;
                    if (ex.InnerException != null)
                        msg += " INNER:" + ex.InnerException.Message;
                    result.NEWSTATUS = msg;
                    //var saveResult = context.SaveChanges();
                    histories.Add(result);
                }



            }

            var saveResult = context.SaveChanges();
            return Ok(histories);
        }



        [Route("api/nira/conflicts/{dfrom}/{dto}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetNiraConflicts(DateTime dfrom, DateTime dto)
        {
            List<NiraConflictResult> conflictResult = new List<NiraConflictResult>();
            var _dfrom = dfrom.Date.ToString("yyyy-MM-dd");
            var _dto = dto.Date.ToString("yyyy-MM-dd");
            dfrom = dfrom.Date;
            dto = dto.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            List<NRSCRSFlightData> niraFlights = new List<NRSCRSFlightData>();
            using (var context = new ppa_vareshEntities())
            {
                var flights = context.ViewNiraFlights.Where(q => q.STDLocal >= dfrom && q.STDLocal <= dto && (q.FlightStatusID == 1 || q.FlightStatusID == 4)).ToList();
                var flightNos = flights.Select(q => q.FlightNumber).Distinct().ToList();


                foreach (var no in flightNos)
                {
                    try
                    {
                        string apiUrl = "http://vr.nirasoft.ir:882/NRSCWS.jsp?ModuleType=SP&ModuleName=CRSFlightData&DepartureDateFrom="
                       + _dfrom
                       + "&DepartureDateTo=" + _dto
                       + "&FlightNo=" + no
                       + "&OfficeUser=THR059.AIRPOCKET&OfficePass=nira123";

                        WebClient client = new WebClient();
                        client.Headers["Content-type"] = "application/json";
                        client.Encoding = Encoding.UTF8;
                        string json = client.DownloadString(apiUrl);
                        json = json.Replace("Child SA-Book", "Child_SA_Book").Replace("Adult SA-Book", "Adult_SA_Book");
                        var obj = JsonConvert.DeserializeObject<NRSCWSResult>(json);
                        niraFlights = niraFlights.Concat(obj.NRSCRSFlightData).ToList();
                    }
                    catch (Exception ex)
                    {
                        int i = 1;
                    }

                }
                foreach (var x in niraFlights)
                    x.Proccessed = false;
                flights = flights.OrderBy(q => q.STD).ToList();

                foreach (var aflt in flights)
                {
                    var niraflt = niraFlights.FirstOrDefault(q => q.FlightNo.PadLeft(4, '0') == aflt.FlightNumber && q.STDDay == ((DateTime)aflt.STDLocal).Date);
                    var conflict = new NiraConflictResult()
                    {
                        Date = ((DateTime)aflt.STDLocal).Date,
                        AirPocket = new _FLT()
                        {
                            FlightId = aflt.ID,
                            Destination = aflt.ToAirportIATA,
                            Origin = aflt.FromAirportIATA,
                            FlightNo = aflt.FlightNumber,
                            Register = aflt.Register,
                            STA = (DateTime)aflt.STALocal,
                            STD = (DateTime)aflt.STDLocal,
                            StatusId = aflt.FlightStatusID,
                            Status = aflt.FlightStatusID == 1 ? "SCHEDULED" : "CNL",
                        },
                    };
                    if (niraflt != null)
                    {
                        conflict.Nira = new _FLT()
                        {
                            Destination = niraflt.Destination,
                            FlightNo = niraflt.FlightNo,
                            Origin = niraflt.Origin,
                            Register = niraflt.Register,
                            STA = niraflt.STA,
                            STD = niraflt.STD,
                            StatusId = niraflt.FlightStatusId,
                            Status = niraflt.FlightStatusId == 1 ? "SCHEDULED" : "CNL",
                        };
                    }
                    conflictResult.Add(conflict);
                }
                // var niraFlights = obj.NRSCRSFlightData;


            }

            //var response = obj.d_envelope.d_body.response.result;
            //var responseJson = JsonConvert.DeserializeObject<List<IdeaSessionX>>(response);
            conflictResult = conflictResult.OrderBy(q => q.Date).ThenByDescending(q => q.IsConflicted).ThenBy(q => q.AirPocket.StatusId).ThenBy(q => q.AirPocket.Register)
                .ThenBy(q => q.AirPocket.STD).ToList();
            return Ok(conflictResult);
        }


        [Route("api/nira/delay/{id}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetNiraDelay(int id)
        {
            var context = new ppa_vareshEntities();
            string airlineCode = ConfigurationManager.AppSettings["airline_code"];
            string airline = airlineCode;
            var flight = context.ViewNiras.FirstOrDefault(q => q.ID == id);
            if (flight == null)
                return Ok(-1);
            if (
                  (flight.Delay > 30 && (flight.FlightStatusID == 1)  && ((DateTime)flight.STD - DateTime.UtcNow).TotalHours > 1)
                  || (flight.Delay<=-15)
               )
            {
                var _std = (DateTime)flight.STDLocal;
                var date = (_std).Year + "-" + (_std).Month.ToString().PadLeft(2, '0') + "-" + (_std).Day.ToString().PadLeft(2, '0');
                if (flight.FlightDate != null)
                {
                    var fd = ((DateTime)flight.FlightDate).Date;
                    date = (fd).Year + "-" + (fd).Month.ToString().PadLeft(2, '0') + "-" + (fd).Day.ToString().PadLeft(2, '0');
                }
                var dep = ((DateTime)flight.ChocksOutLocal).ToString("HH:mm");
                var depDate = ((DateTime)flight.ChocksOutLocal).ToString("yyyy-MM-dd");
                var arr = ((DateTime)flight.ChocksInLocal).ToString("HH:mm");
                var status = "Scheduled";
                if (flight.FlightStatusID == 4)
                    status = "Canceled";
                else if (flight.FlightStatusID == 2)
                    status = "TookOff";
                else if (flight.FlightStatusID == 3)
                    status = "Landed";
                var atireg =flight.Register;

                var url = "http://iv.nirasoft.ir:882//NRSFlightInfo.jsp?Airline=" + airline + "&FlightNo=" + flight.FlightNumber
                    + "&Origin=" + flight.FromAirportIATA + "&Destination=" + flight.ToAirportIATA + "&FlightDate="
                    + date
                    + "&NewDepartureTime=" + dep + "&NewDepartureDate=" + depDate + "&NewArrivalTime=" + arr + "&NewFlightStatus=" + status + "&NewACT=EP-" + atireg
                    + "&Comment=lorem_ipsum&FleetWatchKey=" + flight.ID + "&SendSMS=true&OfficeUser=" + "Thr003.airpocket" + "&OfficePass=" + "nira123";

                var _status = 1;

                var result = new NiraHistory()
                {
                    FlightId = flight.ID,
                    Arrival = flight.ChocksInLocal,
                    Departure = flight.ChocksOutLocal,
                    FlightStatusId = flight.FlightStatusID,
                    Register = flight.Register,
                    DateSend = DateTime.Now,
                    Remark = url
                };
                context.NiraHistories.Add(result);

                try
                {
                    // var _client = new HttpClient();

                    // var _result = await _client.GetStringAsync(url);

                    WebRequest request = WebRequest.Create(url);

                    request.Credentials = CredentialCache.DefaultCredentials;

                    WebResponse response = request.GetResponse();

                    Stream dataStream = response.GetResponseStream();

                    StreamReader reader = new StreamReader(dataStream);

                    string responseFromServer = reader.ReadToEnd();

                    reader.Close();
                    response.Close();

                    result.DateReplied = DateTime.Now;


                    dynamic myObject = JsonConvert.DeserializeObject<dynamic>(responseFromServer);
                    result.CHTIME = Convert.ToString(myObject.CHTIME);
                    result.FLIGHT = Convert.ToString(myObject.FLIGHT);
                    result.NEWAIRCRAFT = Convert.ToString(myObject.NEWAIRCRAFT);
                    result.NEWSTATUS = Convert.ToString(myObject.NEWSTATUS);
                    // result.NEWSTATUS = _result; //Convert.ToString(myObject.NEWSTATUS);

                    

                     

                }
                catch (Exception ex)
                {
                    var msg = "ERROR: " + ex.Message;
                    if (ex.InnerException != null)
                        msg += " INNER:" + ex.InnerException.Message;
                    result.NEWSTATUS = msg;
                    _status = -100;
                }

                var saveResult = context.SaveChanges();
                return Ok(_status);
            }
            else
                return Ok(0);




        }



        [Route("api/fail")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetRaiseException()
        {
            //  throw new Exception("BIG DADDY WILSON");
            var context = new ppa_vareshEntities();
            var flt = context.ViewLegTimes.FirstOrDefault(q => q.ID == -22232123);
            var x = flt.ID;
            return Ok(true);

        }

        [Route("api/fail2")]
        [AcceptVerbs("GET")]
        public IHttpActionResult FailHard()
        {
            //StackOverflow
            return FailHard();
        }

    }
    public class NRSCWSResult
    {
        public List<NRSCRSFlightData> NRSCRSFlightData { get; set; }
    }
    public class NRSCRSFlightData
    {
        public string Origin { get; set; }
        public string Destination { get; set; }
        public int? TotalBook { get; set; }
        public string FlightNo { get; set; }
        public int? Child_SA_Book { get; set; }
        public string FlightStatus { get; set; }
        public string DepartureDateTime { get; set; }
        public string Register { get; set; }
        public string ArrivalDateTime { get; set; }
        public int? ChildBook { get; set; }
        public int? Adult_SA_Book { get; set; }
        public string AircraftTypeCode { get; set; }
        public int? AdultBook { get; set; }

        public bool? Proccessed { get; set; }
        public int FlightStatusId
        {
            get
            {
                switch (this.FlightStatus.ToLower())
                {
                    case "o":
                        return 1;
                    case "x":
                        return 4;
                    default:
                        return -1;
                }
            }
        }

        private DateTime? std = null;
        private DateTime? sta = null;
        public DateTime? STD
        {
            get
            {
                if (std == null)
                {
                    //2021-07-16 23:00:00
                    var prts = this.DepartureDateTime.Split(' ');
                    var dtprts = prts[0].Split('-').Select(q => Convert.ToInt32(q)).ToList();
                    var tiprts = prts[1].Split(':').Select(q => Convert.ToInt32(q)).ToList();
                    std = new DateTime(dtprts[0], dtprts[1], dtprts[2], tiprts[0], tiprts[1], tiprts[2]);
                }
                return std;
            }
        }
        public DateTime? STDDay
        {
            get
            {
                if (this.STD == null)
                    return null;
                return ((DateTime)this.STD).Date;
            }
        }
        public DateTime? STA
        {
            get
            {
                if (sta == null)
                {
                    //2021-07-16 23:00:00
                    var prts = this.ArrivalDateTime.Split(' ');
                    var dtprts = prts[0].Split('-').Select(q => Convert.ToInt32(q)).ToList();
                    var tiprts = prts[1].Split(':').Select(q => Convert.ToInt32(q)).ToList();
                    sta = new DateTime(dtprts[0], dtprts[1], dtprts[2], tiprts[0], tiprts[1], tiprts[2]);
                }
                return sta;
            }
        }


    }
    public class NiraDto
    {
        public List<int> ids { get; set; }
        public string username { get; set; }
        public string password { get; set; }
    }
    public class _FLT
    {
        public int? FlightId { get; set; }
        public string FlightNo { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public DateTime? STD { get; set; }
        public DateTime? STA { get; set; }
        public int? StatusId { get; set; }
        public string Status { get; set; }
        public string Register { get; set; }
        public string Route
        {
            get
            {
                return this.Origin + '-' + this.Destination;
            }
        }
    }
    public class NiraConflictResult
    {
        public DateTime Date { get; set; }
        public _FLT AirPocket { get; set; }
        public _FLT Nira { get; set; }

        public bool IsNiraFound { get { return this.Nira != null; } }
        public bool? IsRegister
        {
            get
            {
                if (!this.IsNiraFound)
                    return null;
                if (string.IsNullOrEmpty(this.Nira.Register))
                    return false;
                if (this.AirPocket.StatusId == 4)
                    return true;
                if (this.Nira.Register.ToLower().EndsWith(this.AirPocket.Register.ToLower()))
                    return true;
                else
                    return false;
            }
        }
        public bool? IsStatus
        {
            get
            {
                if (!this.IsNiraFound)
                    return null;
                if (this.Nira.StatusId == this.AirPocket.StatusId)
                    return true;
                else
                    return false;
            }
        }
        public bool? IsSTD
        {
            get
            {
                if (!this.IsNiraFound)
                    return null;
                if (this.Nira.STD == this.AirPocket.STD)
                    return true;
                else
                    return false;
            }
        }
        public bool? IsSTA
        {
            get
            {
                if (!this.IsNiraFound)
                    return null;
                if (this.Nira.STA == this.AirPocket.STA)
                    return true;
                else
                    return false;
            }
        }

        public bool? IsRoute
        {
            get
            {
                if (!this.IsNiraFound)
                    return null;
                if (this.Nira.Route == this.AirPocket.Route)
                    return true;
                else
                    return false;
            }
        }

        public bool IsConflicted
        {
            get
            {
                if (!this.IsNiraFound)
                    return true;
                var result = IsRegister == true && IsSTA == true && IsSTD == true && IsStatus == true && IsRoute == true;
                return !result;
            }
        }

    }
}
