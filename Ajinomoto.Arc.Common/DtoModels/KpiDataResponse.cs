namespace Ajinomoto.Arc.Common.DtoModels
{
    public class KpiDataResponse
    {
        public string Period { get; set; }
        public string SegmentName { get; set; }
        public int BranchId { get; set; }
        public string KpiProperties { get; set; }
        public int KpiPropertyCurrentId { get; set; }
        public int KpiPropertyTotalId { get; set; }
        public bool HasAmountUsd { get; set; }
        public List<KpiDataSegmentPicResponse> SegmentPics { get; set; }
    }

    public class KpiDataSegmentPicResponse
    {
        public int PicId { get; set; }
        public string PicName { get; set; }
        public double TotalAmountIdr { get; set; }
        public double TotalAmountUsd { get; set; }
        public List<KpiDataDetailResponse> Details { get; set; }
        public KpiDataDetailResponse TotalTransaction { get; set; }
        public List<ChartResponse> Charts { get; set; }
    }

    public class ChartResponse
    {
        public string Legend { get; set; }
        public double Value { get; set; }
    }

    public class KpiDataDetailResponse
    {
        public string Source { get; set; }
        public int ArTransaction { get; set; }
        public int BpkReceived { get; set; }
        public int ClearingAr { get; set; }
        public int UploadInvoice { get; set; }
        public int CreateInvoice { get; set; }
        public string Achievement { get; set; }
    }

    public class KpiDataModel
    {
        public int BranchId { get; set; }
        public string BranchName { get; set; }
        public int PicId { get; set; }
        public string PicName { get; set; }
        public int SourceId { get; set; }
        public string SourceName { get; set; }
        public int ArTransaction { get; set; }
        public int BpkReceived { get; set; }
        public int ClearingAr { get; set; }
        public int UploadInvoice { get; set; }
        public int CreateInvoice { get; set; }
        public double AmountIdr { get; set; }
        public double AmountUsd { get; set; }
    }
}
