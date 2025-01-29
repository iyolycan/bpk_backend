using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ajinomoto.Arc.Data.Models
{
    [Table("invoice")]
    [Index(nameof(InvoiceNumber), Name = "invoice_number", IsUnique = true)]
    public partial class Invoice
    {
        [Key]
        [Column("invoice_id")]
        public Guid InvoiceId { get; set; }
        [Column("invoice_number")]
        [StringLength(30)]
        public string InvoiceNumber { get; set; } = null!;
        [Column("invoice_date")]
        public DateOnly InvoiceDate { get; set; }
        [Column("customer_code")]
        [StringLength(50)]
        public string CustomerCode { get; set; } = null!;
        [Column("amount")]
        public double Amount { get; set; }
        [Column("created_at", TypeName = "timestamp")]
        public DateTime CreatedAt { get; set; }
        [Column("created_app")]
        [StringLength(100)]
        public string CreatedApp { get; set; } = null!;
        [Column("created_by")]
        [StringLength(50)]
        public string CreatedBy { get; set; } = null!;
    }
}
