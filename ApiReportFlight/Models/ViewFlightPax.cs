//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ApiReportFlight.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ViewFlightPax
    {
        public int Id { get; set; }
        public int FlightId { get; set; }
        public Nullable<int> RES_Adult { get; set; }
        public Nullable<int> RES_Child { get; set; }
        public Nullable<int> RES_Infant { get; set; }
        public Nullable<int> STN_Adult { get; set; }
        public Nullable<int> STN_Child { get; set; }
        public Nullable<int> STN_Infant { get; set; }
        public Nullable<int> CHR_Adult { get; set; }
        public Nullable<int> CHR_Child { get; set; }
        public Nullable<int> CHR_Infant { get; set; }
        public Nullable<int> FOC_Adult { get; set; }
        public Nullable<int> FOC_Child { get; set; }
        public Nullable<int> FOC_Infant { get; set; }
        public Nullable<int> OA_Adult { get; set; }
        public Nullable<int> OA_Child { get; set; }
        public Nullable<int> OA_Infant { get; set; }
        public Nullable<int> AirportId { get; set; }
        public Nullable<int> Cargo { get; set; }
        public Nullable<int> Baggage { get; set; }
        public Nullable<int> ToAirportId { get; set; }
        public Nullable<int> FM { get; set; }
        public Nullable<int> DSP { get; set; }
        public Nullable<int> FSG { get; set; }
        public Nullable<int> WCR { get; set; }
        public Nullable<int> MOC { get; set; }
        public Nullable<int> ACM { get; set; }
        public Nullable<int> FlightId2 { get; set; }
        public string FromAirportIATA { get; set; }
        public string ToAirportIATA { get; set; }
        public string FlightNumber { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string PDate { get; set; }
        public Nullable<System.DateTime> STD { get; set; }
        public Nullable<System.DateTime> STA { get; set; }
        public Nullable<System.DateTime> STDLocal { get; set; }
        public Nullable<System.DateTime> STALocal { get; set; }
        public Nullable<System.DateTime> ChocksOut { get; set; }
        public Nullable<System.DateTime> Takeoff { get; set; }
        public Nullable<System.DateTime> Landing { get; set; }
        public Nullable<System.DateTime> ChocksIn { get; set; }
        public string Register { get; set; }
        public string ChrTitle { get; set; }
        public string FlightStatus { get; set; }
        public Nullable<int> Total_RES { get; set; }
        public Nullable<int> Total_STN { get; set; }
        public Nullable<int> Total_CHR { get; set; }
        public Nullable<int> Total_FOC { get; set; }
        public Nullable<int> Total_OA { get; set; }
        public Nullable<int> Rev_RES { get; set; }
        public Nullable<int> Rev_STN { get; set; }
        public Nullable<int> Rev_CHR { get; set; }
        public Nullable<int> Rev_FOC { get; set; }
        public Nullable<int> Rev_OA { get; set; }
        public Nullable<int> Total_Pax { get; set; }
        public Nullable<int> Total_Rev { get; set; }
    }
}