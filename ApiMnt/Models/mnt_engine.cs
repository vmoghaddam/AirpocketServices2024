//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ApiMnt.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class mnt_engine
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public mnt_engine()
        {
            this.mnt_engine_adsb = new HashSet<mnt_engine_adsb>();
            this.mnt_engine_llp = new HashSet<mnt_engine_llp>();
        }
    
        public int id { get; set; }
        public Nullable<int> engine_no { get; set; }
        public string cat { get; set; }
        public string model { get; set; }
        public string serial_no { get; set; }
        public Nullable<int> remaining_cycles { get; set; }
        public Nullable<int> remaining_minutes { get; set; }
        public Nullable<int> aircraft_id { get; set; }
        public Nullable<System.DateTime> date_initial { get; set; }
    
        public virtual Ac_MSN Ac_MSN { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<mnt_engine_adsb> mnt_engine_adsb { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<mnt_engine_llp> mnt_engine_llp { get; set; }
    }
}