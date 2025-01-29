using Ajinomoto.Arc.Common.AppModels;

namespace Ajinomoto.Arc.Common.DtoModels
{
    public class SourceListResponse
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
        public string Filter { get; set; }
        public PagedList<SourceDto> Items { get; set; }
    }

    public class SourceDto
    {
        public int SourceId { get; set; }
        public string SourceName { get; set; }
        public string BankChargeSubAccount { get; set; }
        public bool IsActive { get; set; }
    }
}
