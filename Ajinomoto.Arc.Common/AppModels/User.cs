
namespace Ajinomoto.Arc.Common.AppModels;
using Newtonsoft.Json;

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public int RoleId { get; set; }
    public string? RoleInvoice { get; set; }
    public int? ApprovalId { get; set; }
    public string? ApprovalName { get; set; }
    public string? ApprovalEmail { get; set; }
    public int DataLevelId { get; set; }
    public List<int> AreaIds { get; set; }

    [JsonIgnore]
    public string App { get; set; }
    public string PasswordHash { get; set; }
}