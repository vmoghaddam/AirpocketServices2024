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
    
    public partial class trn_grt_item_values
    {
        public int id { get; set; }
        public Nullable<int> form_id { get; set; }
        public Nullable<int> item_id { get; set; }
        public Nullable<int> grade { get; set; }
    
        public virtual trn_grt_items trn_grt_items { get; set; }
        public virtual trn_grt trn_grt { get; set; }
    }
}