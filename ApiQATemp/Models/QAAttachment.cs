//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ApiQATemp.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class QAAttachment
    {
        public int Id { get; set; }
        public Nullable<int> Type { get; set; }
        public Nullable<int> EntityId { get; set; }
        public Nullable<int> EmployeeId { get; set; }
        public string AttachmentType { get; set; }
        public string URL { get; set; }
        public string Lable { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> DateAttach { get; set; }
        public Nullable<int> CommentId { get; set; }
    }
}
