//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ApiQA.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class survey_question
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public survey_question()
        {
            this.survey_result = new HashSet<survey_result>();
        }
    
        public int id { get; set; }
        public Nullable<int> category_id { get; set; }
        public string title { get; set; }
        public Nullable<int> order_index { get; set; }
    
        public virtual survey_category survey_category { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<survey_result> survey_result { get; set; }
    }
}
