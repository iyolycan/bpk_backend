namespace Ajinomoto.Arc.Common.DtoModels
{
    public class SaveRoleRequest
    {
        public int? RoleId { get; set; }
        public string RoleName { get; set; }
        public int DataLevelId { get; set; }
        public bool IsSetOnSpecificBranch { get; set; }
        public List<int> BranchIds { get; set; }
        public bool IsSetOnSpecificArea { get; set; }
        public List<int> AreaIds { get; set; }
    }
}
