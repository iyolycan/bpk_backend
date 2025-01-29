namespace Ajinomoto.Arc.Common.DtoModels
{
    public  class RejectBpkRequest
    {
        public Guid IncomingPaymentId { get; set; }
        public string Reason { get; set; }
    }
}
