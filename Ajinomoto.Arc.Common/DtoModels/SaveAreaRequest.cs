namespace Ajinomoto.Arc.Common.DtoModels
{
    public class SaveAreaRequest
    {
        public int? AreaId { get; set; }
        public int BranchId { get; set; }
        public string AreaName { get; set; }
        public bool IsActive { get; set; }
    }
}
