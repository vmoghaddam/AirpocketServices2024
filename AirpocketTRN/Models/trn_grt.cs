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
    
    public partial class trn_grt
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public trn_grt()
        {
            this.trn_grt_item_values = new HashSet<trn_grt_item_values>();
        }
    
        public int id { get; set; }
        public string instructor_name { get; set; }
        public Nullable<System.DateTime> assessment_date { get; set; }
        public string applicant_name { get; set; }
        public string department { get; set; }
        public string course_title { get; set; }
        public Nullable<int> class_format { get; set; }
        public string remark { get; set; }
        public string instructor_sign { get; set; }
        public string manager_sign { get; set; }
        public Nullable<int> flight_id { get; set; }
        public Nullable<int> fdp_id { get; set; }
        public Nullable<System.DateTime> training_manager_sign_date { get; set; }
        public Nullable<int> training_manager_user_id { get; set; }
        public Nullable<System.DateTime> ops_training_manager_sign_date { get; set; }
        public Nullable<int> ops_training_manager_user_id { get; set; }
        public Nullable<System.DateTime> instructor_sign_date { get; set; }
        public Nullable<int> instructor_user_id { get; set; }
        public Nullable<System.DateTime> student_sign_date { get; set; }
        public Nullable<int> student_user_id { get; set; }
        public Nullable<int> responsible_id1 { get; set; }
        public Nullable<System.DateTime> responsible_sign_date1 { get; set; }
        public Nullable<int> responsible_id2 { get; set; }
        public Nullable<System.DateTime> responsible_sign_date2 { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<trn_grt_item_values> trn_grt_item_values { get; set; }
    }
}
