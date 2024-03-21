using ApiLog.Models;
using ApiLog.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ApiLog.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class LogController : ApiController
    {
        [Route("api/flight/log/save")]

        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostFlightLog(ViewModels.FlightSaveDto dto)
        {
            try
            {
                if (dto.UpdateDelays == null)
                    dto.UpdateDelays = 1;
                List<int> offCrewIds = new List<int>();
                //marmar
                // return new CustomActionResult(HttpStatusCode.OK, null);
                var context = new ppa_entities();
                var flight = await context.FlightInformations.FirstOrDefaultAsync(q => q.ID == dto.ID);
                var notifiedDelay = flight.NotifiedDelay;
                var leg = await context.ViewLegTimes.FirstOrDefaultAsync(q => q.ID == dto.ID);
                if (flight == null)
                    return new CustomActionResult(HttpStatusCode.NotFound, "");

                var changeLog = new FlightChangeHistory()
                {
                    Date = DateTime.Now,
                    FlightId = flight.ID,
                    User = dto.UserName,

                };
                changeLog.OldFlightNumer = leg.FlightNumber;
                changeLog.OldFromAirportId = leg.FromAirport;
                changeLog.OldToAirportId = leg.ToAirport;
                changeLog.OldSTD = flight.STD;
                changeLog.OldSTA = flight.STA;
                changeLog.OldStatusId = flight.FlightStatusID;
                changeLog.OldRegister = leg.RegisterID;
                changeLog.OldOffBlock = flight.ChocksOut;
                changeLog.OldOnBlock = flight.ChocksIn;
                changeLog.OldTakeOff = flight.Takeoff;
                changeLog.OldLanding = flight.Landing;

                //////////////////////////////////////////////////////////////
                flight.GUID = Guid.NewGuid();
                flight.DateCreate = DateTime.Now.ToUniversalTime();
                flight.FlightStatusUserId = dto.UserId;
                flight.ChocksIn = dto.ChocksIn;
                flight.Landing = dto.Landing;
                flight.ChocksOut = dto.ChocksOut;
                flight.Takeoff = dto.Takeoff;
                flight.GWTO = dto.GWTO;
                if (!string.IsNullOrEmpty(dto.LTR))
                    flight.LTR = dto.LTR;
                if (!string.IsNullOrEmpty(dto.SerialNo))
                    flight.SerialNo = dto.SerialNo;
                if (dto.FuelDensity != null)
                    flight.FuelDensity = dto.FuelDensity;

                flight.FuelDeparture = dto.FuelDeparture;
                flight.FuelArrival = dto.FuelArrival;
                flight.FPFuel = dto.FPFuel;
                flight.Defuel = dto.Defuel;
                flight.UsedFuel = dto.UsedFuel;


                flight.PaxAdult = dto.PaxAdult;
                flight.PaxInfant = dto.PaxInfant;
                flight.PaxChild = dto.PaxChild;
                flight.NightTime = dto.NightTime;


                //////new fuel
                flight.FuelRemained = dto.FuelRemained;
                flight.FuelRemaining = dto.FuelRemaining;
                flight.FuelUplift = dto.FuelUplift;
                flight.FuelUpliftLitr = dto.FuelUpliftLitr;
                flight.FuelTotal = dto.FuelTotal;
                flight.FuelUsed = dto.FuelUsed;

                ////////NEW LOG
                /// 
                flight.CargoWeight = dto.CargoWeight;
                flight.CargoCost = dto.CargoCost;
                flight.CargoUnitID = dto.CargoUnitID;
                flight.BaggageCount = dto.BaggageCount;
                flight.CargoCount = dto.CargoCount;
                flight.FreeAWBWeight = dto.FreeAWBWeight;
                flight.FreeAWBCount = dto.FreeAWBCount;
                flight.BaggageWeight = dto.BaggageWeight;

                flight.NoShowCount = dto.NoShowCount;
                flight.NoShowPieces = dto.NoShowPieces;
                flight.NoGoCount = dto.NoGoCount;
                flight.NoGoPieces = dto.NoGoPieces;
                flight.DSBreakfast = dto.DSBreakfast;
                flight.DSWarmFood = dto.DSWarmFood;
                flight.DSColdFood = dto.DSColdFood;
                flight.DSRefreshment = dto.DSRefreshment;

                flight.Ready = String.IsNullOrEmpty(dto.Ready) ? (DateTime?)null : DateTime.ParseExact(dto.Ready, "yyyyMMddHHmm", CultureInfo.InvariantCulture);
                flight.Start = String.IsNullOrEmpty(dto.Start) ? (DateTime?)null : DateTime.ParseExact(dto.Start, "yyyyMMddHHmm", CultureInfo.InvariantCulture);
                flight.YClass = dto.YClass;
                flight.CClass = dto.CClass;
                flight.PaxAdult50 = dto.PaxAdult50;
                flight.PaxChild50 = dto.PaxChild50;
                flight.PaxInfant50 = dto.PaxInfant50;
                flight.PaxAdult100 = dto.PaxAdult100;
                flight.PaxChild100 = dto.PaxChild100;
                flight.PaxInfant100 = dto.PaxInfant100;
                flight.PaxVIP = dto.PaxVIP;
                flight.PaxCIP = dto.PaxCIP;
                flight.PaxHUM = dto.PaxHUM;
                flight.PaxUM = dto.PaxUM;
                flight.PaxAVI = dto.PaxAVI;
                flight.PaxWCHR = dto.PaxWCHR;
                flight.PaxSTRC = dto.PaxSTRC;
                flight.FreeAWBPieces = dto.FreeAWBPieces;
                flight.CargoPieces = dto.CargoPieces;
                flight.PaxPIRLost = dto.PaxPIRLost;
                flight.PaxPIRDamage = dto.PaxPIRDamage;
                flight.PaxPIRFound = dto.PaxPIRFound;
                flight.CargoPIRLost = dto.CargoPIRLost;
                flight.CargoPIRDamage = dto.CargoPIRDamage;
                flight.CargoPIRFound = dto.CargoPIRFound;
                flight.LimitTag = dto.LimitTag;
                flight.RushTag = dto.RushTag;
                flight.CLCheckIn = String.IsNullOrEmpty(dto.CLCheckIn) ? (DateTime?)null : DateTime.ParseExact(dto.CLCheckIn, "yyyyMMddHHmm", CultureInfo.InvariantCulture);
                flight.CLPark = String.IsNullOrEmpty(dto.CLPark) ? (DateTime?)null : DateTime.ParseExact(dto.CLPark, "yyyyMMddHHmm", CultureInfo.InvariantCulture);
                flight.CLAddTools = String.IsNullOrEmpty(dto.CLAddTools) ? (DateTime?)null : DateTime.ParseExact(dto.CLAddTools, "yyyyMMddHHmm", CultureInfo.InvariantCulture);
                flight.CLBusReady = String.IsNullOrEmpty(dto.CLBusReady) ? (DateTime?)null : DateTime.ParseExact(dto.CLBusReady, "yyyyMMddHHmm", CultureInfo.InvariantCulture);
                flight.CLPaxOut = String.IsNullOrEmpty(dto.CLPaxOut) ? (DateTime?)null : DateTime.ParseExact(dto.CLPaxOut, "yyyyMMddHHmm", CultureInfo.InvariantCulture);
                flight.CLDepoOut = String.IsNullOrEmpty(dto.CLDepoOut) ? (DateTime?)null : DateTime.ParseExact(dto.CLDepoOut, "yyyyMMddHHmm", CultureInfo.InvariantCulture);
                flight.CLServicePresence = String.IsNullOrEmpty(dto.CLServicePresence) ? (DateTime?)null : DateTime.ParseExact(dto.CLServicePresence, "yyyyMMddHHmm", CultureInfo.InvariantCulture);
                flight.CLCleaningStart = String.IsNullOrEmpty(dto.CLCleaningStart) ? (DateTime?)null : DateTime.ParseExact(dto.CLCleaningStart, "yyyyMMddHHmm", CultureInfo.InvariantCulture);
                flight.CLTechReady = String.IsNullOrEmpty(dto.CLTechReady) ? (DateTime?)null : DateTime.ParseExact(dto.CLTechReady, "yyyyMMddHHmm", CultureInfo.InvariantCulture);
                flight.CLBagSent = String.IsNullOrEmpty(dto.CLBagSent) ? (DateTime?)null : DateTime.ParseExact(dto.CLBagSent, "yyyyMMddHHmm", CultureInfo.InvariantCulture);
                flight.CLCateringLoad = String.IsNullOrEmpty(dto.CLCateringLoad) ? (DateTime?)null : DateTime.ParseExact(dto.CLCateringLoad, "yyyyMMddHHmm", CultureInfo.InvariantCulture);
                flight.CLFuelStart = String.IsNullOrEmpty(dto.CLFuelStart) ? (DateTime?)null : DateTime.ParseExact(dto.CLFuelStart, "yyyyMMddHHmm", CultureInfo.InvariantCulture);
                flight.CLFuelEnd = String.IsNullOrEmpty(dto.CLFuelEnd) ? (DateTime?)null : DateTime.ParseExact(dto.CLFuelEnd, "yyyyMMddHHmm", CultureInfo.InvariantCulture);
                flight.CLCleaningEnd = String.IsNullOrEmpty(dto.CLCleaningEnd) ? (DateTime?)null : DateTime.ParseExact(dto.CLCleaningEnd, "yyyyMMddHHmm", CultureInfo.InvariantCulture);
                flight.CLBoardingStart = String.IsNullOrEmpty(dto.CLBoardingStart) ? (DateTime?)null : DateTime.ParseExact(dto.CLBoardingStart, "yyyyMMddHHmm", CultureInfo.InvariantCulture);
                flight.CLBoardingEnd = String.IsNullOrEmpty(dto.CLBoardingEnd) ? (DateTime?)null : DateTime.ParseExact(dto.CLBoardingEnd, "yyyyMMddHHmm", CultureInfo.InvariantCulture);
                flight.CLLoadSheetStart = String.IsNullOrEmpty(dto.CLLoadSheetStart) ? (DateTime?)null : DateTime.ParseExact(dto.CLLoadSheetStart, "yyyyMMddHHmm", CultureInfo.InvariantCulture);
                flight.CLGateClosed = String.IsNullOrEmpty(dto.CLGateClosed) ? (DateTime?)null : DateTime.ParseExact(dto.CLGateClosed, "yyyyMMddHHmm", CultureInfo.InvariantCulture);
                flight.CLTrafficCrew = String.IsNullOrEmpty(dto.CLTrafficCrew) ? (DateTime?)null : DateTime.ParseExact(dto.CLTrafficCrew, "yyyyMMddHHmm", CultureInfo.InvariantCulture);
                flight.CLLoadCrew = String.IsNullOrEmpty(dto.CLLoadCrew) ? (DateTime?)null : DateTime.ParseExact(dto.CLLoadCrew, "yyyyMMddHHmm", CultureInfo.InvariantCulture);
                flight.CLForbiddenObj = String.IsNullOrEmpty(dto.CLForbiddenObj) ? (DateTime?)null : DateTime.ParseExact(dto.CLForbiddenObj, "yyyyMMddHHmm", CultureInfo.InvariantCulture);
                flight.CLLoadSheetSign = String.IsNullOrEmpty(dto.CLLoadSheetSign) ? (DateTime?)null : DateTime.ParseExact(dto.CLLoadSheetSign, "yyyyMMddHHmm", CultureInfo.InvariantCulture);
                flight.CLLoadingEnd = String.IsNullOrEmpty(dto.CLLoadingEnd) ? (DateTime?)null : DateTime.ParseExact(dto.CLLoadingEnd, "yyyyMMddHHmm", CultureInfo.InvariantCulture);
                flight.CLDoorClosed = String.IsNullOrEmpty(dto.CLDoorClosed) ? (DateTime?)null : DateTime.ParseExact(dto.CLDoorClosed, "yyyyMMddHHmm", CultureInfo.InvariantCulture);
                flight.CLEqDC = String.IsNullOrEmpty(dto.CLEqDC) ? (DateTime?)null : DateTime.ParseExact(dto.CLEqDC, "yyyyMMddHHmm", CultureInfo.InvariantCulture);
                flight.CLMotorStart = String.IsNullOrEmpty(dto.CLMotorStart) ? (DateTime?)null : DateTime.ParseExact(dto.CLMotorStart, "yyyyMMddHHmm", CultureInfo.InvariantCulture);
                flight.CLMovingStart = String.IsNullOrEmpty(dto.CLMovingStart) ? (DateTime?)null : DateTime.ParseExact(dto.CLMovingStart, "yyyyMMddHHmm", CultureInfo.InvariantCulture);
                flight.CLACStart = String.IsNullOrEmpty(dto.CLACStart) ? (DateTime?)null : DateTime.ParseExact(dto.CLACStart, "yyyyMMddHHmm", CultureInfo.InvariantCulture);
                flight.CLACEnd = String.IsNullOrEmpty(dto.CLACEnd) ? (DateTime?)null : DateTime.ParseExact(dto.CLACEnd, "yyyyMMddHHmm", CultureInfo.InvariantCulture);
                flight.CLGPUStart = String.IsNullOrEmpty(dto.CLGPUStart) ? (DateTime?)null : DateTime.ParseExact(dto.CLGPUStart, "yyyyMMddHHmm", CultureInfo.InvariantCulture);
                flight.CLGPUEnd = String.IsNullOrEmpty(dto.CLGPUEnd) ? (DateTime?)null : DateTime.ParseExact(dto.CLGPUEnd, "yyyyMMddHHmm", CultureInfo.InvariantCulture);
                flight.CLDepStairs = dto.CLDepStairs;
                flight.CLDepGPU = dto.CLDepGPU;
                flight.CLDepCrewCar = dto.CLDepCrewCar;
                flight.CLDepCrewCarCount = dto.CLDepCrewCarCount;
                flight.CLDepCabinService = dto.CLDepCabinService;
                flight.CLDepCateringCar = dto.CLDepCateringCar;
                flight.CLDepPatientCar = dto.CLDepPatientCar;
                flight.CLDepPaxCar = dto.CLDepPaxCar;
                flight.CLDepPaxCarCount = dto.CLDepPaxCarCount;
                flight.CLDepPushback = dto.CLDepPushback;
                flight.CLDepWaterService = dto.CLDepWaterService;
                flight.CLDepAC = dto.CLDepAC;
                flight.CLDepDeIce = dto.CLDepDeIce;
                flight.CLDepEqRemark = dto.CLDepEqRemark;
                flight.CLArrStairs = dto.CLArrStairs;
                flight.CLArrGPU = dto.CLArrGPU;
                flight.CLArrCrewCar = dto.CLArrCrewCar;
                flight.CLArrCrewCarCount = dto.CLArrCrewCarCount;
                flight.CLArrCabinService = dto.CLArrCabinService;
                flight.CLArrPatientCar = dto.CLArrPatientCar;
                flight.CLArrPaxCar = dto.CLArrPaxCar;
                flight.CLArrPaxCarCount = dto.CLArrPaxCarCount;
                flight.CLArrToiletService = dto.CLArrToiletService;
                flight.CLArrEqRemark = dto.CLArrEqRemark;
                ///END NEW LOG
                //////////////////////

                //flight.FuelUnitID = dto.FuelUnitID;
                flight.DepartureRemark = dto.DepartureRemark;
                flight.FPFlightHH = dto.FPFlightHH;
                flight.FPFlightMM = dto.FPFlightMM;
                //flight.FPFuel = dto.FPFuel;
                flight.Defuel = dto.Defuel;
                // flight.UsedFuel = dto.UsedFuel;
                flight.JLBLHH = dto.JLBLHH;
                flight.JLBLMM = dto.JLBLMM;
                flight.PFLR = dto.PFLR;
                flight.ChrAdult = dto.ChrAdult;
                flight.ChrCapacity = dto.ChrCapacity;
                flight.ChrChild = dto.ChrChild;
                flight.ChrInfant = dto.ChrInfant;
                flight.ChrCode = dto.ChrCode;
                flight.ChrTitle = dto.ChrTitle;
                flight.ArrivalRemark = dto.ArrivalRemark;
                if (dto.FlightStatusID != null)
                    flight.FlightStatusID = dto.FlightStatusID;
                if (flight.FlightStatusID == null)
                    flight.FlightStatusID = 1;
                if (flight.FlightStatusID == 4)
                {
                    var cnlMsn = await context.Ac_MSN.Where(q => q.Register == "CNL").Select(q => q.ID).FirstOrDefaultAsync();
                    flight.RegisterID = cnlMsn;
                    flight.CancelDate = dto.CancelDate;
                    flight.CancelReasonId = dto.CancelReasonId;
                }
                //if (flight.FlightStatusID == 17)
                if (dto.RedirectReasonId != null)
                {

                    flight.RedirectDate = dto.RedirectDate;
                    flight.RedirectReasonId = dto.RedirectReasonId;
                    flight.RedirectRemark = dto.RedirectRemark;
                    if (flight.OSTA == null)
                    {
                        var vflight = await context.ViewFlightInformations.FirstOrDefaultAsync(q => q.ID == flight.ID);
                        flight.OSTA = flight.STA;
                        flight.OToAirportId = vflight.ToAirport;
                        flight.OToAirportIATA = vflight.ToAirportIATA;
                    }

                    // var airport = await context.Airports.FirstOrDefaultAsync(q => q.Id == flight.OToAirportId);
                    flight.ToAirportId = dto.ToAirportId;
                    // if (airport != null)
                    //    flight.OToAirportIATA = airport.IATA;
                }
                else
                {
                    flight.RedirectDate = null;
                    flight.RedirectReasonId = null;
                    // if (flight.FlightPlanId != null)
                    //    flight.ToAirportId = null;
                    flight.OSTA = null;
                    flight.OToAirportId = null;
                    flight.OToAirportIATA = null;

                }
                if (flight.FlightStatusID == 9 || dto.RampReasonId != null)
                {
                    flight.RampDate = dto.RampDate;
                    flight.RampReasonId = dto.RampReasonId;
                    flight.RampRemark = dto.RampRemark;
                }

                if (flight.ChocksIn != null && flight.FlightStatusID == 15)
                {
                    //var vflight = await context.ViewFlightInformations.FirstOrDefaultAsync(q => q.ID == flight.ID);

                    //var flightCrewEmployee = await (from x in context.Employees
                    //                                join y in context.ViewFlightCrewNews on x.Id equals y.CrewId
                    //                                where y.FlightId == flight.ID
                    //                                select x).ToListAsync();

                    //foreach (var x in flightCrewEmployee)
                    //    x.CurrentLocationAirport = vflight.ToAirport;
                    var flightCrews = await (from x in context.Employees
                                             join z in context.FDPs on x.Id equals z.CrewId
                                             join y in context.FDPItems on z.Id equals y.FDPId
                                             where y.FlightId == flight.ID
                                             select x).ToListAsync();

                    foreach (var x in flightCrews)
                        x.CurrentLocationAirport = flight.ToAirportId;
                }
                if (flight.FlightStatusID != null && /*dto.UserId != null*/ !string.IsNullOrEmpty(dto.UserName))
                    context.FlightStatusLogs.Add(new FlightStatusLog()
                    {
                        FlightID = dto.ID,

                        Date = DateTime.Now.ToUniversalTime(),
                        FlightStatusID = (int)flight.FlightStatusID,

                        UserId = dto.UserId != null ? (int)dto.UserId : 128000,
                        Remark = dto.UserName,
                    });
                var result = UpdateDelays(dto, context);

                if (result.Code != HttpStatusCode.OK)
                    return result;

                //  var result2 = await UpdateEstimatedDelays(dto);

                //if (result2.Code != HttpStatusCode.OK)
                //    return result2;

                if (flight.FlightStatusID == 4)
                {
                    UpdateFirstLastFlights(flight.ID, context);
                }


                ////////////////////////////////////////
                changeLog.NewFlightNumber = leg.FlightNumber;
                changeLog.NewFromAirportId = leg.FromAirport;
                changeLog.NewToAirportId = flight.ToAirportId;
                changeLog.NewSTD = flight.STD;
                changeLog.NewSTA = flight.STA;
                changeLog.NewStatusId = flight.FlightStatusID;
                changeLog.NewRegister = leg.RegisterID;
                changeLog.NewOffBlock = flight.ChocksOut;
                changeLog.NewOnBlock = flight.ChocksIn;
                changeLog.NewTakeOff = flight.Takeoff;
                changeLog.NewLanding = flight.Landing;

                context.FlightChangeHistories.Add(changeLog);
                ////////////////////////////////////////

                bool sendNira = false;
                try
                {
                    var xdelay = (int)(((DateTime)dto.ChocksOut) - ((DateTime)leg.STD)).TotalMinutes;
                    if (xdelay > 30 && (flight.FlightStatusID == 1) && notifiedDelay != xdelay && ((DateTime)flight.STD - DateTime.UtcNow).TotalHours > 1)
                    {
                        flight.NotifiedDelay = xdelay;
                        sendNira = true;
                    }
                    if (dto.FlightStatusID == 4)
                    {
                        // offCrewIds = (from q in context.ViewFlightCrewNews
                        //               where q.FlightId == dto.ID
                        //               select q.CrewId).ToList();
                        offCrewIds = await (from x in context.Employees
                                            join z in context.FDPs on x.Id equals z.CrewId
                                            join y in context.FDPItems on z.Id equals y.FDPId
                                            where y.FlightId == flight.ID
                                            select x.Id).ToListAsync();



                    }





                }
                catch (Exception ex)
                {

                }



                //return new CustomActionResult(HttpStatusCode.OK, new updateLogResult()
                //{
                //    sendNira = sendNira,
                //    flight = flight.ID,
                //    offIds = offCrewIds

                //});

                var saveResult = await context.SaveAsync();
                if (saveResult.Code != HttpStatusCode.OK)
                    return saveResult;

                if (offCrewIds != null && offCrewIds.Count > 0)
                {
                    var disoffIds = offCrewIds.Distinct().ToList();
                    foreach (var crewid in disoffIds)
                    {
                        await RemoveItemsFromFDP(flight.ID.ToString(), (int)crewid, 2, "Flight Cancellation - Removed by AirPocket.", 0, 0);
                    }
                }



                //var fg = await unitOfWork.FlightRepository.GetViewFlightGantts().Where(q => q.ID == fresult.flight).ToListAsync();
                var fg = await context.ViewFlightsGantts.Where(q => q.ID == flight.ID).ToListAsync();
                ViewModels.ViewFlightsGanttDto odto = new ViewFlightsGanttDto();
                ViewModels.ViewFlightsGanttDto.FillDto(fg.First(), odto, 0, 1);


                var resgroups = from x in fg
                                group x by new { x.AircraftType, AircraftTypeId = x.TypeId }
                               into grp
                                select new { groupId = grp.Key.AircraftTypeId, Title = grp.Key.AircraftType };
                var ressq = (from x in fg
                             group x by new { x.RegisterID, x.Register, x.TypeId }
                         into grp

                             orderby getOrderIndex(grp.Key.Register, new List<string>())
                             select new { resourceId = grp.Key.RegisterID, resourceName = grp.Key.Register, groupId = grp.Key.TypeId }).ToList();

                odto.resourceId.Add((int)odto.RegisterID);


                var oresult = new
                {
                    flight = odto,
                    resgroups,
                    ressq
                };
                //await unitOfWork.FlightRepository.CreateMVTMessage(dto.ID,dto.UserName);
                //6-28
                // unitOfWork.FlightRepository.CreateMVTMessage(dto.ID, dto.UserName);
                return Ok(oresult);



            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }

        }

        internal string UpdateFirstLastFlights(int flightId, ppa_entities context)
        //(int fdpId,int fdpItemId,bool off)
        {

            var fdps = (from x in context.FDPs
                        join y in context.FDPItems on x.Id equals y.FDPId
                        where y.FlightId == flightId
                        select x).ToList();
            var fdpIds = fdps.Select(q => q.Id).Distinct().ToList();
            var viewFdpItems = (from x in context.ViewFDPItems.AsNoTracking()
                                where fdpIds.Contains(x.FDPId)
                                select x).ToList();


            foreach (var f in fdps)
            {
                var fdpId = f.Id;
                var viewItems = viewFdpItems.Where(q => q.FDPId == fdpId).ToList();
                var firstItem = viewFdpItems.Where(q => (q.IsOff == null || q.IsOff == false) && q.FlightId != flightId).OrderBy(q => q.STD).FirstOrDefault();
                var lastItem = viewFdpItems.Where(q => (q.IsOff == null || q.IsOff == false) && q.FlightId != flightId).OrderByDescending(q => q.STD).FirstOrDefault();
                if (firstItem != null)
                    f.FirstFlightId = firstItem.FlightId;
                if (lastItem != null)
                    f.LastFlightId = lastItem.FlightId;
                if (f.UPD == null)
                    f.UPD = 1;
                else
                    f.UPD++;
            }


            return string.Empty;


        }

        public CustomActionResult UpdateDelays(ViewModels.FlightSaveDto dto, ppa_entities context)
        {
            // if (!string.IsNullOrEmpty(dto.UserName) && dto.UserName.ToLower().StartsWith("aps."))
            //     return new CustomActionResult(HttpStatusCode.OK, "");
            if (dto.UpdateDelays != 1)
                return new CustomActionResult(HttpStatusCode.OK, "");
            var currentDelays = context.FlightDelays.Where(q => q.FlightId == dto.ID);
            context.FlightDelays.RemoveRange(currentDelays);
            foreach (var x in dto.Delays)
            {
                context.FlightDelays.Add(new FlightDelay()
                {
                    DelayCodeId = x.DelayCodeId,
                    FlightId = dto.ID,
                    MM = x.MM,
                    HH = x.HH,
                    Remark = x.Remark,
                    UserId = x.UserId
                });
            }

            return new CustomActionResult(HttpStatusCode.OK, "");
        }


        internal async Task<CustomActionResult> RemoveItemsFromFDP(string strItems, int crewId, int reason, string remark, int notify, int noflight, string username = "")
        {
            ppa_entities context = new ppa_entities();
            //var _fdpItemIds = strItems.Split('*').Select(q => Convert.ToInt32(q)).Distinct().ToList();
            //var _fdpIds = strfdps.Split('*').Select(q => Convert.ToInt32(q)).Distinct().ToList();

            var flightIds = strItems.Split('*').Select(q => (Nullable<int>)Convert.ToInt32(q)).Distinct().ToList();
            // var X_fdpItemIds = await context.ViewFDPItem2.Where(q => q.CrewId == crewId && flightIds.Contains(q.FlightId)).OrderBy(q => q.STD).Select(q => q.Id).ToListAsync();

            var _fdpItemIds = await (from x in context.FDPs
                                     join y in context.FDPItems on x.Id equals y.FDPId
                                     where flightIds.Contains(y.FlightId) && x.CrewId == crewId
                                     select y.Id).ToListAsync();
            var allRemovedItems = await context.FDPItems.Where(q => _fdpItemIds.Contains(q.Id)).ToListAsync();
            var _fdpIds = allRemovedItems.Select(q => q.FDPId).ToList();
            var fdps = await context.FDPs.Where(q => _fdpIds.Contains(q.Id)).ToListAsync();
            var fdpItems = await context.FDPItems.Where(q => _fdpIds.Contains(q.FDPId)).ToListAsync();


            var allFlightIds = fdpItems.Select(q => q.FlightId).ToList();
            var allFlights = await context.ViewLegTimes.Where(q => allFlightIds.Contains(q.ID)).OrderBy(q => q.STD).ToListAsync();
            var crews = await context.ViewEmployeeLights.Where(q => q.Id == crewId).ToListAsync();
            var allRemovedFlights = allFlights.Where(q => flightIds.Contains(q.ID)).OrderBy(q => q.STD).ToList();
            FDP offFDP = null;
            string offSMS = string.Empty;
            List<string> sms = new List<string>();
            List<string> nos = new List<string>();
            List<CrewPickupSM> csms = new List<CrewPickupSM>();
            if (reason != -1)
            {


                offFDP = new FDP()
                {
                    CrewId = crewId,
                    DateStart = allRemovedFlights.First().STD,
                    DateEnd = allRemovedFlights.Last().STA,
                    InitStart = allRemovedFlights.First().STD,
                    InitEnd = allRemovedFlights.Last().STA,

                    InitRestTo = allRemovedFlights.Last().STA,
                    InitKey = allRemovedFlights.First().ID.ToString(),
                    DutyType = 0,
                    GUID = Guid.NewGuid(),
                    IsTemplate = false,
                    Remark = remark,
                    UPD = 1,
                    UserName = username


                };
                offFDP.CanceledNo = string.Join(",", allRemovedFlights.Select(q => q.FlightNumber));
                offFDP.CanceledRoute = string.Join(",", allRemovedFlights.Select(q => q.FromAirportIATA)) + "," + allRemovedFlights.Last().ToAirportIATA;
                switch (reason)
                {
                    case 1:
                        offFDP.DutyType = 100009;
                        offFDP.Remark2 = "Refused by Crew";
                        break;
                    case 5:
                        offFDP.DutyType = 100020;
                        offFDP.Remark2 = "Cenceled due to Rescheduling";
                        break;
                    case 2:
                        offFDP.DutyType = 100021;
                        offFDP.Remark2 = "Cenceled due to Flight(s) Cancellation";
                        break;
                    case 3:
                        offFDP.DutyType = 100022;
                        offFDP.Remark2 = "Cenceled due to Change of A/C Type";
                        break;
                    case 4:
                        offFDP.DutyType = 100023;
                        offFDP.Remark2 = "Cenceled due to Flight/Duty Limitations";
                        break;
                    case 6:
                        offFDP.DutyType = 100024;
                        offFDP.Remark2 = "Cenceled due to Not using Split Duty";
                        break;


                    case 7:
                        offFDP.DutyType = 200000;
                        offFDP.Remark2 = "Refused-Not Home";
                        break;
                    case 8:
                        offFDP.DutyType = 200001;
                        offFDP.Remark2 = "Refused-Family Problem";
                        break;
                    case 9:
                        offFDP.DutyType = 200002;
                        offFDP.Remark2 = "Canceled - Training";
                        break;
                    case 10:
                        offFDP.DutyType = 200003;
                        offFDP.Remark2 = "Ground - Operation";
                        break;
                    case 11:
                        offFDP.DutyType = 200004;
                        offFDP.Remark2 = "Ground - Expired License";
                        break;
                    case 12:
                        offFDP.DutyType = 200005;
                        offFDP.Remark2 = "Ground - Medical";
                        break;
                    default:
                        break;
                }
                foreach (var x in allRemovedFlights)
                {
                    var _ofdpitem = fdpItems.FirstOrDefault(q => q.FlightId == x.ID);
                    string _oremark = string.Empty;
                    if (_ofdpitem != null)
                    {
                        var _ofdp = fdps.Where(q => q.Id == _ofdpitem.FDPId).FirstOrDefault();
                        if (_ofdp != null)
                            _oremark = _ofdp.InitRank;
                    }
                    offFDP.OffItems.Add(new OffItem() { FDP = offFDP, FlightId = x.ID, Remark = _oremark });
                }

                context.FDPs.Add(offFDP);



                var strs = new List<string>();
                strs.Add(ConfigurationManager.AppSettings["airline"] + " Airlines");
                strs.Add("Dear " + crews.FirstOrDefault(q => q.Id == crewId).Name + ", ");
                strs.Add("Canceling Notification");
                var day = ((DateTime)allRemovedFlights.First().STDLocal).Date;
                var dayStr = day.ToString("ddd") + " " + day.Year + "-" + day.Month + "-" + day.Day;
                strs.Add(dayStr);
                strs.Add(offFDP.CanceledNo);
                strs.Add(offFDP.CanceledRoute);
                strs.Add(offFDP.Remark2);
                strs.Add(remark);
                strs.Add("Date sent: " + DateTime.Now.ToLocalTime().ToString("yyyy/MM/dd HH:mm"));
                strs.Add("Crew Scheduling Department");
                offSMS = String.Join("\n", strs);
                sms.Add(offSMS);
                nos.Add(crews.FirstOrDefault(q => q.Id == crewId).Mobile);

                var csm = new CrewPickupSM()
                {
                    CrewId = (int)crewId,
                    DateSent = DateTime.Now,
                    DateStatus = DateTime.Now,
                    FlightId = -1,
                    Message = offSMS,
                    Pickup = null,
                    RefId = "",
                    Status = "",
                    Type = offFDP.DutyType,
                    FDP = offFDP,
                    DutyType = offFDP.Remark2,
                    DutyDate = ((DateTime)offFDP.InitStart).ToLocalTime().Date,
                    Flts = offFDP.CanceledNo,
                    Routes = offFDP.CanceledRoute
                };
                csms.Add(csm);
                if (notify == 1)
                    context.CrewPickupSMS.Add(csm);
            }



            foreach (var x in allRemovedItems)
            {
                var xfdp = fdps.FirstOrDefault(q => q.Id == x.FDPId);
                var xcrew = crews.FirstOrDefault(q => q.Id == xfdp.CrewId);
                var xleg = allFlights.FirstOrDefault(q => q.ID == x.FlightId);


            }

            var updatedIds = new List<int>();
            var updated = new List<FDP>();
            var removed = new List<int>();


            //  List<FDP> deleted = new List<FDP>();
            foreach (var fdp in fdps)
            {
                fdp.Split = 0;
                var allitems = fdpItems.Where(q => q.FDPId == fdp.Id).ToList();
                var removedItems = allitems.Where(q => _fdpItemIds.Contains(q.Id)).ToList();
                var remainItems = allitems.Where(q => !_fdpItemIds.Contains(q.Id)).ToList();
                var remainFlightIds = remainItems.Select(q => q.FlightId).ToList();
                if (allitems.Count == removedItems.Count)
                {
                    removed.Add(fdp.Id);
                    context.FDPItems.RemoveRange(removedItems);

                    context.FDPs.Remove(fdp);
                }
                else
                {
                    //Update FDP

                    context.FDPItems.RemoveRange(removedItems);
                    var items = (from x in remainItems
                                 join y in allFlights on x.FlightId equals y.ID
                                 orderby y.STD
                                 select new { fi = x, flt = y }).ToList();
                    fdp.FirstFlightId = items.First().flt.ID;
                    fdp.LastFlightId = items.Last().flt.ID;
                    fdp.InitStart = ((DateTime)items.First().flt.STD).AddMinutes(-60);
                    fdp.InitEnd = ((DateTime)items.Last().flt.STA).AddMinutes(30);

                    fdp.DateStart = ((DateTime)items.First().flt.STD).AddMinutes(-60);
                    fdp.DateEnd = ((DateTime)items.Last().flt.STA).AddMinutes(30);

                    var rst = 12;
                    if (fdp.InitHomeBase != null && fdp.InitHomeBase != items.Last().flt.ToAirport)
                        rst = 10;
                    fdp.InitRestTo = ((DateTime)items.Last().flt.STA).AddMinutes(30).AddHours(rst);
                    fdp.InitFlts = string.Join(",", items.Select(q => q.flt).Select(q => q.FlightNumber).ToList());
                    fdp.InitRoute = string.Join(",", items.Select(q => q.flt).Select(q => q.FromAirportIATA).ToList());
                    fdp.InitRoute += "," + items.Last().flt.ToAirportIATA;
                    fdp.InitFromIATA = items.First().flt.FromAirport.ToString();
                    fdp.InitToIATA = items.Last().flt.ToAirport.ToString();
                    fdp.InitNo = string.Join("-", items.Select(q => q.flt).Select(q => q.FlightNumber).ToList());
                    fdp.InitKey = string.Join("-", items.Select(q => q.flt).Select(q => q.ID).ToList());
                    fdp.InitFlights = string.Join("*", items.Select(q => q.flt.ID + "_" + (q.fi.IsPositioning == true ? "1" : "0") + "_" + ((DateTime)q.flt.STDLocal).ToString("yyyyMMddHHmm")
                      + "_" + ((DateTime)q.flt.STALocal).ToString("yyyyMMddHHmm")
                      + "_" + q.flt.FlightNumber + "_" + q.flt.FromAirportIATA + "_" + q.flt.ToAirportIATA).ToList()
                    );

                    var keyParts = new List<string>();
                    keyParts.Add(items[0].flt.ID + "*" + (items[0].fi.IsPositioning == true ? "1" : "0"));
                    var breakGreaterThan10Hours = string.Empty;
                    for (int i = 1; i < items.Count; i++)
                    {

                        keyParts.Add(items[i].flt.ID + "*" + (items[i].fi.IsPositioning == true ? "1" : "0"));
                        var dt = (DateTime)items[i].flt.STD - (DateTime)items[i - 1].flt.STA;
                        var minuts = dt.TotalMinutes;
                        // – (0:30 + 0:15 + 0:45)
                        var brk = minuts - 30 - 60; //30:travel time, post flight duty:15, pre flight duty:30
                        if (brk >= 600)
                        {
                            //var tfi = tflights.FirstOrDefault(q => q.ID == flights[i].ID);
                            // var tfi1 = tflights.FirstOrDefault(q => q.ID == flights[i - 1].ID);
                            breakGreaterThan10Hours = "The break is greater than 10 hours.";
                        }
                        else
                        if (brk >= 180)
                        {
                            var xfdpitem = allitems.FirstOrDefault(q => q.Id == items[i].fi.Id);
                            xfdpitem.SplitDuty = true;
                            var pair = allitems.FirstOrDefault(q => q.Id == items[i - 1].fi.Id);
                            pair.SplitDuty = true;
                            xfdpitem.SplitDutyPairId = pair.FlightId;
                            fdp.Split += 0.5 * (brk);

                        }

                    }
                    fdp.UPD = fdp.UPD == null ? 1 : ((int)fdp.UPD) + 1;
                    fdp.Key = string.Join("_", keyParts);
                    fdp.UserName = username;
                    //var flights = allFlights.Where(q => remainFlightIds.Contains(q.ID)).OrderBy(q=>q.STD).ToList();
                    updatedIds.Add(fdp.Id);
                    updated.Add(fdp);

                }
            }





            var saveResult = await context.SaveAsync();
            if (saveResult.Code != HttpStatusCode.OK)
                return saveResult;

            var fdpsIds = fdps.Select(q => q.Id).ToList();
            var maxfdps = await context.HelperMaxFDPs.Where(q => fdpsIds.Contains(q.Id)).ToListAsync();
            var fdpExtras = await context.FDPExtras.Where(q => fdpsIds.Contains(q.FDPId)).ToListAsync();
            context.FDPExtras.RemoveRange(fdpExtras);
            foreach (var x in maxfdps)
            {
                context.FDPExtras.Add(new FDPExtra()
                {
                    FDPId = x.Id,
                    MaxFDP = Convert.ToDecimal(x.MaxFDPExtended),
                });
            }
            saveResult = await context.SaveAsync();
            if (saveResult.Code != HttpStatusCode.OK)
                return saveResult;
            //if (notify == 1)
            //{
            //    Magfa m = new Magfa();
            //    int c = 0;
            //    foreach (var x in sms)
            //    {
            //        var txt = sms[c];
            //        var no = nos[c];

            //        var smsResult = m.enqueue(1, no, txt)[0];
            //        c++;

            //    }
            //}

            //updated = await context.ViewFDPKeys.Where(q => updatedIds.Contains(q.Id)).ToListAsync();

            var result = new
            {
                removed,
                updatedId = updated.Select(q => q.Id).ToList(),
                //updated = getRosterFDPDtos(updated)
            };

            return new CustomActionResult(HttpStatusCode.OK, result);
        }


        public string getOrderIndex(string reg, List<string> grounds)
        {
            var str = "";
            //orderby grp.Key.Register.Contains("CNL") ? "ZZZZ" :( grp.Key.Register[grp.Key.Register.Length - 1].ToString())
            if (reg.Contains("CNL"))
                str = "ZZZZZZ";
            else if (reg.Contains("RBC"))
                str = "ZZZZZY";
            else
           //str = 1000000;
           // if (grounds.Contains(reg))
           if (reg.Contains("."))
            {
                str = "ZZZZ" + reg[reg.Length - 2];
                //str = 100000;
            }
            // str= reg[reg.Length - 1].ToString();
            else
                str = reg[reg.Length - 1].ToString();

            return str;

        }



        [Route("api/flights/gantt/utc/customer/old/{cid}/{from}/{to}/{tzoffset}")]
        [AcceptVerbs("POST", "GET")]
        public async Task<IHttpActionResult> GetFlightsGanttByCustomerIdUTC(int cid, string from, string to, int tzoffset)
        {
            try
            {
                DateTime dateFrom = Helper.BuildDateTimeFromYAFormat(from);
                DateTime dateTo = Helper.BuildDateTimeFromYAFormat(to);

                // var result = await unitOfWork.FlightRepository.GetFlightGanttFleet(cid, dateFrom, dateTo, tzoffset, null, null, 1);
                //return Ok(result);
                var context = new ppa_entities();

                //var flightsQuery = context.ViewFlightsGantts.Where(q => /*q.CustomerId == cid &&*/ q.RegisterID != null &&
                //(
                //(q.STDLocal >= dateFrom && q.STDLocal <= dateTo) || (q.DepartureLocal >= dateFrom && q.DepartureLocal <= dateTo)
                //|| (q.STALocal >= dateFrom && q.STALocal <= dateTo) || (q.ArrivalLocal >= dateFrom && q.ArrivalLocal <= dateTo)
                //)
                //);


                var flightsQuery = context.ViewFlightsGantts.Where(q => /*q.CustomerId == cid &&*/ q.RegisterID != null &&
               (
               (q.STD >= dateFrom && q.STD <= dateTo) || (q.Departure >= dateFrom && q.Departure <= dateTo)
               || (q.STA >= dateFrom && q.STA <= dateTo) || (q.Arrival >= dateFrom && q.Arrival <= dateTo)
               )
               );


                int utc = 1;
                int? doUtc = utc;
                if (cid != -1)
                    flightsQuery = flightsQuery.Where(q => q.CustomerId == cid);



                var flights = await flightsQuery.ToListAsync();



                var grounds = (from x in context.ViewRegisterGrounds
                               where x.CustomerId == cid &&
                               (
                                (dateFrom >= x.DateFrom && dateTo <= x.DateEnd) ||
                                (x.DateFrom >= dateFrom && x.DateEnd <= dateTo) ||

                                (x.DateFrom >= dateFrom && x.DateFrom <= dateTo) ||
                                (x.DateEnd >= dateFrom && x.DateEnd <= dateTo)
                               )
                               select x).ToList();

                flights = flights.OrderBy(q => q.STD).ToList();


                var groundRegs = new List<string>();

                var flightsdto = new List<ViewModels.ViewFlightsGanttDto>();
                foreach (var x in flights)
                {
                    ViewModels.ViewFlightsGanttDto dto = new ViewFlightsGanttDto();
                    ViewModels.ViewFlightsGanttDto.FillDto(x, dto, tzoffset, doUtc);
                    flightsdto.Add(dto);
                }

                var resgroups = from x in flights
                                group x by new { x.AircraftType, AircraftTypeId = x.TypeId }
                                into grp
                                select new { groupId = grp.Key.AircraftTypeId, Title = grp.Key.AircraftType };


                //change other method
                var ressq = (from x in flights
                             group x by new { x.RegisterID, x.Register, x.TypeId }
                         into grp
                             //orderby grp.Key.TypeId, grp.Key.Register
                             // orderby grp.Key.Register.Contains("CNL")?"ZZZZ": grp.Key.Register[grp.Key.Register.Length-1].ToString()
                             orderby getOrderIndex(grp.Key.Register, groundRegs)
                             select new { resourceId = grp.Key.RegisterID, resourceName = grp.Key.Register, groupId = grp.Key.TypeId }).ToList();
                //var ress = ressq.OrderBy(q => q.TypeId).Select((q, i) => new { resourceName = q.Register, groupId = q.TypeId, resourceId = (q.RegisterID >= 0 ? q.RegisterID : -1 * (i + 1)) }).ToList();

                foreach (var x in flightsdto)
                {

                    x.resourceId.Add((int)x.RegisterID);

                }


                var fromAirport = (from x in flights
                                   group x by new { x.FromAirport, x.FromAirportIATA, x.FromAirportName } into g
                                   select new BaseSummary()
                                   {
                                       BaseId = g.Key.FromAirport,
                                       BaseIATA = g.Key.FromAirportIATA,
                                       BaseName = g.Key.FromAirportName,
                                       Total = g.Count(),
                                       TakeOff = g.Where(q => q.Takeoff != null).Count(),
                                       Landing = 0, //g.Where(q => q.Landing != null).Count(),
                                       Canceled = g.Where(q => q.FlightStatusID == 4).Count(),
                                       Redirected = g.Where(q => q.FlightStatusID == 17).Count(),
                                       Diverted = g.Where(q => q.FlightStatusID == 7).Count(),
                                       TotalDelays = g.Where(q => q.ChocksOut != null).Sum(q => q.DelayOffBlock),
                                       DepartedPax = g.Where(q => q.Takeoff != null).Sum(q => q.TotalPax),
                                       ArrivedPax = 0,// g.Where(q => q.Landing != null).Sum(q => q.TotalPax),

                                   }).ToList();
                var toAirport = (from x in flights
                                 group x by new { x.ToAirport, x.ToAirportIATA, x.ToAirportName } into g
                                 select new BaseSummary()
                                 {
                                     BaseId = g.Key.ToAirport,
                                     BaseIATA = g.Key.ToAirportIATA,
                                     BaseName = g.Key.ToAirportName,
                                     Total = g.Count(),
                                     TakeOff = 0,//g.Where(q => q.Takeoff != null).Count(),
                                     Landing = g.Where(q => q.Landing != null).Count(),
                                     Canceled = 0,//g.Where(q => q.FlightStatusID == 4).Count(),
                                     Redirected = 0,// g.Where(q => q.FlightStatusID == 17).Count(),
                                     Diverted = 0,// g.Where(q => q.FlightStatusID == 7).Count(),
                                     TotalDelays = 0,// g.Where(q => q.ChocksOut != null).Sum(q => q.DelayOffBlock),
                                     DepartedPax = 0,// g.Where(q => q.Takeoff != null).Sum(q => q.TotalPax),
                                     ArrivedPax = g.Where(q => q.Landing != null).Sum(q => q.TotalPax),

                                 }).ToList();

                var baseSum = new List<BaseSummary>();
                foreach (var x in fromAirport)
                {
                    var _to = toAirport.FirstOrDefault(q => q.BaseId == x.BaseId);
                    if (_to != null)
                    {
                        x.ArrivedPax += _to.ArrivedPax;
                        x.Canceled += _to.Canceled;
                        x.DepartedPax += _to.DepartedPax;
                        x.Diverted += _to.Diverted;
                        x.Landing += _to.Landing;
                        x.Redirected += _to.Redirected;
                        x.TakeOff += _to.TakeOff;
                        x.Total += _to.Total;
                        x.TotalDelays += _to.TotalDelays;

                    }

                    baseSum.Add(x);
                }




                var result = new
                {
                    flights = flightsdto,
                    resourceGroups = resgroups.ToList(),
                    resources = ressq,
                    baseSummary = baseSum,
                    grounds,
                    // fltgroups,
                    baseDate = DateTime.UtcNow,
                };
                return Ok(result);


                ///////////////////////
                ///////////////////////
                //////////////////////
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   INNER:  " + ex.InnerException.Message;
                return Ok(msg);
            }

        }



        [Route("api/flights/gantt/utc/customer/{cid}/{from}/{to}/{tzoffset}")]
        [AcceptVerbs("POST", "GET")]
        public async Task<IHttpActionResult> GetFlightsGanttByCustomerIdUTCNew(int cid, string from, string to, int tzoffset)
        {
            try
            {
                DateTime dateFrom = Helper.BuildDateTimeFromYAFormat(from);
                DateTime dateTo = Helper.BuildDateTimeFromYAFormat(to);

                // var result = await unitOfWork.FlightRepository.GetFlightGanttFleet(cid, dateFrom, dateTo, tzoffset, null, null, 1);
                //return Ok(result);
                var context = new ppa_entities();

                //var flightsQuery = context.ViewFlightsGantts.Where(q => /*q.CustomerId == cid &&*/ q.RegisterID != null &&
                //(
                //(q.STDLocal >= dateFrom && q.STDLocal <= dateTo) || (q.DepartureLocal >= dateFrom && q.DepartureLocal <= dateTo)
                //|| (q.STALocal >= dateFrom && q.STALocal <= dateTo) || (q.ArrivalLocal >= dateFrom && q.ArrivalLocal <= dateTo)
                //)
                //);


                // var flightsQuery = context.ViewFlightsGanttNews.Where(q => /*q.CustomerId == cid &&*/ q.RegisterID != null &&
                //(
                //(q.STD >= dateFrom && q.STD <= dateTo) || (q.Departure >= dateFrom && q.Departure <= dateTo)
                //|| (q.STA >= dateFrom && q.STA <= dateTo) || (q.Arrival >= dateFrom && q.Arrival <= dateTo)
                //)
                //);

                //    var raw_query = await context.Database.SqlQuery<ViewFlightsGanttNew>("select top 1000 * from ViewFlightsGanttNew ").ToListAsync();
                var flightsQuery = context.ViewFlightsGanttNews.Where(q =>
               (
                  (q.STD >= dateFrom && q.STD <= dateTo)
                   || (q.STA >= dateFrom && q.STA <= dateTo)
               )
               ).Select(q => new
               {
                   q.ID,
                   q.FlightPlanId,
                   q.BaggageCount,
                   q.CargoUnitID,
                   q.CargoUnit,
                   q.CargoWeight,
                   q.PaxChild,
                   q.PaxInfant,
                   q.PaxAdult,
                   q.FuelArrival,
                   q.FuelActual,
                   q.FuelPlanned,
                   q.GWLand,
                   q.GWTO,
                   q.FlightH,
                   q.FlightM,
                   q.ChocksIn,
                   q.Landing,
                   q.Takeoff,
                   q.ChocksOut,
                   q.STD,
                   q.STA,
                   q.STDLocal,
                   q.STALocal,
                   q.Date,
                   q.FlightStatusID,
                   q.RegisterID,
                   q.FlightTypeID,
                   //q.FlightType,
                   q.TypeId,
                   q.OTypeId,
                   q.FlightNumber,
                   q.FromAirport,
                   q.ToAirport,
                   q.FromAirportIATA,
                   q.ToAirportIATA,
                   q.AircraftType,
                   q.OAircraftType,
                   q.Register,
                   // q.MSN,
                   q.FlightStatus,
                   q.status,
                   q.notes,
                   q.taskName,
                   q.startDate,
                   q.PlanId,
                   q.CargoCount,
                   q.BaggageWeight,
                   q.FuelUnitID,
                   q.FuelUnit,
                   q.ArrivalRemark,
                   q.DepartureRemark,
                   q.TotalSeat,
                   // q.EstimatedDelay,
                   q.TotalPax,
                   q.PaxOver,
                   //q.DateStatus,
                   //q.FlightStatusUserId,
                   q.STDDay,
                   q.STADay,
                   q.DelayOffBlock,
                   // q.DelayTakeoff,
                   // q.DelayOnBlock,
                   // q.DelayLanding,
                   q.CancelReasonId,
                   //q.CancelRemark,
                   // q.CancelDate,
                   //q.CancelReason,
                   q.RedirectReasonId,
                   // q.RedirectRemark,
                   // q.RedirectDate,
                   // q.RedirectReason,
                   q.RampReasonId,
                   //q.RampRemark,
                   //q.RampDate,
                   //q.RampReason,
                   q.OSTA,
                   q.OToAirportId,
                   q.OToAirportIATA,
                   q.FPFuel,
                   q.Defuel,
                   q.Departure,
                   q.Arrival,
                   q.DepartureLocal,
                   q.ArrivalLocal,
                   q.BlockTime,
                   q.FlightTime,
                   q.FlightTimeActual,
                   q.UsedFuel,
                   // q.JLBLHH,
                   // q.JLBLMM,
                   // q.PFLR,
                   q.ChrAdult,
                   q.ChrChild,
                   q.ChrInfant,
                   q.ChrCapacity,
                   q.ChrTitle,
                   q.ChrCode,
                   q.DefaultChrId,
                   q.CargoCost,
                   q.FuelDensity,
                   q.SerialNo,
                   q.LTR,
                   q.FuelDeparture,
                   // q.Ready,
                   // q.Start,
                   q.CargoPieces,
                   q.CPDH
               })
                    .AsNoTracking();


                int utc = 1;
                int? doUtc = utc;
                // if (cid != -1)
                //    flightsQuery = flightsQuery.Where(q => q.CustomerId == cid);



                var flights = cid==100? await flightsQuery.ToListAsync() : await flightsQuery.Where(q=>q.CPDH==0).ToListAsync() ;






                //var grounds = (from x in context.ViewRegisterGrounds
                //               where x.CustomerId == cid &&
                //               (
                //                (dateFrom >= x.DateFrom && dateTo <= x.DateEnd) ||
                //                (x.DateFrom >= dateFrom && x.DateEnd <= dateTo) ||

                //                (x.DateFrom >= dateFrom && x.DateFrom <= dateTo) ||
                //                (x.DateEnd >= dateFrom && x.DateEnd <= dateTo)
                //               )
                //               select x).ToList();
                var grounds = new List<ViewRegisterGround>();

                flights = flights.OrderBy(q => q.STD).ToList();


                var groundRegs = new List<string>();

                //var flightsdto = new List<ViewModels.ViewFlightsGanttDto>();
                //foreach (var x in flights)
                //{
                //    ViewModels.ViewFlightsGanttDto dto = new ViewFlightsGanttDto();
                //    ViewModels.ViewFlightsGanttDto.FillDto(x, dto, tzoffset, doUtc);
                //    flightsdto.Add(dto);
                //}
                var flightsdto = flights;

                var resgroups = from x in flights
                                group x by new { x.AircraftType, AircraftTypeId = x.TypeId }
                                into grp
                                select new { groupId = grp.Key.AircraftTypeId, Title = grp.Key.AircraftType };


                //change other method
                var ressq = (from x in flights
                             group x by new { x.RegisterID, x.Register, x.TypeId }
                         into grp
                             //orderby grp.Key.TypeId, grp.Key.Register
                             // orderby grp.Key.Register.Contains("CNL")?"ZZZZ": grp.Key.Register[grp.Key.Register.Length-1].ToString()
                             orderby getOrderIndex(grp.Key.Register, groundRegs)
                             select new { resourceId = grp.Key.RegisterID, resourceName = grp.Key.Register, groupId = grp.Key.TypeId }).ToList();
                //var ress = ressq.OrderBy(q => q.TypeId).Select((q, i) => new { resourceName = q.Register, groupId = q.TypeId, resourceId = (q.RegisterID >= 0 ? q.RegisterID : -1 * (i + 1)) }).ToList();

                //foreach (var x in flightsdto)
                //{

                //    x.resourceId.Add((int)x.RegisterID);

                //}


                var fromAirport = (from x in flights
                                   group x by new { x.FromAirport, x.FromAirportIATA } into g
                                   select new BaseSummary()
                                   {
                                       BaseId = g.Key.FromAirport,
                                       BaseIATA = g.Key.FromAirportIATA,
                                       BaseName = g.Key.FromAirportIATA,
                                       Total = g.Count(),
                                       TakeOff = g.Where(q => q.Takeoff != null).Count(),
                                       Landing = 0, //g.Where(q => q.Landing != null).Count(),
                                       Canceled = g.Where(q => q.FlightStatusID == 4).Count(),
                                       Redirected = g.Where(q => q.FlightStatusID == 17).Count(),
                                       Diverted = g.Where(q => q.FlightStatusID == 7).Count(),
                                       TotalDelays = g.Where(q => q.ChocksOut != null).Sum(q => q.DelayOffBlock),
                                       DepartedPax = g.Where(q => q.Takeoff != null).Sum(q => q.TotalPax),
                                       ArrivedPax = 0,// g.Where(q => q.Landing != null).Sum(q => q.TotalPax),

                                   }).ToList();
                var toAirport = (from x in flights
                                 group x by new { x.ToAirport, x.ToAirportIATA } into g
                                 select new BaseSummary()
                                 {
                                     BaseId = g.Key.ToAirport,
                                     BaseIATA = g.Key.ToAirportIATA,
                                     BaseName = g.Key.ToAirportIATA,
                                     Total = g.Count(),
                                     TakeOff = 0,//g.Where(q => q.Takeoff != null).Count(),
                                     Landing = g.Where(q => q.Landing != null).Count(),
                                     Canceled = 0,//g.Where(q => q.FlightStatusID == 4).Count(),
                                     Redirected = 0,// g.Where(q => q.FlightStatusID == 17).Count(),
                                     Diverted = 0,// g.Where(q => q.FlightStatusID == 7).Count(),
                                     TotalDelays = 0,// g.Where(q => q.ChocksOut != null).Sum(q => q.DelayOffBlock),
                                     DepartedPax = 0,// g.Where(q => q.Takeoff != null).Sum(q => q.TotalPax),
                                     ArrivedPax = g.Where(q => q.Landing != null).Sum(q => q.TotalPax),

                                 }).ToList();

                var baseSum = new List<BaseSummary>();
                foreach (var x in fromAirport)
                {
                    var _to = toAirport.FirstOrDefault(q => q.BaseId == x.BaseId);
                    if (_to != null)
                    {
                        x.ArrivedPax += _to.ArrivedPax;
                        x.Canceled += _to.Canceled;
                        x.DepartedPax += _to.DepartedPax;
                        x.Diverted += _to.Diverted;
                        x.Landing += _to.Landing;
                        x.Redirected += _to.Redirected;
                        x.TakeOff += _to.TakeOff;
                        x.Total += _to.Total;
                        x.TotalDelays += _to.TotalDelays;

                    }

                    baseSum.Add(x);
                }




                var result = new
                {
                    flights = flightsdto,
                    resourceGroups = resgroups.ToList(),
                    resources = ressq,
                    baseSummary = baseSum,
                    grounds,
                    // fltgroups,
                    baseDate = DateTime.UtcNow,
                };
                return Ok(result);


                ///////////////////////
                ///////////////////////
                //////////////////////
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   INNER:  " + ex.InnerException.Message;
                return Ok(msg);
            }

        }



        [Route("api/plan/flights")]
        public async Task<IHttpActionResult> GetFlightsForPlan(DateTime dfrom, DateTime dto)
        {
            dfrom = dfrom.Date;
            dto = dto.Date.AddDays(1);

            // var result = await unitOfWork.FlightRepository.GetFlightGanttFleet(cid, dateFrom, dateTo, tzoffset, null, null, 1);
            //return Ok(result);
            var context = new ppa_entities();
            var query = from x in context.ViewLegTimes
                        where x.FlightStatusID == 1 && (x.STDLocal >= dfrom && x.STDLocal < dto)
                        orderby x.STDDay, x.Register, x.STD
                        select new FlightPlanDto()
                        {
                            ID = x.ID,
                            Register = x.Register,
                            RegisterID = x.RegisterID,
                            FromAirport = x.FromAirport,
                            FromAirportIATA = x.FromAirportIATA,
                            ToAirport = x.ToAirport,
                            ToAirportIATA = x.ToAirportIATA,
                            STD = x.STD,
                            STDLocal = x.STDLocal,
                            STA = x.STA,
                            STALocal = x.STALocal,
                            STDDay = x.STDDay,
                            FlightNumber = x.FlightNumber
                        };
            var result = await query.ToListAsync();
            foreach (var x in result)
            {


            }
            return Ok(result);
        }


        public DateTime getDate(string str)
        {
            var y = Convert.ToInt32(str.Substring(0, 4));
            var m = Convert.ToInt32(str.Substring(4, 2));
            var d = Convert.ToInt32(str.Substring(6, 2));
            return new DateTime(y, m, d);
        }

        public DateTime getDateTime(string str)
        {
            var y = Convert.ToInt32(str.Substring(0, 4));
            var m = Convert.ToInt32(str.Substring(4, 2));
            var d = Convert.ToInt32(str.Substring(6, 2));
            var h = Convert.ToInt32(str.Substring(8, 2));
            var mm = Convert.ToInt32(str.Substring(10, 2));
            return new DateTime(y, m, d, h, mm, 0);
        }


        [AcceptVerbs("POST")]
        [Route("api/plan/flights/save")]
        public async Task<IHttpActionResult> PostFlightPlan(FlightPlanSaveDto dto)
        {
            var context = new ppa_entities();
            var updatedIds = dto.updated.Select(q => q.ID).ToList();
            var flights = await context.FlightInformations.Where(q => updatedIds.Contains(q.ID)).ToListAsync();
            var edited = dto.updated.Where(q => q.ID > 0).ToList();

            var news = dto.updated.Where(q => q.ID < 0).ToList();
            foreach (var rec in news)
            {
                rec.STDDay = getDate(rec.STDDay2);
                rec.STDLocal = getDateTime(rec.STDLocal2);
                rec.STALocal = getDateTime(rec.STALocal2);
                var flight = new FlightInformation();
                flight.RegisterID = rec.RegisterID;
                flight.FromAirportId = rec.FromAirport;
                flight.ToAirportId = rec.ToAirport;
                flight.FlightNumber = rec.FlightNumber;
                flight.FlightTypeID = 109;
                flight.FlightStatusID = 1;
                flight.CustomerId = 4;

                var stdday = ((DateTime)rec.STDDay).Date;
                var stdStr = ((DateTime)rec.STDLocal).ToString("HHmm");
                var stdInt = Convert.ToInt32(stdStr);
                var staStr = ((DateTime)rec.STALocal).ToString("HHmm");
                var staInt = Convert.ToInt32(staStr);

                var stdLocal = new DateTime(stdday.Year, stdday.Month, stdday.Day, Convert.ToInt32(stdStr.Substring(0, 2)), Convert.ToInt32(stdStr.Substring(2, 2)), 0);

                var staday = ((DateTime)rec.STDDay).Date;
                if (stdInt > staInt)
                    staday = staday.AddDays(1);
                var staLocal = new DateTime(staday.Year, staday.Month, staday.Day, Convert.ToInt32(staStr.Substring(0, 2)), Convert.ToInt32(staStr.Substring(2, 2)), 0);

                var stdOffset = -1 * TimeZoneInfo.Local.GetUtcOffset(stdLocal).TotalMinutes;
                var staOffset = -1 * TimeZoneInfo.Local.GetUtcOffset(staLocal).TotalMinutes;

                var std = stdLocal.AddMinutes(stdOffset);
                var sta = staLocal.AddMinutes(staOffset);

                if (sta > std && (sta - std).TotalHours < 12)
                {
                    flight.STD = std;
                    flight.ChocksOut = std;
                    flight.Takeoff = std;
                    flight.Landing = sta;
                    flight.ChocksIn = sta;
                    flight.STA = sta;

                    flight.FlightH = Convert.ToInt32(sta.Subtract((DateTime)std).Hours);
                    flight.FlightM = Convert.ToByte(sta.Subtract((DateTime)std).TotalHours * 60 - flight.FlightH * 60);

                    context.FlightInformations.Add(flight);
                }

            }

            foreach (var rec in edited)
            {
                rec.STDDay = getDate(rec.STDDay2);
                rec.STDLocal = getDateTime(rec.STDLocal2);
                rec.STALocal = getDateTime(rec.STALocal2);
                var flight = flights.Where(q => q.ID == rec.ID).FirstOrDefault();
                var stdday = ((DateTime)rec.STDDay).Date;
                var stdStr = ((DateTime)rec.STDLocal).ToString("HHmm");
                var stdInt = Convert.ToInt32(stdStr);
                var staStr = ((DateTime)rec.STALocal).ToString("HHmm");
                var staInt = Convert.ToInt32(staStr);

                var stdLocal = new DateTime(stdday.Year, stdday.Month, stdday.Day, Convert.ToInt32(stdStr.Substring(0, 2)), Convert.ToInt32(stdStr.Substring(2, 2)), 0);

                var staday = ((DateTime)rec.STDDay).Date;
                if (stdInt > staInt)
                    staday = staday.AddDays(1);
                var staLocal = new DateTime(staday.Year, staday.Month, staday.Day, Convert.ToInt32(staStr.Substring(0, 2)), Convert.ToInt32(staStr.Substring(2, 2)), 0);

                var stdOffset = -1 * TimeZoneInfo.Local.GetUtcOffset(stdLocal).TotalMinutes;
                var staOffset = -1 * TimeZoneInfo.Local.GetUtcOffset(staLocal).TotalMinutes;

                var std = stdLocal.AddMinutes(stdOffset);
                var sta = staLocal.AddMinutes(staOffset);

                if (sta > std && (sta - std).TotalHours < 12)
                {
                    flight.STD = std;
                    flight.ChocksOut = std;
                    flight.Takeoff = std;
                    flight.Landing = sta;
                    flight.ChocksIn = sta;
                    flight.STA = sta;

                    flight.RegisterID = rec.RegisterID;
                    flight.FromAirportId = rec.FromAirport;
                    flight.ToAirportId = rec.ToAirport;
                    flight.FlightNumber = rec.FlightNumber;


                    flight.FlightH = Convert.ToInt32(sta.Subtract((DateTime)std).Hours);
                    flight.FlightM = Convert.ToByte(sta.Subtract((DateTime)std).TotalHours * 60 - flight.FlightH * 60);

                }


            }

            if (dto.deleted != null && dto.deleted.Count > 0)
            {
                var dflights = context.FlightInformations.Where(q => dto.deleted.Contains(q.ID)).ToList();
                var dflightIds = dflights.Select(q => q.ID).ToList().Select(q => (Nullable<int>)q).ToList();
                var fdpItems = context.FDPItems.Where(q => dflightIds.Contains(q.FlightId)).Select(q => q.FlightId).ToList();
                var finalflts = dflights.Where(q => !fdpItems.Contains(q.ID)).ToList();
                var finalfltsids = finalflts.Select(q => q.ID).ToList();
                var offs = context.OffItems.Where(q => finalfltsids.Contains(q.FlightId)).ToList();
                context.OffItems.RemoveRange(offs);
                var logs = context.FlightStatusLogs.Where(q => finalfltsids.Contains(q.FlightID)).ToList();
                context.FlightStatusLogs.RemoveRange(logs);
                var delays = context.FlightDelays.Where(q => finalfltsids.Contains(q.FlightId)).ToList();
                context.FlightDelays.RemoveRange(delays);
                context.FlightInformations.RemoveRange(finalflts);

            }

            var saveResult = await context.SaveAsync();
            if (saveResult.Code != HttpStatusCode.OK)
                return saveResult;

            return Ok(dto);
        }


        public class pax_dto
        {
            public List<FlightPax> dto { get; set; }
            public int id { get; set; }
        }

        [AcceptVerbs("POST")]
        [Route("api/log/flight/pax/save/")]
        public async Task<IHttpActionResult> PostFlightPaxSave(pax_dto pax_dto)
        {
            var context = new ppa_entities();

            var exist = await context.FlightPaxes.Where(q => q.FlightId == pax_dto.id).ToListAsync();
            context.FlightPaxes.RemoveRange(exist);


            foreach (var x in pax_dto.dto)
            {
                context.FlightPaxes.Add(new FlightPax()
                {
                    // Id = -1,
                    AirportId = x.AirportId,
                    Baggage = x.Baggage,
                    FlightId = x.FlightId,
                    Cargo = x.Cargo,
                    CHR_Adult = x.CHR_Adult,
                    CHR_Child = x.CHR_Child,
                    CHR_Infant = x.CHR_Infant,
                    FOC_Adult = x.FOC_Adult,
                    FOC_Child = x.FOC_Child,
                    FOC_Infant = x.FOC_Infant,
                    OA_Adult = x.OA_Adult,
                    OA_Child = x.OA_Child,
                    OA_Infant = x.OA_Infant,
                    RES_Adult = x.RES_Adult,
                    RES_Child = x.RES_Child,
                    RES_Infant = x.RES_Infant,
                    STN_Adult = x.STN_Adult,
                    STN_Child = x.STN_Child,
                    STN_Infant = x.STN_Infant,
                    ACM = x.ACM,
                    DSP = x.DSP,
                    FM = x.FM,
                    FSG = x.FSG,
                    MOC = x.MOC,
                    WCR = x.WCR,
                    ToAirportId = x.ToAirportId,
                    FlightId2=x.FlightId2,
                });
            }

            var saveResult = await context.SaveAsync();
            if (saveResult.Code != HttpStatusCode.OK)
                return saveResult;

            return Ok(pax_dto);
        }

        [Route("api/log/flight/pax/{id}")]
        public async Task<IHttpActionResult> GetFlightPax(int id)
        {

            var context = new ppa_entities();
            var flight = context.FlightInformations.Where(q => q.ID == id).FirstOrDefault();
            var next_flight = context.FlightInformations.Where(q => q.RegisterID == flight.RegisterID && q.STD > flight.STD).OrderBy(q => q.STD).Take(1).FirstOrDefault();
            if (next_flight != null && ((DateTime)next_flight.STD).Date != ((DateTime)flight.STD).Date)
                next_flight = null;
            if (next_flight != null && next_flight.ToAirportId == flight.FromAirportId)
                next_flight = null;
            var recs = await context.ViewFlightPaxes.Where(q => q.FlightId == id).ToListAsync();
            List<ViewFlightPax> result = new List<ViewFlightPax>();

            

            var main_route = recs.Where(q => q.AirportId == flight.FromAirportId && q.ToAirportId == flight.ToAirportId).FirstOrDefault();
            if (main_route == null)
                result.Insert(0, new ViewFlightPax()
                {
                    FlightId = id,
                    Id = -1,
                    AirportId = flight.FromAirportId,
                    ToAirportId = flight.ToAirportId,
                    Total_CHR = 0,
                    Total_FOC = 0,
                    Total_OA = 0,
                    Total_Pax = 0,
                    Total_RES = 0,
                    Total_Rev = 0,
                    Total_STN = 0,


                });
            else
                result.Add(main_route);

            if (next_flight != null)
            {
                var next_route = recs.Where(q => q.AirportId == flight.FromAirportId && q.ToAirportId == next_flight.ToAirportId).FirstOrDefault();
                if (next_route == null)
                {
                    result.Add(new ViewFlightPax()
                    {
                        FlightId = id,
                        Id = -2,
                        AirportId = flight.FromAirportId,
                        ToAirportId = next_flight.ToAirportId,
                        Total_CHR = 0,
                        Total_FOC = 0,
                        Total_OA = 0,
                        Total_Pax = 0,
                        Total_RES = 0,
                        Total_Rev = 0,
                        Total_STN = 0,
                        FlightId2 = next_flight.ID,


                    });
                }
                else
                    result.Add(next_route);
            }

            var pre_flight= await context.ViewFlightPaxes.Where(q => q.FlightId2 == id).FirstOrDefaultAsync();
            if (pre_flight != null)
                result.Add(pre_flight);
            return Ok(result);
        }




        public class FlightPlanSaveDto
        {
            public List<FlightPlanDto> updated { get; set; }
            public List<int> deleted { get; set; }
        }

        public class FlightPlanDto
        {
            public int ID { get; set; }
            public string Register { get; set; }
            public int? RegisterID { get; set; }
            public int? FromAirport { get; set; }
            public int? ToAirport { get; set; }
            public string FromAirportIATA { get; set; }
            public string ToAirportIATA { get; set; }
            public DateTime? STD { get; set; }
            public string STD2 { get; set; }
            public DateTime? STDLocal { get; set; }
            public string STDLocal2 { get; set; }
            public DateTime? STA { get; set; }
            public string STA2 { get; set; }
            public DateTime? STALocal { get; set; }
            public string STALocal2 { get; set; }
            public DateTime? STDDay { get; set; }
            public string STDDay2 { get; set; }
            public string FlightNumber { get; set; }
        }



    }
}
