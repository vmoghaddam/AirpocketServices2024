using ApiWorld.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ApiWorld.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PPSController : ApiController
    {
        [Route("api/pps/test")]
        public async Task<IHttpActionResult> GetPPSTest()
        {
            try
            {

                pps.EfbService pps_ins = new pps.EfbService();
                var session_id = pps_ins.GetSessionID("ArmeniaAirways", "J8V14HNHK", "AMWINTEGRATION", "i$qn719e");
                var _flt = pps_ins.GetFlight(session_id, 9300840, false, false, false, false, "kg");
                var flts = pps_ins.GetSTDFlightList(session_id, new DateTime(2025, 11, 22), new DateTime(2025, 11, 24));
                pps.Flight flt_info = pps_ins.GetFlight(session_id, flts.Items[0].ID, true, true, true, true, "kg");
                var atc = pps_ins.GetATC(session_id, flts.Items[0].ID);
                var apts = pps_ins.GetFlightAirports(session_id, flts.Items[0].ID);
                var wx = pps_ins.GetWX(session_id, flts.Items[0].ID, false);
                var a1 = pps_ins.GetArinc633FlightLog(session_id, flts.Items[0].ID, "kg");
                var flt_id = flts.Items[0].ID;
                //var a2 = pps_ins.GetArinc633WBACommon(session_id, flt_id, "kg");
                //var a3 = pps_ins.GetArinc633FlightPlanAtcIcao(session_id, flt_id);
                //var a4 = pps_ins.GetEff_FullPackage(session_id, flt_id, "kg");
                var a5 = pps_ins.GetFlightDocumentsMeta(session_id, flt_id);
                var a6 = pps_ins.GetRSChart(session_id, flt_id, true, true, true, true, true, true, true);
                var a7 = pps_ins.GetAirport(session_id, "OIMS");
                return Ok(session_id);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                return Ok(msg);
            }

        }
        string get_tme(int n)
        {
            var hh = n / 60;
            var mm = n % 60;
            //"TME": "00:00:00.0000000",
            return hh.ToString().PadLeft(2, '0') + ":" + mm.ToString().PadLeft(2, '0') + ":" + "00.0000000";

        }

        [Route("api/pps/status/{fltid}")]
        [HttpGet]
        public async Task<IHttpActionResult> GeOfpStatus(int fltid)
        {
            ppa_entities context = new ppa_entities();
            var fp=  context.Flights.Where(q => q.FlightId == fltid).FirstOrDefault();
            if (fp == null)
                return Ok(false);
            return Ok(true);
        }

        [Route("api/pps/save/{fltid}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetPPSTestSave(int fltid)
        {
            try
            {
                //var file_path = @"C:\Users\vahid\Desktop\amw\OFP\";
                var file_path = @"C:\Inetpub\vhosts\amwaero.tech\efb.amwaero.tech\upload\ofp\";
                var flight_id = fltid; //582875;
                var plan = new OFPImport()
                {
                    DateCreate = DateTime.Now,
                    //DateFlight = flight.STDDay,
                    //FileName = "",
                    //FlightNo = flight.FlightNumber,
                    //Origin = flight.FromAirportICAO,
                    //Destination = flight.ToAirportICAO,
                    //User = "airpocket",
                    //Text = rawText,
                    FlightId = flight_id,


                };
                ppa_entities context = new ppa_entities();
                var ops_flight = context.FlightInformations.FirstOrDefault(q => q.ID == flight_id);
                var ops_flight_std_date = ((DateTime)ops_flight.STD);

                List<string> props = new List<string>();
                pps.EfbService pps_ins = new pps.EfbService();
                var session_id = pps_ins.GetSessionID("ArmeniaAirways", "J8V14HNHK", "AMWINTEGRATION", "i$qn719e");
                var flts = pps_ins.GetSTDFlightList(session_id, ops_flight_std_date, ops_flight_std_date);

                if (flts == null || flts.Items.Count() == 0)
                {
                    return Ok(false);
                }
                pps.Flight flt_info = pps_ins.GetFlight(session_id, flts.Items[0].ID, true, true, true, true, "kg");

                //var eeee=pps_ins.GetEff_FullPackage(session_id, flts.Items[0].ID, "kg");

                //var file_prefix = flt_info.ID + "_" + ops_flight_std_date.ToString("yyyy-MM-dd_HHmm") + "_" + ops_flight.FlightNumber;
                var file_prefix = flight_id + "_";

                //var flts2 = pps_ins.GetFlightListSearch(session_id, new DateTime(2025, 9, 1), new DateTime(2025, 11, 17), new DateTime(2025, 9, 1)
                //    , null, false, null, null, null, null);


                //  var package=pps_ins.GetEff_FullPackage(session_id, flt_info.ID, "kg").bArray;
                //  File.WriteAllBytes(file_path+ @"packages\package_" + flt_info.ID+"_"+ops_flight_std_date.ToString("yyyy-MM-dd_HHmm")+"_"+ops_flight.FlightNumber+".eff", package);

                //var docs = pps_ins.GetFlightDocumentsMeta(session_id, flt_info.ID).Documents;
                //foreach(var x in docs)
                //{
                //    var pdf_id = x.Identifier;
                //    var pdf = pps_ins.GetFlightDocument(session_id, pdf_id).ByteArray;
                //    //File.WriteAllBytes(file_path + @"documents\doc_" + flt_info.ID + "_" + ops_flight_std_date.ToString("yyyy-MM-dd_HHmm") + "_" + ops_flight.FlightNumber + ".eff", pdf);
                //    File.WriteAllBytes(file_path + @"documents\"+x.Identifier  + ".pdf", pdf);

                //}try{
                try
                {
                    File.WriteAllBytes(file_path + @"atc\" + "atc_" + file_prefix + ".pdf", pps_ins.GetPDF_ATC(session_id, flt_info.ID).bArray);
                    File.WriteAllBytes(file_path + @"log\" + "log_" + file_prefix + ".pdf", pps_ins.GetPDF_Logstring(session_id, flt_info.ID).bArray);
                    File.WriteAllBytes(file_path + @"notam\" + "notam_" + file_prefix + ".pdf", pps_ins.GetPDF_NOTAMs(session_id, flt_info.ID, false).bArray);
                    File.WriteAllBytes(file_path + @"wx\" + "wx_" + file_prefix + ".pdf", pps_ins.GetPDF_WX(session_id, flt_info.ID, false).bArray);

                    var wind_def_fl = pps_ins.GetRSChart(session_id, flt_info.ID, true, false, false, false, false, false, false).Content;
                    File.WriteAllBytes(file_path + @"chart\" + "wind_fl_def_" + file_prefix + ".pdf", wind_def_fl);

                    var wind_low_fl = pps_ins.GetRSChart(session_id, flt_info.ID, false, true, false, false, false, false, false).Content;
                    File.WriteAllBytes(file_path + @"chart\" + "wind_fl_low_" + file_prefix + ".pdf", wind_low_fl);

                    var wind_high_fl = pps_ins.GetRSChart(session_id, flt_info.ID, false, false, true, false, false, false, false).Content;
                    File.WriteAllBytes(file_path + @"chart\" + "wind_fl_high_" + file_prefix + ".pdf", wind_high_fl);

                    var cross_section = pps_ins.GetRSChart(session_id, flt_info.ID, false, false, false, true, false, false, false).Content;
                    File.WriteAllBytes(file_path + @"chart\" + "cross_section_" + file_prefix + ".pdf", cross_section);

                    var swx_chart = pps_ins.GetRSChart(session_id, flt_info.ID, false, false, false, false, true, false, false).Content;
                    File.WriteAllBytes(file_path + @"chart\" + "swx_" + file_prefix + ".pdf", swx_chart);

                    var etps_chart = pps_ins.GetRSChart(session_id, flt_info.ID, false, false, false, false, false, true, false).Content;
                    File.WriteAllBytes(file_path + @"chart\" + "etops_" + file_prefix + ".pdf", etps_chart);

                    var pax_chart = pps_ins.GetRSChart(session_id, flt_info.ID, false, false, false, false, false, false, true).Content;
                    File.WriteAllBytes(file_path + @"chart\" + "pax_" + file_prefix + ".pdf", pax_chart);
                }
                catch (Exception ex)
                {

                }
                // var apt=pps_ins.GetAirport(session_id, "UDYZ");

                // var wx = pps_ins.GetPDF_WX(session_id, flt_info.ID, false).bArray;
                // File.WriteAllBytes(@"C:\Users\vahid\Desktop\amw\OFP\file.pdf", wx);

                // var rs=pps_ins.GetRSChart(session_id, flt_info.ID, true, true, true, true, true, true, true).Content;


                //  File.WriteAllBytes(@"C:\Users\vahid\Desktop\amw\OFP\rs.pdf", rs);


                Models.Flight db_flight = new Flight() { FlightId = flight_id };

                var pf_junction = new plan_flight()
                {
                    Flight = db_flight,
                    FlightInformation = ops_flight,
                };

                var exist_flt = context.Flights.Where(q => q.FlightId == flight_id).ToList();
                var exist_ofp=context.OFPImports.Where(q=>q.FlightId == flight_id).ToList();
                context.Flights.RemoveRange(exist_flt);
                context.OFPImports.RemoveRange(exist_ofp);
                var atcdata = flt_info.ATCData;
                var atc_prts = new List<string>();
                atc_prts.Add("(FPL-" + flt_info.ATCData.ATCID + "-" + flt_info.ATCData.ATCRule + flt_info.ATCData.ATCType);
                atc_prts.Add("-" + atcdata.ATCTOA + "/" + atcdata.ATCWake + "-" + atcdata.ATCEqui + "/" + atcdata.ATCSSR);
                atc_prts.Add("-" + atcdata.ATCDep + atcdata.ATCTime);
                atc_prts.Add("-" + atcdata.ATCSpeed + atcdata.ATCFL + " " + atcdata.ATCRoute);
                atc_prts.Add("-" + atcdata.ATCDest + atcdata.ATCEET + " " + atcdata.ATCAlt1 + " " + atcdata.ATCAlt2);
                atc_prts.Add("-" + atcdata.ATCInfo);
                atc_prts.Add("-E/" + atcdata.ATCEndu + " " + "P/" + atcdata.ATCPers + " " + "R/" + atcdata.ATCRadi + " " + "J/" + atcdata.ATCJack);
                atc_prts.Add("A/" + atcdata.ATCAcco);
                atc_prts.Add("C/" + atcdata.ATCPIC + ")");

                plan.ATC = String.Join("<br>", atc_prts);


                if (flt_info.Messages != null)
                    foreach (var msg in flt_info.Messages)
                    {
                        var db_msg = new Message()
                        {
                            HighPriority = msg.HighPriority,
                            SentFrom = msg.SentFrom,
                            SentTime = msg.SentTime,
                            Subject = msg.Subject,
                            Text = msg.Text,
                            ValidFrom = msg.ValidFrom,
                            ValidTo = msg.ValidTo,
                        };
                        foreach (var rec in msg.Recipients)
                            db_msg.MessageRecipients.Add(
                                new MessageRecipient()
                                {
                                    Recipient = rec.Recipient,
                                    RecipientType = rec.RecipientType,
                                });
                        db_flight.Messages.Add(db_msg);
                    }

                db_flight.OverflightCost = new OverflightCost()
                {
                    Currency = flt_info.OverflightCost.Currency,
                    TotalOverflightCost = flt_info.OverflightCost.TotalOverflightCost,
                    TotalTerminalCost = flt_info.OverflightCost.TotalTerminalCost,
                };
                db_flight.FlightLogID = flt_info.FlightLogID;

                db_flight.PPSName = flt_info.PPSName;
                db_flight.ACFTAIL = flt_info.ACFTAIL;
                db_flight.DEP = flt_info.DEP;
                db_flight.DEST = flt_info.DEST;
                db_flight.ALT1 = flt_info.ALT1;
                db_flight.ALT2 = flt_info.ALT2;
                db_flight.STD = flt_info.STD;
                db_flight.PAX = flt_info.PAX;
                db_flight.FUEL = flt_info.FUEL;
                db_flight.LOAD = flt_info.LOAD;
                db_flight.ValidHrs = flt_info.ValidHrs;
                db_flight.MinFL = flt_info.MinFL;
                db_flight.MaxFL = flt_info.MaxFL;
                db_flight.EROPSAltApts = flt_info.EROPSAltApts;
                //db_flight.AdequateApts = flt_info.AdequateApt;

                // AdequateApt            // string[] - skip
                if (flt_info.AdequateApt != null)
                    foreach (var x in flt_info.AdequateApt)
                        db_flight.AdequateApts.Add(new AdequateApt()
                        {
                            Value = x,
                        });
                // FIR                    // string[] - skip
                if (flt_info.FIR != null)
                    foreach (var x in flt_info.FIR)
                        db_flight.FIRs.Add(new FIR()
                        {
                            Value = x,
                        });
                // AltApts                // string[] - skip
                if (flt_info.AltApts != null)
                    foreach (var x in flt_info.AltApts)
                        db_flight.AltApts.Add(new AltApt()
                        {
                            Value = x,
                        });
                db_flight.TOA = flt_info.TOA;
                db_flight.FMDID = flt_info.FMDID;
                db_flight.DESTSTDALT = flt_info.DESTSTDALT;
                db_flight.FUELCOMP = flt_info.FUELCOMP;
                db_flight.TIMECOMP = flt_info.TIMECOMP;
                db_flight.FUELCONT = flt_info.FUELCONT;
                db_flight.TIMECONT = flt_info.TIMECONT;
                db_flight.PCTCONT = flt_info.PCTCONT;
                db_flight.FUELMIN = flt_info.FUELMIN;
                db_flight.TIMEMIN = flt_info.TIMEMIN;
                db_flight.FUELTAXI = flt_info.FUELTAXI;
                db_flight.TIMETAXI = flt_info.TIMETAXI;
                db_flight.FUELEXTRA = flt_info.FUELEXTRA;
                db_flight.TIMEEXTRA = flt_info.TIMEEXTRA;
                db_flight.FUELLDG = flt_info.FUELLDG;
                db_flight.TIMELDG = flt_info.TIMELDG;
                db_flight.ZFM = flt_info.ZFM;
                db_flight.GCD = flt_info.GCD;
                db_flight.ESAD = flt_info.ESAD;
                db_flight.GL = flt_info.GL;
                db_flight.FUELBIAS = flt_info.FUELBIAS;
                db_flight.STA = flt_info.STA;
                db_flight.ETA = flt_info.ETA;
                // LocalTime              // complex - skip
                db_flight.LocalTime1 = new LocalTime()
                {
                    LocalTimeDeparture = new LocalTimeDeparture()
                    {
                        ETD = flt_info.LocalTime.Departure.ETD,
                        STD = flt_info.LocalTime.Departure.STD,
                        Sunrise = flt_info.LocalTime.Departure.Sunrise,
                        Sunset = flt_info.LocalTime.Departure.Sunset,
                    },
                    LocalTimeDestination = new LocalTimeDestination()
                    {
                        ETA = flt_info.LocalTime.Destination.ETA,
                        STA = flt_info.LocalTime.Destination.STA,
                        Sunrise = flt_info.LocalTime.Destination.Sunrise,
                        Sunset = flt_info.LocalTime.Destination.Sunset,
                    }
                };

                db_flight.SCHBLOCKTIME = flt_info.SCHBLOCKTIME;
                db_flight.DISP = flt_info.DISP;
                db_flight.LastEditDate = flt_info.LastEditDate;
                db_flight.LatestFlightPlanDate = flt_info.LatestFlightPlanDate;
                db_flight.LatestDocumentUploadDate = flt_info.LatestDocumentUploadDate;
                db_flight.FUELMINTO = flt_info.FUELMINTO;
                db_flight.TIMEMINTO = flt_info.TIMEMINTO;
                db_flight.ARAMP = flt_info.ARAMP;
                db_flight.TIMEACT = flt_info.TIMEACT;
                db_flight.FUELACT = flt_info.FUELACT;
                db_flight.DestERA = flt_info.DestERA;
                db_flight.TrafficLoad = flt_info.TrafficLoad;
                db_flight.WeightUnit = flt_info.WeightUnit;
                db_flight.WindComponent = flt_info.WindComponent;
                db_flight.CustomerDataPPS = flt_info.CustomerDataPPS;
                db_flight.CustomerDataScheduled = flt_info.CustomerDataScheduled;
                db_flight.Fl = flt_info.Fl;
                db_flight.RouteDistNM = flt_info.RouteDistNM;
                db_flight.RouteName = flt_info.RouteName;
                db_flight.RouteType = flt_info.RouteType;
                db_flight.RouteRemark = flt_info.RouteRemark;
                db_flight.EmptyWeight = flt_info.EmptyWeight;
                db_flight.TotalDistance = flt_info.TotalDistance;
                db_flight.AltDist = flt_info.AltDist;
                db_flight.DestTime = flt_info.DestTime;
                db_flight.AltTime = flt_info.AltTime;
                db_flight.AltFuel = flt_info.AltFuel;
                db_flight.HoldTime = flt_info.HoldTime;
                db_flight.ReserveTime = flt_info.ReserveTime;
                db_flight.Cargo = flt_info.Cargo;
                db_flight.ActTOW = flt_info.ActTOW;
                db_flight.TripFuel = flt_info.TripFuel;
                db_flight.HoldFuel = flt_info.HoldFuel;
                // Holding                // complex - skip
                db_flight.HoldingFuel = new HoldingFuel()
                {
                    Fuel = flt_info.Holding.Fuel,
                    FuelFlowType = flt_info.Holding.FuelFlowType,
                    Minutes = flt_info.Holding.Minutes,
                    Profile = flt_info.Holding.Profile,
                    Specification = flt_info.Holding.Specification,

                };
                db_flight.Elw = flt_info.Elw;
                db_flight.FuelPolicy = flt_info.FuelPolicy;
                db_flight.Alt2Time = flt_info.Alt2Time;
                db_flight.Alt2Fuel = flt_info.Alt2Fuel;
                db_flight.MaxTOM = flt_info.MaxTOM;
                db_flight.MaxLM = flt_info.MaxLM;
                db_flight.MaxZFM = flt_info.MaxZFM;
                db_flight.WeatherObsTime = flt_info.WeatherObsTime;
                db_flight.WeatherPlanTime = flt_info.WeatherPlanTime;
                db_flight.MFCI = flt_info.MFCI;
                db_flight.CruiseProfile = flt_info.CruiseProfile;
                db_flight.TempTopOfClimb = flt_info.TempTopOfClimb;
                db_flight.Climb = flt_info.Climb;
                db_flight.Descend = flt_info.Descend;
                db_flight.FuelPL = flt_info.FuelPL;
                db_flight.DescendWind = flt_info.DescendWind;
                db_flight.ClimbProfile = flt_info.ClimbProfile;
                db_flight.DescendProfile = flt_info.DescendProfile;
                db_flight.HoldProfile = flt_info.HoldProfile;
                db_flight.StepClimbProfile = flt_info.StepClimbProfile;
                db_flight.FuelContDef = flt_info.FuelContDef;
                db_flight.FuelAltDef = flt_info.FuelAltDef;
                db_flight.AmexsyStatus = flt_info.AmexsyStatus;
                db_flight.AvgTrack = flt_info.AvgTrack;
                // DEPTAF                 // complex - skip
                db_flight.DEPTaf = new DEPTaf()
                {
                    ForecastEndTime = flt_info.DEPTAF.ForecastEndTime,
                    ForecastStartTime = flt_info.DEPTAF.ForecastStartTime,
                    ForecastTime = flt_info.DEPTAF.ForecastTime,
                    ICAO = flt_info.DEPTAF.ICAO,
                    Text = flt_info.DEPTAF.Text,
                    Type = (byte)flt_info.DEPTAF.Type,

                };

                db_flight.DEPMetar = flt_info.DEPMetar;
                // DEPNotam               // complex - skip

                // DESTTAF                // complex - skip
                db_flight.DESTTaf = new DESTTaf()
                {
                    ForecastEndTime = flt_info.DESTTAF.ForecastEndTime,
                    ForecastStartTime = flt_info.DESTTAF.ForecastStartTime,
                    ForecastTime = flt_info.DESTTAF.ForecastTime,
                    ICAO = flt_info.DESTTAF.ICAO,
                    Text = flt_info.DESTTAF.Text,
                    Type = (byte)flt_info.DESTTAF.Type,

                };
                db_flight.DESTMetar = flt_info.DESTMetar;
                // DESTNotam              // complex - skip
                // ALT1TAF                // complex - skip
                if (flt_info.ALT1TAF != null)
                    db_flight.ALT1Taf = new ALT1Taf()
                    {
                        ForecastEndTime = flt_info.ALT1TAF.ForecastEndTime,
                        ForecastStartTime = flt_info.DESTTAF.ForecastStartTime,
                        ForecastTime = flt_info.DESTTAF.ForecastTime,
                        ICAO = flt_info.DESTTAF.ICAO,
                        Text = flt_info.DESTTAF.Text,
                        Type = (byte)flt_info.DESTTAF.Type,

                    };
                // ALT2TAF                // complex - skip
                if (flt_info.ALT2TAF != null)
                    db_flight.ALT2Taf = new ALT2Taf()
                    {
                        ForecastEndTime = flt_info.ALT2TAF.ForecastEndTime,
                        ForecastStartTime = flt_info.DESTTAF.ForecastStartTime,
                        ForecastTime = flt_info.DESTTAF.ForecastTime,
                        ICAO = flt_info.DESTTAF.ICAO,
                        Text = flt_info.DESTTAF.Text,
                        Type = (byte)flt_info.DESTTAF.Type,

                    };
                db_flight.ALT1Metar = flt_info.ALT1Metar;
                db_flight.ALT2Metar = flt_info.ALT2Metar;
                // ALT1Notam              // complex - skip
                // ALT2Notam              // complex - skip
                // RoutePoints            // complex - skip
                var idx = 0;
                List<PSSPoint> main_points = new List<PSSPoint>();
                //2025-12-03
                foreach (var x in flt_info.RoutePoints)
                {
                    var _p = new RoutePoint()
                    {
                        ACCDIST = x.ACCDIST,
                        ACCTIME = x.ACCTIME,
                        ClimbDescent = x.ClimbDescent,
                        DistRemaining = x.DistRemaining,
                        FIR = x.FIR,
                        FL = x.FL,
                        Frequency = Convert.ToDecimal(x.Frequency),
                        FuelFlow = x.FuelFlow,
                        FuelFlowPerEng = Convert.ToDecimal(x.FuelFlowPerEng),
                        FuelRemaining = Convert.ToDecimal(x.FuelRemaining),
                        FuelUsed = Convert.ToDecimal(x.FuelUsed),
                        GroundSpeed = x.GroundSpeed,
                        HLAEntryExit = x.HLAEntryExit,
                        IDENT = x.IDENT,
                        ISA = x.ISA,
                        LAT = Convert.ToDecimal(x.LAT),
                        LegAWY = x.LegAWY,
                        LegCAT = x.LegCAT,
                        LegCourse = Convert.ToDecimal(x.LegCourse),
                        LegDistance = x.LegDistance,
                        LegFuel = Convert.ToDecimal(x.LegFuel),
                        LegName = x.LegName,
                        LegTime = x.LegTime,
                        LON = Convert.ToDecimal(x.LON),
                        MagCourse = Convert.ToDecimal(x.MagCourse),
                        MagneticHeading = Convert.ToDecimal(x.MagneticHeading),
                        MagneticTrack = x.MagneticTrack,
                        MinimumEnrouteAltitude = x.MinimumEnrouteAltitude,
                        MinReqFuel = Convert.ToDecimal(x.MinReqFuel),
                        MORA = x.MORA,
                        Temperature = x.Temperature,
                        TimeRemaining = x.TimeRemaining,
                        TrueAirSpeed = x.TrueAirSpeed,
                        TrueHeading = Convert.ToDecimal(x.TrueHeading),
                        TrueTrack = x.TrueTrack,
                        VARIATION = x.VARIATION,
                        Vol = x.Vol,
                        Wind = x.Wind,
                        WindComponent = x.WindComponent,


                    };
                    var _key = "mpln_WAP_" + _p.IDENT;
                    PSSPoint point = new PSSPoint()
                    {

                        ACCDIST = x.ACCDIST,
                        ACCTIME = x.ACCTIME,
                        ClimbDescent = x.ClimbDescent,
                        DistRemaining = x.DistRemaining,
                        FIR = x.FIR,
                        FL = x.FL,
                        Frequency = Convert.ToDecimal(x.Frequency),
                        FuelFlow = x.FuelFlow,
                        FuelFlowPerEng = Convert.ToDecimal(x.FuelFlowPerEng),
                        FuelRemaining = Convert.ToDecimal(x.FuelRemaining),
                        FuelUsed = Convert.ToDecimal(x.FuelUsed),
                        GroundSpeed = x.GroundSpeed,
                        HLAEntryExit = x.HLAEntryExit,
                        IDENT = x.IDENT,
                        ISA = x.ISA,
                        LAT = Convert.ToDecimal(x.LAT),
                        LegAWY = x.LegAWY,
                        LegCAT = x.LegCAT,
                        LegCourse = Convert.ToDecimal(x.LegCourse),
                        LegDistance = x.LegDistance,
                        LegFuel = Convert.ToDecimal(x.LegFuel),
                        LegName = x.LegName,
                        LegTime = x.LegTime,
                        LON = Convert.ToDecimal(x.LON),
                        MagCourse = Convert.ToDecimal(x.MagCourse),
                        MagneticHeading = Convert.ToDecimal(x.MagneticHeading),
                        MagneticTrack = x.MagneticTrack,
                        MinimumEnrouteAltitude = x.MinimumEnrouteAltitude,
                        MinReqFuel = Convert.ToDecimal(x.MinReqFuel),
                        MORA = x.MORA,
                        Temperature = x.Temperature,
                        TimeRemaining = x.TimeRemaining,
                        TrueAirSpeed = x.TrueAirSpeed,
                        TrueHeading = Convert.ToDecimal(x.TrueHeading),
                        TrueTrack = x.TrueTrack,
                        VARIATION = x.VARIATION,
                        Vol = x.Vol,
                        Wind = x.Wind,
                        WindComponent = x.WindComponent,
                        PlanType = "MAIN",
                        _key = _key,
                        FRE = round_int(x.FuelRemaining).ToString(),
                        FUS = round_int(x.FuelUsed).ToString(),
                        TME = get_tme(x.LegTime),
                        TTM = get_tme(x.ACCTIME)

                    };
                    // "TME": "00:00:00.0000000",
                    main_points.Add(point);

                    if (x.FlightLevelWinds != null)
                        foreach (var w in x.FlightLevelWinds)
                            _p.FlightLevelWinds.Add(new FlightLevelWind()
                            {
                                FlightLevel = w.FlightLevel,
                                Shear = w.Shear,
                                Temp = w.Temp,
                                Velocity = w.Velocity,
                                Wind = w.Wind,

                            });
                    db_flight.RoutePoints.Add(_p);

                    plan.OFPImportProps.Add(new OFPImportProp()
                    {
                        PropName = "prop_" + _key + "_eta_" + idx,
                        DateUpdate = DateTime.UtcNow.ToString("yyyyMMddHHmm"),
                        PropValue = "",
                        Identifier = _p.IDENT,
                        Remark = "MAIN ROUTE",

                    });
                    //props.Add("prop_" + _key + "_eta_" + idx);
                    //props.Add("prop_" + _key + "_ata_" + idx);
                    plan.OFPImportProps.Add(new OFPImportProp()
                    {
                        PropName = "prop_" + _key + "_ata_" + idx,
                        DateUpdate = DateTime.UtcNow.ToString("yyyyMMddHHmm"),
                        PropValue = "",
                        Identifier = _p.IDENT,
                        Remark = "MAIN ROUTE",

                    });
                    //props.Add("prop_" + _key + "_rem_" + idx);
                    plan.OFPImportProps.Add(new OFPImportProp()
                    {
                        PropName = "prop_" + _key + "_rem_" + idx,
                        DateUpdate = DateTime.UtcNow.ToString("yyyyMMddHHmm"),
                        PropValue = "",
                        Identifier = _p.IDENT,
                        Remark = "MAIN ROUTE",

                    });
                    //props.Add("prop_" + _key + "_usd_" + idx);
                    plan.OFPImportProps.Add(new OFPImportProp()
                    {
                        PropName = "prop_" + _key + "_usd_" + idx,
                        DateUpdate = DateTime.UtcNow.ToString("yyyyMMddHHmm"),
                        PropValue = "",
                        Identifier = _p.IDENT,
                        Remark = "MAIN ROUTE",

                    });
                    idx++;
                }
                // Crews                  // complex - skip
                if (flt_info.Crews != null)
                {
                    foreach (var c in flt_info.Crews)
                        db_flight.Crews.Add(new Crew()
                        {
                            CrewName = c.CrewName,
                            CrewType = c.CrewType,
                            GSM = c.GSM,
                            ID = c.ID,
                            Initials = c.Initials,
                            //Mass = c.Mass
                        });
                }
                // Responce               // complex - skip
                if (flt_info.Responce != null)
                {
                    db_flight.Response = new Response()
                    {
                        Message = flt_info.Responce.Message,
                        Succeed = flt_info.Responce.Succeed,
                    };
                }
                // ATCData                // complex - skip

                if (flt_info.ATCData != null)
                {
                    db_flight.ATC = new ATC()
                    {
                        ATCAcco = flt_info.ATCData.ATCAcco,
                        ATCAlt1 = flt_info.ATCData.ATCAlt1,
                        ATCAlt2 = flt_info.ATCData.ATCAlt2,
                        ATCCap = flt_info.ATCData.ATCCap,
                        ATCColo = flt_info.ATCData.ATCColo,
                        ATCCover = flt_info.ATCData.ATCCover,
                        ATCCtot = flt_info.ATCData.ATCCtot,
                        ATCDep = flt_info.ATCData.ATCDep,
                        ATCDest = flt_info.ATCData.ATCDest,
                        ATCDing = flt_info.ATCData.ATCDing,
                        ATCEET = flt_info.ATCData.ATCEET,
                        ATCEndu = flt_info.ATCData.ATCEndu,
                        ATCEqui = flt_info.ATCData.ATCEqui,
                        ATCFL = flt_info.ATCData.ATCFL,
                        ATCID = flt_info.ATCData.ATCID,
                        ATCInfo = flt_info.ATCData.ATCInfo,
                        ATCJack = flt_info.ATCData.ATCJack,
                        ATCNum = flt_info.ATCData.ATCNum,
                        ATCPers = flt_info.ATCData.ATCPers,
                        ATCPIC = flt_info.ATCData.ATCPIC,
                        ATCRadi = flt_info.ATCData.ATCRadi,
                        ATCRem = flt_info.ATCData.ATCRem,
                        ATCRoute = flt_info.ATCData.ATCRoute,
                        ATCRule = flt_info.ATCData.ATCRule,
                        ATCSpeed = flt_info.ATCData.ATCSpeed,
                        ATCSSR = flt_info.ATCData.ATCSSR,
                        ATCSurv = flt_info.ATCData.ATCSurv,
                        ATCTime = flt_info.ATCData.ATCTime,
                        ATCTOA = flt_info.ATCData.ATCTOA,
                        ATCType = flt_info.ATCData.ATCType,
                        ATCWake = flt_info.ATCData.ATCWake,

                    };
                }
                // NextLeg                // complex - skip
                if (flt_info.NextLeg != null)
                    db_flight.NextLeg = new NextLeg()
                    {
                        DEP = flt_info.NextLeg.DEP,
                        DEST = flt_info.NextLeg.DEST,
                        MINFUEL = flt_info.NextLeg.MINFUEL,
                        STD = flt_info.NextLeg.STD,

                    };
                // OptFlightLevels        // complex - skip
                if (flt_info.OptFlightLevels != null)
                {
                    foreach (var x in flt_info.OptFlightLevels)
                        db_flight.OptFlightLevels.Add(new OptFlightLevel()
                        {
                            Cost = x.Cost,
                            CostDiff = x.CostDiff,
                            FuelLower = x.FuelLower,
                            FuelNCruise = x.FuelNCruise,
                            FuelProfile2 = x.FuelProfile2,
                            FuelProfile3 = x.FuelProfile3,
                            Level = x.Level,
                            TimeNCruise = x.TimeNCruise,
                            TimeProfile2 = x.TimeProfile2,
                            TimeProfile3 = x.TimeProfile3,
                            WC = x.WC,
                        });
                }
                // AdequateNotam          // complex - skip
                if (flt_info.AdequateNotam != null)
                    foreach (var x in flt_info.AdequateNotam)
                    {
                        var obj = new AdequateNotam()
                        {
                            ECode = x.ECode,
                            Fir = x.Fir,
                            FromDate = x.FromDate,
                            FromLevel = x.FromLevel,
                            ICAO = x.ICAO,
                            Number = x.Number,
                            Provider = x.Provider,
                            QCode = x.QCode,
                            RoutePart = (int)x.RoutePart,
                            Text = x.Text,
                            ToDate = x.ToDate,
                            ToLevel = x.ToLevel,
                            UniformAbbreviation = x.UniformAbbreviation,
                            Year = x.Year,


                        };
                        //x.PartInformation
                        db_flight.AdequateNotams.Add(obj);
                    }
                // FIRNotam               // complex - skip
                if (flt_info.FIRNotam != null)
                {
                    foreach (var x in flt_info.FIRNotam)
                        db_flight.FIRNotams.Add(new FIRNotam()
                        {
                            ECode = x.ECode,
                            Fir = x.Fir,
                            FromDate = x.FromDate,
                            FromLevel = x.FromLevel,
                            ICAO = x.ICAO,
                            Number = x.Number,
                            Provider = x.Provider,
                            QCode = x.QCode,
                            RoutePart = (int)x.RoutePart,
                            Text = x.Text,
                            ToDate = x.ToDate,
                            ToLevel = x.ToLevel,
                            UniformAbbreviation = x.UniformAbbreviation,
                            Year = x.Year,
                        });
                }
                // AlternateNotam         // complex - skip
                if (flt_info.AlternateNotam != null)
                {
                    foreach (var x in flt_info.AlternateNotam)
                        db_flight.AlternateNotams.Add(new AlternateNotam()
                        {
                            ECode = x.ECode,
                            Fir = x.Fir,
                            FromDate = x.FromDate,
                            FromLevel = x.FromLevel,
                            ICAO = x.ICAO,
                            Number = x.Number,
                            Provider = x.Provider,
                            QCode = x.QCode,
                            RoutePart = (int)x.RoutePart,
                            Text = x.Text,
                            ToDate = x.ToDate,
                            ToLevel = x.ToLevel,
                            UniformAbbreviation = x.UniformAbbreviation,
                            Year = x.Year,
                        });
                }
                // Airports               // complex - skip
                if (flt_info.Airports != null)
                {
                    foreach (var x in flt_info.Airports)
                    {
                        var _apt = new Airport()
                        {
                            ATC = x.ATC,
                            AirportHoursText = x.AirportHours != null ? x.AirportHours.Text : null,
                            Category = x.Category,
                            Dist = x.Dist,
                            Elevation = x.Elevation,
                            Fuel = x.Fuel,
                            Iata = x.Iata,
                            Icao = x.Icao,
                            Lat = x.Lat,
                            Long = x.Long,
                            MAGCURS = x.MAGCURS,
                            Name = x.Name,
                            Rwyl = x.Rwyl,
                            Time = x.Time,
                            Type = x.Type,



                        };
                        foreach (var f in x.Frequencies)
                            _apt.AirportFrequencies.Add(new AirportFrequency()
                            {
                                Information = f.Information,
                                Value = f.Value
                            });
                        foreach (var f in x.Frequencies2)
                            _apt.AirportFrequencies2.Add(new AirportFrequencies2()
                            {
                                Information = f.Information,
                                Value = f.Value
                            });
                        db_flight.Airports.Add(_apt);
                    }

                }
                // EnrouteAlternates      // string[] - skip
                if (flt_info.EnrouteAlternates != null)
                {
                    foreach (var x in flt_info.EnrouteAlternates)
                        db_flight.EnrouteAlternates.Add(new EnrouteAlternate()
                        {
                            Value = x,
                        });
                }
                // Alt1Points             // complex - skip
                var alt1_points = new List<PSSPoint>();
                idx = 0;
                if (flt_info.Alt1Points != null)
                {
                    foreach (var x in flt_info.Alt1Points)
                    {
                        var _p = new Alt1Points()
                        {
                            ACCDIST = x.ACCDIST,
                            ACCTIME = x.ACCTIME,
                            ClimbDescent = x.ClimbDescent,
                            DistRemaining = x.DistRemaining,
                            FIR = x.FIR,
                            FL = x.FL,
                            Frequency = Convert.ToDecimal(x.Frequency),
                            FuelFlow = x.FuelFlow,
                            FuelFlowPerEng = Convert.ToDecimal(x.FuelFlowPerEng),
                            FuelRemaining = Convert.ToDecimal(x.FuelRemaining),
                            FuelUsed = Convert.ToDecimal(x.FuelUsed),
                            GroundSpeed = x.GroundSpeed,
                            HLAEntryExit = x.HLAEntryExit,
                            IDENT = x.IDENT,
                            ISA = x.ISA,
                            LAT = Convert.ToDecimal(x.LAT),
                            LegAWY = x.LegAWY,
                            LegCAT = x.LegCAT,
                            LegCourse = Convert.ToDecimal(x.LegCourse),
                            LegDistance = x.LegDistance,
                            LegFuel = Convert.ToDecimal(x.LegFuel),
                            LegName = x.LegName,
                            LegTime = x.LegTime,
                            LON = Convert.ToDecimal(x.LON),
                            MagCourse = Convert.ToDecimal(x.MagCourse),
                            MagneticHeading = Convert.ToDecimal(x.MagneticHeading),
                            MagneticTrack = x.MagneticTrack,
                            MinimumEnrouteAltitude = x.MinimumEnrouteAltitude,
                            MinReqFuel = Convert.ToDecimal(x.MinReqFuel),
                            MORA = x.MORA,
                            Temperature = x.Temperature,
                            TimeRemaining = x.TimeRemaining,
                            TrueAirSpeed = x.TrueAirSpeed,
                            TrueHeading = Convert.ToDecimal(x.TrueHeading),
                            TrueTrack = x.TrueTrack,
                            VARIATION = x.VARIATION,
                            Vol = x.Vol,
                            Wind = x.Wind,
                            WindComponent = x.WindComponent,



                        };
                        var _key = "apln_WAP_" + _p.IDENT;
                        PSSPoint point = new PSSPoint()
                        {
                            ACCDIST = x.ACCDIST,
                            ACCTIME = x.ACCTIME,
                            ClimbDescent = x.ClimbDescent,
                            DistRemaining = x.DistRemaining,
                            FIR = x.FIR,
                            FL = x.FL,
                            Frequency = Convert.ToDecimal(x.Frequency),
                            FuelFlow = x.FuelFlow,
                            FuelFlowPerEng = Convert.ToDecimal(x.FuelFlowPerEng),
                            FuelRemaining = Convert.ToDecimal(x.FuelRemaining),
                            FuelUsed = Convert.ToDecimal(x.FuelUsed),
                            GroundSpeed = x.GroundSpeed,
                            HLAEntryExit = x.HLAEntryExit,
                            IDENT = x.IDENT,
                            ISA = x.ISA,
                            LAT = Convert.ToDecimal(x.LAT),
                            LegAWY = x.LegAWY,
                            LegCAT = x.LegCAT,
                            LegCourse = Convert.ToDecimal(x.LegCourse),
                            LegDistance = x.LegDistance,
                            LegFuel = Convert.ToDecimal(x.LegFuel),
                            LegName = x.LegName,
                            LegTime = x.LegTime,
                            LON = Convert.ToDecimal(x.LON),
                            MagCourse = Convert.ToDecimal(x.MagCourse),
                            MagneticHeading = Convert.ToDecimal(x.MagneticHeading),
                            MagneticTrack = x.MagneticTrack,
                            MinimumEnrouteAltitude = x.MinimumEnrouteAltitude,
                            MinReqFuel = Convert.ToDecimal(x.MinReqFuel),
                            MORA = x.MORA,
                            Temperature = x.Temperature,
                            TimeRemaining = x.TimeRemaining,
                            TrueAirSpeed = x.TrueAirSpeed,
                            TrueHeading = Convert.ToDecimal(x.TrueHeading),
                            TrueTrack = x.TrueTrack,
                            VARIATION = x.VARIATION,
                            Vol = x.Vol,
                            Wind = x.Wind,
                            WindComponent = x.WindComponent,
                            PlanType = "ALT1",
                            _key = _key,
                            FRE = round_int(x.FuelRemaining).ToString(),
                            FUS = round_int(x.FuelUsed).ToString(),
                            TME = get_tme(x.LegTime),
                            TTM = get_tme(x.ACCTIME)
                        };

                        alt1_points.Add(point);
                        if (x.FlightLevelWinds != null)
                            foreach (var w in x.FlightLevelWinds)
                            {
                                _p.FlightLevelWindAlt1.Add(new FlightLevelWindAlt1()
                                {
                                    FlightLevel = w.FlightLevel,
                                    Shear = w.Shear,
                                    Temp = w.Temp,
                                    Velocity = w.Velocity,
                                    Wind = w.Wind,
                                });
                            }
                        db_flight.Alt1Points.Add(_p);

                        plan.OFPImportProps.Add(new OFPImportProp()
                        {
                            PropName = "prop_" + _key + "_a1eta_" + idx,
                            DateUpdate = DateTime.UtcNow.ToString("yyyyMMddHHmm"),
                            PropValue = "",
                            Identifier = _p.IDENT,
                            Remark = "ALT1 ROUTE",

                        });
                        //props.Add("prop_" + _key + "_eta_" + idx);
                        //props.Add("prop_" + _key + "_ata_" + idx);
                        plan.OFPImportProps.Add(new OFPImportProp()
                        {
                            PropName = "prop_" + _key + "_a1ata_" + idx,
                            DateUpdate = DateTime.UtcNow.ToString("yyyyMMddHHmm"),
                            PropValue = "",
                            Identifier = _p.IDENT,
                            Remark = "ALT1 ROUTE",

                        });
                        //props.Add("prop_" + _key + "_rem_" + idx);
                        plan.OFPImportProps.Add(new OFPImportProp()
                        {
                            PropName = "prop_" + _key + "_a1rem_" + idx,
                            DateUpdate = DateTime.UtcNow.ToString("yyyyMMddHHmm"),
                            PropValue = "",
                            Identifier = _p.IDENT,
                            Remark = "ALT1 ROUTE",

                        });
                        //props.Add("prop_" + _key + "_usd_" + idx);
                        plan.OFPImportProps.Add(new OFPImportProp()
                        {
                            PropName = "prop_" + _key + "_a1usd_" + idx,
                            DateUpdate = DateTime.UtcNow.ToString("yyyyMMddHHmm"),
                            PropValue = "",
                            Identifier = _p.IDENT,
                            Remark = "ALT1 ROUTE",

                        });
                        idx++;
                    }
                }

                var alt2_points = new List<PSSPoint>();
                // Alt2Points             // complex - skip
                idx = 0;
                if (flt_info.Alt2Points != null)
                {
                    foreach (var x in flt_info.Alt2Points)
                    {

                        var _p = new Alt2Points()
                        {
                            ACCDIST = x.ACCDIST,
                            ACCTIME = x.ACCTIME,
                            ClimbDescent = x.ClimbDescent,
                            DistRemaining = x.DistRemaining,
                            FIR = x.FIR,
                            FL = x.FL,
                            Frequency = Convert.ToDecimal(x.Frequency),
                            FuelFlow = x.FuelFlow,
                            FuelFlowPerEng = Convert.ToDecimal(x.FuelFlowPerEng),
                            FuelRemaining = Convert.ToDecimal(x.FuelRemaining),
                            FuelUsed = Convert.ToDecimal(x.FuelUsed),
                            GroundSpeed = x.GroundSpeed,
                            HLAEntryExit = x.HLAEntryExit,
                            IDENT = x.IDENT,
                            ISA = x.ISA,
                            LAT = Convert.ToDecimal(x.LAT),
                            LegAWY = x.LegAWY,
                            LegCAT = x.LegCAT,
                            LegCourse = Convert.ToDecimal(x.LegCourse),
                            LegDistance = x.LegDistance,
                            LegFuel = Convert.ToDecimal(x.LegFuel),
                            LegName = x.LegName,
                            LegTime = x.LegTime,
                            LON = Convert.ToDecimal(x.LON),
                            MagCourse = Convert.ToDecimal(x.MagCourse),
                            MagneticHeading = Convert.ToDecimal(x.MagneticHeading),
                            MagneticTrack = x.MagneticTrack,
                            MinimumEnrouteAltitude = x.MinimumEnrouteAltitude,
                            MinReqFuel = Convert.ToDecimal(x.MinReqFuel),
                            MORA = x.MORA,
                            Temperature = x.Temperature,
                            TimeRemaining = x.TimeRemaining,
                            TrueAirSpeed = x.TrueAirSpeed,
                            TrueHeading = Convert.ToDecimal(x.TrueHeading),
                            TrueTrack = x.TrueTrack,
                            VARIATION = x.VARIATION,
                            Vol = x.Vol,
                            Wind = x.Wind,
                            WindComponent = x.WindComponent,



                        };
                        var _key = "apln_WAP_" + _p.IDENT;
                        PSSPoint point = new PSSPoint()
                        {
                            ACCDIST = x.ACCDIST,
                            ACCTIME = x.ACCTIME,
                            ClimbDescent = x.ClimbDescent,
                            DistRemaining = x.DistRemaining,
                            FIR = x.FIR,
                            FL = x.FL,
                            Frequency = Convert.ToDecimal(x.Frequency),
                            FuelFlow = x.FuelFlow,
                            FuelFlowPerEng = Convert.ToDecimal(x.FuelFlowPerEng),
                            FuelRemaining = Convert.ToDecimal(x.FuelRemaining),
                            FuelUsed = Convert.ToDecimal(x.FuelUsed),
                            GroundSpeed = x.GroundSpeed,
                            HLAEntryExit = x.HLAEntryExit,
                            IDENT = x.IDENT,
                            ISA = x.ISA,
                            LAT = Convert.ToDecimal(x.LAT),
                            LegAWY = x.LegAWY,
                            LegCAT = x.LegCAT,
                            LegCourse = Convert.ToDecimal(x.LegCourse),
                            LegDistance = x.LegDistance,
                            LegFuel = Convert.ToDecimal(x.LegFuel),
                            LegName = x.LegName,
                            LegTime = x.LegTime,
                            LON = Convert.ToDecimal(x.LON),
                            MagCourse = Convert.ToDecimal(x.MagCourse),
                            MagneticHeading = Convert.ToDecimal(x.MagneticHeading),
                            MagneticTrack = x.MagneticTrack,
                            MinimumEnrouteAltitude = x.MinimumEnrouteAltitude,
                            MinReqFuel = Convert.ToDecimal(x.MinReqFuel),
                            MORA = x.MORA,
                            Temperature = x.Temperature,
                            TimeRemaining = x.TimeRemaining,
                            TrueAirSpeed = x.TrueAirSpeed,
                            TrueHeading = Convert.ToDecimal(x.TrueHeading),
                            TrueTrack = x.TrueTrack,
                            VARIATION = x.VARIATION,
                            Vol = x.Vol,
                            Wind = x.Wind,
                            WindComponent = x.WindComponent,
                            PlanType = "ALT2",
                            _key = _key,
                            FRE = round_int(x.FuelRemaining).ToString(),
                            FUS = round_int(x.FuelUsed).ToString(),
                            TME = get_tme(x.LegTime),
                            TTM = get_tme(x.ACCTIME)
                        };
                        alt2_points.Add(point);

                        if (x.FlightLevelWinds != null)
                            foreach (var w in x.FlightLevelWinds)
                            {
                                _p.FlightLevelWindAlt2.Add(new FlightLevelWindAlt2()
                                {
                                    FlightLevel = w.FlightLevel,
                                    Shear = w.Shear,
                                    Temp = w.Temp,
                                    Velocity = w.Velocity,
                                    Wind = w.Wind,
                                });
                            }
                        db_flight.Alt2Points.Add(_p);

                        plan.OFPImportProps.Add(new OFPImportProp()
                        {
                            PropName = "prop_" + _key + "_a2eta_" + idx,
                            DateUpdate = DateTime.UtcNow.ToString("yyyyMMddHHmm"),
                            PropValue = "",
                            Identifier = _p.IDENT,
                            Remark = "ALT2 ROUTE",

                        });
                        //props.Add("prop_" + _key + "_eta_" + idx);
                        //props.Add("prop_" + _key + "_ata_" + idx);
                        plan.OFPImportProps.Add(new OFPImportProp()
                        {
                            PropName = "prop_" + _key + "_a2ata_" + idx,
                            DateUpdate = DateTime.UtcNow.ToString("yyyyMMddHHmm"),
                            PropValue = "",
                            Identifier = _p.IDENT,
                            Remark = "ALT2 ROUTE",

                        });
                        //props.Add("prop_" + _key + "_rem_" + idx);
                        plan.OFPImportProps.Add(new OFPImportProp()
                        {
                            PropName = "prop_" + _key + "_a2rem_" + idx,
                            DateUpdate = DateTime.UtcNow.ToString("yyyyMMddHHmm"),
                            PropValue = "",
                            Identifier = _p.IDENT,
                            Remark = "ALT2 ROUTE",

                        });
                        //props.Add("prop_" + _key + "_usd_" + idx);
                        plan.OFPImportProps.Add(new OFPImportProp()
                        {
                            PropName = "prop_" + _key + "_a2usd_" + idx,
                            DateUpdate = DateTime.UtcNow.ToString("yyyyMMddHHmm"),
                            PropValue = "",
                            Identifier = _p.IDENT,
                            Remark = "ALT2 ROUTE",

                        });
                        idx++;
                    }
                }
                // StdAlternates          // complex - skip
                if (flt_info.StdAlternates != null)
                {
                    foreach (var x in flt_info.StdAlternates)
                        db_flight.StdAlternates.Add(new StdAlternate()
                        {
                            BlockFuel = x.BlockFuel,
                            BlockTime = (x.BlockTime),
                            Course = (x.Course),
                            Distance = x.Distance,
                            ERA = x.ERA,
                            FlightLevel = x.FlightLevel,
                            Fuel = x.Fuel,
                            IATA = x.IATA,
                            ICAO = x.ICAO,
                            Name = x.Name,
                            Route = x.Route,
                            Time = x.Time,
                            Wind = x.Wind,
                            WindVelocity = x.WindVelocity,


                        });
                }
                // CustomerData           // string[] - skip

                // RCFData                // complex - skip
                if (flt_info.RCFData != null)
                {
                    db_flight.RCFData = new RCFData()
                    {
                        AlternateFuel = flt_info.RCFData.AlternateFuel,
                        ATCRoute = flt_info.RCFData.ATCRoute,
                        ContFuel = flt_info.RCFData.ContFuel,
                        ContingencySavingAirportICAO = flt_info.RCFData.ContingencySavingAirports.ContingencySavingAirport.ICAO,
                        ContingencySavingAlternateICAO = flt_info.RCFData.ContingencySavingAirports.ContingencySavingAlternate.ICAO,
                        ContingencySavingEnRouteAlternateAirportICAO = flt_info.RCFData.ContingencySavingAirports.ContingencySavingEnRouteAlternateAirport.ICAO,
                        DecisionPoint = flt_info.RCFData.DecisionPoint,
                        ExtraFuel = flt_info.RCFData.ExtraFuel,
                        HoldingFuel = flt_info.RCFData.HoldingFuel,
                        MinReqFuel = flt_info.RCFData.MinReqFuel,
                        RCFAirport = flt_info.RCFData.RCFAirport,
                        RCFAlternate = flt_info.RCFData.RCFAlternate,
                        RCFERAAirport = flt_info.RCFData.RCFERAAirport,
                        TripFuel = flt_info.RCFData.TripFuel,
                    };
                    foreach (var x in flt_info.RCFData.RCFRoutePoints)
                    {
                        var _p = new RCFRoutePoint()
                        {
                            ACCDIST = x.ACCDIST,
                            ACCTIME = x.ACCTIME,
                            ClimbDescent = x.ClimbDescent,
                            DistRemaining = x.DistRemaining,
                            FIR = x.FIR,
                            FL = x.FL,
                            Frequency = Convert.ToDecimal(x.Frequency),
                            FuelFlow = x.FuelFlow,
                            FuelFlowPerEng = Convert.ToDecimal(x.FuelFlowPerEng),
                            FuelRemaining = Convert.ToDecimal(x.FuelRemaining),
                            FuelUsed = Convert.ToDecimal(x.FuelUsed),
                            GroundSpeed = x.GroundSpeed,
                            HLAEntryExit = x.HLAEntryExit,
                            IDENT = x.IDENT,
                            ISA = x.ISA,
                            LAT = Convert.ToDecimal(x.LAT),
                            LegAWY = x.LegAWY,
                            LegCAT = x.LegCAT,
                            LegCourse = Convert.ToDecimal(x.LegCourse),
                            LegDistance = x.LegDistance,
                            LegFuel = Convert.ToDecimal(x.LegFuel),
                            LegName = x.LegName,
                            LegTime = x.LegTime,
                            LON = Convert.ToDecimal(x.LON),
                            MagCourse = Convert.ToDecimal(x.MagCourse),
                            MagneticHeading = Convert.ToDecimal(x.MagneticHeading),
                            MagneticTrack = x.MagneticTrack,
                            MinimumEnrouteAltitude = x.MinimumEnrouteAltitude,
                            MinReqFuel = Convert.ToDecimal(x.MinReqFuel),
                            MORA = x.MORA,
                            Temperature = x.Temperature,
                            TimeRemaining = x.TimeRemaining,
                            TrueAirSpeed = x.TrueAirSpeed,
                            TrueHeading = Convert.ToDecimal(x.TrueHeading),
                            TrueTrack = x.TrueTrack,
                            VARIATION = x.VARIATION,
                            Vol = x.Vol,
                            Wind = x.Wind,
                            WindComponent = x.WindComponent,
                        };
                        if (x.FlightLevelWinds != null)
                            foreach (var w in x.FlightLevelWinds)
                            {
                                _p.FlightLevelWindRCFs.Add(new FlightLevelWindRCF()
                                {
                                    FlightLevel = w.FlightLevel,
                                    Shear = w.Shear,
                                    Temp = w.Temp,
                                    Velocity = w.Velocity,
                                    Wind = w.Wind,
                                });
                            }
                        db_flight.RCFData.RCFRoutePoints.Add(_p);
                    }
                    foreach (var x in flt_info.RCFData.RCFAltRoutePoints)
                    {
                        var _p = new RCFAltRoutePoint()
                        {
                            ACCDIST = x.ACCDIST,
                            ACCTIME = x.ACCTIME,
                            ClimbDescent = x.ClimbDescent,
                            DistRemaining = x.DistRemaining,
                            FIR = x.FIR,
                            FL = x.FL,
                            Frequency = Convert.ToDecimal(x.Frequency),
                            FuelFlow = x.FuelFlow,
                            FuelFlowPerEng = Convert.ToDecimal(x.FuelFlowPerEng),
                            FuelRemaining = Convert.ToDecimal(x.FuelRemaining),
                            FuelUsed = Convert.ToDecimal(x.FuelUsed),
                            GroundSpeed = x.GroundSpeed,
                            HLAEntryExit = x.HLAEntryExit,
                            IDENT = x.IDENT,
                            ISA = x.ISA,
                            LAT = Convert.ToDecimal(x.LAT),
                            LegAWY = x.LegAWY,
                            LegCAT = x.LegCAT,
                            LegCourse = Convert.ToDecimal(x.LegCourse),
                            LegDistance = x.LegDistance,
                            LegFuel = Convert.ToDecimal(x.LegFuel),
                            LegName = x.LegName,
                            LegTime = x.LegTime,
                            LON = Convert.ToDecimal(x.LON),
                            MagCourse = Convert.ToDecimal(x.MagCourse),
                            MagneticHeading = Convert.ToDecimal(x.MagneticHeading),
                            MagneticTrack = x.MagneticTrack,
                            MinimumEnrouteAltitude = x.MinimumEnrouteAltitude,
                            MinReqFuel = Convert.ToDecimal(x.MinReqFuel),
                            MORA = x.MORA,
                            Temperature = x.Temperature,
                            TimeRemaining = x.TimeRemaining,
                            TrueAirSpeed = x.TrueAirSpeed,
                            TrueHeading = Convert.ToDecimal(x.TrueHeading),
                            TrueTrack = x.TrueTrack,
                            VARIATION = x.VARIATION,
                            Vol = x.Vol,
                            Wind = x.Wind,
                            WindComponent = x.WindComponent,
                        };
                        if (x.FlightLevelWinds != null)
                            foreach (var w in x.FlightLevelWinds)
                            {
                                _p.FlightLevelWindRCFAlts.Add(new FlightLevelWindRCFAlt()
                                {
                                    FlightLevel = w.FlightLevel,
                                    Shear = w.Shear,
                                    Temp = w.Temp,
                                    Velocity = w.Velocity,
                                    Wind = w.Wind,
                                });
                            }
                        db_flight.RCFData.RCFAltRoutePoints.Add(_p);
                    }
                }

                db_flight.TOALT = flt_info.TOALT;
                // RouteStrings           // complex - skip
                if (flt_info.RouteStrings != null)
                {
                    db_flight.RouteString = new RouteString()
                    {
                        ToAlt = flt_info.RouteStrings.TOAlt,
                        ToAlt1 = flt_info.RouteStrings.ToAlt1,
                        ToAlt2 = flt_info.RouteStrings.ToAlt2,
                        ToDest = flt_info.RouteStrings.ToDest,
                    };
                }
                db_flight.DEPIATA = flt_info.DEPIATA;
                db_flight.DESTIATA = flt_info.DESTIATA;
                // SIDPlanned             // complex - skip
                if (flt_info.SIDPlanned != null)
                    db_flight.SIDPlanned = new SIDPlanned()
                    {
                        Distance = flt_info.SIDPlanned.Distance,
                        ProcedureName = flt_info.SIDPlanned.ProcedureName,
                        RunwayName = flt_info.SIDPlanned.RunwayName,
                    };
                // SIDAlternatives        // complex - skip
                if (flt_info.SIDAlternatives != null)
                {
                    foreach (var x in flt_info.SIDAlternatives)
                        db_flight.SIDAlternatives.Add(new SIDAlternative()
                        {
                            Distance = x.Distance,
                            ProcedureName = x.ProcedureName,
                            RunwayName = x.RunwayName,
                        });
                }
                db_flight.FinalReserveMinutes = flt_info.FinalReserveMinutes;
                db_flight.FinalReserveFuel = flt_info.FinalReserveFuel;
                db_flight.AddFuelMinutes = flt_info.AddFuelMinutes;
                db_flight.AddFuel = flt_info.AddFuel;
                db_flight.FlightSummary = flt_info.FlightSummary;
                // MelItems               // complex - skip
                if (flt_info.MelItems != null)
                {
                    foreach (var x in flt_info.MelItems)
                    {
                        var _m = new MelItem()
                        {
                            Identifier = x.Identifier,
                            Remark = x.Remark,
                        };
                        foreach (var l in x.Limitations)
                        {
                            _m.MelItemLimitations.Add(new MelItemLimitation()
                            {
                                Value = l,
                            });
                        }
                        db_flight.MelItems.Add(_m);
                    }

                }
                // PassThroughValues      // complex - skip
                if (flt_info.PassThroughValues != null)
                    foreach (var x in flt_info.PassThroughValues)
                        db_flight.PassThroughValues.Add(new PassThroughValue()
                        {
                            Name = x.Name,
                            Value = x.Value,
                        });
                db_flight.CommercialFlightNumber = flt_info.CommercialFlightNumber;
                // FreeTextItems          // complex - skip
                if (flt_info.FreeTextItems != null)
                {
                    foreach (var x in flt_info.FreeTextItems)
                        db_flight.FreeTexts.Add(new FreeText()
                        {
                            Numbering = x.Numbering,
                            Value = x.Value,
                        });
                }
                // EtopsInformation       // complex - skip
                if (flt_info.EtopsInformation != null)
                {
                    db_flight.EtopsInformation = new EtopsInformation()
                    {
                        IcingPercentage = flt_info.EtopsInformation.IcingPercentage,
                        RuleTimeUsed = flt_info.EtopsInformation.RuleTimeUsed,

                    };
                    if (flt_info.EtopsInformation.EntryPoints != null)
                    {
                        foreach (var x in flt_info.EtopsInformation.EntryPoints)
                            db_flight.EtopsInformation.EtopsEntryPoints.Add(new EtopsEntryPoint()
                            {
                                Lat = x.Position.Lat,
                                Lon = x.Position.Lon,

                            });
                    }

                    if (flt_info.EtopsInformation.ExitPoints != null)
                    {
                        foreach (var x in flt_info.EtopsInformation.ExitPoints)
                            db_flight.EtopsInformation.EtopsExitPoints.Add(new EtopsExitPoint()
                            {
                                Lat = x.Position.Lat,
                                Lon = x.Position.Lon,

                            });
                    }
                    if (flt_info.EtopsInformation.EqualTimePoints != null)
                    {
                        foreach (var x in flt_info.EtopsInformation.EqualTimePoints)
                        {
                            var _p = new EtopsEqualTimePoint()
                            {
                                Lat = x.Position.Lat,
                                Lon = x.Position.Lon,
                                Time = x.Time,


                            };
                            if (x.EqualTimeAirports != null)
                            {
                                foreach (var y in x.EqualTimeAirports)
                                    _p.EtopsEqualTimeAirports.Add(new EtopsEqualTimeAirport()
                                    {
                                        AirportHoursText = y.AirportHours.Text,
                                        Distance = y.Distance,
                                        ICAO = y.ICAO,

                                    });
                            }
                            db_flight.EtopsInformation.EtopsEqualTimePoints.Add(_p);
                        }

                    }

                    if (flt_info.EtopsInformation.Alternates != null)
                    {
                        foreach (var x in flt_info.EtopsInformation.Alternates)
                        {
                            db_flight.EtopsInformation.EtopsAlternates.Add(new EtopsAlternate()
                            {
                                AirportHoursText = x.AirportHours.Text,
                                ICAO = x.ICAO,
                                Lat = x.Position.Lat,
                                Lon = x.Position.Lon,

                            });
                        }
                    }
                    if (flt_info.EtopsInformation.EtopsLimits != null)
                    {
                        foreach (var x in flt_info.EtopsInformation.EtopsLimits)
                        {
                            db_flight.EtopsInformation.EtopsLimits.Add(new EtopsLimit()
                            {
                                MaxDistance = x.MaxDistance,
                                RuleTime = x.RuleTime,

                            });
                        }
                    }

                    if (flt_info.EtopsInformation.EtopsProfiles != null)
                    {
                        foreach (var x in flt_info.EtopsInformation.EtopsProfiles)
                        {
                            var _p = new EtopsProfile()
                            {
                                ProfileIndex = x.ProfileIndex,
                                ProfileName = x.ProfileName,

                            };
                            foreach (var y in x.EqualTimePoints)
                            {
                                var _q = new EtopsFullETP()
                                {
                                    ActualFuel = y.ActualFuel,
                                    DistanceDESTToETP = y.DistanceDESTToETP,
                                    Lat = y.Position.Lat,
                                    Lon = y.Position.Lon,
                                    Mass = y.Mass,
                                    MassAtETP = y.MassAtETP,
                                    MinimumRequiredFuel = y.MinimumRequiredFuel,
                                    RemainingFuel = y.RemainingFuel,
                                    TimeDESTToETP = y.TimeDESTToETP,
                                    TimeETPToAlternate = y.TimeETPToAlternate,

                                };
                                foreach (var z in y.Airports)
                                {
                                    _q.EtopsFullETPAirports.Add(new EtopsFullETPAirport()
                                    {
                                        AirportHoursText = z.AirportHours.Text,
                                        Distance = z.Distance,
                                        FlightLevel = z.FlightLevel,
                                        Fuel = z.Fuel,
                                        FuelReserve = z.FuelReserve,

                                    });
                                }
                                _p.EtopsFullETPs.Add(_q);
                            }

                        }
                    }


                }
                db_flight.FuelINCRBurn = flt_info.FuelINCRBurn;
                // CorrectionTable        // complex - skip
                if (flt_info.CorrectionTable != null)
                {
                    foreach (var x in flt_info.CorrectionTable)
                        db_flight.CorrectionTables.Add(new CorrectionTable()
                        {
                            CtID = x.CtID,
                            DifferentialCost = x.DifferentialCost,
                            Flightlevel = x.Flightlevel,
                            FuelForSecondProfile = x.FuelForSecondProfile,
                            FuelForSelectedProfile = x.FuelForSelectedProfile,
                            FuelForXProfile = x.FuelForXProfile,
                            TimeInHoursMinutesForAltCruiseProfile = x.TimeInHoursMinutesForAltCruiseProfile.ToString(),
                            TimeInHoursMinutesForCruiseProfile = x.TimeInHoursMinutesForCruiseProfile,
                            TimeInHoursMinutesForXProfile = x.TimeInHoursMinutesForXProfile.ToString(),
                            TimeInMinutesForAltCruiseProfile = x.TimeInMinutesForAltCruiseProfile.ToString(),
                            TimeInMinutesForCruiseProfile = x.TimeInMinutesForCruiseProfile,
                            TimeInMinutesForXProfile = x.TimeInMinutesForXProfile,
                            TotalFuelIncreaseWith10ktWind = x.TotalFuelIncreaseWith10ktWind,
                            WindCorrection = x.WindCorrection,

                        });
                }
                db_flight.ExternalFlightId = flt_info.ExternalFlightId;
                db_flight.GUFI = flt_info.GUFI.ToString();
                // PDPPoints              // complex - skip
                if (flt_info.PDPPoints != null)
                {
                    foreach (var x in flt_info.PDPPoints)
                    {
                        var _p = new PDPPoint()
                        {
                            ACCDIST = x.ACCDIST,
                            ACCTIME = x.ACCTIME,
                            ClimbDescent = x.ClimbDescent,
                            DistRemaining = x.DistRemaining,
                            FIR = x.FIR,
                            FL = x.FL,
                            Frequency = Convert.ToDecimal(x.Frequency),
                            FuelFlow = x.FuelFlow,
                            FuelFlowPerEng = Convert.ToDecimal(x.FuelFlowPerEng),
                            FuelRemaining = Convert.ToDecimal(x.FuelRemaining),
                            FuelUsed = Convert.ToDecimal(x.FuelUsed),
                            GroundSpeed = x.GroundSpeed,
                            HLAEntryExit = x.HLAEntryExit,
                            IDENT = x.IDENT,
                            ISA = x.ISA,
                            LAT = Convert.ToDecimal(x.LAT),
                            LegAWY = x.LegAWY,
                            LegCAT = x.LegCAT,
                            LegCourse = Convert.ToDecimal(x.LegCourse),
                            LegDistance = x.LegDistance,
                            LegFuel = Convert.ToDecimal(x.LegFuel),
                            LegName = x.LegName,
                            LegTime = x.LegTime,
                            LON = Convert.ToDecimal(x.LON),
                            MagCourse = Convert.ToDecimal(x.MagCourse),
                            MagneticHeading = Convert.ToDecimal(x.MagneticHeading),
                            MagneticTrack = x.MagneticTrack,
                            MinimumEnrouteAltitude = x.MinimumEnrouteAltitude,
                            MinReqFuel = Convert.ToDecimal(x.MinReqFuel),
                            MORA = x.MORA,
                            Temperature = x.Temperature,
                            TimeRemaining = x.TimeRemaining,
                            TrueAirSpeed = x.TrueAirSpeed,
                            TrueHeading = Convert.ToDecimal(x.TrueHeading),
                            TrueTrack = x.TrueTrack,
                            VARIATION = x.VARIATION,
                            Vol = x.Vol,
                            Wind = x.Wind,
                            WindComponent = x.WindComponent,



                        };
                        if (x.FlightLevelWinds != null)
                            foreach (var w in x.FlightLevelWinds)
                            {
                                _p.FlightLevelWindPDPs.Add(new FlightLevelWindPDP()
                                {
                                    FlightLevel = w.FlightLevel,
                                    Shear = w.Shear,
                                    Temp = w.Temp,
                                    Velocity = w.Velocity,
                                    Wind = w.Wind,
                                });
                            }
                        db_flight.PDPPoints.Add(_p);
                    }
                }
                // SidAndStarProcedures   // complex - skip
                if (flt_info.SidAndStarProcedures != null)
                {
                    db_flight.SidAndStarProcedure = new SidAndStarProcedure()
                    {
                        SidStarInfoSid = new SidStarInfoSid() { Info = flt_info.SidAndStarProcedures.Sid.Info, Name = flt_info.SidAndStarProcedures.Sid.Name },
                        SidStarInfoStar = new SidStarInfoStar() { Info = flt_info.SidAndStarProcedures.Star.Info, Name = flt_info.SidAndStarProcedures.Star.Name },
                    };
                }

                db_flight.FMRI = flt_info.FMRI;
                // Load                   // complex - skip
                if (flt_info.Load != null)
                {
                    var _load = new Load()
                    {
                        Cargo = new Cargo() { ActTotal = flt_info.Load.Cargo.ActTotal, },
                        Pax = new Pax()
                        {
                            Total = flt_info.Load.Pax.Total,
                            LoadPaxSection = new LoadPaxSection()
                            {
                                ActMass = flt_info.Load.Pax.PaxData.ActMass.ToString(),
                                ActPax = flt_info.Load.Pax.PaxData.ActPax,
                                Children = flt_info.Load.Pax.PaxData.Children,
                                CustMass = flt_info.Load.Pax.PaxData.CustMass.ToString(),
                                Female = flt_info.Load.Pax.PaxData.Female,
                                Infant = flt_info.Load.Pax.PaxData.Infant,
                                Male = flt_info.Load.Pax.PaxData.Male,
                                MaxPax = flt_info.Load.Pax.PaxData.MaxPax,
                                PaxAmount = flt_info.Load.Pax.PaxData.PaxAmount,

                            },


                        },
                        Payload = new Payload()
                        {
                            MaxCargo = flt_info.Load.Payload.MaxCargo,
                            MaxPayload = flt_info.Load.Payload.MaxPayload,
                            Mlm = flt_info.Load.Payload.Mlm,
                            Mrmp = flt_info.Load.Payload.Mrmp,
                            Mtom = flt_info.Load.Payload.Mtom,
                            Mzfm = flt_info.Load.Payload.Mzfm,
                        },
                        DryOperating = new DryOperating()
                        {
                            BasicEmptyArm = flt_info.Load.DryOperating.BasicEmptyArm,
                            BasicEmptyWeight = flt_info.Load.DryOperating.BasicEmptyWeight,
                            DryOperatingMassArm = flt_info.Load.DryOperating.DryOperatingMassArm,
                            DryOperatingWeight = flt_info.Load.DryOperating.DryOperatingWeight,
                        },
                        Fuel = new Fuel()
                        {
                            ActTotal = flt_info.Load.Fuel.ActTotal,

                        }

                    };
                    foreach (var x in flt_info.Load.Pax.PaxSections)
                    {
                        _load.Pax.SimplePaxSections.Add(new SimplePaxSection()
                        {
                            CustMass = x.CustMass.ToString(),
                            ActMass = x.ActMass,
                            ActPax = x.ActPax,
                            Children = x.Children,
                            Female = x.Female,
                            Infant = x.Infant,
                            Male = x.Male,
                            Row = x.Row,

                        });
                    }
                    foreach (var x in flt_info.Load.Fuel.LoadFuel)
                    {
                        _load.Fuel.LoadFuelSections.Add(new LoadFuelSection()
                        {
                            ActMass = x.ActMass.ToString(),
                            ID = x.ID,
                        });
                    }
                    _load.MassBalance = new MassBalance()
                    {
                        ArmPositionLanding = new ArmPositionLanding()
                        {
                            ActualPosition = flt_info.Load.MassBalance.Landing.ActualPosition,
                            AftLimit = flt_info.Load.MassBalance.Landing.AftLimit,
                            ForwardLimit = flt_info.Load.MassBalance.Landing.ForwardLimit,

                        },
                        ArmPositionTakeoff = new ArmPositionTakeoff()
                        {
                            ActualPosition = flt_info.Load.MassBalance.Takeoff.ActualPosition,
                            AftLimit = flt_info.Load.MassBalance.Takeoff.AftLimit,
                            ForwardLimit = flt_info.Load.MassBalance.Takeoff.ForwardLimit,

                        },
                        ArmPositionZeroFuel = new ArmPositionZeroFuel()
                        {
                            ActualPosition = flt_info.Load.MassBalance.ZeroFuel.ActualPosition,
                            AftLimit = flt_info.Load.MassBalance.ZeroFuel.AftLimit,
                            ForwardLimit = flt_info.Load.MassBalance.ZeroFuel.ForwardLimit,

                        },
                        MassBalanceIndex = new MassBalanceIndex()
                        {
                            DryOperatingIndex = flt_info.Load.MassBalance.Index.DryOperatingIndex,
                            ZeroFuelAftLimit = flt_info.Load.MassBalance.Index.ZeroFuelAftLimit,
                            ZeroFuelForwardLimit = flt_info.Load.MassBalance.Index.ZeroFuelForwardLimit,
                            ZeroFuelWeightIndex = flt_info.Load.MassBalance.Index.ZeroFuelWeightIndex,
                        }
                    };


                    db_flight.Load1 = _load;

                }
                // AircraftConfiguration  // complex - skip
                if (flt_info.AircraftConfiguration != null)
                {
                    db_flight.AircraftConfiguration = new AircraftConfiguration()
                    {
                        Name = flt_info.AircraftConfiguration.Name,
                    };
                    foreach (var x in flt_info.AircraftConfiguration.Crew)
                    {
                        db_flight.AircraftConfiguration.CrewAircraftConfigurations.Add(new CrewAircraftConfiguration()
                        {
                            CrewName = x.CrewName,
                            CrewType = x.CrewType,
                            GSM = x.GSM,
                            ID = x.ID,
                            Initials = x.Initials,
                            Mass = x.Mass.ToString(),
                        });
                    }
                }

                db_flight.IsRecalc = flt_info.IsRecalc;
                db_flight.MaxRampWeight = flt_info.MaxRampWeight;
                db_flight.UnderloadFactor = flt_info.UnderloadFactor;
                db_flight.AvgISA = flt_info.AvgISA;
                db_flight.HWCorrection20KtsTime = flt_info.HWCorrection20KtsTime;
                db_flight.HWCorrection20KtsFuel = flt_info.HWCorrection20KtsFuel;
                db_flight.Correction1TON = flt_info.Correction1TON;
                db_flight.Correction2TON = flt_info.Correction2TON;
                db_flight.RcfHeader = flt_info.RcfHeader;
                db_flight.ALT2AsInfoOnly = flt_info.ALT2AsInfoOnly;
                // PpsVersionInformation  // complex - skip
                db_flight.PpsVersionInformation = new PpsVersionInformation()
                {
                    PpsApplicationVersion = flt_info.PpsVersionInformation.PpsApplicationVersion,
                    PpsExeVersion = flt_info.PpsVersionInformation?.PpsExeVersion,
                };
                // CustomReferences       // complex - skip
                if (flt_info.CustomReferences != null)
                    db_flight.CustomReference = new CustomReference()
                    {
                        MilID = flt_info.CustomReferences.MilID,
                        RefID = flt_info.CustomReferences.RefID,
                    };
                // ToAlt1Points           // complex - skip
                if (flt_info.ToAlt1Points != null)
                {
                    foreach (var x in flt_info.ToAlt1Points)
                    {
                        var _p = new ToAlt1Points()
                        {
                            ACCDIST = x.ACCDIST,
                            ACCTIME = x.ACCTIME,
                            ClimbDescent = x.ClimbDescent,
                            DistRemaining = x.DistRemaining,
                            FIR = x.FIR,
                            FL = x.FL,
                            Frequency = Convert.ToDecimal(x.Frequency),
                            FuelFlow = x.FuelFlow,
                            FuelFlowPerEng = Convert.ToDecimal(x.FuelFlowPerEng),
                            FuelRemaining = Convert.ToDecimal(x.FuelRemaining),
                            FuelUsed = Convert.ToDecimal(x.FuelUsed),
                            GroundSpeed = x.GroundSpeed,
                            HLAEntryExit = x.HLAEntryExit,
                            IDENT = x.IDENT,
                            ISA = x.ISA,
                            LAT = Convert.ToDecimal(x.LAT),
                            LegAWY = x.LegAWY,
                            LegCAT = x.LegCAT,
                            LegCourse = Convert.ToDecimal(x.LegCourse),
                            LegDistance = x.LegDistance,
                            LegFuel = Convert.ToDecimal(x.LegFuel),
                            LegName = x.LegName,
                            LegTime = x.LegTime,
                            LON = Convert.ToDecimal(x.LON),
                            MagCourse = Convert.ToDecimal(x.MagCourse),
                            MagneticHeading = Convert.ToDecimal(x.MagneticHeading),
                            MagneticTrack = x.MagneticTrack,
                            MinimumEnrouteAltitude = x.MinimumEnrouteAltitude,
                            MinReqFuel = Convert.ToDecimal(x.MinReqFuel),
                            MORA = x.MORA,
                            Temperature = x.Temperature,
                            TimeRemaining = x.TimeRemaining,
                            TrueAirSpeed = x.TrueAirSpeed,
                            TrueHeading = Convert.ToDecimal(x.TrueHeading),
                            TrueTrack = x.TrueTrack,
                            VARIATION = x.VARIATION,
                            Vol = x.Vol,
                            Wind = x.Wind,
                            WindComponent = x.WindComponent,



                        };
                        //foreach (var w in x.FlightLevelWinds)
                        //{
                        //    _p.FlightLevelWindt.Add(new FlightLevelWindPDP()
                        //    {
                        //        FlightLevel = w.FlightLevel,
                        //        Shear = w.Shear,
                        //        Temp = w.Temp,
                        //        Velocity = w.Velocity,
                        //        Wind = w.Wind,
                        //    });
                        //}
                        db_flight.ToAlt1Points.Add(_p);
                    }
                }
                db_flight.CFMUStatus = flt_info.CFMUStatus;
                db_flight.StructuralTOM = flt_info.StructuralTOM;
                db_flight.FW1 = flt_info.FW1;
                db_flight.FW2 = flt_info.FW2;
                db_flight.FW3 = flt_info.FW3;
                db_flight.FW4 = flt_info.FW4;
                db_flight.FW5 = flt_info.FW5;
                db_flight.FW6 = flt_info.FW6;
                db_flight.FW7 = flt_info.FW7;
                db_flight.FW8 = flt_info.FW8;
                db_flight.FW9 = flt_info.FW9;
                db_flight.TOTALPAXWEIGHT = flt_info.TOTALPAXWEIGHT;
                db_flight.Alt2Dist = flt_info.Alt2Dist;
                db_flight.FMSIdent = flt_info.FMSIdent;
                // ExtraFuels             // complex - skip
                if (flt_info.ExtraFuels != null)
                {
                    foreach (var x in flt_info.ExtraFuels)
                    {
                        db_flight.ExtraFuels.Add(new ExtraFuel()
                        {
                            Fuel = x.Fuel,
                            Time = x.Time,
                            Type = x.Type,
                        });
                    }
                }
                db_flight.AircraftFuelBias = flt_info.AircraftFuelBias;
                db_flight.MelFuelBias = flt_info.MelFuelBias;

                // DepartureAlternateAirport            // complex - skip
                if (flt_info.DepartureAlternateAirport != null)
                    db_flight.DepartureAlternateAirport = new DepartureAlternateAirport()
                    {
                        ICAO = flt_info.DepartureAlternateAirport.ICAO,
                    };
                // EnRouteAlternateAirport              // complex - skip
                if (flt_info.EnRouteAlternateAirport != null)
                    db_flight.EnRouteAlternateAirport = new EnRouteAlternateAirport()
                    {
                        ICAO = flt_info.EnRouteAlternateAirport.ICAO,
                    };
                // PlanningEnRouteAlternateAirports     // complex - skip
                foreach (var x in flt_info.PlanningEnRouteAlternateAirports)
                {
                    db_flight.PlanningEnRouteAlternateAirports.Add(new PlanningEnRouteAlternateAirport()
                    {
                        ICAO = x.ICAO,
                    });
                }

                ////////PROPS  
                var fff = pps_ins.GetArinc633FlightLog(session_id, flts.Items[0].ID, "kg");
                var f1 = fff.FlightPlan;
                var f2 = fff.FlightPlan.WeightHeader.TakeoffWeight.EstimatedWeight;

                var other = new List<fuelPrm>();

                other.Add(new fuelPrm() { prm = "FPF", value = db_flight.Correction1TON.ToString() });
                props.Add("prop_fpf");
                other.Add(new fuelPrm() { prm = "FPF2", value = db_flight.Correction2TON.ToString() });
                props.Add("prop_fpf2");

                other.Add(new fuelPrm() { prm = "MACH", value = db_flight.CruiseProfile });
                props.Add("prop_mach");
                // plan.MCI= db_flight.CruiseProfile
                other.Add(new fuelPrm() { prm = "FL", value = db_flight.Fl.ToString() });
                props.Add("prop_fl");
                plan.FLL = Convert.ToDecimal(db_flight.Fl);

                other.Add(new fuelPrm() { prm = "DOW", value = (flt_info.EmptyWeight).ToString() });
                props.Add("prop_dow");
                plan.DOW = Convert.ToDecimal(flt_info.EmptyWeight);//Convert.ToDecimal( db_flight.Load1.DryOperating.DryOperatingWeight);


                other.Add(new fuelPrm() { prm = "PLD", value = round_int(Convert.ToDouble(flt_info.TrafficLoad)).ToString() });
                plan.PLD = round_int(Convert.ToDouble(flt_info.TrafficLoad)).ToString();
                props.Add("prop_pld");
                //db_flight.zfm


                other.Add(new fuelPrm() { prm = "EZFW", value = Math.Round(Convert.ToDouble(flt_info.ZFM)).ToString() });
                props.Add("prop_ezfw");
                plan.EZFW = Math.Round(Convert.ToDouble(flt_info.ZFM)).ToString();
                other.Add(new fuelPrm() { prm = "MZFW", value = Math.Round(flt_info.MaxZFM).ToString() });
                plan.mzfw = (int)Math.Round(flt_info.MaxZFM, MidpointRounding.AwayFromZero);
                props.Add("prop_mzfw");

                other.Add(new fuelPrm() { prm = "ETOW", value = round_int(Convert.ToDouble(flt_info.ActTOW)).ToString() });
                props.Add("prop_etow");
                other.Add(new fuelPrm() { prm = "MTOW", value = round_int(Convert.ToDouble(flt_info.MaxTOM)).ToString() });
                props.Add("prop_mtow");
                plan.MTOW = round_int(Convert.ToDouble(flt_info.MaxTOM)).ToString();
                plan.ETOW = round_int(Convert.ToDouble(flt_info.ActTOW)).ToString();

                other.Add(new fuelPrm() { prm = "ELDW", value = round_int(Convert.ToDouble(flt_info.Elw)).ToString() });
                props.Add("prop_eldw");
                other.Add(new fuelPrm() { prm = "MLDW", value = round_int(Convert.ToDouble(flt_info.MaxLM)).ToString() });
                props.Add("prop_mldw");
                plan.ELDW = round_int(Convert.ToDouble(flt_info.Elw)).ToString();
                plan.MLDW = round_int(Convert.ToDouble(flt_info.MaxLM)).ToString();





                plan.RTM = flt_info.RouteStrings.ToDest;



                plan.RTA = flt_info.RouteStrings.ToAlt1;


                plan.RTB = flt_info.RouteStrings.ToAlt2;
                // var rtt = infoRows.FirstOrDefault(q => q.StartsWith("RTT")).Split('=')[1];

                plan.RTT = flt_info.RouteStrings.TOAlt;
                // var thm = infoRows.FirstOrDefault(q => q.StartsWith("THM")).Split('=')[1];

                plan.THM = "FMACH";
                // var unt = infoRows.FirstOrDefault(q => q.StartsWith("UNT")).Split('=')[1];

                plan.UNT = "KGS";



                plan.CRW = "2/4";

                //plan.PLD = db_flight.Load1.Payload.MaxPayload.ToString();
                //// var ezfw = infoRows.FirstOrDefault(q => q.StartsWith("EZFW")).Split('=')[1];

                //plan.EZFW = db_flight.ZFM.ToString();
                //// var etow = infoRows.FirstOrDefault(q => q.StartsWith("ETOW")).Split('=')[1];

                //plan.ETOW = fff.FlightPlan.WeightHeader.TakeoffWeight.EstimatedWeight.ToString();
                ////  var eldw = infoRows.FirstOrDefault(q => q.StartsWith("ELDW")).Split('=')[1];

                //plan.ELDW = fff.FlightPlan.WeightHeader.LandingWeight.EstimatedWeight.ToString();
                //// var eta = infoRows.FirstOrDefault(q => q.StartsWith("ETA")).Split('=')[1];

                plan.ETA = flt_info.ETA.ToString("HH:mm");


                plan.ETD = flt_info.STD.ToString("HH:mm");


                plan.FPF = "FUEL BURN ADJUSTMENT FOR 1000KGS INCREASE IN T/O WT " + db_flight.Correction1TON.ToString() + " KG";


                //plan.MSH = MSH;

                //plan.CM1 = CM1;

                //plan.CM2 = CM2;

                //plan.DSPNAME = DSP;


                //plan.ATC = "";


                // plan.MTOW = db_flight.MaxTOM.ToString();

                // plan.MLDW = db_flight.MaxLM.ToString();

                // plan.mzfw = 0; // db_flight.ZFM.ToString();

                plan.ELDP = "";

                plan.ELDS = "";

                plan.ELAL = "";

                plan.ELBL = "";

                plan.mod1_stn = "";
                plan.mod1 = 0;

                plan.mod2_stn = "";
                plan.mod2 = 0;

                plan.ralt = "";

                plan.JPlan = "";
                plan.JAPlan1 = "";
                plan.JAPlan2 = "";

                plan.ALT1 = "";
                plan.ALT2 = "";

                other.Add(new fuelPrm() { prm = "CREW1", value = "" });
                props.Add("prop_crew1");
                other.Add(new fuelPrm() { prm = "CREW2", value = "" });
                props.Add("prop_crew2");

                other.Add(new fuelPrm() { prm = "CREW3", value = "" });
                props.Add("prop_crew3");

                other.Add(new fuelPrm() { prm = "CREW4", value = "" });
                props.Add("prop_crew4");

                other.Add(new fuelPrm() { prm = "CREW5", value = "" });
                props.Add("prop_crew5");




                other.Add(new fuelPrm() { prm = "ETD", value = flt_info.STD.ToString("HH:mm") });
                props.Add("prop_etd");

                other.Add(new fuelPrm() { prm = "ETA", value = flt_info.ETA.ToString("HH:mm") });
                props.Add("prop_eta");

                other.Add(new fuelPrm() { prm = "RTM", value = flt_info.RouteStrings.ToDest });
                props.Add("prop_rtm");

                other.Add(new fuelPrm() { prm = "RTA", value = flt_info.RouteStrings.ToAlt1 });
                props.Add("prop_rta");
                other.Add(new fuelPrm() { prm = "RTB", value = flt_info.RouteStrings.ToAlt2 });
                props.Add("prop_rtb");
                other.Add(new fuelPrm() { prm = "RTT", value = flt_info.RouteStrings.TOAlt });
                props.Add("prop_rtt");

                other.Add(new fuelPrm() { prm = "THM", value = "FMACH" });
                props.Add("prop_thm");

                other.Add(new fuelPrm() { prm = "UNT", value = "KGS" });
                props.Add("prop_unt");

                other.Add(new fuelPrm() { prm = "CRW", value = "2/4" });
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

                props.Add("prop_ldg_va");
                props.Add("prop_ldg_vsl");
                props.Add("prop_ldg_vf15");
                props.Add("prop_ldg_vapp");
                props.Add("prop_ldg_vga");
                props.Add("prop_ldg_conf");
                props.Add("prop_ldg_rwy");
                props.Add("prop_ldg_cond");


                var dtupd = DateTime.UtcNow.ToString("yyyyMMddHHmm");


                foreach (var prop in props)
                    plan.OFPImportProps.Add(new OFPImportProp()
                    {
                        DateUpdate = dtupd,
                        PropName = prop,
                        PropValue = "",
                        User = "airpocket",

                    });

                var fuellll = f1.FuelHeader.TripFuel;
                var min_to = db_flight.FUELMINTO;


                var mmdmd = flt_info.FUELMIN;
                var mmdmd1 = flt_info.FUELMINTO;

                var fuel = new List<fuelPrm>()
                {
                     new fuelPrm { prm = "TRIP FUEL",   time = to_fuel_duration(f1.FuelHeader.TripFuel.Duration.Value),
                         value =round_int( flt_info.TripFuel).ToString(),  _key = "fuel_tripfuel" },


                     new fuelPrm { prm = "CONT[5%]",    time = db_flight.TIMECONT ,
                         value =round_int(Convert.ToDouble( flt_info.FUELCONT)).ToString(),   _key = "fuel_cont05" },
                     new fuelPrm() { prm = "ALTN 1", time = to_time(flt_info.AltTime),
                         value = flt_info.AltFuel.ToString(), _key = "fuel_altn1" },
                     new fuelPrm() { prm = "ALTN 2", time = to_time(flt_info.Alt2Time), value = flt_info.Alt2Fuel.ToString(), _key = "fuel_altn2" },
                     new fuelPrm { prm = "HOLD",         time = to_time((int)db_flight.HoldTime),             value =round_int( Convert.ToDouble( flt_info.HoldFuel)).ToString()  ,    _key = "fuel_hold" },
                };
                if (f1.FuelHeader.AdditionalFuels != null && f1.FuelHeader.AdditionalFuels.Count() > 0)
                    fuel.Add(new fuelPrm { prm = "ADDI", time = to_time(flt_info.AddFuelMinutes), value = flt_info.AddFuel.ToString(), _key = "fuel_addi" });
                fuel.Add(new fuelPrm { prm = "COMP", time = db_flight.TIMECOMP, value = db_flight.FUELCOMP, _key = "fuel_comp" });
                fuel.Add(new fuelPrm
                {
                    prm = "REQD",
                    time = db_flight.TIMEMINTO,
                    value = round_int(Convert.ToDouble(flt_info.FUELMINTO)).ToString(),
                    _key = "fuel_reqd"
                });
                fuel.Add(new fuelPrm
                {
                    prm = "TAXI",
                    time = db_flight.TIMETAXI,
                    value = round_int(Convert.ToDouble(flt_info.FUELTAXI)).ToString(),
                    _key = "fuel_taxi"
                });
                if (f1.FuelHeader.ExtraFuels != null && f1.FuelHeader.ExtraFuels.Count() > 0)
                    fuel.Add(new fuelPrm
                    {
                        prm = "XTRA",
                        time = flt_info.TIMEEXTRA,
                        value = round_int(Convert.ToDouble(flt_info.FUELEXTRA)).ToString(),
                        _key = "fuel_xtra"
                    });

                // fuel.Add(new fuelPrm { prm = "FINAL RES", time = to_time((int)db_flight.FinalReserveMinutes), value = flt_info.FinalReserveFuel.ToString(), _key = "fuel_finalres" });

                fuel.Add(new fuelPrm
                {
                    prm = "TOTAL",
                    time = db_flight.TIMEACT,
                    value = round_int(Convert.ToDouble(flt_info.FUELACT)).ToString(),
                    _key = "fuel_total"
                });



                ops_flight.OFPTRIPFUEL = round_int(flt_info.TripFuel);
                ops_flight.OFPCONTFUEL = round_int(Convert.ToDouble(flt_info.FUELCONT));
                ops_flight.OFPALT1FUEL = flt_info.AltFuel;
                ops_flight.OFPALT2FUEL = flt_info.Alt2Fuel;
                ops_flight.OFPMINTOFFUEL = round_int(Convert.ToDouble(flt_info.FUELMINTO));
                ops_flight.OFPTANKERINGFUEL = round_int(Convert.ToDouble(flt_info.FUELEXTRA));
                ops_flight.OFPTAXIFUEL = round_int(Convert.ToDouble(flt_info.FUELTAXI));
                ops_flight.OFPTOTALFUEL = round_int(Convert.ToDouble(flt_info.FUELACT));

                ops_flight.ALT1 = flt_info.ALT1;
                ops_flight.ALT2 = flt_info.ALT2;

                plan.FPTripFuel = Convert.ToDecimal(flt_info.TripFuel);
                //plan.MCI = 0;
                plan.FuelALT1 = flt_info.AltFuel;
                plan.FuelALT2 = flt_info.Alt2Fuel;
                plan.FuelTAXI = round_int(Convert.ToDouble(flt_info.FUELTAXI));
                plan.FuelCONT = round_int(Convert.ToDouble(flt_info.FUELCONT));
                plan.FuelMINTOF = round_int(Convert.ToDouble(flt_info.FUELMINTO));
                plan.FuelOPSEXTRA = round_int(Convert.ToDouble(flt_info.FUELEXTRA));
                plan.FuelTOTALFUEL = round_int(Convert.ToDouble(flt_info.FUELACT));

                plan.VDT = "OFP Generated On: " + flt_info.LastEditDate.ToString("dd MMM yyyy HH:mm") + "  and Valid until: " + flt_info.LastEditDate.AddHours(24).ToString("dd MMM yyyy HH:mm") + " (UTC)";

                string[] lines = flt_info.FlightSummary.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
                var lnr = lines.Where(q => q.Contains("LOG NR:")).FirstOrDefault();
                if (!string.IsNullOrEmpty(lnr))
                    plan.DID = "LOG NR: " + (lnr.Split(new[] { "LOG NR:" }, StringSplitOptions.None))[1];



                //to_time



                //  if (f1.FuelHeader.ArrivalFuel != null)
                //     fuel.Add(new fuelPrm { prm = "ARR", time = "", value = f1.FuelHeader.ArrivalFuel.EstimatedWeight.Value.Text[0], _key = "fuel_altn1" });


                //   if (f1.FuelHeader.LandingFuel != null)
                //      fuel.Add(new fuelPrm { prm = "LND", time = "", value = f1.FuelHeader.LandingFuel.EstimatedWeight.Value.Text[0], _key = "fuel_altn1" });



                //if (f1.FuelHeader.ETOPSFuel != null)
                //    fuel.Add(new fuelPrm { prm = "ETOPS", time = to_fuel_duration(f1.FuelHeader.ETOPSFuel.Duration.Value), value = f1.FuelHeader.ETOPSFuel.EstimatedWeight.Value.Text[0], _key = "fuel_etops" });





                // if (f1.FuelHeader.PossibleExtraFuel != null  )
                //     fuel.Add(new fuelPrm { prm = "POSS EXTRA", time = to_fuel_duration(f1.FuelHeader.PossibleExtraFuel. .Duration.Value), value = "3373", _key = "fuel_altn1" });

                // fuel.Add(new fuelPrm() { prm = "ALTN 1", time = to_time(flt_info.AltTime), value = flt_info.AltFuel.ToString(), _key = "fuel_altn1" });
                //  fuel.Add(new fuelPrm() { prm = "ALTN 2", time = to_time(flt_info.Alt2Time), value = flt_info.Alt2Fuel.ToString(), _key = "fuel_altn2" });

                //fuel.Add(new fuelPrm()
                //{
                //    prm = prm,
                //    time = tim,
                //    value = val,
                //    _key = _key,
                //});

                ///////////////////////////////

                var _fuel = JsonConvert.SerializeObject(fuel);

                plan.JAPlan1 = JsonConvert.SerializeObject(alt1_points);
                plan.JAPlan2 = JsonConvert.SerializeObject(alt2_points);
                plan.JPlan = JsonConvert.SerializeObject(main_points);
                plan.JFuel = _fuel;

                plan.FlightNo = flt_info.FlightLogID;
                plan.Destination = flt_info.DEST;
                plan.Origin = flt_info.DEP;
                plan.DateFlight = flt_info.STD.Date;
                plan.DateCreate = DateTime.Now;
                plan.ALT1 = flt_info.ALT1;
                plan.ALT2 = flt_info.ALT2;
                plan.DSPNAME = flt_info.DISP;



                context.OFPImports.Add(plan);
                context.Flights.Add(db_flight);
                var save_result = await context.SaveAsync();
                return Ok(session_id);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                return Ok(msg);
            }

        }
        string to_fuel_duration(string str)
        {
            string input = "PT1H17M";

            // حذف "PT"
            string body = input.Substring(2);   // "1H17M"

            // جدا کردن بر اساس H و M
            string[] parts = body.Split(new[] { 'H', 'M' }, StringSplitOptions.RemoveEmptyEntries);

            // parts[0] = "1" → ساعت
            // parts[1] = "17" → دقیقه
            int hours = int.Parse(parts[0]);
            int minutes = int.Parse(parts[1]);

            TimeSpan ts = new TimeSpan(hours, minutes, 0);
            string result = ts.ToString(@"hh\:mm\:ss");   // 01:17:00
            return result;
        }

        string to_time(int v)
        {
            var hh = v / 60;
            var mm = v % 60;
            return hh.ToString().PadLeft(2, '0') + ":" + mm.ToString().PadLeft(2, '0');
        }

        int round_int(double num)
        {
            var result = (int)Math.Round(num, MidpointRounding.AwayFromZero);
            return result;
        }
        public class fuelPrm
        {
            public string prm { get; set; }
            public string time { get; set; }
            public string value { get; set; }

            public string _key { get; set; }
        }

    }
}
