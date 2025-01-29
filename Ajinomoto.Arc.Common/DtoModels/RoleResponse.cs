namespace Ajinomoto.Arc.Common.DtoModels
{
    public class RoleResponse
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public int DataLevelId { get; set; }
        public bool IsSetOnSpecificBranch { get; set; }
        public List<string> BranchIds { get; set; }
        public List<string> BranchTexts { get; set; }
        public bool IsSetOnSpecificArea { get; set; }
        public List<string> AreaIds { get; set; }
        public List<string> AreaTexts { get; set; }
    }
}
