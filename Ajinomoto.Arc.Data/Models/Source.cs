using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ajinomoto.Arc.Data.Models
{
    [Table("source")]
    [Index(nameof(Name), Name = "name", IsUnique = true)]
    public partial class Source
    {
        public Source()
        {
            IncomingPaymentNonSpms = new HashSet<IncomingPaymentNonSpm>();
            IncomingPayments = new HashSet<IncomingPayment>();
        }

        [Key]
        [Column("source_id", TypeName = "int(11)")]
        public int SourceId { get; set; }
        [Column("name")]
        [StringLength(50)]
        public string Name { get; set; } = null!;
        [Column("bank_charge_sub_account")]
        [StringLength(30)]
        public string? BankChargeSubAccount { get; set; }
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
        public int Revision { get; set; }

        [InverseProperty(nameof(IncomingPaymentNonSpm.Source))]
        public virtual ICollection<IncomingPaymentNonSpm> IncomingPaymentNonSpms { get; set; }
        [InverseProperty(nameof(IncomingPayment.Source))]
        public virtual ICollection<IncomingPayment> IncomingPayments { get; set; }
    }
}
