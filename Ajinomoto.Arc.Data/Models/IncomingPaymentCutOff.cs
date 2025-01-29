using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ajinomoto.Arc.Data.Models
{
    [Table("incoming_payment_cut_off")]
    public partial class IncomingPaymentCutOff
    {
        [Key]
        [Column("incoming_payment_cut_off_id")]
        public Guid IncomingPaymentCutOffId { get; set; }
        [Column("period")]
        public DateOnly Period { get; set; }
        [Column("start_date")]
        public DateOnly StartDate { get; set; }
        [Column("cut_off_date")]
        public DateOnly CutOffDate { get; set; }
        [Column("created_at", TypeName = "timestamp")]
        public DateTime CreatedAt { get; set; }
        [Column("created_app")]
        [StringLength(100)]
        public string CreatedApp { get; set; } = null!;
        [Column("Created_by")]
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
        public int Revision { get; set; }
    }
}
