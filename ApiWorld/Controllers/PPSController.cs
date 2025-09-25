using ApiWorld.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace ApiWorld.Controllers
{
    public class PPSController : ApiController
    {
        [Route("api/pps/test")]
        public async Task<IHttpActionResult> GetPPSTest()
        {
            try
            {
                pps.EfbService pps_ins = new pps.EfbService();
                var session_id = pps_ins.GetSessionID("ArmeniaAirways", "J8V14HNHK", "AMWINTEGRATION", "i$qn719e");
                var flts = pps_ins.GetSTDFlightList(session_id, new DateTime(2025, 8, 27), new DateTime(2025, 8, 29));
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


        [Route("api/pps/test/save")]
        public async Task<IHttpActionResult> GetPPSTestSave()
        {
            try
            {
                ppa_entities context = new ppa_entities();

                pps.EfbService pps_ins = new pps.EfbService();
                var session_id = pps_ins.GetSessionID("ArmeniaAirways", "J8V14HNHK", "AMWINTEGRATION", "i$qn719e");
                var flts = pps_ins.GetSTDFlightList(session_id, new DateTime(2025, 8, 27), new DateTime(2025, 8, 29));
                pps.Flight flt_info = pps_ins.GetFlight(session_id, flts.Items[0].ID, true, true, true, true, "kg");

                Models.Flight db_flight = new Flight();


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
                
                db_flight.DEPMetar = flt_info.DEPMetar;
                // DEPNotam               // complex - skip
                // DESTTAF                // complex - skip
                db_flight.DESTMetar = flt_info.DESTMetar;
                // DESTNotam              // complex - skip
                // ALT1TAF                // complex - skip
                // ALT2TAF                // complex - skip
                db_flight.ALT1Metar = flt_info.ALT1Metar;
                db_flight.ALT2Metar = flt_info.ALT2Metar;
                // ALT1Notam              // complex - skip
                // ALT2Notam              // complex - skip
                // RoutePoints            // complex - skip
                // Crews                  // complex - skip
                // Responce               // complex - skip
                // ATCData                // complex - skip
                // NextLeg                // complex - skip
                // OptFlightLevels        // complex - skip
                // AdequateNotam          // complex - skip
                // FIRNotam               // complex - skip
                // AlternateNotam         // complex - skip
                // Airports               // complex - skip
                // EnrouteAlternates      // string[] - skip
                // Alt1Points             // complex - skip
                // Alt2Points             // complex - skip
                // StdAlternates          // complex - skip
                // CustomerData           // string[] - skip
                // RCFData                // complex - skip
                db_flight.TOALT = flt_info.TOALT;
                // RouteStrings           // complex - skip
                db_flight.DEPIATA = flt_info.DEPIATA;
                db_flight.DESTIATA = flt_info.DESTIATA;
                // SIDPlanned             // complex - skip
                // SIDAlternatives        // complex - skip
                db_flight.FinalReserveMinutes = flt_info.FinalReserveMinutes;
                db_flight.FinalReserveFuel = flt_info.FinalReserveFuel;
                db_flight.AddFuelMinutes = flt_info.AddFuelMinutes;
                db_flight.AddFuel = flt_info.AddFuel;
                db_flight.FlightSummary = flt_info.FlightSummary;
                // MelItems               // complex - skip
                // PassThroughValues      // complex - skip
                db_flight.CommercialFlightNumber = flt_info.CommercialFlightNumber;
                // FreeTextItems          // complex - skip
                // EtopsInformation       // complex - skip
                db_flight.FuelINCRBurn = flt_info.FuelINCRBurn;
                // CorrectionTable        // complex - skip
                db_flight.ExternalFlightId = flt_info.ExternalFlightId;
                db_flight.GUFI = flt_info.GUFI.ToString();
                // PDPPoints              // complex - skip
                // SidAndStarProcedures   // complex - skip
                db_flight.FMRI = flt_info.FMRI;
                // Load                   // complex - skip
                // AircraftConfiguration  // complex - skip
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
                // CustomReferences       // complex - skip
                // ToAlt1Points           // complex - skip
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
                db_flight.AircraftFuelBias = flt_info.AircraftFuelBias;
                db_flight.MelFuelBias = flt_info.MelFuelBias;
                // DepartureAlternateAirport            // complex - skip
                // EnRouteAlternateAirport              // complex - skip
                // PlanningEnRouteAlternateAirports     // complex - skip


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



    }
}
