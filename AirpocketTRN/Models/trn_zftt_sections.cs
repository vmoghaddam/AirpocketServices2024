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
    
    public partial class trn_zftt_sections
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public trn_zftt_sections()
        {
            this.trn_zftt_actions = new HashSet<trn_zftt_actions>();
        }
    
        public int id { get; set; }
        public string section_name { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<trn_zftt_actions> trn_zftt_actions { get; set; }
    }
}
