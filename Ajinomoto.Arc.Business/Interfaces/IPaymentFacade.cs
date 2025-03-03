using Ajinomoto.Arc.Common.AppModels;
using Ajinomoto.Arc.Common.DtoModels;

namespace Ajinomoto.Arc.Business.Interfaces
{
    public interface IPaymentFacade
    {
        Task<ResultBase<IncomingPaymentListResponse>> GetIncomingPaymentList(IncomingPaymentListRequest param);
        Task<ResultBase<InvoiceDetailsListResponse>> GetInvoiceDetailsList(InvoiceDetailsRequest param);
        Task<ResultBase> ImportIncomingPayment(ImportIncomingPaymentRequest param);
        Task<ResultBase> UpdateBaseLine(UpdateBaseLineRequest param);
        Task<ResultBase> ImportInvoice(ImportInvoiceRequest param);
        Task<ResultBase> RemoveIncomingPayment(Guid incomingPaymentId);
        Task<ResultBase> RevertResettedBpkFromClearing();
    }
}
