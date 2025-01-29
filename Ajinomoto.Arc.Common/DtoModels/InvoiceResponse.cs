namespace Ajinomoto.Arc.Common.DtoModels
{
    public class InvoiceResponse
    {
        public string InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public double Amount { get; set; }
    }
}
