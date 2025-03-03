using Ajinomoto.Arc.Common.AppModels;
using Ajinomoto.Arc.Common.Enums;
using System.Text.Json.Serialization;

namespace Ajinomoto.Arc.Common.DtoModels
{
    public class InvoiceDetailsListResponse
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
        public string Filter { get; set; }
        public InvoiceDetailsColumn CurrentSort { get; set; }
        public SortingDirection SortDirection { get; set; }
        public PagedList<InvoiceDetailsDto> Items { get; set; }
    }

    public class InvoiceDetailsDto
    {
        public string InvoiceDetailsId { get; set; } // UUID will be generated manually

        public string? CabangId { get; set; }

        public string? CustomerName { get; set; }

        public string? SalesGrup { get; set; }

        public string? FiscalYear { get; set; }

        public int? IDCustomerSoldTo { get; set; }

        public int? DocumentNumber { get; set; }

        public string? NoInvoice { get; set; }

        public string? NoPO { get; set; }

        public double? AmtInLocCur { get; set; }

        public int? ShipTo { get; set; }

        public string? Store { get; set; }

        public string? TextDesc { get; set; }

        public DateOnly? DocDate { get; set; }

        public DateOnly? BaselineDate { get; set; }

        public DateOnly? NetDueDt { get; set; }

        public string? NoSubmitted { get; set; }

        public string? StatusTukarFaktur { get; set; }

        public string? Status { get; set; }

        public string? Action { get; set; }

        public int? BusA { get; set; }

        public DateOnly? TglKirimBarang { get; set; }

        public string? TempatTukarFaktur { get; set; }

        public DateOnly? TgKirimBerkasKeKAMT { get; set; }

        public DateOnly? TglTerimaDOBack { get; set; }

        public DateOnly? TglTerimaFakturPajak { get; set; }

        public DateOnly? TglCompletedDoc { get; set; }

        public DateOnly? TglTukarFaktur { get; set; }

        public DateOnly? TanggalBayar { get; set; }

        public DateOnly? TglTerimaBerkas { get; set; }

        public int? IdealTukarFaktur { get; set; }

        public int? TOPOutlet { get; set; }

        public string? OverdueDatabaseSAP { get; set; }

        public string? StatusOverdueDatabase { get; set; }

        public string? StatusInternalSAP { get; set; }

        public string? StatusExternalTOPOutlet { get; set; }

        public string? Keterangan { get; set; }

        public string? ActionCabangMT { get; set; }

        public string? RealisasiCabangMT { get; set; }

        // public DateOnly CreatedAt { get; set; } = DateOnly.FromDateTime(DateTime.Now); // Default value

        public string? CreatedBy { get; set; }

        public DateOnly? UpdatedAt { get; set; }

        public int? UpdatedBy { get; set; }
    }
}
