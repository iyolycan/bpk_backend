using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ajinomoto.Arc.Data.Models
{
    [Table("app_user_branch")]
    [Index(nameof(AppUserId), Name = "FK__app_user")]
    [Index(nameof(BranchId), Name = "FK__branch")]
    [Index(nameof(AppUserId), nameof(BranchId), Name = "Index 4", IsUnique = true)]
    public partial class AppUserBranch
    {
        [Key]
        [Column("app_user_branch_id")]
        public Guid AppUserBranchId { get; set; }
        [Column("app_user_id", TypeName = "int(11)")]
        public int AppUserId { get; set; }
        [Column("branch_id", TypeName = "int(11)")]
        public int BranchId { get; set; }

        [ForeignKey(nameof(AppUserId))]
        [InverseProperty("AppUserBranches")]
        public virtual AppUser AppUser { get; set; } = null!;
        [ForeignKey(nameof(BranchId))]
        [InverseProperty("AppUserBranches")]
        public virtual Branch Branch { get; set; } = null!;
    }
}
