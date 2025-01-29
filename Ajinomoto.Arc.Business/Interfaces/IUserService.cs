using Ajinomoto.Arc.Common.AppModels;

namespace Ajinomoto.Arc.Business.Interfaces
{
    public interface IUserService
    {
        Task<ResultBase> RequestResetPassword(string account);
        Task<ResultBase> ResetPassword(string token, string newPassword);
        Task<ResultBase> ValidateLinkReset(string token);
        User? GetUser(string username);
        User? GetById(int id);
        List<int> GetUserAreaIds(User user);
        List<string> GetPicAreaEmails(int areaId);
    }
}
