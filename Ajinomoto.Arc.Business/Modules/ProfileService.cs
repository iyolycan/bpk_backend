using Ajinomoto.Arc.Business.Interfaces;
using Ajinomoto.Arc.Common.AppModels;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Ajinomoto.Arc.Business.Modules
{
    public class ProfileService : IProfileService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProfileService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public User GetUserLogin()
        {
            try
            {
                var user = (User)_httpContextAccessor.HttpContext.Items["User"];
                if (user == null)
                {
                    throw new Exception("Unauthorized");
                }

                Console.WriteLine("email cek: " + user?.Email);
                return user;
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: GetUserLogin(), Message: {ex.Message}");
                throw;
            }
        }

    }
}
