﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ApiForm.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ppa_entities : DbContext
    {
        public ppa_entities()
            : base("name=ppa_entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AppFuel> AppFuels { get; set; }
        public virtual DbSet<ViewProfile> ViewProfiles { get; set; }
        public virtual DbSet<ViewCrewDutyTimeLineNew> ViewCrewDutyTimeLineNews { get; set; }
        public virtual DbSet<ViewFormVacation> ViewFormVacations { get; set; }
        public virtual DbSet<FDP> FDPs { get; set; }
        public virtual DbSet<FormVacation> FormVacations { get; set; }
        public virtual DbSet<ViewEmployee> ViewEmployees { get; set; }
        public virtual DbSet<qa_notification_history> qa_notification_history { get; set; }
        public virtual DbSet<ViewCrew> ViewCrews { get; set; }
        public virtual DbSet<request_receiver> request_receiver { get; set; }
    }
}
