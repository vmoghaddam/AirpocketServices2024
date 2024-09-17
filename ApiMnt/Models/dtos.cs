using ApiMnt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiMnt
{
    
        public class vira_document_dto
        {
            public int id { get; set; }
            public int requestId { get; set; }
            public int receiptType { get; set; }
            public string acfT_TypeId { get; set; }
            public List<string> acfT_TypeIds { get; set; }
            public List<int> acfT_MSNId { get; set; }
            public int companyId { get; set; }
            public int priorityId { get; set; }
            public DateTime? deadline { get; set; }
            public int sender_LocationId { get; set; }
            public int sender_UserId { get; set; }
            public int receiver_LocationId { get; set; }
            public int receiver_UserId { get; set; }
            public string receivedPaperNo { get; set; }
            public DateTime? receivedPaperDate { get; set; }
            public string receivedInvoiveNo { get; set; }
            public DateTime? receivedInvoiveDate { get; set; }
            public string remark { get; set; }
            public string paper_no { get; set; }
            public int vira_id { get; set; }
            public string type { get; set; }

            public List<vira_document_item> items { get; set; }
        }

        public class DeliveryOrderItem
        {
            public int Id { get; set; }
            public int PaperItemId { get; set; }
            public int PartNumberId { get; set; }
            public int ComponentId { get; set; }
            public int ConditionId { get; set; }
            public int MeasurementUnitId { get; set; }
            public int ItemNo { get; set; }
            public int Quantity { get; set; }
            public string ShelfFrom { get; set; }
            public string ShelfTo { get; set; }
            public string Remark { get; set; }
        }

        public class DeliveryOrder
        {
            public int Id { get; set; }
            public int RequestId { get; set; }
            public string AcfT_TypeId { get; set; }
            public int AcfT_MSNId { get; set; }
            public int Sender_LocationId { get; set; }
            public int Sender_UserId { get; set; }
            public int Receiver_LocationId { get; set; }
            public int Receiver_UserId { get; set; }
            public string Remark { get; set; }
            public List<DeliveryOrderItem> DeliveryOrderItems { get; set; }

            public DeliveryOrder()
            {
                DeliveryOrderItems = new List<DeliveryOrderItem>();
            }
        }

        public class StockItems
        {
            public int Id { get; set; }
            public int PaperId { get; set; }
            public DateTime PaperDate { get; set; }
            public string FullNo { get; set; }
            public int PaperYear { get; set; }
            public int PaperType { get; set; }
            public int SenderLocationId { get; set; }
            public int ReceiverLocationId { get; set; }
            public int CmP_PartNumberId { get; set; }
            public int CmP_ComponentId { get; set; }
            public int MeasurementUnitId { get; set; }
            public string SN_BN { get; set; }
            public decimal Quantity { get; set; }
            public string ShelfFrom { get; set; }
            public string ShelfTo { get; set; }
            public int ItemNo { get; set; }
            public string Remark { get; set; }
            public string FinancialAccount { get; set; }
            public string Description { get; set; }
            public string PartNumber { get; set; }
            public string UOM { get; set; }
            public string Currency { get; set; }
            public string Condition { get; set; }
            public string SenderLocationTitle { get; set; }
            public string ReceiverLocationTitle { get; set; }
            public string SenderUserFullName { get; set; }
            public string ReceiverUserFullName { get; set; }

        }



        public class StockPaper
        {
            public int id { get; set; }
            public DateTime paperDate { get; set; }
            public string fullNo { get; set; }
            public int paperYear { get; set; }
            public int paperType { get; set; }
            public int sender_LocationId { get; set; }
            public int receiver_LocationId { get; set; }
            public string remark { get; set; }
            public string senderLocation_Title { get; set; }
            public string receiverLocation_Title { get; set; }
            public string senderUser_FullName { get; set; }
            public string receiverUser_FullName { get; set; }
            public string acfT_Type { get; set; }
        public List<StockItems> Items { get; set; }

    }

        //public class StockWithItems
        //{
        //    public StockPaper Paper { get; set; }
        //    public List<StockItems> Items { get; set; }
        //}


        public class PaperItem
        {

            public int Id { get; set; }
            public int PaperId { get; set; }
            public int? PaperItemId { get; set; }
            public int CmP_PartNumberId { get; set; }
            public int? CmP_ComponentId { get; set; }
            public int? CmP_PositionId { get; set; }
            public int MeasurementUnitId { get; set; }
            public int? CurrencyId { get; set; }
            public int LastStatusId { get; set; }
            public int? ConditionId { get; set; }
            public int? DocumentTypeId { get; set; }
            public string DocumentNo { get; set; }
            public string SnBn { get; set; }
            public int ItemNo { get; set; }
            public string AtaChapter { get; set; }
            public decimal? Quantity { get; set; }
            public double? Price { get; set; }
            public string Reference { get; set; }
            public DateTime? ExpireDate { get; set; }
            public DateTime? ManufactureDate { get; set; }
            public string ShelfFrom { get; set; }
            public string ShelfTo { get; set; }
            public string FinancialAccount { get; set; }
            public string Remark { get; set; }
        }




        public class auth
        {
            public string token { get; set; }
        }

        public class response
        {
            public int errorCode { get; set; }
            public string errormessage { get; set; }
            public dynamic data { get; set; }
        }


    public class vira_info
    {
        public string vira_no { get; set; }
        public int vira_id { get; set; }
        public string vira_type { get; set; }
    }

    public class RequestItem
    {
        public int Id { get; set; }
        public int PaperId { get; set; }
        public int? CmP_PositionId { get; set; }
        public int CmP_PartNumberId { get; set; }
        public int MeasurementUnitId { get; set; }
        public int LastStatusId { get; set; }
        public int ItemNo { get; set; }
        public string AtaChapter { get; set; }
        public decimal Quantity { get; set; }
        public string Reference { get; set; }
        public string Remark { get; set; }
        public string Uom { get; set; }
        public string PartNumber { get; set; }
        public bool BlockList { get; set; }
        public string Description { get; set; }
        public int IsAvailable { get; set; }
        public int PartNumberStatusInt { get; set; }
    }
    public class Request
    {
        public int Id { get; set; }
        public int? AcfT_TypeId { get; set; }
        public int? AcfT_MSNId { get; set; }
        public int? PriorityId { get; set; }
        public string RequestType { get; set; }
        public int SenderLocationId { get; set; }
        public int SenderUserId { get; set; }
        public int? ReceiverLocationId { get; set; }
        public int? ReceiverUserId { get; set; }
        public int? ApproverLocationId { get; set; }
        public int? ApproverUserId { get; set; }
        public int LastStatusId { get; set; }
        public string FullNo { get; set; }
        public DateTime PaperDate { get; set; }
        public DateTime? Deadline { get; set; }
        public string Remark { get; set; }
        public int IsApproved { get; set; }
        public int? RequestStatusInt { get; set; }
        public string Priority { get; set; }
        public string Register { get; set; }
        public string SenderUserFullName { get; set; }
        public string SenderLocationTitle { get; set; }
        public string SenderLocationFullCode { get; set; }
        public string ApproverUserFullName { get; set; }
        public string ApproverLocationTitle { get; set; }
        public string ReceiverLocationTitle { get; set; }
        public string ReceiverLocationFullCode { get; set; }

        public List<RequestItem> RequestItems { get; set; }
    }


    public class NISResponse
    {
        public int PaperItemId { get; set; }
        public int CmP_PartNumberId { get; set; }
        public int PriorityId { get; set; }
        public int SenderLocationId { get; set; }
        public int SenderUserId { get; set; }
        public decimal Quantity { get; set; }
        public string Remark { get; set; }
    }


    public class nis_cartable
    {
        public int Id { get; set; }
        public string NisNo { get; set; }
        public int CmP_PartNumberId { get; set; }
        public int PriorityId { get; set; }
        public string Priority { get; set; }
        public DateTime CreateDate { get; set; }
        public string SenderLocationTitle { get; set; }
        public string SenderUserFullName { get; set; }
        public string ApproverLocationTitle { get; set; }
        public string ApproverUserFullName { get; set; }
        public string CancellerLocationTitle { get; set; }
        public string CancellerUserFullName { get; set; }
        public DateTime? VerifiedDate { get; set; }
        public string Remark { get; set; }
        public int LastStatusId { get; set; }
        public double Quantity { get; set; }
        public string Uom { get; set; }
        public string PartNumber { get; set; }
        public string Description { get; set; }
        public int PartNumberStatusInt { get; set; }
        public string IpCReference { get; set; }
        public string RequestFullNo { get; set; }
        public DateTime RequestCreateDate { get; set; }
        public string RequestACFTType { get; set; }
        public string RequestRegister { get; set; }
        public string RequestPriority { get; set; }
        public string StockTitle { get; set; }
        public string RequestSenderUserFullName { get; set; }
        public string RequestApproverUserFullName { get; set; }
        public string RequestPartNumber { get; set; }
        public string RequestPartDescription { get; set; }
    }

}


