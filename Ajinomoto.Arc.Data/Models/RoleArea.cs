using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ajinomoto.Arc.Data.Models
{
    [Table("role_area")]
    [Index(nameof(AreaId), Name = "FK__area")]
    [Index(nameof(RoleId), Name = "FK__role")]
    [Index(nameof(RoleId), nameof(AreaId), Name = "Index 4", IsUnique = true)]
    public partial class RoleArea
    {
        [Key]
        [Column("role_area_id")]
        public Guid RoleAreaId { get; set; }
        [Column("role_id", TypeName = "int(11)")]
        public int RoleId { get; set; }
        [Column("area_id", TypeName = "int(11)")]
        public int AreaId { get; set; }

        [ForeignKey(nameof(AreaId))]
        [InverseProperty("RoleAreas")]
        public virtual Area Area { get; set; } = null!;
        [ForeignKey(nameof(RoleId))]
        [InverseProperty("RoleAreas")]
        public virtual Role Role { get; set; } = null!;
    }
}
