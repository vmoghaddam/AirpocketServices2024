using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XAPI
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Alternate1NavLog
    {
        public string WayPoint { get; set; }
        public string FlightLevel { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string Frequency { get; set; }
        public string Airway { get; set; }
        public string MEA { get; set; }
        public string MORA { get; set; }
        public int? ZoneDistance { get; set; }
        public int? CumulativeDistance { get; set; }
        public string Wind { get; set; }
        public string MagneticTrack { get; set; }
        public string Temperature { get; set; }

       [JsonProperty("Zone Time")]
        public string ZoneTime { get; set; }
        public string CumulativeTime { get; set; }
        public int? FuelRemained { get; set; }
        public int? FuelUsed { get; set; }
        public double? MachNo { get; set; }
        public int? TrueAirSpeed { get; set; }
        public int? GroundSpeed { get; set; }
    }
    public class MainNavLog
    {
        public string Airway { get; set; }
        public string CumulativeTime { get; set; }
        public int? CumulativeDistance { get; set; }
        public int? FuelRemained { get; set; }
        public int? FuelUsed { get; set; }
        public string FlightLevel { get; set; }
        public string Frequency { get; set; }
        public int? GroundSpeed { get; set; }
        public double? Latitude { get; set; }
        public string LatitudeStr { get; set; }
        public double? Longitude { get; set; }
        public string LongitudeStr { get; set; }
        public string MachNo { get; set; }
        public string MagneticTrack { get; set; }
        public string MEA { get; set; }
        public string MORA { get; set; }
        public string Temperature { get; set; }
        public int? TrueAirSpeed { get; set; }
        public string WayPoint { get; set; }
        public string Wind { get; set; }
        public int? ZoneDistance { get; set; }
        public string ZoneTime { get; set; }


       
        public int? CumulativeFuel { get; set; }
      
        public string Direction { get; set; }
       
        public string WindComponent { get; set; }
        
        public int? ZoneFuel { get; set; }
        


    }

    public class Alternate2NavLog
    {
        public string WayPoint { get; set; }
        public string FlightLevel { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string Frequency { get; set; }
        public string Airway { get; set; }
        public string MEA { get; set; }
        public string MORA { get; set; }
        public int? ZoneDistance { get; set; }
        public int? CumulativeDistance { get; set; }
        public string Wind { get; set; }
        public string MagneticTrack { get; set; }
        public string Temperature { get; set; }

       [JsonProperty("Zone Time")]
        public string ZoneTime { get; set; }
        public string CumulativeTime { get; set; }
        public int? FuelRemained { get; set; }
        public int? FuelUsed { get; set; }
        public double? MachNo { get; set; }
        public int? TrueAirSpeed { get; set; }
        public int? GroundSpeed { get; set; }
    }

   

    public class Distances
    {
         
        public int? AirDistance { get; set; }
        public int? Alternate1 { get; set; }
        public int? Alternate2 { get; set; }
        public int? GroundDistance { get; set; }
        public int MainGCD { get; set; }
        public int? TakeOffAlternate { get; set; }
        public int? Trip { get; set; }
 
    }

    public class Fuels
    {
        
        public int? Additional { get; set; }
        public int? Alternate { get; set; }
        public int? Contingency { get; set; }
        public int? Extra { get; set; }
        public int? FinalReserve { get; set; }
        public int? Holding { get; set; }
        public int? Landing { get; set; }
        public int? MinimumRequired { get; set; }
        public int? MODAlternate1 { get; set; }
        public int? MODAlternate2 { get; set; }
        public int? TaxiIn { get; set; }
        public int? TaxiOut { get; set; }
        public int? Total { get; set; }
        public int? Trip { get; set; }

        
        public string AdditionalDescription { get; set; }
        public int? Alternate1 { get; set; }
        public int? Alternate2 { get; set; }
        public string ExtraDescription { get; set; }
        public int? TakeoffAlternate { get; set; }
        

    }

    public class HeightChange
    {
        public object Value { get; set; }
        public object Fuel { get; set; }
    }
    public class BurnOffAdjustment
    {
        public object Value { get; set; }
        public object Fuel { get; set; }
    }


    public class Root
    {
        public string AircraftSubType { get; set; }
        public string AircraftType { get; set; }
        public int? AirDistance { get; set; }
        public string AirlineName { get; set; }
        public string Alternate { get; set; }
        public string Alternate1 { get; set; }
        public string Alternate1AverageTempISA { get; set; }
        public string Alternate1AverageWindComponent { get; set; }
        public int? Alternate1Elevation { get; set; }
        public int? Alternate1FlightLevel { get; set; }
        public List<MainNavLog> Alternate1NavLog { get; set; }
        public string Alternate1Route { get; set; }
        public List<object> Alternate1WindTemperature { get; set; }
        public string Alternate2 { get; set; }
        public string Alternate2AverageTempISA { get; set; }
        public string Alternate2AverageWindComponent { get; set; }
        public int? Alternate2Elevation { get; set; }
        public int? Alternate2FlightLevel { get; set; }
        public List<MainNavLog> Alternate2NavLog { get; set; }
        public string Alternate2Route { get; set; }
        public List<object> Alternate2WindTemperature { get; set; }
        public string AlternateEnroute { get; set; }
        public BurnOffAdjustment BurnOffAdjustment { get; set; }
        public int? Cabin { get; set; }
        public int? Cockpit { get; set; }
        public int? ContingencyPercent { get; set; }
        public int? CostIndex { get; set; }
        public string CruiseSpeed { get; set; }
        public double? CruisePerformanceFactor { get; set; }
        public string Destination { get; set; }
        public int? DestinationElevation { get; set; }
        public string DestinationIATA { get; set; }
        public Distances Distances { get; set; }
        public int? DryOperatingWeight { get; set; }
        public string Dispatcher { get; set; }
        public int? EstimatedLandingWeight { get; set; }
        public int? EstimatedTakeoffWeight { get; set; }
        public int? EstimatedZeroFuelWeight { get; set; }
        public FIRs FIRs { get; set; }
        public string FlightNo { get; set; }
        public string FlightRule { get; set; }
        public Fuels Fuels { get; set; }
        public DateTime? GenerationDate { get; set; }
        public int? GroundDistance { get; set; }
        public HeightChange HeightChange { get; set; }
        public string ICAOFlightPlan { get; set; }
        public string MainRoute { get; set; }
        public List<MainNavLog> MainNavLog { get; set; }
        public int? MainFlightLevel { get; set; }
        public List<object> MainWindTemperature { get; set; }
        public string ManeuveringFuel { get; set; }
        public string ManeuveringTime { get; set; }
        public string MaxShear { get; set; }
        public string MaxWindShearLevel { get; set; }
        public string MaxWindShearPointName { get; set; }
        public int? MaximumLandingWeight { get; set; }
        public int? MaximumTakeoffWeight { get; set; }
        public int? MaximumZeroFuelWeight { get; set; }
        public int? MODAlernate1 { get; set; }
        public int? MODAlternate2 { get; set; }
        public string MSN { get; set; }
        public string Origin { get; set; }
        public int? OriginElevation { get; set; }
        public string OriginIATA { get; set; }
        public int? Pantry { get; set; }
        public int? Payload { get; set; }
        public DateTime? PlanValidity { get; set; }
        public string Pilot1 { get; set; }
        public string Pilot2 { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime? ScheduledTimeArrival { get; set; }
        public DateTime? ScheduledTimeDeparture { get; set; }
        public string TailNo { get; set; }
        public string TakeoffAlternate { get; set; }
        public int? TakeoffAlternateElevation { get; set; }
        public int? TakeoffAlternateFlightLevel { get; set; }
        public string TakeoffAlternateRoute { get; set; }
        public List<object> TakeOffAlternateWindTemperature { get; set; }
        public Times Times { get; set; }
        public string TripAverageTempISA { get; set; }
        public string TripAverageWindComponent { get; set; }
        public string TripLevel { get; set; }
        public string Warning1 { get; set; }
        public string Warning2 { get; set; }
        public string Warning3 { get; set; }
        public string WeatherCycle { get; set; }
        public string WeightUnit { get; set; }
        public int? Extra { get; set; }
    }
    public class FIRs
    {
        public string Main { get; set; }
    }
    public class Times
    {

        
        public string Additional { get; set; }
        public string Alternate { get; set; }
        public string Alternate1 { get; set; }
        public string Alternate2 { get; set; }
        public string Contingency { get; set; }
        public string Extra { get; set; }
        public string FinalReserve { get; set; }
        public string Holding { get; set; }
        public string MinimumRequired { get; set; }
        public string TakeOffAlternate { get; set; }
        public string Total { get; set; }
        public string Trip { get; set; }


       
        public string AdditionalStr { get; set; }
        
        public string AlternateStr { get; set; }
      
        public string Alternate1Str { get; set; }
      
        public string Alternate2Str { get; set; }
       
        public string ContingencyStr { get; set; }
        
        public string ExtraStr { get; set; }
      
        public string HoldingStr { get; set; }
        
        public string MinimumRequiredStr { get; set; }
        
        public string TakeOffAlternateStr { get; set; }
        public string TaxiIn { get; set; }
        public string TaxiInStr { get; set; }
        public string TaxiOut { get; set; }
        public string TaxiOutStr { get; set; }
        
        public string TotalStr { get; set; }
        
        public string TripStr { get; set; }
    }
    public class WindTemperature 
    {
        public string FlightLevel { get; set; }
        
        public string Temperature { get; set; }
        public string Wind { get; set; }
    }

    public class WindTemperatureClimb
    {
        public string FlightLevel { get; set; }
        public string Wind { get; set; }
        public string Temperature { get; set; }
    }

    public class WindTemperatureCruise
    {
        public string FlightLevel { get; set; }
        public string Wind { get; set; }
        public string Temperature { get; set; }
    }

    public class WindTemperatureDescent
    {
        public string FlightLevel { get; set; }
        public string Wind { get; set; }
        public string Temperature { get; set; }
    }


}