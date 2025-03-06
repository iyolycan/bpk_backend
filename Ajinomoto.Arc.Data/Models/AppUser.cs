using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ajinomoto.Arc.Data.Models
{
    [Table("app_user")]
    [Index(nameof(RoleId), Name = "role_id")]
    [Index(nameof(Username), Name = "username", IsUnique = true)]
    public partial class AppUser
    {
        public AppUser()
        {
            AppUserAreas = new HashSet<AppUserArea>();
            AppUserBranches = new HashSet<AppUserBranch>();
            IncomingPaymentNonSpms = new HashSet<IncomingPaymentNonSpm>();
            IncomingPayments = new HashSet<IncomingPayment>();
        }

        [Key]
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
        [Column("password")]
        [StringLength(60)]
        public string Password { get; set; } = null!;
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
        [Column("deleted_flag")]
        public bool DeletedFlag { get; set; }

        [ForeignKey(nameof(RoleId))]
        [InverseProperty("AppUsers")]
        public virtual Role Role { get; set; } = null!;
        [InverseProperty(nameof(AppUserArea.AppUser))]
        public virtual ICollection<AppUserArea> AppUserAreas { get; set; }
        [InverseProperty(nameof(AppUserBranch.AppUser))]
        public virtual ICollection<AppUserBranch> AppUserBranches { get; set; }
        [InverseProperty(nameof(IncomingPaymentNonSpm.Pic))]
        public virtual ICollection<IncomingPaymentNonSpm> IncomingPaymentNonSpms { get; set; }
        [InverseProperty(nameof(IncomingPayment.Pic))]
        public virtual ICollection<IncomingPayment> IncomingPayments { get; set; }
    }
}
