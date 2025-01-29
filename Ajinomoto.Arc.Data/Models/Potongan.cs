using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ajinomoto.Arc.Data.Models
{
    [Table("potongan")]
    [Index(nameof(BranchId), Name = "FK_potongan_branch")]
    [Index(nameof(PotonganTypeId), Name = "potongan_type_id")]
    public partial class Potongan
    {
        [Key]
        [Column("potongan_id")]
        public Guid PotonganId { get; set; }
        [Column("potongan_number")]
        [StringLength(50)]
        public string PotonganNumber { get; set; } = null!;
        [Column("number_po_eps")]
        [StringLength(50)]
        public string nomorPoEps { get; set; } = null;
        [Column("potongan_type_id", TypeName = "int(11)")]
        public int PotonganTypeId { get; set; }
        [Column("branch_id", TypeName = "int(11)")]
        public int? BranchId { get; set; }
        [Column("customer_code")]
        [StringLength(50)]
        public string CustomerCode { get; set; } = null!;
        [Column("potongan_date")]
        public DateOnly? PotonganDate { get; set; }
        [Column("amount")]
        public double Amount { get; set; }
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

        [ForeignKey(nameof(BranchId))]
        [InverseProperty("Potongans")]
        public virtual Branch? Branch { get; set; }
        [ForeignKey(nameof(PotonganTypeId))]
        [InverseProperty("Potongans")]
        public virtual PotonganType PotonganType { get; set; } = null!;
        [InverseProperty("Potongan")]
        public virtual BpkDetail? BpkDetail { get; set; }
    }
}
