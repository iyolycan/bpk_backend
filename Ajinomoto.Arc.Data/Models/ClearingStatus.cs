using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ajinomoto.Arc.Data.Models
{
    [Table("clearing_status")]
    [Index(nameof(Name), Name = "name", IsUnique = true)]
    public partial class ClearingStatus
    {
        public ClearingStatus()
        {
            BpkHistories = new HashSet<BpkHistory>();
            IncomingPayments = new HashSet<IncomingPayment>();
        }

        [Key]
        [Column("clearing_status_id", TypeName = "int(11)")]
        public int ClearingStatusId { get; set; }
        [Column("name")]
        [StringLength(30)]
        public string Name { get; set; } = null!;

        [InverseProperty(nameof(BpkHistory.ClearingStatus))]
        public virtual ICollection<BpkHistory> BpkHistories { get; set; }
        [InverseProperty(nameof(IncomingPayment.ClearingStatus))]
        public virtual ICollection<IncomingPayment> IncomingPayments { get; set; }
    }
}
