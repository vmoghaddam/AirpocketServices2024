//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ApiScheduling.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ViewFDPRest
    {
        public int Id { get; set; }
        public Nullable<int> CrewId { get; set; }
        public Nullable<int> JobGroupId { get; set; }
        public string JobGroup { get; set; }
        public Nullable<int> BoxId { get; set; }
        public Nullable<int> Sectors { get; set; }
        public Nullable<int> ACTypeId { get; set; }
        public string FromAirport { get; set; }
        public string FromAirportIATA { get; set; }
        public Nullable<int> FromAirportId { get; set; }
        public Nullable<int> FromAirportCityId { get; set; }
        public Nullable<int> CityId { get; set; }
        public Nullable<int> FDPLocationId { get; set; }
        public string ToAirport { get; set; }
        public string ToAirportIATA { get; set; }
        public Nullable<int> ToAirportId { get; set; }
        public Nullable<System.DateTime> STD { get; set; }
        public Nullable<System.DateTime> STA { get; set; }
        public Nullable<System.DateTime> STDLocal { get; set; }
        public Nullable<System.DateTime> STALocal { get; set; }
        public Nullable<System.DateTime> Departure { get; set; }
        public Nullable<System.DateTime> Arrival { get; set; }
        public Nullable<System.DateTime> DepartureLocal { get; set; }
        public Nullable<System.DateTime> ArrivalLocal { get; set; }
        public Nullable<System.DateTime> DefaultReportingTime { get; set; }
        public Nullable<System.DateTime> DefaultReportingTimeLocal { get; set; }
        public Nullable<System.DateTime> ReportingTime { get; set; }
        public Nullable<System.DateTime> ReportingTimeLocal { get; set; }
        public Nullable<int> FDPScheduled { get; set; }
        public Nullable<int> FDP { get; set; }
        public Nullable<double> DutyScheduled { get; set; }
        public Nullable<double> Duty { get; set; }
        public Nullable<System.DateTime> FDPStart { get; set; }
        public Nullable<System.DateTime> FDPStartLocal { get; set; }
        public Nullable<System.DateTime> FDPEnd { get; set; }
        public Nullable<System.DateTime> FDPEndLocal { get; set; }
        public Nullable<System.DateTime> DateStart { get; set; }
        public Nullable<System.DateTime> DutyStart { get; set; }
        public Nullable<System.DateTime> DateStartLocal { get; set; }
        public Nullable<System.DateTime> DutyStartLocal { get; set; }
        public Nullable<System.DateTime> DateEnd { get; set; }
        public Nullable<System.DateTime> DutyEnd { get; set; }
        public Nullable<System.DateTime> DateEndLocal { get; set; }
        public Nullable<System.DateTime> DutyEndLocal { get; set; }
        public double DelayAmount { get; set; }
        public Nullable<System.DateTime> DelayedReportingTime { get; set; }
        public Nullable<System.DateTime> NextNotification { get; set; }
        public Nullable<System.DateTime> RevisedDelayedReportingTime { get; set; }
        public Nullable<System.DateTime> FirstNotification { get; set; }
        public Nullable<double> MaxFDP { get; set; }
        public Nullable<double> MaxFDPExtended { get; set; }
        public int IsDutyOver { get; set; }
        public int ExtendedBySplitDuty { get; set; }
        public Nullable<int> StandById { get; set; }
        public Nullable<System.DateTime> StandByStart { get; set; }
        public Nullable<System.DateTime> StandByStartLocal { get; set; }
        public Nullable<int> StandByDuration { get; set; }
        public Nullable<int> FDPStandByScheduled { get; set; }
        public Nullable<int> FDPStandby { get; set; }
        public int FDPStandByScheduledError { get; set; }
        public int FDPStandbyError { get; set; }
        public Nullable<double> FDPReductionByStandBy { get; set; }
        public bool IsTemplate { get; set; }
        public Nullable<System.DateTime> DateContact { get; set; }
        public Nullable<System.DateTime> DateContactLocal { get; set; }
        public int DutyType { get; set; }
        public string DutyTypeTitle { get; set; }
        public Nullable<System.DateTime> RestUntil { get; set; }
        public Nullable<System.DateTime> RestUntilLocal { get; set; }
        public Nullable<int> LocationId { get; set; }
        public string Location { get; set; }
        public Nullable<System.DateTime> DaySTDLocal { get; set; }
        public Nullable<int> FDPId { get; set; }
        public Nullable<int> DateStartYear { get; set; }
        public Nullable<int> DateStartMonth { get; set; }
        public Nullable<int> DateStartDay { get; set; }
        public Nullable<int> TemplateId { get; set; }
        public string Remark { get; set; }
        public string Remark2 { get; set; }
        public string InitFlts { get; set; }
        public string InitRoute { get; set; }
        public string CanceledNo { get; set; }
        public string CanceledRoute { get; set; }
        public Nullable<int> Extension { get; set; }
        public int IsExtension { get; set; }
        public Nullable<System.DateTime> DateConfirmed { get; set; }
        public string ConfirmedBy { get; set; }
        public int IsConfirmed { get; set; }
        public string UserName { get; set; }
        public Nullable<int> UPD { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public string PosFrom { get; set; }
        public string PosTo { get; set; }
        public Nullable<System.DateTime> PosDep { get; set; }
        public Nullable<System.DateTime> PosArr { get; set; }
        public string PosAirline { get; set; }
        public Nullable<int> PosFDPId { get; set; }
        public string PosRemark { get; set; }
        public string PosTicketUrl { get; set; }
    }
}
