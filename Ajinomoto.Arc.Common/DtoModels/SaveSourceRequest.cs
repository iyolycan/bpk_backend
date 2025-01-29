namespace Ajinomoto.Arc.Common.DtoModels
{
    public class SaveSourceRequest
    {
        public int? SourceId { get; set; }
        public string SourceName { get; set; }
        public string BankChargeSubAccount { get; set; }
        public bool IsActive { get; set; }
    }
}
