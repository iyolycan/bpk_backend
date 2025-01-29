using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ajinomoto.Arc.Data.Models
{
    [Keyless]
    [Table("segment_kpi_property")]
    [Index(nameof(KpiPropertyId), Name = "FK_segment_kpi_property_kpi_property")]
    [Index(nameof(SegmentId), nameof(KpiPropertyId), Name = "segment_id", IsUnique = true)]
    public partial class SegmentKpiProperty
    {
        [Column("segment_id", TypeName = "int(11)")]
        public int SegmentId { get; set; }
        [Column("kpi_property_id", TypeName = "int(11)")]
        public int KpiPropertyId { get; set; }

        [ForeignKey(nameof(KpiPropertyId))]
        public virtual KpiProperty KpiProperty { get; set; } = null!;
        [ForeignKey(nameof(SegmentId))]
        public virtual Segment Segment { get; set; } = null!;
    }
}
