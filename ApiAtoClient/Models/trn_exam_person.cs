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
    
    public partial class trn_exam_person
    {
        public int id { get; set; }
        public int exam_id { get; set; }
        public int person_id { get; set; }
        public string remark { get; set; }
    
        public virtual trn_exam trn_exam { get; set; }
    }
}