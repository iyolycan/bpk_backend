using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ajinomoto.Arc.Data.Models
{
    [Table("bpk_detail")]
    [Index(nameof(BpkId), Name = "bpk_id")]
    [Index(nameof(InvoiceNumber), Name = "invoice_number", IsUnique = true)]
    [Index(nameof(PotonganId), Name = "potongan_id", IsUnique = true)]
    public partial class BpkDetail
    {
        [Key]
        [Column("bpk_detail_id")]
        public Guid BpkDetailId { get; set; }
        [Column("bpk_id")]
        public Guid BpkId { get; set; }
        [Column("invoice_number")]
        [StringLength(30)]
        public string? InvoiceNumber { get; set; }
        [Column("potongan_id")]
        public Guid? PotonganId { get; set; }
        [Column("created_at", TypeName = "timestamp")]
        public DateTime CreatedAt { get; set; }
        [Column("created_app")]
        [StringLength(100)]
        public string CreatedApp { get; set; } = null!;
        [Column("created_by")]
        [StringLength(50)]
        public string CreatedBy { get; set; } = null!;
        [Column("deleted_flag")]
        public bool DeletedFlag { get; set; }

        [ForeignKey(nameof(BpkId))]
        [InverseProperty("BpkDetails")]
        public virtual Bpk Bpk { get; set; } = null!;
        [ForeignKey(nameof(PotonganId))]
        [InverseProperty("BpkDetail")]
        public virtual Potongan? Potongan { get; set; }
    }
}
