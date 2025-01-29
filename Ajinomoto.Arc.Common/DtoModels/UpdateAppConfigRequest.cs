namespace Ajinomoto.Arc.Common.DtoModels
{
    public class UpdateAppConfigRequest
    {
        public int DaysEmailReminder { get; set; }
        public int DaysReSendEmail { get; set; }
        public int? LimitPembulatan { get; set; }
        public int MaxExportPaymentList { get; set; }
    }
}
