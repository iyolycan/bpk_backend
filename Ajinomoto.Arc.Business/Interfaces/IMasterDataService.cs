using Ajinomoto.Arc.Common.AppModels;
using Ajinomoto.Arc.Common.DtoModels;
using Ajinomoto.Arc.Data.Models;

namespace Ajinomoto.Arc.Business.Interfaces
{
    public interface IMasterDataService
    {
        Task<IEnumerable<DropdownDto>> GetDdlArea(string filter);
        Task<IEnumerable<DropdownDto>> GetDdlBranch();
        Task<IEnumerable<DropdownDto>> GetDdlCustomer();
        Task<IEnumerable<DropdownDto>> GetDdlCustomer(string filter);
        Task<IEnumerable<DropdownDto>> GetDdlDataLevel();
        Task<IEnumerable<DropdownDto>> GetDdlInvoice(string filter);
        Task<IEnumerable<DropdownDto>> GetDdlInvoiceByCustomer(string customerCode);
        Task<IEnumerable<DropdownDto>> GetDdlKpiProperty();
        Task<IEnumerable<DropdownDto>> GetDdlPotonganType();
        Task<IEnumerable<DropdownDto>> GetDdlRole();
        Task<IEnumerable<DropdownDto>> GetDdlSegment();
        Task<IEnumerable<DropdownDto>> GetDdlSource();
        Task<IEnumerable<DropdownDto>> GetDdlTemplateUploadType();
        Task<IEnumerable<DropdownDto>> GetDdlUser();
        Task<IEnumerable<DropdownDto>> GetDdlUserCoec();
        IQueryable<AppUser> GetAllActiveAppUser();
        IQueryable<Area> GetAllActiveArea();
        IQueryable<BpkDetail> GetAllActiveBpkDetail();
        IQueryable<Branch> GetAllActiveBranch();
        IQueryable<PotonganType> GetAllActivePotonganType();
        IQueryable<Source> GetAllActiveSource();
        IQueryable<Segment> GetAllActiveSegment();
        Task<AreaResponse?> GetArea(int areaId);
        Task<AreaListResponse> GetAreaList(string filter, int limit, int page);
        Task<ResultBase> SaveArea(SaveAreaRequest model);
        Task<ResultBase> RemoveArea(int areaId);
        Task<BranchResponse?> GetBranch(int branchId);
        Task<BranchListResponse> GetBranchList(string filter, int limit, int page);
        Task<ResultBase> SaveBranch(SaveBranchRequest model);
        Task<ResultBase> RemoveBranch(int branchId);
        Task<PotonganTypeResponse?> GetPotonganType(int potonganTypeId);
        Task<PotonganTypeListResponse> GetPotonganTypeList(string filter, int limit, int page);
        Task<ResultBase> SavePotonganType(SavePotonganTypeRequest model);
        Task<ResultBase> RemovePotonganType(int potonganTypeId);
        Task<RoleResponse?> GetRole(int roleId);
        Task<RoleListResponse> GetRoleList(string filter, int limit, int page);
        Task<ResultBase> SaveRole(SaveRoleRequest model);
        Task<ResultBase> RemoveRole(int roleId);
        Task<SegmentResponse?> GetSegment(int segmentId);
        Task<SegmentListResponse> GetSegmentList(string filter, int limit, int page);
        Task<ResultBase> SaveSegment(SaveSegmentRequest model);
        Task<ResultBase> RemoveSegment(int segmentId);
        Task<SourceResponse?> GetSource(int sourceId);
        Task<SourceListResponse> GetSourceList(string filter, int limit, int page);
        Task<ResultBase> SaveSource(SaveSourceRequest model);
        Task<ResultBase> RemoveSource(int sourceId);
        Task<UserResponse?> GetUser(int UserId);
        Task<UserSetupByRoleResponse?> GetUserSetupByRole(int roleId);
        Task<UserListResponse> GetUserList(string filter, int limit, int page);
        Task<ResultBase> SaveUser(SaveUserRequest model);
    }
}
