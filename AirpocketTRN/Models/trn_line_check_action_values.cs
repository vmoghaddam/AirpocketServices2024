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
    
    public partial class trn_line_check_action_values
    {
        public int id { get; set; }
        public Nullable<int> action_id { get; set; }
        public Nullable<int> form_id { get; set; }
        public Nullable<int> rating { get; set; }
    
        public virtual trn_line_check trn_line_check { get; set; }
        public virtual trn_line_check_actions trn_line_check_actions { get; set; }
    }
}
