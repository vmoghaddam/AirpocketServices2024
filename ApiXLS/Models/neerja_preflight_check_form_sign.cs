//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ApiXLS.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class neerja_preflight_check_form_sign
    {
        public int id { get; set; }
        public int form_id { get; set; }
        public int crew_id { get; set; }
        public Nullable<System.DateTime> date_sign { get; set; }
        public string crew_no { get; set; }
        public string remark { get; set; }
        public int flight_id { get; set; }
    
        public virtual FlightInformation FlightInformation { get; set; }
    }
}