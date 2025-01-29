using Ajinomoto.Arc.Common.Enums;

namespace Ajinomoto.Arc.Common.DtoModels
{
    public class IncomingPaymentListRequest
    {
        public int Limit { get; set; }
        public int Page { get; set; }
        public IncomingPaymentColumn? SortOrder { get; set; }
        public IncomingPaymentColumn CurrentSort { get; set; }
        public SortingDirection? SortDirection { get; set; }
        public string Filter { get; set; }

        // New parameters for query
        public string? Cabang { get; set; }  // Branch filter
        public string? Customer { get; set; }  // Customer filter
        public string? StatusBpk { get; set; }  // BPK status filter
        public string? StatusClearing { get; set; }  // Clearing status filter
        public string? Area { get; set; }  // Area filter

        // New date parameters
        public DateOnly? FromDate { get; set; }  // Start date filter
        public DateOnly? ToDate { get; set; }    // End date filter
    }
}