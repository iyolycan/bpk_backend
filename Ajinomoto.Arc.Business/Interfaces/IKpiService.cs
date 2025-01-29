using Ajinomoto.Arc.Common.DtoModels;
using ClosedXML.Excel;

namespace Ajinomoto.Arc.Business.Interfaces
{
    public interface IKpiService
    {
        Task<List<KpiDataResponse>> GetKpiData(string period);
        Task<XLWorkbook> GenerateKpiReport(string period, int? picId, List<int> kpiProperties = null, string labels = null);
    }
}
