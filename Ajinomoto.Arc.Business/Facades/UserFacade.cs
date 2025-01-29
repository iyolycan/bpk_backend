using Ajinomoto.Arc.Business.Interfaces;
using Ajinomoto.Arc.Common.AppModels;
using Ajinomoto.Arc.Common.Constants;
using Ajinomoto.Arc.Common.DtoModels;
using Serilog;

namespace Ajinomoto.Arc.Business.Facades
{
    public class UserFacade : IUserFacade
    {
        private readonly IUserService _userService;

        public UserFacade(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<ResultBase> RequestResetPassword(string account)
        {
            var result = await _userService.RequestResetPassword(account);

            Log.Logger.Error($"Method: ForgotPassword(), email: {account}, " +
                        $"result: {result}");

            return new ResultBase
            {
                Success = true,
                Message = MessageConstants.S_EMAIL_FORGOT_PASSWORD
            };
        }

        public async Task<ResultBase> ResetPassword(ResetPasswordRequest request)
        {
            var result = await _userService.ResetPassword(request.Token, request.NewPassword);

            return result;
        }

        public async Task<ResultBase> ValidateLinkReset(string token)
        {
            var result = await _userService.ValidateLinkReset(token);

            return result;
        }
    }
}
