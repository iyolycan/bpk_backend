using Ajinomoto.Arc.Common.DtoModels;
using ClosedXML.Excel;

namespace Ajinomoto.Arc.Business.Interfaces
{
    public interface IReportFacade
    {
        Task<XLWorkbook> GenerateBpkReport(Guid incomingPaymentId);
        Task<XLWorkbook> GenerateIncomingPaymentListReport(IncomingPaymentListRequest param);
        Task<XLWorkbook> GenerateKpiReport(string period, int? picId);
    }
}
