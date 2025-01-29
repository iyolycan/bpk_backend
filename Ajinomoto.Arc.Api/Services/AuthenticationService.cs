namespace Ajinomoto.Arc.Api.Services;
using Ajinomoto.Arc.Api.Helpers;
using Ajinomoto.Arc.Business.Interfaces;
using Ajinomoto.Arc.Common.DtoModels;
using Ajinomoto.Arc.Common.Helpers;
using Ajinomoto.Arc.Common.Utils;
using BCrypt.Net;
using Microsoft.Extensions.Options;

public interface IAuthenticationService
{
    AuthenticateResponse Authenticate(AuthenticateRequest model);
}

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserService _userService;
    private IJwtUtils _jwtUtils;
    private readonly AppSettings _appSettings;

    public AuthenticationService(
        IJwtUtils jwtUtils,
        IOptions<AppSettings> appSettings,
        IUserService userService)
    {
        _jwtUtils = jwtUtils;
        _appSettings = appSettings.Value;
        _userService = userService;
    }


    public AuthenticateResponse Authenticate(AuthenticateRequest model)
    {
        var user = _userService.GetUser(model.Username);

        // validate
        if (user == null || !BCrypt.Verify(model.Password, user.PasswordHash))
            throw new AppException("Username or password is incorrect");

        // authentication successful so generate jwt token
        var jwtToken = _jwtUtils.GenerateJwtToken(user);

        return new AuthenticateResponse(user, jwtToken);
    }
}