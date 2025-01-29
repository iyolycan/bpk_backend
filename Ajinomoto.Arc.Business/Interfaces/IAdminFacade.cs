using Ajinomoto.Arc.Common.AppModels;
using Ajinomoto.Arc.Common.DtoModels;

namespace Ajinomoto.Arc.Business.Interfaces
{
    public interface IAdminFacade
    {
        Task<ResultBase<AreaResponse>> GetArea(int areaId);
        Task<ResultBase<AreaListResponse>> GetAreaList(BasicListRequest request);
        Task<ResultBase> SaveArea(SaveAreaRequest request);
        Task<ResultBase> RemoveArea(int areaId);
        Task<ResultBase<BranchResponse>> GetBranch(int branchId);
        Task<ResultBase<BranchListResponse>> GetBranchList(BasicListRequest request);
        Task<ResultBase> SaveBranch(SaveBranchRequest request);
        Task<ResultBase> RemoveBranch(int branchId);
        Task<ResultBase<RoleResponse>> GetRole(int roleId);
        Task<ResultBase<RoleListResponse>> GetRoleList(BasicListRequest request);
        Task<ResultBase> SaveRole(SaveRoleRequest request);
        Task<ResultBase> RemoveRole(int RoleId);
        Task<ResultBase<PotonganTypeResponse>> GetPotonganType(int potonganTypeId);
        Task<ResultBase<PotonganTypeListResponse>> GetPotonganTypeList(BasicListRequest request);
        Task<ResultBase> SavePotonganType(SavePotonganTypeRequest request);
        Task<ResultBase> RemovePotonganType(int potonganTypeId);
        Task<ResultBase<SegmentResponse>> GetSegment(int segmentId);
        Task<ResultBase<SegmentListResponse>> GetSegmentList(BasicListRequest request);
        Task<ResultBase> SaveSegment(SaveSegmentRequest request);
        Task<ResultBase> RemoveSegment(int segmentId);
        Task<ResultBase<SourceResponse>> GetSource(int sourceId);
        Task<ResultBase<SourceListResponse>> GetSourceList(BasicListRequest request);
        Task<ResultBase> SaveSource(SaveSourceRequest request);
        Task<ResultBase> RemoveSource(int sourceId);
        Task<ResultBase<UserResponse>> GetUser(int UserId);
        Task<ResultBase<UserSetupByRoleResponse>> GetUserSetupByRole(int roleId);
        Task<ResultBase<UserListResponse>> GetUserList(BasicListRequest request);
        Task<ResultBase> SaveUser(SaveUserRequest request);
        Task<ResultBase> EmailTesting();
    }
}
