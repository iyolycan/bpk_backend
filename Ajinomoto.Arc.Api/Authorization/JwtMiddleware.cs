namespace Ajinomoto.Arc.Api.Authorization;

using Ajinomoto.Arc.Business.Interfaces;
using Ajinomoto.Arc.Common.Constants;
using Ajinomoto.Arc.Common.Utils;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IUserService userService, IJwtUtils jwtUtils)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        var userId = jwtUtils.ValidateJwtToken(token);
        if (userId != null)
        {
            // attach user to context on successful jwt validation
            var user = userService.GetById(userId.Value);
            if (user != null)
            {
                user.App = AppSource.S_WEB_APP;
            }

            context.Items["User"] = user;
        }

        await _next(context);
    }
}