//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class ViewFormVacation
    {
        public int Id { get; set; }
        public int Reason { get; set; }
        public string ReasonStr { get; set; }
        public Nullable<System.DateTime> DateFrom { get; set; }
        public Nullable<System.DateTime> DateTo { get; set; }
        public string Remark { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<System.DateTime> DateCreate { get; set; }
        public string OperationRemak { get; set; }
        public string SchedulingRemark { get; set; }
        public Nullable<System.DateTime> DateStatus { get; set; }
        public string Status { get; set; }
        public Nullable<int> OperatorId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Name { get; set; }
        public string PID { get; set; }
        public string NID { get; set; }
        public string Mobile { get; set; }
        public string JobGroup { get; set; }
        public Nullable<int> ResponsibleId { get; set; }
        public string Responsible { get; set; }
        public Nullable<System.DateTime> ResponsibleDateVisit { get; set; }
        public Nullable<int> ResponsibleActionId { get; set; }
        public int PersonId { get; set; }
        public int EmployeeId { get; set; }
        public string ResponsibleAction { get; set; }
        public Nullable<System.DateTime> ResponsibleActionDate { get; set; }
        public string ResponsibleRemark { get; set; }
        public Nullable<int> FDPId { get; set; }
        public string RosterException { get; set; }
    }
}
