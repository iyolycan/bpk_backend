using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ajinomoto.Arc.Data.Models
{
    [Table("role")]
    [Index(nameof(DataLevelId), Name = "FK_role_data_level")]
    [Index(nameof(Name), Name = "name", IsUnique = true)]
    public partial class Role
    {
        public Role()
        {
            AppUsers = new HashSet<AppUser>();
            RoleAreas = new HashSet<RoleArea>();
            RoleBranches = new HashSet<RoleBranch>();
        }

        [Key]
        [Column("role_id", TypeName = "int(11)")]
        public int RoleId { get; set; }
        [Column("name")]
        [StringLength(50)]
        public string Name { get; set; } = null!;
        [Column("data_level_id", TypeName = "int(11)")]
        public int DataLevelId { get; set; }
        [Column("is_set_on_specific_branch")]
        public bool IsSetOnSpecificBranch { get; set; }
        [Column("is_set_on_specific_area")]
        public bool IsSetOnSpecificArea { get; set; }
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

        [ForeignKey(nameof(DataLevelId))]
        [InverseProperty("Roles")]
        public virtual DataLevel DataLevel { get; set; } = null!;
        [InverseProperty(nameof(AppUser.Role))]
        public virtual ICollection<AppUser> AppUsers { get; set; }
        [InverseProperty(nameof(RoleArea.Role))]
        public virtual ICollection<RoleArea> RoleAreas { get; set; }
        [InverseProperty(nameof(RoleBranch.Role))]
        public virtual ICollection<RoleBranch> RoleBranches { get; set; }
    }
}
