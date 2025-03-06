namespace Ajinomoto.Arc.Common.DtoModels
{
    public class UserResponse
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
        public string? RoleInvoice { get; set; }
        public int? ApprovalId { get; set; }
        public string? ApprovalName { get; set; }
        public string? ApprovalEmail { get; set; }
        public bool IsActive { get; set; }
        public bool IsSetUserArea { get; set; }
        public bool IsSetUserBranch { get; set; }
        public List<string> AreaIds { get; set; }
        public List<string> AreaTexts { get; set; }
        public List<string> BranchIds { get; set; }
        public List<string> BranchTexts { get; set; }
    }
}
