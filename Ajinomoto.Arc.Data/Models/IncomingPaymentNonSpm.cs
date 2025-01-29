using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ajinomoto.Arc.Data.Models
{
    [Table("incoming_payment_non_spm")]
    [Index(nameof(PicId), Name = "pic_id")]
    [Index(nameof(SegmentId), Name = "segment_id")]
    [Index(nameof(SourceId), Name = "source_id")]
    public partial class IncomingPaymentNonSpm
    {
        [Key]
        [Column("incoming_payment_non_spm_id")]
        public Guid IncomingPaymentNonSpmId { get; set; }
        [Column("segment_id", TypeName = "int(11)")]
        public int SegmentId { get; set; }
        [Column("source_id", TypeName = "int(11)")]
        public int SourceId { get; set; }
        [Column("pic_id", TypeName = "int(11)")]
        public int PicId { get; set; }
        [Column("payment_date")]
        public DateOnly PaymentDate { get; set; }
        [Column("interface_number")]
        [StringLength(30)]
        public string? InterfaceNumber { get; set; }
        [Column("billing_number")]
        [StringLength(100)]
        public string? BillingNumber { get; set; }
        [Column("arnex_code")]
        [StringLength(100)]
        public string? ArnexCode { get; set; }
        [Column("customer_name")]
        [StringLength(50)]
        public string? CustomerName { get; set; }
        [Column("clearing_number")]
        [StringLength(30)]
        public string? ClearingNumber { get; set; }
        [Column("clearing_date")]
        public DateOnly? ClearingDate { get; set; }
        [Column("upload_date")]
        public DateOnly? UploadDate { get; set; }
        [Column("upload_date_input_date")]
        public DateOnly? UploadDateInputDate { get; set; }
        [Column("invoice_number")]
        [StringLength(50)]
        public string? InvoiceNumber { get; set; }
        [Column("invoice_date")]
        public DateOnly? InvoiceDate { get; set; }
        [Column("amount_idr")]
        public double AmountIdr { get; set; }
        [Column("amount_usd")]
        public double? AmountUsd { get; set; }
        [Column("area_code")]
        [StringLength(50)]
        public string? AreaCode { get; set; }
        [Column("branch_code")]
        [StringLength(50)]
        public string? BranchCode { get; set; }
        [Column("group_code")]
        [StringLength(50)]
        public string? GroupCode { get; set; }
        [Column("customer_code")]
        [StringLength(50)]
        public string? CustomerCode { get; set; }
        [Column("customer_po_number")]
        [StringLength(50)]
        public string? CustomerPoNumber { get; set; }
        [Column("bii_number")]
        [StringLength(50)]
        public string? BiiNumber { get; set; }
        [Column("virtual_account_number")]
        [StringLength(100)]
        public string? VirtualAccountNumber { get; set; }
        [Column("virtual_account_name")]
        [StringLength(50)]
        public string? VirtualAccountName { get; set; }
        [Column("destination")]
        [StringLength(50)]
        public string? Destination { get; set; }
        [Column("remark")]
        [StringLength(150)]
        public string? Remark { get; set; }
        [Column("created_at", TypeName = "timestamp")]
        public DateTime CreatedAt { get; set; }
        [Column("created_app")]
        [StringLength(100)]
        public string CreatedApp { get; set; } = null!;
        [Column("created_by")]
        [StringLength(50)]
        public string CreatedBy { get; set; } = null!;
        [Column("updated_at", TypeName = "timestamp")]
        public DateTime? UpdatedAt { get; set; }
        [Column("updated_app")]
        [StringLength(100)]
        public string? UpdatedApp { get; set; }
        [Column("updated_by")]
        [StringLength(50)]
        public string? UpdatedBy { get; set; }
        [Column("revision", TypeName = "int(11)")]
        public int Revision { get; set; }
        [Column("deleted_flag")]
        public bool DeletedFlag { get; set; }

        [ForeignKey(nameof(PicId))]
        [InverseProperty(nameof(AppUser.IncomingPaymentNonSpms))]
        public virtual AppUser Pic { get; set; } = null!;
        [ForeignKey(nameof(SegmentId))]
        [InverseProperty("IncomingPaymentNonSpms")]
        public virtual Segment Segment { get; set; } = null!;
        [ForeignKey(nameof(SourceId))]
        [InverseProperty("IncomingPaymentNonSpms")]
        public virtual Source Source { get; set; } = null!;
    }
}
