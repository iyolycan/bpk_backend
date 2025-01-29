using Ajinomoto.Arc.Common.AppModels;

namespace Ajinomoto.Arc.Common.DtoModels
{
    public class AreaListResponse
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
        public string Filter { get; set; }
        public PagedList<AreaDto> Items { get; set; }
    }

    public class AreaDto
    {
        public int AreaId { get; set; }
        public string Branch { get; set; }
        public string AreaName { get; set; }
        public bool IsActive { get; set; }
    }
}
