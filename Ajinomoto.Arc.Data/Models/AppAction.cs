using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ajinomoto.Arc.Data.Models
{
    [Table("app_action")]
    public partial class AppAction
    {
        public AppAction()
        {
            BpkHistories = new HashSet<BpkHistory>();
        }

        [Key]
        [Column("app_action_id", TypeName = "int(11)")]
        public int AppActionId { get; set; }
        [Column("name")]
        [StringLength(50)]
        public string Name { get; set; } = null!;

        [InverseProperty(nameof(BpkHistory.AppAction))]
        public virtual ICollection<BpkHistory> BpkHistories { get; set; }
    }
}
