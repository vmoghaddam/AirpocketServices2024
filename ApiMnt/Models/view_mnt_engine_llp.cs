//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ApiMnt.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class view_mnt_engine_llp
    {
        public int id { get; set; }
        public string cat_a { get; set; }
        public string cat_b { get; set; }
        public string cat_c { get; set; }
        public string cat { get; set; }
        public Nullable<int> remaining_cycles { get; set; }
        public Nullable<System.DateTime> date_due { get; set; }
        public string title { get; set; }
        public Nullable<int> remaining_due_actual { get; set; }
        public Nullable<System.DateTime> date_initial { get; set; }
        public Nullable<int> remaining_initial_actual { get; set; }
        public Nullable<int> engine_id { get; set; }
    }
}
