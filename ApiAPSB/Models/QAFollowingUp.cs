//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ApiAPSB.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class QAFollowingUp
    {
        public int Id { get; set; }
        public Nullable<int> Type { get; set; }
        public Nullable<int> EntityId { get; set; }
        public Nullable<int> ReferrerId { get; set; }
        public Nullable<int> ReferredId { get; set; }
        public Nullable<int> ReviewResult { get; set; }
        public string Comment { get; set; }
        public Nullable<System.DateTime> DateStatus { get; set; }
        public Nullable<int> ParentId { get; set; }
        public Nullable<System.DateTime> DeadLine { get; set; }
        public Nullable<int> Priority { get; set; }
        public Nullable<System.DateTime> DateVisit { get; set; }
        public Nullable<System.DateTime> ReceiverDateVisit { get; set; }
        public Nullable<System.DateTime> DateNotified { get; set; }
        public Nullable<int> NotificationCount { get; set; }
        public string SMS { get; set; }
        public Nullable<int> SMSRefId { get; set; }
    }
}