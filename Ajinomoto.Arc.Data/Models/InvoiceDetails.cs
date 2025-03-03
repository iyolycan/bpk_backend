using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ajinomoto.Arc.Data.Models
{
    [Table("invoice_details")] // Maps to the `invoice_details` table
    public partial class InvoiceDetails
    {
        [Key] // Primary key
        [Column("invoice_details_id")]
        public string InvoiceDetailsId { get; set; } // UUID will be generated manually

        [Column("cabang_id")]
        public string? CabangId { get; set; }

        [Column("customer_name")]
        public string? CustomerName { get; set; }

        [Column("sales_grup")]
        public string? SalesGrup { get; set; }

        [Column("fiscal_year")]
        public string? FiscalYear { get; set; }

        [Column("IDCustomerSoldTo")]
        public int? IDCustomerSoldTo { get; set; }

        [Column("DocumentNumber")]
        public int? DocumentNumber { get; set; }

        [Column("NoInvoice")]
        public string? NoInvoice { get; set; }

        [Column("NoPO")]
        public string? NoPO { get; set; }

        [Column("AmtInLocCur")]
        public double? AmtInLocCur { get; set; }

        [Column("ShipTo")]
        public int? ShipTo { get; set; }

        [Column("store")]
        public string? Store { get; set; }

        [Column("textDesc")]
        public string? TextDesc { get; set; }

        [Column("DocDate")]
        public DateOnly? DocDate { get; set; }

        [Column("BaselineDate")]
        public DateOnly? BaselineDate { get; set; }

        [Column("NetDueDt")]
        public DateOnly? NetDueDt { get; set; }

        [Column("NoSubmitted")]
        public string? NoSubmitted { get; set; }

        [Column("StatusTukarFaktur")]
        public string StatusTukarFaktur { get; set; } = "Not Ok"; // Default value

        [Column("status")]
        public string Status { get; set; } = "Not Created"; // Default value

        [Column("action")]
        public string Action { get; set; } = "Draft"; // Default value

        [Column("BusA")]
        public int? BusA { get; set; }

        [Column("TglKirimBarang")]
        public DateOnly? TglKirimBarang { get; set; }

        [Column("TempatTukarFaktur")]
        public string? TempatTukarFaktur { get; set; }

        [Column("TgKirimBerkasKeKAMT")]
        public DateOnly? TgKirimBerkasKeKAMT { get; set; }

        [Column("TglTerimaDOBack")]
        public DateOnly? TglTerimaDOBack { get; set; }

        [Column("TglTerimaFakturPajak")]
        public DateOnly? TglTerimaFakturPajak { get; set; }

        [Column("TglCompletedDoc")]
        public DateOnly? TglCompletedDoc { get; set; }

        [Column("TglTukarFaktur")]
        public DateOnly? TglTukarFaktur { get; set; }

        [Column("tanggalBayar")]
        public DateOnly? TanggalBayar { get; set; }

        [Column("TglTerimaBerkas")]
        public DateOnly? TglTerimaBerkas { get; set; }

        [Column("IdealTukarFaktur")]
        public int? IdealTukarFaktur { get; set; }

        [Column("TOPOutlet")]
        public int? TOPOutlet { get; set; }

        [Column("OverdueDatabaseSAP")]
        public string? OverdueDatabaseSAP { get; set; }

        [Column("StatusOverdueDatabase")]
        public string? StatusOverdueDatabase { get; set; }

        [Column("StatusInternalSAP")]
        public string? StatusInternalSAP { get; set; }

        [Column("StatusExternalTOPOutlet")]
        public string? StatusExternalTOPOutlet { get; set; }

        [Column("keterangan")]
        public string? Keterangan { get; set; }

        [Column("actionCabangMT")]
        public string? ActionCabangMT { get; set; }

        [Column("realisasiCabangMT")]
        public string? RealisasiCabangMT { get; set; }

        [Column("created_at")]
        public DateOnly CreatedAt { get; set; } = DateOnly.FromDateTime(DateTime.Now); // Default value

        [Column("created_by")]
        public string? CreatedBy { get; set; }

        [Column("updated_at")]
        public DateOnly? UpdatedAt { get; set; }

        [Column("updated_by")]
        public int? UpdatedBy { get; set; }
    }
}