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
                var flts = pps_ins.GetSTDFlightList(session_id, new DateTime(2025, 9, 28), new DateTime(2025, 9, 30));
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
                            AirportHoursText =x.AirportHours ? x.AirportHours.Text :null,
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
                    }
                }
                // Alt2Points             // complex - skip
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
                db_flight.DepartureAlternateAirport = new DepartureAlternateAirport()
                {
                    ICAO = flt_info.DepartureAlternateAirport.ICAO,
                };
                // EnRouteAlternateAirport              // complex - skip
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
