using Ajinomoto.Arc.Common.AppModels;

namespace Ajinomoto.Arc.Common.DtoModels
{
    public class SegmentListResponse
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
        public string Filter { get; set; }
        public PagedList<SegmentDto> Items { get; set; }
    }

    public class SegmentDto
    {
        public int SegmentId { get; set; }
        public string SegmentName { get; set; }
        public string TemplateUploadType { get; set; }
        public string KpiPropertyCurrent { get; set; }
        public string KpiPropertyTotal { get; set; }
        public bool HasAmountUsd { get; set; }
        public bool IsActive { get; set; }
    }
}
