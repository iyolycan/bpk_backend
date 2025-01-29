using Ajinomoto.Arc.Business.Interfaces;
using Ajinomoto.Arc.Common.AppModels;
using Ajinomoto.Arc.Common.Constants;
using Ajinomoto.Arc.Common.DtoModels;
using Ajinomoto.Arc.Common.Enums;
using Ajinomoto.Arc.Data.Models;
using Serilog;
using System.Globalization;

namespace Ajinomoto.Arc.Business.Modules
{
    public class ConfigService : IConfigService
    {
        private readonly IDomainService _domainService;
        private readonly IProfileService _profileService;

        public ConfigService(IDomainService domainService, IProfileService profileService)
        {
            _domainService = domainService;
            _profileService = profileService;
        }

        public async Task<ConfigGeneralResponse?> GetAppConfig()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var configs = _domainService.GetAllAppConfig().ToList();

                    var daysEmailReminder = configs.Single(x => x.AppConfigId == (int)AppConfigEnum.DAYS_TO_BPK_REMINDER);
                    var reSendEmail = configs.Single(x => x.AppConfigId == (int)AppConfigEnum.DAYS_RE_SEND_EMAIL);
                    var maxPembulatan = configs.Single(x => x.AppConfigId == (int)AppConfigEnum.MAX_PEMBULATAN);
                    var maxExportPaymentList = configs.Single(x => x.AppConfigId == (int)AppConfigEnum.MAX_EXPORT_PAYMENT_LIST);

                    var result = new ConfigGeneralResponse
                    {
                        DaysEmailReminder = daysEmailReminder.IntValue.HasValue ?
                            daysEmailReminder.IntValue.Value : 0,
                        DaysReSendEmail = reSendEmail.IntValue.HasValue ?
                            reSendEmail.IntValue.Value : 0,
                        UsingLimitPembulatan = maxPembulatan.IntValue.HasValue,
                        LimitPembulatan = maxPembulatan.IntValue,
                        MaxExportPaymentList = maxExportPaymentList.IntValue.HasValue ?
                            maxExportPaymentList.IntValue.Value : 0
                    };

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: GetConfigGeneral()" +
                        $"Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<ResultBase> UpdateAppConfig(UpdateAppConfigRequest request)
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

                    var userLogin = _profileService.GetUserLogin();

                    var now = DateTime.Now;
                    var currentUser = userLogin.Username;
                    var currentApp = userLogin.App;

                    var paramToUpdate = new List<int>
                    {
                        (int)AppConfigEnum.DAYS_TO_BPK_REMINDER,
                        (int)AppConfigEnum.DAYS_RE_SEND_EMAIL,
                        (int)AppConfigEnum.MAX_PEMBULATAN,
                        (int)AppConfigEnum.MAX_EXPORT_PAYMENT_LIST
                    };

                    var configs = _domainService.GetAllAppConfig()
                        .Where(x => paramToUpdate.Contains(x.AppConfigId))
                        .ToList();

                    foreach (var config in configs)
                    {
                        switch (config.AppConfigId)
                        {
                            case (int)AppConfigEnum.DAYS_TO_BPK_REMINDER:
                                config.IntValue = request.DaysEmailReminder;
                                break;
                            case (int)AppConfigEnum.DAYS_RE_SEND_EMAIL:
                                config.IntValue = request.DaysReSendEmail;
                                break;
                            case (int)AppConfigEnum.MAX_PEMBULATAN:
                                config.IntValue = request.LimitPembulatan;
                                break;
                            case (int)AppConfigEnum.MAX_EXPORT_PAYMENT_LIST:
                                config.IntValue = request.MaxExportPaymentList;
                                break;
                            default:
                                result.Message = MessageConstants.S_CONFIG_NOT_FOUND;
                                return result;
                        }

                        config.UpdatedAt = now;
                        config.UpdatedApp = currentUser;
                        config.UpdatedBy = currentApp;
                        config.Revision += 1;

                        _domainService.UpdateAppConfig(config);
                    }

                    result.Success = true;
                    result.Message = MessageConstants.S_SUCCESSFULLY;

                    _domainService.SaveChanges();

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: UpdateConfigGeneral(), request: {request}" +
                       $"Message: {ex.Message}");
                    throw;
                }

            }).ConfigureAwait(false);
        }

        public async Task<ConfigArCutOffResponse?> GetIncomingPaymentCutOff(string yearPeriod)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var incomingPaymenCutOffs = _domainService.GetAllIncomingPaymentCutOff().Where(x => x.Period.Year.ToString() == yearPeriod).ToList();

                    if (incomingPaymenCutOffs.Count == 0)
                    {
                        return null;
                    }

                    var result = new ConfigArCutOffResponse
                    {
                        YearPeriod = yearPeriod,
                        Details = new List<CutOffDto>()
                    };

                    int year = Convert.ToInt32(yearPeriod);

                    for (int i = 1; i <= 12; i++)
                    {
                        var trx = new CutOffDto
                        {
                            Period = (new DateTime(year, i, 1)).ToString("MMMM", CultureInfo.InvariantCulture),
                        };

                        var cutOff = incomingPaymenCutOffs.Where(x => x.Period.Month == i).FirstOrDefault();

                        if (cutOff == null)
                        {
                            trx.StartDate = new DateOnly(year, i, 1).ToString(ConfigConstants.S_FORMAT_DATE);
                            trx.CutOffDate = new DateOnly(year, i, DateTime.DaysInMonth(year, i)).ToString(ConfigConstants.S_FORMAT_DATE);
                        }
                        else
                        {
                            trx.StartDate = cutOff.StartDate.ToString(ConfigConstants.S_FORMAT_DATE);
                            trx.CutOffDate = cutOff.CutOffDate.ToString(ConfigConstants.S_FORMAT_DATE);
                        }

                        result.Details.Add(trx);
                    }

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: GetConfigArCutOff(), yearPeriod: {yearPeriod}" +
                        $"Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<ResultBase> CreateIncomingPaymentCutOff(string yearPeriod)
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

                    var userLogin = _profileService.GetUserLogin();
                    var now = DateTime.Now;

                    int year = Convert.ToInt32(yearPeriod);
                    for (int i = 1; i <= 12; i++)
                    {
                        var arCutOff = new IncomingPaymentCutOff
                        {
                            IncomingPaymentCutOffId = Guid.NewGuid(),
                            Period = DateOnly.FromDateTime(new DateTime(year, i, 1)),
                            StartDate = new DateOnly(year, i, 1),
                            CutOffDate = new DateOnly(year, i, DateTime.DaysInMonth(year, i)),

                            CreatedAt = now,
                            CreatedApp = userLogin.App,
                            CreatedBy = userLogin.Username,
                            Revision = ConfigConstants.N_INIT_REVISION
                        };

                        _domainService.InsertIncomingPaymentCutOff(arCutOff);
                    }

                    result.Success = true;
                    result.Message = MessageConstants.S_SUCCESSFULLY;

                    _domainService.SaveChanges();

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: CreateArCutOff(), yearPeriod: {yearPeriod}" +
                        $"Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<ResultBase> UpdateIncomingPaymentCutOff(ConfigRequest configRequest)
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

                    var userLogin = _profileService.GetUserLogin();

                    var now = DateTime.Now;
                    var currentUser = userLogin.Username;
                    var currentApp = userLogin.App;

                    var incomingPaymenCutOffs = _domainService.GetAllIncomingPaymentCutOff()
                        .Where(x => x.Period.Year.ToString() == configRequest.YearPeriod).ToList();

                    if (!incomingPaymenCutOffs.Any())
                    {
                        result.Message = MessageConstants.S_PERIOD_CUT_OFF_NOT_FOUND;

                        return result;
                    }

                    int year = Convert.ToInt32(configRequest.YearPeriod);
                    for (int i = 1; i <= 12; i++)
                    {
                        var originCutOff = incomingPaymenCutOffs.SingleOrDefault(x => x.Period.Month == i);
                        var localCutOff = configRequest.Details.SingleOrDefault(x => x.Month == i);
                        if (originCutOff == null)
                        {
                            originCutOff = new IncomingPaymentCutOff
                            {
                                IncomingPaymentCutOffId = Guid.NewGuid(),
                                Period = new DateOnly(year, i, 1),
                                StartDate = localCutOff == null ? new DateOnly(year, i, 1) :
                                    DateOnly.ParseExact(localCutOff.StartDate, ConfigConstants.S_FORMAT_DATE, null),
                                CutOffDate = localCutOff == null ? new DateOnly(year, i, DateTime.DaysInMonth(year, i)) :
                                    DateOnly.ParseExact(localCutOff.CutOffDate, ConfigConstants.S_FORMAT_DATE, null),

                                CreatedApp = currentApp,
                                CreatedAt = now,
                                CreatedBy = currentUser,
                                Revision = ConfigConstants.N_INIT_REVISION
                            };

                            _domainService.InsertIncomingPaymentCutOff(originCutOff);
                        }
                        else
                        {
                            originCutOff.StartDate = localCutOff == null ? new DateOnly(year, i, 1) :
                                DateOnly.ParseExact(localCutOff.StartDate, ConfigConstants.S_FORMAT_DATE, null);
                            originCutOff.CutOffDate = localCutOff == null ? new DateOnly(year, i, DateTime.DaysInMonth(year, i)) :
                                DateOnly.ParseExact(localCutOff.CutOffDate, ConfigConstants.S_FORMAT_DATE, null);

                            originCutOff.UpdatedApp = currentApp;
                            originCutOff.UpdatedAt = now;
                            originCutOff.UpdatedBy = currentUser;
                            originCutOff.Revision += 1;

                            _domainService.UpdateIncomingPaymentCutOff(originCutOff);
                        }
                    }

                    result.Success = true;
                    result.Message = MessageConstants.S_SUCCESSFULLY;

                    _domainService.SaveChanges();

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: UpdateIncomingPaymentCutOff(), configRequest: {configRequest}" +
                        $"Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }
    }
}
