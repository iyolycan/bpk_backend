using Ajinomoto.Arc.Common.Enums;

namespace Ajinomoto.Arc.Common.DtoModels
{
    public class InvoiceDetailsRequest
    {
        public int Limit { get; set; }
        public int Page { get; set; }
        public InvoiceDetailsColumn? SortOrder { get; set; }
        public InvoiceDetailsColumn CurrentSort { get; set; }
        public SortingDirection? SortDirection { get; set; }
        public string Filter { get; set; }

        // New parameters for query
        public string? Cabang { get; set; }  // Branch filter
        public int? Customer { get; set; }  // Customer filter
        public string? StatusTukarFaktur { get; set; }  // BPK status filter
        public string? Status { get; set; }  // Clearing status filter

        // New date parameters
        public DateOnly? FromDate { get; set; }  // Start date filter
        public DateOnly? ToDate { get; set; }    // End date filter
    }

    public class UpdateBaseLineRequest
    {
        public string? InvoiceDetailsId { get; set; }
        public DateOnly? TglTerimaDoBack { get; set; }  // Date of receiving DO back
        public DateOnly? TglTerimaFakturPajak { get; set; }  // Date of receiving tax invoice
        public DateOnly? TglCompletedDoc { get; set; }  // Date of completed document
        public DateOnly? TglTukarFaktur { get; set; }  // Date of invoice exchange
    }
}