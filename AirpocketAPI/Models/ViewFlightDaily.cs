//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AirpocketAPI.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ViewFlightDaily
    {
        public int ID { get; set; }
        public int FlightId { get; set; }
        public Nullable<int> FlightPlanId { get; set; }
        public Nullable<System.DateTime> STD { get; set; }
        public Nullable<System.DateTime> STA { get; set; }
        public Nullable<System.DateTime> STDLocal { get; set; }
        public Nullable<System.DateTime> STALocal { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public Nullable<int> FlightStatusID { get; set; }
        public Nullable<int> RegisterID { get; set; }
        public Nullable<int> FlightTypeID { get; set; }
        public string AircraftType { get; set; }
        public Nullable<int> TypeId { get; set; }
        public string FlightNumber { get; set; }
        public Nullable<int> FromAirport { get; set; }
        public string FromAirportICAO { get; set; }
        public Nullable<int> ToAirport { get; set; }
        public string ToAirportICAO { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public string FromAirportIATA { get; set; }
        public string ToAirportIATA { get; set; }
        public string Register { get; set; }
        public string FlightStatus { get; set; }
        public string ArrivalRemark { get; set; }
        public string DepartureRemark { get; set; }
        public Nullable<System.DateTime> STDDay { get; set; }
        public Nullable<System.DateTime> STDDayLocal { get; set; }
        public Nullable<System.DateTime> STADay { get; set; }
        public Nullable<int> DelayOffBlock { get; set; }
        public Nullable<int> DelayTakeoff { get; set; }
        public Nullable<System.DateTime> OSTA { get; set; }
        public Nullable<int> OToAirportId { get; set; }
        public string OToAirportIATA { get; set; }
        public Nullable<int> FPFlightHH { get; set; }
        public Nullable<int> FPFlightMM { get; set; }
        public Nullable<System.DateTime> Departure { get; set; }
        public Nullable<System.DateTime> Arrival { get; set; }
        public Nullable<System.DateTime> BlockOff { get; set; }
        public Nullable<System.DateTime> BlockOn { get; set; }
        public Nullable<System.DateTime> TakeOff { get; set; }
        public Nullable<System.DateTime> Landing { get; set; }
        public Nullable<System.DateTime> BlockOffLocal { get; set; }
        public Nullable<System.DateTime> BlockOnLocal { get; set; }
        public Nullable<System.DateTime> TakeOffLocal { get; set; }
        public Nullable<System.DateTime> LandingLocal { get; set; }
        public Nullable<System.DateTime> DepartureLocal { get; set; }
        public Nullable<System.DateTime> ArrivalLocal { get; set; }
        public Nullable<int> BlockTime { get; set; }
        public Nullable<int> ScheduledTime { get; set; }
        public Nullable<int> FlightTime { get; set; }
        public Nullable<int> status { get; set; }
        public Nullable<System.DateTime> JLOffBlock { get; set; }
        public Nullable<System.DateTime> JLOnBlock { get; set; }
        public Nullable<System.DateTime> JLTakeOff { get; set; }
        public Nullable<System.DateTime> JLLanding { get; set; }
        public Nullable<int> PFLR { get; set; }
        public int PaxChild { get; set; }
        public int PaxInfant { get; set; }
        public int PaxAdult { get; set; }
        public Nullable<int> RevPax { get; set; }
        public Nullable<int> TotalPax { get; set; }
        public Nullable<int> FuelUnitID { get; set; }
        public Nullable<decimal> FuelArrival { get; set; }
        public Nullable<decimal> FuelDeparture { get; set; }
        public Nullable<double> UpliftLtr { get; set; }
        public Nullable<double> UpliftLbs { get; set; }
        public Nullable<double> UpliftKg { get; set; }
        public Nullable<decimal> UsedFuel { get; set; }
        public Nullable<int> TotalSeat { get; set; }
        public int BaggageWeight { get; set; }
        public int CargoWeight { get; set; }
        public Nullable<int> Freight { get; set; }
        public Nullable<double> BaggageWeightLbs { get; set; }
        public Nullable<double> BaggageWeightKg { get; set; }
        public Nullable<double> CargoWeightLbs { get; set; }
        public Nullable<double> CargoWeightKg { get; set; }
        public Nullable<double> FreightLbs { get; set; }
        public Nullable<double> FreightKg { get; set; }
        public Nullable<System.DateTime> FlightDate { get; set; }
        public Nullable<int> CargoCount { get; set; }
        public Nullable<int> BaggageCount { get; set; }
        public Nullable<int> JLBlockTime { get; set; }
        public Nullable<int> JLFlightTime { get; set; }
        public Nullable<decimal> FPFuel { get; set; }
        public Nullable<decimal> FPTripFuel { get; set; }
        public Nullable<int> MaxWeightTO { get; set; }
        public Nullable<int> MaxWeightLND { get; set; }
        public string MaxWeighUnit { get; set; }
        public string ChrCode { get; set; }
        public string ChrTitle { get; set; }
        public Nullable<int> ChrCapacity { get; set; }
        public Nullable<int> ChrAdult { get; set; }
        public Nullable<int> ChrChild { get; set; }
        public Nullable<int> ChrInfant { get; set; }
        public Nullable<int> PMonth { get; set; }
        public string PMonthName { get; set; }
        public string PDayName { get; set; }
        public string FlightType2 { get; set; }
        public string FlightIndex { get; set; }
        public Nullable<int> AirlineSold { get; set; }
        public Nullable<int> CherterSold { get; set; }
        public Nullable<int> OverSeat { get; set; }
        public Nullable<int> EmptySeat { get; set; }
        public string DelayReason { get; set; }
        public Nullable<double> Distance { get; set; }
        public Nullable<double> StationIncome { get; set; }
        public string TotalRemark { get; set; }
        public string Route { get; set; }
        public string PDate { get; set; }
        public Nullable<System.DateTime> DateLocal { get; set; }
        public Nullable<System.DateTime> TakeOffDay { get; set; }
        public Nullable<System.DateTime> TakeOffDayLocal { get; set; }
        public Nullable<int> PMonthTakeOff { get; set; }
        public string PMonthNameTakeOff { get; set; }
        public string PDayNameTakeOff { get; set; }
        public string PDateTakeOff { get; set; }
        public int IsDepInt { get; set; }
        public int IsArrInt { get; set; }
        public string XRoute { get; set; }
        public int FixTime { get; set; }
        public string PYear { get; set; }
        public int IsINT { get; set; }
    }
}
