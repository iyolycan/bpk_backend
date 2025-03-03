using Ajinomoto.Arc.Common.AppModels;
using Ajinomoto.Arc.Common.DtoModels;
using Ajinomoto.Arc.Common.Enums;
using ClosedXML.Excel;

namespace Ajinomoto.Arc.Business.Interfaces
{
    public interface IIncomingPaymentService
    {
        Task<IncomingPaymentListResponse> GetIncomingPaymentList(string filter,
            IncomingPaymentColumn? sortOrder, IncomingPaymentColumn currentSort,
            SortingDirection? sortDirection, int limit, int page, string? Cabang, string? Customer, string? StatusBpk, string? StatusClearing, string? Area, DateOnly? FromDate, DateOnly? ToDate);
        Task<InvoiceDetailsListResponse> GetInvoiceDetailsList(string filter,
            InvoiceDetailsColumn? sortOrder, InvoiceDetailsColumn currentSort,
            SortingDirection? sortDirection, int limit, int page, string? Cabang, int? Customer, string? StatusTukarFaktur, string? Status, DateOnly? FromDate, DateOnly? ToDate);
        Task<ResultBase> ImportIncomingPayment(ImportIncomingPaymentRequest param);
        Task<ResultBase> ImportInvoice(ImportInvoiceRequest param);
        Task<ResultBase> UpdateBaseLine(UpdateBaseLineRequest param);
        Task<XLWorkbook> GenerateIncomingPaymentListReport(IncomingPaymentListRequest param);
        Task<ResultBase> GenerateSapFile(Guid incomingPaymentId);
        Task<ResultBase> RemoveIncomingPayment(Guid incomingPaymentId);
        Task<ResultBase> RevertResettedBpkFromClearing();
    }
}
