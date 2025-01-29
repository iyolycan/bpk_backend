using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ajinomoto.Arc.Data.Models
{
    [Table("app_user_area")]
    [Index(nameof(AppUserId), nameof(AreaId), Name = "Index 4", IsUnique = true)]
    [Index(nameof(AppUserId), Name = "app_user_id")]
    [Index(nameof(AreaId), Name = "area_id")]
    public partial class AppUserArea
    {
        [Key]
        [Column("app_user_area_id")]
        public Guid AppUserAreaId { get; set; }
        [Column("app_user_id", TypeName = "int(11)")]
        public int AppUserId { get; set; }
        [Column("area_id", TypeName = "int(11)")]
        public int AreaId { get; set; }

        [ForeignKey(nameof(AppUserId))]
        [InverseProperty("AppUserAreas")]
        public virtual AppUser AppUser { get; set; } = null!;
        [ForeignKey(nameof(AreaId))]
        [InverseProperty("AppUserAreas")]
        public virtual Area Area { get; set; } = null!;
    }
}
