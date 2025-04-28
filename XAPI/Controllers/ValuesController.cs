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
    public class ValuesController : ApiController
    {
        [Route("api/flt")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetFlt(DateTime dt, string origin, string destination, string no, string key)
        {
            if (key != "taban@1359A")
                return BadRequest("Not Authorized");
            var ctx = new PPAEntities();
            var _dt = dt.Date;
            var _de = _dt.AddDays(1);
            var query = from x in ctx.ViewLegTimes
                        where x.STDLocal >= _dt && x.STDLocal < _de
                        select x;

            if (!string.IsNullOrEmpty(origin))
                query = query.Where(q => q.FromAirportIATA == origin);

            if (!string.IsNullOrEmpty(destination))
                query = query.Where(q => q.ToAirportIATA == destination);

            if (!string.IsNullOrEmpty(no))
                query = query.Where(q => q.FlightNumber == no);

            var result = query.ToList().OrderBy(q => q.STD).Select(q => new
            {
                FlightId = q.ID,
                Date = ((DateTime)q.STDLocal).Date,
                FltNo = q.FlightNumber,
                STD = q.STDLocal,
                STA = q.STALocal,
                STDUtc = q.STD,
                STAUtc = q.STA,
                DateUtc = ((DateTime)q.STD).Date,
                Dep = q.FromAirportIATA,
                Arr = q.ToAirportIATA,
                Departure = q.DepartureLocal,
                Arrival = q.ArrivalLocal,
                DepartureUtc = q.Departure,
                ArrivalUtc = q.Arrival,
                q.Register,
                q.FlightStatus



            }).ToList();

            return Ok(result);

        }


        public class fltQry
        {
            public DateTime? date { get; set; }
            public string key { get; set; }
        }
        [Route("api/flts")]
        [AcceptVerbs("POST")]
        public IHttpActionResult GetFlts(/*DateTime dt, string key*/fltQry dto)
        {
            if (dto.date == null)
                return Ok("Date not found.");
            if (string.IsNullOrEmpty(dto.key))
                return Ok("Authorization key not found.");

            if (dto.key != "taban@1359A")
                return Ok("Authorization key is wrong.");

            var ctx = new PPAEntities();
            var _dt = ((DateTime)dto.date).Date;
            var _de = _dt.AddDays(1);
            var query = from x in ctx.ViewLegTimes
                        where x.STD >= _dt && x.STD < _de
                        select x;



            var result = query.ToList().OrderBy(q => q.STD).Select(q => new
            {
                FlightId = q.ID,
                Date = ((DateTime)q.STDLocal).Date,
                FltNo = q.FlightNumber,
                STD = q.STDLocal,
                STA = q.STALocal,
                STDUtc = q.STD,
                STAUtc = q.STA,
                DateUtc = ((DateTime)q.STD).Date,
                Dep = q.FromAirportIATA,
                Arr = q.ToAirportIATA,
                Departure = q.DepartureLocal,
                Arrival = q.ArrivalLocal,
                DepartureUtc = q.Departure,
                ArrivalUtc = q.Arrival,
                q.Register,
                q.FlightStatus



            }).ToList();

            return Ok(result);

        }


        [Route("api/flt/flown")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetFlownFlt(DateTime Start, DateTime Finish, string Pass)
        {
            if (Pass != "kru@1359A")
                return BadRequest("Not Authorized");
            var ctx = new PPAEntities();
            var _dt = Start.Date;
            var _de = Finish.Date.AddDays(1).Date;
            var query = from x in ctx.ViewLegTimes
                        where x.STALocal >= _dt && x.STALocal < _de //&& (x.FlightStatusID == 15)
                        select x;

            //    Baggage,PAXADL,PAXCHD,PAXINF,TotalPAX,Status
            //2022 - 05 - 14,CH 8756,THR,AWZ,M82,CBH,2022 - 05 - 14 10:30:00,2022 - 05 - 14 11:40:00,2022 - 05 - 14 14:00:00,2022 - 05 - 14 15:10:00,0,123,1,0,124,F

            var result = query.ToList().OrderBy(q => q.STD).Select(q => new
            {

                Date = ((DateTime)q.STDLocal).Date,
                FltNo = q.FlightNumber,
                DepStn = q.FromAirportIATA,
                ArrStn = q.ToAirportIATA,
                ACType = q.AircraftType,
                ACReg = q.Register,
                DepTimeLCL = q.DepartureLocal,
                ArrTimeLCL = q.ArrivalLocal,
                DepTime = q.Departure,
                ArrTime = q.Arrival,
                Baggage = q.BaggageCount,
                PAXADL = q.PaxAdult != null ? q.PaxAdult : 0,
                PAXCHD = q.PaxChild != null ? q.PaxChild : 0,
                PAXINF = q.PaxInfant != null ? q.PaxInfant : 0,
                TotalPAX = (q.PaxAdult != null ? q.PaxAdult : 0) + (q.PaxChild != null ? q.PaxChild : 0) + (q.PaxInfant != null ? q.PaxInfant : 0),

                Status = q.FlightStatus





            }).ToList();

            return Ok(result);

        }


        [Route("api/mail")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetMail()
        {

            var mailRepository = new MailRepository("outlook.office365.com", 993, true, "flightcard.varesh@outlook.com", "Atrina1359");
            var allEmails = mailRepository.GetAllMails();

            //foreach (var email in allEmails)
            //{
            //    Console.WriteLine(email);
            //}

            //Assert.IsTrue(allEmails.ToList().Any());
            return Ok(allEmails);

        }
        [Route("api/mail/{flt}/{dt}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetMailByFlight(string flt, string dt)
        {

            var mailRepository = new MailRepository("outlook.office365.com", 993, true, "flightcard.varesh@outlook.com", "Atrina1359");
            var allEmails = mailRepository.GetAllMailsByFlight(flt, dt);

            //foreach (var email in allEmails)
            //{
            //    Console.WriteLine(email);
            //}

            //Assert.IsTrue(allEmails.ToList().Any());
            return Ok(allEmails);

        }


        [Route("api/fc/{flt}/{dt}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetFlightCardByFlight(string flt, string dt)
        {
            var ctx = new PPAEntities();
            var key = "FlightCard_" + dt + "_" + flt + ".pdf";
            var fc = ctx.FlightCards.Where(q => q.Key == key).OrderByDescending(q => q.DateCreate).FirstOrDefault();
            if (fc != null)
            {
                return Ok(new List<string>() { key });
            }
            else
            {
                var mailRepository = new MailRepository("outlook.office365.com", 993, true, "flightcard.varesh@outlook.com", "Atrina1359");
                var allEmails = mailRepository.GetAllMailsByFlight(flt, dt);

                //foreach (var email in allEmails)
                //{
                //    Console.WriteLine(email);
                //}

                //Assert.IsTrue(allEmails.ToList().Any());
                return Ok(allEmails);
            }


        }

        [Route("api/nira/chrs")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetFlightCardByFlight(string flt, DateTime dt)
        {
            var _dt = dt.ToString("yyyy-MM-dd");
            var url = "http://iv.nirasoft.ir:882/NRSCWS.jsp?ModuleType=SP&ModuleName=CharterOfficesCapacity&DepartureDateFrom=" + _dt + "&DepartureDateTo=" + _dt + "&FlightNo=" + flt + "&OfficeUser=Thr003.airpocket&OfficePass=nira123";
            WebRequest request = WebRequest.Create(url);

            request.Credentials = CredentialCache.DefaultCredentials;

            WebResponse response = request.GetResponse(); ;

            Stream dataStream = response.GetResponseStream();

            StreamReader reader = new StreamReader(dataStream);

            string responseFromServer = reader.ReadToEnd();
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseFromServer);

            return Ok(result);

        }


        public class ofp_dto
        {
            public string plan { get; set; }

        }
        //[Route("api/ofp")]
        //[AcceptVerbs("POST")]
        //public IHttpActionResult PostOFP(ofp_dto dto)
        //{
        //    var ctx = new PPAEntities();
        //    var ofp = new OFPSkyPuter();
        //    ofp.OFP = dto.plan;
        //    ofp.AIRLINE = "KARUN";
        //    ctx.OFPSkyPuters.Add(ofp);
        //    ctx.SaveChanges();
        //    return Ok(true);
        //}


        [Route("api/ofp")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostOFP()
        {
            string result = "";
            var ctx = new PPAEntities();
            try
            {
                result = await Request.Content.ReadAsStringAsync();

                result = result.Replace(": +", ": ");

                var qqqq = JsonConvert.DeserializeObject<Root>(result);



                var ofp = new OFPSkyPuter();
                ofp.OFP = result;
                ofp.AIRLINE = "KARUN";
                ctx.OFPSkyPuters.Add(ofp);
                ctx.SaveChanges();



                string responsebody = "NO";
                using (WebClient client = new WebClient())
                {
                    var reqparm = new System.Collections.Specialized.NameValueCollection();

                    // reqparm.Add("key", dto.key);
                    reqparm.Add("plan", result);
                    byte[] responsebytes = client.UploadValues("https://airpocket.karunair.ir/xapi/api/ofp/karun", "POST", reqparm);
                    responsebody = Encoding.UTF8.GetString(responsebytes);

                }






                return Ok(true);
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                if (ex.InnerException != null)
                    message += ex.InnerException.Message;
                var ofp = new OFPSkyPuter();
                ofp.OFP = result;
                ofp.UploadMessage = message;
                ofp.AIRLINE = "KARUN";
                ctx.OFPSkyPuters.Add(ofp);
                ctx.SaveChanges();
                return Ok(false);
            }

        }

        [Route("api/ofp/karun")]
        [AcceptVerbs("POST")]
        public IHttpActionResult PostOFPBKARUN(skyputer dto)
        {
            try
            {

                if (string.IsNullOrEmpty(dto.plan))
                    return Ok("Plan cannot be empty.");




                var entity = new OFPSkyPuter()
                {
                    OFP = dto.plan,
                    DateCreate = DateTime.Now,
                    UploadStatus = 0,


                };
                var ctx = new PPAEntities();
                ctx.Database.CommandTimeout = 1000;
                ctx.OFPSkyPuters.Add(entity);
                ctx.SaveChanges();
                new Thread(async () =>
                {
                    GetOFPBImport(entity.Id);

                }).Start();
                return Ok(true);



            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += " Inner: " + ex.InnerException.Message;
                return Ok(msg);
            }

        }
        [Route("api/ofp/b/import/{id}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetOFPBImport(int id)
        {

            var context = new PPAEntities();
            context.Database.CommandTimeout = 5000;
            var dto = context.OFPSkyPuters.Where(q => q.Id == id).FirstOrDefault();
            if (dto == null)
                return BadRequest("not found");


            try
            {
                var rawText = dto.OFP;
                var json_obj = JsonConvert.DeserializeObject<Root>(rawText);

                var _date = ((DateTime)json_obj.ScheduledTimeDeparture).Date;
                var _register = json_obj.TailNo.Replace("EP-", "");
                var _flight = context.ViewLegTimes.Where(q => q.STDDay == _date && q.FlightNumber == json_obj.FlightNo
                && q.Register == _register && q.FromAirportICAO == json_obj.Origin && q.ToAirportICAO == json_obj.Destination).FirstOrDefault();
                if (_flight == null)
                {
                    _flight = context.ViewLegTimes.OrderByDescending(q => q.ID).FirstOrDefault();
                    dto.UploadStatus = -1;
                    dto.UploadMessage = "flight not found";
                    context.SaveChanges();
                    return BadRequest("flight not found");
                }
                var exist = context.OFPB_Root.FirstOrDefault(q => q.FlightID == _flight.ID);
                if (exist != null)
                {
                    context.OFPB_Root.Remove(exist);
                    context.SaveChanges();
                }
                var fltobj = context.FlightInformations.Where(q => q.ID == _flight.ID).FirstOrDefault();
                OFPB_Root root = new OFPB_Root();
                root.RawOFPId = dto.Id;


                //root.ReferenceNo = json_obj.ReferenceNo;
                //root.AirlineName = json_obj.AirlineName;
                //root.WeightUnit = json_obj.WeightUnit;
                //root.CruisePerformanceFactor = json_obj.CruisePerformanceFactor;
                //root.ContingencyPercent = json_obj.ContingencyPercent;
                //root.FlightNo = json_obj.FlightNo;
                //root.GenerationDate = json_obj.GenerationDate;
                //root.ScheduledTimeDeparture = json_obj.ScheduledTimeDeparture;
                //root.ScheduledTimeArrival = json_obj.ScheduledTimeArrival;
                //root.TailNo = json_obj.TailNo;
                //root.CruiseSpeed = json_obj.CruiseSpeed;
                //root.CostIndex = json_obj.CostIndex;
                //root.MainFlightLevel = json_obj.MainFlightLevel;
                //root.Alternate1 = json_obj.Alternate1;
                //root.Alternate2 = json_obj.Alternate2;
                //root.Alternate1FlightLevel = json_obj.Alternate1FlightLevel;
                //root.Alternate2FlightLevel = json_obj.Alternate2FlightLevel;
                //root.Alternate1WC = "P001";
                //root.Alternate2WC = string.IsNullOrEmpty(json_obj.Alternate2) ? "" : "P001";
                //root.DryOperatingWeight = json_obj.DryOperatingWeight;
                //root.Payload = json_obj.Payload;
                //root.GroundDistance = json_obj.GroundDistance;
                //root.AirDistance = json_obj.AirDistance;
                //root.Origin = json_obj.Origin;
                //root.Destination = json_obj.Destination;
                //root.TakeoffAlternate = json_obj.TakeoffAlternate;
                //root.MODAlernate1 = json_obj.MODAlernate1;
                //root.MODAlternate2 = json_obj.MODAlternate2;
                //root.Cockpit = json_obj.Cockpit;
                //root.Cabin = json_obj.Cabin;
                //root.Extra = json_obj.Extra;
                //root.Pantry = json_obj.Pantry;
                //root.Pilot1 = json_obj.Pilot1;
                //root.Pilot2 = json_obj.Pilot2;
                //root.Dispatcher = json_obj.Dispatcher;
                //root.OriginElevation = json_obj.OriginElevation;
                //root.DestinationElevation = json_obj.DestinationElevation;
                //root.Alternate1Elevation = json_obj.Alternate1Elevation;
                //root.Alternate2Elevation = json_obj.Alternate2Elevation;
                //root.TakeoffAlternateElevation = json_obj.TakeoffAlternateElevation;
                //root.MaxShear = json_obj.MaxShear;
                //root.MaximumZeroFuelWeight = json_obj.MaximumZeroFuelWeight;
                //root.MaximumTakeoffWeight = json_obj.MaximumTakeoffWeight;
                //root.MaximumLandingWeight = json_obj.MaximumLandingWeight;
                //root.EstimatedZeroFuelWeight = json_obj.EstimatedZeroFuelWeight;
                //root.EstimatedTakeoffWeight = json_obj.EstimatedTakeoffWeight;
                //root.EstimatedLandingWeight = json_obj.EstimatedLandingWeight;
                //root.MainRoute = json_obj.MainRoute;
                //root.Alternate1Route = json_obj.Alternate1Route;
                //root.Alternate2Route = json_obj.Alternate2Route;
                //root.TakeoffAlternateRoute = json_obj.TakeoffAlternateRoute;
                //root.PlanValidity = json_obj.PlanValidity;

                //root.MaxWindShearLevel = json_obj.MaxWindShearLevel;
                //root.MaxWindShearPointName = json_obj.MaxWindShearPointName;
                //root.FlightRule = json_obj.FlightRule;



                //root.MSN = json_obj.MSN;


                //root.AircraftType = json_obj.AircraftType;

                //root.AircraftSubType = json_obj.AircraftSubType;

                //root.ManeuveringTime = json_obj.ManeuveringTime;

                //root.ManeuveringFuel = json_obj.ManeuveringFuel;



                //root.WeatherCycle = json_obj.WeatherCycle;

                //root.Warning1 = json_obj.Warning1;

                //root.Warning2 = json_obj.Warning2;

                //root.Warning3 = json_obj.Warning3;

                //root.TripAverageWindComponent = json_obj.TripAverageWindComponent;

                //root.TripAverageTempISA = json_obj.TripAverageTempISA;

                //root.TripLevel = json_obj.TripLevel;

                //root.Alternate1AverageWindComponent = json_obj.Alternate1AverageWindComponent;

                //root.Alternate1AverageTempISA = json_obj.Alternate1AverageTempISA;

                //root.Alternate2AverageWindComponent = json_obj.Alternate2AverageWindComponent;

                //root.Alternate2AverageTempISA = json_obj.Alternate2AverageTempISA;

                fltobj.ALT1 = root.Alternate1;
                fltobj.ALT2 = root.Alternate2;


                root.AircraftSubType = json_obj.AircraftSubType;
                root.AircraftType = json_obj.AircraftType;
                root.AirDistance = json_obj.AirDistance;
                root.AirlineName = json_obj.AirlineName;
                root.Alternate = json_obj.Alternate;
                root.Alternate1 = json_obj.Alternate1;
                root.Alternate1AverageTempISA = json_obj.Alternate1AverageTempISA;
                root.Alternate1AverageWindComponent = json_obj.Alternate1AverageWindComponent;
                root.Alternate1Elevation = json_obj.Alternate1Elevation;
                root.Alternate1FlightLevel = json_obj.Alternate1FlightLevel;
                //root.Alternate1NavLog = json_obj.Alternate1NavLog;
                root.Alternate1Route = json_obj.Alternate1Route;
                // root.Alternate1WindTemperature = json_obj.Alternate1WindTemperature;
                root.Alternate2 = json_obj.Alternate2;
                root.Alternate2AverageTempISA = json_obj.Alternate2AverageTempISA;
                root.Alternate2AverageWindComponent = json_obj.Alternate2AverageWindComponent;
                root.Alternate2Elevation = json_obj.Alternate2Elevation;
                root.Alternate2FlightLevel = json_obj.Alternate2FlightLevel;
                //root.Alternate2NavLog = json_obj.Alternate2NavLog;
                root.Alternate2Route = json_obj.Alternate2Route;
                //root.Alternate2WindTemperature = json_obj.Alternate2WindTemperature;
                root.AlternateEnroute = json_obj.AlternateEnroute;
                //root.BurnOffAdjustment = json_obj.BurnOffAdjustment;
                root.Cabin = json_obj.Cabin;
                root.Cockpit = json_obj.Cockpit;
                root.ContingencyPercent = json_obj.ContingencyPercent;
                root.CostIndex = json_obj.CostIndex;
                root.CruiseSpeed = json_obj.CruiseSpeed;
                root.CruisePerformanceFactor = Convert.ToInt32(Math.Round(json_obj.CruisePerformanceFactor != null ? (double)json_obj.CruisePerformanceFactor : 0));
                root.Destination = json_obj.Destination;
                root.DestinationElevation = json_obj.DestinationElevation;
                root.DestinationIATA = json_obj.DestinationIATA;
                //root.Distances = json_obj.Distances;
                root.DryOperatingWeight = json_obj.DryOperatingWeight;
                root.Dispatcher = json_obj.Dispatcher;
                root.EstimatedLandingWeight = json_obj.EstimatedLandingWeight;
                root.EstimatedTakeoffWeight = json_obj.EstimatedTakeoffWeight;
                root.EstimatedZeroFuelWeight = json_obj.EstimatedZeroFuelWeight;
                //root.FIRs = json_obj.FIRs;
                root.FlightNo = json_obj.FlightNo;
                root.FlightRule = json_obj.FlightRule;
                // root.Fuels = json_obj.Fuels;
                root.GenerationDate = json_obj.GenerationDate;
                root.GroundDistance = json_obj.GroundDistance;
                //root.HeightChange = json_obj.HeightChange;
                root.ICAOFlightPlan = json_obj.ICAOFlightPlan;
                root.MainRoute = json_obj.MainRoute;
                // root.MainNavLog = json_obj.MainNavLog;
                root.MainFlightLevel = json_obj.MainFlightLevel;
                //root.MainWindTemperature = json_obj.MainWindTemperature;
                root.ManeuveringFuel = json_obj.ManeuveringFuel;
                root.ManeuveringTime = json_obj.ManeuveringTime;
                root.MaxShear = json_obj.MaxShear;
                root.MaxWindShearLevel = json_obj.MaxWindShearLevel;
                root.MaxWindShearPointName = json_obj.MaxWindShearPointName;
                root.MaximumLandingWeight = json_obj.MaximumLandingWeight;
                root.MaximumTakeoffWeight = json_obj.MaximumTakeoffWeight;
                root.MaximumZeroFuelWeight = json_obj.MaximumZeroFuelWeight;
                root.MODAlernate1 = json_obj.MODAlernate1;
                root.MODAlternate2 = json_obj.MODAlternate2;
                root.MSN = json_obj.MSN;
                root.Origin = json_obj.Origin;
                root.OriginElevation = json_obj.OriginElevation;
                root.OriginIATA = json_obj.OriginIATA;
                root.Pantry = json_obj.Pantry;
                root.Payload = json_obj.Payload;
                root.PlanValidity = json_obj.PlanValidity;
                root.Pilot1 = json_obj.Pilot1;
                root.Pilot2 = json_obj.Pilot2;
                root.ReferenceNo = json_obj.ReferenceNo;
                root.ScheduledTimeArrival = json_obj.ScheduledTimeArrival;
                root.ScheduledTimeDeparture = json_obj.ScheduledTimeDeparture;
                root.TailNo = json_obj.TailNo;
                root.TakeoffAlternate = json_obj.TakeoffAlternate;
                root.TakeoffAlternateElevation = json_obj.TakeoffAlternateElevation;
                root.TakeoffAlternateFlightLevel = json_obj.TakeoffAlternateFlightLevel;
                root.TakeoffAlternateRoute = json_obj.TakeoffAlternateRoute;
                //root.TakeOffAlternateWindTemperature = json_obj.TakeOffAlternateWindTemperature;
                //root.Times = json_obj.Times;
                root.TripAverageTempISA = json_obj.TripAverageTempISA;
                root.TripAverageWindComponent = json_obj.TripAverageWindComponent;
                root.TripLevel = json_obj.TripLevel;
                root.Warning1 = json_obj.Warning1;
                root.Warning2 = json_obj.Warning2;
                root.Warning3 = json_obj.Warning3;
                root.WeatherCycle = json_obj.WeatherCycle;
                root.WeightUnit = json_obj.WeightUnit;
                root.Extra = json_obj.Extra;



                root.firs_main = json_obj.FIRs.Main;

                root.FlightID = fltobj.ID;
                //root.FlightID
                root.DateCreate = DateTime.Now;


                //              "Fuels": {
                //                  "Trip": 2389,
                //  "Alternate": 1724,
                //  "Holding": 1050,
                //  "Contingency": 225,
                //  "TaxiOut": 100,
                //  "TaxiIn": 0,
                //  "MinimumRequired": 5489,
                //  "Additional": 0,
                //  "Extra": 0,
                //  "Total": 5489,
                //  "Landing": 2999,
                //  "MODAlternate1": 2774,
                //  "MODAlternate2": 0
                //},
                var fuels = json_obj.Fuels;
                root.fuel_additional = fuels.Additional;
                root.fuel_alt = fuels.Alternate;
                root.fuel_alt1 = fuels.Alternate1;
                root.fuel_alt2 = fuels.Alternate2;
                root.fuel_contigency = fuels.Contingency;
                root.fuel_extra = fuels.Extra;
                root.fuel_holding = fuels.Holding;
                root.fuel_landing = fuels.Landing;
                root.fuel_min_required = fuels.MinimumRequired;
                root.fuel_mod_alt1 = fuels.MODAlternate1;
                root.fuel_mod_alt2 = fuels.MODAlternate2;
                root.fuel_taxiin = fuels.TaxiIn;
                root.fuel_taxiout = fuels.TaxiOut;
                root.fuel_trip = fuels.Trip;
                root.fuel_total = fuels.Total;



                ////////////////////////////////////////
                ///
                fltobj.OFPTRIPFUEL = root.fuel_trip;
                fltobj.OFPCONTFUEL = Convert.ToInt32(root.fuel_contigency);
                fltobj.OFPALT1FUEL = root.fuel_mod_alt1;
                fltobj.OFPALT2FUEL = root.fuel_mod_alt2;
                fltobj.OFPFINALRESFUEL = root.fuel_holding;
                fltobj.OFPETOPSADDNLFUEL = root.fuel_additional;
                //fltobj.OFPOPSEXTRAFUEL = Convert.ToInt32(val);
                fltobj.OFPMINTOFFUEL = root.fuel_min_required;

                //fltobj.OFPTANKERINGFUEL = Convert.ToInt32(val);

                // fltobj.ACTUALTANKERINGFUEL = Convert.ToInt32(val);
                fltobj.OFPTAXIFUEL = root.fuel_taxiout;
                fltobj.OFPTOTALFUEL = root.fuel_total;
                // fltobj.OFPOFFBLOCKFUEL = root.fuel_total;
                fltobj.OFPExtra = root.fuel_extra;

                //////////////////////////////////////////


                var tms = json_obj.Times;
                root.time_additional = ofpb_time_to_int(tms.Additional);
                root.time_alt = ofpb_time_to_int(tms.Alternate);
                root.time_alt1 = ofpb_time_to_int(tms.Alternate1);
                root.time_alt2 = ofpb_time_to_int(tms.Alternate2);
                root.time_alt_takeof = ofpb_time_to_int(tms.TakeOffAlternate);
                root.time_contigency = ofpb_time_to_int(tms.Contingency);
                root.time_extra = ofpb_time_to_int(tms.Extra);
                root.time_holding = ofpb_time_to_int(tms.Holding);
                root.time_min_required = ofpb_time_to_int(tms.MinimumRequired);
                root.time_total = ofpb_time_to_int(tms.Total);
                root.time_trip = ofpb_time_to_int(tms.Trip);

                root.time_final_reserve = ofpb_time_to_int(tms.FinalReserve);
                root.time_taxi_in = ofpb_time_to_int(tms.TaxiIn);
                root.time_taxi_out = ofpb_time_to_int(tms.TaxiOut);
                root.AdditionalStr = tms.AdditionalStr;
                root.AlternateStr = tms.AlternateStr;
                root.Alternate1Str = tms.Alternate1Str;
                root.Alternate2Str = tms.Alternate2Str;
                root.ContingencyStr = tms.ContingencyStr;
                root.ExtraStr = tms.ExtraStr;
                root.HoldingStr = tms.HoldingStr;
                root.MinimumRequiredStr = tms.MinimumRequiredStr;
                root.TakeOffAlternateStr = tms.TakeOffAlternateStr;
                root.TaxiInStr = tms.TaxiInStr;
                root.TaxiOutStr = tms.TaxiOutStr;
                root.TotalStr = tms.TotalStr;
                root.TripStr = tms.TripStr;






                var dis = json_obj.Distances;
                root.dis_air = dis.AirDistance;
                root.dis_alt1 = dis.Alternate1;
                root.dis_alt2 = dis.Alternate2;
                root.dis_alt_takeoff = dis.TakeOffAlternate;
                root.dis_ground = dis.GroundDistance;
                root.dis_trip = dis.Trip;
                root.dis_maingcd = dis.MainGCD;



                root.burnoffadj_fuel = Convert.ToString(json_obj.BurnOffAdjustment.Fuel);
                root.burnoffadj_value = Convert.ToString(json_obj.BurnOffAdjustment.Value);

                root.heightchange_fuel = Convert.ToString(json_obj.HeightChange.Fuel);
                root.heightchange_value = Convert.ToString(json_obj.HeightChange.Value);






                var main_route = json_obj.MainNavLog;
                foreach (var pt in main_route)
                {
                    root.OFPB_MainNavLog.Add(new OFPB_MainNavLog()
                    {
                        NavType = "MAIN",
                        WayPoint = pt.WayPoint,
                        FlightLevel = pt.FlightLevel,
                        Latitude = pt.Latitude,
                        Longitude = pt.Longitude,
                        Frequency = pt.Frequency,
                        Airway = pt.Airway,
                        MEA = pt.MEA,
                        MORA = pt.MORA,
                        ZoneDistance = pt.ZoneDistance,
                        CumulativeDistance = pt.CumulativeDistance,
                        Wind = pt.Wind,
                        MagneticTrack = pt.MagneticTrack,
                        Temperature = pt.Temperature,
                        ZoneTime = pt.ZoneTime,
                        CumulativeTime = pt.CumulativeTime,
                        FuelRemained = pt.FuelRemained,
                        FuelUsed = pt.ZoneFuel,
                        MachNo = pt.MachNo,
                        TrueAirSpeed = pt.TrueAirSpeed,
                        GroundSpeed = pt.GroundSpeed,
                        LatitudeStr = pt.LatitudeStr,
                        LongitudeStr = pt.LongitudeStr,


                        CumulativeFuel = pt.CumulativeFuel,

                        Direction = pt.Direction,

                        WindComponent = pt.WindComponent,

                        ZoneFuel = pt.ZoneFuel,

                    });
                }


                var alt1_route = json_obj.Alternate1NavLog;
                if (alt1_route != null && alt1_route.Count > 0)
                {
                    foreach (var pt in alt1_route)
                    {
                        root.OFPB_MainNavLog.Add(new OFPB_MainNavLog()
                        {
                            NavType = "ALT1",

                            WayPoint = pt.WayPoint,
                            FlightLevel = pt.FlightLevel,
                            Latitude = pt.Latitude,
                            Longitude = pt.Longitude,
                            Frequency = pt.Frequency,
                            Airway = pt.Airway,
                            MEA = pt.MEA,
                            MORA = pt.MORA,
                            ZoneDistance = pt.ZoneDistance,
                            CumulativeDistance = pt.CumulativeDistance,
                            Wind = pt.Wind,
                            MagneticTrack = pt.MagneticTrack,
                            Temperature = pt.Temperature,
                            ZoneTime = pt.ZoneTime,
                            CumulativeTime = pt.CumulativeTime,
                            FuelRemained = pt.FuelRemained,
                            FuelUsed = pt.ZoneFuel,
                            MachNo = pt.MachNo,
                            TrueAirSpeed = pt.TrueAirSpeed,
                            GroundSpeed = pt.GroundSpeed,
                            LatitudeStr = pt.LatitudeStr,
                            LongitudeStr = pt.LongitudeStr,
                            CumulativeFuel = pt.CumulativeFuel,

                            Direction = pt.Direction,

                            WindComponent = pt.WindComponent,

                            ZoneFuel = pt.ZoneFuel,
                        });
                    }

                }

                var alt2_route = json_obj.Alternate2NavLog;
                if (alt2_route != null && alt2_route.Count > 0)
                {
                    foreach (var pt in alt2_route)
                    {
                        root.OFPB_MainNavLog.Add(new OFPB_MainNavLog()
                        {
                            NavType = "ALT2",

                            WayPoint = pt.WayPoint,
                            FlightLevel = pt.FlightLevel,
                            Latitude = pt.Latitude,
                            Longitude = pt.Longitude,
                            Frequency = pt.Frequency,
                            Airway = pt.Airway,
                            MEA = pt.MEA,
                            MORA = pt.MORA,
                            ZoneDistance = pt.ZoneDistance,
                            CumulativeDistance = pt.CumulativeDistance,
                            Wind = pt.Wind,
                            MagneticTrack = pt.MagneticTrack,
                            Temperature = pt.Temperature,
                            ZoneTime = pt.ZoneTime,
                            CumulativeTime = pt.CumulativeTime,
                            FuelRemained = pt.FuelRemained,
                            FuelUsed = pt.ZoneFuel,
                            MachNo = pt.MachNo,
                            TrueAirSpeed = pt.TrueAirSpeed,
                            GroundSpeed = pt.GroundSpeed,
                            LatitudeStr = pt.LatitudeStr,
                            LongitudeStr = pt.LongitudeStr,
                            CumulativeFuel = pt.CumulativeFuel,

                            Direction = pt.Direction,

                            WindComponent = pt.WindComponent,

                            ZoneFuel = pt.ZoneFuel,
                        });
                    }
                }

                var _name = "";
                if (json_obj.MainWindTemperature != null)
                    foreach (var x in json_obj.MainWindTemperature)
                    {
                        var _str = x.ToString();
                        Dictionary<string, string> rows = JsonConvert.DeserializeObject<Dictionary<string, string>>(_str);
                        _name = rows["Name"];
                        foreach (var kvp in rows)
                        {
                            if (kvp.Key == "Name")
                                root.OFPB_WindTemperature.Add(new OFPB_WindTemperature()
                                {
                                    Type = "MAIN",
                                    WayPoint = _name,

                                });
                            else
                                root.OFPB_WindTemperature.Add(new OFPB_WindTemperature()
                                {
                                    Type = "MAIN",
                                    WayPoint = _name,
                                    FlightLevel = kvp.Key,
                                    WindTemprature = kvp.Value,
                                });

                        }

                        if (rows.Count < 5)
                        {
                            var n = 5 - rows.Count;
                            for (int y = 1; y <= n; y++)
                                root.OFPB_WindTemperature.Add(new OFPB_WindTemperature()
                                {
                                    Type = "MAIN",
                                    WayPoint = _name,

                                });

                        }



                    }



                _name = "";
                if (json_obj.Alternate1WindTemperature != null)
                    foreach (var x in json_obj.Alternate1WindTemperature)
                    {
                        var _str = x.ToString();
                        Dictionary<string, string> rows = JsonConvert.DeserializeObject<Dictionary<string, string>>(_str);
                        _name = rows["Name"];
                        foreach (var kvp in rows)
                        {
                            if (kvp.Key == "Name")
                                root.OFPB_WindTemperature.Add(new OFPB_WindTemperature()
                                {
                                    Type = "ALT1",
                                    WayPoint = _name,

                                });
                            else
                                root.OFPB_WindTemperature.Add(new OFPB_WindTemperature()
                                {
                                    Type = "ALT1",
                                    WayPoint = _name,
                                    FlightLevel = kvp.Key,
                                    WindTemprature = kvp.Value,
                                });

                        }
                        if (rows.Count < 5)
                        {
                            var n = 5 - rows.Count;
                            for (int y = 1; y <= n; y++)
                                root.OFPB_WindTemperature.Add(new OFPB_WindTemperature()
                                {
                                    Type = "ALT1",
                                    WayPoint = _name,

                                });

                        }

                    }
                //if (json_obj.Alternate1WindTemperature != null && json_obj.Alternate1WindTemperature.Count < 5 && !string.IsNullOrEmpty(_name))
                //{
                //    var n = 5 - json_obj.Alternate1WindTemperature.Count;
                //    for (int y = 1; y <= n; y++)
                //        root.OFPB_WindTemperature.Add(new OFPB_WindTemperature()
                //        {
                //            Type = "ALT1",
                //            WayPoint = _name,

                //        });

                //}


                _name = "";
                if (json_obj.Alternate2WindTemperature != null)
                    foreach (var x in json_obj.Alternate2WindTemperature)
                    {
                        var _str = x.ToString();
                        Dictionary<string, string> rows = JsonConvert.DeserializeObject<Dictionary<string, string>>(_str);
                        _name = rows["Name"];
                        foreach (var kvp in rows)
                        {
                            if (kvp.Key == "Name")
                                root.OFPB_WindTemperature.Add(new OFPB_WindTemperature()
                                {
                                    Type = "ALT2",
                                    WayPoint = _name,

                                });
                            else
                                root.OFPB_WindTemperature.Add(new OFPB_WindTemperature()
                                {
                                    Type = "ALT2",
                                    WayPoint = _name,
                                    FlightLevel = kvp.Key,
                                    WindTemprature = kvp.Value,
                                });

                        }
                        if (rows.Count < 5)
                        {
                            var n = 5 - rows.Count;
                            for (int y = 1; y <= n; y++)
                                root.OFPB_WindTemperature.Add(new OFPB_WindTemperature()
                                {
                                    Type = "ALT2",
                                    WayPoint = _name,

                                });

                        }

                    }
                //if (json_obj.Alternate2WindTemperature != null && json_obj.Alternate2WindTemperature.Count < 5 && !string.IsNullOrEmpty(_name))
                //{
                //    var n = 5 - json_obj.Alternate2WindTemperature.Count;
                //    for (int y = 1; y <= n; y++)
                //        root.OFPB_WindTemperature.Add(new OFPB_WindTemperature()
                //        {
                //            Type = "ALT2",
                //            WayPoint = _name,

                //        });

                //}

                _name = "";
                if (json_obj.TakeOffAlternateWindTemperature != null)
                    foreach (var x in json_obj.TakeOffAlternateWindTemperature)
                    {
                        var _str = x.ToString();
                        Dictionary<string, string> rows = JsonConvert.DeserializeObject<Dictionary<string, string>>(_str);
                        _name = rows["Name"];
                        foreach (var kvp in rows)
                        {
                            if (kvp.Key == "Name")
                                root.OFPB_WindTemperature.Add(new OFPB_WindTemperature()
                                {
                                    Type = "ALTTO",
                                    WayPoint = _name,

                                });
                            else
                                root.OFPB_WindTemperature.Add(new OFPB_WindTemperature()
                                {
                                    Type = "ALTTO",
                                    WayPoint = _name,
                                    FlightLevel = kvp.Key,
                                    WindTemprature = kvp.Value,
                                });

                        }
                        if (rows.Count < 5)
                        {
                            var n = 5 - rows.Count;
                            for (int y = 1; y <= n; y++)
                                root.OFPB_WindTemperature.Add(new OFPB_WindTemperature()
                                {
                                    Type = "ALTTO",
                                    WayPoint = _name,

                                });

                        }

                    }
                //if (json_obj.TakeOffAlternateWindTemperature != null && json_obj.TakeOffAlternateWindTemperature.Count < 5 && !string.IsNullOrEmpty(_name))
                //{
                //    var n = 5 - json_obj.TakeOffAlternateWindTemperature.Count;
                //    for (int y = 1; y <= n; y++)
                //        root.OFPB_WindTemperature.Add(new OFPB_WindTemperature()
                //        {
                //            Type = "ALTTO",
                //            WayPoint = _name,

                //        });

                //}


                dto.DateUpload = DateTime.Now;
                dto.UploadStatus = 1;
                dto.UploadMessage = "OK";
                context.OFPB_Root.Add(root);
                context.SaveChanges();
                return Ok(true);



            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += ex.InnerException.Message;
                dto.UploadStatus = -1;
                dto.UploadMessage = msg;
                context.SaveChanges();
                return Ok("Not Uploaded ");
            }
        }

        [Route("api/ofp/b/import/test/{id}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetOFPBImportTest(int id)
        {

            var context = new PPAEntities();
            context.Database.CommandTimeout = 5000;
            var dto = context.OFPSkyPuters.Where(q => q.Id == id).FirstOrDefault();
            if (dto == null)
                return BadRequest("not found");


            try
            {
                var rawText = dto.OFP;
                var json_obj = JsonConvert.DeserializeObject<Root>(rawText);

                var _date = ((DateTime)json_obj.ScheduledTimeDeparture).Date;
                var _register = json_obj.TailNo.Replace("EP-", "");



                OFPB_Root root = new OFPB_Root();
                root.RawOFPId = dto.Id;

                root.ReferenceNo = json_obj.ReferenceNo;
                root.AirlineName = json_obj.AirlineName;
                root.WeightUnit = json_obj.WeightUnit;
                root.CruisePerformanceFactor = Convert.ToInt32(json_obj.CruisePerformanceFactor == null ? 0 : (double)json_obj.CruisePerformanceFactor);
                root.ContingencyPercent = json_obj.ContingencyPercent;
                root.FlightNo = json_obj.FlightNo;
                root.GenerationDate = json_obj.GenerationDate;
                root.ScheduledTimeDeparture = json_obj.ScheduledTimeDeparture;
                root.ScheduledTimeArrival = json_obj.ScheduledTimeArrival;
                root.TailNo = json_obj.TailNo;
                root.CruiseSpeed = json_obj.CruiseSpeed;
                root.CostIndex = json_obj.CostIndex;
                root.MainFlightLevel = json_obj.MainFlightLevel;
                root.Alternate1 = json_obj.Alternate1;
                root.Alternate2 = json_obj.Alternate2;
                root.Alternate1FlightLevel = json_obj.Alternate1FlightLevel;
                root.Alternate2FlightLevel = json_obj.Alternate2FlightLevel;
                root.Alternate1WC = "P001";
                root.Alternate2WC = string.IsNullOrEmpty(json_obj.Alternate2) ? "" : "P001";
                root.DryOperatingWeight = json_obj.DryOperatingWeight;
                root.Payload = json_obj.Payload;
                root.GroundDistance = json_obj.GroundDistance;
                root.AirDistance = json_obj.AirDistance;
                root.Origin = json_obj.Origin;
                root.Destination = json_obj.Destination;


                root.TakeoffAlternate = json_obj.TakeoffAlternate;
                root.MODAlernate1 = json_obj.MODAlernate1;
                root.MODAlternate2 = json_obj.MODAlternate2;
                root.Cockpit = json_obj.Cockpit;
                root.Cabin = json_obj.Cabin;
                root.Extra = json_obj.Extra;
                root.Pantry = json_obj.Pantry;
                root.Pilot1 = json_obj.Pilot1;
                root.Pilot2 = json_obj.Pilot2;
                root.Dispatcher = json_obj.Dispatcher;
                root.OriginElevation = json_obj.OriginElevation;
                root.DestinationElevation = json_obj.DestinationElevation;
                root.Alternate1Elevation = json_obj.Alternate1Elevation;
                root.Alternate2Elevation = json_obj.Alternate2Elevation;
                root.TakeoffAlternateElevation = json_obj.TakeoffAlternateElevation;
                root.MaxShear = json_obj.MaxShear;
                root.MaximumZeroFuelWeight = json_obj.MaximumZeroFuelWeight;
                root.MaximumTakeoffWeight = json_obj.MaximumTakeoffWeight;
                root.MaximumLandingWeight = json_obj.MaximumLandingWeight;
                root.EstimatedZeroFuelWeight = json_obj.EstimatedZeroFuelWeight;
                root.EstimatedTakeoffWeight = json_obj.EstimatedTakeoffWeight;
                root.EstimatedLandingWeight = json_obj.EstimatedLandingWeight;
                root.MainRoute = json_obj.MainRoute;
                root.Alternate1Route = json_obj.Alternate1Route;
                root.Alternate2Route = json_obj.Alternate2Route;
                root.TakeoffAlternateRoute = json_obj.TakeoffAlternateRoute;
                root.PlanValidity = json_obj.PlanValidity;

                root.MaxWindShearLevel = json_obj.MaxWindShearLevel;
                root.MaxWindShearPointName = json_obj.MaxWindShearPointName;
                root.FlightRule = json_obj.FlightRule;
                root.FlightRule = json_obj.FlightRule;
                //root.FlightID = fltobj.ID;
                //root.FlightID
                root.DateCreate = DateTime.Now;


                //              "Fuels": {
                //                  "Trip": 2389,
                //  "Alternate": 1724,
                //  "Holding": 1050,
                //  "Contingency": 225,
                //  "TaxiOut": 100,
                //  "TaxiIn": 0,
                //  "MinimumRequired": 5489,
                //  "Additional": 0,
                //  "Extra": 0,
                //  "Total": 5489,
                //  "Landing": 2999,
                //  "MODAlternate1": 2774,
                //  "MODAlternate2": 0
                //},
                var fuels = json_obj.Fuels;
                root.fuel_additional = fuels.Additional;
                root.fuel_alt = fuels.Alternate;
                root.fuel_contigency = fuels.Contingency;
                root.fuel_extra = fuels.Extra;
                root.fuel_holding = fuels.Holding;
                root.fuel_landing = fuels.Landing;
                root.fuel_min_required = fuels.MinimumRequired;
                root.fuel_mod_alt1 = fuels.MODAlternate1;
                root.fuel_mod_alt2 = fuels.MODAlternate2;
                root.fuel_taxiin = fuels.TaxiIn;
                root.fuel_taxiout = fuels.TaxiOut;
                root.fuel_trip = fuels.Trip;
                root.fuel_total = fuels.Total;



                ////////////////////////////////////////
                ///


                //////////////////////////////////////////


                var tms = json_obj.Times;
                root.time_additional = ofpb_time_to_int(tms.Additional);
                root.time_alt = ofpb_time_to_int(tms.Alternate);
                root.time_alt1 = ofpb_time_to_int(tms.Alternate1);
                root.time_alt2 = ofpb_time_to_int(tms.Alternate2);
                root.time_alt_takeof = ofpb_time_to_int(tms.TakeOffAlternate);
                root.time_contigency = ofpb_time_to_int(tms.Contingency);
                root.time_extra = ofpb_time_to_int(tms.Extra);
                root.time_holding = ofpb_time_to_int(tms.Holding);
                root.time_min_required = ofpb_time_to_int(tms.MinimumRequired);
                root.time_total = ofpb_time_to_int(tms.Total);
                root.time_trip = ofpb_time_to_int(tms.Trip);


                var dis = json_obj.Distances;
                root.dis_air = dis.AirDistance;
                root.dis_alt1 = dis.Alternate1;
                root.dis_alt2 = dis.Alternate2;
                root.dis_alt_takeoff = dis.TakeOffAlternate;
                root.dis_ground = dis.GroundDistance;
                root.dis_air = dis.AirDistance;
                root.dis_trip = dis.Trip;



                root.burnoffadj_fuel = Convert.ToString(json_obj.BurnOffAdjustment.Fuel);
                root.burnoffadj_value = Convert.ToString(json_obj.BurnOffAdjustment.Value);

                root.heightchange_fuel = Convert.ToString(json_obj.HeightChange.Fuel);
                root.heightchange_value = Convert.ToString(json_obj.HeightChange.Value);






                var main_route = json_obj.MainNavLog;
                foreach (var pt in main_route)
                {
                    root.OFPB_MainNavLog.Add(new OFPB_MainNavLog()
                    {
                        NavType = "MAIN",

                        WayPoint = pt.WayPoint,
                        FlightLevel = pt.FlightLevel,
                        Latitude = pt.Latitude,
                        Longitude = pt.Longitude,
                        Frequency = pt.Frequency,
                        Airway = pt.Airway,
                        MEA = pt.MEA,
                        MORA = pt.MORA,
                        ZoneDistance = pt.ZoneDistance,
                        CumulativeDistance = pt.CumulativeDistance,
                        Wind = pt.Wind,
                        MagneticTrack = pt.MagneticTrack,
                        Temperature = pt.Temperature,
                        ZoneTime = pt.ZoneTime,
                        CumulativeTime = pt.CumulativeTime,
                        FuelRemained = pt.FuelRemained,
                        FuelUsed = pt.FuelUsed,
                        MachNo = pt.MachNo,
                        TrueAirSpeed = pt.TrueAirSpeed,
                        GroundSpeed = pt.GroundSpeed,
                        LatitudeStr = pt.LatitudeStr,
                        LongitudeStr = pt.LongitudeStr




                    });
                }


                var alt1_route = json_obj.Alternate1NavLog;
                if (alt1_route != null && alt1_route.Count > 0)
                {
                    foreach (var pt in alt1_route)
                    {
                        root.OFPB_MainNavLog.Add(new OFPB_MainNavLog()
                        {
                            NavType = "ALT1",

                            WayPoint = pt.WayPoint,
                            FlightLevel = pt.FlightLevel,
                            Latitude = pt.Latitude,
                            Longitude = pt.Longitude,
                            Frequency = pt.Frequency,
                            Airway = pt.Airway,
                            MEA = pt.MEA,
                            MORA = pt.MORA,
                            ZoneDistance = pt.ZoneDistance,
                            CumulativeDistance = pt.CumulativeDistance,
                            Wind = pt.Wind,
                            MagneticTrack = pt.MagneticTrack,
                            Temperature = pt.Temperature,
                            ZoneTime = pt.ZoneTime,
                            CumulativeTime = pt.CumulativeTime,
                            FuelRemained = pt.FuelRemained,
                            FuelUsed = pt.FuelUsed,
                            MachNo = pt.MachNo,
                            TrueAirSpeed = pt.TrueAirSpeed,
                            GroundSpeed = pt.GroundSpeed,
                            LatitudeStr = pt.LatitudeStr,
                            LongitudeStr = pt.LongitudeStr
                        });
                    }

                }

                var alt2_route = json_obj.Alternate2NavLog;
                if (alt2_route != null && alt2_route.Count > 0)
                {
                    foreach (var pt in alt2_route)
                    {
                        root.OFPB_MainNavLog.Add(new OFPB_MainNavLog()
                        {
                            NavType = "ALT2",

                            WayPoint = pt.WayPoint,
                            FlightLevel = pt.FlightLevel,
                            Latitude = pt.Latitude,
                            Longitude = pt.Longitude,
                            Frequency = pt.Frequency,
                            Airway = pt.Airway,
                            MEA = pt.MEA,
                            MORA = pt.MORA,
                            ZoneDistance = pt.ZoneDistance,
                            CumulativeDistance = pt.CumulativeDistance,
                            Wind = pt.Wind,
                            MagneticTrack = pt.MagneticTrack,
                            Temperature = pt.Temperature,
                            ZoneTime = pt.ZoneTime,
                            CumulativeTime = pt.CumulativeTime,
                            FuelRemained = pt.FuelRemained,
                            FuelUsed = pt.FuelUsed,
                            MachNo = pt.MachNo,
                            TrueAirSpeed = pt.TrueAirSpeed,
                            GroundSpeed = pt.GroundSpeed,
                            LatitudeStr = pt.LatitudeStr,
                            LongitudeStr = pt.LongitudeStr
                        });
                    }
                }

                var _name = "";
                if (json_obj.MainWindTemperature != null)
                    foreach (var x in json_obj.MainWindTemperature)
                    {
                        var _str = x.ToString();
                        Dictionary<string, string> rows = JsonConvert.DeserializeObject<Dictionary<string, string>>(_str);
                        _name = rows["Name"];
                        foreach (var kvp in rows)
                        {
                            if (kvp.Key == "Name")
                                root.OFPB_WindTemperature.Add(new OFPB_WindTemperature()
                                {
                                    Type = "MAIN",
                                    WayPoint = _name,

                                });
                            else
                                root.OFPB_WindTemperature.Add(new OFPB_WindTemperature()
                                {
                                    Type = "MAIN",
                                    WayPoint = _name,
                                    FlightLevel = kvp.Key,
                                    WindTemprature = kvp.Value,
                                });

                        }

                        if (rows.Count < 5)
                        {
                            var n = 5 - rows.Count;
                            for (int y = 1; y <= n; y++)
                                root.OFPB_WindTemperature.Add(new OFPB_WindTemperature()
                                {
                                    Type = "MAIN",
                                    WayPoint = _name,

                                });

                        }



                    }



                _name = "";
                if (json_obj.Alternate1WindTemperature != null)
                    foreach (var x in json_obj.Alternate1WindTemperature)
                    {
                        var _str = x.ToString();
                        Dictionary<string, string> rows = JsonConvert.DeserializeObject<Dictionary<string, string>>(_str);
                        _name = rows["Name"];
                        foreach (var kvp in rows)
                        {
                            if (kvp.Key == "Name")
                                root.OFPB_WindTemperature.Add(new OFPB_WindTemperature()
                                {
                                    Type = "ALT1",
                                    WayPoint = _name,

                                });
                            else
                                root.OFPB_WindTemperature.Add(new OFPB_WindTemperature()
                                {
                                    Type = "ALT1",
                                    WayPoint = _name,
                                    FlightLevel = kvp.Key,
                                    WindTemprature = kvp.Value,
                                });

                        }
                        if (rows.Count < 5)
                        {
                            var n = 5 - rows.Count;
                            for (int y = 1; y <= n; y++)
                                root.OFPB_WindTemperature.Add(new OFPB_WindTemperature()
                                {
                                    Type = "ALT1",
                                    WayPoint = _name,

                                });

                        }

                    }
                //if (json_obj.Alternate1WindTemperature != null && json_obj.Alternate1WindTemperature.Count < 5 && !string.IsNullOrEmpty(_name))
                //{
                //    var n = 5 - json_obj.Alternate1WindTemperature.Count;
                //    for (int y = 1; y <= n; y++)
                //        root.OFPB_WindTemperature.Add(new OFPB_WindTemperature()
                //        {
                //            Type = "ALT1",
                //            WayPoint = _name,

                //        });

                //}


                _name = "";
                if (json_obj.Alternate2WindTemperature != null)
                    foreach (var x in json_obj.Alternate2WindTemperature)
                    {
                        var _str = x.ToString();
                        Dictionary<string, string> rows = JsonConvert.DeserializeObject<Dictionary<string, string>>(_str);
                        _name = rows["Name"];
                        foreach (var kvp in rows)
                        {
                            if (kvp.Key == "Name")
                                root.OFPB_WindTemperature.Add(new OFPB_WindTemperature()
                                {
                                    Type = "ALT2",
                                    WayPoint = _name,

                                });
                            else
                                root.OFPB_WindTemperature.Add(new OFPB_WindTemperature()
                                {
                                    Type = "ALT2",
                                    WayPoint = _name,
                                    FlightLevel = kvp.Key,
                                    WindTemprature = kvp.Value,
                                });

                        }
                        if (rows.Count < 5)
                        {
                            var n = 5 - rows.Count;
                            for (int y = 1; y <= n; y++)
                                root.OFPB_WindTemperature.Add(new OFPB_WindTemperature()
                                {
                                    Type = "ALT2",
                                    WayPoint = _name,

                                });

                        }

                    }
                //if (json_obj.Alternate2WindTemperature != null && json_obj.Alternate2WindTemperature.Count < 5 && !string.IsNullOrEmpty(_name))
                //{
                //    var n = 5 - json_obj.Alternate2WindTemperature.Count;
                //    for (int y = 1; y <= n; y++)
                //        root.OFPB_WindTemperature.Add(new OFPB_WindTemperature()
                //        {
                //            Type = "ALT2",
                //            WayPoint = _name,

                //        });

                //}

                _name = "";
                if (json_obj.TakeOffAlternateWindTemperature != null)
                    foreach (var x in json_obj.TakeOffAlternateWindTemperature)
                    {
                        var _str = x.ToString();
                        Dictionary<string, string> rows = JsonConvert.DeserializeObject<Dictionary<string, string>>(_str);
                        _name = rows["Name"];
                        foreach (var kvp in rows)
                        {
                            if (kvp.Key == "Name")
                                root.OFPB_WindTemperature.Add(new OFPB_WindTemperature()
                                {
                                    Type = "ALTTO",
                                    WayPoint = _name,

                                });
                            else
                                root.OFPB_WindTemperature.Add(new OFPB_WindTemperature()
                                {
                                    Type = "ALTTO",
                                    WayPoint = _name,
                                    FlightLevel = kvp.Key,
                                    WindTemprature = kvp.Value,
                                });

                        }
                        if (rows.Count < 5)
                        {
                            var n = 5 - rows.Count;
                            for (int y = 1; y <= n; y++)
                                root.OFPB_WindTemperature.Add(new OFPB_WindTemperature()
                                {
                                    Type = "ALTTO",
                                    WayPoint = _name,

                                });

                        }

                    }
                //if (json_obj.TakeOffAlternateWindTemperature != null && json_obj.TakeOffAlternateWindTemperature.Count < 5 && !string.IsNullOrEmpty(_name))
                //{
                //    var n = 5 - json_obj.TakeOffAlternateWindTemperature.Count;
                //    for (int y = 1; y <= n; y++)
                //        root.OFPB_WindTemperature.Add(new OFPB_WindTemperature()
                //        {
                //            Type = "ALTTO",
                //            WayPoint = _name,

                //        });

                //}


                dto.DateUpload = DateTime.Now;
                dto.UploadStatus = 1;
                dto.UploadMessage = "OK";
                context.OFPB_Root.Add(root);
                context.SaveChanges();
                return Ok(true);



            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += ex.InnerException.Message;
                dto.UploadStatus = -1;
                dto.UploadMessage = msg;
                context.SaveChanges();
                return Ok("Not Uploaded ");
            }
        }

        public int? ofpb_time_to_int(string str)
        {
            //00.49
            if (string.IsNullOrEmpty(str))
                return 0;
            if (str.Contains('.') || str.Contains(':'))
            {
                var chr = str.Contains('.') ? '.' : ':';
                var prts = str.Split(chr);
                var hh = Convert.ToInt32(prts[0]) * 60;
                var mm = Convert.ToInt32(prts[1]);
                return hh + mm;
            }
            else
            {
                var mm = Convert.ToInt32(str);
                return mm;
            }

        }

        //https://xpi.sbvaresh.ir/api/skyputer
        [Route("api/skyputer")]
        [AcceptVerbs("POST")]
        public IHttpActionResult PostSkyputer(skyputer dto)
        {
            var result = "GNRL";
            try
            {
                if (string.IsNullOrEmpty(dto.key))
                    return Ok("Authorization key not found.");
                if (string.IsNullOrEmpty(dto.plan))
                    return Ok("Plan cannot be empty.");
                if (dto.key != "Skyputer@1359#")
                    return Ok("Authorization key is wrong.");

                if (dto.plan.Contains("FlyPersia") || dto.plan.Contains("FLYPERSIA"))
                {
                    result = "FLY";
                    var entity = new OFPSkyPuter()
                    {
                        OFP = dto.plan,
                        DateCreate = DateTime.Now,
                        UploadStatus = 0,


                    };
                    var ctx = new PPAEntities();
                    ctx.Database.CommandTimeout = 1000;
                    ctx.OFPSkyPuters.Add(entity);
                    ctx.SaveChanges();


                    string responsebody = "NO";
                    using (WebClient client = new WebClient())
                    {
                        var reqparm = new System.Collections.Specialized.NameValueCollection();
                        reqparm.Add("key", dto.key);
                        reqparm.Add("plan", dto.plan);
                        byte[] responsebytes = client.UploadValues("https://fleet.flypersiaairlines.ir/xpi/api/skyputer/flypersia", "POST", reqparm);
                        responsebody = Encoding.UTF8.GetString(responsebytes);

                    }
                    return Ok(true);
                }
                else if (dto.plan.Contains("CASPIAN"))
                {
                    result = "CASPIAN";
                    var entity = new OFPSkyPuter()
                    {
                        OFP = dto.plan,
                        DateCreate = DateTime.Now,
                        UploadStatus = 0,


                    };
                    var ctx = new PPAEntities();
                    ctx.Database.CommandTimeout = 1000;
                    ctx.OFPSkyPuters.Add(entity);
                    ctx.SaveChanges();


                    string responsebody = "NO";
                    using (WebClient client = new WebClient())
                    {
                        var reqparm = new System.Collections.Specialized.NameValueCollection();
                        reqparm.Add("key", dto.key);
                        reqparm.Add("plan", dto.plan);
                        byte[] responsebytes = client.UploadValues("https://fleet.caspianairlines.com/zxapi/api/skyputer/cpn", "POST", reqparm);
                        responsebody = Encoding.UTF8.GetString(responsebytes);

                    }
                    return Ok(true);
                }
                else if (dto.plan.Contains("CHABAHAR") || dto.plan.Contains("Chabahar"))
                {
                    result = "CHB";
                    var entity = new OFPSkyPuter()
                    {
                        OFP = dto.plan,
                        DateCreate = DateTime.Now,
                        UploadStatus = 0,


                    };
                    var ctx = new PPAEntities();
                    ctx.Database.CommandTimeout = 1000;
                    ctx.OFPSkyPuters.Add(entity);
                    ctx.SaveChanges();


                    string responsebody = "NO";
                    using (WebClient client = new WebClient())
                    {
                        var reqparm = new System.Collections.Specialized.NameValueCollection();
                        reqparm.Add("key", dto.key);
                        reqparm.Add("plan", dto.plan);
                        byte[] responsebytes = client.UploadValues(/*"https://xpi.chb.skybag.click/api/skyputer/chb"*/"https://chb.skybag.app/xpi/api/skyputer/chb", "POST", reqparm);
                        responsebody = Encoding.UTF8.GetString(responsebytes);

                    }
                    return Ok(true);
                }
                else if (dto.plan.Contains("ATLAS") || dto.plan.Contains("Atlas"))
                {
                    result = "ATLAS";
                    var entity = new OFPSkyPuter()
                    {
                        OFP = dto.plan,
                        DateCreate = DateTime.Now,
                        UploadStatus = 0,


                    };
                    var ctx = new PPAEntities();
                    ctx.Database.CommandTimeout = 1000;
                    ctx.OFPSkyPuters.Add(entity);
                    ctx.SaveChanges();


                    string responsebody = "NO";
                    using (WebClient client = new WebClient())
                    {
                        var reqparm = new System.Collections.Specialized.NameValueCollection();
                        reqparm.Add("key", dto.key);
                        reqparm.Add("plan", dto.plan);
                        byte[] responsebytes = client.UploadValues("https://xpi.atlas.skybag.click/api/skyputer/atlas", "POST", reqparm);
                        responsebody = Encoding.UTF8.GetString(responsebytes);

                    }
                    return Ok(true);
                }
                else if (dto.plan.Contains("TABAN") || dto.plan.Contains("Taban"))
                {
                    result = "TBN";
                    var entity = new OFPSkyPuter()
                    {
                        OFP = dto.plan,
                        DateCreate = DateTime.Now,
                        UploadStatus = 0,


                    };
                    var ctx = new PPAEntities();
                    ctx.Database.CommandTimeout = 1000;
                    ctx.OFPSkyPuters.Add(entity);
                    ctx.SaveChanges();


                    string responsebody = "NO";
                    using (WebClient client = new WebClient())
                    {
                        var reqparm = new System.Collections.Specialized.NameValueCollection();
                        reqparm.Add("key", dto.key);
                        reqparm.Add("plan", dto.plan);
                        byte[] responsebytes = client.UploadValues("https://taban.skybag.app/xpi/api/skyputer/tbn", "POST", reqparm);
                        responsebody = Encoding.UTF8.GetString(responsebytes);

                    }
                    return Ok(true);
                }
                else if (dto.plan.Contains("AIR1AIR"))
                {
                    result = "AIR1AIR";
                    var entity = new OFPSkyPuter()
                    {
                        OFP = dto.plan,
                        DateCreate = DateTime.Now,
                        UploadStatus = 0,


                    };
                    var ctx = new PPAEntities();
                    ctx.Database.CommandTimeout = 1000;
                    ctx.OFPSkyPuters.Add(entity);
                    ctx.SaveChanges();


                    string responsebody = "NO";
                    using (WebClient client = new WebClient())
                    {
                        var reqparm = new System.Collections.Specialized.NameValueCollection();
                        reqparm.Add("key", dto.key);
                        reqparm.Add("plan", dto.plan);
                        byte[] responsebytes = client.UploadValues("https://air1.skybag.app/xpi/api/skyputer/airone", "POST", reqparm);
                        responsebody = Encoding.UTF8.GetString(responsebytes);

                    }
                    return Ok(true);
                }
                else if (dto.plan.Contains("AVA AIR"))
                {
                    result = "AVA AIR";
                    var entity = new OFPSkyPuter()
                    {
                        OFP = dto.plan,
                        DateCreate = DateTime.Now,
                        UploadStatus = 0,


                    };
                    var ctx = new PPAEntities();
                    ctx.Database.CommandTimeout = 1000;
                    ctx.OFPSkyPuters.Add(entity);
                    ctx.SaveChanges();


                    string responsebody = "NO";
                    using (WebClient client = new WebClient())
                    {
                        var reqparm = new System.Collections.Specialized.NameValueCollection();
                        reqparm.Add("key", dto.key);
                        reqparm.Add("plan", dto.plan);
                        byte[] responsebytes = client.UploadValues("https://xpiava.skybag.app/api/skyputer/ava", "POST", reqparm);
                        responsebody = Encoding.UTF8.GetString(responsebytes);

                    }
                    return Ok(true);
                }
                //Karun
                else if (dto.plan.Contains("Karun"))
                {
                    result = "Karun";
                    var entity = new OFPSkyPuter()
                    {
                        OFP = dto.plan,
                        DateCreate = DateTime.Now,
                        UploadStatus = 0,


                    };
                    var ctx = new PPAEntities();
                    ctx.Database.CommandTimeout = 1000;
                    ctx.OFPSkyPuters.Add(entity);
                    ctx.SaveChanges();


                    string responsebody = "NO";
                    using (WebClient client = new WebClient())
                    {
                        var reqparm = new System.Collections.Specialized.NameValueCollection();
                        reqparm.Add("key", dto.key);
                        reqparm.Add("plan", dto.plan);
                        byte[] responsebytes = client.UploadValues("https://airpocket.karunair.ir/xapi/api/skyputer/karun", "POST", reqparm);
                        responsebody = Encoding.UTF8.GetString(responsebytes);

                    }
                    return Ok(true);
                }
                else if (dto.plan.Contains("RAIMON"))
                {
                    result = "RAIMON";
                    var entity = new OFPSkyPuter()
                    {
                        OFP = dto.plan,
                        DateCreate = DateTime.Now,
                        UploadStatus = 0,


                    };
                    var ctx = new PPAEntities();
                    ctx.Database.CommandTimeout = 1000;
                    ctx.OFPSkyPuters.Add(entity);
                    ctx.SaveChanges();


                    string responsebody = "NO";
                    using (WebClient client = new WebClient())
                    {
                        var reqparm = new System.Collections.Specialized.NameValueCollection();
                        reqparm.Add("key", dto.key);
                        reqparm.Add("plan", dto.plan);
                        byte[] responsebytes = client.UploadValues("https://rai.xpi.aerok.tech/api/skyputer/rai", "POST", reqparm);
                        responsebody = Encoding.UTF8.GetString(responsebytes);

                    }
                    return Ok(true);
                }
                //FLYKISH
                else if (dto.plan.Contains("FLYKISH"))
                {
                    result = "FLYKISH";
                    var entity = new OFPSkyPuter()
                    {
                        OFP = dto.plan,
                        DateCreate = DateTime.Now,
                        UploadStatus = 0,


                    };
                    var ctx = new PPAEntities();
                    ctx.Database.CommandTimeout = 1000;
                    ctx.OFPSkyPuters.Add(entity);
                    ctx.SaveChanges();


                    string responsebody = "NO";
                    using (WebClient client = new WebClient())
                    {
                        var reqparm = new System.Collections.Specialized.NameValueCollection();
                        reqparm.Add("key", dto.key);
                        reqparm.Add("plan", dto.plan);
                        byte[] responsebytes = client.UploadValues("https://fly.xpi.myaero.tech/api/skyputer/flykish", "POST", reqparm);
                        responsebody = Encoding.UTF8.GetString(responsebytes);

                    }
                    return Ok(true);
                }
                else
                {
                    var entity = new OFPSkyPuter()
                    {
                        OFP = dto.plan,
                        DateCreate = DateTime.Now,
                        UploadStatus = 0,


                    };
                    var ctx = new PPAEntities();
                    ctx.Database.CommandTimeout = 1000;
                    ctx.OFPSkyPuters.Add(entity);
                    ctx.SaveChanges();
                    new Thread(async () =>
                    {
                        GetSkyputerImport(entity.Id);
                    }).Start();
                    return Ok(true);
                }


            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += " INNER:" + ex.InnerException.Message;
                return Ok(msg);
            }

        }


        [Route("api/skyputer/flypersia")]
        [AcceptVerbs("POST")]
        public IHttpActionResult PostSkyputerFlyPersia(skyputer dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.key))
                    return Ok("Authorization key not found.");
                if (string.IsNullOrEmpty(dto.plan))
                    return Ok("Plan cannot be empty.");
                if (dto.key != "Skyputer@1359#")
                    return Ok("Authorization key is wrong.");



                var entity = new OFPSkyPuter()
                {
                    OFP = dto.plan,
                    DateCreate = DateTime.Now,
                    UploadStatus = 0,


                };
                var ctx = new PPAEntities();
                ctx.Database.CommandTimeout = 1000;
                ctx.OFPSkyPuters.Add(entity);
                ctx.SaveChanges();
                new Thread(async () =>
                {
                    GetSkyputerImport(entity.Id);
                }).Start();
                return Ok(true);



            }
            catch (Exception ex)
            {
                return Ok(true);
            }

        }

        [Route("api/skyputer/chb")]
        [AcceptVerbs("POST")]
        public IHttpActionResult PostSkyputerCHB(skyputer dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.key))
                    return Ok("Authorization key not found.");
                if (string.IsNullOrEmpty(dto.plan))
                    return Ok("Plan cannot be empty.");
                if (dto.key != "Skyputer@1359#")
                    return Ok("Authorization key is wrong.");



                var entity = new OFPSkyPuter()
                {
                    OFP = dto.plan,
                    DateCreate = DateTime.Now,
                    UploadStatus = 0,


                };
                var ctx = new PPAEntities();
                ctx.Database.CommandTimeout = 1000;
                ctx.OFPSkyPuters.Add(entity);
                ctx.SaveChanges();
                new Thread(async () =>
                {
                    GetSkyputerImport(entity.Id);
                }).Start();
                return Ok(true);



            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += " Inner: " + ex.InnerException.Message;
                return Ok(msg);
            }

        }

        [Route("api/skyputer/cpn")]
        [AcceptVerbs("POST")]
        public IHttpActionResult PostSkyputerCPN(skyputer dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.key))
                    return Ok("Authorization key not found.");
                if (string.IsNullOrEmpty(dto.plan))
                    return Ok("Plan cannot be empty.");
                if (dto.key != "Skyputer@1359#")
                    return Ok("Authorization key is wrong.");



                var entity = new OFPSkyPuter()
                {
                    OFP = dto.plan,
                    DateCreate = DateTime.Now,
                    UploadStatus = 0,


                };
                var ctx = new PPAEntities();
                ctx.Database.CommandTimeout = 1000;
                ctx.OFPSkyPuters.Add(entity);
                ctx.SaveChanges();
                new Thread(async () =>
                {
                    GetSkyputerImport(entity.Id);
                }).Start();
                return Ok(true);



            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += " Inner: " + ex.InnerException.Message;
                return Ok(msg);
            }

        }


        [Route("api/skyputer/tbn")]
        [AcceptVerbs("POST")]
        public IHttpActionResult PostSkyputerTBN(skyputer dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.key))
                    return Ok("Authorization key not found.");
                if (string.IsNullOrEmpty(dto.plan))
                    return Ok("Plan cannot be empty.");
                if (dto.key != "Skyputer@1359#")
                    return Ok("Authorization key is wrong.");



                var entity = new OFPSkyPuter()
                {
                    OFP = dto.plan,
                    DateCreate = DateTime.Now,
                    UploadStatus = 0,


                };
                var ctx = new PPAEntities();
                ctx.Database.CommandTimeout = 1000;
                ctx.OFPSkyPuters.Add(entity);
                ctx.SaveChanges();
                new Thread(async () =>
                {
                    GetSkyputerImport(entity.Id);
                }).Start();
                return Ok(true);



            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += " Inner: " + ex.InnerException.Message;
                return Ok(msg);
            }

        }
        [Route("api/skyputer/airone")]
        [AcceptVerbs("POST")]
        public IHttpActionResult PostSkyputerAIR1(skyputer dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.key))
                    return Ok("Authorization key not found.");
                if (string.IsNullOrEmpty(dto.plan))
                    return Ok("Plan cannot be empty.");
                if (dto.key != "Skyputer@1359#")
                    return Ok("Authorization key is wrong.");



                var entity = new OFPSkyPuter()
                {
                    OFP = dto.plan,
                    DateCreate = DateTime.Now,
                    UploadStatus = 0,


                };
                var ctx = new PPAEntities();
                ctx.Database.CommandTimeout = 1000;
                ctx.OFPSkyPuters.Add(entity);
                ctx.SaveChanges();
                new Thread(async () =>
                {
                    GetSkyputerImport(entity.Id);
                }).Start();
                return Ok(true);



            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += " Inner: " + ex.InnerException.Message;
                return Ok(msg);
            }

        }


        [Route("api/skyputer/ava")]
        [AcceptVerbs("POST")]
        public IHttpActionResult PostSkyputerAVA(skyputer dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.key))
                    return Ok("Authorization key not found.");
                if (string.IsNullOrEmpty(dto.plan))
                    return Ok("Plan cannot be empty.");
                if (dto.key != "Skyputer@1359#")
                    return Ok("Authorization key is wrong.");



                var entity = new OFPSkyPuter()
                {
                    OFP = dto.plan,
                    DateCreate = DateTime.Now,
                    UploadStatus = 0,


                };
                var ctx = new PPAEntities();
                ctx.Database.CommandTimeout = 1000;
                ctx.OFPSkyPuters.Add(entity);
                ctx.SaveChanges();
                new Thread(async () =>
                {
                    GetSkyputerImport(entity.Id);
                }).Start();
                return Ok(true);



            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += " Inner: " + ex.InnerException.Message;
                return Ok(msg);
            }

        }

        [Route("api/skyputer/rai")]
        [AcceptVerbs("POST")]
        public IHttpActionResult PostSkyputerRAI(skyputer dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.key))
                    return Ok("Authorization key not found.");
                if (string.IsNullOrEmpty(dto.plan))
                    return Ok("Plan cannot be empty.");
                if (dto.key != "Skyputer@1359#")
                    return Ok("Authorization key is wrong.");



                var entity = new OFPSkyPuter()
                {
                    OFP = dto.plan,
                    DateCreate = DateTime.Now,
                    UploadStatus = 0,


                };
                var ctx = new PPAEntities();
                ctx.Database.CommandTimeout = 1000;
                ctx.OFPSkyPuters.Add(entity);
                ctx.SaveChanges();
                new Thread(async () =>
                {
                    GetSkyputerImport(entity.Id);
                }).Start();
                return Ok(true);



            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += " Inner: " + ex.InnerException.Message;
                return Ok(msg);
            }

        }

        [Route("api/skyputer/flykish")]
        [AcceptVerbs("POST")]
        public IHttpActionResult PostSkyputerFLYKISH(skyputer dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.key))
                    return Ok("Authorization key not found.");
                if (string.IsNullOrEmpty(dto.plan))
                    return Ok("Plan cannot be empty.");
                if (dto.key != "Skyputer@1359#")
                    return Ok("Authorization key is wrong.");



                var entity = new OFPSkyPuter()
                {
                    OFP = dto.plan,
                    DateCreate = DateTime.Now,
                    UploadStatus = 0,


                };
                var ctx = new PPAEntities();
                ctx.Database.CommandTimeout = 1000;
                ctx.OFPSkyPuters.Add(entity);
                ctx.SaveChanges();
                new Thread(async () =>
                {
                    GetSkyputerImport(entity.Id);
                }).Start();
                return Ok(true);



            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += " Inner: " + ex.InnerException.Message;
                return Ok(msg);
            }

        }

        [Route("api/skyputer/karun")]
        [AcceptVerbs("POST")]
        public IHttpActionResult PostSkyputerKARUN(skyputer dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.key))
                    return Ok("Authorization key not found.");
                if (string.IsNullOrEmpty(dto.plan))
                    return Ok("Plan cannot be empty.");
                if (dto.key != "Skyputer@1359#")
                    return Ok("Authorization key is wrong.");



                var entity = new OFPSkyPuter()
                {
                    OFP = dto.plan,
                    DateCreate = DateTime.Now,
                    UploadStatus = 0,


                };
                var ctx = new PPAEntities();
                ctx.Database.CommandTimeout = 1000;
                ctx.OFPSkyPuters.Add(entity);
                ctx.SaveChanges();
                new Thread(async () =>
                {
                    GetOFPBImport(entity.Id);

                }).Start();
                return Ok(true);



            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += " Inner: " + ex.InnerException.Message;
                return Ok(msg);
            }

        }


        [Route("api/skyputer/atlas")]
        [AcceptVerbs("POST")]
        public IHttpActionResult PostSkyputerATLAS(skyputer dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.key))
                    return Ok("Authorization key not found.");
                if (string.IsNullOrEmpty(dto.plan))
                    return Ok("Plan cannot be empty.");
                if (dto.key != "Skyputer@1359#")
                    return Ok("Authorization key is wrong.");



                var entity = new OFPSkyPuter()
                {
                    OFP = dto.plan,
                    DateCreate = DateTime.Now,
                    UploadStatus = 0,


                };
                var ctx = new PPAEntities();
                ctx.Database.CommandTimeout = 1000;
                ctx.OFPSkyPuters.Add(entity);
                ctx.SaveChanges();
                new Thread(async () =>
                {
                    GetSkyputerImport(entity.Id);
                }).Start();
                return Ok(true);



            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += " Inner: " + ex.InnerException.Message;
                return Ok(msg);
            }

        }

        [Route("api/php")]
        [AcceptVerbs("POST")]
        public IHttpActionResult PostPHP(skyputer dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.key))
                    return Ok("Authorization key not found.");
                if (string.IsNullOrEmpty(dto.fltno))
                    return Ok("Flight No. cannot be empty.");
                if (string.IsNullOrEmpty(dto.date))
                    return Ok("Flight Date cannot be empty.");
                if (string.IsNullOrEmpty(dto.plan))
                    return Ok("Plan cannot be empty.");
                if (dto.key != "Php@2022#")
                    return Ok("Authorization key is wrong.");
                var dtparts = dto.date.Split('-').Select(q => Convert.ToInt32(q)).ToList();
                var fltDate = new DateTime(dtparts[0], dtparts[1], dtparts[2]);
                var fltno = dto.fltno.ToUpper().Replace("TBN", "").Replace(" ", "");
                var entity = new OFPSkyPuter()
                {
                    OFP = dto.plan,
                    DateCreate = DateTime.Now,
                    UploadStatus = 0,
                    FlightDate = fltDate,
                    FlightNumber = fltno,


                };
                var ctx = new PPAEntities();
                ctx.Database.CommandTimeout = 1000;
                var flight = ctx.ViewLegTimes.Where(q => q.STDDay == fltDate && q.FlightNumber == fltno).FirstOrDefault();
                if (flight == null)
                    return Ok("Flight not found.");
                if (flight.FlightStatusID == 15 || flight.FlightStatusID == 3)
                    return Ok("Flight Status is not valid.");
                entity.FlightId = flight.ID;

                ctx.OFPSkyPuters.Add(entity);
                ctx.SaveChanges();
                return Ok(true);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += " INNER: " + ex.InnerException.Message;
                return Ok(msg);
            }

        }


        public class GeoPoint
        {

            public double Degree { get; set; }
            public double Minute { get; set; }
            public double Second { get; set; }
            public double Decimal { get; set; }
            //N3525.0 E05109.1
            public static string ConvertToDecimal(string str)
            {
                var prts = str.Split(' ');
                var lat_str = prts[0].Replace("N", "");
                var long_str = prts[1].Replace("E0", "");
                //= 51° + 34'/60 + 3"/3600
                var lat_deg = Convert.ToDouble(lat_str.Substring(0, 2));
                var lat_min = Convert.ToDouble(lat_str.Substring(2, 2));
                var lat_sec = lat_str.Contains(".") ? Convert.ToDouble(lat_str.Split('.')[1]) : 0;
                var lat_dec = Math.Round(lat_deg + (lat_min * 1.0 / 60) + (lat_sec * 1.0 / 3600), 6);


                var long_deg = Convert.ToDouble(long_str.Substring(0, 2));
                var long_min = Convert.ToDouble(long_str.Substring(2, 2));
                var long_sec = lat_str.Contains(".") ? Convert.ToDouble(long_str.Split('.')[1]) : 0;
                var long_dec = long_deg + (long_min * 1.0 / 60) + (long_sec * 1.0 / 3600);

                return lat_dec.ToString() + " " + long_dec.ToString();

            }
        }


        //https://xpi.sbvaresh.ir/api/skyputer
        [Route("api/skyputer/import/{id}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetSkyputerImport(int id)
        {

            var context = new PPAEntities();




            context.Database.CommandTimeout = 5000;
            var dto = context.OFPSkyPuters.Where(q => q.Id == id).FirstOrDefault();
            if (dto == null)
                return BadRequest("not found");
            try
            {
                List<string> props = new List<string>();

                // var ofpSky = context.OFPSkyPuters.FirstOrDefault();
                var rawText = dto.OFP;
                // var mpln = rawText.Split(new string[] { "mpln:|" }, StringSplitOptions.None).ToList()[1];
                var parts = rawText.Split(new string[] { "||" }, StringSplitOptions.None).ToList();
                var atc_prt = parts.FirstOrDefault(q => q.StartsWith("icatc:|"));
                var atc = atc_prt != null ? atc_prt.Replace("icatc:|", "") : "";

                var info = parts.FirstOrDefault(q => q.StartsWith("binfo:|")).Replace("binfo:|", "");
                var infoRows = info.Split(';').ToList();
                //binfo:|OPT=VARESH AIRLINE;FLN=VAR5820;DTE=6/24/2022 12:00:00 AM;ETD=02:35;REG=;MCI=78;FLL=330;DOW=43742
                var opt = infoRows.FirstOrDefault(q => q.StartsWith("OPT")) == null ? "" : infoRows.FirstOrDefault(q => q.StartsWith("OPT")).Split('=')[1];
                var fln = infoRows.FirstOrDefault(q => q.StartsWith("FLN")) == null ? "" : infoRows.FirstOrDefault(q => q.StartsWith("FLN")).Split('=')[1];
                var dte = infoRows.FirstOrDefault(q => q.StartsWith("DTE")) == null ? "" : infoRows.FirstOrDefault(q => q.StartsWith("DTE")).Split('=')[1];
                var etd = infoRows.FirstOrDefault(q => q.StartsWith("ETD")) == null ? "" : infoRows.FirstOrDefault(q => q.StartsWith("ETD")).Split('=')[1];

                var reg = infoRows.FirstOrDefault(q => q.StartsWith("REG")) == null ? "" : infoRows.FirstOrDefault(q => q.StartsWith("REG")).Split('=')[1];
                var mci = infoRows.FirstOrDefault(q => q.StartsWith("MCI")) == null ? "" : infoRows.FirstOrDefault(q => q.StartsWith("MCI")).Split('=')[1];
                var fll = infoRows.FirstOrDefault(q => q.StartsWith("FLL")) == null ? "" : infoRows.FirstOrDefault(q => q.StartsWith("FLL")).Split('=')[1];
                var dow = infoRows.FirstOrDefault(q => q.StartsWith("DOW")) == null ? "" : infoRows.FirstOrDefault(q => q.StartsWith("DOW")).Split('=')[1];

                var rtm = infoRows.FirstOrDefault(q => q.StartsWith("RTM")) == null ? "" : infoRows.FirstOrDefault(q => q.StartsWith("RTM")).Split('=')[1];
                var rta = infoRows.FirstOrDefault(q => q.StartsWith("RTA")) == null ? "" : infoRows.FirstOrDefault(q => q.StartsWith("RTA")).Split('=')[1];
                var rtb = infoRows.FirstOrDefault(q => q.StartsWith("RTB")) == null ? "" : infoRows.FirstOrDefault(q => q.StartsWith("RTB")).Split('=')[1];
                var rtt = infoRows.FirstOrDefault(q => q.StartsWith("RTT")) == null ? "" : infoRows.FirstOrDefault(q => q.StartsWith("RTT")).Split('=')[1];
                var thm = infoRows.FirstOrDefault(q => q.StartsWith("THM")) == null ? "" : infoRows.FirstOrDefault(q => q.StartsWith("THM")).Split('=')[1];
                var unt = infoRows.FirstOrDefault(q => q.StartsWith("UNT")) == null ? "" : infoRows.FirstOrDefault(q => q.StartsWith("UNT")).Split('=')[1];
                var crw = infoRows.FirstOrDefault(q => q.StartsWith("CRW")) == null ? "" : infoRows.FirstOrDefault(q => q.StartsWith("CRW")).Split('=')[1];
                var pld = infoRows.FirstOrDefault(q => q.StartsWith("PLD")) == null ? "" : infoRows.FirstOrDefault(q => q.StartsWith("PLD")).Split('=')[1];
                var ezfw = infoRows.FirstOrDefault(q => q.StartsWith("EZFW")) == null ? "" : infoRows.FirstOrDefault(q => q.StartsWith("EZFW")).Split('=')[1];
                var etow = infoRows.FirstOrDefault(q => q.StartsWith("ETOW")) == null ? "" : infoRows.FirstOrDefault(q => q.StartsWith("ETOW")).Split('=')[1];
                var eldw = infoRows.FirstOrDefault(q => q.StartsWith("ELDW")) == null ? "" : infoRows.FirstOrDefault(q => q.StartsWith("ELDW")).Split('=')[1];
                var eta = infoRows.FirstOrDefault(q => q.StartsWith("ETA")) == null ? "" : infoRows.FirstOrDefault(q => q.StartsWith("ETA")).Split('=')[1];

                var fpf = infoRows.FirstOrDefault(q => q.StartsWith("FPF")) == null ? "" : infoRows.FirstOrDefault(q => q.StartsWith("FPF")).Split('=')[1];


                var DSP = infoRows.FirstOrDefault(q => q.StartsWith("DSP")) == null ? "" : infoRows.FirstOrDefault(q => q.StartsWith("DSP")).Split('=')[1];
                var CM1 = infoRows.FirstOrDefault(q => q.StartsWith("CM1")) == null ? "" : infoRows.FirstOrDefault(q => q.StartsWith("CM1")).Split('=')[1];
                var CM2 = infoRows.FirstOrDefault(q => q.StartsWith("CM2")) == null ? "" : infoRows.FirstOrDefault(q => q.StartsWith("CM2")).Split('=')[1];
                var MSH = infoRows.FirstOrDefault(q => q.StartsWith("MSH")) == null ? "" : infoRows.FirstOrDefault(q => q.StartsWith("MSH")).Split('=')[1];


                var MTOW = infoRows.FirstOrDefault(q => q.StartsWith("MTOW")) == null ? "" : infoRows.FirstOrDefault(q => q.StartsWith("MTOW")).Split('=')[1];
                var MLDW = infoRows.FirstOrDefault(q => q.StartsWith("MLDW")) == null ? "" : infoRows.FirstOrDefault(q => q.StartsWith("MLDW")).Split('=')[1];

                var ELDP = infoRows.FirstOrDefault(q => q.StartsWith("ELDP")) == null ? "" : infoRows.FirstOrDefault(q => q.StartsWith("ELDP")).Split('=')[1];
                var ELDS = infoRows.FirstOrDefault(q => q.StartsWith("ELDS")) == null ? "" : infoRows.FirstOrDefault(q => q.StartsWith("ELDS")).Split('=')[1];
                var ELAL = infoRows.FirstOrDefault(q => q.StartsWith("ELAL")) == null ? "" : infoRows.FirstOrDefault(q => q.StartsWith("ELAL")).Split('=')[1];
                var ELBL = infoRows.FirstOrDefault(q => q.StartsWith("ELBL")) == null ? "" : infoRows.FirstOrDefault(q => q.StartsWith("ELBL")).Split('=')[1];

                //ELDP=ELEV: 65;ELDS=ELEV: 3956;ELAL=ELEV: 5058;ELBL=;

                // var vdt= infoRows.FirstOrDefault(q => q.StartsWith/av("VDT")) == null ? "" : infoRows.FirstOrDefault(q => q.StartsWith("VDT")).Split('=')[1];
                string alt1 = "";
                string alt2 = "";
                var flightDate = DateTime.Parse(dte);
                fln = fln.Trim();
                var no = fln.Contains(" ") ? fln.Substring(4) : fln.Substring(3);
                var main_flight_no = fln.Replace(" ", "").ToUpper();
                if (no.Length == 3 && no.StartsWith("0"))
                    no = "0" + no;
                no = no.Replace(" ", "");
                if (no.StartsWith("A"))
                    no = no.Replace("A", "");
                //  var _flt_flt = context.ViewLegTimes.OrderByDescending(q => q.STD).Take(10).ToList();
                // var _ffff = context.ViewLegTimes.OrderByDescending(q => q.ID).FirstOrDefault();
                var flight = context.ViewLegTimes.Where(q => q.STDDay == flightDate && q.FlightNumber == no && q.FlightStatusID != 4).FirstOrDefault();


                //if (flight == null)
                //    return Ok("Flight Not Found");
                if (flight == null)
                    flight = context.ViewLegTimes.First();


                var fltobj = context.FlightInformations.Where(q => q.ID == flight.ID).FirstOrDefault();
                var cplan = context.OFPImports.FirstOrDefault(q => q.FlightId == flight.ID);
                if (cplan != null)
                    context.OFPImports.Remove(cplan);

                var plan = new OFPImport()
                {
                    DateCreate = DateTime.Now,
                    DateFlight = flight.STDDay,
                    FileName = "",
                    FlightNo = flight.FlightNumber,
                    Origin = flight.FromAirportICAO,
                    Destination = flight.ToAirportICAO,
                    User = "airpocket",
                    Text = rawText,



                };


                if (!string.IsNullOrEmpty(dow))
                    plan.DOW = Convert.ToDecimal(dow);
                if (!string.IsNullOrEmpty(fll))
                    plan.FLL = Convert.ToDecimal(fll);
                if (!string.IsNullOrEmpty(mci))
                {
                    if (mci == "ETAS")
                        plan.MCI = -1;
                    else
                        plan.MCI = Convert.ToDecimal(mci);
                }




                if (!string.IsNullOrEmpty(rtm))
                    plan.RTM = rtm;
                //var rta = infoRows.FirstOrDefault(q => q.StartsWith("RTA")).Split('=')[1];
                if (!string.IsNullOrEmpty(rta))
                    plan.RTA = rta;
                //var rtb = infoRows.FirstOrDefault(q => q.StartsWith("RTB")).Split('=')[1];
                if (!string.IsNullOrEmpty(rtb))
                    plan.RTB = rtb;
                // var rtt = infoRows.FirstOrDefault(q => q.StartsWith("RTT")).Split('=')[1];
                if (!string.IsNullOrEmpty(rtt))
                    plan.RTT = rtt;
                // var thm = infoRows.FirstOrDefault(q => q.StartsWith("THM")).Split('=')[1];
                if (!string.IsNullOrEmpty(thm))
                    plan.THM = thm;
                // var unt = infoRows.FirstOrDefault(q => q.StartsWith("UNT")).Split('=')[1];
                if (!string.IsNullOrEmpty(unt))
                    plan.UNT = unt;
                //var crw = infoRows.FirstOrDefault(q => q.StartsWith("CRW")).Split('=')[1];
                if (!string.IsNullOrEmpty(crw))
                    plan.CRW = crw;
                //    var pld = infoRows.FirstOrDefault(q => q.StartsWith("PLD")).Split('=')[1];
                if (!string.IsNullOrEmpty(pld))
                    plan.PLD = pld;
                // var ezfw = infoRows.FirstOrDefault(q => q.StartsWith("EZFW")).Split('=')[1];
                if (!string.IsNullOrEmpty(ezfw))
                    plan.EZFW = ezfw;
                // var etow = infoRows.FirstOrDefault(q => q.StartsWith("ETOW")).Split('=')[1];
                if (!string.IsNullOrEmpty(etow))
                    plan.ETOW = etow;
                //  var eldw = infoRows.FirstOrDefault(q => q.StartsWith("ELDW")).Split('=')[1];
                if (!string.IsNullOrEmpty(eldw))
                    plan.ELDW = eldw;
                // var eta = infoRows.FirstOrDefault(q => q.StartsWith("ETA")).Split('=')[1];
                if (!string.IsNullOrEmpty(eta))
                    plan.ETA = eta;

                if (!string.IsNullOrEmpty(etd))
                    plan.ETD = etd;

                if (!string.IsNullOrEmpty(fpf))
                    plan.FPF = fpf;

                if (!string.IsNullOrEmpty(MSH))
                    plan.MSH = MSH;
                if (!string.IsNullOrEmpty(CM1))
                    plan.CM1 = CM1;
                if (!string.IsNullOrEmpty(CM2))
                    plan.CM2 = CM2;
                if (!string.IsNullOrEmpty(DSP))
                    plan.DSPNAME = DSP;

                if (!string.IsNullOrEmpty(atc))
                    plan.ATC = atc;

                if (!string.IsNullOrEmpty(MTOW))
                    plan.MTOW = MTOW;
                if (!string.IsNullOrEmpty(MLDW))
                    plan.MLDW = MLDW;
                if (!string.IsNullOrEmpty(ELDP))
                    plan.ELDP = ELDP;
                if (!string.IsNullOrEmpty(ELDS))
                    plan.ELDS = ELDS;
                if (!string.IsNullOrEmpty(ELAL))
                    plan.ELAL = ELAL;
                if (!string.IsNullOrEmpty(ELBL))
                    plan.ELBL = ELBL;


                plan.Source = "SkyPuter";

                if (flight != null)
                    plan.FlightId = flight.ID;
                context.OFPImports.Add(plan);

                var mpln = parts.FirstOrDefault(q => q.StartsWith("mpln:|")).Replace("mpln:|", "");
                var mplnRows = mpln.Split('|').ToList();
                //WAP=OIKB;COR=N27° 13' 06" ,  E056;FRE= ;VIA=ASMU1A;ALT=CLB;MEA=0;GMR=131;DIS=0;TDS=0;WID=;TRK=;TMP=;TME=00:00:00.0000000;TTM=00:00:00.0000000;FRE=-200;FUS=200;TAS=361;GSP=0
                List<JObject> mplnpJson = new List<JObject>();
                List<string> mainPlanPoints = new List<string>();
                List<OFPPoint> mplan_points = new List<OFPPoint>();
                var idx = 0;
                foreach (var r in mplnRows)
                {
                    var procStr = "";
                    var _r = r.Replace("=;", "= ;");
                    var prts = _r.Split(';');
                    //  mainPlanPoints.Add(GeoPoint.ConvertToDecimal(prts[1].Replace("GEO=", "")));
                    var _pnt = new OFPPoint();
                    try
                    {
                        var latlang = GeoPoint.ConvertToDecimal(prts[1].Replace("GEO=", ""));
                        _pnt.Lat = Convert.ToDecimal(latlang.Split(' ')[0]);
                        _pnt.Long = Convert.ToDecimal(latlang.Split(' ')[1]);


                        var _wap = prts.FirstOrDefault(q => q.StartsWith("WAP"));
                        _pnt.WAP = _wap == null ? "" : _wap.Split('=')[1];


                        var _geo = prts.FirstOrDefault(q => q.StartsWith("GEO"));



                        var _frq = prts.FirstOrDefault(q => q.StartsWith("FRQ"));
                        _pnt.FRQ = _frq == null ? "" : _frq.Split('=')[1];

                        var _via = prts.FirstOrDefault(q => q.StartsWith("VIA"));
                        _pnt.VIA = _via == null ? "" : _via.Split('=')[1];
                        var _alt = prts.FirstOrDefault(q => q.StartsWith("ALT"));
                        _pnt.ALT = _alt == null ? "" : _alt.Split('=')[1];
                        var _mea = prts.FirstOrDefault(q => q.StartsWith("MEA"));
                        _pnt.MEA = _mea == null ? "" : _mea.Split('=')[1];
                        var _gmr = prts.FirstOrDefault(q => q.StartsWith("GMR"));
                        _pnt.GMR = _gmr == null ? "" : _gmr.Split('=')[1];
                        var _dis = prts.FirstOrDefault(q => q.StartsWith("DIS"));
                        _pnt.DIS = _dis == null ? "" : _dis.Split('=')[1];
                        var _tds = prts.FirstOrDefault(q => q.StartsWith("TDS"));
                        _pnt.TDS = _tds == null ? "" : _tds.Split('=')[1];
                        var _wind = prts.FirstOrDefault(q => q.StartsWith("WID"));
                        _pnt.WIND = _wind == null ? "" : _wind.Split('=')[1];
                        var _trk = prts.FirstOrDefault(q => q.StartsWith("TRK"));
                        _pnt.WAP = _wap == null ? "" : _wap.Split('=')[1];
                        var _tmp = prts.FirstOrDefault(q => q.StartsWith("TMP"));
                        _pnt.TMP = _tmp == null ? "" : _tmp.Split('=')[1];
                        var _fre = prts.FirstOrDefault(q => q.StartsWith("FRE"));
                        _pnt.FRE = _fre == null ? "" : _fre.Split('=')[1];
                        var _fus = prts.FirstOrDefault(q => q.StartsWith("FUS"));
                        _pnt.FUS = _fus == null ? "" : _fus.Split('=')[1];
                        var _tas = prts.FirstOrDefault(q => q.StartsWith("TAS"));
                        _pnt.TAS = _tas == null ? "" : _tas.Split('=')[1];
                        var _gsp = prts.FirstOrDefault(q => q.StartsWith("GSP"));
                        _pnt.GSP = _gsp == null ? "" : _gsp.Split('=')[1];

                        var _tme = prts.FirstOrDefault(q => q.StartsWith("TME"));
                        if (_tme != null)
                        {
                            var _tme_p = _tme.Split('=')[1].Substring(0, 5).Split(':').Select(q => Convert.ToInt32(q)).ToList();
                            _pnt.TME = _tme_p[0] * 60 + _tme_p[1];
                        }
                        else
                            _pnt.TME = 0;

                        var _ttm = prts.FirstOrDefault(q => q.StartsWith("TTM"));
                        if (_ttm != null)
                        {
                            var _ttm_p = _ttm.Split('=')[1].Substring(0, 5).Split(':').Select(q => Convert.ToInt32(q)).ToList();
                            _pnt.TTM = _ttm_p[0] * 60 + _ttm_p[1];
                        }
                        else
                            _pnt.TTM = 0;

                        _pnt.Plan = "MAIN";
                    }
                    catch (Exception ex)
                    {

                    }
                    mplan_points.Add(_pnt);

                    foreach (var x in prts)
                    {
                        var str = x.Replace("\"", "^").Replace("'", "#").Replace("GEO", "COR");
                        var substr = str.Split('=')[0] + ":'" + str.Split('=')[1] + "'";

                        procStr += substr;
                        if (x != prts.Last())
                            procStr += ",";
                    }
                    procStr = "{" + procStr + "}";

                    var jsonObj = JsonConvert.DeserializeObject<JObject>(procStr);
                    var _key = ("mpln_WAP_" + jsonObj.GetValue("WAP").ToString()).Replace(" ", "").ToLower();
                    jsonObj.Add("_key", _key);
                    props.Add("prop_" + _key + "_eta_" + idx);
                    props.Add("prop_" + _key + "_ata_" + idx);
                    props.Add("prop_" + _key + "_rem_" + idx);
                    props.Add("prop_" + _key + "_usd_" + idx);
                    mplnpJson.Add(jsonObj);
                    idx++;

                }
                //   var _r0 ="{"+ mplnRows.First().Replace("=;", ":''").Replace("= ;", ":''").Replace("=", ":").Replace(";", ",")+"}";
                // var jsonObj = JsonConvert.DeserializeObject<JObject>(_r0);


                var apln1 = parts.FirstOrDefault(q => q.StartsWith("apln:|"));
                if (apln1 != null)
                {
                    apln1 = apln1.Replace("apln:|", "");
                    var apln1Rows = apln1.Split('|').ToList();
                    List<JObject> apln1Json = new List<JObject>();
                    idx = 0;
                    foreach (var r in apln1Rows)
                    {
                        var procStr = "";
                        var _r = r.Replace("=;", "= ;");
                        var prts = _r.Split(';');
                        foreach (var x in prts)
                        {
                            // var str = x.Replace("\"", "^").Replace("'", "#");
                            var str = x.Replace("\"", "^").Replace("'", "#").Replace("GEO", "COR");
                            var substr = str.Split('=')[0] + ":'" + str.Split('=')[1] + "'";

                            procStr += substr;
                            if (x != prts.Last())
                                procStr += ",";
                        }
                        procStr = "{" + procStr + "}";

                        var jsonObj = JsonConvert.DeserializeObject<JObject>(procStr);
                        var _key = ("apln_WAP_" + jsonObj.GetValue("WAP").ToString()).Replace(" ", "").ToLower();
                        jsonObj.Add("_key", _key);
                        props.Add("prop_" + _key + "_a1eta_" + idx);
                        props.Add("prop_" + _key + "_a1ata_" + idx);
                        props.Add("prop_" + _key + "_a1rem_" + idx);
                        props.Add("prop_" + _key + "_a1usd_" + idx);
                        apln1Json.Add(jsonObj);
                        if (r == apln1Rows.Last())
                            alt1 = jsonObj.GetValue("WAP").ToString().Replace(" ", "").ToUpper();
                        idx++;

                    }
                    //FUCK
                    plan.JAPlan1 = "[" + string.Join(",", apln1Json) + "]";

                }

                string apln2 = parts.Where(q => q.StartsWith("apln:|")).Count() > 1 ? parts.Where(q => q.StartsWith("apln:|")).ToList()[1] : null;
                if (string.IsNullOrEmpty(apln2))
                    //bpln
                    apln2 = parts.FirstOrDefault(q => q.StartsWith("bpln:|"));
                if (apln2 != null)
                {
                    apln2 = apln2.Replace("apln:|", "").Replace("bpln:|", "");
                    var apln2Rows = apln2.Split('|').ToList();
                    List<JObject> apln2Json = new List<JObject>();
                    idx = 0;
                    foreach (var r in apln2Rows)
                    {
                        var procStr = "";
                        var _r = r.Replace("=;", "= ;");
                        var prts = _r.Split(';');
                        foreach (var x in prts)
                        {
                            // var str = x.Replace("\"", "^").Replace("'", "#");
                            var str = x.Replace("\"", "^").Replace("'", "#").Replace("GEO", "COR");
                            var substr = str.Split('=')[0] + ":'" + str.Split('=')[1] + "'";

                            procStr += substr;
                            if (x != prts.Last())
                                procStr += ",";
                        }
                        procStr = "{" + procStr + "}";

                        var jsonObj = JsonConvert.DeserializeObject<JObject>(procStr);
                        var _key = ("apln_WAP_" + jsonObj.GetValue("WAP").ToString()).Replace(" ", "").ToLower();
                        jsonObj.Add("_key", _key);
                        props.Add("prop_" + _key + "_a2eta_" + idx);
                        props.Add("prop_" + _key + "_a2ata_" + idx);
                        props.Add("prop_" + _key + "_a2rem_" + idx);
                        props.Add("prop_" + _key + "_a2usd_" + idx);
                        apln2Json.Add(jsonObj);
                        if (r == apln2Rows.Last())
                            alt2 = jsonObj.GetValue("WAP").ToString().Replace(" ", "").ToUpper();
                        idx++;

                    }

                    plan.JAPlan2 = "[" + string.Join(",", apln2Json) + "]";
                    //FUCK
                }


                var cstbl = parts.FirstOrDefault(q => q.StartsWith("cstbl:|"));
                if (cstbl != null)
                {
                    cstbl = cstbl.Replace("cstbl:|", "");
                    var cstblRows = cstbl.Split('|').ToList();
                    List<JObject> cstblJson = new List<JObject>();
                    idx = 0;
                    foreach (var r in cstblRows)
                    {
                        var procStr = "";
                        var _r = r.Replace("=;", "= ;");
                        var prts = _r.Split(';');
                        foreach (var x in prts)
                        {
                            var str = x.Replace("\"", "^").Replace("'", "#");
                            var substr = str.Split('=')[0] + ":'" + str.Split('=')[1] + "'";

                            procStr += substr;
                            if (x != prts.Last())
                                procStr += ",";
                        }
                        procStr = "{" + procStr + "}";

                        var jsonObj = JsonConvert.DeserializeObject<JObject>(procStr);
                        var _key = ("cstbl_ETN_" + jsonObj.GetValue("ETN").ToString()).Replace(" ", "").ToLower();
                        jsonObj.Add("_key", _key);
                        // props.Add("prop_" + _key + "_eta_" + idx);
                        // props.Add("prop_" + _key + "_ata_" + idx);
                        // props.Add("prop_" + _key + "_rem_" + idx);
                        // props.Add("prop_" + _key + "_usd_" + idx);
                        cstblJson.Add(jsonObj);
                        idx++;

                    }
                    plan.JCSTBL = "[" + string.Join(",", cstblJson) + "]";
                    //FUCK
                }

                var aldrf = parts.FirstOrDefault(q => q.StartsWith("aldrf:|"));
                if (aldrf != null)
                {
                    aldrf = aldrf.Replace("aldrf:|", "");
                    var aldrfRows = aldrf.Split('/').Where(q => !string.IsNullOrEmpty(q)).ToList();
                    List<JObject> aldrfJson = new List<JObject>();
                    idx = 0;

                    foreach (var r in aldrfRows)
                    {
                        var procStr = "";
                        var _r = r.Replace("=;", "= ;");
                        var prts = _r.Split(new string[] { "  " }, StringSplitOptions.None).Where(q => !string.IsNullOrEmpty(q)).ToList();
                        //  var prts2 = _r.Split(new string[] { " " }, StringSplitOptions.None);
                        //foreach (var x in prts)
                        //{
                        //    var str = x.Replace("\"", "^").Replace("'", "#");
                        //    var substr = str.Split('=')[0] + ":'" + str.Split('=')[1] + "'";

                        //    procStr += substr;
                        //    if (x != prts.Last())
                        //        procStr += ",";
                        //}
                        procStr += "FL:'" + prts[0].Replace(" ", "").Replace("|", "") + "'";
                        procStr += ",WIND:'" + prts[1].Replace(" ", "") + "'";
                        procStr += ",FUEL:'" + prts[2].Replace(" ", "") + "'";
                        procStr += ",T:'" + prts[3].Replace(" ", "") + "'";
                        procStr += ",SH1:'" + prts[4].Replace(" ", "") + "'";
                        procStr += ",SH2:'" + prts[5].Replace(" ", "") + "'";
                        procStr += ",DEV:'" + prts[6].Replace(" ", "") + "'";
                        procStr = "{" + procStr + "}";

                        var jsonObj = JsonConvert.DeserializeObject<JObject>(procStr);
                        var _key = ("aldrf_FL_" + jsonObj.GetValue("FL").ToString()).Replace(" ", "").ToLower();
                        jsonObj.Add("_key", _key);

                        aldrfJson.Add(jsonObj);
                        idx++;

                    }
                    plan.JALDRF = "[" + string.Join(",", aldrfJson) + "]";
                    //FUCK
                }

                var wtdrf = parts.FirstOrDefault(q => q.StartsWith("wtdrf:|"));
                if (wtdrf != null)
                {
                    wtdrf = wtdrf.Replace("wtdrf:|", "");
                    var aldrfRows = wtdrf.Split(new string[] { "  " }, StringSplitOptions.None).Where(q => !string.IsNullOrEmpty(q)).ToList();
                    List<JObject> wtdrfJson = new List<JObject>();
                    idx = 0;
                    try
                    {
                        var procStr = "{IDX:'1',X:'-8',FUEL:'" + aldrfRows[1].Replace(" ", "") + "'}";
                        var jsonObj = JsonConvert.DeserializeObject<JObject>(procStr);
                        var _key = ("wtdrf_IDX_" + jsonObj.GetValue("IDX").ToString()).Replace(" ", "").ToLower();
                        jsonObj.Add("_key", _key);
                        wtdrfJson.Add(jsonObj);


                        procStr = "{IDX:'2',X:'-6',FUEL:'" + aldrfRows[3].Replace(" ", "") + "'}";
                        jsonObj = JsonConvert.DeserializeObject<JObject>(procStr);
                        _key = ("wtdrf_IDX_" + jsonObj.GetValue("IDX").ToString()).Replace(" ", "").ToLower();
                        jsonObj.Add("_key", _key);
                        wtdrfJson.Add(jsonObj);

                        procStr = "{IDX:'3',X:'-4',FUEL:'" + aldrfRows[5].Replace(" ", "") + "'}";
                        jsonObj = JsonConvert.DeserializeObject<JObject>(procStr);
                        _key = ("wtdrf_IDX_" + jsonObj.GetValue("IDX").ToString()).Replace(" ", "").ToLower();
                        jsonObj.Add("_key", _key);
                        wtdrfJson.Add(jsonObj);


                        procStr = "{IDX:'4',X:'-2',FUEL:'" + aldrfRows[7].Replace(" ", "") + "'}";
                        jsonObj = JsonConvert.DeserializeObject<JObject>(procStr);
                        _key = ("wtdrf_IDX_" + jsonObj.GetValue("IDX").ToString()).Replace(" ", "").ToLower();
                        jsonObj.Add("_key", _key);
                        wtdrfJson.Add(jsonObj);

                        procStr = "{IDX:'5',X:'+2',FUEL:'" + aldrfRows[9].Replace(" ", "") + "'}";
                        jsonObj = JsonConvert.DeserializeObject<JObject>(procStr);
                        _key = ("wtdrf_IDX_" + jsonObj.GetValue("IDX").ToString()).Replace(" ", "").ToLower();
                        jsonObj.Add("_key", _key);
                        wtdrfJson.Add(jsonObj);


                        procStr = "{IDX:'6',X:'+4',FUEL:'" + aldrfRows[11].Replace(" ", "") + "'}";
                        jsonObj = JsonConvert.DeserializeObject<JObject>(procStr);
                        _key = ("wtdrf_IDX_" + jsonObj.GetValue("IDX").ToString()).Replace(" ", "").ToLower();
                        jsonObj.Add("_key", _key);
                        wtdrfJson.Add(jsonObj);


                        procStr = "{IDX:'7',X:'+6',FUEL:'" + aldrfRows[13].Replace(" ", "") + "'}";
                        jsonObj = JsonConvert.DeserializeObject<JObject>(procStr);
                        _key = ("wtdrf_IDX_" + jsonObj.GetValue("IDX").ToString()).Replace(" ", "").ToLower();
                        jsonObj.Add("_key", _key);
                        wtdrfJson.Add(jsonObj);

                        procStr = "{IDX:'8',X:'+8',FUEL:'" + aldrfRows[15].Replace(" ", "") + "'}";
                        jsonObj = JsonConvert.DeserializeObject<JObject>(procStr);
                        _key = ("wtdrf_IDX_" + jsonObj.GetValue("IDX").ToString()).Replace(" ", "").ToLower();
                        jsonObj.Add("_key", _key);
                        wtdrfJson.Add(jsonObj);
                    }
                    catch (Exception ex) { }

                    plan.JWTDRF = "[" + string.Join(",", wtdrfJson) + "]";
                    //FUCK

                }


                var futbl = parts.FirstOrDefault(q => q.StartsWith("futbl:|")).Replace("futbl:|", "");
                var prmParts = futbl.Split('|');
                var fuel = new List<fuelPrm>();
                idx = 0;
                foreach (var x in prmParts)
                {
                    var _prts = x.Split(';');
                    //PRM=TRIP FUEL;TIM=17:26:00.00000;VAL=99960|
                    var prm = _prts[0].Split('=')[1];
                    var tim = _prts[1].Split('=')[1];
                    var val = _prts[2].Split('=')[1];
                    var _key = "fuel_" + (prm != "CONT[5%]" ? prm.Replace(" ", "").ToLower() : "cont05");
                    fuel.Add(new fuelPrm()
                    {
                        prm = prm,
                        time = tim,
                        value = val,
                        _key = _key,
                    });

                    if (prm == "TRIP FUEL")
                    {
                        plan.FPTripFuel = Convert.ToDecimal(val);
                        fltobj.OFPTRIPFUEL = Convert.ToInt32(plan.FPTripFuel);
                    }
                    if (prm == "CONT[5%]" || prm.StartsWith("CONT["))
                    {
                        plan.FuelCONT = Convert.ToInt32(val);
                        fltobj.OFPCONTFUEL = Convert.ToInt32(val);
                    }

                    if (prm == "ALT 1" || prm == "ALTN 1")
                    {
                        plan.FuelALT1 = Convert.ToInt32(val);
                        fltobj.OFPALT1FUEL = Convert.ToInt32(val);

                    }
                    if (prm == "ALT 2" || prm == "ALTN 2")
                    {
                        plan.FuelALT2 = Convert.ToInt32(val);
                        fltobj.OFPALT2FUEL = Convert.ToInt32(val);
                    }
                    if (prm == "FINAL RES")
                    {
                        plan.FuelFINALRES = Convert.ToInt32(val);
                        fltobj.OFPFINALRESFUEL = Convert.ToInt32(val);
                    }

                    if (prm == "ETOPS/ADDNL")
                    {
                        plan.FuelETOPSADDNL = Convert.ToInt32(val);
                        fltobj.OFPETOPSADDNLFUEL = Convert.ToInt32(val);
                    }

                    if (prm == "OPS.EXTRA")
                    {
                        plan.FuelOPSEXTRA = Convert.ToInt32(val);
                        fltobj.OFPOPSEXTRAFUEL = Convert.ToInt32(val);
                    }

                    if (prm == "MIN TOF")
                    {
                        plan.FuelMINTOF = Convert.ToInt32(val);
                        fltobj.OFPMINTOFFUEL = Convert.ToInt32(val);
                    }

                    if (prm == "TANKERING")
                    {
                        plan.FuelTANKERING = Convert.ToInt32(val);
                        fltobj.OFPTANKERINGFUEL = Convert.ToInt32(val);

                        plan.FuelACTUALTANKERING = Convert.ToInt32(val);
                        fltobj.ACTUALTANKERINGFUEL = Convert.ToInt32(val);
                    }

                    if (prm == "TAXI")
                    {
                        plan.FuelTAXI = Convert.ToInt32(val);
                        fltobj.OFPTAXIFUEL = Convert.ToInt32(val);
                    }

                    if (prm == "TOTAL FUEL")
                    {
                        plan.FuelTOTALFUEL = Convert.ToInt32(val);
                        fltobj.OFPTOTALFUEL = Convert.ToInt32(val);
                    }

                    if (prm == "TOF")
                    {
                        plan.FuelTOF = Convert.ToInt32(val);
                        //  fltobj.FPFuel = Convert.ToDecimal(val);
                    }





                    if (prm == "OFF BLK")
                    {
                        plan.FuelOFFBLOCK = Convert.ToInt32(val);
                        fltobj.OFPOFFBLOCKFUEL = Convert.ToInt32(val);
                    }

                    if (prm == "EXTRA")
                    {
                        plan.FuelExtra = Convert.ToInt32(val);
                        fltobj.OFPExtra = Convert.ToInt32(val);

                    }



                    props.Add("prop_" + _key);

                    idx++;
                }

                var mindivalt1 = plan.FuelALT1 + plan.FuelFINALRES;
                if (plan.FuelALT2 != null && plan.FuelALT2 > 0)
                {
                    var mindivalt2 = plan.FuelALT2 + plan.FuelFINALRES;
                    if (mindivalt1 >= mindivalt2)
                        plan.MINDIVFUEL = "[" + alt1 + ", " + mindivalt1 + "]" + "     " + "[" + plan.ALT2 + ", " + mindivalt2 + "]";
                    else
                        plan.MINDIVFUEL = "[" + alt2 + ", " + mindivalt2 + "]" + "     " + "[" + plan.ALT1 + ", " + mindivalt1 + "]";
                }
                else
                {
                    plan.MINDIVFUEL = "[" + alt1 + ", " + mindivalt1 + "]";
                }

                fuel.Add(new fuelPrm()
                {
                    prm = "REQ",
                    _key = "fuel_" + "req"
                }); ;
                props.Add("prop_fuel_" + "req");



                var did = parts.FirstOrDefault(q => q.ToUpper().Contains("DID"));
                var vdt = parts.FirstOrDefault(q => q.ToUpper().Contains("VDT"));
                if (did != null)
                {
                    plan.DID = did.Split('=')[1];
                }

                if (vdt != null)
                {
                    plan.VDT = vdt.Split('=')[1];
                }

                var wdtmp = parts.FirstOrDefault(q => q.StartsWith("wdtmp:|"));
                if (wdtmp != null)
                {
                    try
                    {
                        wdtmp = wdtmp.Replace("wdtmp:|", "");
                        var wdtmp_parts = wdtmp.Split('|');
                        var wdtmp_a = wdtmp_parts[0].Replace("/", "_").Substring(0, wdtmp_parts[0].Length - 1);
                        var wdtmp_b_parts = wdtmp_parts[1].Split('F').Where(q => !string.IsNullOrEmpty(q.Replace(" ", ""))).Select(q => "F" + q).ToList();
                        int _gwsize = Convert.ToInt32((wdtmp_b_parts.Count / wdtmp_a.Split('_').Count()));
                        var _gindex = 0;
                        var grpgw = wdtmp_b_parts.GroupBy(x => _gindex++ / _gwsize).Select(q => string.Join("_", q)).ToList();
                        var wdtmp_b = string.Join("*", grpgw);
                        plan.WDTMP = wdtmp_a + "^" + wdtmp_b;
                    }
                    catch (Exception _wdex)
                    {

                    }

                }


                var wdclb = parts.FirstOrDefault(q => q.StartsWith("wdclb:|"));
                var wddes = parts.FirstOrDefault(q => q.StartsWith("wddes:|"));
                if (wdclb != null)
                {
                    try
                    {
                        wdclb = wdclb.Replace("wdclb:|", "");
                        var wdclb_parts = wdclb.Split('F').Where(q => !string.IsNullOrEmpty(q.Replace(" ", ""))).Select(q => "F" + q).ToList();
                        plan.WDCLB = string.Join("*", wdclb_parts);
                    }
                    catch (Exception _wdex)
                    {

                    }

                }


                if (wddes != null)
                {
                    try
                    {
                        wddes = wddes.Replace("wddes:|", "");
                        var wddes_parts = wddes.Split('F').Where(q => !string.IsNullOrEmpty(q.Replace(" ", ""))).Select(q => "F" + q).ToList();
                        plan.WDDES = string.Join("*", wddes_parts);
                    }
                    catch (Exception _wdex)
                    {

                    }

                }


                var other = new List<fuelPrm>();

                other.Add(new fuelPrm() { prm = "FPF", value = fpf });
                props.Add("prop_fpf");

                other.Add(new fuelPrm() { prm = "MACH", value = mci });
                props.Add("prop_mach");
                other.Add(new fuelPrm() { prm = "FL", value = fll });
                props.Add("prop_fl");
                other.Add(new fuelPrm() { prm = "DOW", value = dow });
                props.Add("prop_dow");
                other.Add(new fuelPrm() { prm = "PLD", value = pld });
                props.Add("prop_pld");
                other.Add(new fuelPrm() { prm = "EZFW", value = ezfw });
                props.Add("prop_ezfw");

                other.Add(new fuelPrm() { prm = "ETOW", value = etow });
                props.Add("prop_etow");

                other.Add(new fuelPrm() { prm = "ELDW", value = eldw });
                props.Add("prop_eldw");

                other.Add(new fuelPrm() { prm = "CREW1", value = crw.Contains("-") ? crw.Split('-')[0] : "0" });
                props.Add("prop_crew1");
                other.Add(new fuelPrm() { prm = "CREW2", value = crw.Contains("-") ? crw.Split('-')[1] : "0" });
                props.Add("prop_crew2");

                other.Add(new fuelPrm() { prm = "CREW3", value = "" });
                props.Add("prop_crew3");

                other.Add(new fuelPrm() { prm = "CREW4", value = "" });
                props.Add("prop_crew4");

                other.Add(new fuelPrm() { prm = "CREW5", value = "" });
                props.Add("prop_crew5");




                other.Add(new fuelPrm() { prm = "ETD", value = etd });
                props.Add("prop_etd");

                other.Add(new fuelPrm() { prm = "ETA", value = eta });
                props.Add("prop_eta");

                other.Add(new fuelPrm() { prm = "RTM", value = rtm });
                props.Add("prop_rtm");

                other.Add(new fuelPrm() { prm = "RTA", value = rta });
                props.Add("prop_rta");
                other.Add(new fuelPrm() { prm = "RTB", value = rtb });
                props.Add("prop_rtb");
                other.Add(new fuelPrm() { prm = "RTT", value = rtt });
                props.Add("prop_rtt");

                other.Add(new fuelPrm() { prm = "THM", value = thm });
                props.Add("prop_thm");

                other.Add(new fuelPrm() { prm = "UNT", value = unt });
                props.Add("prop_unt");

                other.Add(new fuelPrm() { prm = "CRW", value = crw });
                props.Add("prop_crw");

                other.Add(new fuelPrm() { prm = "PAX_ADULT", value = "" });
                props.Add("prop_pax_adult");
                other.Add(new fuelPrm() { prm = "PAX_CHILD", value = "" });
                props.Add("prop_pax_child");
                other.Add(new fuelPrm() { prm = "PAX_INFANT", value = "" });
                props.Add("prop_pax_infant");

                other.Add(new fuelPrm() { prm = "PAX_MALE", value = "" });
                props.Add("prop_pax_male");

                other.Add(new fuelPrm() { prm = "PAX_FEMALE", value = "" });
                props.Add("prop_pax_female");



                other.Add(new fuelPrm() { prm = "SOB", value = "" });
                props.Add("prop_sob");
                other.Add(new fuelPrm() { prm = "CLEARANCE", value = "" });
                props.Add("prop_clearance");

                other.Add(new fuelPrm() { prm = "ATIS1", value = "" });
                props.Add("prop_atis1");
                other.Add(new fuelPrm() { prm = "ATIS2", value = "" });
                props.Add("prop_atis2");
                other.Add(new fuelPrm() { prm = "ATIS3", value = "" });
                props.Add("prop_atis3");
                other.Add(new fuelPrm() { prm = "ATIS4", value = "" });
                props.Add("prop_atis4");

                other.Add(new fuelPrm() { prm = "RVSM_FLT_L", value = "" });
                props.Add("prop_rvsm_flt_l");
                other.Add(new fuelPrm() { prm = "RVSM_FLT_STBY", value = "" });
                props.Add("prop_rvsm_flt_stby");
                other.Add(new fuelPrm() { prm = "RVSM_FLT_R", value = "" });
                props.Add("prop_rvsm_flt_r");
                other.Add(new fuelPrm() { prm = "RVSM_FLT_TIME", value = "" });
                props.Add("prop_rvsm_flt_time");


                other.Add(new fuelPrm() { prm = "RVSM_FLT_L2", value = "" });
                props.Add("prop_rvsm_flt_l2");
                other.Add(new fuelPrm() { prm = "RVSM_FLT_STBY2", value = "" });
                props.Add("prop_rvsm_flt_stby2");
                other.Add(new fuelPrm() { prm = "RVSM_FLT_R2", value = "" });
                props.Add("prop_rvsm_flt_r2");
                other.Add(new fuelPrm() { prm = "RVSM_FLT_TIME2", value = "" });
                props.Add("prop_rvsm_flt_time2");

                other.Add(new fuelPrm() { prm = "RVSM_FLT_L3", value = "" });
                props.Add("prop_rvsm_flt_l3");
                other.Add(new fuelPrm() { prm = "RVSM_FLT_STBY3", value = "" });
                props.Add("prop_rvsm_flt_stby3");
                other.Add(new fuelPrm() { prm = "RVSM_FLT_R3", value = "" });
                props.Add("prop_rvsm_flt_r3");
                other.Add(new fuelPrm() { prm = "RVSM_FLT_TIME3", value = "" });
                props.Add("prop_rvsm_flt_time3");


                other.Add(new fuelPrm() { prm = "RVSM_FLT_L4", value = "" });
                props.Add("prop_rvsm_flt_l4");
                other.Add(new fuelPrm() { prm = "RVSM_FLT_STBY4", value = "" });
                props.Add("prop_rvsm_flt_stby4");
                other.Add(new fuelPrm() { prm = "RVSM_FLT_R4", value = "" });
                props.Add("prop_rvsm_flt_r4");
                other.Add(new fuelPrm() { prm = "RVSM_FLT_TIME4", value = "" });
                props.Add("prop_rvsm_flt_time4");



                other.Add(new fuelPrm() { prm = "RVSM_GND_L", value = "" });
                props.Add("prop_rvsm_gnd_l");
                other.Add(new fuelPrm() { prm = "RVSM_GND_STBY", value = "" });
                props.Add("prop_rvsm_gnd_stby");
                other.Add(new fuelPrm() { prm = "RVSM_GND_R", value = "" });
                props.Add("prop_rvsm_gnd_r");
                other.Add(new fuelPrm() { prm = "RVSM_GND_TIME", value = "" });
                props.Add("prop_rvsm_gnd_time");


                other.Add(new fuelPrm() { prm = "RVSM_FLT_LVL", value = "" });
                props.Add("prop_rvsm_flt_lvl");

                other.Add(new fuelPrm() { prm = "RVSM_FLT_LVL2", value = "" });
                props.Add("prop_rvsm_flt_lvl2");


                other.Add(new fuelPrm() { prm = "RVSM_FLT_LVL3", value = "" });
                props.Add("prop_rvsm_flt_lvl3");

                other.Add(new fuelPrm() { prm = "RVSM_FLT_LVL4", value = "" });
                props.Add("prop_rvsm_flt_lvl4");



                other.Add(new fuelPrm() { prm = "FILLED_CPT", value = "" });
                props.Add("prop_filled_cpt");
                other.Add(new fuelPrm() { prm = "FILLED_FO", value = "" });
                props.Add("prop_filled_fo");

                //prop_offblock
                other.Add(new fuelPrm() { prm = "OFFBLOCK", value = "" });
                props.Add("prop_offblock");
                //prop_takeoff
                other.Add(new fuelPrm() { prm = "TAKEOFF", value = "" });
                props.Add("prop_takeoff");
                //prop_landing
                other.Add(new fuelPrm() { prm = "LANDING", value = "" });
                props.Add("prop_landing");
                //prop_onblock
                other.Add(new fuelPrm() { prm = "ONBLOCK", value = "" });
                props.Add("prop_onblock");
                //prop_fuel_onblock
                other.Add(new fuelPrm() { prm = "FUEL_ONBLOCK", value = "" });
                props.Add("prop_fuel_onblock");
                other.Add(new fuelPrm() { prm = "FUEL_ONBLOCK_ALT1", value = "" });
                props.Add("prop_fuel_onblock_alt1");
                other.Add(new fuelPrm() { prm = "FUEL_ONBLOCK_ALT2", value = "" });
                props.Add("prop_fuel_onblock_alt2");

                other.Add(new fuelPrm() { prm = "ARR_ATIS", value = "" });
                props.Add("prop_arr_atis");
                other.Add(new fuelPrm() { prm = "DEP_ATIS1", value = "" });
                props.Add("prop_dep_atis1");
                other.Add(new fuelPrm() { prm = "DEP_ATIS2", value = "" });
                props.Add("prop_dep_atis2");

                other.Add(new fuelPrm() { prm = "ARR_QNH", value = "" });
                props.Add("prop_arr_qnh");
                other.Add(new fuelPrm() { prm = "DEP_QNH1", value = "" });
                props.Add("prop_dep_qnh1");
                other.Add(new fuelPrm() { prm = "DEP_QNH2", value = "" });
                props.Add("prop_dep_qnh2");


                other.Add(new fuelPrm() { prm = "TO_V1", value = "" });
                props.Add("prop_to_v1");
                other.Add(new fuelPrm() { prm = "TO_VR", value = "" });
                props.Add("prop_to_vr");
                other.Add(new fuelPrm() { prm = "TO_V2", value = "" });
                props.Add("prop_to_v2");
                other.Add(new fuelPrm() { prm = "TO_CONF", value = "" });
                props.Add("prop_to_conf");
                other.Add(new fuelPrm() { prm = "TO_ASMD", value = "" });
                props.Add("prop_to_asmd");
                other.Add(new fuelPrm() { prm = "TO_RWY", value = "" });
                props.Add("prop_to_rwy");
                other.Add(new fuelPrm() { prm = "TO_COND", value = "" });
                props.Add("prop_to_cond");

                other.Add(new fuelPrm() { prm = "TO_INFO", value = "" });
                props.Add("prop_to_info");
                other.Add(new fuelPrm() { prm = "TO_TIME", value = "" });
                props.Add("prop_to_time");
                other.Add(new fuelPrm() { prm = "TO_TA", value = "" });
                props.Add("prop_to_ta");
                other.Add(new fuelPrm() { prm = "TO_FE", value = "" });
                props.Add("prop_to_fe");
                other.Add(new fuelPrm() { prm = "TO_WIND", value = "" });
                props.Add("prop_to_wind");
                other.Add(new fuelPrm() { prm = "TO_VIS", value = "" });
                props.Add("prop_to_vis");
                other.Add(new fuelPrm() { prm = "TO_CLOUD", value = "" });
                props.Add("prop_to_cloud");
                other.Add(new fuelPrm() { prm = "TO_TEMP", value = "" });
                props.Add("prop_to_temp");
                other.Add(new fuelPrm() { prm = "TO_DEWP", value = "" });
                props.Add("prop_to_dewp");
                other.Add(new fuelPrm() { prm = "TO_QNH", value = "" });
                props.Add("prop_to_qnh");
                other.Add(new fuelPrm() { prm = "TO_FLAP", value = "" });
                props.Add("prop_to_flap");
                other.Add(new fuelPrm() { prm = "TO_STAB", value = "" });
                props.Add("prop_to_stab");
                other.Add(new fuelPrm() { prm = "TO_CG", value = "" });
                props.Add("prop_to_cg");
                other.Add(new fuelPrm() { prm = "TO_ALTFUEL", value = "" });
                props.Add("prop_to_altfuel");




                other.Add(new fuelPrm() { prm = "LND_STAR", value = "" });
                props.Add("prop_lnd_star");
                other.Add(new fuelPrm() { prm = "LND_APP", value = "" });
                props.Add("prop_lnd_app");
                other.Add(new fuelPrm() { prm = "LND_VREF", value = "" });
                props.Add("prop_lnd_vref");
                other.Add(new fuelPrm() { prm = "LND_CONF", value = "" });
                props.Add("prop_lnd_conf");
                other.Add(new fuelPrm() { prm = "LND_LDA", value = "" });
                props.Add("prop_lnd_lda");
                other.Add(new fuelPrm() { prm = "LND_RWY", value = "" });
                props.Add("prop_lnd_rwy");
                other.Add(new fuelPrm() { prm = "LND_COND", value = "" });
                props.Add("prop_lnd_cond");
                other.Add(new fuelPrm() { prm = "LND_INFO", value = "" });
                props.Add("prop_lnd_info");
                other.Add(new fuelPrm() { prm = "LND_TIME", value = "" });
                props.Add("prop_lnd_time");
                other.Add(new fuelPrm() { prm = "LND_TL", value = "" });
                props.Add("prop_lnd_tl");
                other.Add(new fuelPrm() { prm = "LND_FE", value = "" });
                props.Add("prop_lnd_fe");
                other.Add(new fuelPrm() { prm = "LND_WIND", value = "" });
                props.Add("prop_lnd_wind");
                other.Add(new fuelPrm() { prm = "LND_VIS", value = "" });
                props.Add("prop_lnd_vis");

                other.Add(new fuelPrm() { prm = "LND_CLOUD", value = "" });
                props.Add("prop_lnd_cloud");
                other.Add(new fuelPrm() { prm = "LND_TEMP", value = "" });
                props.Add("prop_lnd_temp");
                other.Add(new fuelPrm() { prm = "LND_DEWP", value = "" });
                props.Add("prop_lnd_dewp");
                other.Add(new fuelPrm() { prm = "LND_QNH", value = "" });
                props.Add("prop_lnd_qnh");
                other.Add(new fuelPrm() { prm = "LND_FLAP", value = "" });
                props.Add("prop_lnd_flap");
                other.Add(new fuelPrm() { prm = "LND_STAB", value = "" });
                props.Add("prop_lnd_stab");
                other.Add(new fuelPrm() { prm = "LND_MAS", value = "" });
                props.Add("prop_lnd_mas");
                other.Add(new fuelPrm() { prm = "LND_ACTWEIGHT", value = "" });
                props.Add("prop_lnd_actweight");
                other.Add(new fuelPrm() { prm = "LND_ALTFUEL", value = "" });
                props.Add("prop_lnd_altfuel");

                //  other.Add(new fuelPrm() { prm = "LND_COND", value = "" });
                // props.Add("prop_lnd_cond");

                other.Add(new fuelPrm() { prm = "CLR_TAXIOUT", value = "" });
                props.Add("prop_clr_taxiout");

                other.Add(new fuelPrm() { prm = "CLR_TAXIIN", value = "" });
                props.Add("prop_clr_taxiin");

                other.Add(new fuelPrm() { prm = "CLR_TAXIOUT_STAND", value = "" });
                props.Add("prop_clr_taxiout_stand");

                other.Add(new fuelPrm() { prm = "CLR_TAXIIN_STAND", value = "" });
                props.Add("prop_clr_taxiin_stand");

                other.Add(new fuelPrm() { prm = "ECTM_TAT", value = "" });
                props.Add("prop_ectm_tat");

                other.Add(new fuelPrm() { prm = "ECTM_MACH", value = "" });
                props.Add("prop_ectm_mach");

                other.Add(new fuelPrm() { prm = "ECTM_PRESSURE_ALT", value = "" });
                props.Add("prop_ectm_pressure_alt");

                other.Add(new fuelPrm() { prm = "ECTM_COMPUTED_AIRSPEED", value = "" });
                props.Add("prop_ectm_computed_airspeed");

                other.Add(new fuelPrm() { prm = "ECTM_ENG1_EGT", value = "" });
                props.Add("prop_ectm_eng1_egt");

                other.Add(new fuelPrm() { prm = "ECTM_ENG1_N1", value = "" });
                props.Add("prop_ectm_eng1_n1");

                other.Add(new fuelPrm() { prm = "ECTM_ENG1_N2", value = "" });
                props.Add("prop_ectm_eng1_n2");

                other.Add(new fuelPrm() { prm = "ECTM_ENG1_EPR", value = "" });
                props.Add("prop_ectm_eng1_epr");

                other.Add(new fuelPrm() { prm = "ECTM_ENG1_FUEL_FLOW", value = "" });
                props.Add("prop_ectm_eng1_fuel_flow");

                other.Add(new fuelPrm() { prm = "ECTM_ENG2_EGT", value = "" });
                props.Add("prop_ectm_eng2_egt");

                other.Add(new fuelPrm() { prm = "ECTM_ENG2_N1", value = "" });
                props.Add("prop_ectm_eng2_n1");

                other.Add(new fuelPrm() { prm = "ECTM_ENG2_N2", value = "" });
                props.Add("prop_ectm_eng2_n2");

                other.Add(new fuelPrm() { prm = "ECTM_ENG2_EPR", value = "" });
                props.Add("prop_ectm_eng2_epr");

                other.Add(new fuelPrm() { prm = "ECTM_ENG2_FUEL_FLOW", value = "" });
                props.Add("prop_ectm_eng2_fuel_flow");


                var dtupd = DateTime.UtcNow.ToString("yyyyMMddHHmm");


                foreach (var prop in props)
                    plan.OFPImportProps.Add(new OFPImportProp()
                    {
                        DateUpdate = dtupd,
                        PropName = prop,
                        PropValue = "",
                        User = "airpocket",

                    });
                var _fuel = JsonConvert.SerializeObject(fuel);
                plan.JFuel = _fuel; //"["+string.Join(",", fuel)+"]";
                plan.JPlan = "[" + string.Join(",", mplnpJson) + "]";
                //FUCK







                plan.ALT1 = alt1;
                plan.ALT2 = alt2;

                fltobj.ALT1 = alt1;
                fltobj.ALT2 = alt2;


                plan.TextOutput = JsonConvert.SerializeObject(other);

                dto.DateUpload = DateTime.Now;
                dto.UploadStatus = 1;
                dto.UploadMessage = "OK";


                //foreach (var x in mainPlanPoints)
                //{
                //    plan.OFPPoints.Add(new OFPPoint()
                //    {
                //        Plan = "MAIN",
                //        Lat = Convert.ToDecimal(x.Split(' ')[0]),
                //        Long = Convert.ToDecimal(x.Split(' ')[1]),

                //    });

                //}
                context.SaveChanges();
                /*
                foreach (var x in mplan_points)
                // plan.OFPPoints.Add(x);
                {
                    x.OFPId = plan.Id;
                    context.OFPPoints.Add(x);
                }

                context.SaveChanges();

                ppa_main main_context = new ppa_main();
                var exists = main_context.OFPPools.Where(q => q.FlightNo == main_flight_no && q.FlightId == plan.FlightId).ToList();
                main_context.OFPPools.RemoveRange(exists);

                var pool_ofp = new OFPPool()
                {
                    ALT1 = plan.ALT1,
                    ALT2 = plan.ALT2,
                    BaseId = plan.Id,
                    CRW = plan.CRW,
                    JALDRF = plan.JALDRF,
                    JAPlan1 = plan.JAPlan1,
                    JAPlan2 = plan.JAPlan2,
                    JCSTBL = plan.JCSTBL,
                    JFuel = plan.JFuel,
                    JLDatePICApproved = plan.JLDatePICApproved,
                    JLSignedBy = plan.JLSignedBy,
                    JPlan = plan.JPlan,
                    DateConfirmed = plan.DateConfirmed,
                    DateCreate = plan.DateCreate,
                    DateFlight = plan.DateFlight,
                    DateUpdate = plan.DateUpdate,
                    Destination = plan.Destination,
                    DID = plan.DID,
                    DOW = plan.DOW,
                    ELDW = plan.ELDW,
                    ETA = plan.ETA,
                    ETD = plan.ETD,
                    ETOW = plan.ETOW,
                    EZFW = plan.EZFW,
                    FileName = plan.FileName,
                    FlightId = plan.FlightId,
                    FlightNo = main_flight_no,
                    FLL = plan.FLL,
                    FPF = plan.FPF,
                    FPFuel = plan.FPFuel,
                    FPTripFuel = plan.FPTripFuel,
                    FuelACTUALTANKERING = plan.FuelACTUALTANKERING,
                    FuelALT1 = plan.FuelALT1,
                    FuelALT2 = plan.FuelALT2,
                    FuelCONT = plan.FuelCONT,
                    FuelETOPSADDNL = plan.FuelETOPSADDNL,
                    FuelExtra = plan.FuelExtra,
                    FuelFINALRES = plan.FuelFINALRES,
                    FuelMINTOF = plan.FuelMINTOF,
                    FuelOFFBLOCK = plan.FuelOFFBLOCK,
                    FuelOPSEXTRA = plan.FuelOPSEXTRA,
                    FuelTANKERING = plan.FuelTANKERING,
                    FuelTAXI = plan.FuelTAXI,
                    FuelTOF = plan.FuelTOF,
                    FuelTOTALFUEL = plan.FuelTOTALFUEL,
                    JWTDRF = plan.JWTDRF,
                    MAXSHEER = plan.MAXSHEER,
                    MCI = plan.MCI,
                    MINDIVFUEL = plan.MINDIVFUEL,
                    Origin = plan.Origin,
                    PIC = plan.PIC,
                    PICId = plan.PICId,
                    PLD = plan.PLD,
                    RTA = plan.RTA,
                    RTB = plan.RTB,
                    RTM = plan.RTM,
                    RTT = plan.RTT,
                    Source = plan.Source,
                    TALT1 = plan.TALT1,
                    TALT2 = plan.TALT2,
                    Text = plan.Text,
                    TextOutput = plan.TextOutput,
                    THM = plan.THM,
                    UNT = plan.UNT,
                    User = plan.User,
                    UserConfirmed = plan.UserConfirmed,
                    VDT = plan.VDT,
                    WDCLB = plan.WDCLB,
                    WDDES = plan.WDDES,
                    WDTMP = plan.WDTMP

                };
                main_context.OFPPools.Add(pool_ofp);

                foreach (var p in mplan_points)
                {
                    pool_ofp.OFPPoolPoints.Add(new OFPPoolPoint()
                    {
                        ALT = p.ALT,
                        BODY = p.BODY,
                        DIS = p.DIS,
                        FRE = p.FRE,
                        FRQ = p.FRQ,
                        FUS = p.FUS,
                        GMR = p.GMR,
                        GSP = p.GSP,
                        Lat = p.Lat,
                        Long = p.Long,
                        MEA = p.MEA,
                        Plan = p.Plan,
                        TAS = p.TAS,
                        TDS = p.TDS,
                        TMP = p.TMP,
                        TRK = p.TRK,
                        VIA = p.VIA,
                        WAP = p.WAP,
                        WIND = p.WIND,
                        TME = p.TME,
                        TTM = p.TTM

                    });
                }


                main_context.SaveChanges();

                */

                return Ok(true);
            }
            catch (DbEntityValidationException e)
            {
                List<string> errs = new List<string>();
                foreach (var eve in e.EntityValidationErrors)
                {
                    //Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                    //eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    errs.Add(eve.Entry.Entity.GetType().Name + "    " + eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        // Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                        //     ve.PropertyName, ve.ErrorMessage);
                        errs.Add(ve.PropertyName + "    " + ve.ErrorMessage);
                    }
                }
                return Ok("Not Uploaded " + string.Join(",", errs));
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                if (ex.InnerException != null)
                    message += "  INNER: " + ex.InnerException.Message;
                if (ex.InnerException.InnerException != null)
                    message += "  INNER: " + ex.InnerException.InnerException.Message;
                dto.DateUpload = DateTime.Now;
                dto.UploadStatus = -1;
                dto.UploadMessage = message;
                //try
                //{
                //    context.SaveChanges();
                //}
                //catch (Exception ex) { }

                return Ok("Not Uploaded " + message);
            }






        }

        [Route("api/ofp/pool/import/{id}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetImportToPool(int id)
        {
            try
            {
                var context = new PPAEntities();
                ppa_main main_context = new ppa_main();
                var plan = context.OFPImports.Where(q => q.Id == id).FirstOrDefault();
                var points = context.OFPPoints.Where(q => q.OFPId == id).OrderBy(q => q.Id).ToList();
                var exists = main_context.OFPPools.Where(q => q.FlightNo == "VRH" + plan.FlightNo && q.FlightId == plan.FlightId).ToList();

                main_context.OFPPools.RemoveRange(exists);

                var pool_ofp = new OFPPool()
                {
                    ALT1 = plan.ALT1,
                    ALT2 = plan.ALT2,
                    BaseId = plan.Id,
                    CRW = plan.CRW,
                    JALDRF = plan.JALDRF,
                    JAPlan1 = plan.JAPlan1,
                    JAPlan2 = plan.JAPlan2,
                    JCSTBL = plan.JCSTBL,
                    JFuel = plan.JFuel,
                    JLDatePICApproved = plan.JLDatePICApproved,
                    JLSignedBy = plan.JLSignedBy,
                    JPlan = plan.JPlan,
                    DateConfirmed = plan.DateConfirmed,
                    DateCreate = plan.DateCreate,
                    DateFlight = plan.DateFlight,
                    DateUpdate = plan.DateUpdate,
                    Destination = plan.Destination,
                    DID = plan.DID,
                    DOW = plan.DOW,
                    ELDW = plan.ELDW,
                    ETA = plan.ETA,
                    ETD = plan.ETD,
                    ETOW = plan.ETOW,
                    EZFW = plan.EZFW,
                    FileName = plan.FileName,
                    FlightId = plan.FlightId,
                    FlightNo = "VRH" + plan.FlightNo,
                    FLL = plan.FLL,
                    FPF = plan.FPF,
                    FPFuel = plan.FPFuel,
                    FPTripFuel = plan.FPTripFuel,
                    FuelACTUALTANKERING = plan.FuelACTUALTANKERING,
                    FuelALT1 = plan.FuelALT1,
                    FuelALT2 = plan.FuelALT2,
                    FuelCONT = plan.FuelCONT,
                    FuelETOPSADDNL = plan.FuelETOPSADDNL,
                    FuelExtra = plan.FuelExtra,
                    FuelFINALRES = plan.FuelFINALRES,
                    FuelMINTOF = plan.FuelMINTOF,
                    FuelOFFBLOCK = plan.FuelOFFBLOCK,
                    FuelOPSEXTRA = plan.FuelOPSEXTRA,
                    FuelTANKERING = plan.FuelTANKERING,
                    FuelTAXI = plan.FuelTAXI,
                    FuelTOF = plan.FuelTOF,
                    FuelTOTALFUEL = plan.FuelTOTALFUEL,
                    JWTDRF = plan.JWTDRF,
                    MAXSHEER = plan.MAXSHEER,
                    MCI = plan.MCI,
                    MINDIVFUEL = plan.MINDIVFUEL,
                    Origin = plan.Origin,
                    PIC = plan.PIC,
                    PICId = plan.PICId,
                    PLD = plan.PLD,
                    RTA = plan.RTA,
                    RTB = plan.RTB,
                    RTM = plan.RTM,
                    RTT = plan.RTT,
                    Source = plan.Source,
                    TALT1 = plan.TALT1,
                    TALT2 = plan.TALT2,
                    Text = plan.Text,
                    TextOutput = plan.TextOutput,
                    THM = plan.THM,
                    UNT = plan.UNT,
                    User = plan.User,
                    UserConfirmed = plan.UserConfirmed,
                    VDT = plan.VDT,
                    WDCLB = plan.WDCLB,
                    WDDES = plan.WDDES,
                    WDTMP = plan.WDTMP

                };
                main_context.OFPPools.Add(pool_ofp);

                foreach (var p in points)
                {
                    pool_ofp.OFPPoolPoints.Add(new OFPPoolPoint()
                    {
                        ALT = p.ALT,
                        BODY = p.BODY,
                        DIS = p.DIS,
                        FRE = p.FRE,
                        FRQ = p.FRQ,
                        FUS = p.FUS,
                        GMR = p.GMR,
                        GSP = p.GSP,
                        Lat = p.Lat,
                        Long = p.Long,
                        MEA = p.MEA,
                        Plan = p.Plan,
                        TAS = p.TAS,
                        TDS = p.TDS,
                        TMP = p.TMP,
                        TRK = p.TRK,
                        VIA = p.VIA,
                        WAP = p.WAP,
                        WIND = p.WIND

                    });
                }


                main_context.SaveChanges();



                return Ok(true);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                return Ok(msg);
            }
        }

        [Route("api/ofp/points/{id}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetOFPPoints(int id)
        {
            var context = new PPAEntities();
            var points = context.ViewOFPPoints.Where(q => q.OFPId == id).OrderBy(q => q.Id).ToList();
            return Ok(points);
        }

        [Route("api/ofp/points/flight/{id}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetOFPPointsByFlight(int id)
        {
            var context = new PPAEntities();
            var points = context.ViewOFPPoints.Where(q => q.FlightId == id).OrderBy(q => q.Id).ToList();
            return Ok(points);
        }


        [Route("api/appleg/ofp/{flightId}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetOPF(int flightId)
        {


            var context = new PPAEntities();
            var ofp = context.OFPImports.FirstOrDefault(q => q.FlightId == flightId);
            if (ofp == null)
                return Ok(new { Id = -1 });
            else
            {
                var dr = context.EFBDSPReleases.FirstOrDefault(q => q.FlightId == flightId);
                var _props = context.OFPImportProps.Where(q => q.OFPId == ofp.Id).ToList();
                if (dr != null && dr.MinFuelRequiredPilotReq != null)
                {
                    var fprop = _props.FirstOrDefault(q => q.PropName == "prop_reqfuel");
                    if (fprop != null)
                    {
                        fprop.PropValue = dr.MinFuelRequiredPilotReq.ToString();
                    }
                }
                var props = _props.Select(q =>
                  new
                  {
                      q.Id,
                      q.OFPId,
                      q.PropName,
                      q.PropType,
                      q.PropValue,
                      q.User,
                      q.DateUpdate,

                  }
                    ).ToList();

                return Ok(new
                {
                    ofp.Id,
                    ofp.FlightId,
                    ofp.TextOutput,
                    ofp.User,
                    ofp.DateCreate,
                    ofp.PIC,
                    ofp.PICId,
                    ofp.JLSignedBy,
                    ofp.JLDatePICApproved,
                    props

                });
            }



        }



        public class fuelPrm
        {
            public string prm { get; set; }
            public string time { get; set; }
            public string value { get; set; }

            public string _key { get; set; }
        }

        public class windy_auth_dto
        {
            public string hostname { get; set; }
            public string key { get; set; }
        }
        public class windy_auth_feather
        {

        }
        public class windy_auth_response
        {
            public bool paid { get; set; }
            public bool exceeded { get; set; }
            public string domains { get; set; }
            public windy_auth_feather features { get; set; }
            public string apiUser { get; set; }
            public int id { get; set; }
            public string name { get; set; }
            public string key { get; set; }
            public string auth { get; set; }

        }

        [Route("api/windy/auth2")]
        [AcceptVerbs("POST")]
        public IHttpActionResult PostWindyAuth(windy_auth_dto dto)
        {
            //            {
            //                "paid": true,
            //    "exceeded": false,
            //    "domains": "api4.windy.com,api.windy.com,api-staging.windy.com",
            //    "features": { },
            //    "apiUser": "admin",
            //    "id": 0,
            //    "name": "https:api4.windy.com",
            //    "key": "PsLAtXpsPTZexBwUkO7Mx5I",
            //    "auth": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJwYWlkIjp0cnVlLCJleGNlZWRlZCI6ZmFsc2UsImRvbWFpbnMiOiJhcGk0LndpbmR5LmNvbSxhcGkud2luZHkuY29tLGFwaS1zdGFnaW5nLndpbmR5LmNvbSIsImZlYXR1cmVzIjp7fSwiYXBpVXNlciI6ImFkbWluIiwiaWQiOjAsIm5hbWUiOiJodHRwczphcGk0LndpbmR5LmNvbSIsImtleSI6IlBzTEF0WHBzUFRaZXhCd1VrTzdNeDVJIiwidWEiOiJNb3ppbGxhLzUuMCAoV2luZG93cyBOVCAxMC4wOyBXaW42NDsgeDY0KSBBcHBsZVdlYktpdC81MzcuMzYgKEtIVE1MLCBsaWtlIEdlY2tvKSBDaHJvbWUvMTE3LjAuMC4wIFNhZmFyaS81MzcuMzYgRWRnLzExNy4wLjIwNDUuNjAiLCJpYXQiOjE2OTcxMzI4MjQsImV4cCI6MTY5NzQzMjgyNH0.1SYTh1ePqCsjWuDe07F9Rn2wc1tm9mK9jKOwfqFEm2E"
            //}


            var response = new windy_auth_response()
            {
                apiUser = "admin",
                auth = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJwYWlkIjp0cnVlLCJleGNlZWRlZCI6ZmFsc2UsImRvbWFpbnMiOiJhcGk0LndpbmR5LmNvbSxhcGkud2luZHkuY29tLGFwaS1zdGFnaW5nLndpbmR5LmNvbSIsImZlYXR1cmVzIjp7fSwiYXBpVXNlciI6ImFkbWluIiwiaWQiOjAsIm5hbWUiOiJodHRwczphcGk0LndpbmR5LmNvbSIsImtleSI6IlBzTEF0WHBzUFRaZXhCd1VrTzdNeDVJIiwidWEiOiJNb3ppbGxhLzUuMCAoV2luZG93cyBOVCAxMC4wOyBXaW42NDsgeDY0KSBBcHBsZVdlYktpdC81MzcuMzYgKEtIVE1MLCBsaWtlIEdlY2tvKSBDaHJvbWUvMTE3LjAuMC4wIFNhZmFyaS81MzcuMzYgRWRnLzExNy4wLjIwNDUuNjAiLCJpYXQiOjE2OTcxMzI4MjQsImV4cCI6MTY5NzQzMjgyNH0.1SYTh1ePqCsjWuDe07F9Rn2wc1tm9mK9jKOwfqFEm2E",
                domains = "api4.windy.com,api.windy.com,api-staging.windy.com",
                exceeded = false,
                features = new windy_auth_feather(),
                id = 0,
                key = "PsLAtXpsPTZexBwUkO7Mx5I",
                name = "https:api4.windy.com",
                paid = true,



            };
            return Ok(response);

        }

        [Route("api/windy/auth")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetWindyAuth()
        {
            //            {
            //                "paid": true,
            //    "exceeded": false,
            //    "domains": "api4.windy.com,api.windy.com,api-staging.windy.com",
            //    "features": { },
            //    "apiUser": "admin",
            //    "id": 0,
            //    "name": "https:api4.windy.com",
            //    "key": "PsLAtXpsPTZexBwUkO7Mx5I",
            //    "auth": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJwYWlkIjp0cnVlLCJleGNlZWRlZCI6ZmFsc2UsImRvbWFpbnMiOiJhcGk0LndpbmR5LmNvbSxhcGkud2luZHkuY29tLGFwaS1zdGFnaW5nLndpbmR5LmNvbSIsImZlYXR1cmVzIjp7fSwiYXBpVXNlciI6ImFkbWluIiwiaWQiOjAsIm5hbWUiOiJodHRwczphcGk0LndpbmR5LmNvbSIsImtleSI6IlBzTEF0WHBzUFRaZXhCd1VrTzdNeDVJIiwidWEiOiJNb3ppbGxhLzUuMCAoV2luZG93cyBOVCAxMC4wOyBXaW42NDsgeDY0KSBBcHBsZVdlYktpdC81MzcuMzYgKEtIVE1MLCBsaWtlIEdlY2tvKSBDaHJvbWUvMTE3LjAuMC4wIFNhZmFyaS81MzcuMzYgRWRnLzExNy4wLjIwNDUuNjAiLCJpYXQiOjE2OTcxMzI4MjQsImV4cCI6MTY5NzQzMjgyNH0.1SYTh1ePqCsjWuDe07F9Rn2wc1tm9mK9jKOwfqFEm2E"
            //}


            var response = new windy_auth_response()
            {
                apiUser = "admin",
                auth = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJwYWlkIjp0cnVlLCJleGNlZWRlZCI6ZmFsc2UsImRvbWFpbnMiOiJhcGk0LndpbmR5LmNvbSxhcGkud2luZHkuY29tLGFwaS1zdGFnaW5nLndpbmR5LmNvbSIsImZlYXR1cmVzIjp7fSwiYXBpVXNlciI6ImFkbWluIiwiaWQiOjAsIm5hbWUiOiJodHRwczphcGk0LndpbmR5LmNvbSIsImtleSI6IlBzTEF0WHBzUFRaZXhCd1VrTzdNeDVJIiwidWEiOiJNb3ppbGxhLzUuMCAoV2luZG93cyBOVCAxMC4wOyBXaW42NDsgeDY0KSBBcHBsZVdlYktpdC81MzcuMzYgKEtIVE1MLCBsaWtlIEdlY2tvKSBDaHJvbWUvMTE3LjAuMC4wIFNhZmFyaS81MzcuMzYgRWRnLzExNy4wLjIwNDUuNjAiLCJpYXQiOjE2OTcxMzI4MjQsImV4cCI6MTY5NzQzMjgyNH0.1SYTh1ePqCsjWuDe07F9Rn2wc1tm9mK9jKOwfqFEm2E",
                domains = "api4.windy.com,api.windy.com,api-staging.windy.com",
                exceeded = false,
                features = new windy_auth_feather(),
                id = 0,
                key = "PsLAtXpsPTZexBwUkO7Mx5I",
                name = "https:api4.windy.com",
                paid = true,



            };
            return Ok(response);

        }

        public static class WebUtils
        {
            public static Encoding GetEncodingFrom(
                NameValueCollection responseHeaders,
                Encoding defaultEncoding = null)
            {
                if (responseHeaders == null)
                    throw new ArgumentNullException("responseHeaders");

                //Note that key lookup is case-insensitive
                var contentType = responseHeaders["Content-Type"];
                if (contentType == null)
                    return defaultEncoding;

                var contentTypeParts = contentType.Split(';');
                if (contentTypeParts.Length <= 1)
                    return defaultEncoding;

                var charsetPart =
                    contentTypeParts.Skip(1).FirstOrDefault(
                        p => p.TrimStart().StartsWith("charset", StringComparison.InvariantCultureIgnoreCase));
                if (charsetPart == null)
                    return defaultEncoding;

                var charsetPartParts = charsetPart.Split('=');
                if (charsetPartParts.Length != 2)
                    return defaultEncoding;

                var charsetName = charsetPartParts[1].Trim();
                if (charsetName == "")
                    return defaultEncoding;

                try
                {
                    return Encoding.GetEncoding(charsetName);
                }
                catch (ArgumentException ex)
                {
                    throw new InvalidOperationException(

                        "The server returned data in an unknown encoding: " + charsetName
                        );
                }
            }
        }

        [Route("api/windy/img")]

        public async Task<IHttpActionResult> getProductImage(string url)
        {
            url = url.Replace("xhtt", "htt");
            using (HttpClient client = new HttpClient())
            {

                HttpResponseMessage _response = await client.GetAsync(url);
                byte[] content = await _response.Content.ReadAsByteArrayAsync();

                // return Ok(File(content, "image/png"));


                var response = new HttpResponseMessage(HttpStatusCode.OK);
                Stream stream = new MemoryStream(content);
                response.Content = new StreamContent(stream);

                response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                response.Content.Headers.ContentLength = stream.Length;

                return ResponseMessage(response);

                // return Ok(true);

            }
        }


        [Route("api/windy/url")]
        // [AcceptVerbs("GET")]
        public IHttpActionResult GetUrl(string url)
        {
            try
            {
                url = url.Replace("xhtt", "htt");
                string response = null;
                using (WebClient webClient = new WebClient())
                {
                    // webClient.Encoding = Encoding.ASCII;
                    response = webClient.DownloadString(url);

                    // obj_metar = JsonConvert.DeserializeObject<List<WeatherMetar>>(str_metar);



                }
                try
                {
                    var js = JsonConvert.DeserializeObject(response);
                    return Ok(js);
                }
                catch (Exception ex)
                {
                    string result = "";
                    string result2 = "";
                    object res;
                    //using (var client =new   WebClient())
                    //{
                    //    client.Headers.Add("Accept-Language", "en-gb,en;q=0.5");
                    //    client.Headers.Add("Accept-Charset", "ISO-8859-1,utf-8;q=0.7,*;q=0.7");
                    //    result =   client.DownloadString(url);


                    //}

                    HttpClientHandler handler = new HttpClientHandler()
                    {
                        AutomaticDecompression = DecompressionMethods.GZip,
                    };


                    using (var client = new HttpClient(handler))
                    {
                        result2 = client.GetStringAsync(url).Result;
                        try
                        {
                            res = JsonConvert.DeserializeObject(result2);
                        }
                        catch (Exception exx)
                        {
                            res = string.Empty;
                        }


                    }
                    return Ok(res);
                }
            }
            catch (Exception exxxxx)
            {
                return Ok(JsonConvert.DeserializeObject(string.Empty));
            }



        }

        [Route("api/windy/test")]
        [AcceptVerbs("GET")]
        public IHttpActionResult Gettest()
        {

            HttpContext.Current.Response.Headers.Add("MaxRecords", "1000");
            return Ok(true);

        }

        //[HttpGet]
        //[Route("api/windy/test")]
        //public HttpResponseMessage Gettest()
        //{
        //    // Get students from Database

        //    // Create the response
        //    var response = Request.CreateResponse(HttpStatusCode.OK, true);

        //    // Set headers for paging
        //    response.Headers.Add("X-Students-Total-Count", "QWERT");

        //    return response;
        //}

        [Route("api/windy/connection")]
        [AcceptVerbs("HEAD")]
        public IHttpActionResult GetConnection()
        {


            return Ok(true);

        }



        [Route("api/skyputer/get")]
        [AcceptVerbs("GET")]
        public IHttpActionResult PostSkyputer2(skyputer dto)
        {

            if (string.IsNullOrEmpty(dto.key))
                return Ok("Authorization key not found.");
            if (string.IsNullOrEmpty(dto.plan))
                return Ok("Plan cannot be empty.");
            if (dto.key != "Skyputer@1359#")
                return Ok("Authorization key is wrong.");
            var entity = new OFPSkyPuter()
            {
                OFP = dto.plan,

            };
            var ctx = new PPAEntities();
            ctx.OFPSkyPuters.Add(entity);
            ctx.SaveChanges();
            return Ok(true);


        }

        [Route("api/fdp/ext/get/{clr}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetFMISASSIGN(int clr, DateTime from)
        {
            var upd = "'upd" + DateTime.Now.ToString("yyyyMMdd-hhmmss") + "'";
            var dt = from.Date; //DateTime.Now.Date;
            var fmis_cnn_string = "Data Source=VA.FMIS.IR,2019;Initial Catalog=CrewVA;User ID=WinUsers;Password=Crew1018!)!*";
            var ap_cnn_string = "Data Source=65.21.14.236;Initial Catalog=ppa_varesh;User ID=Vahid;Password=Atrina1359@aA";
            //185.141.132.14
            //var ap_cnn_string = "Data Source=185.141.132.14;Initial Catalog=x_varesh;User ID=Vahid;Password=Atrina1359@aA";

            SqlConnection cnnAP = new SqlConnection(ap_cnn_string);
            cnnAP.Open();
            SqlConnection cnnFMIS = new SqlConnection(fmis_cnn_string);
            cnnFMIS.Open();
            //if (clr==0)
            {
                SqlCommand cmd1 = new SqlCommand("DELETE FROM FMISLEG", cnnAP);
                cmd1.ExecuteNonQuery();
                SqlCommand cmd2 = new SqlCommand("DELETE FROM FMISLEGASSIGN", cnnAP);
                cmd2.ExecuteNonQuery();



                var fmisSql = "SELECT   DateUTC ,FltNo ,DepStn ,ArrStn ,DepTime ,ArrTime ,DepTimeLCL ,ArrTimeLCL ,STD ,STA ,STC ,ACType ,ACReg ,Flt ,UpdateFlag ,ScheduleGroup ,TurnType ,RouteType ,LegDesc ,Change ,Importance ,LastUpdateTime ,LastUpdateScher ,Comment ,FltGroup ,StandardTime ,Holyday ,HolydayName ,NormalCorrectedTime ,ChangedCorrectedTime FROM dbo.Leg "
                             + " WHERE DateUTC='" + dt.ToString("yyyy-MM-dd") + "'";
                SqlDataAdapter da = new SqlDataAdapter(fmisSql, cnnFMIS);
                DataSet ds = new DataSet();
                da.FillError += new FillErrorEventHandler(FillError);
                da.Fill(ds);
                var tbl = ds.Tables[0];
                var columns = tbl.Columns;
                var rows = tbl.Select();

                SqlBulkCopy sqlbc = new SqlBulkCopy(cnnAP);
                sqlbc.BulkCopyTimeout = 6000;
                sqlbc.NotifyAfter = 100000;
                //sqlbc.BatchSize = 1;
                var xxxx = sqlbc.BatchSize;

                sqlbc.DestinationTableName = "FMISLEG";
                sqlbc.WriteToServer(rows);

                //update _xleg set [key]=cast(dateutc as varchar(500))+'_'+REPLACE(FltNo, 'VA ', '')+'_'+DepStn+'_'+ArrStn
                using (SqlCommand command = new SqlCommand("update FMISLEG set [key]=convert(varchar, cast(dateutc as date), 23)+'_'+FltNo+'_'+DepStn+'_'+ArrStn", cnnAP))
                {
                    var r1x = command.ExecuteNonQuery();
                }
                //UPDATE T SET T.Id = TT.ROW_ID  FROM _xleg AS T INNER JOIN (SELECT ROW_NUMBER() OVER (ORDER BY cast(std as datetime)) AS ROW_ID  ,[key] FROM _xleg) AS TT ON T.[key] = TT.[key]
                using (SqlCommand command = new SqlCommand("UPDATE T SET T.Id = TT.ROW_ID  FROM FMISLEG AS T INNER JOIN (SELECT ROW_NUMBER() OVER (ORDER BY cast(std as datetime)) AS ROW_ID  ,[key] FROM _xleg) AS TT ON T.[key] = TT.[key]", cnnAP))
                {
                    var r1x = command.ExecuteNonQuery();
                }

                using (SqlCommand command = new SqlCommand(" update FMISLEG set flightnumber = REPLACE(fltno, 'VA ', '')", cnnAP))
                {
                    var r1x = command.ExecuteNonQuery();
                }


                using (SqlCommand command = new SqlCommand("update FMISLEG set depstn = 'GSM' where DepStn = 'QSM'", cnnAP))
                {
                    var r1x = command.ExecuteNonQuery();
                }


                using (SqlCommand command = new SqlCommand("update FMISLEG set arrstn = 'GSM' where arrstn = 'QSM'", cnnAP))
                {
                    var r1x = command.ExecuteNonQuery();
                }



                using (SqlCommand command = new SqlCommand("update FMISLEG set fromairportid = (select id from airport where iata = DepStn)", cnnAP))
                {
                    var r1x = command.ExecuteNonQuery();
                }




                using (SqlCommand command = new SqlCommand("update FMISLEG set toairportid = (select id from airport where iata = ArrStn)", cnnAP))
                {
                    var r1x = command.ExecuteNonQuery();
                }




                using (SqlCommand command = new SqlCommand("update FMISLEG set regid = (select id from Ac_MSN where REGISTER = acreg)", cnnAP))
                {
                    var r1x = command.ExecuteNonQuery();
                }

                //update _xlegassign set initindex=cast(dbo.UDF_ExtractNumbers(pos) as int)

                //////////////////////////////////
                var fmisSql2 = "SELECT   Route ,Crew ,Pos ,Rank ,TurnType ,ScheduleGroup ,Scheduler ,DateUTC ,FltNo ,DepStn ,ArrStn ,DepTime ,ArrTime ,ACType ,ACReg ,Flt ,RouteType ,JobType ,DepTimeLCL ,ArrTimeLCL ,Change ,StandardTime ,Status ,Expr1 ,OffBlock ,OnBlock FROM dbo.LegAssign "
                            + " WHERE DateUTC='" + dt.ToString("yyyy-MM-dd") + "'";
                SqlDataAdapter da2 = new SqlDataAdapter(fmisSql2, cnnFMIS);
                DataSet ds2 = new DataSet();
                da2.FillError += new FillErrorEventHandler(FillError);
                da2.Fill(ds2);
                var tbl2 = ds2.Tables[0];
                var columns2 = tbl2.Columns;
                var rows2 = tbl2.Select();

                SqlBulkCopy sqlbc2 = new SqlBulkCopy(cnnAP);
                sqlbc2.BulkCopyTimeout = 6000;
                sqlbc2.NotifyAfter = 100000;
                //sqlbc.BatchSize = 1;


                sqlbc2.DestinationTableName = "FMISLEGASSIGN";
                sqlbc2.WriteToServer(rows2);


                using (SqlCommand command = new SqlCommand("update FMISLEGASSIGN set initindex=cast(dbo.UDF_ExtractNumbers(pos) as int)", cnnAP))
                {
                    var r1x = command.ExecuteNonQuery();
                }
                //update FMISLEGASSIGN set crewid=(select id from viewcrew where viewcrew.code=crew)
                using (SqlCommand command = new SqlCommand("update FMISLEGASSIGN set crewid=(select id from viewcrew where viewcrew.code=crew)", cnnAP))
                {
                    var r1x = command.ExecuteNonQuery();
                }

                var _cmd1 = "update FMISLEGASSIGN 	set HomeBase=(select baseairportid from viewcrew where viewcrew.id=crewid) "
            + ", schedulename = (select schedulename from viewcrew where viewcrew.id = crewid) "
            + ",jobgroupid = (select groupid from viewcrew where viewcrew.id = crewid) "
            + ",InitGroup = (select jobgroup from viewcrew where viewcrew.id = crewid) "
            + " where crewid is not null";
                using (SqlCommand command = new SqlCommand(_cmd1, cnnAP))
                {
                    var r1x = command.ExecuteNonQuery();
                }

                //update _xlegassign set initindex=2 where pos like 'AGFO%'
                using (SqlCommand command = new SqlCommand("update FMISLEGASSIGN set initindex=2 where pos like 'AGFO%'", cnnAP))
                {
                    var r1x = command.ExecuteNonQuery();
                }

                var _cmd2 = "update FMISLEGASSIGN "
    + "		set initrank=( "
    + "		   CASE dbo.UDF_ExtractChars(pos) "
    + "	     WHEN 'IP' THEN 'IP' "
    + "	     when 'CPT' then 'P1' "
    + "			 when 'FA' then 'CCM' "
    + "			 when 'FP' then 'SCCM' "
    + "			 when 'IFP' then 'ISCCM' "
    + "			 when 'FO' then 'P2' "
    + "			 when 'OB' then 'OBS' "
    + "			 when 'OBS' then 'OBS' "
    + "			 when 'SAFETY' then 'SAFETY' "
    + "			 else 'OBS' "
    + "      END "

    + "		) "

    + "		where crewid is not null ";
                using (SqlCommand command = new SqlCommand(_cmd2, cnnAP))
                {
                    var r1x = command.ExecuteNonQuery();
                }
                //update _xlegassign set [key]=cast(dateutc as varchar(500))+'_'+REPLACE(FltNo, 'VA ', '')+'_'+DepStn+'_'+ArrStn
                using (SqlCommand command = new SqlCommand("update FMISLEGASSIGN set [key]=convert(varchar, cast(dateutc as date), 23)+'_'+FltNo+'_'+DepStn+'_'+ArrStn", cnnAP))
                {
                    var r1x = command.ExecuteNonQuery();
                }

                //update _xlegassign set flightid = (select id from FlightInformation where departureremark =[key])
                using (SqlCommand command = new SqlCommand("update FMISLEGASSIGN set flightid = (select id from FlightInformation where ALT5 =[key])", cnnAP))
                {
                    var r1x = command.ExecuteNonQuery();
                }

                using (SqlCommand command = new SqlCommand("update FMISLEG set flightid = (select id from FlightInformation where ALT5 =[key])", cnnAP))
                {
                    var r1x = command.ExecuteNonQuery();
                }
                //////////////////////////////////////
            }
            var c1 = "select DISTINCT flt+'_'+convert(varchar, cast(dateutc as date), 23) from fmisleg where flt is not null and flt not in (select fdp from _xfdp)";
            List<string> newFLTs = new List<string>();
            using (SqlCommand command = new SqlCommand(c1, cnnAP))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        newFLTs.Add(reader.GetString(0));
                    }
                }
            }

            string insertNewFLT = File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath("~/bin/fdpnew.txt"));
            foreach (var flt in newFLTs)
            {
                var qry = insertNewFLT.Replace("@flt", "'" + flt.Split('_')[0] + "'").Replace("@Date", "'" + flt.Split('_')[1] + "'").Replace("@upd", upd);
                using (SqlCommand command = new SqlCommand(qry, cnnAP))
                {
                    var r1y = command.ExecuteNonQuery();
                }
            }




            ///////////////////////////////////////
            var c3 = "SELECT  id from _XFDP where crewid is not null and FDP+'_'+cast(crewid as varchar(10)) not in (select flt+'_'+cast(crewid as varchar(10)) from fmislegassign where crewid is not null)";
            List<string> delFLTCrews = new List<string>();
            using (SqlCommand command = new SqlCommand(c3, cnnAP))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        delFLTCrews.Add(reader.GetInt32(0).ToString());
                    }
                }
            }
            if (delFLTCrews.Count > 0)
            {
                using (SqlCommand command = new SqlCommand("delete from _xfdp where id in (" + string.Join(",", delFLTCrews) + ")", cnnAP))
                {
                    var r1y = command.ExecuteNonQuery();
                }
            }
            ////////////////////////////////////////
            //select DISTINCT flt+'_'+cast(CrewId as varchar(5)) from fmislegassign where flt+'_'+cast(CrewId as varchar(5)) not in (select fdp+'_'+cast(CrewId as varchar(5)) from _xfdpitem where crewid is not null) and crewid is not null
            var c2 = "select DISTINCT flt+'_'+cast(CrewId as varchar(5))+'_'+convert(varchar, cast(dateutc as date), 23) from fmislegassign where flt+'_'+cast(CrewId as varchar(5))+'_'+convert(varchar, cast(dateutc as date), 23) not in (select fdp+'_'+cast(CrewId as varchar(5))+'_'+convert(varchar, cast(dateutc as date), 23) from _xfdp  where crewid is not null) and crewid is not null";
            List<string> newFLTCrews = new List<string>();
            using (SqlCommand command = new SqlCommand(c2, cnnAP))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        newFLTCrews.Add(reader.GetString(0));
                    }
                }
            }

            string insertNewFLTCrew = File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath("~/bin/fdpcrewnew.txt"));
            foreach (var flt in newFLTCrews)
            {
                var qry = insertNewFLTCrew.Replace("@flt", "'" + flt.Split('_')[0] + "'").Replace("@Crew", "'" + flt.Split('_')[1] + "'").Replace("@Date", "'" + flt.Split('_')[2] + "'").Replace("@upd", upd);
                using (SqlCommand command = new SqlCommand(qry, cnnAP))
                {
                    var r1y = command.ExecuteNonQuery();
                }
            }

            /////////////////////////////////////
            string insertNewFLT_FDP = File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath("~/bin/mainfdpnew.txt"));
            foreach (var flt in newFLTs)
            {
                var qry = insertNewFLT_FDP.Replace("@flt", "'" + flt.Split('_')[0] + "'").Replace("@upd", upd).Replace("@temp", "1");
                using (SqlCommand command = new SqlCommand(qry, cnnAP))
                {
                    var r1y = command.ExecuteNonQuery();
                }
            }

            foreach (var flt in newFLTs)
            {
                var qry = insertNewFLT_FDP.Replace("@flt", "'" + flt.Split('_')[0] + "'").Replace("@upd", upd).Replace("@temp", "0");
                using (SqlCommand command = new SqlCommand(qry, cnnAP))
                {
                    var r1y = command.ExecuteNonQuery();
                }
            }

            if (newFLTs.Count > 0)
            {
                var updfdpqry = "update FDP  set TemplateId = (select top 1 ID from fdp f where f.IsTemplate = 1 and f.remark2 = fdp.remark2) where fdp.IsTemplate = 0 and remark = " + upd;
                using (SqlCommand command = new SqlCommand(updfdpqry, cnnAP))
                {
                    var r1y = command.ExecuteNonQuery();
                }

                var fdpitemqry = "INSERT INTO dbo.FDPItem (  FDPId ,FlightId ,IsSector ,IsPositioning ,IsOff ,PositionId ,RosterPositionId ,remark) "
                              + " SELECT (select top 1 Id from fdp where upd = xfdpid)  ,flightid ,1  ,0  ,0  ,pos ,rosterindex ," + upd + " from _xfdpitem where upd = " + upd;
                using (SqlCommand command = new SqlCommand(fdpitemqry, cnnAP))
                {
                    command.CommandTimeout = 1000000;
                    var r1y = command.ExecuteNonQuery();
                }
            }



            /////////////////////////////////////////////
            return Ok(true);
        }

        [Route("api/flt/ext/get/{clr}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetFMIS(int clr, DateTime from)
        {
            try
            {
                var dt = from.Date; //DateTime.Now.Date;

                //var ctx = new PPAEntities();
                //var fmis = new CrewVAEntities();
                //var query = from x in fmis.FlightInformations
                //            join 
                //            where x.DateUTC == dt
                //            select x;

                var fmis_cnn_string = "Data Source=VA.FMIS.IR,2019;Initial Catalog=CrewVA;User ID=WinUsers;Password=Crew1018!)!*";
                var ap_cnn_string = "Data Source=65.21.14.236;Initial Catalog=ppa_varesh;User ID=Vahid;Password=Atrina1359@aA";

                //var fmis_cnn_string = "Data Source=185.116.160.80;Initial Catalog=CrewCH;User ID=WinUsers;Password=Crew1018!)!*";
                //var ap_cnn_string = "Data Source=185.141.132.14;Initial Catalog=ppa_chb;User ID=chb;Password=Atrina1359@aA";



                SqlConnection cnnAP = new SqlConnection(ap_cnn_string);
                cnnAP.Open();
                if (clr == 1)
                {
                    using (SqlCommand command = new SqlCommand("delete from flightinformation where cast(std as date)='" + dt.ToString("yyyy-MM-dd") + "'", cnnAP))
                    {
                        command.CommandTimeout = 1000000;
                        var rr = command.ExecuteNonQuery();
                    }
                }



                SqlConnection cnnFMIS = new SqlConnection(fmis_cnn_string);
                cnnFMIS.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM FMISFLT", cnnAP);
                cmd.ExecuteNonQuery();


                // var fmisSql = "SELECT  DateUTC ,FltNo ,DepStn ,ArrStn ,ACType ,ACReg ,STD ,STA ,ATD ,ATA ,OffBlock ,OnBlock ,TakeOff ,OnRunway ,SaveTime ,PaxADL ,PaxCHD ,PaxINF ,TotalSeats ,OverPax ,FuelRemain ,FuelUpLift ,FuelDefuel ,FuelTotal ,FuelTaxi ,FuelTrip ,FuelUnit ,CargoWeight ,CargoPiece ,Baggage ,BagPiece ,ExtraBag ,ExtraBagPiece ,ExtraBagAmount ,CargoUnit ,FlightType ,FlightCharterer ,DelayReason ,Distance ,StationIncome ,CrewXML ,PaxXML ,DelayXML ,ExtraXML ,CargoXML ,MaintenanceXML ,Tag1 ,Tag2 ,Tag3 ,Parking ,PAXStation ,StationIncomeCurrency ,AlternateStation ,Status ,UpdateUser ,UpdateTime ,SavingTime ,Remark ,Male ,Female FROM dbo.FlightInformation "
                //              + " WHERE DateUTC='" + dt.ToString("yyyy-MM-dd") + "'";

                var fmisSql = "SELECT  DateUTC ,FltNo ,DepStn ,ArrStn ,ACType ,ACReg ,STD ,STA  FROM dbo.LEG "
                              + " WHERE DateUTC='" + dt.ToString("yyyy-MM-dd") + "'";
                SqlDataAdapter da = new SqlDataAdapter(fmisSql, cnnFMIS);
                DataSet ds = new DataSet();
                da.FillError += new FillErrorEventHandler(FillError);
                da.Fill(ds);
                var tbl = ds.Tables[0];
                var columns = tbl.Columns;
                var rows = tbl.Select();

                SqlBulkCopy sqlbc = new SqlBulkCopy(cnnAP);
                sqlbc.BulkCopyTimeout = 6000;
                sqlbc.NotifyAfter = 100000;
                //sqlbc.BatchSize = 1;
                var xxxx = sqlbc.BatchSize;

                sqlbc.DestinationTableName = "FMISFLT";
                sqlbc.WriteToServer(rows);

                var updkey = "update FMISFLT set [key]=convert(varchar, dateutc, 23)+'_'+FltNo+'_'+DepStn+'_'+ArrStn";
                SqlCommand upd1 = new SqlCommand(updkey, cnnAP);
                var r1 = upd1.ExecuteNonQuery();



                var updmvt = "update FMISFLT set OffBlock=STD,TakeOff=STD,OnBlock=STA,OnRunway=STA";
                SqlCommand updmvtcom = new SqlCommand(updmvt, cnnAP);
                var r1111 = updmvtcom.ExecuteNonQuery();





                cnnFMIS.Close();


                var newRecsTxt = "select [Key] from FMISFLT where [Key] not in (select isnull(alt5,'') from FlightInformation)";
                List<string> newKeys = new List<string>();
                using (SqlCommand command = new SqlCommand(newRecsTxt, cnnAP))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            newKeys.Add(reader.GetString(0));
                        }
                    }
                }

                if (newKeys.Count > 0)
                {
                    #region new

                    string insertNewCmd = File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath("~/bin/insertnew.txt"));
                    insertNewCmd = insertNewCmd.Replace("#rem", dt.ToString("yyyy-MM-dd"));
                    insertNewCmd = insertNewCmd.Replace("#key", string.Join(",", newKeys.Select(q => "'" + q + "'").ToList()));
                    using (SqlCommand command = new SqlCommand(insertNewCmd, cnnAP))
                    {
                        var r1y = command.ExecuteNonQuery();
                    }
                    #endregion
                }

                //////////////////////////////////
                //////////////////////////////////
                var updRecsTxt = "select ID from ViewFMISFLT  where DateUTC='" + dt.ToString("yyyy-MM-dd") + "' and (STD<>STD1  or STA<>STA1 or Takeoff<>Takeoff1 or offblock<>offblock1 or onblock<>onblock1 or landing<>landing1 or reg<>reg1 or StatusId1<>statusid or depstn<>depstn1 or arrstn<>arrstn1)";
                List<string> updKeys = new List<string>();
                using (SqlCommand command = new SqlCommand(updRecsTxt, cnnAP))
                {
                    command.CommandTimeout = 1000000;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            updKeys.Add(reader.GetInt32(0).ToString());
                        }
                    }
                }
                if (updKeys.Count > 0)
                {
                    #region new

                    string insertNewCmd = File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath("~/bin/update.txt"));
                    insertNewCmd = insertNewCmd.Replace("#ID", string.Join(",", updKeys.Select(q => q.ToString()).ToList()));

                    using (SqlCommand command = new SqlCommand(insertNewCmd, cnnAP))
                    {
                        command.CommandTimeout = 100000;
                        var r1x = command.ExecuteNonQuery();
                    }
                    #endregion
                }

                cnnAP.Close();

                //ctx.OFPSkyPuters.Add(entity);
                //ctx.SaveChanges();
                return Ok(true);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "  INNER: " + ex.InnerException.Message;
                return Ok(msg);
            }



        }

        [Route("api/flt/ext/get/status/{clr}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetFMISStatus(int clr, DateTime from)
        {
            try
            {
                var dt = from.Date; //DateTime.Now.Date;

                //var ctx = new PPAEntities();
                //var fmis = new CrewVAEntities();
                //var query = from x in fmis.FlightInformations
                //            join 
                //            where x.DateUTC == dt
                //            select x;

                var fmis_cnn_string = "Data Source=VA.FMIS.IR,2019;Initial Catalog=CrewVA;User ID=WinUsers;Password=Crew1018!)!*";
                //var fmis_cnn_string = "Data Source=65.21.100.132;Initial Catalog=VARESH;User ID=sa;Password=Atrina1359";
                var ap_cnn_string = "Data Source=65.21.14.236;Initial Catalog=ppa_varesh;User ID=Vahid;Password=Atrina1359@aA";

                //var fmis_cnn_string = "Data Source=185.116.160.80;Initial Catalog=CrewCH;User ID=WinUsers;Password=Crew1018!)!*";
                //var ap_cnn_string = "Data Source=185.141.132.14;Initial Catalog=ppa_chb;User ID=chb;Password=Atrina1359@aA";



                SqlConnection cnnAP = new SqlConnection(ap_cnn_string);
                cnnAP.Open();
                if (clr == 1)
                {
                    using (SqlCommand command = new SqlCommand("delete from flightinformation where cast(std as date)='" + dt.ToString("yyyy-MM-dd") + "'", cnnAP))
                    {
                        command.CommandTimeout = 1000000;
                        var rr = command.ExecuteNonQuery();
                    }
                }



                SqlConnection cnnFMIS = new SqlConnection(fmis_cnn_string);
                cnnFMIS.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM FMISFLT", cnnAP);
                cmd.ExecuteNonQuery();


                var fmisSql = "SELECT  DateUTC ,FltNo ,DepStn ,ArrStn ,ACType ,ACReg ,STD ,STA ,ATD ,ATA ,OffBlock ,OnBlock ,TakeOff ,OnRunway ,SaveTime ,PaxADL ,PaxCHD ,PaxINF ,TotalSeats ,OverPax ,FuelRemain ,FuelUpLift ,FuelDefuel ,FuelTotal ,FuelTaxi ,FuelTrip ,FuelUnit ,CargoWeight ,CargoPiece ,Baggage ,BagPiece ,ExtraBag ,ExtraBagPiece ,ExtraBagAmount ,CargoUnit ,FlightType ,FlightCharterer ,DelayReason ,Distance ,StationIncome ,CrewXML ,PaxXML ,DelayXML ,ExtraXML ,CargoXML ,MaintenanceXML ,Tag1 ,Tag2 ,Tag3 ,Parking ,PAXStation ,StationIncomeCurrency ,AlternateStation ,Status ,UpdateUser ,UpdateTime ,SavingTime ,Remark ,Male ,Female FROM dbo.FlightInformation "
                             + " WHERE DateUTC='" + dt.ToString("yyyy-MM-dd") + "'";

                // var fmisSql = "SELECT  DateUTC ,FltNo ,DepStn ,ArrStn ,ACType ,ACReg ,STD ,STA  FROM dbo.LEG "
                //              + " WHERE DateUTC='" + dt.ToString("yyyy-MM-dd") + "'";
                SqlDataAdapter da = new SqlDataAdapter(fmisSql, cnnFMIS);
                DataSet ds = new DataSet();
                da.FillError += new FillErrorEventHandler(FillError);
                da.Fill(ds);
                var tbl = ds.Tables[0];
                var columns = tbl.Columns;
                var rows = tbl.Select();

                SqlBulkCopy sqlbc = new SqlBulkCopy(cnnAP);
                sqlbc.BulkCopyTimeout = 6000;
                sqlbc.NotifyAfter = 100000;
                //sqlbc.BatchSize = 1;
                var xxxx = sqlbc.BatchSize;

                sqlbc.DestinationTableName = "FMISFLT";
                sqlbc.WriteToServer(rows);

                var updkey = "update FMISFLT set [key]=convert(varchar, dateutc, 23)+'_'+FltNo+'_'+DepStn+'_'+ArrStn";
                SqlCommand upd1 = new SqlCommand(updkey, cnnAP);
                var r1 = upd1.ExecuteNonQuery();



                //   var updmvt = "update FMISFLT set OffBlock=STD,TakeOff=STD,OnBlock=STA,OnRunway=STA";
                //   SqlCommand updmvtcom = new SqlCommand(updmvt, cnnAP);
                //   var r1111 = updmvtcom.ExecuteNonQuery();





                cnnFMIS.Close();


                var newRecsTxt = "select [Key] from FMISFLT where [Key] not in (select isnull(alt5,'') from FlightInformation)";
                List<string> newKeys = new List<string>();
                using (SqlCommand command = new SqlCommand(newRecsTxt, cnnAP))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            newKeys.Add(reader.GetString(0));
                        }
                    }
                }

                if (newKeys.Count > 0)
                {
                    #region new

                    string insertNewCmd = File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath("~/bin/insertnew.txt"));
                    insertNewCmd = insertNewCmd.Replace("#rem", dt.ToString("yyyy-MM-dd"));
                    insertNewCmd = insertNewCmd.Replace("#key", string.Join(",", newKeys.Select(q => "'" + q + "'").ToList()));
                    using (SqlCommand command = new SqlCommand(insertNewCmd, cnnAP))
                    {
                        var r1y = command.ExecuteNonQuery();
                    }
                    #endregion
                }

                //////////////////////////////////
                //////////////////////////////////
                var updRecsTxt = "select ID from ViewFMISFLT  where DateUTC='" + dt.ToString("yyyy-MM-dd") + "' and (STD<>STD1  or STA<>STA1 or Takeoff<>Takeoff1 or offblock<>offblock1 or onblock<>onblock1 or landing<>landing1 or reg<>reg1 or StatusId1<>statusid or depstn<>depstn1 or arrstn<>arrstn1 or PaxAdult1<>PaxAdult or PaxChild1<>PaxChild or PaxInfant1<>PaxInfant)";
                List<string> updKeys = new List<string>();
                using (SqlCommand command = new SqlCommand(updRecsTxt, cnnAP))
                {
                    command.CommandTimeout = 1000000;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            updKeys.Add(reader.GetInt32(0).ToString());
                        }
                    }
                }
                if (updKeys.Count > 0)
                {
                    #region new

                    string insertNewCmd = File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath("~/bin/update.txt"));
                    insertNewCmd = insertNewCmd.Replace("#ID", string.Join(",", updKeys.Select(q => q.ToString()).ToList()));

                    using (SqlCommand command = new SqlCommand(insertNewCmd, cnnAP))
                    {
                        command.CommandTimeout = 100000;
                        var r1x = command.ExecuteNonQuery();
                    }
                    #endregion
                }

                cnnAP.Close();

                //ctx.OFPSkyPuters.Add(entity);
                //ctx.SaveChanges();
                return Ok(true);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "  INNER: " + ex.InnerException.Message;
                return Ok(msg);
            }



        }




        protected void FillError(object sender, FillErrorEventArgs args)
        {
            //var str=
            var str = args.Errors.Message + " ## " + args.Values[0].ToString();
            //logError(str);
            args.Continue = true;

        }

        //[Route("api/skyputer/get")]
        //[AcceptVerbs("GET")]
        //public IHttpActionResult GetSkyputer(string key, string plan)
        //{

        //    if (string.IsNullOrEmpty( key))
        //        return BadRequest("Authorization key not found.");
        //    if (string.IsNullOrEmpty( plan))
        //        return BadRequest("Plan cannot be empty.");
        //    if ( key != "Skyputer@1359#")
        //        return BadRequest("Authorization key is wrong.");
        //    var entity = new OFPSkyPuter()
        //    {
        //        OFP =  plan,

        //    };
        //    var ctx = new PPAEntities();
        //    ctx.OFPSkyPuters.Add(entity);
        //    ctx.SaveChanges();
        //    return Ok(true);


        //}

        public class skyputer
        {
            public string plan { get; set; }
            public string fltno { get; set; }
            public string date { get; set; }

            public string key { get; set; }
        }


    }
}
