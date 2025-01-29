using Ajinomoto.Arc.Common.AppModels;

namespace Ajinomoto.Arc.Common.DtoModels
{
    public class PotonganTypeListResponse
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
        public string Filter { get; set; }
        public PagedList<PotonganTypeDto> Items { get; set; }
    }

    public class PotonganTypeDto
    {
        public int PotonganTypeId { get; set; }
        public string PotonganTypeName { get; set; }
        public string GlAccount { get; set; }
        public string PostingKey { get; set; }
        public string TaxCode { get; set; }
        public string SubAccount { get; set; }
        public string Material { get; set; }
        public string BusinessArea { get; set; }
        public string CostCentre { get; set; }
        public string TextInSap { get; set; }
        public bool IsActive { get; set; }
    }
}
