
using Ajinomoto.Arc.Common.AppModels;

namespace Ajinomoto.Arc.Common.DtoModels;

public class AuthenticateResponse
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Username { get; set; }
    public int RoleId { get; set; }
    public int DataLevelId { get; set; }
    public string Token { get; set; }

    public AuthenticateResponse(User user, string token)
    {
        FullName = user.FullName;
        Username = user.Username;
        RoleId = user.RoleId;
        DataLevelId = user.DataLevelId;
        Token = token;
    }
}