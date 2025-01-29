using Ajinomoto.Arc.Common.AppModels;
using Ajinomoto.Arc.Common.DtoModels;

namespace Ajinomoto.Arc.Business.Interfaces
{
    public interface IUserFacade
    {
        Task<ResultBase> RequestResetPassword(string account);
        Task<ResultBase> ValidateLinkReset(string token);
        Task<ResultBase> ResetPassword(ResetPasswordRequest request);
    }
}
