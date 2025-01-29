using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ajinomoto.Arc.Data.Models
{
    [Table("role_branch")]
    [Index(nameof(BranchId), Name = "FK_role_branch_branch")]
    [Index(nameof(RoleId), Name = "FK_role_branch_role")]
    [Index(nameof(RoleId), nameof(BranchId), Name = "Index 4", IsUnique = true)]
    public partial class RoleBranch
    {
        [Key]
        [Column("role_branch_id")]
        public Guid RoleBranchId { get; set; }
        [Column("role_id", TypeName = "int(11)")]
        public int RoleId { get; set; }
        [Column("branch_id", TypeName = "int(11)")]
        public int BranchId { get; set; }

        [ForeignKey(nameof(BranchId))]
        [InverseProperty("RoleBranches")]
        public virtual Branch Branch { get; set; } = null!;
        [ForeignKey(nameof(RoleId))]
        [InverseProperty("RoleBranches")]
        public virtual Role Role { get; set; } = null!;
    }
}
