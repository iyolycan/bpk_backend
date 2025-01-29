using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ajinomoto.Arc.Data.Models
{
    [Table("kpi_property")]
    [Index(nameof(Name), Name = "name", IsUnique = true)]
    public partial class KpiProperty
    {
        public KpiProperty()
        {
            SegmentKpiPropertyCurrents = new HashSet<Segment>();
            SegmentKpiPropertyTotals = new HashSet<Segment>();
        }

        [Key]
        [Column("kpi_property_id", TypeName = "int(11)")]
        public int KpiPropertyId { get; set; }
        [Column("name")]
        [StringLength(50)]
        public string Name { get; set; } = null!;

        [InverseProperty(nameof(Segment.KpiPropertyCurrent))]
        public virtual ICollection<Segment> SegmentKpiPropertyCurrents { get; set; }
        [InverseProperty(nameof(Segment.KpiPropertyTotal))]
        public virtual ICollection<Segment> SegmentKpiPropertyTotals { get; set; }
    }
}
