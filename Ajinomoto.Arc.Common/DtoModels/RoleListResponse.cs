using Ajinomoto.Arc.Common.AppModels;

namespace Ajinomoto.Arc.Common.DtoModels
{
    public class RoleListResponse
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
        public string Filter { get; set; }
        public PagedList<RoleDto> Items { get; set; }
    }

    public class RoleDto
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string DataLevelName { get; set; }
        public string SetOnSpecificBranch { get; set; }
        public string SetOnSpecificArea { get; set; }
    }
}
