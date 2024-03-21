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
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ApiCAO.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class NiraController : ApiController
    {

        
        [Route("api/nira/conflicts/{dfrom}/{dto}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetNiraConflicts(DateTime dfrom, DateTime dto)
        {
            try
            {
                var _dfrom = dfrom.Date.ToString("yyyy-MM-dd");
                var _dto = dto.Date.ToString("yyyy-MM-dd");

                string nira_url = "http://iv.nirasoft.ir:882/NRSCWS.jsp?ModuleType=SP&ModuleName=FlightsForAirPocket"
                    + "&OfficeUser=Thr003.airpocket&OfficePass=nira123"
                    + "&FlightNo=&FromDate=" + _dfrom + "&ToDate=" + _dto;

                List<NRSCRSFlightDataX> nira_flights = new List<NRSCRSFlightDataX>();

                using (WebClient client = new WebClient())
                {
                    client.Headers["Content-type"] = "application/json";
                    client.Encoding = Encoding.UTF8;
                    string json = client.DownloadString(nira_url);
                    var obj = JsonConvert.DeserializeObject<NRSCWSResultX>(json);
                    nira_flights = obj.NRSFlightsForAirPocket;
                    foreach (var x in nira_flights)
                    {
                        x.AircraftCode = string.IsNullOrEmpty(x.AircraftCode) ? "" : x.AircraftCode.Replace("EP-", "");
                        if (!string.IsNullOrEmpty(x.FlightNo))
                        {
                            if (x.FlightNo.Length == 2)
                                x.FlightNo = "00" + x.FlightNo;
                            if (x.FlightNo.Length == 3)
                                x.FlightNo = "0" + x.FlightNo;
                        }

                        x.DepartureDateTimeUTC = x.DepartureDateTime.AddMinutes(-210);
                        x.ArrivalDateTimeUTC = x.ArrivalDateTime.AddMinutes(-210);

                    }


                }

                var df = dfrom.Date;
                var dt = dto.Date.AddDays(1);
                ppa_entities context = new ppa_entities();
                var flights = context.ViewLegTimes.Where(q => q.STDLocal >= df && q.STDLocal < dt && (q.FlightStatusID == 1 || q.FlightStatusID == 5)).OrderBy(q => q.Register).ThenBy(q => q.STD).ToList();


                List<NiraConflictResult> conflictResult = new List<NiraConflictResult>();



                foreach (var aflt in flights)
                {
                    //var niraflt = niraFlights.FirstOrDefault(q => q.FlightNo.PadLeft(4, '0') == aflt.FlightNumber && q.STDDay == ((DateTime)aflt.STDLocal).Date);
                    var nira_flight = nira_flights.Where(q => q.FleetWatchKey == aflt.ID.ToString()).FirstOrDefault();
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
                            // STA = ((DateTime)aflt.ChocksIn).AddMinutes(210), //(DateTime)aflt.STALocal,
                            // STD = ((DateTime)aflt.ChocksOut).AddMinutes(210), //(DateTime)aflt.STDLocal,
                            STA = ((DateTime)aflt.ArrivalLocal),
                            STD = ((DateTime)aflt.DepartureLocal),
                            STAUtc = aflt.ChocksIn!=null?(DateTime)aflt.ChocksIn: (DateTime)aflt.STA,
                            STDUtc = aflt.ChocksOut!=null?(DateTime)aflt.ChocksOut: (DateTime)aflt.STD,
                            StatusId = aflt.FlightStatusID,
                            Status = aflt.FlightStatus, //aflt.FlightStatusID == 1 ? "SCHEDULED" : "CNL",
                        },
                    };
                    if (nira_flight != null)
                    {
                        conflict.Nira = new _FLT()
                        {
                            Destination = nira_flight.Destination,
                            FlightNo = nira_flight.FlightNo,
                            Origin = nira_flight.Origin,
                            Register = nira_flight.AircraftCode,
                            STA = nira_flight.ArrivalDateTime,
                            STD = nira_flight.DepartureDateTime,
                            STAUtc = nira_flight.ArrivalDateTimeUTC,
                            STDUtc = nira_flight.DepartureDateTimeUTC,
                            StatusId = -1,
                            Status = "-",
                        };
                    }
                    conflictResult.Add(conflict);
                }

                return Ok(conflictResult);
            }
            catch(Exception ex)
            {
                return (Ok(ex.Message));
            }
          

           
        }


        [Route("api/nira/conflicted/{days}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetNiraConflicts(int days )
        {
            var dfrom = DateTime.Now.Date;
            var dto = DateTime.Now.Date.AddDays(7).Date;

            var _dfrom = dfrom.Date.ToString("yyyy-MM-dd");
            var _dto = dto.Date.ToString("yyyy-MM-dd");

            string nira_url = "http://iv.nirasoft.ir:882/NRSCWS.jsp?ModuleType=SP&ModuleName=FlightsForAirPocket"
                + "&OfficeUser=Thr003.airpocket&OfficePass=nira123"
                + "&FlightNo=&FromDate=" + _dfrom + "&ToDate=" + _dto;

            List<NRSCRSFlightDataX> nira_flights = new List<NRSCRSFlightDataX>();

            using (WebClient client = new WebClient())
            {
                client.Headers["Content-type"] = "application/json";
                client.Encoding = Encoding.UTF8;
                string json = client.DownloadString(nira_url);
                var obj = JsonConvert.DeserializeObject<NRSCWSResultX>(json);
                nira_flights = obj.NRSFlightsForAirPocket;
                foreach (var x in nira_flights)
                {
                    x.AircraftCode = string.IsNullOrEmpty(x.AircraftCode) ? "" : x.AircraftCode.Replace("EP-", "");
                    if (!string.IsNullOrEmpty(x.FlightNo))
                    {
                        if (x.FlightNo.Length == 2)
                            x.FlightNo = "00" + x.FlightNo;
                        if (x.FlightNo.Length == 3)
                            x.FlightNo = "0" + x.FlightNo;
                    }

                    x.DepartureDateTimeUTC = x.DepartureDateTime.AddMinutes(-210);
                    x.ArrivalDateTimeUTC = x.ArrivalDateTime.AddMinutes(-210);

                }


            }

            var df = dfrom.Date;
            var dt = dto.Date.AddDays(1);
            ppa_entities context = new ppa_entities();
            var flights = context.ViewLegTimes.Where(q => q.STDLocal >= df && q.STDLocal < dt && (q.FlightStatusID == 1 || q.FlightStatusID==5)).OrderBy(q => q.Register).ThenBy(q => q.STD).ToList();


            List<NiraConflictResult> conflictResult = new List<NiraConflictResult>();



            foreach (var aflt in flights)
            {
                //var niraflt = niraFlights.FirstOrDefault(q => q.FlightNo.PadLeft(4, '0') == aflt.FlightNumber && q.STDDay == ((DateTime)aflt.STDLocal).Date);
                var nira_flight = nira_flights.Where(q => q.FleetWatchKey == aflt.ID.ToString()).FirstOrDefault();
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
                        STA = ((DateTime)aflt.ChocksIn).AddMinutes(210), //(DateTime)aflt.STALocal,
                        STD = ((DateTime)aflt.ChocksOut).AddMinutes(210), //(DateTime)aflt.STDLocal,
                        STAUtc = (DateTime)aflt.ChocksIn,
                        STDUtc = (DateTime)aflt.ChocksOut,
                        StatusId = aflt.FlightStatusID,
                        Status = aflt.FlightStatus, //aflt.FlightStatusID == 1 ? "SCHEDULED" : "CNL",
                    },
                };
                if (nira_flight != null)
                {
                    conflict.Nira = new _FLT()
                    {
                        Destination = nira_flight.Destination,
                        FlightNo = nira_flight.FlightNo,
                        Origin = nira_flight.Origin,
                        Register = nira_flight.AircraftCode,
                        STA = nira_flight.ArrivalDateTime,
                        STD = nira_flight.DepartureDateTime,
                        STAUtc = nira_flight.ArrivalDateTimeUTC,
                        STDUtc = nira_flight.DepartureDateTimeUTC,
                        StatusId = -1,
                        Status = "-",
                    };
                }
                conflictResult.Add(conflict);
            }
            conflictResult = conflictResult.Where(q => q.IsConflicted).ToList();
            return Ok(conflictResult);


        }



    }

    public class NRSCWSResultX
    {
        public List<NRSCRSFlightDataX> NRSFlightsForAirPocket { get; set; }
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
        public DateTime DepartureDateTimeUTC { get; set; }
        public DateTime ArrivalDateTimeUTC { get; set; }

    }

    public class _FLT
    {
        public int? FlightId { get; set; }
        public string FlightNo { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public DateTime? STD { get; set; }
        public DateTime? STA { get; set; }
        public DateTime? STDUtc { get; set; }
        public DateTime? STAUtc { get; set; }

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
                return true;
                //if (!this.IsNiraFound)
                //    return null;
                //if (this.Nira.StatusId == this.AirPocket.StatusId)
                //    return true;
                //else
                //    return false;
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
                // if (this.Nira.STA == this.AirPocket.STA)
                //     return true;
                if (this.Nira.STA == this.AirPocket.STA || (this.Nira.STA!=null && ((DateTime) this.Nira.STA).AddDays(1)== this.AirPocket.STA))
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
