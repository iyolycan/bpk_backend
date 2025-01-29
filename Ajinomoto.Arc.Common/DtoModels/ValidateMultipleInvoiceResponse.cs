namespace Ajinomoto.Arc.Common.DtoModels
{
    public class ValidateMultipleInvoiceResponse
    {
        public List<string> ValidationMessage { get; set; }
        public List<ValidateMultipleInvoiceDetailResponse> Data { get; set; }
    }

    public class ValidateMultipleInvoiceDetailResponse
    {
        public string InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public double Amount { get; set; }
        public bool IsNotExist { get; set; }
        public bool IsUsed { get; set; }
    }
}
