//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ApiXLS.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class neerja_preflight_check_item_category
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public neerja_preflight_check_item_category()
        {
            this.neerja_preflight_check_item = new HashSet<neerja_preflight_check_item>();
        }
    
        public int id { get; set; }
        public string category { get; set; }
        public string remark { get; set; }
        public Nullable<int> order_id { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<neerja_preflight_check_item> neerja_preflight_check_item { get; set; }
    }
}
