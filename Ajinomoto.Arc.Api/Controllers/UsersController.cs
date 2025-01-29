
namespace Ajinomoto.Arc.Api.Controllers;

using Ajinomoto.Arc.Api.Authorization;
using Ajinomoto.Arc.Api.Services;
using Ajinomoto.Arc.Business.Interfaces;
using Ajinomoto.Arc.Common.DtoModels;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IAuthenticationService _authenticationService;
    private readonly IUserFacade _userFacade;

    public UsersController(IUserService userService, IAuthenticationService authenticationService, IUserFacade userFacade)
    {
        _userService = userService;
        _authenticationService = authenticationService;
        _userFacade = userFacade;
    }

    [AllowAnonymous]
    [HttpPost("[action]")]
    public IActionResult Authenticate(AuthenticateRequest model)
    {
        var response = _authenticationService.Authenticate(model);

        return Ok(response);
    }

    [AllowAnonymous]
    [Route("RequestResetPassword")]
    [HttpGet]
    public async Task<ActionResult> RequestResetPassword(string account)
    {
        var resVal = await _userFacade.RequestResetPassword(account);

        return Ok(resVal);
    }

    [AllowAnonymous]
    [Route("ValidateLinkReset")]
    [HttpGet]
    public async Task<ActionResult> ValidateLinkReset(string token)
    {
        var resVal = await _userFacade.ValidateLinkReset(token);

        return Ok(resVal);
    }

    [AllowAnonymous]
    [Route("ResetPassword")]
    [HttpPost]
    public async Task<ActionResult> ResetPassword(ResetPasswordRequest request)
    {
        var resVal = await _userFacade.ResetPassword(request);

        return Ok(resVal);
    }
}