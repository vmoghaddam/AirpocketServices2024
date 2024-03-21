using AirpocketAPI.Models;
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
using System.Transactions;

namespace AirpocketAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class NiraController : ApiController
    {
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
            using (var context = new AirpocketAPI.Models.FLYEntities())
            {
                var flights = context.ViewLegTimes.Where(q => q.STDLocal >= dfrom && q.STDLocal <= dto && (q.FlightStatusID == 1 || q.FlightStatusID == 4)).ToList();
                var flightNos = flights.Select(q => q.FlightNumber).Distinct().ToList();


                foreach (var no in flightNos)
                {
                    try
                    {
                        string apiUrl = "http://iv.nirasoft.ir:882/NRSCWS.jsp?ModuleType=SP&ModuleName=CRSFlightData&DepartureDateFrom="
                       + _dfrom
                       + "&DepartureDateTo=" + _dto
                       + "&FlightNo=" + no
                       + "&OfficeUser=Thr003.airpocket&OfficePass=nira123";

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


        [Route("api/nira/conflicts/sync/{dfrom}/{dto}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetNiraConflictsSync(DateTime dfrom, DateTime dto)
        {
            List<NiraConflictResult> conflictResult = new List<NiraConflictResult>();
            var _dfrom = dfrom.Date.ToString("yyyy-MM-dd");
            var _dto = dto.Date.ToString("yyyy-MM-dd");
            dfrom = dfrom.Date;
            dto = dto.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            List<NRSCRSFlightData> niraFlights = new List<NRSCRSFlightData>();
            using (var context = new AirpocketAPI.Models.FLYEntities())
            {
                var flights = context.ViewLegTimes.Where(q => q.STDLocal >= dfrom && q.STDLocal <= dto && (q.FlightStatusID == 1 || q.FlightStatusID == 4)).ToList();
                var flightNos = flights.Select(q => q.FlightNumber).Distinct().ToList();


                foreach (var no in flightNos)
                {
                    try
                    {
                        string apiUrl = "http://iv.nirasoft.ir:882/NRSCWS.jsp?ModuleType=SP&ModuleName=CRSFlightData&DepartureDateFrom="
                       + _dfrom
                       + "&DepartureDateTo=" + _dto
                       + "&FlightNo=" + no
                       + "&OfficeUser=Thr003.airpocket&OfficePass=nira123";

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

            var regs = conflictResult.Where(q => q.IsRegister == false).ToList();

            var ids = regs.Select(q => (int)q.AirPocket.FlightId).ToList();
            var syncRes = AllNiraFiltered(ids);
            return Ok(syncRes);
        }


        [Route("api/nira/force/sync/{dfrom}/{dto}/{fns}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetNiraForceSync(DateTime dfrom, DateTime dto,string fns)
        {
            List<NiraConflictResult> conflictResult = new List<NiraConflictResult>();
            var _dfrom = dfrom.Date.ToString("yyyy-MM-dd");
            var _dto = dto.Date.ToString("yyyy-MM-dd");
            dfrom = dfrom.Date;
            dto = dto.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            var context = new AirpocketAPI.Models.FLYEntities();
            

            List<string> _fn =fns=="-1"?new List<string>(): fns.Split('_').ToList();

            var qry = context.ViewLegTimes.Where(q => q.STDLocal >= dfrom && q.STDLocal <= dto && (q.FlightStatusID == 1 || q.FlightStatusID == 4));
            if (_fn.Count > 0)
                qry = qry.Where(q => _fn.Contains(q.FlightNumber));
            var flights = qry.ToList();
                //conntext.ViewLegTimes.Where(q => q.STDLocal >= dfrom && q.STDLocal <= dto && (q.FlightStatusID == 1 || q.FlightStatusID == 4)).ToList();
           

            var ids = flights.Select(q => (int)q.ID).ToList();
            var syncRes = AllNiraFiltered(ids);
            return Ok(syncRes);
        }


        [Route("api/nira/conflicts/sync/id/{id}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetNiraConflictsSyncId(int id)
        {
            List<NiraConflictResult> conflictResult = new List<NiraConflictResult>();
            //var _dfrom = dfrom.Date.ToString("yyyy-MM-dd");
            //var _dto = dto.Date.ToString("yyyy-MM-dd");
            //dfrom = dfrom.Date;
            //dto = dto.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            List<NRSCRSFlightData> niraFlights = new List<NRSCRSFlightData>();
            using (var context = new AirpocketAPI.Models.FLYEntities())
            {
                var flights = context.ViewLegTimes.Where(q => q.ID == id).FirstOrDefault();
                //  var flightNos = flights.Select(q => q.FlightNumber).Distinct().ToList();



                try
                {
                    string apiUrl = "http://iv.nirasoft.ir:882/NRSCWS.jsp?ModuleType=SP&ModuleName=CRSFlightData&DepartureDateFrom="
                   + ((DateTime)flights.STDLocal).Date.ToString("yyyy-MM-dd")
                   + "&DepartureDateTo=" + ((DateTime)flights.STALocal).Date.ToString("yyyy-MM-dd")
                   + "&FlightNo=" + flights.FlightNumber
                   + "&OfficeUser=Thr003.airpocket&OfficePass=nira123";

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


                foreach (var x in niraFlights)
                    x.Proccessed = false;

                var aflt = flights;
                //foreach (var aflt in flights)
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

            var regs = conflictResult.Where(q => q.IsRegister == false).ToList();

            var ids = regs.Select(q => (int)q.AirPocket.FlightId).ToList();
            var syncRes = AllNiraFiltered(ids);
            return Ok(syncRes);
        }



        List<NiraHistory> AllNiraFiltered(List<int> ids)
        {
            //if (ConfigurationManager.AppSettings["nira"] == "0")
            //    return true;
            var histories = new List<NiraHistory>();
            var context = new AirpocketAPI.Models.FLYEntities();
            var legs = context.ViewLegTimes.Where(q => q.FlightStatusID == 1).OrderBy(q => q.STDLocal).ToList();
            if (ids != null && ids.Count > 0)
                legs = legs.Where(q => ids.IndexOf(q.ID) != -1).ToList();
            foreach (var leg in legs)
            {
                var _std = (DateTime)leg.STDLocal;
                string airline = "IV";
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
                //var duration = arr.Subtract(dep).TotalMinutes;
                //var _dep = dep.Year + "-" + dep.Month.ToString().PadLeft(2, '0') + "-" + dep.Day.ToString().PadLeft(2, '0') + "T" + dep.Hour + "%3A" + dep.Minute;
                var url2 = "http://iv.nirasoft.ir:882//NRSFlightInfo.jsp?Airline=" + airline + "&FlightNo=" + leg.FlightNumber
                    + "&Origin=" + leg.FromAirportIATA + "&Destination=" + leg.ToAirportIATA + "&FlightDate="
                    + date
                    + "&NewDepartureTime=" + dep + "&NewArrivalTime=" + arr + "&NewFlightStatus=" + status + "&NewACT=EP-" + leg.Register
                    + "&Comment=lorem_ipsum&FleetWatchKey" + leg.ID + "&SendSMS=NO&OfficeUser=" + "Thr003.airpocket" + "&OfficePass=" + "nira123";


                var url = "http://iv.nirasoft.ir:882//NRSFlightInfo.jsp?Airline=" + airline + "&FlightNo=" + leg.FlightNumber
                    + "&Origin=" + leg.FromAirportIATA + "&Destination=" + leg.ToAirportIATA + "&FlightDate="
                    + date
                    + "&NewDepartureTime=" + dep + "&NewDepartureDate=" + depDate + "&NewArrivalTime=" + arr + "&NewFlightStatus=" + status + "&NewACT=EP-" + leg.Register
                    + "&Comment=lorem_ipsum&FleetWatchKey=" + leg.ID + "&SendSMS=NO&OfficeUser=" + "Thr003.airpocket" + "&OfficePass=" + "nira123";
                var result = new NiraHistory()
                {
                    FlightId = leg.ID,
                    Arrival = leg.Arrival,
                    Departure = leg.Departure,
                    FlightStatusId = leg.FlightStatusID,
                    Register = leg.Register,
                    DateSend = DateTime.Now,
                    Remark = "AUTOSYNC-FORCED- " + url
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
                    result.NEWSTATUS =Convert.ToString(myObject.NEWSTATUS);
                                                // result.NEWSTATUS = _result; //Convert.ToString(myObject.NEWSTATUS);

                    context.NiraHistories.Add(result);
                    var saveResult = context.SaveChanges();
                    histories.Add(result);

                }
                catch (Exception ex)
                {
                    var msg = "ERROR: " + ex.Message;
                    if (ex.InnerException != null)
                        msg += " INNER:" + ex.InnerException.Message;
                    result.NEWSTATUS = msg;
                    var saveResult = context.SaveChanges();
                    histories.Add(result);
                }
            }

            return histories;
        }


        [Route("api/nira/notify/{id}/{ac}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult NotifyNira(int id, string ac)
        {
            var ids = new List<int> { id };
            var result = AllNiraFiltered(ids);
            return Ok(result);
            //string airlineCode =ac;
            //using (var _context = new AirpocketAPI.Models.FLYEntities())
            //{
            //    var _nreg = "";


            //    var leg =   _context.ViewLegTimes.FirstOrDefault(q => q.ID == id);
            //    if (leg == null)
            //        return Ok(false);
            //    if (leg.Register.ToLower().Contains("cnl"))
            //        return Ok(false);
            //    var _std = (DateTime)leg.STDLocal;
            //    string airline = airlineCode;// "IV";
            //    var date = (_std).Year + "-" + (_std).Month.ToString().PadLeft(2, '0') + "-" + (_std).Day.ToString().PadLeft(2, '0');
            //    if (leg.FlightDate != null)
            //    {
            //        var fd = ((DateTime)leg.FlightDate).Date;
            //        date = (fd).Year + "-" + (fd).Month.ToString().PadLeft(2, '0') + "-" + (fd).Day.ToString().PadLeft(2, '0');
            //    }


            //    var dep = ((DateTime)leg.DepartureLocal).ToString("HH:mm");
            //    var depDate = ((DateTime)leg.DepartureLocal).ToString("yyyy-MM-dd");
            //    var arr = ((DateTime)leg.ArrivalLocal).ToString("HH:mm");
            //    var status = "Scheduled";
            //    if (leg.FlightStatusID == 4)
            //        status = "Canceled";
            //    else if (leg.FlightStatusID == 2)
            //        status = "TookOff";
            //    else if (leg.FlightStatusID == 3)
            //        status = "Landed";
            //    //var duration = arr.Subtract(dep).TotalMinutes;
            //    //var _dep = dep.Year + "-" + dep.Month.ToString().PadLeft(2, '0') + "-" + dep.Day.ToString().PadLeft(2, '0') + "T" + dep.Hour + "%3A" + dep.Minute;

            //    var atireg = !string.IsNullOrEmpty(_nreg) ? _nreg : leg.Register;
            //    //http://fp.nirasoftware.com:882/NRSFlightInfo.jsp?Airline=FP&FlightNo=1002&Origin=UGT&Destination=TTQ&FlightDate=2022-02-02&NewDepartureTime=18%3A00&NewDepartureDate=2022-02-0&NewArrivalTime=20%3A00&NewFlightStatus=Scheduled&NewACT=EP-FPA&Comment=lorem_ipsum&FleetWatchKey=76475&SendSMS=false&OfficeUser=NSTHR007.FleetWatch&OfficePass=A2020
            //    //var url = "http://iv.nirasoftware.com:882/NRSFlightInfo.jsp?Airline=" + airline + "&FlightNo=" + leg.FlightNumber
            //    //    + "&Origin=" + leg.FromAirportIATA + "&Destination=" + leg.ToAirportIATA + "&FlightDate="
            //    //    + date
            //    //    + "&NewDepartureTime=" + dep + "&NewDepartureDate=" + depDate + "&NewArrivalTime=" + arr + "&NewFlightStatus=" + status + "&NewACT=EP-" + atireg
            //    //    + "&Comment=lorem_ipsum&FleetWatchKey=" + leg.ID + "&SendSMS=false&OfficeUser=" + "NSTHR007.FleetWatch" + "&OfficePass=" + "A2020";
            //    var url = "http://api.flypersia.aero/ws1/NRSFlightInfo.jsp?Airline=" + "FP" + "&FlightNo=" + leg.FlightNumber
            //        + "&Origin=" + leg.FromAirportIATA + "&Destination=" + leg.ToAirportIATA + "&FlightDate="
            //        + date
            //        + "&NewDepartureTime=" + dep + "&NewDepartureDate=" + depDate + "&NewArrivalTime=" + arr + "&NewFlightStatus=" + status + "&NewACT=EP-" + atireg
            //        + "&Comment=lorem_ipsum&FleetWatchKey=" + leg.ID + "&SendSMS=false&OfficeUser=" + "NSTHR007.FleetWatch" + "&OfficePass=" + "A2020";
            //    try
            //    {
            //        var result = new NiraHistory()
            //        {
            //            FlightId = leg.ID,
            //            Arrival = leg.Arrival,
            //            Departure = leg.Departure,
            //            FlightStatusId = leg.FlightStatusID,
            //            Register = leg.Register,
            //            DateSend = DateTime.Now,
            //            Remark =   url
            //        };
            //        WebRequest request = WebRequest.Create(url);

            //        request.Credentials = CredentialCache.DefaultCredentials;

            //        WebResponse response =   request.GetResponse ();

            //        Stream dataStream = response.GetResponseStream();

            //        StreamReader reader = new StreamReader(dataStream);

            //        string responseFromServer = reader.ReadToEnd();

            //        reader.Close();
            //        response.Close();

            //        result.DateReplied = DateTime.Now;


            //        dynamic myObject = JsonConvert.DeserializeObject<dynamic>(responseFromServer);
            //        result.CHTIME = Convert.ToString(myObject.CHTIME);
            //        result.FLIGHT = Convert.ToString(myObject.FLIGHT);
            //        result.NEWAIRCRAFT = Convert.ToString(myObject.NEWAIRCRAFT);
            //        result.NEWSTATUS = Convert.ToString(myObject.NEWSTATUS);

            //        _context.NiraHistories.Add(result);
            //        var saveResult =   _context.SaveChanges();

            //        return Ok( true);
            //    }
            //    catch (Exception ex)
            //    {
            //        var msg = ex.Message;
            //        if (ex.InnerException != null)
            //            msg += "_______" + ex.InnerException.Message;
            //        return Ok( new
            //        {
            //            MESSAGE = msg
            //        });
            //    }
            //}
        }

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

    //Child SA-Book
    //Adult SA-Book
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

    public class NRSCWSResultX
    {
        public List<NRSCRSFlightDataX> NRSCRSFlightData { get; set; }
    }

    public class NRSCRSFlightDataX
    {
        public string AirlineCode { get; set; }
          public string Origin { get; set; }
        public string Destination { get; set; }
        public string AircraftCode { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public string FleetWatchKey { get; set; }
        public string FlightNo { get; set; }
    }

}
