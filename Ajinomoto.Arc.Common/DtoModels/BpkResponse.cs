namespace Ajinomoto.Arc.Common.DtoModels
{
    public class BpkResponse
    {
        public string BpkNumber { get; set; }
        public string InterfaceNumber { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Date { get; set; }
        public string PaymentType { get; set; }
        public double AmountReceived { get; set; }
        public string Status { get; set; }
        public int? LimitPembulatan { get; set; }
        public List<BpkResponseInvoice> Invoices { get; set; }
        public List<BpkResponsePotongan> Potongans { get; set; }
        public List<BpkResponseHistory> Histories { get; set; }

    }

    public class BpkResponseInvoice
    {
        public Guid? BpkDetailId { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        public string CustomerName { get; set; }
        public double Amount { get; set; }
    }

    public class BpkResponsePotongan
    {
        public Guid? BpkDetailId { get; set; }
        public Guid? PotonganId { get; set; }
        public int PotonganTypeId { get; set; }
        public string PotonganTypeName { get; set; }
        public int? BranchId { get; set; }
        public string? BranchName { get; set; }
        public string PotonganNumber { get; set; }
        public string nomorPoEps { get; set; }
        public string PotonganDate { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public double Amount { get; set; }
    }

    public class BpkResponseHistory
    {
        public string StatusName { get; set; }
        public string ActionByName { get; set; }
        public string ActionAt { get; set; }
    }

    public class BpkResponseExportSap
    {
        public string InvoiceNumber { get; set; }
        public string CustomerId { get; set; }
        public string BpkNumber { get; set; }
        public string DocumentDate { get; set; }
        public string PostinganDate { get; set; }
        public string HeaderText { get; set; }
        public string ClearingText { get; set; }
        public string ValueDate { get; set; }
        public string BlineDate { get; set; }
        public string Period { get; set; }
        public string Type { get; set; }
        public int TypeId { get; set; }
        public double Amount { get; set; }
        public string OtherCriteria { get; set; }
        public string SelectedArea { get; set; }
        public string InterfaceNumber { get; set; }
        public string GlAccountNumber { get; set; }
        public string PstKey { get; set; }
        public string TaxCode { get; set; }
        public string SubAccount { get; set; }
        public string Material { get; set; }
        public string BusinessArea { get; set; }
        public string CostCentre { get; set; }
        public string TextInSap { get; set; }
        public string AssignmentInSap { get; set; }
        public string NomorPoEps { get; set; }
        public string PotonganNumber { get; set; }
        public string BranchBusinessArea { get; set; }
        public string CostCenterChargePO { get; set; }
    }

    public class BpkStatusResponse
    {
        public int BpkStatusId { get; set; }
        public string? Name { get; set; }
    }

    public class BpkMasterClearingStatusResponse
    {
        public int ClearingStatusId { get; set; }
        public string? Name { get; set; }
    }

}
