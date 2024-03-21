using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace ApiLogUTC.ViewModels
{
    public class FlightSaveDto
    {
        public int ID { get; set; }

        public int? SendDelaySMS { get; set; }
        public int? SendCancelSMS { get; set; }
        public int? SendNiraSMS { get; set; }
        public int? FlightStatusID { get; set; }

        public DateTime? ChocksOut { get; set; }
        public DateTime? Takeoff { get; set; }
        public DateTime? Landing { get; set; }
        public DateTime? ChocksIn { get; set; }

        public int? BlockH { get; set; }
        public int? BlockM { get; set; }
        public decimal? GWTO { get; set; }
        public decimal? GWLand { get; set; }

        public decimal? FuelDeparture { get; set; }
        public decimal? FuelDensity { get; set; }
        public decimal? FuelArrival { get; set; }

        public string SerialNo { get; set; }
        public string LTR { get; set; }
        public int? PaxAdult { get; set; }
        public int? NightTime { get; set; }
        public int? PaxInfant { get; set; }
        public int? PaxChild { get; set; }
        public int? CargoWeight { get; set; }
        public int? CargoUnitID { get; set; }
        public int? BaggageCount { get; set; }

        public int? CargoCount { get; set; }
        public int? BaggageWeight { get; set; }
        public int? FuelUnitID { get; set; }
        public string ArrivalRemark { get; set; }
        public string DepartureRemark { get; set; }

        public int? UserId { get; set; }
        public string UserName { get; set; }
        public int? ToAirportId { get; set; }

        public DateTime? STA { get; set; }
        public int? CancelReasonId { get; set; }
        public string CancelRemark { get; set; }
        public DateTime? CancelDate { get; set; }
        public int? OToAirportId { get; set; }
        public DateTime? OSTA { get; set; }
        public string OToAirportIATA { get; set; }
        public int? RedirectReasonId { get; set; }
        public string RedirectRemark { get; set; }
        public DateTime? RedirectDate { get; set; }
        public int? RampReasonId { get; set; }
        public string RampRemark { get; set; }
        public DateTime? RampDate { get; set; }
        public int? FPFlightHH { get; set; }
        public int? FPFlightMM { get; set; }
        public decimal? FPFuel { get; set; }
        public decimal? Defuel { get; set; }
        public decimal? UsedFuel { get; set; }
        public int? JLBLHH { get; set; }
        public int? JLBLMM { get; set; }
        public int? PFLR { get; set; }

        public List<Models.FlightStatusLog> StatusLog = new List<Models.FlightStatusLog>();
        public List<Models.FlightDelay> Delays = new List<Models.FlightDelay>();
        // public List<ViewModels.EstimatedDelay> EstimatedDelays = new List<ViewModels.EstimatedDelay>();

        public string ChrCode { get; set; }
        public string ChrTitle { get; set; }
        public int? ChrAdult { get; set; }
        public int? ChrChild { get; set; }
        public int? ChrInfant { get; set; }
        public int? ChrCapacity { get; set; }
        public int? UpdateDelays { get; set; }



        public int? FreeAWBCount { get; set; }
        public int? FreeAWBWeight { get; set; }
        public int? CargoCost { get; set; }
        public int? NoShowCount { get; set; }
        public int? NoShowPieces { get; set; }
        public int? NoGoCount { get; set; }
        public int? NoGoPieces { get; set; }
        public int? DSBreakfast { get; set; }
        public int? DSWarmFood { get; set; }
        public int? DSColdFood { get; set; }
        public int? DSRefreshment { get; set; }
        public string Ready { get; set; }
        public string Start { get; set; }
        public int? YClass { get; set; }
        public int? CClass { get; set; }
        public int? PaxAdult50 { get; set; }
        public int? PaxChild50 { get; set; }
        public int? PaxInfant50 { get; set; }
        public int? PaxAdult100 { get; set; }
        public int? PaxChild100 { get; set; }
        public int? PaxInfant100 { get; set; }
        public int? PaxVIP { get; set; }
        public int? PaxCIP { get; set; }
        public int? PaxHUM { get; set; }
        public int? PaxUM { get; set; }
        public int? PaxAVI { get; set; }
        public int? PaxWCHR { get; set; }
        public int? PaxSTRC { get; set; }
        public int? FreeAWBPieces { get; set; }
        public int? CargoPieces { get; set; }
        public int? PaxPIRLost { get; set; }
        public int? PaxPIRDamage { get; set; }
        public int? PaxPIRFound { get; set; }
        public int? CargoPIRLost { get; set; }
        public int? CargoPIRDamage { get; set; }
        public int? CargoPIRFound { get; set; }
        public int? LimitTag { get; set; }
        public int? RushTag { get; set; }
        public string CLCheckIn { get; set; }
        public string CLPark { get; set; }
        public string CLAddTools { get; set; }
        public string CLBusReady { get; set; }
        public string CLPaxOut { get; set; }
        public string CLDepoOut { get; set; }
        public string CLServicePresence { get; set; }
        public string CLCleaningStart { get; set; }
        public string CLTechReady { get; set; }
        public string CLBagSent { get; set; }
        public string CLCateringLoad { get; set; }
        public string CLFuelStart { get; set; }
        public string CLFuelEnd { get; set; }
        public string CLCleaningEnd { get; set; }
        public string CLBoardingStart { get; set; }
        public string CLBoardingEnd { get; set; }
        public string CLLoadSheetStart { get; set; }
        public string CLGateClosed { get; set; }
        public string CLTrafficCrew { get; set; }
        public string CLLoadCrew { get; set; }
        public string CLForbiddenObj { get; set; }
        public string CLLoadSheetSign { get; set; }
        public string CLLoadingEnd { get; set; }
        public string CLDoorClosed { get; set; }
        public string CLEqDC { get; set; }
        public string CLMotorStart { get; set; }
        public string CLMovingStart { get; set; }
        public string CLACStart { get; set; }
        public string CLACEnd { get; set; }
        public string CLGPUStart { get; set; }
        public string CLGPUEnd { get; set; }
        public int? CLDepStairs { get; set; }
        public int? CLDepGPU { get; set; }
        public int? CLDepCrewCar { get; set; }
        public int? CLDepCrewCarCount { get; set; }
        public int? CLDepCabinService { get; set; }
        public int? CLDepCateringCar { get; set; }
        public int? CLDepPatientCar { get; set; }
        public int? CLDepPaxCar { get; set; }
        public int? CLDepPaxCarCount { get; set; }
        public int? CLDepPushback { get; set; }
        public int? CLDepWaterService { get; set; }
        public int? CLDepAC { get; set; }
        public int? CLDepDeIce { get; set; }
        public string CLDepEqRemark { get; set; }
        public int? CLArrStairs { get; set; }
        public int? CLArrGPU { get; set; }
        public int? CLArrCrewCar { get; set; }
        public int? CLArrCrewCarCount { get; set; }
        public int? CLArrCabinService { get; set; }
        public int? CLArrPatientCar { get; set; }
        public int? CLArrPaxCar { get; set; }
        public int? CLArrPaxCarCount { get; set; }
        public int? CLArrToiletService { get; set; }
        public string CLArrEqRemark { get; set; }

        public int? WLCount { get; set; }
        public long? WLCost { get; set; }
        public int? ExBagWeight { get; set; }
        public long? ExBagCost { get; set; }

        public int? TotalTrafficLoad { get; set; }


    }


    public class ViewFlightInformationDto
    {
        public int ID { get; set; }
        public string ResKey { get; set; }
        public DateTime? DutyStartLocal { get; set; }
        public DateTime? DutyEndLocal { get; set; }
        public string ResTitle { get; set; }
        public int Id { get; set; }
        public bool IsBox { get; set; }
        public int? Duty { get; set; }
        public double? MaxFDPExtended { get; set; }



        public int IsDutyOver { get; set; }
        public int WOCLError { get; set; }
        public int? Flight { get; set; }
        public bool HasCrew { get; set; }
        public bool HasCrewProblem { get; set; }
        public bool ExtendedBySplitDuty { get; set; }
        // public bool SplitDuty { get; set; }

        public bool AllCrewAssigned { get; set; }
        public int? BoxId { get; set; }
        public string Flights { get; set; }
        public int? CalendarId { get; set; }
        public DateTime? Date { get; set; }
        public List<ViewFlightInformationDto> BoxItems = new List<ViewFlightInformationDto>();
        public int taskID { get; set; }
        public int? FlightPlanId { get; set; }
        public int? BaggageCount { get; set; }
        public int? CargoUnitID { get; set; }
        public string CargoUnit { get; set; }
        public string FuelUnit { get; set; }
        public int? CargoWeight { get; set; }
        public int? PaxChild { get; set; }
        public int? PaxInfant { get; set; }
        public int? FlightStatusUserId { get; set; }
        public int? PaxAdult { get; set; }
        public int? NightTime { get; set; }
        public int? TotalPax { get; set; }
        public int? PaxOver { get; set; }
        public decimal? FuelArrival { get; set; }
        public decimal? FuelDeparture { get; set; }
        public decimal? FuelActual { get; set; }
        public decimal? FuelPlanned { get; set; }
        public decimal? GWLand { get; set; }
        public decimal? GWTO { get; set; }
        public byte? BlockM { get; set; }
        public int? BlockH { get; set; }
        public int? FlightH { get; set; }
        public byte? FlightM { get; set; }
        public DateTime? ChocksIn { get; set; }
        public DateTime? DateStatus { get; set; }
        public DateTime? Landing { get; set; }
        public DateTime? Takeoff { get; set; }
        public DateTime? ChocksOut { get; set; }
        public DateTime? STD { get; set; }
        public DateTime? STA { get; set; }
        public DateTime STDDay { get; set; }
        public int FlightStatusID { get; set; }
        public int? RegisterID { get; set; }
        public int? FlightTypeID { get; set; }
        public int? TypeId { get; set; }
        public int? AirlineOperatorsID { get; set; }
        public string FlightNumber { get; set; }
        public int? FromAirport { get; set; }
        public int? ToAirport { get; set; }
        public DateTime? STAPlanned { get; set; }
        public DateTime? STDPlanned { get; set; }
        public int? FlightHPlanned { get; set; }
        public int? FlightMPlanned { get; set; }
        public string FlightPlan { get; set; }
        public int? CustomerId { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? DateActive { get; set; }
        public string FromAirportName { get; set; }
        public string FromAirportIATA { get; set; }
        public int? FromAirportCityId { get; set; }
        public string ToAirportName { get; set; }
        public string ToAirportIATA { get; set; }
        public int? ToAirportCityId { get; set; }
        public string FromAirportCity { get; set; }
        public string ToAirportCity { get; set; }
        public string AircraftType { get; set; }
        public string Register { get; set; }
        public int? MSN { get; set; }
        public string FlightStatus { get; set; }
        public string FlightStatusBgColor { get; set; }
        public string FlightStatusColor { get; set; }
        public string FlightStatusClass { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public string notes { get; set; }
        public int status { get; set; }
        public int progress { get; set; }
        public string taskName { get; set; }
        public DateTime? startDate { get; set; }
        public decimal? duration { get; set; }
        public int taskId { get; set; }
        public int? FlightGroupID { get; set; }
        public int? PlanId { get; set; }
        public int? ManufacturerId { get; set; }
        public string Manufacturer { get; set; }
        public string ToCountry { get; set; }
        public string ToSortName { get; set; }
        public string ToCity { get; set; }
        public string FromSortName { get; set; }
        public string FromContry { get; set; }
        public string FromCity { get; set; }
        public decimal? FromLatitude { get; set; }
        public decimal? FromLongitude { get; set; }
        public decimal? ToLatitude { get; set; }
        public decimal? ToLongitude { get; set; }
        public int? CargoCount { get; set; }
        public int? BaggageWeight { get; set; }
        public int? FuelUnitID { get; set; }
        public string ArrivalRemark { get; set; }
        public string DepartureRemark { get; set; }
        public int? TotalSeat { get; set; }
        public int? EstimatedDelay { get; set; }
        public int? CancelReasonId { get; set; }
        public string CancelReason { get; set; }
        public string CancelRemark { get; set; }
        public DateTime? CancelDate { get; set; }

        public int? RedirectReasonId { get; set; }
        public string RedirectReason { get; set; }
        public string RedirectRemark { get; set; }
        public DateTime? RedirectDate { get; set; }
        public int? RampReasonId { get; set; }
        public string RampReason { get; set; }
        public string RampRemark { get; set; }
        public DateTime? RampDate { get; set; }

        public int? OToAirportId { get; set; }
        public string OToAirportIATA { get; set; }
        public DateTime? OSTA { get; set; }
        public List<int> resourceId { get; set; }

        public int? BaseId { get; set; }
        public string BaseIATA { get; set; }

        public string BaseName { get; set; }

        public decimal? Defuel { get; set; }
        public decimal? FPFuel { get; set; }
        public bool? SplitDuty { get; set; }
        public bool? IsPositioning { get; set; }
        public int? FPFlightHH { get; set; }
        public int? FPFlightMM { get; set; }

        public int MaleFemalError { get; set; }
        public int? MatchingListError { get; set; }
        public int? LinkedFlight { get; set; }
        public string LinkedFlightNumber { get; set; }
        public decimal? UsedFuel { get; set; }
        public int? JLBLHH { get; set; }
        public int? JLBLMM { get; set; }
        public int? PFLR { get; set; }
        public static void Fill(Models.ViewFlightInformation entity, ViewModels.ViewFlightInformationDto viewflightinformation)
        {
            entity.ID = viewflightinformation.ID;

            entity.FlightPlanId = viewflightinformation.FlightPlanId;
            entity.BaggageCount = viewflightinformation.BaggageCount;
            entity.CargoUnitID = viewflightinformation.CargoUnitID;
            entity.CargoUnit = viewflightinformation.CargoUnit;
            entity.CargoWeight = viewflightinformation.CargoWeight;
            entity.PaxChild = viewflightinformation.PaxChild;
            entity.PaxInfant = viewflightinformation.PaxInfant;
            entity.PaxAdult = viewflightinformation.PaxAdult;
            entity.FuelArrival = viewflightinformation.FuelArrival;
            entity.FuelDeparture = viewflightinformation.FuelDeparture;
            entity.FuelActual = viewflightinformation.FuelActual;
            entity.FuelPlanned = viewflightinformation.FuelPlanned;
            entity.GWLand = viewflightinformation.GWLand;
            entity.GWTO = viewflightinformation.GWTO;
            entity.BlockM = viewflightinformation.BlockM;
            entity.BlockH = viewflightinformation.BlockH;
            entity.FlightH = viewflightinformation.FlightH;
            entity.FlightM = viewflightinformation.FlightM;
            entity.ChocksIn = viewflightinformation.ChocksIn;
            entity.Landing = viewflightinformation.Landing;
            entity.Takeoff = viewflightinformation.Takeoff;
            entity.ChocksOut = viewflightinformation.ChocksOut;
            entity.STD = viewflightinformation.STD;
            entity.STA = viewflightinformation.STA;
            entity.FlightStatusID = viewflightinformation.FlightStatusID;
            entity.RegisterID = viewflightinformation.RegisterID;
            entity.FlightTypeID = viewflightinformation.FlightTypeID;
            entity.TypeId = viewflightinformation.TypeId;
            entity.AirlineOperatorsID = viewflightinformation.AirlineOperatorsID;
            entity.FlightNumber = viewflightinformation.FlightNumber;
            entity.FromAirport = viewflightinformation.FromAirport;
            entity.ToAirport = viewflightinformation.ToAirport;
            entity.STAPlanned = viewflightinformation.STAPlanned;
            entity.STDPlanned = viewflightinformation.STDPlanned;
            entity.FlightHPlanned = viewflightinformation.FlightHPlanned;
            entity.FlightMPlanned = viewflightinformation.FlightMPlanned;
            entity.FlightPlan = viewflightinformation.FlightPlan;
            entity.CustomerId = viewflightinformation.CustomerId;
            entity.IsActive = viewflightinformation.IsActive;
            entity.DateActive = viewflightinformation.DateActive;
            entity.FromAirportName = viewflightinformation.FromAirportName;
            entity.FromAirportIATA = viewflightinformation.FromAirportIATA;
            //entity.FromAirportCityId = viewflightinformation.FromAirportCityId;
            entity.ToAirportName = viewflightinformation.ToAirportName;
            entity.ToAirportIATA = viewflightinformation.ToAirportIATA;
            //  entity.ToAirportCityId = viewflightinformation.ToAirportCityId;
            //  entity.FromAirportCity = viewflightinformation.FromAirportCity;
            //  entity.ToAirportCity = viewflightinformation.ToAirportCity;
            entity.AircraftType = viewflightinformation.AircraftType;
            entity.Register = viewflightinformation.Register;
            entity.MSN = viewflightinformation.MSN;
            entity.FlightStatus = viewflightinformation.FlightStatus;
            entity.FlightStatusBgColor = viewflightinformation.FlightStatusBgColor;
            entity.FlightStatusColor = viewflightinformation.FlightStatusColor;
            entity.FlightStatusClass = viewflightinformation.FlightStatusClass;
            entity.from = viewflightinformation.from;
            entity.to = viewflightinformation.to;
            entity.notes = viewflightinformation.notes;
            entity.status = viewflightinformation.status;
            entity.progress = viewflightinformation.progress;
            entity.taskName = viewflightinformation.taskName;
            entity.startDate = viewflightinformation.startDate;
            entity.duration = viewflightinformation.duration;
            entity.taskId = viewflightinformation.taskId;
            entity.FlightGroupID = viewflightinformation.FlightGroupID;
            entity.PlanId = viewflightinformation.PlanId;
            entity.ManufacturerId = viewflightinformation.ManufacturerId;
            entity.Manufacturer = viewflightinformation.Manufacturer;
            // entity.ToCountry = viewflightinformation.ToCountry;
            // entity.ToSortName = viewflightinformation.ToSortName;
            // entity.ToCity = viewflightinformation.ToCity;
            entity.FromSortName = viewflightinformation.FromSortName;
            entity.FromContry = viewflightinformation.FromContry;
            //  entity.FromCity = viewflightinformation.FromCity;
            entity.FromLatitude = viewflightinformation.FromLatitude;
            entity.FromLongitude = viewflightinformation.FromLongitude;
            entity.ToLatitude = viewflightinformation.ToLatitude;
            entity.ToLongitude = viewflightinformation.ToLongitude;
            entity.CargoCount = viewflightinformation.CargoCount;
            entity.BaggageWeight = viewflightinformation.BaggageWeight;
            entity.FuelUnitID = viewflightinformation.FuelUnitID;
            entity.ArrivalRemark = viewflightinformation.ArrivalRemark;
            entity.DepartureRemark = viewflightinformation.DepartureRemark;
            entity.TotalSeat = viewflightinformation.TotalSeat;
            entity.EstimatedDelay = viewflightinformation.EstimatedDelay;
            entity.PaxOver = viewflightinformation.PaxOver;
            entity.TotalPax = viewflightinformation.TotalPax;
            entity.FuelUnit = viewflightinformation.FuelUnit;
            entity.DateStatus = viewflightinformation.DateStatus;
            entity.FlightStatusUserId = viewflightinformation.FlightStatusUserId;
            entity.CancelDate = viewflightinformation.CancelDate;
            entity.CancelReasonId = viewflightinformation.CancelReasonId;
            entity.CancelReason = viewflightinformation.CancelReason;
            entity.CancelRemark = viewflightinformation.CancelRemark;
            entity.RedirectDate = viewflightinformation.RedirectDate;
            entity.RedirectReasonId = viewflightinformation.RedirectReasonId;
            entity.RedirectReason = viewflightinformation.RedirectReason;
            entity.RedirectRemark = viewflightinformation.RedirectRemark;
            entity.OSTA = viewflightinformation.OSTA;
            entity.OToAirportIATA = viewflightinformation.OToAirportIATA;
            entity.OToAirportId = viewflightinformation.OToAirportId;

            entity.RampDate = viewflightinformation.RampDate;
            entity.RampReasonId = viewflightinformation.RampReasonId;
            entity.RampReason = viewflightinformation.RampReason;
            entity.RampRemark = viewflightinformation.RampRemark;

            entity.FPFlightHH = viewflightinformation.FPFlightHH;
            entity.FPFlightMM = viewflightinformation.FPFlightMM;
            entity.Defuel = viewflightinformation.Defuel;
            entity.FPFuel = viewflightinformation.FPFuel;


        }
        public static void FillDto(Models.ViewFlightInformation entity, ViewModels.ViewFlightInformationDto viewflightinformation, int tzoffset, int? utc = 0)
        {

            tzoffset = Helper.GetTimeOffset((DateTime)entity.STD);
            if (utc == 1)
                tzoffset = 0;
            viewflightinformation.Date = entity.Date;
            viewflightinformation.resourceId = new List<int>();
            viewflightinformation.ID = entity.ID;
            viewflightinformation.Id = entity.ID;
            viewflightinformation.IsBox = false;
            viewflightinformation.HasCrew = false;
            viewflightinformation.BoxId = entity.BoxId;
            viewflightinformation.CalendarId = entity.CalendarId;
            viewflightinformation.HasCrewProblem = false;
            viewflightinformation.AllCrewAssigned = false;
            viewflightinformation.FlightPlanId = entity.FlightPlanId;
            viewflightinformation.BaggageCount = entity.BaggageCount;
            viewflightinformation.CargoUnitID = entity.CargoUnitID;
            viewflightinformation.CargoUnit = entity.CargoUnit;
            viewflightinformation.CargoWeight = entity.CargoWeight;
            viewflightinformation.PaxChild = entity.PaxChild;
            viewflightinformation.PaxInfant = entity.PaxInfant;
            viewflightinformation.PaxAdult = entity.PaxAdult;
            viewflightinformation.FuelArrival = entity.FuelArrival;
            viewflightinformation.FuelDeparture = entity.FuelDeparture;
            viewflightinformation.FuelActual = entity.FuelActual;
            viewflightinformation.FuelPlanned = entity.FuelPlanned;
            viewflightinformation.GWLand = entity.GWLand;
            viewflightinformation.GWTO = entity.GWTO;
            viewflightinformation.BlockM = entity.BlockM;
            viewflightinformation.BlockH = entity.BlockH;
            viewflightinformation.FlightH = entity.FlightH;
            viewflightinformation.FlightM = entity.FlightM;
            viewflightinformation.ChocksIn = entity.ChocksIn == null ? null : (Nullable<DateTime>)((DateTime)entity.ChocksIn).AddMinutes(tzoffset);
            viewflightinformation.Landing = entity.Landing == null ? null : (Nullable<DateTime>)((DateTime)entity.Landing).AddMinutes(tzoffset); ;
            viewflightinformation.Takeoff = entity.Takeoff == null ? null : (Nullable<DateTime>)((DateTime)entity.Takeoff).AddMinutes(tzoffset);
            viewflightinformation.ChocksOut = entity.ChocksOut == null ? null : (Nullable<DateTime>)((DateTime)entity.ChocksOut).AddMinutes(tzoffset);
            viewflightinformation.STD = entity.STD == null ? null : (Nullable<DateTime>)((DateTime)entity.STD).AddMinutes(tzoffset);
            viewflightinformation.STA = entity.STA == null ? null : (Nullable<DateTime>)((DateTime)entity.STA).AddMinutes(tzoffset);
            viewflightinformation.RampDate = entity.RampDate == null ? null : (Nullable<DateTime>)((DateTime)entity.RampDate).AddMinutes(tzoffset);
            viewflightinformation.FlightStatusID = (int)entity.FlightStatusID;
            viewflightinformation.RegisterID = entity.RegisterID;
            viewflightinformation.FlightTypeID = entity.FlightTypeID;
            viewflightinformation.TypeId = entity.TypeId;
            viewflightinformation.AirlineOperatorsID = entity.AirlineOperatorsID;
            viewflightinformation.FlightNumber = entity.FlightNumber;
            viewflightinformation.FromAirport = entity.FromAirport;
            viewflightinformation.ToAirport = entity.ToAirport;
            viewflightinformation.STAPlanned = entity.STAPlanned == null ? null : (Nullable<DateTime>)((DateTime)entity.STAPlanned).AddMinutes(tzoffset);
            viewflightinformation.STDPlanned = entity.STDPlanned == null ? null : (Nullable<DateTime>)((DateTime)entity.STDPlanned).AddMinutes(tzoffset);
            viewflightinformation.FlightHPlanned = entity.FlightHPlanned;
            viewflightinformation.FlightMPlanned = entity.FlightMPlanned;
            viewflightinformation.FlightPlan = entity.FlightPlan;
            viewflightinformation.CustomerId = entity.CustomerId;
            viewflightinformation.IsActive = entity.IsActive;
            viewflightinformation.DateActive = entity.DateActive;
            viewflightinformation.FromAirportName = entity.FromAirportName;
            viewflightinformation.FromAirportIATA = entity.FromAirportIATA;
            // viewflightinformation.FromAirportCityId = entity.FromAirportCityId;
            viewflightinformation.ToAirportName = entity.ToAirportName;
            viewflightinformation.ToAirportIATA = entity.ToAirportIATA;
            //viewflightinformation.ToAirportCityId = entity.ToAirportCityId;
            //viewflightinformation.FromAirportCity = entity.FromAirportCity;
            // viewflightinformation.ToAirportCity = entity.ToAirportCity;
            viewflightinformation.AircraftType = entity.AircraftType;
            viewflightinformation.Register = entity.Register;
            viewflightinformation.MSN = entity.MSN;
            viewflightinformation.FlightStatus = entity.FlightStatus;
            viewflightinformation.FlightStatusBgColor = entity.FlightStatusBgColor;
            viewflightinformation.FlightStatusColor = entity.FlightStatusColor;
            viewflightinformation.FlightStatusClass = entity.FlightStatusClass;
            viewflightinformation.from = entity.from;
            viewflightinformation.to = entity.to;
            viewflightinformation.notes = entity.notes;
            viewflightinformation.status = entity.status;
            viewflightinformation.progress = entity.progress;
            viewflightinformation.taskName = entity.taskName;
            viewflightinformation.startDate = entity.startDate == null ? null : (Nullable<DateTime>)((DateTime)entity.startDate).AddMinutes(tzoffset);
            viewflightinformation.duration = entity.duration;
            viewflightinformation.taskId = entity.taskId;
            viewflightinformation.taskID = entity.taskId;
            viewflightinformation.FlightGroupID = entity.FlightGroupID;
            viewflightinformation.PlanId = entity.PlanId;
            viewflightinformation.ManufacturerId = entity.ManufacturerId;
            viewflightinformation.Manufacturer = entity.Manufacturer;
            //viewflightinformation.ToCountry = entity.ToCountry;
            //viewflightinformation.ToSortName = entity.ToSortName;
            //viewflightinformation.ToCity = entity.ToCity;
            viewflightinformation.FromSortName = entity.FromSortName;
            viewflightinformation.FromContry = entity.FromContry;
            //viewflightinformation.FromCity = entity.FromCity;
            viewflightinformation.FromLatitude = entity.FromLatitude;
            viewflightinformation.FromLongitude = entity.FromLongitude;
            viewflightinformation.ToLatitude = entity.ToLatitude;
            viewflightinformation.ToLongitude = entity.ToLongitude;
            viewflightinformation.CargoCount = entity.CargoCount;
            viewflightinformation.BaggageWeight = entity.BaggageWeight;
            viewflightinformation.FuelUnitID = entity.FuelUnitID;
            viewflightinformation.ArrivalRemark = entity.ArrivalRemark;
            viewflightinformation.DepartureRemark = entity.DepartureRemark;
            viewflightinformation.TotalSeat = entity.TotalSeat;
            viewflightinformation.EstimatedDelay = entity.EstimatedDelay;
            viewflightinformation.PaxOver = entity.PaxOver;
            viewflightinformation.TotalPax = entity.TotalPax;
            viewflightinformation.NightTime = entity.NightTime;


            viewflightinformation.FuelUnit = entity.FuelUnit;
            viewflightinformation.DateStatus = entity.DateStatus == null ? null : (Nullable<DateTime>)((DateTime)entity.DateStatus).AddMinutes(tzoffset);
            viewflightinformation.FlightStatusUserId = entity.FlightStatusUserId;

            viewflightinformation.CancelDate = entity.CancelDate == null ? null : (Nullable<DateTime>)((DateTime)entity.CancelDate).AddMinutes(tzoffset);
            viewflightinformation.CancelReasonId = entity.CancelReasonId;
            viewflightinformation.CancelReason = entity.CancelReason;
            viewflightinformation.CancelRemark = entity.CancelRemark;



            viewflightinformation.RampReasonId = entity.RampReasonId;
            viewflightinformation.RampReason = entity.RampReason;
            viewflightinformation.RampRemark = entity.RampRemark;

            viewflightinformation.RedirectDate = entity.RedirectDate == null ? null : (Nullable<DateTime>)((DateTime)entity.RedirectDate).AddMinutes(tzoffset); ;
            viewflightinformation.RedirectReasonId = entity.RedirectReasonId;
            viewflightinformation.RedirectReason = entity.RedirectReason;
            viewflightinformation.RedirectRemark = entity.RedirectRemark;
            viewflightinformation.OSTA = entity.OSTA;
            viewflightinformation.OToAirportIATA = entity.OToAirportIATA;
            viewflightinformation.OToAirportId = entity.OToAirportId;

            viewflightinformation.BaseIATA = entity.BaseIATA;
            viewflightinformation.BaseId = entity.BaseId;
            viewflightinformation.BaseName = entity.BaseName;

            viewflightinformation.FPFlightHH = entity.FPFlightHH;
            viewflightinformation.FPFlightMM = entity.FPFlightMM;
            viewflightinformation.Defuel = entity.Defuel;
            viewflightinformation.FPFuel = entity.FPFuel;

            viewflightinformation.SplitDuty = entity.SplitDuty;
            viewflightinformation.MaleFemalError = entity.MaleFemalError == null ? 0 : (int)entity.MaleFemalError;
            viewflightinformation.MatchingListError = entity.MatchingListError == null ? 0 : (int)entity.MatchingListError;
            viewflightinformation.LinkedFlight = entity.LinkedFlight;
            viewflightinformation.LinkedFlightNumber = entity.LinkedFlightNumber;
            viewflightinformation.UsedFuel = entity.UsedFuel;
            viewflightinformation.JLBLHH = entity.JLBLHH;
            viewflightinformation.JLBLMM = entity.JLBLMM;
            viewflightinformation.PFLR = entity.PFLR;


        }
        public static void FillDto(Models.ViewLegTime entity, ViewModels.ViewFlightInformationDto viewflightinformation, int tzoffset)
        {
            viewflightinformation.Date = entity.Date;
            viewflightinformation.resourceId = new List<int>();
            viewflightinformation.ID = entity.ID;
            viewflightinformation.Id = entity.ID;
            viewflightinformation.IsBox = false;
            viewflightinformation.HasCrew = false;

            viewflightinformation.HasCrewProblem = false;
            viewflightinformation.AllCrewAssigned = false;
            viewflightinformation.FlightPlanId = entity.FlightPlanId;

            viewflightinformation.BlockM = entity.BlockM;
            viewflightinformation.BlockH = entity.BlockH;
            viewflightinformation.FlightH = entity.FlightH;
            viewflightinformation.FlightM = entity.FlightM;
            viewflightinformation.ChocksIn = entity.ChocksIn == null ? null : (Nullable<DateTime>)((DateTime)entity.ChocksIn).AddMinutes(tzoffset);
            viewflightinformation.Landing = entity.Landing == null ? null : (Nullable<DateTime>)((DateTime)entity.Landing).AddMinutes(tzoffset); ;
            viewflightinformation.Takeoff = entity.Takeoff == null ? null : (Nullable<DateTime>)((DateTime)entity.Takeoff).AddMinutes(tzoffset);
            viewflightinformation.ChocksOut = entity.ChocksOut == null ? null : (Nullable<DateTime>)((DateTime)entity.ChocksOut).AddMinutes(tzoffset);
            viewflightinformation.STD = entity.STD == null ? null : (Nullable<DateTime>)((DateTime)entity.STD).AddMinutes(tzoffset);
            viewflightinformation.STA = entity.STA == null ? null : (Nullable<DateTime>)((DateTime)entity.STA).AddMinutes(tzoffset);

            viewflightinformation.FlightStatusID = (int)entity.FlightStatusID;
            viewflightinformation.RegisterID = entity.RegisterID;
            viewflightinformation.FlightTypeID = entity.FlightTypeID;
            viewflightinformation.TypeId = entity.TypeId;

            viewflightinformation.FlightNumber = entity.FlightNumber;
            viewflightinformation.FromAirport = entity.FromAirport;
            viewflightinformation.ToAirport = entity.ToAirport;
            viewflightinformation.STAPlanned = entity.STAPlanned == null ? null : (Nullable<DateTime>)((DateTime)entity.STAPlanned).AddMinutes(tzoffset);
            viewflightinformation.STDPlanned = entity.STDPlanned == null ? null : (Nullable<DateTime>)((DateTime)entity.STDPlanned).AddMinutes(tzoffset);
            viewflightinformation.FlightHPlanned = entity.FlightHPlanned;
            viewflightinformation.FlightMPlanned = entity.FlightMPlanned;
            viewflightinformation.FlightPlan = entity.FlightPlan;
            viewflightinformation.CustomerId = entity.CustomerId;

            viewflightinformation.FromAirportIATA = entity.FromAirportIATA;
            // viewflightinformation.FromAirportCityId = entity.FromAirportCityId;

            viewflightinformation.ToAirportIATA = entity.ToAirportIATA;
            //viewflightinformation.ToAirportCityId = entity.ToAirportCityId;
            //viewflightinformation.FromAirportCity = entity.FromAirportCity;
            // viewflightinformation.ToAirportCity = entity.ToAirportCity;
            viewflightinformation.AircraftType = entity.AircraftType;
            viewflightinformation.Register = entity.Register;
            viewflightinformation.MSN = entity.MSN;
            viewflightinformation.FlightStatus = entity.FlightStatus;

            viewflightinformation.from = entity.from;
            viewflightinformation.to = entity.to;
            viewflightinformation.notes = entity.notes;
            viewflightinformation.status = (int)entity.status;
            viewflightinformation.progress = entity.progress;
            viewflightinformation.taskName = entity.taskName;
            viewflightinformation.duration = entity.duration;
            viewflightinformation.taskId = entity.taskId;
            viewflightinformation.taskID = entity.taskId;


            viewflightinformation.startDate = entity.startDate == null ? null : (Nullable<DateTime>)((DateTime)entity.startDate).AddMinutes(tzoffset);



            viewflightinformation.ArrivalRemark = entity.ArrivalRemark;
            viewflightinformation.DepartureRemark = entity.DepartureRemark;

            viewflightinformation.EstimatedDelay = entity.EstimatedDelay;





            viewflightinformation.OSTA = entity.OSTA;
            viewflightinformation.OToAirportIATA = entity.OToAirportIATA;
            viewflightinformation.OToAirportId = entity.OToAirportId;



            viewflightinformation.FPFlightHH = entity.FPFlightHH;
            viewflightinformation.FPFlightMM = entity.FPFlightMM;





        }

        public static ViewFlightInformationDto GetDto(Models.ViewFlightInformation entity, int tzoffset)
        {
            ViewModels.ViewFlightInformationDto viewflightinformation = new ViewFlightInformationDto();
            viewflightinformation.resourceId = new List<int>();
            viewflightinformation.ID = entity.ID;
            viewflightinformation.FlightPlanId = entity.FlightPlanId;
            viewflightinformation.BaggageCount = entity.BaggageCount;
            viewflightinformation.CargoUnitID = entity.CargoUnitID;
            viewflightinformation.CargoUnit = entity.CargoUnit;
            viewflightinformation.CargoWeight = entity.CargoWeight;
            viewflightinformation.PaxChild = entity.PaxChild;
            viewflightinformation.PaxInfant = entity.PaxInfant;
            viewflightinformation.PaxAdult = entity.PaxAdult;
            viewflightinformation.FuelArrival = entity.FuelArrival;
            viewflightinformation.FuelDeparture = entity.FuelDeparture;
            viewflightinformation.FuelActual = entity.FuelActual;
            viewflightinformation.FuelPlanned = entity.FuelPlanned;
            viewflightinformation.GWLand = entity.GWLand;
            viewflightinformation.GWTO = entity.GWTO;
            viewflightinformation.BlockM = entity.BlockM;
            viewflightinformation.BlockH = entity.BlockH;
            viewflightinformation.FlightH = entity.FlightH;
            viewflightinformation.FlightM = entity.FlightM;
            viewflightinformation.ChocksIn = entity.ChocksIn == null ? null : (Nullable<DateTime>)((DateTime)entity.ChocksIn).AddMinutes(tzoffset);
            viewflightinformation.Landing = entity.Landing == null ? null : (Nullable<DateTime>)((DateTime)entity.Landing).AddMinutes(tzoffset); ;
            viewflightinformation.Takeoff = entity.Takeoff == null ? null : (Nullable<DateTime>)((DateTime)entity.Takeoff).AddMinutes(tzoffset);
            viewflightinformation.ChocksOut = entity.ChocksOut == null ? null : (Nullable<DateTime>)((DateTime)entity.ChocksOut).AddMinutes(tzoffset);
            viewflightinformation.STD = entity.STD == null ? null : (Nullable<DateTime>)((DateTime)entity.STD).AddMinutes(tzoffset);
            viewflightinformation.STA = entity.STA == null ? null : (Nullable<DateTime>)((DateTime)entity.STA).AddMinutes(tzoffset);
            viewflightinformation.FlightStatusID = (int)entity.FlightStatusID;
            viewflightinformation.RegisterID = entity.RegisterID;
            viewflightinformation.FlightTypeID = entity.FlightTypeID;
            viewflightinformation.TypeId = entity.TypeId;
            viewflightinformation.AirlineOperatorsID = entity.AirlineOperatorsID;
            viewflightinformation.FlightNumber = entity.FlightNumber;
            viewflightinformation.FromAirport = entity.FromAirport;
            viewflightinformation.ToAirport = entity.ToAirport;
            viewflightinformation.STAPlanned = entity.STAPlanned == null ? null : (Nullable<DateTime>)((DateTime)entity.STAPlanned).AddMinutes(tzoffset);
            viewflightinformation.STDPlanned = entity.STDPlanned == null ? null : (Nullable<DateTime>)((DateTime)entity.STDPlanned).AddMinutes(tzoffset);
            viewflightinformation.FlightHPlanned = entity.FlightHPlanned;
            viewflightinformation.FlightMPlanned = entity.FlightMPlanned;
            viewflightinformation.FlightPlan = entity.FlightPlan;
            viewflightinformation.CustomerId = entity.CustomerId;
            viewflightinformation.IsActive = entity.IsActive;
            viewflightinformation.DateActive = entity.DateActive;
            viewflightinformation.FromAirportName = entity.FromAirportName;
            viewflightinformation.FromAirportIATA = entity.FromAirportIATA;
            // viewflightinformation.FromAirportCityId = entity.FromAirportCityId;
            viewflightinformation.ToAirportName = entity.ToAirportName;
            viewflightinformation.ToAirportIATA = entity.ToAirportIATA;
            // viewflightinformation.ToAirportCityId = entity.ToAirportCityId;
            // viewflightinformation.FromAirportCity = entity.FromAirportCity;
            // viewflightinformation.ToAirportCity = entity.ToAirportCity;
            viewflightinformation.AircraftType = entity.AircraftType;
            viewflightinformation.Register = entity.Register;
            viewflightinformation.MSN = entity.MSN;
            viewflightinformation.FlightStatus = entity.FlightStatus;
            viewflightinformation.FlightStatusBgColor = entity.FlightStatusBgColor;
            viewflightinformation.FlightStatusColor = entity.FlightStatusColor;
            viewflightinformation.FlightStatusClass = entity.FlightStatusClass;
            viewflightinformation.from = entity.from;
            viewflightinformation.to = entity.to;
            viewflightinformation.notes = entity.notes;
            viewflightinformation.status = entity.status;
            viewflightinformation.progress = entity.progress;
            viewflightinformation.taskName = entity.taskName;
            viewflightinformation.startDate = entity.startDate == null ? null : (Nullable<DateTime>)((DateTime)entity.startDate).AddMinutes(tzoffset);
            viewflightinformation.duration = entity.duration;
            viewflightinformation.taskId = entity.taskId;
            viewflightinformation.FlightGroupID = entity.FlightGroupID;
            viewflightinformation.PlanId = entity.PlanId;
            viewflightinformation.ManufacturerId = entity.ManufacturerId;
            viewflightinformation.Manufacturer = entity.Manufacturer;
            //viewflightinformation.ToCountry = entity.ToCountry;
            //viewflightinformation.ToSortName = entity.ToSortName;
            //viewflightinformation.ToCity = entity.ToCity;
            viewflightinformation.FromSortName = entity.FromSortName;
            viewflightinformation.FromContry = entity.FromContry;
            //viewflightinformation.FromCity = entity.FromCity;
            viewflightinformation.FromLatitude = entity.FromLatitude;
            viewflightinformation.FromLongitude = entity.FromLongitude;
            viewflightinformation.ToLatitude = entity.ToLatitude;
            viewflightinformation.ToLongitude = entity.ToLongitude;
            viewflightinformation.CargoCount = entity.CargoCount;
            viewflightinformation.BaggageWeight = entity.BaggageWeight;
            viewflightinformation.FuelUnitID = entity.FuelUnitID;
            viewflightinformation.ArrivalRemark = entity.ArrivalRemark;
            viewflightinformation.DepartureRemark = entity.DepartureRemark;
            viewflightinformation.TotalSeat = entity.TotalSeat;
            viewflightinformation.EstimatedDelay = entity.EstimatedDelay;
            viewflightinformation.PaxOver = entity.PaxOver;
            viewflightinformation.TotalPax = entity.TotalPax;
            viewflightinformation.FuelUnit = entity.FuelUnit;
            viewflightinformation.DateStatus = entity.DateStatus == null ? null : (Nullable<DateTime>)((DateTime)entity.DateStatus).AddMinutes(tzoffset);
            viewflightinformation.FlightStatusUserId = entity.FlightStatusUserId;
            viewflightinformation.CancelDate = entity.CancelDate;
            viewflightinformation.CancelReasonId = entity.CancelReasonId;
            viewflightinformation.CancelReason = entity.CancelReason;
            viewflightinformation.CancelRemark = entity.CancelRemark;



            viewflightinformation.RampDate = entity.RampDate == null ? null : (Nullable<DateTime>)((DateTime)entity.RampDate).AddMinutes(tzoffset);
            viewflightinformation.RampReasonId = entity.RampReasonId;
            viewflightinformation.RampReason = entity.RampReason;
            viewflightinformation.RampRemark = entity.RampRemark;

            viewflightinformation.RedirectDate = entity.RedirectDate;
            viewflightinformation.RedirectReasonId = entity.RedirectReasonId;
            viewflightinformation.RedirectReason = entity.RedirectReason;
            viewflightinformation.RedirectRemark = entity.RedirectRemark;
            viewflightinformation.OSTA = entity.OSTA;
            viewflightinformation.OToAirportIATA = entity.OToAirportIATA;
            viewflightinformation.OToAirportId = entity.OToAirportId;

            return viewflightinformation;
        }
    }
    public class ViewFlightsGanttDto
    {
        public int ID { get; set; }
        public string ResKey { get; set; }
        public DateTime? DutyStartLocal { get; set; }
        public DateTime? DutyEndLocal { get; set; }
        public string ResTitle { get; set; }
        public int Id { get; set; }
        public bool IsBox { get; set; }
        public int? Duty { get; set; }
        public double? MaxFDPExtended { get; set; }



        public int IsDutyOver { get; set; }
        public int WOCLError { get; set; }
        public int? Flight { get; set; }
        public bool HasCrew { get; set; }
        public bool HasCrewProblem { get; set; }
        public bool ExtendedBySplitDuty { get; set; }
        // public bool SplitDuty { get; set; }

        public bool AllCrewAssigned { get; set; }
        public int? BoxId { get; set; }
        public string Flights { get; set; }
        public int? CalendarId { get; set; }
        public DateTime? Date { get; set; }
        public List<ViewFlightInformationDto> BoxItems = new List<ViewFlightInformationDto>();
        public int taskID { get; set; }
        public int? FlightPlanId { get; set; }
        public int? BaggageCount { get; set; }
        public int? CargoUnitID { get; set; }
        public string CargoUnit { get; set; }
        public string FuelUnit { get; set; }
        public int? CargoWeight { get; set; }
        public int? PaxChild { get; set; }
        public int? PaxInfant { get; set; }
        public int? FlightStatusUserId { get; set; }
        public int? PaxAdult { get; set; }
        public int? NightTime { get; set; }
        public int? TotalPax { get; set; }
        public int? PaxOver { get; set; }
        public decimal? FuelArrival { get; set; }
        public decimal? FuelDeparture { get; set; }
        public decimal? FuelActual { get; set; }
        public decimal? FuelPlanned { get; set; }
        public decimal? GWLand { get; set; }
        public decimal? GWTO { get; set; }
        public byte? BlockM { get; set; }
        public int? BlockH { get; set; }
        public int? FlightH { get; set; }
        public byte? FlightM { get; set; }
        public DateTime? ChocksIn { get; set; }
        public DateTime? DateStatus { get; set; }
        public DateTime? Landing { get; set; }
        public DateTime? Takeoff { get; set; }
        public DateTime? ChocksOut { get; set; }
        public DateTime? STD { get; set; }
        public DateTime? STA { get; set; }
        public DateTime STDDay { get; set; }
        public int FlightStatusID { get; set; }
        public int? RegisterID { get; set; }
        public int? FlightTypeID { get; set; }
        public int? TypeId { get; set; }
        public int? AirlineOperatorsID { get; set; }
        public string FlightNumber { get; set; }
        public int? FromAirport { get; set; }
        public int? ToAirport { get; set; }
        public DateTime? STAPlanned { get; set; }
        public DateTime? STDPlanned { get; set; }
        public int? FlightHPlanned { get; set; }
        public int? FlightMPlanned { get; set; }
        public string FlightPlan { get; set; }
        public int? CustomerId { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? DateActive { get; set; }
        public string FromAirportName { get; set; }
        public string FromAirportIATA { get; set; }
        public int? FromAirportCityId { get; set; }
        public string ToAirportName { get; set; }
        public string ToAirportIATA { get; set; }
        public int? ToAirportCityId { get; set; }
        public string FromAirportCity { get; set; }
        public string ToAirportCity { get; set; }
        public string AircraftType { get; set; }
        public string Register { get; set; }
        public int? MSN { get; set; }
        public string FlightStatus { get; set; }
        public string FlightStatusBgColor { get; set; }
        public string FlightStatusColor { get; set; }
        public string FlightStatusClass { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public string notes { get; set; }
        public int status { get; set; }
        public int progress { get; set; }
        public string taskName { get; set; }
        public DateTime? startDate { get; set; }
        public decimal? duration { get; set; }
        public int taskId { get; set; }
        public int? FlightGroupID { get; set; }
        public int? PlanId { get; set; }
        public int? ManufacturerId { get; set; }
        public string Manufacturer { get; set; }
        public string ToCountry { get; set; }
        public string ToSortName { get; set; }
        public string ToCity { get; set; }
        public string FromSortName { get; set; }
        public string FromContry { get; set; }
        public string FromCity { get; set; }
        public double? FromLatitude { get; set; }
        public double? FromLongitude { get; set; }
        public double? ToLatitude { get; set; }
        public double? ToLongitude { get; set; }
        public int? CargoCount { get; set; }
        public int? BaggageWeight { get; set; }
        public int? FuelUnitID { get; set; }
        public string ArrivalRemark { get; set; }
        public string DepartureRemark { get; set; }
        public int? TotalSeat { get; set; }
        public int? EstimatedDelay { get; set; }
        public int? CancelReasonId { get; set; }
        public string CancelReason { get; set; }
        public string CancelRemark { get; set; }
        public DateTime? CancelDate { get; set; }

        public int? RedirectReasonId { get; set; }
        public string RedirectReason { get; set; }
        public string RedirectRemark { get; set; }
        public DateTime? RedirectDate { get; set; }
        public int? RampReasonId { get; set; }
        public string RampReason { get; set; }
        public string RampRemark { get; set; }
        public DateTime? RampDate { get; set; }

        public int? OToAirportId { get; set; }
        public string OToAirportIATA { get; set; }
        public DateTime? OSTA { get; set; }
        public List<int> resourceId { get; set; }

        public int? BaseId { get; set; }
        public string BaseIATA { get; set; }

        public string BaseName { get; set; }

        public decimal? Defuel { get; set; }
        public decimal? FPFuel { get; set; }
        public bool? SplitDuty { get; set; }
        public bool? IsPositioning { get; set; }
        public int? FPFlightHH { get; set; }
        public int? FPFlightMM { get; set; }

        public int MaleFemalError { get; set; }
        public int? MatchingListError { get; set; }
        public int? LinkedFlight { get; set; }
        public string LinkedFlightNumber { get; set; }
        public decimal? UsedFuel { get; set; }
        public int? JLBLHH { get; set; }
        public int? JLBLMM { get; set; }
        public int? PFLR { get; set; }
        public int? OTypeId { get; set; }

        public int? ChrAdult { get; set; }
        public int? ChrChild { get; set; }
        public int? ChrInfant { get; set; }
        public int? ChrCapacity { get; set; }

        public string ChrCode { get; set; }
        public string ChrTitle { get; set; }

        public string LTR { get; set; }
        public decimal? FuelDensity { get; set; }
        public string SerialNo { get; set; }

        public int? DefaultChrId { get; set; }

        public DateTime? Ready { get; set; }
        public DateTime? Start { get; set; }
        public int? CargoPieces { get; set; }
        public long? CargoCost { get; set; }
        public int? FreeAWBCount { get; set; }
        public int? FreeAWBPieces { get; set; }
        public int? FreeAWBWeight { get; set; }
        public int? NoShowCount { get; set; }
        public int? NoShowPieces { get; set; }
        public int? NoGoCount { get; set; }
        public int? NoGoPieces { get; set; }
        public int? DSBreakfast { get; set; }
        public int? DSWarmFood { get; set; }
        public int? DSColdFood { get; set; }
        public int? DSRefreshment { get; set; }
        public int? YClass { get; set; }
        public int? CClass { get; set; }
        public int? PaxAdult50 { get; set; }
        public int? PaxChild50 { get; set; }
        public int? PaxInfant50 { get; set; }
        public int? PaxAdult100 { get; set; }
        public int? PaxChild100 { get; set; }
        public int? PaxInfant100 { get; set; }
        public int? PaxVIP { get; set; }
        public int? PaxCIP { get; set; }
        public int? PaxHUM { get; set; }
        public int? PaxUM { get; set; }
        public int? PaxAVI { get; set; }
        public int? PaxWCHR { get; set; }
        public int? PaxSTRC { get; set; }
        public int? PaxPIRLost { get; set; }
        public int? PaxPIRDamage { get; set; }
        public int? PaxPIRFound { get; set; }
        public int? CargoPIRLost { get; set; }
        public int? CargoPIRDamage { get; set; }
        public int? CargoPIRFound { get; set; }
        public int? LimitTag { get; set; }
        public int? RushTag { get; set; }
        public DateTime? CLCheckIn { get; set; }
        public DateTime? CLPark { get; set; }
        public DateTime? CLAddTools { get; set; }
        public DateTime? CLBusReady { get; set; }
        public DateTime? CLPaxOut { get; set; }
        public DateTime? CLDepoOut { get; set; }
        public DateTime? CLServicePresence { get; set; }
        public DateTime? CLCleaningStart { get; set; }
        public DateTime? CLTechReady { get; set; }
        public DateTime? CLBagSent { get; set; }
        public DateTime? CLCateringLoad { get; set; }
        public DateTime? CLFuelStart { get; set; }
        public DateTime? CLFuelEnd { get; set; }
        public DateTime? CLCleaningEnd { get; set; }
        public DateTime? CLBoardingStart { get; set; }
        public DateTime? CLBoardingEnd { get; set; }
        public DateTime? CLLoadSheetStart { get; set; }
        public DateTime? CLGateClosed { get; set; }
        public DateTime? CLTrafficCrew { get; set; }
        public DateTime? CLLoadCrew { get; set; }
        public DateTime? CLForbiddenObj { get; set; }
        public DateTime? CLLoadSheetSign { get; set; }
        public DateTime? CLLoadingEnd { get; set; }
        public DateTime? CLDoorClosed { get; set; }
        public DateTime? CLEqDC { get; set; }
        public DateTime? CLMotorStart { get; set; }
        public DateTime? CLMovingStart { get; set; }
        public DateTime? CLACStart { get; set; }
        public DateTime? CLACEnd { get; set; }
        public DateTime? CLGPUStart { get; set; }
        public DateTime? CLGPUEnd { get; set; }
        public int? CLDepStairs { get; set; }
        public int? CLDepGPU { get; set; }
        public int? CLDepCrewCar { get; set; }
        public int? CLDepCrewCarCount { get; set; }
        public int? CLDepCabinService { get; set; }
        public int? CLDepCateringCar { get; set; }
        public int? CLDepPatientCar { get; set; }
        public int? CLDepPaxCar { get; set; }
        public int? CLDepPaxCarCount { get; set; }
        public int? CLDepPushback { get; set; }
        public int? CLDepWaterService { get; set; }
        public int? CLDepAC { get; set; }
        public int? CLDepDeIce { get; set; }
        public string CLDepEqRemark { get; set; }
        public int? CLArrStairs { get; set; }
        public int? CLArrGPU { get; set; }
        public int? CLArrCrewCar { get; set; }
        public int? CLArrCrewCarCount { get; set; }
        public int? CLArrCabinService { get; set; }
        public int? CLArrPatientCar { get; set; }
        public int? CLArrPaxCar { get; set; }
        public int? CLArrPaxCarCount { get; set; }
        public int? CLArrToiletService { get; set; }
        public string CLArrEqRemark { get; set; }


        public int? WLCount { get; set; }
        public long? WLCost { get; set; }
        public int? ExBagWeight { get; set; }
        public long? ExBagCost { get; set; }


        public int? TotalTrafficLoad { get; set; }


        public string OAircraftType { get; set; }
        public static void Fill(Models.ViewFlightsGantt entity, ViewModels.ViewFlightsGanttDto viewflightinformation)
        {
            entity.ID = viewflightinformation.ID;

            entity.FlightPlanId = viewflightinformation.FlightPlanId;
            entity.BaggageCount = viewflightinformation.BaggageCount;
            entity.CargoUnitID = viewflightinformation.CargoUnitID;
            entity.CargoUnit = viewflightinformation.CargoUnit;
            entity.CargoWeight = viewflightinformation.CargoWeight;
            entity.PaxChild = viewflightinformation.PaxChild;
            entity.PaxInfant = viewflightinformation.PaxInfant;
            entity.PaxAdult = viewflightinformation.PaxAdult;
            entity.FuelArrival = viewflightinformation.FuelArrival;
            entity.FuelDeparture = viewflightinformation.FuelDeparture;
            entity.FuelActual = viewflightinformation.FuelActual;
            entity.FuelPlanned = viewflightinformation.FuelPlanned;
            entity.GWLand = viewflightinformation.GWLand;
            entity.GWTO = viewflightinformation.GWTO;
            entity.BlockM = viewflightinformation.BlockM;
            entity.BlockH = viewflightinformation.BlockH;
            entity.FlightH = viewflightinformation.FlightH;
            entity.FlightM = viewflightinformation.FlightM;
            entity.ChocksIn = viewflightinformation.ChocksIn;
            entity.Landing = viewflightinformation.Landing;
            entity.Takeoff = viewflightinformation.Takeoff;
            entity.ChocksOut = viewflightinformation.ChocksOut;
            entity.STD = viewflightinformation.STD;
            entity.STA = viewflightinformation.STA;
            entity.FlightStatusID = viewflightinformation.FlightStatusID;
            entity.RegisterID = viewflightinformation.RegisterID;
            entity.FlightTypeID = viewflightinformation.FlightTypeID ?? 0;
            entity.TypeId = (int)viewflightinformation.TypeId;
            entity.AirlineOperatorsID = viewflightinformation.AirlineOperatorsID;
            entity.FlightNumber = viewflightinformation.FlightNumber;
            entity.FromAirport = viewflightinformation.FromAirport;
            entity.ToAirport = viewflightinformation.ToAirport;
            entity.STAPlanned = viewflightinformation.STAPlanned;
            entity.STDPlanned = viewflightinformation.STDPlanned;
            entity.FlightHPlanned = viewflightinformation.FlightHPlanned;
            entity.FlightMPlanned = viewflightinformation.FlightMPlanned;
            entity.FlightPlan = viewflightinformation.FlightPlan;
            entity.CustomerId = viewflightinformation.CustomerId;
            entity.IsActive = viewflightinformation.IsActive;
            entity.DateActive = viewflightinformation.DateActive;
            entity.FromAirportName = viewflightinformation.FromAirportName;
            entity.FromAirportIATA = viewflightinformation.FromAirportIATA;
            //entity.FromAirportCityId = viewflightinformation.FromAirportCityId;
            entity.ToAirportName = viewflightinformation.ToAirportName;
            entity.ToAirportIATA = viewflightinformation.ToAirportIATA;
            //  entity.ToAirportCityId = viewflightinformation.ToAirportCityId;
            //  entity.FromAirportCity = viewflightinformation.FromAirportCity;
            //  entity.ToAirportCity = viewflightinformation.ToAirportCity;
            entity.AircraftType = viewflightinformation.AircraftType;
            entity.Register = viewflightinformation.Register;
            entity.MSN = viewflightinformation.MSN;
            entity.FlightStatus = viewflightinformation.FlightStatus;
            entity.FlightStatusBgColor = viewflightinformation.FlightStatusBgColor;
            entity.FlightStatusColor = viewflightinformation.FlightStatusColor;
            entity.FlightStatusClass = viewflightinformation.FlightStatusClass;
            entity.from = viewflightinformation.from;
            entity.to = viewflightinformation.to;
            entity.notes = viewflightinformation.notes;
            entity.status = viewflightinformation.status;
            entity.progress = viewflightinformation.progress;
            entity.taskName = viewflightinformation.taskName;
            entity.startDate = viewflightinformation.startDate;
            entity.duration = viewflightinformation.duration;
            entity.taskId = viewflightinformation.taskId;
            entity.FlightGroupID = viewflightinformation.FlightGroupID;
            entity.PlanId = viewflightinformation.PlanId;
            entity.ManufacturerId = viewflightinformation.ManufacturerId;
            entity.Manufacturer = viewflightinformation.Manufacturer;
            // entity.ToCountry = viewflightinformation.ToCountry;
            // entity.ToSortName = viewflightinformation.ToSortName;
            // entity.ToCity = viewflightinformation.ToCity;
            entity.FromSortName = viewflightinformation.FromSortName;
            entity.FromContry = viewflightinformation.FromContry;
            //  entity.FromCity = viewflightinformation.FromCity;
            entity.FromLatitude = viewflightinformation.FromLatitude;
            entity.FromLongitude = viewflightinformation.FromLongitude;
            entity.ToLatitude = viewflightinformation.ToLatitude;
            entity.ToLongitude = viewflightinformation.ToLongitude;
            entity.CargoCount = viewflightinformation.CargoCount;
            entity.BaggageWeight = viewflightinformation.BaggageWeight;
            entity.FuelUnitID = viewflightinformation.FuelUnitID;
            entity.ArrivalRemark = viewflightinformation.ArrivalRemark;
            entity.DepartureRemark = viewflightinformation.DepartureRemark;
            entity.TotalSeat = viewflightinformation.TotalSeat;
            entity.EstimatedDelay = viewflightinformation.EstimatedDelay ?? 0;
            entity.PaxOver = viewflightinformation.PaxOver ?? 0;
            entity.TotalPax = viewflightinformation.TotalPax;
            entity.FuelUnit = viewflightinformation.FuelUnit;
            entity.DateStatus = viewflightinformation.DateStatus;
            entity.FlightStatusUserId = viewflightinformation.FlightStatusUserId;
            entity.CancelDate = viewflightinformation.CancelDate;
            entity.CancelReasonId = viewflightinformation.CancelReasonId;
            entity.CancelReason = viewflightinformation.CancelReason;
            entity.CancelRemark = viewflightinformation.CancelRemark;
            entity.RedirectDate = viewflightinformation.RedirectDate;
            entity.RedirectReasonId = viewflightinformation.RedirectReasonId;
            entity.RedirectReason = viewflightinformation.RedirectReason;
            entity.RedirectRemark = viewflightinformation.RedirectRemark;
            entity.OSTA = viewflightinformation.OSTA;
            entity.OToAirportIATA = viewflightinformation.OToAirportIATA;
            entity.OToAirportId = viewflightinformation.OToAirportId;

            entity.RampDate = viewflightinformation.RampDate;
            entity.RampReasonId = viewflightinformation.RampReasonId;
            entity.RampReason = viewflightinformation.RampReason;
            entity.RampRemark = viewflightinformation.RampRemark;

            entity.FPFlightHH = viewflightinformation.FPFlightHH;
            entity.FPFlightMM = viewflightinformation.FPFlightMM;
            entity.Defuel = viewflightinformation.Defuel;
            entity.FPFuel = viewflightinformation.FPFuel;




            entity.Ready = viewflightinformation.Ready;
            entity.Start = viewflightinformation.Start;
            entity.CargoPieces = viewflightinformation.CargoPieces;
            entity.CargoCost = viewflightinformation.CargoCost;
            entity.FreeAWBCount = viewflightinformation.FreeAWBCount;
            entity.FreeAWBPieces = viewflightinformation.FreeAWBPieces;
            entity.FreeAWBWeight = viewflightinformation.FreeAWBWeight;
            entity.NoShowCount = viewflightinformation.NoShowCount;
            entity.NoShowPieces = viewflightinformation.NoShowPieces;
            entity.NoGoCount = viewflightinformation.NoGoCount;
            entity.NoGoPieces = viewflightinformation.NoGoPieces;
            entity.DSBreakfast = viewflightinformation.DSBreakfast;
            entity.DSWarmFood = viewflightinformation.DSWarmFood;
            entity.DSColdFood = viewflightinformation.DSColdFood;
            entity.DSRefreshment = viewflightinformation.DSRefreshment;
            entity.YClass = viewflightinformation.YClass;
            entity.CClass = viewflightinformation.CClass;
            entity.PaxAdult50 = viewflightinformation.PaxAdult50;
            entity.PaxChild50 = viewflightinformation.PaxChild50;
            entity.PaxInfant50 = viewflightinformation.PaxInfant50;
            entity.PaxAdult100 = viewflightinformation.PaxAdult100;
            entity.PaxChild100 = viewflightinformation.PaxChild100;
            entity.PaxInfant100 = viewflightinformation.PaxInfant100;
            entity.PaxVIP = viewflightinformation.PaxVIP;
            entity.PaxCIP = viewflightinformation.PaxCIP;
            entity.PaxHUM = viewflightinformation.PaxHUM;
            entity.PaxUM = viewflightinformation.PaxUM;
            entity.PaxAVI = viewflightinformation.PaxAVI;
            entity.PaxWCHR = viewflightinformation.PaxWCHR;
            entity.PaxSTRC = viewflightinformation.PaxSTRC;
            entity.PaxPIRLost = viewflightinformation.PaxPIRLost;
            entity.PaxPIRDamage = viewflightinformation.PaxPIRDamage;
            entity.PaxPIRFound = viewflightinformation.PaxPIRFound;
            entity.CargoPIRLost = viewflightinformation.CargoPIRLost;
            entity.CargoPIRDamage = viewflightinformation.CargoPIRDamage;
            entity.CargoPIRFound = viewflightinformation.CargoPIRFound;
            entity.LimitTag = viewflightinformation.LimitTag;
            entity.RushTag = viewflightinformation.RushTag;
            entity.CLCheckIn = viewflightinformation.CLCheckIn;
            entity.CLPark = viewflightinformation.CLPark;
            entity.CLAddTools = viewflightinformation.CLAddTools;
            entity.CLBusReady = viewflightinformation.CLBusReady;
            entity.CLPaxOut = viewflightinformation.CLPaxOut;
            entity.CLDepoOut = viewflightinformation.CLDepoOut;
            entity.CLServicePresence = viewflightinformation.CLServicePresence;
            entity.CLCleaningStart = viewflightinformation.CLCleaningStart;
            entity.CLTechReady = viewflightinformation.CLTechReady;
            entity.CLBagSent = viewflightinformation.CLBagSent;
            entity.CLCateringLoad = viewflightinformation.CLCateringLoad;
            entity.CLFuelStart = viewflightinformation.CLFuelStart;
            entity.CLFuelEnd = viewflightinformation.CLFuelEnd;
            entity.CLCleaningEnd = viewflightinformation.CLCleaningEnd;
            entity.CLBoardingStart = viewflightinformation.CLBoardingStart;
            entity.CLBoardingEnd = viewflightinformation.CLBoardingEnd;
            entity.CLLoadSheetStart = viewflightinformation.CLLoadSheetStart;
            entity.CLGateClosed = viewflightinformation.CLGateClosed;
            entity.CLTrafficCrew = viewflightinformation.CLTrafficCrew;
            entity.CLLoadCrew = viewflightinformation.CLLoadCrew;
            entity.CLForbiddenObj = viewflightinformation.CLForbiddenObj;
            entity.CLLoadSheetSign = viewflightinformation.CLLoadSheetSign;
            entity.CLLoadingEnd = viewflightinformation.CLLoadingEnd;
            entity.CLDoorClosed = viewflightinformation.CLDoorClosed;
            entity.CLEqDC = viewflightinformation.CLEqDC;
            entity.CLMotorStart = viewflightinformation.CLMotorStart;
            entity.CLMovingStart = viewflightinformation.CLMovingStart;
            entity.CLACStart = viewflightinformation.CLACStart;
            entity.CLACEnd = viewflightinformation.CLACEnd;
            entity.CLGPUStart = viewflightinformation.CLGPUStart;
            entity.CLGPUEnd = viewflightinformation.CLGPUEnd;
            entity.CLDepStairs = viewflightinformation.CLDepStairs;
            entity.CLDepGPU = viewflightinformation.CLDepGPU;
            entity.CLDepCrewCar = viewflightinformation.CLDepCrewCar;
            entity.CLDepCrewCarCount = viewflightinformation.CLDepCrewCarCount;
            entity.CLDepCabinService = viewflightinformation.CLDepCabinService;
            entity.CLDepCateringCar = viewflightinformation.CLDepCateringCar;
            entity.CLDepPatientCar = viewflightinformation.CLDepPatientCar;
            entity.CLDepPaxCar = viewflightinformation.CLDepPaxCar;
            entity.CLDepPaxCarCount = viewflightinformation.CLDepPaxCarCount;
            entity.CLDepPushback = viewflightinformation.CLDepPushback;
            entity.CLDepWaterService = viewflightinformation.CLDepWaterService;
            entity.CLDepAC = viewflightinformation.CLDepAC;
            entity.CLDepDeIce = viewflightinformation.CLDepDeIce;
            entity.CLDepEqRemark = viewflightinformation.CLDepEqRemark;
            entity.CLArrStairs = viewflightinformation.CLArrStairs;
            entity.CLArrGPU = viewflightinformation.CLArrGPU;
            entity.CLArrCrewCar = viewflightinformation.CLArrCrewCar;
            entity.CLArrCrewCarCount = viewflightinformation.CLArrCrewCarCount;
            entity.CLArrCabinService = viewflightinformation.CLArrCabinService;
            entity.CLArrPatientCar = viewflightinformation.CLArrPatientCar;
            entity.CLArrPaxCar = viewflightinformation.CLArrPaxCar;
            entity.CLArrPaxCarCount = viewflightinformation.CLArrPaxCarCount;
            entity.CLArrToiletService = viewflightinformation.CLArrToiletService;
            entity.CLArrEqRemark = viewflightinformation.CLArrEqRemark;





        }
        public static void FillDto(Models.ViewFlightsGantt entity, ViewModels.ViewFlightsGanttDto viewflightinformation, int tzoffset, int? utc = 0)
        {

            tzoffset = Helper.GetTimeOffset((DateTime)entity.STD);
            var tzoffset2 = Helper.GetTimeOffset((DateTime)entity.STD);
            var tzoffset3 = TimeZoneInfo.Local.GetUtcOffset((DateTime)entity.STD).TotalMinutes;
            if (utc == 1)
            { tzoffset = 0; tzoffset3 = 0; }
            viewflightinformation.Date = entity.Date;
            viewflightinformation.OTypeId = entity.OTypeId;
            viewflightinformation.OAircraftType = entity.OAircraftType;
            viewflightinformation.resourceId = new List<int>();
            viewflightinformation.ID = entity.ID;
            viewflightinformation.Id = entity.ID;
            viewflightinformation.IsBox = false;
            viewflightinformation.HasCrew = false;
            viewflightinformation.BoxId = entity.BoxId;
            viewflightinformation.CalendarId = entity.CalendarId;
            viewflightinformation.HasCrewProblem = false;
            viewflightinformation.AllCrewAssigned = false;
            viewflightinformation.FlightPlanId = entity.FlightPlanId;
            viewflightinformation.BaggageCount = entity.BaggageCount;
            viewflightinformation.CargoUnitID = entity.CargoUnitID;
            viewflightinformation.CargoUnit = entity.CargoUnit;
            viewflightinformation.CargoWeight = entity.CargoWeight;
            viewflightinformation.PaxChild = entity.PaxChild;
            viewflightinformation.PaxInfant = entity.PaxInfant;
            viewflightinformation.PaxAdult = entity.PaxAdult;
            viewflightinformation.FuelArrival = entity.FuelArrival;
            viewflightinformation.FuelDeparture = entity.FuelDeparture;
            viewflightinformation.FuelActual = entity.FuelActual;
            viewflightinformation.FuelPlanned = entity.FuelPlanned;
            viewflightinformation.GWLand = entity.GWLand;
            viewflightinformation.GWTO = entity.GWTO;
            viewflightinformation.BlockM = entity.BlockM;
            viewflightinformation.BlockH = entity.BlockH;
            if (entity.FromAirportIATA == "NJF" || entity.FromAirportIATA == "BSR")
                viewflightinformation.GWLand = 180;
            else
                viewflightinformation.GWLand = tzoffset2;

            if (entity.ToAirportIATA == "NJF" || entity.ToAirportIATA == "BSR")
                viewflightinformation.GWTO = 180;
            else
                viewflightinformation.GWTO = tzoffset2;


            viewflightinformation.FlightH = entity.FlightH;
            viewflightinformation.FlightM = entity.FlightM;
            viewflightinformation.ChocksIn = entity.ChocksIn == null ? null : (Nullable<DateTime>)((DateTime)entity.ChocksIn).AddMinutes(tzoffset3);
            viewflightinformation.Landing = entity.Landing == null ? null : (Nullable<DateTime>)((DateTime)entity.Landing).AddMinutes(tzoffset3); ;
            viewflightinformation.Takeoff = entity.Takeoff == null ? null : (Nullable<DateTime>)((DateTime)entity.Takeoff).AddMinutes(tzoffset3);
            viewflightinformation.ChocksOut = entity.ChocksOut == null ? null : (Nullable<DateTime>)((DateTime)entity.ChocksOut).AddMinutes(tzoffset3);
            viewflightinformation.STD = entity.STD == null ? null : (Nullable<DateTime>)((DateTime)entity.STD).AddMinutes(tzoffset3);
            viewflightinformation.STA = entity.STA == null ? null : (Nullable<DateTime>)((DateTime)entity.STA).AddMinutes(tzoffset3);
            viewflightinformation.RampDate = entity.RampDate == null ? null : (Nullable<DateTime>)((DateTime)entity.RampDate).AddMinutes(tzoffset3);
            viewflightinformation.FlightStatusID = (int)entity.FlightStatusID;
            viewflightinformation.RegisterID = entity.RegisterID;
            viewflightinformation.FlightTypeID = entity.FlightTypeID;
            viewflightinformation.TypeId = entity.TypeId;
            viewflightinformation.AirlineOperatorsID = entity.AirlineOperatorsID;
            viewflightinformation.FlightNumber = entity.FlightNumber;
            viewflightinformation.FromAirport = entity.FromAirport;
            viewflightinformation.ToAirport = entity.ToAirport;
            viewflightinformation.STAPlanned = entity.STAPlanned == null ? null : (Nullable<DateTime>)((DateTime)entity.STAPlanned).AddMinutes(tzoffset3);
            viewflightinformation.STDPlanned = entity.STDPlanned == null ? null : (Nullable<DateTime>)((DateTime)entity.STDPlanned).AddMinutes(tzoffset3);
            viewflightinformation.FlightHPlanned = entity.FlightHPlanned;
            viewflightinformation.FlightMPlanned = entity.FlightMPlanned;
            viewflightinformation.FlightPlan = entity.FlightPlan;
            viewflightinformation.CustomerId = entity.CustomerId;
            viewflightinformation.IsActive = entity.IsActive;
            viewflightinformation.DateActive = entity.DateActive;
            viewflightinformation.FromAirportName = entity.FromAirportName;
            viewflightinformation.FromAirportIATA = entity.FromAirportIATA;
            // viewflightinformation.FromAirportCityId = entity.FromAirportCityId;
            viewflightinformation.ToAirportName = entity.ToAirportName;
            viewflightinformation.ToAirportIATA = entity.ToAirportIATA;
            //viewflightinformation.ToAirportCityId = entity.ToAirportCityId;
            //viewflightinformation.FromAirportCity = entity.FromAirportCity;
            // viewflightinformation.ToAirportCity = entity.ToAirportCity;
            viewflightinformation.AircraftType = entity.AircraftType;
            viewflightinformation.Register = entity.Register;
            viewflightinformation.MSN = entity.MSN;
            viewflightinformation.FlightStatus = entity.FlightStatus;
            viewflightinformation.FlightStatusBgColor = entity.FlightStatusBgColor;
            viewflightinformation.FlightStatusColor = entity.FlightStatusColor;
            viewflightinformation.FlightStatusClass = entity.FlightStatusClass;
            viewflightinformation.from = entity.from;
            viewflightinformation.to = entity.to;
            viewflightinformation.notes = entity.notes;
            viewflightinformation.status = entity.status;
            viewflightinformation.progress = entity.progress;
            viewflightinformation.taskName = entity.taskName;
            viewflightinformation.startDate = entity.startDate == null ? null : (Nullable<DateTime>)((DateTime)entity.startDate).AddMinutes(tzoffset3);
            viewflightinformation.duration = entity.duration;
            viewflightinformation.taskId = entity.taskId;
            viewflightinformation.taskID = entity.taskId;
            viewflightinformation.FlightGroupID = entity.FlightGroupID;
            viewflightinformation.PlanId = entity.PlanId;
            viewflightinformation.ManufacturerId = entity.ManufacturerId;
            viewflightinformation.Manufacturer = entity.Manufacturer;
            //viewflightinformation.ToCountry = entity.ToCountry;
            //viewflightinformation.ToSortName = entity.ToSortName;
            //viewflightinformation.ToCity = entity.ToCity;
            viewflightinformation.FromSortName = entity.FromSortName;
            viewflightinformation.FromContry = entity.FromContry;
            //viewflightinformation.FromCity = entity.FromCity;
            viewflightinformation.FromLatitude = entity.FromLatitude;
            viewflightinformation.FromLongitude = entity.FromLongitude;
            viewflightinformation.ToLatitude = entity.ToLatitude;
            viewflightinformation.ToLongitude = entity.ToLongitude;
            viewflightinformation.CargoCount = entity.CargoCount;
            viewflightinformation.BaggageWeight = entity.BaggageWeight;
            viewflightinformation.FuelUnitID = entity.FuelUnitID;
            viewflightinformation.ArrivalRemark = entity.ArrivalRemark;
            viewflightinformation.DepartureRemark = entity.DepartureRemark;
            viewflightinformation.TotalSeat = entity.TotalSeat;
            viewflightinformation.EstimatedDelay = entity.EstimatedDelay;
            viewflightinformation.PaxOver = entity.PaxOver;
            viewflightinformation.TotalPax = entity.TotalPax;
            viewflightinformation.NightTime = entity.NightTime;


            viewflightinformation.FuelUnit = entity.FuelUnit;
            viewflightinformation.DateStatus = entity.DateStatus == null ? null : (Nullable<DateTime>)((DateTime)entity.DateStatus).AddMinutes(tzoffset3);
            viewflightinformation.FlightStatusUserId = entity.FlightStatusUserId;

            viewflightinformation.CancelDate = entity.CancelDate == null ? null : (Nullable<DateTime>)((DateTime)entity.CancelDate).AddMinutes(tzoffset3);
            viewflightinformation.CancelReasonId = entity.CancelReasonId;
            viewflightinformation.CancelReason = entity.CancelReason;
            viewflightinformation.CancelRemark = entity.CancelRemark;



            viewflightinformation.RampReasonId = entity.RampReasonId;
            viewflightinformation.RampReason = entity.RampReason;
            viewflightinformation.RampRemark = entity.RampRemark;

            viewflightinformation.RedirectDate = entity.RedirectDate == null ? null : (Nullable<DateTime>)((DateTime)entity.RedirectDate).AddMinutes(tzoffset3); ;
            viewflightinformation.RedirectReasonId = entity.RedirectReasonId;
            viewflightinformation.RedirectReason = entity.RedirectReason;
            viewflightinformation.RedirectRemark = entity.RedirectRemark;
            viewflightinformation.OSTA = entity.OSTA;
            viewflightinformation.OToAirportIATA = entity.OToAirportIATA;
            viewflightinformation.OToAirportId = entity.OToAirportId;

            viewflightinformation.BaseIATA = entity.BaseIATA;
            viewflightinformation.BaseId = entity.BaseId;
            viewflightinformation.BaseName = entity.BaseName;

            viewflightinformation.FPFlightHH = entity.FPFlightHH;
            viewflightinformation.FPFlightMM = entity.FPFlightMM;
            viewflightinformation.Defuel = entity.Defuel;
            viewflightinformation.FPFuel = entity.FPFuel;

            viewflightinformation.SplitDuty = entity.SplitDuty;
            viewflightinformation.MaleFemalError = entity.MaleFemalError == null ? 0 : (int)entity.MaleFemalError;
            viewflightinformation.MatchingListError = entity.MatchingListError == null ? 0 : (int)entity.MatchingListError;
            viewflightinformation.LinkedFlight = entity.LinkedFlight;
            viewflightinformation.LinkedFlightNumber = entity.LinkedFlightNumber;
            viewflightinformation.UsedFuel = entity.UsedFuel;
            viewflightinformation.JLBLHH = entity.JLBLHH;
            viewflightinformation.JLBLMM = entity.JLBLMM;
            viewflightinformation.PFLR = entity.PFLR;

            viewflightinformation.ChrAdult = entity.ChrAdult;
            viewflightinformation.ChrChild = entity.ChrChild;
            viewflightinformation.ChrInfant = entity.ChrInfant;
            viewflightinformation.ChrCapacity = entity.ChrCapacity;
            viewflightinformation.ChrCode = entity.ChrCode;
            viewflightinformation.ChrTitle = entity.ChrTitle;

            viewflightinformation.DefaultChrId = entity.DefaultChrId;
            viewflightinformation.FuelDensity = entity.FuelDensity;
            viewflightinformation.LTR = entity.LTR != null ? entity.LTR.ToString() : null; //string.IsNullOrEmpty(entity.LTR)?null: entity.LTR.ToString();
            viewflightinformation.SerialNo = entity.SerialNo;


            viewflightinformation.Ready = entity.Ready;
            viewflightinformation.Start = entity.Start;
            viewflightinformation.CargoPieces = entity.CargoPieces;
            viewflightinformation.CargoCost = entity.CargoCost;
            viewflightinformation.FreeAWBCount = entity.FreeAWBCount;
            viewflightinformation.FreeAWBPieces = entity.FreeAWBPieces;
            viewflightinformation.FreeAWBWeight = entity.FreeAWBWeight;
            viewflightinformation.NoShowCount = entity.NoShowCount;
            viewflightinformation.NoShowPieces = entity.NoShowPieces;
            viewflightinformation.NoGoCount = entity.NoGoCount;
            viewflightinformation.NoGoPieces = entity.NoGoPieces;
            viewflightinformation.DSBreakfast = entity.DSBreakfast;
            viewflightinformation.DSWarmFood = entity.DSWarmFood;
            viewflightinformation.DSColdFood = entity.DSColdFood;
            viewflightinformation.DSRefreshment = entity.DSRefreshment;
            viewflightinformation.YClass = entity.YClass;
            viewflightinformation.CClass = entity.CClass;
            viewflightinformation.PaxAdult50 = entity.PaxAdult50;
            viewflightinformation.PaxChild50 = entity.PaxChild50;
            viewflightinformation.PaxInfant50 = entity.PaxInfant50;
            viewflightinformation.PaxAdult100 = entity.PaxAdult100;
            viewflightinformation.PaxChild100 = entity.PaxChild100;
            viewflightinformation.PaxInfant100 = entity.PaxInfant100;
            viewflightinformation.PaxVIP = entity.PaxVIP;
            viewflightinformation.PaxCIP = entity.PaxCIP;
            viewflightinformation.PaxHUM = entity.PaxHUM;
            viewflightinformation.PaxUM = entity.PaxUM;
            viewflightinformation.PaxAVI = entity.PaxAVI;
            viewflightinformation.PaxWCHR = entity.PaxWCHR;
            viewflightinformation.PaxSTRC = entity.PaxSTRC;
            viewflightinformation.PaxPIRLost = entity.PaxPIRLost;
            viewflightinformation.PaxPIRDamage = entity.PaxPIRDamage;
            viewflightinformation.PaxPIRFound = entity.PaxPIRFound;
            viewflightinformation.CargoPIRLost = entity.CargoPIRLost;
            viewflightinformation.CargoPIRDamage = entity.CargoPIRDamage;
            viewflightinformation.CargoPIRFound = entity.CargoPIRFound;
            viewflightinformation.LimitTag = entity.LimitTag;
            viewflightinformation.RushTag = entity.RushTag;
            viewflightinformation.CLCheckIn = entity.CLCheckIn;
            viewflightinformation.CLPark = entity.CLPark;
            viewflightinformation.CLAddTools = entity.CLAddTools;
            viewflightinformation.CLBusReady = entity.CLBusReady;
            viewflightinformation.CLPaxOut = entity.CLPaxOut;
            viewflightinformation.CLDepoOut = entity.CLDepoOut;
            viewflightinformation.CLServicePresence = entity.CLServicePresence;
            viewflightinformation.CLCleaningStart = entity.CLCleaningStart;
            viewflightinformation.CLTechReady = entity.CLTechReady;
            viewflightinformation.CLBagSent = entity.CLBagSent;
            viewflightinformation.CLCateringLoad = entity.CLCateringLoad;
            viewflightinformation.CLFuelStart = entity.CLFuelStart;
            viewflightinformation.CLFuelEnd = entity.CLFuelEnd;
            viewflightinformation.CLCleaningEnd = entity.CLCleaningEnd;
            viewflightinformation.CLBoardingStart = entity.CLBoardingStart;
            viewflightinformation.CLBoardingEnd = entity.CLBoardingEnd;
            viewflightinformation.CLLoadSheetStart = entity.CLLoadSheetStart;
            viewflightinformation.CLGateClosed = entity.CLGateClosed;
            viewflightinformation.CLTrafficCrew = entity.CLTrafficCrew;
            viewflightinformation.CLLoadCrew = entity.CLLoadCrew;
            viewflightinformation.CLForbiddenObj = entity.CLForbiddenObj;
            viewflightinformation.CLLoadSheetSign = entity.CLLoadSheetSign;
            viewflightinformation.CLLoadingEnd = entity.CLLoadingEnd;
            viewflightinformation.CLDoorClosed = entity.CLDoorClosed;
            viewflightinformation.CLEqDC = entity.CLEqDC;
            viewflightinformation.CLMotorStart = entity.CLMotorStart;
            viewflightinformation.CLMovingStart = entity.CLMovingStart;
            viewflightinformation.CLACStart = entity.CLACStart;
            viewflightinformation.CLACEnd = entity.CLACEnd;
            viewflightinformation.CLGPUStart = entity.CLGPUStart;
            viewflightinformation.CLGPUEnd = entity.CLGPUEnd;
            viewflightinformation.CLDepStairs = entity.CLDepStairs;
            viewflightinformation.CLDepGPU = entity.CLDepGPU;
            viewflightinformation.CLDepCrewCar = entity.CLDepCrewCar;
            viewflightinformation.CLDepCrewCarCount = entity.CLDepCrewCarCount;
            viewflightinformation.CLDepCabinService = entity.CLDepCabinService;
            viewflightinformation.CLDepCateringCar = entity.CLDepCateringCar;
            viewflightinformation.CLDepPatientCar = entity.CLDepPatientCar;
            viewflightinformation.CLDepPaxCar = entity.CLDepPaxCar;
            viewflightinformation.CLDepPaxCarCount = entity.CLDepPaxCarCount;
            viewflightinformation.CLDepPushback = entity.CLDepPushback;
            viewflightinformation.CLDepWaterService = entity.CLDepWaterService;
            viewflightinformation.CLDepAC = entity.CLDepAC;
            viewflightinformation.CLDepDeIce = entity.CLDepDeIce;
            viewflightinformation.CLDepEqRemark = entity.CLDepEqRemark;
            viewflightinformation.CLArrStairs = entity.CLArrStairs;
            viewflightinformation.CLArrGPU = entity.CLArrGPU;
            viewflightinformation.CLArrCrewCar = entity.CLArrCrewCar;
            viewflightinformation.CLArrCrewCarCount = entity.CLArrCrewCarCount;
            viewflightinformation.CLArrCabinService = entity.CLArrCabinService;
            viewflightinformation.CLArrPatientCar = entity.CLArrPatientCar;
            viewflightinformation.CLArrPaxCar = entity.CLArrPaxCar;
            viewflightinformation.CLArrPaxCarCount = entity.CLArrPaxCarCount;
            viewflightinformation.CLArrToiletService = entity.CLArrToiletService;
            viewflightinformation.CLArrEqRemark = entity.CLArrEqRemark;

            viewflightinformation.WLCost = entity.WLCost;
            viewflightinformation.WLCount = entity.WLCount;
            viewflightinformation.ExBagCost = entity.ExBagCost;
            viewflightinformation.ExBagWeight = entity.ExBagWeight;

            viewflightinformation.TotalTrafficLoad = entity.TotalTrafficLoad;


        }

        public static ViewFlightsGanttDto GetDto(Models.ViewFlightsGantt entity, int tzoffset)
        {
            ViewModels.ViewFlightsGanttDto viewflightinformation = new ViewFlightsGanttDto();
            viewflightinformation.resourceId = new List<int>();
            viewflightinformation.ID = entity.ID;
            viewflightinformation.FlightPlanId = entity.FlightPlanId;
            viewflightinformation.BaggageCount = entity.BaggageCount;
            viewflightinformation.CargoUnitID = entity.CargoUnitID;
            viewflightinformation.CargoUnit = entity.CargoUnit;
            viewflightinformation.CargoWeight = entity.CargoWeight;
            viewflightinformation.PaxChild = entity.PaxChild;
            viewflightinformation.PaxInfant = entity.PaxInfant;
            viewflightinformation.PaxAdult = entity.PaxAdult;
            viewflightinformation.FuelArrival = entity.FuelArrival;
            viewflightinformation.FuelDeparture = entity.FuelDeparture;
            viewflightinformation.FuelActual = entity.FuelActual;
            viewflightinformation.FuelPlanned = entity.FuelPlanned;
            viewflightinformation.GWLand = entity.GWLand;
            viewflightinformation.GWTO = entity.GWTO;
            viewflightinformation.BlockM = entity.BlockM;
            viewflightinformation.BlockH = entity.BlockH;
            viewflightinformation.FlightH = entity.FlightH;
            viewflightinformation.FlightM = entity.FlightM;
            viewflightinformation.ChocksIn = entity.ChocksIn == null ? null : (Nullable<DateTime>)((DateTime)entity.ChocksIn).AddMinutes(tzoffset);
            viewflightinformation.Landing = entity.Landing == null ? null : (Nullable<DateTime>)((DateTime)entity.Landing).AddMinutes(tzoffset); ;
            viewflightinformation.Takeoff = entity.Takeoff == null ? null : (Nullable<DateTime>)((DateTime)entity.Takeoff).AddMinutes(tzoffset);
            viewflightinformation.ChocksOut = entity.ChocksOut == null ? null : (Nullable<DateTime>)((DateTime)entity.ChocksOut).AddMinutes(tzoffset);
            viewflightinformation.STD = entity.STD == null ? null : (Nullable<DateTime>)((DateTime)entity.STD).AddMinutes(tzoffset);
            viewflightinformation.STA = entity.STA == null ? null : (Nullable<DateTime>)((DateTime)entity.STA).AddMinutes(tzoffset);
            viewflightinformation.FlightStatusID = (int)entity.FlightStatusID;
            viewflightinformation.RegisterID = entity.RegisterID;
            viewflightinformation.FlightTypeID = entity.FlightTypeID;
            viewflightinformation.TypeId = entity.TypeId;
            viewflightinformation.AirlineOperatorsID = entity.AirlineOperatorsID;
            viewflightinformation.FlightNumber = entity.FlightNumber;
            viewflightinformation.FromAirport = entity.FromAirport;
            viewflightinformation.ToAirport = entity.ToAirport;
            viewflightinformation.STAPlanned = entity.STAPlanned == null ? null : (Nullable<DateTime>)((DateTime)entity.STAPlanned).AddMinutes(tzoffset);
            viewflightinformation.STDPlanned = entity.STDPlanned == null ? null : (Nullable<DateTime>)((DateTime)entity.STDPlanned).AddMinutes(tzoffset);
            viewflightinformation.FlightHPlanned = entity.FlightHPlanned;
            viewflightinformation.FlightMPlanned = entity.FlightMPlanned;
            viewflightinformation.FlightPlan = entity.FlightPlan;
            viewflightinformation.CustomerId = entity.CustomerId;
            viewflightinformation.IsActive = entity.IsActive;
            viewflightinformation.DateActive = entity.DateActive;
            viewflightinformation.FromAirportName = entity.FromAirportName;
            viewflightinformation.FromAirportIATA = entity.FromAirportIATA;
            // viewflightinformation.FromAirportCityId = entity.FromAirportCityId;
            viewflightinformation.ToAirportName = entity.ToAirportName;
            viewflightinformation.ToAirportIATA = entity.ToAirportIATA;
            // viewflightinformation.ToAirportCityId = entity.ToAirportCityId;
            // viewflightinformation.FromAirportCity = entity.FromAirportCity;
            // viewflightinformation.ToAirportCity = entity.ToAirportCity;
            viewflightinformation.AircraftType = entity.AircraftType;
            viewflightinformation.Register = entity.Register;
            viewflightinformation.MSN = entity.MSN;
            viewflightinformation.FlightStatus = entity.FlightStatus;
            viewflightinformation.FlightStatusBgColor = entity.FlightStatusBgColor;
            viewflightinformation.FlightStatusColor = entity.FlightStatusColor;
            viewflightinformation.FlightStatusClass = entity.FlightStatusClass;
            viewflightinformation.from = entity.from;
            viewflightinformation.to = entity.to;
            viewflightinformation.notes = entity.notes;
            viewflightinformation.status = entity.status;
            viewflightinformation.progress = entity.progress;
            viewflightinformation.taskName = entity.taskName;
            viewflightinformation.startDate = entity.startDate == null ? null : (Nullable<DateTime>)((DateTime)entity.startDate).AddMinutes(tzoffset);
            viewflightinformation.duration = entity.duration;
            viewflightinformation.taskId = entity.taskId;
            viewflightinformation.FlightGroupID = entity.FlightGroupID;
            viewflightinformation.PlanId = entity.PlanId;
            viewflightinformation.ManufacturerId = entity.ManufacturerId;
            viewflightinformation.Manufacturer = entity.Manufacturer;
            //viewflightinformation.ToCountry = entity.ToCountry;
            //viewflightinformation.ToSortName = entity.ToSortName;
            //viewflightinformation.ToCity = entity.ToCity;
            viewflightinformation.FromSortName = entity.FromSortName;
            viewflightinformation.FromContry = entity.FromContry;
            //viewflightinformation.FromCity = entity.FromCity;
            viewflightinformation.FromLatitude = entity.FromLatitude;
            viewflightinformation.FromLongitude = entity.FromLongitude;
            viewflightinformation.ToLatitude = entity.ToLatitude;
            viewflightinformation.ToLongitude = entity.ToLongitude;
            viewflightinformation.CargoCount = entity.CargoCount;
            viewflightinformation.BaggageWeight = entity.BaggageWeight;
            viewflightinformation.FuelUnitID = entity.FuelUnitID;
            viewflightinformation.ArrivalRemark = entity.ArrivalRemark;
            viewflightinformation.DepartureRemark = entity.DepartureRemark;
            viewflightinformation.TotalSeat = entity.TotalSeat;
            viewflightinformation.EstimatedDelay = entity.EstimatedDelay;
            viewflightinformation.PaxOver = entity.PaxOver;
            viewflightinformation.TotalPax = entity.TotalPax;
            viewflightinformation.FuelUnit = entity.FuelUnit;
            viewflightinformation.DateStatus = entity.DateStatus == null ? null : (Nullable<DateTime>)((DateTime)entity.DateStatus).AddMinutes(tzoffset);
            viewflightinformation.FlightStatusUserId = entity.FlightStatusUserId;
            viewflightinformation.CancelDate = entity.CancelDate;
            viewflightinformation.CancelReasonId = entity.CancelReasonId;
            viewflightinformation.CancelReason = entity.CancelReason;
            viewflightinformation.CancelRemark = entity.CancelRemark;



            viewflightinformation.RampDate = entity.RampDate == null ? null : (Nullable<DateTime>)((DateTime)entity.RampDate).AddMinutes(tzoffset);
            viewflightinformation.RampReasonId = entity.RampReasonId;
            viewflightinformation.RampReason = entity.RampReason;
            viewflightinformation.RampRemark = entity.RampRemark;

            viewflightinformation.RedirectDate = entity.RedirectDate;
            viewflightinformation.RedirectReasonId = entity.RedirectReasonId;
            viewflightinformation.RedirectReason = entity.RedirectReason;
            viewflightinformation.RedirectRemark = entity.RedirectRemark;
            viewflightinformation.OSTA = entity.OSTA;
            viewflightinformation.OToAirportIATA = entity.OToAirportIATA;
            viewflightinformation.OToAirportId = entity.OToAirportId;

            return viewflightinformation;
        }
    }


    public class CustomActionResult : IHttpActionResult
    {

        private System.Net.HttpStatusCode statusCode;

        public object data;
        public System.Net.HttpStatusCode Code { get { return statusCode; } }
        public CustomActionResult(System.Net.HttpStatusCode statusCode, object data)
        {

            this.statusCode = statusCode;

            this.data = data;

        }
        public HttpResponseMessage CreateResponse(System.Net.HttpStatusCode statusCode, object data)
        {

            HttpRequestMessage request = new HttpRequestMessage();
            request.Properties.Add(System.Web.Http.Hosting.HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

            HttpResponseMessage response = request.CreateResponse(statusCode, data);

            return response;

        }
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(CreateResponse(this.statusCode, data));
        }

    }


    public class updateLogResult
    {
        public bool sendNira { get; set; }
        public int flight { get; set; }
        public List<int> offIds { get; set; }
        public List<int> fltIds { get; set; }
        public List<offcrew> offcrews { get; set; }
    }

    public class offcrew
    {
        public int? flightId { get; set; }
        public List<int?> crews { get; set; }
    }

    public class BaseSummary
    {
        public int? BaseId { get; set; }
        public string BaseIATA { get; set; }
        public string BaseName { get; set; }
        public int Total { get; set; }
        public int TakeOff { get; set; }
        public int Landing { get; set; }
        public int Canceled { get; set; }
        public int Redirected { get; set; }
        public int Diverted { get; set; }
        public int? TotalDelays { get; set; }
        public int? DepartedPax { get; set; }
        public int? ArrivedPax { get; set; }
    }

}