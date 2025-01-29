namespace Ajinomoto.Arc.Common.DtoModels
{
    public class SaveBpkRequest
    {
        public Guid IncomingPaymentId { get; set; }
        public List<SaveBpkRequestInvoice> Invoices { get; set; }
        public List<SaveBpkRequestPotongan> Potongans { get; set; }
    }

    public class SaveBpkRequestInvoice
    {
        public Guid? BpkDetailId { get; set; }
        public string InvoiceNumber { get; set; }
        public bool IsDelete { get; set; }
    }

    public class SaveBpkRequestPotongan
    {
        public Guid? BpkDetailId { get; set; }
        public Guid? PotonganId { get; set; }
        public int? BranchId { get; set; }
        public string PotonganNumber { get; set; }
        public string nomorPoEps { get; set; }
        public string? PotonganDate { get; set; }
        public string CustomerCode { get; set; }
        public double Amount { get; set; }
        public int PotonganTypeId { get; set; }
        public bool IsUpdate { get; set; }
        public bool IsDelete { get; set; }
    }
}
