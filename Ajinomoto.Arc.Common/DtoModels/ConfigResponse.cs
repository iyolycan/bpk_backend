namespace Ajinomoto.Arc.Common.DtoModels
{
    public class ConfigGeneralResponse
    {
        public int DaysEmailReminder { get; set; }
        public int DaysReSendEmail { get; set; }
        public bool UsingLimitPembulatan { get; set; }
        public int? LimitPembulatan { get; set; }
        public int MaxExportPaymentList { get; set; }
    }

    public class ConfigArCutOffResponse
    {
        public string YearPeriod { get; set; }
        public List<CutOffDto> Details { get; set; }
    }

    public class CutOffDto
    {
        public string Period { get; set; }
        public string StartDate { get; set; }
        public string CutOffDate { get; set; }
    }
}
