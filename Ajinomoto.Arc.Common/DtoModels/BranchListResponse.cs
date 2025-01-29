using Ajinomoto.Arc.Common.AppModels;

namespace Ajinomoto.Arc.Common.DtoModels
{
    public class BranchListResponse
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
        public string Filter { get; set; }
        public PagedList<BranchDto> Items { get; set; }
    }

    public class BranchDto
    {
        public int BranchId { get; set; }
        public string BranchName { get; set; }
        public string BusinessArea { get; set; }
        public string ChargePoCostCenter { get; set; }
        public bool IsActive { get; set; }
    }

}
