using Ajinomoto.Arc.Common.AppModels;
using Ajinomoto.Arc.Common.DtoModels;

namespace Ajinomoto.Arc.Business.Interfaces
{
    public interface IInvoiceService
    {
        Task<InvoiceResponse> GetInvoice(string invoiceNumber);
        Task<ValidateMultipleInvoiceResponse> ValidateMultipleInvoices(List<string> invoices);

        Task<ResultBase> UpdateBaseLine(UpdateBaseLineRequest request);
    }
}
