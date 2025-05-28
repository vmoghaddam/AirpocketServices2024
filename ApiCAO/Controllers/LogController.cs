using ApiCAO.Models;
using ApiCAO.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ApiCAO.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class LogController : ApiController
    {
        
        public class mvtDelayObj
        {
            public int amount { get; set; }
            public string reasonCode { get; set; }
        }
        public class mvtFlightNumberObj
        {
            public string carrier { get; set; }
            public int number { get; set; }
            public string postFix { get; set; }
        }
        public class mvtPassengerObj
        {
            public int male { get; set; }
            public int female { get; set; }
            public int child { get; set; }
            public int infant { get; set; }
            public int adult { get; set; }
        }
        public class mvtObj
        {
            public string acRegister { get; set; }
            public string destination { get; set; }
            public Int64 flightDate { get; set; }
            public Int64 landingDate { get; set; }
            public string messageType { get; set; }
            public Int64 offBlockDate { get; set; }
            public Int64 onBlockDate { get; set; }
            public string origin { get; set; }
            public Int64 takeOffDate { get; set; }
            public List<mvtDelayObj> mvtDelays { get; set; }
            public mvtFlightNumberObj mvtFlightNumber { get; set; }
            public mvtPassengerObj mvtPassenger { get; set; }
        }



        public class flightInfoBaggage
        {
            public string bagPiece { get; set; }
            public string bagWeight { get; set; }
            public string cargoPiece { get; set; }
            public string cargoWeight { get; set; }
            public string excessBag { get; set; }
            public string excessPiece { get; set; }
            public string unit { get; set; }
        }

        public class flightInfoCrew
        {
            public string code { get; set; }
            public string fullName { get; set; }
            public string pos { get; set; }
        }
        public class flightInfoFuel
        {
            public string defuel { get; set; }
            public string fpTrip { get; set; }
            public string fuelUnit { get; set; }
            public string remain { get; set; }
            public string taxi { get; set; }
            public string total { get; set; }
            public string trip { get; set; }
            public string uplift { get; set; }
        }
        public class flightInfoMain
        {
            public string acRegister { get; set; }
            public string acType { get; set; }
            public string carrier { get; set; }
            public string destination { get; set; }
            public string flightId { get; set; }
            public string flightNumber { get; set; }
            public string flightStatus { get; set; }
            public string origin { get; set; }
        }
        public class flightInfoPax
        {
            public string adult { get; set; }
            public string child { get; set; }
            public string female { get; set; }
            public string infant { get; set; }
            public string male { get; set; }
            public string overPax { get; set; }
            public string stationPax { get; set; }
            public string totalPax { get; set; }
            public string totalSeats { get; set; }
        }
        public class flightInfoTimes
        {
            public Int64 landing { get; set; }
            public Int64 offBlock { get; set; }
            public Int64 onBlock { get; set; }
            public Int64 sta { get; set; }
            public Int64 std { get; set; }
            public Int64 takeOff { get; set; }
        }

        public class ldm
        {
            public string cabinNo { get; set; }
            public string cockpitNo { get; set; }
            public string config { get; set; }
            public string crewDHNo { get; set; }
            public string equipment { get; set; }


            public string mail { get; set; }
            public string supplementaryInformationText { get; set; }
            public List<ldmpax> ldmPaxes { get; set; }
            public List<ldmCompartment> ldmCompartments { get; set; }
        }

        public class ldmCompartment
        {
            public string load { get; set; }
            public string name { get; set; }
        }
        public class ldmpax
        {
            public string kind { get; set; }
            public string number { get; set; }
        }

        public class mvtDelay
        {
            public int amount { get; set; }
            public string reasonCode { get; set; }
        }

        public class mvt
        {
            public List<mvtDelay> mvtDelays { get; set; }
        }

        public class flight
        {
            public flightInfoBaggage flightInfoBaggage { get; set; }
            public List<flightInfoCrew> flightInfoCrews { get; set; }
            public flightInfoFuel flightInfoFuel { get; set; }
            public flightInfoMain flightInfoMain { get; set; }
            public flightInfoPax flightInfoPax { get; set; }
            public flightInfoTimes flightInfoTimes { get; set; }
        }

        public class caoMsg
        {
            public flight flight { get; set; }
            public ldm ldm { get; set; }
            public mvt mvt { get; set; }
        }

        [Route("api/cao/mvt2/{id}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetCAOMVT(int id)
        {
            try
            {
                string iata = ConfigurationManager.AppSettings["iata"];
                string icao = ConfigurationManager.AppSettings["icao"];
                ppa_entities context = new ppa_entities();
                var flt = context.ViewLegTimes.Where(q => q.ID == id).FirstOrDefault();
                if (flt.FlightStatusID != 2 && !(flt.FlightStatusID == 15 || flt.FlightStatusID == 3))
                    return Ok();
                CaoMVTLog log = new CaoMVTLog()
                {
                    DateCreate = DateTime.Now,
                    FlightId = id,
                };
                context.CaoMVTLogs.Add(log);
                var delays = context.ViewFlightDelayCodes.Where(q => q.FlightId == id).OrderBy(q => q.Code).Select(q => new { q.Code, q.HH, q.MM }).ToList();

                var _type = flt.FlightStatusID == 2 ? "DEPARTURE" : "ARRIVAL";
                log.MessageType = _type;

                var mvt = new mvtObj()
                {
                    acRegister = flt.Register,
                    destination = flt.ToAirportIATA,
                    flightDate = Convert.ToInt64(((DateTimeOffset)flt.STDDay).ToUnixTimeSeconds()),
                    landingDate = Convert.ToInt64(((DateTimeOffset)flt.Landing).ToUnixTimeSeconds()),
                    messageType = _type,
                    offBlockDate = Convert.ToInt64(((DateTimeOffset)flt.ChocksOut).ToUnixTimeSeconds()),
                    onBlockDate = Convert.ToInt64(((DateTimeOffset)flt.ChocksIn).ToUnixTimeSeconds()),
                    takeOffDate = Convert.ToInt64(((DateTimeOffset)flt.Takeoff).ToUnixTimeSeconds()),
                    origin = flt.FromAirportIATA,
                    mvtDelays = new List<mvtDelayObj>(),
                    mvtFlightNumber = new mvtFlightNumberObj()
                    {
                        carrier = iata,
                        number = Convert.ToInt32(flt.FlightNumber.ToLower().Replace("a", "").Replace("b", "")),
                        postFix = icao,

                    },
                    mvtPassenger = new mvtPassengerObj()
                    {
                        child = flt.PaxChild == null ? 0 : (int)flt.PaxChild,
                        female = 0,
                        male = flt.PaxAdult == null ? 0 : (int)flt.PaxAdult,
                        infant = flt.PaxInfant == null ? 0 : (int)flt.PaxInfant,
                    }


                };
                if (delays.Count > 0)
                {

                    foreach (var x in delays)
                    {
                        mvt.mvtDelays.Add(new mvtDelayObj()
                        {
                            amount = (x.HH ?? 0) * 60 + (x.MM ?? 0),
                            reasonCode = x.Code
                        });
                    }


                }
                var mvts = new List<mvtObj>();
                mvts.Add(mvt);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://cao.raman-it.com/mvt");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    //string json = new JavaScriptSerializer().Serialize(new
                    //{
                    //    user = "Foo",
                    //    password = "Baz"
                    //});
                    string json = JsonConvert.SerializeObject(mvts);
                    log.Message = json;
                    streamWriter.Write(json);
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    log.Response = result;
                }
                context.SaveChanges();
                return Ok(true);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   INNER: " + ex.InnerException.Message;
                return Ok(msg);
            }



        }


        [Route("api/cao/mvt/{id}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetCAOMSG(int id)
        {
            try
            {
                string iata = ConfigurationManager.AppSettings["iata"];
                 string icao = ConfigurationManager.AppSettings["icao"];
                 string key = ConfigurationManager.AppSettings["caoapikey"];
                ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                //caspian
                //var iata = "IV";
                // var icao = "CPN";
                // string key = "d41668c393974880aae19ef35f5099eb";

                ppa_entities context = new ppa_entities(); 
                var flt = context.ViewCaos.Where(q => q.ID == id).FirstOrDefault(); // context.ViewLegTimes.Where(q => q.ID == id).FirstOrDefault();
                if (flt.FlightStatusID != 2 && !(flt.FlightStatusID == 15 || flt.FlightStatusID == 3))
                    return Ok();


                CaoMVTLog log = new CaoMVTLog()
                {
                    DateCreate = DateTime.Now,
                    FlightId = id,
                };
                context.CaoMVTLogs.Add(log);
                var delays = context.ViewFlightDelayCodes.Where(q => q.FlightId == id).OrderBy(q => q.Code).Select(q => new { q.Code, q.HH, q.MM }).ToList();

                var _type = flt.FlightStatusID == 2 ? "DEPARTURE" : "ARRIVAL";
                log.MessageType = _type;

                var msg = new caoMsg();
                msg.flight = new flight();
                msg.flight.flightInfoMain = new flightInfoMain()
                {
                    acRegister = flt.Register,
                    acType = flt.AircraftType,
                    carrier = icao,
                    destination = flt.ToAirportIATA,
                    origin = flt.FromAirportIATA,
                    flightId = "AP-"+icao + "-" + flt.ID.ToString(),
                    flightNumber = flt.FlightNumber,
                    flightStatus = flt.FlightStatus,


                };
                msg.flight.flightInfoPax = new flightInfoPax()
                {
                    adult = flt.PaxAdult.ToString(),
                    child = flt.PaxChild.ToString(),
                    infant = flt.PaxInfant.ToString(),
                    totalSeats = flt.TotalSeat.ToString(),
                    totalPax = (flt.PaxAdult ?? 0 + flt.PaxChild ?? 0).ToString(),
                    overPax = (flt.TotalSeat ?? 0 - flt.PaxAdult ?? 0 - flt.PaxChild ?? 0).ToString()
                };
                msg.flight.flightInfoBaggage = new flightInfoBaggage()
                {
                    bagPiece = flt.BaggageCount.ToString(),
                    bagWeight = flt.BaggageWeight.ToString(),
                    cargoPiece = flt.CargoCount.ToString(),
                    cargoWeight = flt.CargoWeight.ToString(),
                    unit = (flt.AircraftType.ToLower().Contains("m") ? "lbs" : "kg"),
                };
                msg.flight.flightInfoFuel = new flightInfoFuel()
                {
                    fuelUnit = (flt.AircraftType.ToLower().Contains("m") ? "lbs" : "kg"),
                    taxi = flt.OFPTAXIFUEL.ToString(),
                    trip = flt.FuelUsed.ToString(),
                    fpTrip = flt.OFPTRIPFUEL.ToString(),
                    total = flt.FuelTotal.ToString(),
                    uplift = flt.FuelUplift.ToString(),
                    remain = (flt.FuelTotal - flt.FuelUsed).ToString(),
                };
                msg.flight.flightInfoTimes = new flightInfoTimes()
                {
                    std = Convert.ToInt64(((DateTimeOffset)flt.STD).ToUnixTimeSeconds()),
                    sta = Convert.ToInt64(((DateTimeOffset)flt.STA).ToUnixTimeSeconds()),
                    offBlock = Convert.ToInt64(((DateTimeOffset)flt.BlockOffStation).ToUnixTimeSeconds()),
                    onBlock = Convert.ToInt64(((DateTimeOffset)flt.BlockOnStation).ToUnixTimeSeconds()),
                    takeOff = Convert.ToInt64(((DateTimeOffset)flt.TakeoffStation).ToUnixTimeSeconds()),
                    landing = Convert.ToInt64(((DateTimeOffset)flt.LandingStation).ToUnixTimeSeconds()),
                };
                msg.flight.flightInfoCrews = new List<flightInfoCrew>();
                var crew = context.XFlightCrews.Where(q => q.FlightId == flt.ID).OrderBy(q => q.GroupOrder).ToList();
                foreach (var c in crew)
                    msg.flight.flightInfoCrews.Add(new flightInfoCrew()
                    {
                        code = "****",
                        fullName = "****",
                        pos = c.Position,
                    });
                msg.ldm = new ldm()
                {
                    ldmCompartments = new List<ldmCompartment>() { new ldmCompartment() },
                    ldmPaxes = new List<ldmpax>() { new ldmpax() },
                };
                msg.mvt = new mvt()
                {
                    mvtDelays = new List<mvtDelay>()
                };

                if (delays.Count > 0)
                {

                    foreach (var x in delays)
                    {
                        msg.mvt.mvtDelays.Add(new  mvtDelay()
                        {
                            amount = (x.HH ?? 0) * 60 + (x.MM ?? 0),
                            reasonCode = x.Code
                        });
                    }


                }


                //var mvt = new mvtObj()
                //{
                //    acRegister = flt.Register,
                //    destination = flt.ToAirportIATA,
                //    flightDate = Convert.ToInt64(((DateTimeOffset)flt.STDDay).ToUnixTimeSeconds()),
                //    landingDate = Convert.ToInt64(((DateTimeOffset)flt.Landing).ToUnixTimeSeconds()),
                //    messageType = _type,
                //    offBlockDate = Convert.ToInt64(((DateTimeOffset)flt.ChocksOut).ToUnixTimeSeconds()),
                //    onBlockDate = Convert.ToInt64(((DateTimeOffset)flt.ChocksIn).ToUnixTimeSeconds()),
                //    takeOffDate = Convert.ToInt64(((DateTimeOffset)flt.Takeoff).ToUnixTimeSeconds()),
                //    origin = flt.FromAirportIATA,
                //    mvtDelays = new List<mvtDelayObj>(),
                //    mvtFlightNumber = new mvtFlightNumberObj()
                //    {
                //        carrier = iata,
                //        number = Convert.ToInt32(flt.FlightNumber.ToLower().Replace("a", "").Replace("b", "")),
                //        postFix = icao,

                //    },
                //    mvtPassenger = new mvtPassengerObj()
                //    {
                //        child = flt.PaxChild == null ? 0 : (int)flt.PaxChild,
                //        female = 0,
                //        male = flt.PaxAdult == null ? 0 : (int)flt.PaxAdult,
                //        infant = flt.PaxInfant == null ? 0 : (int)flt.PaxInfant,
                //    }


                //};
                //if (delays.Count > 0)
                //{

                //    foreach (var x in delays)
                //    {
                //        mvt.mvtDelays.Add(new mvtDelayObj()
                //        {
                //            amount = (x.HH ?? 0) * 60 + (x.MM ?? 0),
                //            reasonCode = x.Code
                //        });
                //    }


                //}
                //var mvts = new List<mvtObj>();
                //mvts.Add(mvt);
                
                var msg_obj = new List<caoMsg>() { msg };
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(/*"https://cao.raman-it.com/mvt"*/"https://caadc.cao.ir:443/api/flight");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                 httpWebRequest.Headers.Add("api-key", key );

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    //string json = new JavaScriptSerializer().Serialize(new
                    //{
                    //    user = "Foo",
                    //    password = "Baz"
                    //});
                    string json = JsonConvert.SerializeObject(msg_obj);
                    log.Message = json;
                    streamWriter.Write(json);
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    log.Response = result;
                }
                context.SaveChanges();
                return Ok(true);
            }
            catch (Exception ex)
            {
                
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();


                var msg = ex.StackTrace+"   "+line + "     "+ ex.Message;
                if (ex.InnerException != null)
                    msg += "   INNER: " + ex.InnerException.Message;
                return Ok(msg);
            }



        }

        [Route("api/cao/mvt/all/")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetCAOMSGAll()
        {
            try
            {
                string iata = ConfigurationManager.AppSettings["iata"];
                string icao = ConfigurationManager.AppSettings["icao"];
                string key = ConfigurationManager.AppSettings["caoapikey"];
                ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                //caspian
                //var iata = "IV";
                // var icao = "CPN";
                // string key = "d41668c393974880aae19ef35f5099eb";

                ppa_entities context = new ppa_entities();
                //var flt = context.ViewCaos.Where(q => q.ID == id).FirstOrDefault(); // context.ViewLegTimes.Where(q => q.ID == id).FirstOrDefault();
                var dt = new DateTime(2024, 2, 8);
                var flts = context.ViewCaos.Where(q => q.STD >= dt && (q.FlightStatusID==3 || q.FlightStatusID==15 || q.FlightStatusID==7 || q.FlightStatusID==17)).ToList();
                //if (flt.FlightStatusID != 2 && !(flt.FlightStatusID == 15 || flt.FlightStatusID == 3))
                //    return Ok();
                var flt_ids=flts.Select(q=>q.ID).ToList();
                var _delays = context.ViewFlightDelayCodes.Where(q=>flt_ids.Contains(q.FlightId)).ToList();
                int _c = 0;
                foreach (var flt in flts)
                {
                    _c++;
                    try
                    {
                        var id = flt.ID;
                        CaoMVTLog log = new CaoMVTLog()
                        {
                            DateCreate = DateTime.Now,
                            FlightId = id,
                        };
                        context.CaoMVTLogs.Add(log);
                        var delays = _delays.Where(q => q.FlightId == id).OrderBy(q => q.Code).Select(q => new { q.Code, q.HH, q.MM }).ToList();

                        var _type = flt.FlightStatusID == 2 ? "DEPARTURE" : "ARRIVAL";
                        log.MessageType = _type;

                        var msg = new caoMsg();
                        msg.flight = new flight();
                        msg.flight.flightInfoMain = new flightInfoMain()
                        {
                            acRegister = flt.Register,
                            acType = flt.AircraftType,
                            carrier = icao,
                            destination = flt.ToAirportIATA,
                            origin = flt.FromAirportIATA,
                            flightId = "AP-" + icao + "-" + flt.ID.ToString(),
                            flightNumber = flt.FlightNumber,
                            flightStatus = flt.FlightStatus,


                        };
                        msg.flight.flightInfoPax = new flightInfoPax()
                        {
                            adult = flt.PaxAdult.ToString(),
                            child = flt.PaxChild.ToString(),
                            infant = flt.PaxInfant.ToString(),
                            totalSeats = flt.TotalSeat.ToString(),
                            totalPax = (flt.PaxAdult ?? 0 + flt.PaxChild ?? 0).ToString(),
                            overPax = (flt.TotalSeat ?? 0 - flt.PaxAdult ?? 0 - flt.PaxChild ?? 0).ToString()
                        };
                        msg.flight.flightInfoBaggage = new flightInfoBaggage()
                        {
                            bagPiece = flt.BaggageCount.ToString(),
                            bagWeight = flt.BaggageWeight.ToString(),
                            cargoPiece = flt.CargoCount.ToString(),
                            cargoWeight = flt.CargoWeight.ToString(),
                            unit = (flt.AircraftType.ToLower().Contains("m") ? "lbs" : "kg"),
                        };
                        msg.flight.flightInfoFuel = new flightInfoFuel()
                        {
                            fuelUnit = (flt.AircraftType.ToLower().Contains("m") ? "lbs" : "kg"),
                            taxi = flt.OFPTAXIFUEL.ToString(),
                            trip = flt.FuelUsed.ToString(),
                            fpTrip = flt.OFPTRIPFUEL.ToString(),
                            total = flt.FuelTotal.ToString(),
                            uplift = flt.FuelUplift.ToString(),
                            remain = (flt.FuelTotal - flt.FuelUsed).ToString(),
                        };
                        msg.flight.flightInfoTimes = new flightInfoTimes()
                        {
                            std = Convert.ToInt64(((DateTimeOffset)flt.STD).ToUnixTimeSeconds()),
                            sta = Convert.ToInt64(((DateTimeOffset)flt.STA).ToUnixTimeSeconds()),
                            offBlock = Convert.ToInt64(((DateTimeOffset)flt.BlockOffStation).ToUnixTimeSeconds()),
                            onBlock = Convert.ToInt64(((DateTimeOffset)flt.BlockOnStation).ToUnixTimeSeconds()),
                            takeOff = Convert.ToInt64(((DateTimeOffset)flt.TakeoffStation).ToUnixTimeSeconds()),
                            landing = Convert.ToInt64(((DateTimeOffset)flt.LandingStation).ToUnixTimeSeconds()),
                        };
                        msg.flight.flightInfoCrews = new List<flightInfoCrew>();
                        var crew = context.XFlightCrews.Where(q => q.FlightId == flt.ID).OrderBy(q => q.GroupOrder).ToList();
                        foreach (var c in crew)
                            msg.flight.flightInfoCrews.Add(new flightInfoCrew()
                            {
                                code = "****",
                                fullName = "****",
                                pos = c.Position,
                            });
                        msg.ldm = new ldm()
                        {
                            ldmCompartments = new List<ldmCompartment>() { new ldmCompartment() },
                            ldmPaxes = new List<ldmpax>() { new ldmpax() },
                        };
                        msg.mvt = new mvt()
                        {
                            mvtDelays = new List<mvtDelay>()
                        };

                        if (delays.Count > 0)
                        {

                            foreach (var x in delays)
                            {
                                msg.mvt.mvtDelays.Add(new mvtDelay()
                                {
                                    amount = (x.HH ?? 0) * 60 + (x.MM ?? 0),
                                    reasonCode = x.Code
                                });
                            }


                        }


                        //var mvt = new mvtObj()
                        //{
                        //    acRegister = flt.Register,
                        //    destination = flt.ToAirportIATA,
                        //    flightDate = Convert.ToInt64(((DateTimeOffset)flt.STDDay).ToUnixTimeSeconds()),
                        //    landingDate = Convert.ToInt64(((DateTimeOffset)flt.Landing).ToUnixTimeSeconds()),
                        //    messageType = _type,
                        //    offBlockDate = Convert.ToInt64(((DateTimeOffset)flt.ChocksOut).ToUnixTimeSeconds()),
                        //    onBlockDate = Convert.ToInt64(((DateTimeOffset)flt.ChocksIn).ToUnixTimeSeconds()),
                        //    takeOffDate = Convert.ToInt64(((DateTimeOffset)flt.Takeoff).ToUnixTimeSeconds()),
                        //    origin = flt.FromAirportIATA,
                        //    mvtDelays = new List<mvtDelayObj>(),
                        //    mvtFlightNumber = new mvtFlightNumberObj()
                        //    {
                        //        carrier = iata,
                        //        number = Convert.ToInt32(flt.FlightNumber.ToLower().Replace("a", "").Replace("b", "")),
                        //        postFix = icao,

                        //    },
                        //    mvtPassenger = new mvtPassengerObj()
                        //    {
                        //        child = flt.PaxChild == null ? 0 : (int)flt.PaxChild,
                        //        female = 0,
                        //        male = flt.PaxAdult == null ? 0 : (int)flt.PaxAdult,
                        //        infant = flt.PaxInfant == null ? 0 : (int)flt.PaxInfant,
                        //    }


                        //};
                        //if (delays.Count > 0)
                        //{

                        //    foreach (var x in delays)
                        //    {
                        //        mvt.mvtDelays.Add(new mvtDelayObj()
                        //        {
                        //            amount = (x.HH ?? 0) * 60 + (x.MM ?? 0),
                        //            reasonCode = x.Code
                        //        });
                        //    }


                        //}
                        //var mvts = new List<mvtObj>();
                        //mvts.Add(mvt);

                        var msg_obj = new List<caoMsg>() { msg };
                        var httpWebRequest = (HttpWebRequest)WebRequest.Create(/*"https://cao.raman-it.com/mvt"*/"https://caadc.cao.ir:443/api/flight");
                        httpWebRequest.ContentType = "application/json";
                        httpWebRequest.Method = "POST";
                        httpWebRequest.Headers.Add("api-key", key);

                        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                        {
                            //string json = new JavaScriptSerializer().Serialize(new
                            //{
                            //    user = "Foo",
                            //    password = "Baz"
                            //});
                            string json = JsonConvert.SerializeObject(msg_obj);
                            log.Message = json;
                            streamWriter.Write(json);
                        }

                        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();
                            log.Response = result;
                        }

                        System.Threading.Thread.Sleep(500);
                    }
                    catch (Exception exxxs)
                    {
                        var xxmsg =exxxs.Message;
                    }
                ////dfdfdfdfdfd
                
                
                
                
                }
               
                context.SaveChanges();
                return Ok(true);
            }
            catch (Exception ex)
            {

                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();


                var msg = ex.StackTrace + "   " + line + "     " + ex.Message;
                if (ex.InnerException != null)
                    msg += "   INNER: " + ex.InnerException.Message;
                return Ok(msg);
            }



        }




        public class AuthInfo
        {
            public string userName { get; set; }
            public string password { get; set; }
        }
        [Route("api/flight/status/{username}/{password}/{date}/{no}")]
        [AcceptVerbs("GET")]
        ///<summary>
        ///Get Status Of Flight
        ///</summary>
        ///<remarks>


        ///</remarks>
        public async Task<IHttpActionResult> PostFlightStatus(string username, string password, string date, string no)
        {
            try
            {
                if (!(username == "pnlairpocket" && password == "Pnl1234Za"))
                    return BadRequest("Authentication Failed");

                no = no.PadLeft(4, '0');
                List<int> prts = new List<int>();
                try
                {
                    prts = date.Split('-').Select(q => Convert.ToInt32(q)).ToList();
                }
                catch (Exception ex)
                {
                    return BadRequest("Incorrect Date");
                }

                if (prts.Count != 3)
                    return BadRequest("Incorrect Date");
                if (prts[0] < 1300)
                    return BadRequest("Incorrect Date (Year)");
                //if (prts[1] < 1 || prts[1]>12)
                //    return BadRequest("Incorrect Date (Month)");
                //if (prts[2] < 1 || prts[1] > 31)
                //    return BadRequest("Incorrect Date (Day)");

                System.Globalization.PersianCalendar pc = new System.Globalization.PersianCalendar();
                var gd = (pc.ToDateTime(prts[0], prts[1], prts[2], 0, 0, 0, 0)).Date;
                var context = new ppa_entities();
                var flight = await context.ExpFlights.Where(q => q.DepartureDay == gd && q.FlightNo == no).FirstOrDefaultAsync();
                if (flight == null)
                    return BadRequest("Flight Not Found");
                var delay = (((DateTime)flight.Departure) - ((DateTime)flight.STD)).TotalMinutes;
                if (delay < 0)
                    delay = 0;
                var result = new
                {
                    flightId = flight.Id,
                    flightNo = flight.FlightNo,
                    date = flight.DepartureDay,
                    departure = flight.DepartureLocal,
                    arrival = flight.ArrivalLocal,
                    departureUTC = flight.Departure,
                    arrivalUTC = flight.Arrival,
                    status = flight.FlightStatus,
                    statusId = flight.FlightStatusId,
                    origin = flight.Origin,
                    destination = flight.Destination,
                    aircraftType = flight.AircraftType,
                    register = flight.Register,
                    isDelayed = delay > 0,
                    delay

                };
                return Ok(result);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "    Inner Exception:" + ex.InnerException.Message;
                return BadRequest(msg);
            }








        }

    }
}
