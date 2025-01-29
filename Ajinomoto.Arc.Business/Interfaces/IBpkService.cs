using Ajinomoto.Arc.Common.AppModels;
using Ajinomoto.Arc.Common.DtoModels;
using ClosedXML.Excel;

namespace Ajinomoto.Arc.Business.Interfaces
{
    public interface IBpkService
    {
        Task<XLWorkbook> GenerateBpkReport(Guid incomingPaymentId);
        Task<BpkResponse?> GetBpk(Guid incomingPaymentId);
        Task<ResultBase> OpenClearingBpk(Guid incomingPaymentId);
        Task<ResultBase> RequestForRevisionBpk(Guid incomingPaymentId);
        Task<ResultBase> ReviseApproveBpk(Guid incomingPaymentId);
        Task<ResultBase> ReviseRejectBpk(RejectBpkRequest param);
        Task<ResultBase> SaveBpk(SaveBpkRequest param);
        Task<ResultBase> SendReminder(string executeBy, string executeApp);
        Task<ResultBase> SubmitBpk(Guid incomingPaymentId);
        Task<List<BpkStatusResponse>> GetBpkStatus();
        Task<List<BpkMasterClearingStatusResponse>> GetMasterClearingStatus();
    }
}
