using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ajinomoto.Arc.Data.Models
{
    [Table("area")]
    [Index(nameof(BranchId), Name = "branch_id")]
    [Index(nameof(Name), Name = "name", IsUnique = true)]
    public partial class Area
    {
        public Area()
        {
            AppUserAreas = new HashSet<AppUserArea>();
            IncomingPayments = new HashSet<IncomingPayment>();
            RoleAreas = new HashSet<RoleArea>();
        }

        [Key]
        [Column("area_id", TypeName = "int(11)")]
        public int AreaId { get; set; }
        [Column("branch_id", TypeName = "int(11)")]
        public int BranchId { get; set; }
        [Column("name")]
        [StringLength(50)]
        public string Name { get; set; } = null!;
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

        [ForeignKey(nameof(BranchId))]
        [InverseProperty("Areas")]
        public virtual Branch Branch { get; set; } = null!;
        [InverseProperty(nameof(AppUserArea.Area))]
        public virtual ICollection<AppUserArea> AppUserAreas { get; set; }
        [InverseProperty(nameof(IncomingPayment.Area))]
        public virtual ICollection<IncomingPayment> IncomingPayments { get; set; }
        [InverseProperty(nameof(RoleArea.Area))]
        public virtual ICollection<RoleArea> RoleAreas { get; set; }
    }
}
