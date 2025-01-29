namespace Ajinomoto.Arc.Common.DtoModels
{
    public class SaveBranchRequest
    {
        public int? BranchId { get; set; }
        public string BranchName { get; set; }
        public string BusinessArea { get; set; }
        public string ChargePoCostCenter { get; set; }
        public bool IsActive { get; set; }
    }
}
