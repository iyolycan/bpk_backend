using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ajinomoto.Arc.Data.Models
{
    [Table("bpk_history")]
    [Index(nameof(AppActionId), Name = "FK_bpk_history_app_action")]
    [Index(nameof(ClearingStatusId), Name = "FK_bpk_history_clearing_status")]
    [Index(nameof(BpkId), Name = "bpk_id")]
    [Index(nameof(BpkStatusId), Name = "bpk_status_id")]
    public partial class BpkHistory
    {
        [Key]
        [Column("bpk_history_id")]
        public Guid BpkHistoryId { get; set; }
        [Column("bpk_id")]
        public Guid BpkId { get; set; }
        [Column("bpk_status_id", TypeName = "int(11)")]
        public int BpkStatusId { get; set; }
        [Column("clearing_status_id", TypeName = "int(11)")]
        public int ClearingStatusId { get; set; }
        [Column("app_action_id", TypeName = "int(11)")]
        public int AppActionId { get; set; }
        [Column("action_by")]
        [StringLength(50)]
        public string ActionBy { get; set; } = null!;
        [Column("action_at", TypeName = "timestamp")]
        public DateTime ActionAt { get; set; }
        [Column("created_at", TypeName = "timestamp")]
        public DateTime CreatedAt { get; set; }
        [Column("created_app")]
        [StringLength(100)]
        public string CreatedApp { get; set; } = null!;
        [Column("created_by")]
        [StringLength(50)]
        public string CreatedBy { get; set; } = null!;

        [ForeignKey(nameof(AppActionId))]
        [InverseProperty("BpkHistories")]
        public virtual AppAction AppAction { get; set; } = null!;
        [ForeignKey(nameof(BpkId))]
        [InverseProperty("BpkHistories")]
        public virtual Bpk Bpk { get; set; } = null!;
        [ForeignKey(nameof(BpkStatusId))]
        [InverseProperty("BpkHistories")]
        public virtual BpkStatus BpkStatus { get; set; } = null!;
        [ForeignKey(nameof(ClearingStatusId))]
        [InverseProperty("BpkHistories")]
        public virtual ClearingStatus ClearingStatus { get; set; } = null!;
    }
}
