using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ajinomoto.Arc.Data.Models
{
    [Keyless]
    public partial class IncomingPaymentView
    {
        [Column("no")]
        [StringLength(0)]
        public string No { get; set; } = null!;
        [Column("incoming_payment_id")]
        public Guid IncomingPaymentId { get; set; }
        [Column("interface_number")]
        [StringLength(30)]
        public string InterfaceNumber { get; set; } = null!;
        [Column("payment_date")]
        public DateOnly PaymentDate { get; set; }
        [Column("payment_date_string")]
        [StringLength(10)]
        public string? PaymentDateString { get; set; }
        [Column("amount")]
        public double Amount { get; set; }
        [Column("amount_string")]
        [StringLength(414)]
        public string? AmountString { get; set; }
        [Column("customer_code")]
        [StringLength(50)]
        public string CustomerCode { get; set; } = null!;
        [Column("customer_name")]
        [StringLength(50)]
        public string CustomerName { get; set; } = null!;
        [Column("area_id", TypeName = "int(11)")]
        public int AreaId { get; set; }
        [Column("area_name")]
        [StringLength(50)]
        public string AreaName { get; set; } = null!;
        [Column("branch_name")]
        [StringLength(50)]
        public string BranchName { get; set; } = null!;
        [Column("bpk_id")]
        public Guid? BpkId { get; set; }
        [Column("bpk_number")]
        [StringLength(120)]
        public string? BpkNumber { get; set; }
        [Column("bpk_status_id", TypeName = "int(11)")]
        public int? BpkStatusId { get; set; }
        [Column("bpk_status")]
        [StringLength(50)]
        public string? BpkStatus { get; set; }
        [Column("clearing_number")]
        [StringLength(30)]
        public string? ClearingNumber { get; set; }
        [Column("clearing_status_id", TypeName = "int(11)")]
        public int ClearingStatusId { get; set; }
        [Column("clearing_status")]
        [StringLength(30)]
        public string ClearingStatus { get; set; } = null!;
        [Column("is_clearing_manual")]
        public bool IsClearingManual { get; set; }
        [Column("clearing_date")]
        public DateOnly? ClearingDate { get; set; }
    }
}
