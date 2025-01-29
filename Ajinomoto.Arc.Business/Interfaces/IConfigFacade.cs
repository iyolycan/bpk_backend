using Ajinomoto.Arc.Common.AppModels;
using Ajinomoto.Arc.Common.DtoModels;

namespace Ajinomoto.Arc.Business.Interfaces
{
    public interface IConfigFacade
    {
        Task<ResultBase<ConfigGeneralResponse>> GetAppConfig();
        Task<ResultBase> UpdateAppConfig(UpdateAppConfigRequest request);
        Task<ResultBase<ConfigArCutOffResponse>> GetIncomingPaymentCutOff(string YearPeriod);
        Task<ResultBase> CreateIncomingPaymentCutOff(string YearPeriod);
        Task<ResultBase> UpdateIncomingPaymentCutOff(ConfigRequest configRequest);
    }
}
