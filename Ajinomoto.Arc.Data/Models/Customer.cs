using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ajinomoto.Arc.Data.Models
{
    [Table("customer")]
    [Index(nameof(CustomerCode), Name = "customer_code", IsUnique = true)]
    public partial class Customer
    {
        [Key]
        [Column("customer_id")]
        public Guid CustomerId { get; set; }
        [Column("customer_code")]
        [StringLength(50)]
        public string CustomerCode { get; set; } = null!;
        [Column("name")]
        [StringLength(50)]
        public string Name { get; set; } = null!;
        [Column("created_at", TypeName = "timestamp")]
        public DateTime CreatedAt { get; set; }
        [Column("created_app")]
        [StringLength(100)]
        public string CreatedApp { get; set; } = null!;
        [Column("created_by")]
        [StringLength(50)]
        [MySqlCharSet("utf8mb3")]
        [MySqlCollation("utf8mb3_general_ci")]
        public string CreatedBy { get; set; } = null!;
    }
}
