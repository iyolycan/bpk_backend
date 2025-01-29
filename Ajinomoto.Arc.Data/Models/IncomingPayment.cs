using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ajinomoto.Arc.Data.Models
{
    [Table("incoming_payment")]
    [Index(nameof(AreaId), Name = "area_id")]
    [Index(nameof(BpkId), Name = "bpk_id")]
    [Index(nameof(ClearingStatusId), Name = "clearing_status_id")]
    [Index(nameof(PicId), Name = "pic_id")]
    [Index(nameof(SegmentId), Name = "segment_id")]
    [Index(nameof(SourceId), Name = "source_id")]
    public partial class IncomingPayment
    {
        [Key]
        [Column("incoming_payment_id")]
        public Guid IncomingPaymentId { get; set; }
        [Column("bpk_id")]
        public Guid? BpkId { get; set; }
        [Column("interface_number")]
        [StringLength(30)]
        public string InterfaceNumber { get; set; } = null!;
        [Column("segment_id", TypeName = "int(11)")]
        public int SegmentId { get; set; }
        [Column("area_id", TypeName = "int(11)")]
        public int AreaId { get; set; }
        [Column("pic_id", TypeName = "int(11)")]
        public int PicId { get; set; }
        [Column("source_id", TypeName = "int(11)")]
        public int SourceId { get; set; }
        [Column("customer_code")]
        [StringLength(50)]
        public string CustomerCode { get; set; } = null!;
        [Column("billing_number")]
        [StringLength(100)]
        public string? BillingNumber { get; set; }
        [Column("payment_date")]
        public DateOnly PaymentDate { get; set; }
        [Column("posting_date")]
        public DateOnly PostingDate { get; set; }
        [Column("virtual_account")]
        [StringLength(100)]
        public string VirtualAccount { get; set; } = null!;
        [Column("amount")]
        public double Amount { get; set; }
        [Column("clearing_number")]
        [StringLength(30)]
        public string? ClearingNumber { get; set; }
        [Column("clearing_status_id", TypeName = "int(11)")]
        public int ClearingStatusId { get; set; }
        [Column("is_clearing_manual")]
        public bool IsClearingManual { get; set; }
        [Column("clearing_date")]
        public DateOnly? ClearingDate { get; set; }
        [Column("remark")]
        [StringLength(150)]
        public string? Remark { get; set; }
        [Column("bpk_email_remind_at", TypeName = "timestamp")]
        public DateTime? BpkEmailRemindAt { get; set; }
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

        [ForeignKey(nameof(AreaId))]
        [InverseProperty("IncomingPayments")]
        public virtual Area Area { get; set; } = null!;
        [ForeignKey(nameof(BpkId))]
        [InverseProperty("IncomingPayments")]
        public virtual Bpk? Bpk { get; set; }
        [ForeignKey(nameof(ClearingStatusId))]
        [InverseProperty("IncomingPayments")]
        public virtual ClearingStatus ClearingStatus { get; set; } = null!;
        [ForeignKey(nameof(PicId))]
        [InverseProperty(nameof(AppUser.IncomingPayments))]
        public virtual AppUser Pic { get; set; } = null!;
        [ForeignKey(nameof(SegmentId))]
        [InverseProperty("IncomingPayments")]
        public virtual Segment Segment { get; set; } = null!;
        [ForeignKey(nameof(SourceId))]
        [InverseProperty("IncomingPayments")]
        public virtual Source Source { get; set; } = null!;
    }
}
