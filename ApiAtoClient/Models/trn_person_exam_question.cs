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
    
    public partial class trn_person_exam_question
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public trn_person_exam_question()
        {
            this.trn_exam_student_answer = new HashSet<trn_exam_student_answer>();
        }
    
        public int id { get; set; }
        public int exam_id { get; set; }
        public int question_id { get; set; }
        public Nullable<int> response_time { get; set; }
        public string remark { get; set; }
        public Nullable<int> person_id { get; set; }
    
        public virtual trn_person_exam trn_person_exam { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<trn_exam_student_answer> trn_exam_student_answer { get; set; }
    }
}