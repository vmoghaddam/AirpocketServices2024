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
        public string MachNo { get; set; }
        public int? TrueAirSpeed { get; set; }
        public int? GroundSpeed { get; set; }
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
     
        public int? Trip { get; set; }
        public int? Alternate1 { get; set; }
        public int? Alternate2 { get; set; }
        public int? TakeOffAlternate { get; set; }
        public int? GroundDistance { get; set; }
        public int? AirDistance { get; set; }
    }

    public class Fuels
    {
        public int? Trip { get; set; }
        public int? Alternate { get; set; }
        public int? FinalReserve { get; set; }
        public int? Contingency { get; set; }
        public int? TaxiOut { get; set; }
        public int? TaxiIn { get; set; }
        public int? MinimumRequired { get; set; }
        public int? Additional { get; set; }
        public int? Extra { get; set; }
        public int? Total { get; set; }
        public int? Landing { get; set; }
        public int? Holding { get; set; }

        public int? MODAlternate1 { get; set; }
        public int? MODAlternate2 { get; set; }

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
        public string ReferenceNo { get; set; }
        public string AirlineName { get; set; }
        public string WeightUnit { get; set; }
        public int? CruisePerformanceFactor { get; set; }
        public int? ContingencyPercent { get; set; }
        public string FlightNo { get; set; }
        public DateTime? GenerationDate { get; set; }
        public DateTime? ScheduledTimeDeparture { get; set; }
        public DateTime? ScheduledTimeArrival { get; set; }
        public string TailNo { get; set; }
        public string CruiseSpeed { get; set; }
        public int? CostIndex { get; set; }
        public int? MainFlightLevel { get; set; }
        public int? DryOperatingWeight { get; set; }
        public int? Payload { get; set; }
        public int? GroundDistance { get; set; }
        public int? AirDistance { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string Alternate1 { get; set; }
        public string Alternate2 { get; set; }
        public string TakeoffAlternate { get; set; }
        public int? MODAlernate1 { get; set; }
        public int? MODAlternate2 { get; set; }
        public int? Cockpit { get; set; }
        public int? Cabin { get; set; }
        public int? Extra { get; set; }
        public int? Pantry { get; set; }
        public string Pilot1 { get; set; }
        public string Pilot2 { get; set; }
        public string Dispatcher { get; set; }
        public int? OriginElevation { get; set; }
        public int? DestinationElevation { get; set; }
        public int? Alternate1Elevation { get; set; }
        public int? Alternate2Elevation { get; set; }
        public int? TakeoffAlternateElevation { get; set; }
        public string MaxShear { get; set; }
        public int? MaximumZeroFuelWeight { get; set; }
        public int? MaximumTakeoffWeight { get; set; }
        public int? MaximumLandingWeight { get; set; }
        public int? EstimatedZeroFuelWeight { get; set; }
        public int? EstimatedTakeoffWeight { get; set; }
        public int? EstimatedLandingWeight { get; set; }
        public string MainRoute { get; set; }
        public string Alternate1Route { get; set; }
        public string Alternate2Route { get; set; }
        public string TakeoffAlternateRoute { get; set; }
        public string MaxWindShearLevel { get; set; }
        public string MaxWindShearPointName { get; set; }
        public string FlightRule { get; set; }
        public string ICAOFlightPlan { get; set; }
        public DateTime? PlanValidity { get; set; }
        public Fuels Fuels { get; set; }
        public Times Times { get; set; }
        public Distances Distances { get; set; }
        public List<MainNavLog> MainNavLog { get; set; }
        public List<MainNavLog> Alternate1NavLog { get; set; }
        public List<MainNavLog> Alternate2NavLog { get; set; }
        //public List<HeightChange> HeightChange { get; set; }
        public  HeightChange  HeightChange { get; set; }
        //public List<BurnOffAdjustment> BurnOffAdjustment { get; set; }
        public  BurnOffAdjustment  BurnOffAdjustment { get; set; }
        //public List<WindTemperature> WindTemperatureClimb { get; set; }
        // public List<WindTemperature> WindTemperatureCruise { get; set; }
        // public List<WindTemperature> WindTemperatureDescent { get; set; }
        //List<List<KeyValuePair<string,string>>>

        public List<object> MainWindTemperature { get; set; }
        public List<object> Alternate1WindTemperature { get; set; }
        public List<object> Alternate2WindTemperature { get; set; }
        public List<object> TakeOffAlternateWindTemperature { get; set; }
    }

    public class Times
    {
     
        public string Trip { get; set; }
        public string Alternate { get; set; }
        public string Alternate1 { get; set; }
        public string Alternate2 { get; set; }
        public string TakeOffAlternate { get; set; }
        public string FinalReserve { get; set; }
        public string Holding { get; set; }
        public string Contingency { get; set; }
        public string MinimumRequired { get; set; }
        public string Additional { get; set; }
        public string Extra { get; set; }
        public string Total { get; set; }
    }
    public class WindTemperature 
    {
        public string FlightLevel { get; set; }
        public string Wind { get; set; }
        public string Temperature { get; set; }
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