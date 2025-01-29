using Ajinomoto.Arc.Common.AppModels;
using Ajinomoto.Arc.Common.DtoModels;

namespace Ajinomoto.Arc.Business.Interfaces
{
    public interface IBpkFacade
    {
        Task<ResultBase> SaveBpk(SaveBpkRequest param);
        Task<ResultBase> SubmitBpk(SubmitBpkRequest param);
        Task<ResultBase> OpenClearingBpk(Guid incomingPaymentId);
        Task<ResultBase> RequestForRevisionBpk(Guid incomingPaymentId);
        Task<ResultBase> ReviseApproveBpk(Guid incomingPaymentId);
        Task<ResultBase> ReviseRejectBpk(RejectBpkRequest param);
        Task<ResultBase> SendReminder(string executeBy, string executeApp);
        Task<ResultBase<BpkResponse>> GetBpk(Guid incomingPaymentId);
        Task<ResultBase<InvoiceResponse>> GetInvoice(string invoiceNumber);
        Task<ResultBase<ValidateMultipleInvoiceResponse>> ValidateMultipleInvoices(string invoices);
        Task<List<BpkStatusResponse>> GetBpkStatusesAsync();
        Task<List<BpkMasterClearingStatusResponse>> GetMasterClearingStatusAsync();
    }
}
