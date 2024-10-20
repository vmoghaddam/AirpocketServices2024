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
    
    public partial class view_mnt_aircraft
    {
        public int id { get; set; }
        public string register { get; set; }
        public Nullable<System.DateTime> date_initial { get; set; }
        public Nullable<int> landing_gear_ldg_remaining { get; set; }
        public Nullable<System.DateTime> date_initial_landing_gear { get; set; }
        public Nullable<System.DateTime> date_initial_apu { get; set; }
        public Nullable<System.DateTime> date_initial_ht1 { get; set; }
        public Nullable<System.DateTime> date_initial_ht2 { get; set; }
        public Nullable<System.DateTime> date_initial_ht3 { get; set; }
        public Nullable<System.DateTime> date_initial_due { get; set; }
        public Nullable<int> total_flight_cycle_actual { get; set; }
        public Nullable<int> total_flight_minute_actual { get; set; }
        public Nullable<int> total_flight_cycle { get; set; }
        public Nullable<int> total_flight_minute { get; set; }
        public Nullable<int> deffects_no { get; set; }
        public Nullable<int> eng1_remaining { get; set; }
        public Nullable<int> eng2_remaining { get; set; }
        public Nullable<int> landing_gear_remaining_actual { get; set; }
        public Nullable<int> apu_remaining_actual { get; set; }
        public Nullable<int> ht1_remaining_actual { get; set; }
        public Nullable<int> ht2_remaining_actual { get; set; }
        public Nullable<int> ht3_remaining_actual { get; set; }
        public Nullable<int> first_due_remaining_actual { get; set; }
        public Nullable<int> apu_remaining { get; set; }
        public Nullable<int> landing_gear_remaining { get; set; }
        public Nullable<int> ht1_remaining { get; set; }
        public Nullable<int> ht2_remaining { get; set; }
        public Nullable<int> ht3_remaining { get; set; }
        public Nullable<int> first_due_remaining { get; set; }
        public string serial_no { get; set; }
        public string maintenance_setting_group { get; set; }
    }
}
