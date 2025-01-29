using Ajinomoto.Arc.Common.AppModels;
using Ajinomoto.Arc.Common.Enums;
using System.Text.Json.Serialization;

namespace Ajinomoto.Arc.Common.DtoModels
{
    public class IncomingPaymentListResponse
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
        public string Filter { get; set; }
        public IncomingPaymentColumn CurrentSort { get; set; }
        public SortingDirection SortDirection { get; set; }
        public PagedList<IncomingPaymentDto> Items { get; set; }
    }

    public class IncomingPaymentDto
    {
        public Guid IncomingPaymentId { get; set; }
        public Guid? BpkId { get; set; }
        public string BpkNumber { get; set; }
        [JsonIgnore]
        public DateOnly PaymentDate { get; set; }
        public string PaymentDateString { get; set; }
        public double Amount { get; set; }
        public string AmountString { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Area { get; set; }
        public string Branch { get; set; }
        public string InterfaceNumber { get; set; }
        public string ClearingNumber { get; set; }
        public int? BpkStatusId { get; set; }
        public string BpkStatus { get; set; }
        public int ClearingStatusId { get; set; }
        public string ClearingStatus { get; set; }
        public bool IsClearingManual { get; set; }
    }
}
