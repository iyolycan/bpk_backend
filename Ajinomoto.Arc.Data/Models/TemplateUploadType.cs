using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ajinomoto.Arc.Data.Models
{
    [Table("template_upload_type")]
    [Index(nameof(Name), Name = "name", IsUnique = true)]
    public partial class TemplateUploadType
    {
        public TemplateUploadType()
        {
            Segments = new HashSet<Segment>();
        }

        [Key]
        [Column("template_upload_type_id", TypeName = "int(11)")]
        public int TemplateUploadTypeId { get; set; }
        [Column("name")]
        [StringLength(50)]
        public string Name { get; set; } = null!;
        [Column("template_file_name")]
        [StringLength(50)]
        public string TemplateFileName { get; set; } = null!;

        [InverseProperty(nameof(Segment.TemplateUploadType))]
        public virtual ICollection<Segment> Segments { get; set; }
    }
}
