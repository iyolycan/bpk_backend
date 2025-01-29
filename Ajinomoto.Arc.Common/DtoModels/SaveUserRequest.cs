namespace Ajinomoto.Arc.Common.DtoModels
{
    public class SaveUserRequest
    {
        public int? UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
        public List<int> BranchIds { get; set; }
        public List<int> AreaIds { get; set; }
        public bool IsActive { get; set; }
    }
}
