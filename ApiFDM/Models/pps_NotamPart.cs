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
    
    public partial class pps_NotamPart
    {
        public int Id { get; set; }
        public int NotamId { get; set; }
        public Nullable<int> PartNumber { get; set; }
    
        public virtual pps_Notam pps_Notam { get; set; }
    }
}
