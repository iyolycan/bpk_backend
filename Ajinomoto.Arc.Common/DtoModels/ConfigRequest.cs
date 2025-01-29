namespace Ajinomoto.Arc.Common.DtoModels
{
    public class ConfigRequest
    {
        public string YearPeriod { get; set; }
        public List<CutOffRequestDto> Details { get; set; }
    }

    public class CutOffRequestDto
    {
        public int Month { get; set; }
        public string StartDate { get; set; }
        public string CutOffDate { get; set; }
    }
}
