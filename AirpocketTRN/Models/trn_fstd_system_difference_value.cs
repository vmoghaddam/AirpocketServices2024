//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AirpocketTRN.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class trn_fstd_system_difference_value
    {
        public int id { get; set; }
        public Nullable<int> form_id { get; set; }
        public Nullable<int> system_id { get; set; }
        public string differences { get; set; }
        public string syllabus_ref { get; set; }
        public Nullable<int> compliance_level { get; set; }
        public Nullable<bool> fchar { get; set; }
        public Nullable<bool> proc { get; set; }
    
        public virtual trn_fstd_system_differences trn_fstd_system_differences { get; set; }
        public virtual trn_fstd trn_fstd { get; set; }
    }
}