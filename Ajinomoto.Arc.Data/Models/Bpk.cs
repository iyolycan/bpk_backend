using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ajinomoto.Arc.Data.Models
{
    [Table("bpk")]
    [Index(nameof(BpkStatusId), Name = "bpk_status_id")]
    public partial class Bpk
    {
        public Bpk()
        {
            BpkDetails = new HashSet<BpkDetail>();
            BpkHistories = new HashSet<BpkHistory>();
            IncomingPayments = new HashSet<IncomingPayment>();
        }

        [Key]
        [Column("bpk_id")]
        public Guid BpkId { get; set; }
        [Column("bpk_number")]
        [StringLength(120)]
        public string BpkNumber { get; set; } = null!;
        [Column("rejected_at", TypeName = "timestamp")]
        public DateTime? RejectedAt { get; set; }
        [Column("rejected_by")]
        [StringLength(50)]
        public string? RejectedBy { get; set; }
        [Column("rejected_reason")]
        [StringLength(255)]
        public string? RejectedReason { get; set; }
        [Column("bpk_status_id", TypeName = "int(11)")]
        public int BpkStatusId { get; set; }
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

        [ForeignKey(nameof(BpkStatusId))]
        [InverseProperty("Bpks")]
        public virtual BpkStatus BpkStatus { get; set; } = null!;
        [InverseProperty(nameof(BpkDetail.Bpk))]
        public virtual ICollection<BpkDetail> BpkDetails { get; set; }
        [InverseProperty(nameof(BpkHistory.Bpk))]
        public virtual ICollection<BpkHistory> BpkHistories { get; set; }
        [InverseProperty(nameof(IncomingPayment.Bpk))]
        public virtual ICollection<IncomingPayment> IncomingPayments { get; set; }
    }
}
