using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ajinomoto.Arc.Data.Models
{
    [Keyless]
    public partial class KpiSummary
    {
        [Column("periode")]
        [StringLength(7)]
        public string? Periode { get; set; }
        [Column("segment_id", TypeName = "int(11)")]
        public int SegmentId { get; set; }
        [Column("segment_name")]
        [StringLength(50)]
        public string? SegmentName { get; set; }
        [Column("branch_id", TypeName = "int(11)")]
        public int? BranchId { get; set; }
        [Column("branch_name")]
        [StringLength(50)]
        public string? BranchName { get; set; }
        [Column("pic_id", TypeName = "int(11)")]
        public int? PicId { get; set; }
        [Column("pic_name")]
        [StringLength(50)]
        public string? PicName { get; set; }
        [Column("source_name")]
        [StringLength(50)]
        public string? SourceName { get; set; }
        [Column("ar_transaction", TypeName = "bigint(21)")]
        public long ArTransaction { get; set; }
        [Column("bpk_received")]
        [Precision(22, 0)]
        public decimal? BpkReceived { get; set; }
        [Column("clearing_ar")]
        [Precision(22, 0)]
        public decimal? ClearingAr { get; set; }
        [Column("upload_invoice", TypeName = "int(1)")]
        public int UploadInvoice { get; set; }
        [Column("create_invoice", TypeName = "int(1)")]
        public int CreateInvoice { get; set; }
        [Column("amount")]
        public double? Amount { get; set; }
    }
}
