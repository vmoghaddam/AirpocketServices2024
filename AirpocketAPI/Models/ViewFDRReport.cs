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
    
    public partial class ViewFDRReport
    {
        public int Id { get; set; }
        public Nullable<System.DateTime> STDDay { get; set; }
        public string FlightNumber { get; set; }
        public string Register { get; set; }
        public Nullable<int> RegisterID { get; set; }
        public string AircraftType { get; set; }
        public Nullable<int> TypeId { get; set; }
        public Nullable<System.DateTime> STD { get; set; }
        public Nullable<System.DateTime> STDLocal { get; set; }
        public Nullable<System.DateTime> STA { get; set; }
        public Nullable<System.DateTime> STALocal { get; set; }
        public Nullable<System.DateTime> Takeoff { get; set; }
        public Nullable<System.DateTime> Landing { get; set; }
        public Nullable<System.DateTime> TakeoffLocal { get; set; }
        public Nullable<System.DateTime> LandingLocal { get; set; }
        public string IP { get; set; }
        public string IPSCH { get; set; }
        public string IPCode { get; set; }
        public Nullable<int> IPId { get; set; }
        public string P1 { get; set; }
        public string P1SCH { get; set; }
        public string P1Code { get; set; }
        public Nullable<int> P1Id { get; set; }
        public string P2 { get; set; }
        public string P2SCH { get; set; }
        public string P2Code { get; set; }
        public Nullable<int> P2Id { get; set; }
        public string FlightStatus { get; set; }
        public Nullable<int> FlightStatusID { get; set; }
        public string FromAirportIATA { get; set; }
        public string ToAirportIATA { get; set; }
        public Nullable<int> FromAirport { get; set; }
        public Nullable<int> ToAirport { get; set; }
        public Nullable<decimal> FuelRemaining { get; set; }
        public Nullable<decimal> FuelUplift { get; set; }
        public Nullable<decimal> FuelUsed { get; set; }
        public Nullable<int> FuelUnitID { get; set; }
        public string FuelUnit { get; set; }
        public Nullable<int> PFLR { get; set; }
        public Nullable<decimal> FPFuel { get; set; }
        public string PFLRTitle { get; set; }
        public Nullable<System.DateTime> DepartureLocal { get; set; }
        public Nullable<System.DateTime> ArrivalLocal { get; set; }
        public Nullable<System.DateTime> Departure { get; set; }
        public Nullable<System.DateTime> Arrival { get; set; }
        public Nullable<int> BlockTime { get; set; }
        public Nullable<int> FlightTime { get; set; }
        public int Freight { get; set; }
        public Nullable<decimal> FPTripFuel { get; set; }
        public Nullable<int> MaxWeightTO { get; set; }
        public Nullable<int> MaxWeightLND { get; set; }
        public string MaxWeighUnit { get; set; }
    }
}
