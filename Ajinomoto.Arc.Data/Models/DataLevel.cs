using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ajinomoto.Arc.Data.Models
{
    [Table("data_level")]
    public partial class DataLevel
    {
        public DataLevel()
        {
            Roles = new HashSet<Role>();
        }

        [Key]
        [Column("data_level_id", TypeName = "int(11)")]
        public int DataLevelId { get; set; }
        [Column("name")]
        [StringLength(50)]
        public string Name { get; set; } = null!;

        [InverseProperty(nameof(Role.DataLevel))]
        public virtual ICollection<Role> Roles { get; set; }
    }
}
