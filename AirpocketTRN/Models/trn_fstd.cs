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
    
    public partial class trn_fstd
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public trn_fstd()
        {
            this.trn_fstd_system_difference_value = new HashSet<trn_fstd_system_difference_value>();
        }
    
        public int id { get; set; }
        public string fstd_approval { get; set; }
        public string location { get; set; }
        public string fstd_operator { get; set; }
        public string fstd_type { get; set; }
        public string aircraft_type { get; set; }
        public string approval_level { get; set; }
        public Nullable<System.DateTime> qualification_valid_until { get; set; }
        public string restrictions { get; set; }
        public string other_remarks { get; set; }
        public string low_visibility_training_remarks { get; set; }
        public string recurrent_training_checking_remarks { get; set; }
        public string operation_from_either_pilots_seat_remarks { get; set; }
        public string difference_training_remarks { get; set; }
        public string recency_of_experience_remarks { get; set; }
        public string zero_flighttime_training_remarks { get; set; }
        public string other_training_remarks { get; set; }
        public string sign { get; set; }
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
        public virtual ICollection<trn_fstd_system_difference_value> trn_fstd_system_difference_value { get; set; }
    }
}