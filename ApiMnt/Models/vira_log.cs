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
    
    public partial class vira_log
    {
        public int id { get; set; }
        public string paper_no { get; set; }
        public Nullable<int> paper_id { get; set; }
        public Nullable<int> paper_item_id { get; set; }
        public string paper_type { get; set; }
        public Nullable<System.DateTime> date_create { get; set; }
        public string error_message { get; set; }
        public string request_payload { get; set; }
        public string request_url { get; set; }
    }
}
