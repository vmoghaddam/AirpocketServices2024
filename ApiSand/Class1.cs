using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiSand.Test
{
    public partial class Message
    {
        public string Subject { get; set; }
        public string Text { get; set; }
        public string SentFrom { get; set; }
        public DateTime SentTime { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public bool HighPriority { get; set; }
        public MessageRecipient[] Recipients { get; set; }
    }

    public partial class MessageRecipient
    {
        public string RecipientType { get; set; }
        public string Recipient { get; set; }
    }

    public partial class OverflightCost
    {
        public FIROverflightCost[] Cost { get; set; }
        public string Currency { get; set; }
        public int TotalOverflightCost { get; set; }
        public int TotalTerminalCost { get; set; }
    }

    public partial class FIROverflightCost
    {
        public string FIR { get; set; }
        public int Distance { get; set; }
        public int Cost { get; set; }
    }

    public partial class LocalTimeDeparture
    {
        public DateTime STD { get; set; }
        public DateTime ETD { get; set; }
        public DateTime Sunrise { get; set; }
        public DateTime Sunset { get; set; }
    }

    public partial class LocalTimeDestination
    {
        public DateTime STA { get; set; }
        public DateTime ETA { get; set; }
        public DateTime Sunrise { get; set; }
        public DateTime Sunset { get; set; }
    }

    public partial class LocalTime
    {
        public LocalTimeDeparture Departure { get; set; }
        public LocalTimeDestination Destination { get; set; }
    }

    public class HoldingFuel
    {
        public int Fuel { get; set; }
        public int Minutes { get; set; }
        public string Profile { get; set; }
        public string Specification { get; set; }
        public string FuelFlowType { get; set; }
    }
    public class Taf
    {
        public TafType Type { get; set; }
        public string Text { get; set; }
        public string ICAO { get; set; }
        public DateTime ForecastTime { get; set; }
        public DateTime ForecastStartTime { get; set; }
        public DateTime ForecastEndTime { get; set; }
    }

    public enum TafType
    {
        FT,
        FC
    }

    public class Notam
    {
        public string Number { get; set; }
        public string Text { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int FromLevel { get; set; }
        public int ToLevel { get; set; }
        public string Fir { get; set; }
        public string QCode { get; set; }
        public string ECode { get; set; }
        public string ICAO { get; set; }
        public string UniformAbbreviation { get; set; }
        public int Year { get; set; }
        public NotamPartInformation PartInformation { get; set; }
        public RouteSegment RoutePart { get; set; }
        public string Provider { get; set; }
    }
    public class NotamPartInformation
    {
        public int Part { get; set; }        // شماره قسمت جاری
        public int TotalParts { get; set; }  // تعداد کل قسمت‌ها
    }
    public enum RouteSegment
    {
        Cruise,          // مسیر در فاز کروز
        ClimbOrDescent,  // مسیر در فاز اوج‌گیری یا نزول
        EntireRoute,     // کل مسیر
        None             // بدون segment خاص
    }
    public class RoutePoint
    {
        public int ID { get; set; }
        public string IDENT { get; set; }
        public int FL { get; set; }
        public int Wind { get; set; }
        public int Vol { get; set; }
        public int ISA { get; set; }
        public int LegTime { get; set; }
        public double LegCourse { get; set; }
        public int LegDistance { get; set; }
        public int LegCAT { get; set; }
        public string LegName { get; set; }
        public string LegAWY { get; set; }
        public double FuelUsed { get; set; }
        public int FuelFlow { get; set; }
        public double LAT { get; set; }
        public double LON { get; set; }
        public int VARIATION { get; set; }
        public int ACCDIST { get; set; }
        public int ACCTIME { get; set; }
        public double MagCourse { get; set; }
        public int TrueAirSpeed { get; set; }
        public int GroundSpeed { get; set; }
        public double FuelRemaining { get; set; }
        public int DistRemaining { get; set; }
        public int TimeRemaining { get; set; }
        public double MinReqFuel { get; set; }
        public double FuelFlowPerEng { get; set; }
        public int Temperature { get; set; }
        public int MORA { get; set; }
        public double Frequency { get; set; }
        public int WindComponent { get; set; }
        public int MinimumEnrouteAltitude { get; set; }
        public EcoInfo Eco { get; set; }
        public double MagneticHeading { get; set; }
        public double TrueHeading { get; set; }
        public int MagneticTrack { get; set; }
        public int TrueTrack { get; set; }
        public string HLAEntryExit { get; set; }
        public string FIR { get; set; }
        public FlightLevelWind[] FlightLevelWinds { get; set; }
        public string ClimbDescent { get; set; }
        public double LegFuel { get; set; }
    }

    public class EcoInfo
    {
        public int OptSpeedFL { get; set; }  // ارتفاع بهینه برای حداکثر سرعت
        public int SpeedGain { get; set; }  // افزایش سرعت در حالت بهینه
        public int OptEcoFL { get; set; }  // ارتفاع بهینه برای صرفه‌جویی سوخت
        public int MoneyGain { get; set; }  // صرفه‌جویی اقتصادی
        public int OptFuelFL { get; set; }  // ارتفاع بهینه برای مصرف سوخت
        public double FuelGain { get; set; }  // صرفه‌جویی سوختی
    }

    public class FlightLevelWind
    {
        public int FlightLevel { get; set; }  // سطح پروازی
        public int Wind { get; set; }  // جهت باد
        public int Velocity { get; set; }  // سرعت باد
        public int Temp { get; set; }  // دمای هوا
        public int Shear { get; set; }  // تغییر ناگهانی سرعت یا جهت باد
    }
    public class Crew
    {
        public string ID { get; set; }
        public string CrewType { get; set; }
        public string CrewName { get; set; }
        public string Initials { get; set; }
        public string GSM { get; set; }
        public float? Mass { get; set; }
    }
    public class Response
    {
        public string Message { get; set; }
        public bool Succeed { get; set; }
    }
    public class ATC
    {
        public string ATCID { get; set; }
        public string ATCTOA { get; set; }
        public string ATCRule { get; set; }
        public string ATCType { get; set; }
        public string ATCNum { get; set; }
        public string ATCWake { get; set; }
        public string ATCEqui { get; set; }
        public string ATCSSR { get; set; }
        public string ATCDep { get; set; }
        public string ATCTime { get; set; }
        public string ATCSpeed { get; set; }
        public string ATCFL { get; set; }
        public string ATCRoute { get; set; }
        public string ATCDest { get; set; }
        public string ATCEET { get; set; }
        public string ATCAlt1 { get; set; }
        public string ATCAlt2 { get; set; }
        public string ATCInfo { get; set; }
        public string ATCEndu { get; set; }
        public string ATCPers { get; set; }
        public string ATCRadi { get; set; }
        public string ATCSurv { get; set; }
        public string ATCJack { get; set; }
        public string ATCDing { get; set; }
        public string ATCCap { get; set; }
        public string ATCCover { get; set; }
        public string ATCColo { get; set; }
        public string ATCAcco { get; set; }
        public string ATCRem { get; set; }
        public string ATCPIC { get; set; }
        public string ATCCtot { get; set; }
    }
    public class NextLeg
    {
        public DateTime STD { get; set; }  // زمان برنامه‌ریزی شده پرواز
        public string DEP { get; set; }  // فرودگاه مبداء
        public string DEST { get; set; }  // فرودگاه مقصد
        public int MINFUEL { get; set; }  // حداقل سوخت مورد نیاز
    }
    public class FlightLevel
    {
        public int Level { get; set; }           // سطح پروازی
        public int Cost { get; set; }            // هزینه مربوطه
        public int WC { get; set; }              // Wind Correction
        public double TimeNCruise { get; set; }  // زمان در حالت نرمال کروز
        public int FuelNCruise { get; set; }     // مصرف سوخت در حالت نرمال کروز
        public double TimeProfile2 { get; set; } // زمان در پروفایل ۲
        public int FuelProfile2 { get; set; }    // مصرف سوخت در پروفایل ۲
        public double TimeProfile3 { get; set; } // زمان در پروفایل ۳
        public int FuelProfile3 { get; set; }    // مصرف سوخت در پروفایل ۳
        public int FuelLower { get; set; }       // مصرف سوخت در پایین‌ترین حالت
        public int CostDiff { get; set; }        // اختلاف هزینه
    }
    public class AltAirport
    {
        public string Type { get; set; }
        public string Icao { get; set; }
        public int Dist { get; set; }
        public int Time { get; set; }
        public int Fuel { get; set; }
        public int MAGCURS { get; set; }
        public string ATC { get; set; }
        public double Lat { get; set; }
        public double Long { get; set; }
        public int Rwyl { get; set; }
        public int Elevation { get; set; }
        public string Name { get; set; }
        public string Iata { get; set; }
        public string Category { get; set; }
        public Frequency[] Frequencies { get; set; }
        public AirportHours AirportHours { get; set; }
        public Frequency2[] Frequencies2 { get; set; }
    }
    public class Frequency
    {
        public double Value { get; set; }
        public string Information { get; set; }
    }
    public class Frequency2
    {
        public string Value { get; set; }
        public string Information { get; set; }
    }

    public class AirportHours
    {
        public string Text { get; set; }
    }
    public class AlternateAirport
    {
        public string ICAO { get; set; }
        public string IATA { get; set; }
        public string Name { get; set; }
        public int Wind { get; set; }
        public int WindVelocity { get; set; }
        public int FlightLevel { get; set; }
        public int Distance { get; set; }
        public double Course { get; set; }
        public int Time { get; set; }
        public double Fuel { get; set; }
        public int BlockTime { get; set; }
        public double BlockFuel { get; set; }
        public string ERA { get; set; }
        public string Route { get; set; }
    }
    public class RCFData
    {
        public string RCFAirport { get; set; }
        public string RCFAlternate { get; set; }
        public string DecisionPoint { get; set; }
        public string ATCRoute { get; set; }
        public int MinReqFuel { get; set; }
        public int TripFuel { get; set; }
        public int AlternateFuel { get; set; }
        public int ExtraFuel { get; set; }
        public int HoldingFuel { get; set; }
        public int ContFuel { get; set; }
        public RoutePoint[] RCFRoutePoints { get; set; }
        public RoutePoint[] RCFAltRoutePoints { get; set; }
        public string RCFERAAirport { get; set; }
        public ContingencySavingData ContingencySavingAirports { get; set; }
    }
    public class ContingencySavingData
    {
        public AirportWeatherData ContingencySavingAirport { get; set; }
        public AirportWeatherData ContingencySavingAlternate { get; set; }
        public AirportWeatherData ContingencySavingEnRouteAlternateAirport { get; set; }
    }
    public class AirportWeatherData
    {
        public string ICAO { get; set; }
        public Taf TAF { get; set; }
        public Metar Metar { get; set; }
    }
    public class Metar
    {
        public string Text { get; set; }
        public string ICAO { get; set; }
        public DateTime ObservationTime { get; set; }
        public AirportWeatherObservationType ObservationType { get; set; }
    }

    public enum AirportWeatherObservationType
    {
        METAR,
        SPECI
    }
    public class RouteStrings
    {
        public string ToDest { get; set; }
        public string ToAlt1 { get; set; }
        public string ToAlt2 { get; set; }
        public string ToAlt { get; set; }
    }
    public class SID
    {
        public string RunwayName { get; set; }
        public string ProcedureName { get; set; }
        public int Distance { get; set; }
    }
    public class MelItem
    {
        public string Identifier { get; set; }
        public string Remark { get; set; }
        public string[] Limitations { get; set; }
    }
    public class PassThrough
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
    public class FreeText
    {
        public int Numbering { get; set; }
        public string Value { get; set; }
    }
    

public class EtopsInformation
    {
        public int RuleTimeUsed { get; set; }
        public int IcingPercentage { get; set; }
        public EntryPoint[] EntryPoints { get; set; }
        public ExitPoint[] ExitPoints { get; set; }
        public EqualTimePoint[] EqualTimePoints { get; set; }
        public EtopsAlternate[] Alternates { get; set; }
        public EtopsLimit[] EtopsLimits { get; set; }
        public EtopsProfile[] EtopsProfiles { get; set; }
        public AirportWeatherData[] ETOPSSuitableAirports { get; set; }
    }
    public class EtopsProfile
    {
        public int ProfileIndex { get; set; }
        public string ProfileName { get; set; }
        public FullEqualTimePoint[] EqualTimePoints { get; set; }
    }
    public class EntryPoint
    {
        public Position Position { get; set; }
    }
    public class ExitPoint
    {
        public Position Position { get; set; }
    }
    public class Position
    {
        public double Lat { get; set; }
        public double Lon { get; set; }
    }
    public class EqualTimePoint
    {
        public Position Position { get; set; }
        public double Time { get; set; }
        public EqualTimeAirport[] EqualTimeAirports { get; set; }
    }
    public class FullEqualTimePoint
    {
        public Position Position { get; set; }
        public int Mass { get; set; }
        public int MassAtETP { get; set; }
        public int TimeDESTToETP { get; set; }
        public int DistanceDESTToETP { get; set; }
        public int TimeETPToAlternate { get; set; }
        public int ActualFuel { get; set; }
        public int MinimumRequiredFuel { get; set; }
        public int RemainingFuel { get; set; }
        public FullEqualTimeAirport[] Airports { get; set; }
    }
    public class FullEqualTimeAirport
    {
        public string ICAO { get; set; }
        public int Fuel { get; set; }
        public int Distance { get; set; }
        public int Track { get; set; }
        public int Wind { get; set; }
        public int Velocity { get; set; }
        public int Temp { get; set; }
        public int Tas { get; set; }
        public int Gs { get; set; }
        public int FlightLevel { get; set; }
        public int Mora { get; set; }
        public int FuelReserve { get; set; }
        public AirportHours AirportHours { get; set; }
    }
    public class EqualTimeAirport
    {
        public string ICAO { get; set; }
        public double Distance { get; set; }
        public AirportHours AirportHours { get; set; }
    }
    public class EtopsLimit
    {
        public int RuleTime { get; set; }
        public int MaxDistance { get; set; }
    }
    public class EtopsAlternate
    {
        public string ICAO { get; set; }
        public Position Position { get; set; }
        public AirportHours AirportHours { get; set; }
    }
    public class CorrectionTable
    {
        public int CtID { get; set; }
        public int Flightlevel { get; set; }
        public int WindCorrection { get; set; }
        public double TimeInMinutesForCruiseProfile { get; set; }
        public double TimeInHoursMinutesForCruiseProfile { get; set; }
        public TimeSpan TimeInHoursMinutesForAltCruiseProfile { get; set; }
        public TimeSpan TimeInMinutesForAltCruiseProfile { get; set; }
        public double TimeInMinutesForXProfile { get; set; }
        public TimeSpan TimeInHoursMinutesForXProfile { get; set; }
        public double FuelForSelectedProfile { get; set; }
        public double FuelForSecondProfile { get; set; }
        public double FuelForXProfile { get; set; }
        public double DifferentialCost { get; set; }
        public double TotalFuelIncreaseWith10ktWind { get; set; }
    }
    public class DepartureAndArrivalProcedures
    {
        public SidStarInfo Sid { get; set; }
        public SidStarInfo Star { get; set; }
    }
    public class SidStarInfo
    {
        public string Name { get; set; }
        public string Info { get; set; }
    }
    public class Fuel
    {
        public float ActTotal { get; set; }
        public LoadFuelSection[] LoadFuel { get; set; }
    }
    public class LoadFuelSection
    {
        public float? ActMass { get; set; }
        public string ID { get; set; }
    }
    public class LoadCargoSection
    {
        public float? ActMass { get; set; }
        public string ID { get; set; }
    }
    public class Cargo
    {
        public float ActTotal { get; set; }
        public LoadCargoSection[] LoadCargo { get; set; }
    }
    public class LoadPaxSection
    {
        public int MaxPax { get; set; }    // حداکثر مسافر
        public int ActPax { get; set; }    // مسافر واقعی
        public float? ActMass { get; set; }    // جرم واقعی (nullable)
        public int PaxAmount { get; set; }    // تعداد کل مسافران
        public int Male { get; set; }    // تعداد مسافران مرد
        public int Female { get; set; }    // تعداد مسافران زن
        public int Children { get; set; }    // تعداد کودکان
        public int Infant { get; set; }    // تعداد نوزادان
        public float? CustMass { get; set; }    // جرم سرنشینان (nullable)
    }
    public class SimplePaxSection
    {
        public string Row { get; set; }
        public int ActPax { get; set; }
        public int ActMass { get; set; }
        public int Male { get; set; }
        public int Female { get; set; }
        public int Children { get; set; }
        public int Infant { get; set; }
        public int? CustMass { get; set; }
    }

    public class Pax
    {
        public float Total { get; set; }
        public LoadPaxSection PaxData { get; set; }
        public SimplePaxSection[] PaxSections { get; set; }
    }
    public class MassBalanceIndex
    {
        public double DryOperatingIndex { get; set; }
        public double ZeroFuelForwardLimit { get; set; }
        public double ZeroFuelWeightIndex { get; set; }
        public double ZeroFuelAftLimit { get; set; }
    }
    public class ArmPosition
    {
        public double ForwardLimit { get; set; }
        public double ActualPosition { get; set; }
        public double AftLimit { get; set; }
    }

    public class MassBalance
    {
        public ArmPosition Takeoff { get; set; }
        public ArmPosition Landing { get; set; }
        public ArmPosition ZeroFuel { get; set; }
        public MassBalanceIndex Index { get; set; }
    }
    public class Load
    {
        public Fuel Fuel { get; set; }
        public Cargo Cargo { get; set; }
        public Pax Pax { get; set; }
        public MassBalance MassBalance { get; set; }
        public Payload Payload { get; set; }
        public DryOperating DryOperating { get; set; }
    }
    public class DryOperating
    {
        public double BasicEmptyArm { get; set; }
        public double BasicEmptyWeight { get; set; }
        public double DryOperatingMassArm { get; set; }
        public double DryOperatingWeight { get; set; }
    }
    public class Payload
    {
        public double MaxPayload { get; set; }
        public double Mzfm { get; set; }
        public double Mtom { get; set; }
        public double Mlm { get; set; }
        public double Mrmp { get; set; }
        public double MaxCargo { get; set; }
    }
    public class AircraftConfiguration
    {
        public string Name { get; set; }
        public Crew[] Crew { get; set; }
    }
    public class PpsVersionInformation
    {
        public string PpsApplicationVersion { get; set; }
        public string PpsExeVersion { get; set; }
    }
    public class CustomReferences
    {
        public string RefID { get; set; }
        public string MilID { get; set; }
    }
    // Flight class for C# versions before 8 (using auto-properties)
    // Simplified POCO without init-only or record types
    public class Flight
    {
        public Message[] Messages { get; set; }
        public OverflightCost OverflightCost { get; set; }
        public string FlightLogID { get; set; }
        public int ID { get; set; }
        public string PPSName { get; set; }
        public string ACFTAIL { get; set; }
        public string DEP { get; set; }
        public string DEST { get; set; }
        public string ALT1 { get; set; }
        public string ALT2 { get; set; }
        public DateTime STD { get; set; }
        public int PAX { get; set; }
        public double FUEL { get; set; }
        public int LOAD { get; set; }
        public string ValidHrs { get; set; }
        public int MinFL { get; set; }
        public int MaxFL { get; set; }
        public string EROPSAltApts { get; set; }
        public string[] AdequateApt { get; set; }
        public string[] FIR { get; set; }
        public string[] AltApts { get; set; }
        public string TOA { get; set; }
        public string FMDID { get; set; }
        public string DESTSTDALT { get; set; }
        public string FUELCOMP { get; set; }
        public string TIMECOMP { get; set; }
        public string FUELCONT { get; set; }
        public string TIMECONT { get; set; }
        public string PCTCONT { get; set; }
        public string FUELMIN { get; set; }
        public string TIMEMIN { get; set; }
        public string FUELTAXI { get; set; }
        public string TIMETAXI { get; set; }
        public string FUELEXTRA { get; set; }
        public string TIMEEXTRA { get; set; }
        public string FUELLDG { get; set; }
        public string TIMELDG { get; set; }
        public string ZFM { get; set; }
        public string GCD { get; set; }
        public string ESAD { get; set; }
        public string GL { get; set; }
        public string FUELBIAS { get; set; }
        public DateTime STA { get; set; }
        public DateTime ETA { get; set; }
        public LocalTime LocalTime { get; set; }
        public int SCHBLOCKTIME { get; set; }
        public string DISP { get; set; }
        public DateTime LastEditDate { get; set; }
        public DateTime LatestFlightPlanDate { get; set; }
        public DateTime LatestDocumentUploadDate { get; set; }
        public string FUELMINTO { get; set; }
        public string TIMEMINTO { get; set; }
        public string ARAMP { get; set; }
        public string TIMEACT { get; set; }
        public string FUELACT { get; set; }
        public string DestERA { get; set; }
        public string TrafficLoad { get; set; }
        public string WeightUnit { get; set; }
        public string WindComponent { get; set; }
        public string CustomerDataPPS { get; set; }
        public string CustomerDataScheduled { get; set; }
        public int Fl { get; set; }
        public int RouteDistNM { get; set; }
        public string RouteName { get; set; }
        public string RouteType { get; set; }
        public string RouteRemark { get; set; }
        public int EmptyWeight { get; set; }
        public int TotalDistance { get; set; }
        public int AltDist { get; set; }
        public int DestTime { get; set; }
        public int AltTime { get; set; }
        public int AltFuel { get; set; }
        public int HoldTime { get; set; }
        public int ReserveTime { get; set; }
        public int Cargo { get; set; }
        public double ActTOW { get; set; }
        public double TripFuel { get; set; }
        public double HoldFuel { get; set; }
        public HoldingFuel Holding { get; set; }
        public double Elw { get; set; }
        public string FuelPolicy { get; set; }
        public int Alt2Time { get; set; }
        public int Alt2Fuel { get; set; }
        public double MaxTOM { get; set; }
        public double MaxLM { get; set; }
        public double MaxZFM { get; set; }
        public DateTime WeatherObsTime { get; set; }
        public DateTime WeatherPlanTime { get; set; }
        public string MFCI { get; set; }
        public string CruiseProfile { get; set; }
        public int TempTopOfClimb { get; set; }
        public string Climb { get; set; }
        public string Descend { get; set; }
        public string FuelPL { get; set; }
        public string DescendWind { get; set; }
        public string ClimbProfile { get; set; }
        public string DescendProfile { get; set; }
        public string HoldProfile { get; set; }
        public string StepClimbProfile { get; set; }
        public string FuelContDef { get; set; }
        public string FuelAltDef { get; set; }
        public string AmexsyStatus { get; set; }
        public int AvgTrack { get; set; }
        public Taf DEPTAF { get; set; }
        public string DEPMetar { get; set; }
        public Notam[] DEPNotam { get; set; }
        public Taf DESTTAF { get; set; }
        public string DESTMetar { get; set; }
        public Notam[] DESTNotam { get; set; }
        public Taf ALT1TAF { get; set; }
        public Taf ALT2TAF { get; set; }
        public string ALT1Metar { get; set; }
        public string ALT2Metar { get; set; }
        public Notam[] ALT1Notam { get; set; }
        public Notam[] ALT2Notam { get; set; }
        public RoutePoint[] RoutePoints { get; set; }
        public Crew[] Crews { get; set; }
        public Response Responce { get; set; }
        public ATC ATCData { get; set; }
        public NextLeg NextLeg { get; set; }
        public FlightLevel[] OptFlightLevels { get; set; }
        public Notam[] AdequateNotam { get; set; }
        public Notam[] FIRNotam { get; set; }
        public Notam[] AlternateNotam { get; set; }
        public AltAirport[] Airports { get; set; }
        public string[] EnrouteAlternates { get; set; }
        public RoutePoint[] Alt1Points { get; set; }
        public RoutePoint[] Alt2Points { get; set; }
        public AlternateAirport[] StdAlternates { get; set; }
        public string[] CustomerData { get; set; }
        public RCFData RCFData { get; set; }
        public string TOALT { get; set; }
        public RouteStrings RouteStrings { get; set; }
        public string DEPIATA { get; set; }
        public string DESTIATA { get; set; }
        public SID SIDPlanned { get; set; }
        public SID[] SIDAlternatives { get; set; }
        public int FinalReserveMinutes { get; set; }
        public int FinalReserveFuel { get; set; }
        public int AddFuelMinutes { get; set; }
        public int AddFuel { get; set; }
        public string FlightSummary { get; set; }
        public MelItem[] MelItems { get; set; }
        public PassThrough[] PassThroughValues { get; set; }
        public string CommercialFlightNumber { get; set; }
        public FreeText[] FreeTextItems { get; set; }
        public EtopsInformation EtopsInformation { get; set; }
        public double FuelINCRBurn { get; set; }
        public CorrectionTable[] CorrectionTable { get; set; }
        public string ExternalFlightId { get; set; }
        public Guid GUFI { get; set; }
        public RoutePoint[] PDPPoints { get; set; }
        public DepartureAndArrivalProcedures SidAndStarProcedures { get; set; }
        public double FMRI { get; set; }
        public Load Load { get; set; }
        public AircraftConfiguration AircraftConfiguration { get; set; }
        public bool IsRecalc { get; set; }
        public double MaxRampWeight { get; set; }
        public string UnderloadFactor { get; set; }
        public int AvgISA { get; set; }
        public string HWCorrection20KtsTime { get; set; }
        public double HWCorrection20KtsFuel { get; set; }
        public double Correction1TON { get; set; }
        public double Correction2TON { get; set; }
        public string RcfHeader { get; set; }
        public bool ALT2AsInfoOnly { get; set; }
        public PpsVersionInformation PpsVersionInformation { get; set; }
        public CustomReferences CustomReferences { get; set; }
        public RoutePoint[] ToAlt1Points { get; set; }
        public string CFMUStatus { get; set; }
        public double StructuralTOM { get; set; }
        public string FW1 { get; set; }
        public string FW2 { get; set; }
        public string FW3 { get; set; }
        public string FW4 { get; set; }
        public string FW5 { get; set; }
        public string FW6 { get; set; }
        public string FW7 { get; set; }
        public string FW8 { get; set; }
        public string FW9 { get; set; }
        public double TOTALPAXWEIGHT { get; set; }
        public int Alt2Dist { get; set; }
        public string FMSIdent { get; set; }
        public ExtraFuel[] ExtraFuels { get; set; }
        public string AircraftFuelBias { get; set; }
        public string MelFuelBias { get; set; }
        public AirportWeatherData DepartureAlternateAirport { get; set; }
        public AirportWeatherData EnRouteAlternateAirport { get; set; }
        public AirportWeatherData[] PlanningEnRouteAlternateAirports { get; set; }
    }

    public class ExtraFuel
    {
        public string Type { get; set; }
        public double Fuel { get; set; }
        public string Time { get; set; }
    }

}