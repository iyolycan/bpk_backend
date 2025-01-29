using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ajinomoto.Arc.Data.Models
{
    [Table("branch")]
    [Index(nameof(Name), Name = "name", IsUnique = true)]
    public partial class Branch
    {
        public Branch()
        {
            AppUserBranches = new HashSet<AppUserBranch>();
            Areas = new HashSet<Area>();
            Potongans = new HashSet<Potongan>();
            RoleBranches = new HashSet<RoleBranch>();
        }

        [Key]
        [Column("branch_id", TypeName = "int(11)")]
        public int BranchId { get; set; }
        [Column("name")]
        [StringLength(50)]
        public string Name { get; set; } = null!;
        [Column("business_area")]
        [StringLength(30)]
        public string BusinessArea { get; set; } = null!;
        [Column("charge_po_cost_center")]
        [StringLength(50)]
        public string? ChargePoCostCenter { get; set; }
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

        [InverseProperty(nameof(AppUserBranch.Branch))]
        public virtual ICollection<AppUserBranch> AppUserBranches { get; set; }
        [InverseProperty(nameof(Area.Branch))]
        public virtual ICollection<Area> Areas { get; set; }
        [InverseProperty(nameof(Potongan.Branch))]
        public virtual ICollection<Potongan> Potongans { get; set; }
        [InverseProperty(nameof(RoleBranch.Branch))]
        public virtual ICollection<RoleBranch> RoleBranches { get; set; }
    }
}
