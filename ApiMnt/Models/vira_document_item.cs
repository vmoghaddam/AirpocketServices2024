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
    
    public partial class vira_document_item
    {
        public int id { get; set; }
        public Nullable<int> paperId { get; set; }
        public Nullable<int> paperItemId { get; set; }
        public Nullable<int> cmP_PartNumberId { get; set; }
        public Nullable<int> partNumber_TypeId { get; set; }
        public Nullable<int> cmP_ComponentId { get; set; }
        public Nullable<int> cmP_PositionId { get; set; }
        public Nullable<int> conditionId { get; set; }
        public Nullable<int> measurementUnitId { get; set; }
        public Nullable<int> currencyId { get; set; }
        public Nullable<int> documentTypeId { get; set; }
        public string documentNo { get; set; }
        public string sN_BN { get; set; }
        public Nullable<int> shelfFromId { get; set; }
        public Nullable<int> shelfToId { get; set; }
        public Nullable<int> itemNo { get; set; }
        public Nullable<decimal> quantity { get; set; }
        public Nullable<int> price { get; set; }
        public string ataChapter { get; set; }
        public string reference { get; set; }
        public Nullable<System.DateTime> expireDate { get; set; }
        public Nullable<System.DateTime> manufactureDate { get; set; }
        public string remark { get; set; }
        public Nullable<int> document_id { get; set; }
        public Nullable<int> vira_id { get; set; }
        public Nullable<System.DateTime> paperDate { get; set; }
        public string fullNo { get; set; }
        public Nullable<int> paperYear { get; set; }
        public Nullable<int> paperType { get; set; }
        public Nullable<int> sender_LocationId { get; set; }
        public Nullable<int> receiver_LocationId { get; set; }
        public string financialAccount { get; set; }
        public string description { get; set; }
        public string partNumber { get; set; }
        public string uom { get; set; }
        public string currency { get; set; }
        public string condition { get; set; }
        public string senderLocation_Title { get; set; }
        public string receiverLocation_Title { get; set; }
        public string senderUser_FullName { get; set; }
        public string receiverUser_FullName { get; set; }
    
        public virtual vira_document vira_document { get; set; }
    }
}