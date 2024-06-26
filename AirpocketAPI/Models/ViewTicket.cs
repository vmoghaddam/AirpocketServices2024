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
    
    public partial class ViewTicket
    {
        public int Id { get; set; }
        public int CrewId { get; set; }
        public string ScheduleName { get; set; }
        public string Mobile { get; set; }
        public string JobGroup { get; set; }
        public Nullable<System.DateTime> DateLocal { get; set; }
        public Nullable<System.DateTime> DateStartLocal { get; set; }
        public Nullable<System.DateTime> DateEndLocal { get; set; }
        public Nullable<System.DateTime> Start { get; set; }
        public Nullable<System.DateTime> StartUTC { get; set; }
        public Nullable<System.DateTime> End { get; set; }
        public Nullable<System.DateTime> EndUTC { get; set; }
        public string Remark { get; set; }
        public string PosFrom { get; set; }
        public string PosTo { get; set; }
        public Nullable<System.DateTime> PosDepUtc { get; set; }
        public Nullable<System.DateTime> PosArrUtc { get; set; }
        public Nullable<System.DateTime> PosDep { get; set; }
        public Nullable<System.DateTime> PosArr { get; set; }
        public string PosAirline { get; set; }
        public string FlightNumber { get; set; }
        public string PosTicketUrl { get; set; }
    }
}
