using Ajinomoto.Arc.Business.Interfaces;
using Ajinomoto.Arc.Common.AppModels;
using Ajinomoto.Arc.Common.Constants;
using Ajinomoto.Arc.Common.DtoModels;

namespace Ajinomoto.Arc.Business.Facades
{
    public class ConfigFacade : IConfigFacade
    {
        private readonly IConfigService _configService;

        public ConfigFacade(IConfigService configService)
        {
            _configService = configService;
        }

        public async Task<ResultBase<ConfigGeneralResponse>> GetAppConfig()
        {
            var result = await _configService.GetAppConfig();

            if (result != null)
            {
                return new ResultBase<ConfigGeneralResponse>()
                {
                    Success = true,
                    Message = MessageConstants.S_SUCCESSFULLY,
                    Model = result
                };
            }

            var errorResponse = new ResultBase<ConfigGeneralResponse>()
            {
                Success = false,
                Message = MessageConstants.S_DATA_NOT_FOUND,
                Model = result
            };

            return errorResponse;
        }

        public async Task<ResultBase> UpdateAppConfig(UpdateAppConfigRequest request)
        {
            var result = await _configService.UpdateAppConfig(request);

            return result;
        }

        public async Task<ResultBase<ConfigArCutOffResponse>> GetIncomingPaymentCutOff(string YearPeriod)
        {
            var result = await _configService.GetIncomingPaymentCutOff(YearPeriod);

            if (result != null)
            {
                return new ResultBase<ConfigArCutOffResponse>()
                {
                    Success = true,
                    Message = MessageConstants.S_SUCCESSFULLY,
                    Model = result
                };
            }

            var errorResponse = new ResultBase<ConfigArCutOffResponse>()
            {
                Success = false,
                Message = MessageConstants.S_DATA_NOT_FOUND,
                Model = result
            };

            return errorResponse;
        }

        public async Task<ResultBase> CreateIncomingPaymentCutOff(string YearPeriod)
        {
            var result = await _configService.CreateIncomingPaymentCutOff(YearPeriod);

            return result;
        }

        public async Task<ResultBase> UpdateIncomingPaymentCutOff(ConfigRequest configRequest)
        {
            var result = await _configService.UpdateIncomingPaymentCutOff(configRequest);

            return result;
        }
    }
}
