using Ajinomoto.Arc.Business.Interfaces;
using Ajinomoto.Arc.Common.AppModels;
using Ajinomoto.Arc.Common.Constants;
using Ajinomoto.Arc.Common.DtoModels;

namespace Ajinomoto.Arc.Business.Facades
{
    public class KpiFacade : IKpiFacade
    {
        private readonly IKpiService _kpiService;

        public KpiFacade(IKpiService kpiService)
        {
            _kpiService = kpiService;
        }

        public async Task<ResultBase<List<KpiDataResponse>>> GetKpiData(string period)
        {
            var result = await _kpiService.GetKpiData(period);

            if (result != null)
            {
                return new ResultBase<List<KpiDataResponse>>()
                {
                    Success = true,
                    Message = MessageConstants.S_SUCCESSFULLY,
                    Model = result
                };
            }

            var errorResponse = new ResultBase<List<KpiDataResponse>>()
            {
                Success = false,
                Message = MessageConstants.S_DATA_NOT_FOUND,
                Model = result
            };

            return errorResponse;
        }
    }
}
