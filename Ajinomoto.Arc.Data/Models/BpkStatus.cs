using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ajinomoto.Arc.Data.Models
{
    [Table("bpk_status")]
    [Index(nameof(Name), Name = "name", IsUnique = true)]
    public partial class BpkStatus
    {
        public BpkStatus()
        {
            BpkHistories = new HashSet<BpkHistory>();
            Bpks = new HashSet<Bpk>();
        }

        [Key]
        [Column("bpk_status_id", TypeName = "int(11)")]
        public int BpkStatusId { get; set; }
        [Column("name")]
        [StringLength(50)]
        public string Name { get; set; } = null!;

        [InverseProperty(nameof(BpkHistory.BpkStatus))]
        public virtual ICollection<BpkHistory> BpkHistories { get; set; }
        [InverseProperty(nameof(Bpk.BpkStatus))]
        public virtual ICollection<Bpk> Bpks { get; set; }
    }
}
