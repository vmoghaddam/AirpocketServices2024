//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace XAPI.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class OFPB_Fuels
    {
        public int Id { get; set; }
        public Nullable<int> Trip { get; set; }
        public Nullable<int> Alternate { get; set; }
        public Nullable<int> FinalReserve { get; set; }
        public Nullable<int> Contingency { get; set; }
        public Nullable<int> TaxiOut { get; set; }
        public Nullable<int> TaxiIn { get; set; }
        public Nullable<int> MinimumRequired { get; set; }
        public Nullable<int> Additional { get; set; }
        public Nullable<int> Extra { get; set; }
        public Nullable<int> Total { get; set; }
        public Nullable<int> Landing { get; set; }
        public Nullable<int> RootId { get; set; }
    
        public virtual OFPB_Root OFPB_Root { get; set; }
    }
}