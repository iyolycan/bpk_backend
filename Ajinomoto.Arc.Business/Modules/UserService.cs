using Ajinomoto.Arc.Business.Interfaces;
using Ajinomoto.Arc.Common.AppModels;
using Ajinomoto.Arc.Common.Constants;
using Ajinomoto.Arc.Common.DtoModels;
using Ajinomoto.Arc.Common.Enums;
using Ajinomoto.Arc.Common.Helpers;
using Ajinomoto.Arc.Common.Utils;
using Ajinomoto.Arc.Data.Models;
using Microsoft.Extensions.Options;
using Serilog;
using System.Net.Mail;

namespace Ajinomoto.Arc.Business.Modules
{
    public class UserService : IUserService
    {
        private readonly IDomainService _domainService;
        private readonly IMasterDataService _masterDataService;
        private readonly IMailService _mailService;
        private readonly IJwtUtils _jwtUtils;
        private readonly AppSettings _appSettings;

        public UserService(IDomainService domainService,
            IMasterDataService masterDataService,
            IOptions<AppSettings> appSettings,
            IMailService mailService, IJwtUtils jwtUtils, 
            IProfileService profileService)
        {
            _domainService = domainService;
            _masterDataService = masterDataService;
            _appSettings = appSettings.Value;
            _mailService = mailService;
            _jwtUtils = jwtUtils;
        }

        public async Task<ResultBase> RequestResetPassword(string account)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = new ResultBase
                    {
                        Success = false,
                        Message = ""
                    };

                    MailAddress addr;
                    AppUser user;
                    if (!AppHelper.TryParseEmail(account, out addr))
                    {
                        // username
                        user = _masterDataService.GetAllActiveAppUser().SingleOrDefault(x =>
                            x.Username.Equals(account));
                        if (user == null)
                        {
                            result.Message = MessageConstants.S_USERNAME_NOT_FOUND;

                            return result;
                        }

                        AppHelper.TryParseEmail(user.Email, out addr);
                    }
                    else
                    {
                        if (addr.Host != _appSettings.AjinomotoDomain)
                        {
                            result.Message = MessageConstants.S_NOT_AJINOMOTO_DOMAIN;

                            return result;
                        }

                        user = _masterDataService.GetAllActiveAppUser().SingleOrDefault(x =>
                            x.Username.Equals(addr.User));
                        if (user == null)
                        {
                            result.Message = MessageConstants.S_EMAIL_NOT_FOUND;

                            return result;
                        }
                    }

                    var mailRequest = new MailRequest
                    {
                        ToEmail = new List<string>(),
                        Cc = new List<string>(),
                        Subject = EmailConstants.S_RESET_PASSWORD_EMAIL_SUBJECT,
                    };

                    mailRequest.ToEmail.Add(addr.Address);

                    var token = _jwtUtils.GenerateJwtTokenResetPassword(user);
                    var link = _appSettings.WebUrl + "/" + ConfigConstants.S_RESET_PASSWORD_MODULE +
                        "?code=" + token;

                    mailRequest.Body = string.Format(EmailConstants.S_RESET_PASSWORD_EMAIL_BODY,
                           user.FullName, link);

                    _mailService.SendEmailAsync(mailRequest);
                    Log.Logger.Information($"Method: ForgotPassword(), email: {addr.Address}");

                    result.Success = true;
                    result.Message = MessageConstants.S_EMAIL_SENT;

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: ForgotPassword(), email: {account}" +
                        $"Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<ResultBase> ResetPassword(string token, string newPassword)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = new ResultBase
                    {
                        Success = false,
                        Message = ""
                    };

                    var username = _jwtUtils.GetClaimsUsername(token);
                    if (string.IsNullOrEmpty(username))
                    {
                        result.Message = MessageConstants.S_INVALID_LINK;

                        return result;
                    }

                    var user = _masterDataService.GetAllActiveAppUser().SingleOrDefault(x =>
                        x.Username.Equals(username));

                    if(user == null)
                    {
                        result.Message = MessageConstants.S_USERNAME_NOT_FOUND_INACTIVE;

                        return result;
                    }

                    if (!_jwtUtils.IsValidJwtTokenResetPassword(token, user.Password))
                    {
                        result.Message = MessageConstants.S_INVALID_LINK;

                        return result;
                    }

                    var now = DateTime.Now;
                    var currentUser = AppInternalUser.S_SYSTEM_USER;
                    var currentApp = AppSource.S_WEB_APP;

                    user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword); ;

                    user.UpdatedAt = now;
                    user.UpdatedApp = currentApp;
                    user.UpdatedBy = currentUser;
                    user.Revision += 1;

                    _domainService.UpdateAppUser(user);
                    _domainService.SaveChanges();

                    result.Success = true;
                    result.Message = MessageConstants.S_PASSWORD_CHANGES;

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: ResetPassword(), token: {token}" +
                        $"Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<ResultBase> ValidateLinkReset(string token)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = new ResultBase
                    {
                        Success = false,
                        Message = ""
                    };

                    var username = _jwtUtils.GetClaimsUsername(token);
                    if (string.IsNullOrEmpty(username))
                    {
                        result.Message = MessageConstants.S_INVALID_LINK;

                        return result;
                    }

                    var user = _masterDataService.GetAllActiveAppUser().SingleOrDefault(x =>
                        x.Username.Equals(username));

                    if (!_jwtUtils.IsValidJwtTokenResetPassword(token, user.Password))
                    {
                        result.Message = MessageConstants.S_INVALID_LINK;

                        return result;
                    }

                    result.Success = true;
                    result.Message = MessageConstants.S_VALID_LINK;

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: ValidateLinkReset(), token: {token}" +
                        $"Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public User? GetById(int id)
        {
            var user = (from a in _masterDataService.GetAllActiveAppUser()
                        where a.AppUserId == id
                        select new User
                        {
                            Id = a.AppUserId,
                            FullName = a.FullName,
                            Username = a.Username,
                            Email = a.Email,
                            RoleId = a.RoleId,
                            RoleInvoice = a.RoleInvoice,
                            ApprovalId = a.ApprovalId,
                            ApprovalName = a.ApprovalName,
                            ApprovalEmail = a.ApprovalEmail,
                        }).SingleOrDefault();

            if (user != null)
            {
                user.AreaIds = GetUserAreaIds(user);
            }

            return user;
        }

        public User? GetUser(string username)
        {
            var result = (from a in _domainService.GetAllAppUser()
                          join b in _domainService.GetAllRole() on a.RoleId equals b.RoleId
                          where a.Username == username
                          select new User
                          {
                              Id = a.AppUserId,
                              FullName = a.FullName,
                              Username = a.Username,
                              RoleId = a.RoleId,
                              DataLevelId = b.DataLevelId,
                              PasswordHash = a.Password,
                              RoleInvoice = a.RoleInvoice,
                            ApprovalId = a.ApprovalId,
                            ApprovalName = a.ApprovalName,
                            ApprovalEmail = a.ApprovalEmail,
                          }).SingleOrDefault();

            return result;
        }

        public List<int> GetUserAreaIds(User user)
        {
            var result = new List<int>();

            var role = _domainService.GetAllRole().Single(x => x.RoleId == user.RoleId);
            switch (role.DataLevelId)
            {
                case (int)DataLevelEnum.AllData:
                    result = _masterDataService.GetAllActiveArea().Select(x => x.AreaId).ToList();

                    break;
                case (int)DataLevelEnum.BranchLevel:
                    var branchIds = new List<int>();
                    if (role.IsSetOnSpecificBranch)
                    {
                        branchIds = _domainService.GetAllRoleBranch()
                            .Where(x => x.RoleId == role.RoleId)
                            .Select(x => x.BranchId)
                            .ToList();
                    }
                    else
                    {
                        branchIds = _domainService.GetAllAppUserBranch()
                            .Where(x => x.AppUserId == user.Id)
                            .Select(x => x.BranchId)
                            .ToList();
                    }

                    result = _domainService.GetAllArea()
                        .Where(x => branchIds.Contains(x.BranchId))
                        .Select(x => x.AreaId)
                        .ToList();

                    break;
                case (int)DataLevelEnum.AreaLevel:
                    if (role.IsSetOnSpecificArea)
                    {
                        result = _domainService.GetAllRoleArea()
                            .Where(x => x.RoleId == role.RoleId)
                            .Select(x => x.AreaId)
                            .ToList();
                    }
                    else
                    {
                        result = _domainService.GetAllAppUserArea()
                            .Where(x => x.AppUserId == user.Id)
                            .Select(x => x.AreaId)
                            .ToList();
                    }

                    break;
            }

            return result;
        }

        public List<string> GetPicAreaEmails(int areaId)
        {
            try
            {
                var result = new List<string>();

                var roleAreaIds = _domainService.GetAllRoleArea()
                    .Where(x => x.AreaId == areaId)
                    .Select(x => x.RoleId)
                    .Distinct()
                    .ToList();

                var picByRole = _masterDataService.GetAllActiveAppUser()
                    .Where(x => roleAreaIds.Contains(x.RoleId))
                    .ToList();

                var picByAssignment = from a in _masterDataService.GetAllActiveAppUser()
                             join b in _domainService.GetAllAppUserArea() on a.AppUserId equals b.AppUserId
                             where b.AreaId == areaId
                             select a;

                result = picByRole.Union(picByAssignment).Select(x => x.Email).ToList();

                return result;
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: GetPicAreaEmails(), areaId: {areaId}" +
                    $"Message: {ex.Message}");
                throw;
            }
        }
    }
}
