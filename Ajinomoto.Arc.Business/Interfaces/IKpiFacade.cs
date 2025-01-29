using Ajinomoto.Arc.Common.AppModels;
using Ajinomoto.Arc.Common.DtoModels;

namespace Ajinomoto.Arc.Business.Interfaces
{
    public interface IKpiFacade
    {
        Task<ResultBase<List<KpiDataResponse>>> GetKpiData(string period);
    }
}
