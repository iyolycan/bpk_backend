using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ajinomoto.Arc.Data.Models
{
    [Table("app_config")]
    public partial class AppConfig
    {
        [Key]
        [Column("app_config_id", TypeName = "int(11)")]
        public int AppConfigId { get; set; }
        [Column("key")]
        [StringLength(50)]
        public string? Key { get; set; }
        [Column("string_value")]
        [StringLength(50)]
        public string? StringValue { get; set; }
        [Column("int_value", TypeName = "int(11)")]
        public int? IntValue { get; set; }
        [Column("date_value")]
        public DateOnly? DateValue { get; set; }
        [Column("created_at", TypeName = "timestamp")]
        public DateTime CreatedAt { get; set; }
        [Column("created_app")]
        [StringLength(100)]
        public string? CreatedApp { get; set; }
        [Column("created_by")]
        [StringLength(50)]
        public string? CreatedBy { get; set; }
        [Column("updated_at", TypeName = "timestamp")]
        public DateTime? UpdatedAt { get; set; }
        [Column("updated_app")]
        [StringLength(100)]
        public string? UpdatedApp { get; set; }
        [Column("updated_by")]
        [StringLength(50)]
        public string? UpdatedBy { get; set; }
        [Column("revision", TypeName = "int(11)")]
        public int? Revision { get; set; }
    }
}
