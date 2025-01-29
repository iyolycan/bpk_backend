namespace Ajinomoto.Arc.Common.DtoModels
{
    public class SaveSegmentRequest
    {
        public int? SegmentId { get; set; }
        public string SegmentName { get; set; }
        public int TemplateUploadTypeId { get; set; }
        public int KpiPropertyCurrentId { get; set; }
        public int KpiPropertyTotalId { get; set; }
        public bool HasAmountUsd { get; set; }
        public bool IsActive { get; set; }
    }
}
