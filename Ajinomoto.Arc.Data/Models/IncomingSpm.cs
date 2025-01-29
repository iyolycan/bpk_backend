using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ajinomoto.Arc.Data.Models
{
    [Keyless]
    public partial class IncomingSpm
    {
        [Column("NO")]
        [StringLength(0)]
        public string No { get; set; } = null!;
        [Column("incoming_payment_id")]
        public Guid IncomingPaymentId { get; set; }
        [Column("interface_number")]
        [StringLength(30)]
        public string InterfaceNumber { get; set; } = null!;
        [Column("segment_id", TypeName = "int(11)")]
        public int SegmentId { get; set; }
        [Column("segment_name")]
        [StringLength(50)]
        public string SegmentName { get; set; } = null!;
        [Column("area_id", TypeName = "int(11)")]
        public int AreaId { get; set; }
        [Column("area_name")]
        [StringLength(50)]
        public string AreaName { get; set; } = null!;
        [Column("branch_id", TypeName = "int(11)")]
        public int BranchId { get; set; }
        [Column("branch_name")]
        [StringLength(50)]
        public string BranchName { get; set; } = null!;
        [Column("pic_id", TypeName = "int(11)")]
        public int PicId { get; set; }
        [Column("pic_name")]
        [StringLength(50)]
        public string PicName { get; set; } = null!;
        [Column("bpk_id")]
        public Guid? BpkId { get; set; }
        [Column("bpk_number")]
        [StringLength(120)]
        public string? BpkNumber { get; set; }
        [Column("bpk_status_id", TypeName = "int(11)")]
        public int? BpkStatusId { get; set; }
        [Column("bpk_status")]
        [StringLength(50)]
        public string? BpkStatus { get; set; }
        [Column("clearing_number")]
        [StringLength(30)]
        public string? ClearingNumber { get; set; }
        [Column("is_clearing_manual")]
        public bool IsClearingManual { get; set; }
        [Column("clearing_status_id", TypeName = "int(11)")]
        public int ClearingStatusId { get; set; }
        [Column("clearing_status_name")]
        [StringLength(30)]
        public string ClearingStatusName { get; set; } = null!;
        [Column("clearing_date")]
        public DateOnly? ClearingDate { get; set; }
    }
}
