using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ajinomoto.Arc.Data.Models
{
    [Keyless]
    public partial class SegmentConfig
    {
        [Column("empty")]
        [StringLength(1)]
        public string Empty { get; set; } = null!;
        [Column("segment_id", TypeName = "int(11)")]
        public int SegmentId { get; set; }
        [Column("segment")]
        [StringLength(50)]
        public string Segment { get; set; } = null!;
        [Column("comparation1")]
        [StringLength(50)]
        public string? Comparation1 { get; set; }
        [Column("comparation2")]
        [StringLength(50)]
        public string? Comparation2 { get; set; }
        [Column("template upload", TypeName = "int(11)")]
        public int? TemplateUpload { get; set; }
    }
}
