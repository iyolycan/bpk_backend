using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ajinomoto.Arc.Data.Models
{
    [Keyless]
    public partial class UserView
    {
        [Column("no")]
        [StringLength(0)]
        public string No { get; set; } = null!;
        [Column("app_user_id", TypeName = "int(11)")]
        public int AppUserId { get; set; }
        [Column("username")]
        [StringLength(50)]
        public string Username { get; set; } = null!;
        [Column("full_name")]
        [StringLength(50)]
        public string FullName { get; set; } = null!;
        [Column("email")]
        [StringLength(50)]
        public string Email { get; set; } = null!;
        [Column("role_id", TypeName = "int(11)")]
        public int RoleId { get; set; }

        [Column("role_invoice")]
        [StringLength(10)]
        public string? RoleInvoice { get; set; } = null!;
        
        [Column("approval_id", TypeName = "int(11)")]
        public int? ApprovalId { get; set; } = null!;

        [Column("approval_name")]
        [StringLength(50)]
        public string? ApprovalName { get; set; } = null!;
        
        [Column("approval_email")]
        [StringLength(50)]
        public string? ApprovalEmail { get; set; } = null!;

        [Column("is_active")]
        public bool IsActive { get; set; }
        [Column("role_name")]
        [StringLength(50)]
        public string RoleName { get; set; } = null!;
        [Column("area_ids", TypeName = "mediumtext")]
        public string? AreaIds { get; set; }
        [Column("areas", TypeName = "mediumtext")]
        public string? Areas { get; set; }
        [Column("branch_ids", TypeName = "mediumtext")]
        public string? BranchIds { get; set; }
        [Column("branches", TypeName = "mediumtext")]
        public string? Branches { get; set; }
    }
}
