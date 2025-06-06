//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ApiProfile.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class PersonDocument
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PersonDocument()
        {
            this.Documents = new HashSet<Document>();
        }
    
        public int PersonId { get; set; }
        public string Title { get; set; }
        public string Remark { get; set; }
        public int Id { get; set; }
        public int DocumentTypeId { get; set; }
        public Nullable<System.DateTime> DateIssue { get; set; }
        public Nullable<System.DateTime> DateExpire { get; set; }
    
        public virtual Person Person { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Document> Documents { get; set; }
    }
}
