//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ApiFDM.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class fdm_crew
    {
        public int processed_id { get; set; }
        public int crew_id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string name { get; set; }
        public string rank { get; set; }
        public Nullable<int> rank_code { get; set; }
        public string position { get; set; }
        public string remark { get; set; }
        public string reserved1 { get; set; }
        public string reserved2 { get; set; }
        public string reserved3 { get; set; }
        public string reserved4 { get; set; }
        public string reserved5 { get; set; }
        public string aircraft_type { get; set; }
        public Nullable<int> aircraft_type_id { get; set; }
    
        public virtual fdm_processed fdm_processed { get; set; }
    }
}
