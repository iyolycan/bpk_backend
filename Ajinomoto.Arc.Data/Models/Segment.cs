using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ajinomoto.Arc.Data.Models
{
    [Table("segment")]
    [Index(nameof(KpiPropertyTotalId), Name = "kpi_property_current_id")]
    [Index(nameof(KpiPropertyCurrentId), Name = "kpi_property_total_id")]
    [Index(nameof(Name), Name = "name", IsUnique = true)]
    [Index(nameof(TemplateUploadTypeId), Name = "template_upload_type_id")]
    public partial class Segment
    {
        public Segment()
        {
            IncomingPaymentNonSpms = new HashSet<IncomingPaymentNonSpm>();
            IncomingPayments = new HashSet<IncomingPayment>();
        }

        [Key]
        [Column("segment_id", TypeName = "int(11)")]
        public int SegmentId { get; set; }
        [Column("name")]
        [StringLength(50)]
        public string Name { get; set; } = null!;
        [Column("template_upload_type_id", TypeName = "int(11)")]
        public int TemplateUploadTypeId { get; set; }
        [Column("kpi_property_current_id", TypeName = "int(11)")]
        public int KpiPropertyCurrentId { get; set; }
        [Column("kpi_property_total_id", TypeName = "int(11)")]
        public int KpiPropertyTotalId { get; set; }
        [Column("has_amount_usd")]
        public bool HasAmountUsd { get; set; }
        [Column("is_active")]
        public bool IsActive { get; set; }
        [Column("created_at", TypeName = "timestamp")]
        public DateTime CreatedAt { get; set; }
        [Column("created_app")]
        [StringLength(100)]
        public string CreatedApp { get; set; } = null!;
        [Column("created_by")]
        [StringLength(50)]
        public string CreatedBy { get; set; } = null!;
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

        [ForeignKey(nameof(KpiPropertyCurrentId))]
        [InverseProperty(nameof(KpiProperty.SegmentKpiPropertyCurrents))]
        public virtual KpiProperty KpiPropertyCurrent { get; set; } = null!;
        [ForeignKey(nameof(KpiPropertyTotalId))]
        [InverseProperty(nameof(KpiProperty.SegmentKpiPropertyTotals))]
        public virtual KpiProperty KpiPropertyTotal { get; set; } = null!;
        [ForeignKey(nameof(TemplateUploadTypeId))]
        [InverseProperty("Segments")]
        public virtual TemplateUploadType TemplateUploadType { get; set; } = null!;
        [InverseProperty(nameof(IncomingPaymentNonSpm.Segment))]
        public virtual ICollection<IncomingPaymentNonSpm> IncomingPaymentNonSpms { get; set; }
        [InverseProperty(nameof(IncomingPayment.Segment))]
        public virtual ICollection<IncomingPayment> IncomingPayments { get; set; }
    }
}
