using Ajinomoto.Arc.Common.AppModels;

namespace Ajinomoto.Arc.Business.Interfaces
{
    public interface IDropdownFacade
    {
        Task<ResultBase<IEnumerable<DropdownDto>>> GetDdlArea(string filter);
        Task<ResultBase<IEnumerable<DropdownDto>>> GetDdlBranch();
        Task<ResultBase<IEnumerable<DropdownDto>>> GetDdlCustomer();
        Task<ResultBase<IEnumerable<DropdownDto>>> GetDdlCustomer(string filter);
        Task<ResultBase<IEnumerable<DropdownDto>>> GetDdlDataLevel();
        Task<ResultBase<IEnumerable<DropdownDto>>> GetDdlInvoice(string filter);
        Task<ResultBase<IEnumerable<DropdownDto>>> GetDdlInvoiceByCustomer(string customerCode);
        Task<ResultBase<IEnumerable<DropdownDto>>> GetDdlKpiProperty();
        Task<ResultBase<IEnumerable<DropdownDto>>> GetDdlKpiPropertyCurrent();
        Task<ResultBase<IEnumerable<DropdownDto>>> GetDdlKpiPropertyTotal();
        Task<ResultBase<IEnumerable<DropdownDto>>> GetDdlPotonganType();
        Task<ResultBase<IEnumerable<DropdownDto>>> GetDdlRole();
        Task<ResultBase<IEnumerable<DropdownDto>>> GetDdlSegment();
        Task<ResultBase<IEnumerable<DropdownDto>>> GetDdlSource();
        Task<ResultBase<IEnumerable<DropdownDto>>> GetDdlTemplateUploadType();
        Task<ResultBase<IEnumerable<DropdownDto>>> GetDdlUser();
        Task<ResultBase<IEnumerable<DropdownDto>>> GetDdlUserCoec();
    }
}
