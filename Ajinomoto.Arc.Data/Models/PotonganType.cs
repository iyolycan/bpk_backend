using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ajinomoto.Arc.Data.Models
{
    [Table("potongan_type")]
    [Index(nameof(Name), Name = "name", IsUnique = true)]
    public partial class PotonganType
    {
        public PotonganType()
        {
            Potongans = new HashSet<Potongan>();
        }

        [Key]
        [Column("potongan_type_id", TypeName = "int(11)")]
        public int PotonganTypeId { get; set; }
        [Column("name")]
        [StringLength(50)]
        public string Name { get; set; } = null!;
        [Column("gl_account")]
        [StringLength(30)]
        public string? GlAccount { get; set; }
        [Column("posting_key")]
        [StringLength(10)]
        public string? PostingKey { get; set; }
        [Column("tax_code")]
        [StringLength(10)]
        public string? TaxCode { get; set; }
        [Column("sub_account")]
        [StringLength(30)]
        public string? SubAccount { get; set; }
        [Column("material")]
        [StringLength(30)]
        public string? Material { get; set; }
        [Column("business_area")]
        [StringLength(10)]
        public string? BusinessArea { get; set; }
        [Column("cost_center")]
        [StringLength(30)]
        public string? CostCenter { get; set; }
        [Column("text_in_sap")]
        [StringLength(50)]
        public string? TextInSap { get; set; }
        [Column("is_active")]
        public bool IsActive { get; set; }
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

        [InverseProperty(nameof(Potongan.PotonganType))]
        public virtual ICollection<Potongan> Potongans { get; set; }
    }
}
