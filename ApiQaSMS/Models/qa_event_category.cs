//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ApiQaSMS.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class qa_event_category
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public qa_event_category()
        {
            this.qa_event_category1 = new HashSet<qa_event_category>();
        }
    
        public int id { get; set; }
        public string title { get; set; }
        public string abbreviation { get; set; }
        public string tire { get; set; }
        public Nullable<int> parent_id { get; set; }
        public string remark { get; set; }
        public string code { get; set; }
        public string full_code { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<qa_event_category> qa_event_category1 { get; set; }
        public virtual qa_event_category qa_event_category2 { get; set; }
    }
}
