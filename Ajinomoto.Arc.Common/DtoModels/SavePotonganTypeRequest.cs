namespace Ajinomoto.Arc.Common.DtoModels
{
    public class SavePotonganTypeRequest
    {
        public int? PotonganTypeId { get; set; }
        public string PotonganTypeName { get; set; }
        public string? GlAccount { get; set; }
        public string? PostingKey { get; set; }
        public string? TaxCode { get; set; }
        public string? SubAccount { get; set; }
        public string? Material { get; set; }
        public string? BusinessArea { get; set; }
        public string? CostCentre { get; set; }
        public string? TextInSap { get; set; }
        public bool IsActive { get; set; }

    }
}
