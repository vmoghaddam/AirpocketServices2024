//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ApiAtoClient.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class view_trn_profile
    {
        public int Id { get; set; }
        public string NID { get; set; }
        public int SexId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Nullable<System.DateTime> DateBirth { get; set; }
        public string Email { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Mobile { get; set; }
        public string PassportNumber { get; set; }
        public Nullable<System.DateTime> DatePassportIssue { get; set; }
        public Nullable<System.DateTime> DatePassportExpire { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string Remark { get; set; }
        public Nullable<int> CityId { get; set; }
        public string FatherName { get; set; }
        public string IDNo { get; set; }
        public string UserId { get; set; }
        public string ImageUrl { get; set; }
        public string LicenceTitle { get; set; }
        public string PostalCode { get; set; }
        public string ScheduleName { get; set; }
        public string Code { get; set; }
        public string WhatsApp { get; set; }
        public string Telegram { get; set; }
        public string LinkedIn { get; set; }
    }
}