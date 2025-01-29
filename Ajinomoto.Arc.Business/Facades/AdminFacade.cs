using Ajinomoto.Arc.Business.Interfaces;
using Ajinomoto.Arc.Common.AppModels;
using Ajinomoto.Arc.Common.Constants;
using Ajinomoto.Arc.Common.DtoModels;

namespace Ajinomoto.Arc.Business.Facades
{
    public class AdminFacade : IAdminFacade
    {
        private readonly IMasterDataService _masterDataService;
        private readonly IAdminService _adminService;

        public AdminFacade(IMasterDataService masterDataService, IAdminService adminService)
        {
            _masterDataService = masterDataService;
            _adminService = adminService;
        }

        public async Task<ResultBase<AreaResponse>> GetArea(int areaId)
        {
            var result = await _masterDataService.GetArea(areaId);

            if (result != null)
            {
                return new ResultBase<AreaResponse>()
                {
                    Success = true,
                    Message = MessageConstants.S_SUCCESSFULLY,
                    Model = result
                };
            }

            var errorResponse = new ResultBase<AreaResponse>()
            {
                Success = false,
                Message = MessageConstants.S_DATA_NOT_FOUND,
                Model = result
            };

            return errorResponse;
        }

        public async Task<ResultBase<AreaListResponse>> GetAreaList(BasicListRequest request)
        {
            var result = await _masterDataService.GetAreaList(request.Filter, request.Limit, request.Page);

            if (result != null)
            {
                return new ResultBase<AreaListResponse>()
                {
                    Success = true,
                    Message = MessageConstants.S_SUCCESSFULLY,
                    Model = result
                };
            }

            var errorResponse = new ResultBase<AreaListResponse>()
            {
                Success = false,
                Message = MessageConstants.S_DATA_NOT_FOUND,
                Model = result
            };

            return errorResponse;
        }

        public async Task<ResultBase> SaveArea(SaveAreaRequest request)
        {
            var result = await _masterDataService.SaveArea(request);

            return result;
        }

        public async Task<ResultBase> RemoveArea(int areaId)
        {
            var result = await _masterDataService.RemoveArea(areaId);

            return result;
        }

        public async Task<ResultBase<BranchResponse>> GetBranch(int branchId)
        {
            var result = await _masterDataService.GetBranch(branchId);

            if (result != null)
            {
                return new ResultBase<BranchResponse>()
                {
                    Success = true,
                    Message = MessageConstants.S_SUCCESSFULLY,
                    Model = result
                };
            }

            var errorResponse = new ResultBase<BranchResponse>()
            {
                Success = false,
                Message = MessageConstants.S_DATA_NOT_FOUND,
                Model = result
            };

            return errorResponse;
        }

        public async Task<ResultBase<BranchListResponse>> GetBranchList(BasicListRequest request)
        {
            var result = await _masterDataService.GetBranchList(request.Filter, request.Limit, request.Page);

            if (result != null)
            {
                return new ResultBase<BranchListResponse>()
                {
                    Success = true,
                    Message = MessageConstants.S_SUCCESSFULLY,
                    Model = result
                };
            }

            var errorResponse = new ResultBase<BranchListResponse>()
            {
                Success = false,
                Message = MessageConstants.S_DATA_NOT_FOUND,
                Model = result
            };

            return errorResponse;
        }

        public async Task<ResultBase> SaveBranch(SaveBranchRequest request)
        {
            var result = await _masterDataService.SaveBranch(request);

            return result;
        }

        public async Task<ResultBase> RemoveBranch(int branchId)
        {
            var result = await _masterDataService.RemoveBranch(branchId);

            return result;
        }

        public async Task<ResultBase<PotonganTypeResponse>> GetPotonganType(int PotonganTypeId)
        {
            var result = await _masterDataService.GetPotonganType(PotonganTypeId);

            if (result != null)
            {
                return new ResultBase<PotonganTypeResponse>()
                {
                    Success = true,
                    Message = MessageConstants.S_SUCCESSFULLY,
                    Model = result
                };
            }

            var errorResponse = new ResultBase<PotonganTypeResponse>()
            {
                Success = false,
                Message = MessageConstants.S_DATA_NOT_FOUND,
                Model = result
            };

            return errorResponse;
        }

        public async Task<ResultBase<PotonganTypeListResponse>> GetPotonganTypeList(BasicListRequest request)
        {
            var result = await _masterDataService.GetPotonganTypeList(request.Filter, request.Limit, request.Page);

            if (result != null)
            {
                return new ResultBase<PotonganTypeListResponse>()
                {
                    Success = true,
                    Message = MessageConstants.S_SUCCESSFULLY,
                    Model = result
                };
            }

            var errorResponse = new ResultBase<PotonganTypeListResponse>()
            {
                Success = false,
                Message = MessageConstants.S_DATA_NOT_FOUND,
                Model = result
            };

            return errorResponse;
        }

        public async Task<ResultBase> SavePotonganType(SavePotonganTypeRequest request)
        {
            var result = await _masterDataService.SavePotonganType(request);

            return result;
        }

        public async Task<ResultBase> RemovePotonganType(int PotonganTypeId)
        {
            var result = await _masterDataService.RemovePotonganType(PotonganTypeId);

            return result;
        }

        public async Task<ResultBase<RoleResponse>> GetRole(int roleId)
        {
            var result = await _masterDataService.GetRole(roleId);

            if (result != null)
            {
                return new ResultBase<RoleResponse>()
                {
                    Success = true,
                    Message = MessageConstants.S_SUCCESSFULLY,
                    Model = result
                };
            }

            var errorResponse = new ResultBase<RoleResponse>()
            {
                Success = false,
                Message = MessageConstants.S_DATA_NOT_FOUND,
                Model = result
            };

            return errorResponse;
        }

        public async Task<ResultBase<RoleListResponse>> GetRoleList(BasicListRequest request)
        {
            var result = await _masterDataService.GetRoleList(request.Filter, request.Limit, request.Page);

            if (result != null)
            {
                return new ResultBase<RoleListResponse>()
                {
                    Success = true,
                    Message = MessageConstants.S_SUCCESSFULLY,
                    Model = result
                };
            }

            var errorResponse = new ResultBase<RoleListResponse>()
            {
                Success = false,
                Message = MessageConstants.S_DATA_NOT_FOUND,
                Model = result
            };

            return errorResponse;
        }

        public async Task<ResultBase> SaveRole(SaveRoleRequest request)
        {
            var result = await _masterDataService.SaveRole(request);

            return result;
        }

        public async Task<ResultBase> RemoveRole(int roleId)
        {
            var result = await _masterDataService.RemoveRole(roleId);

            return result;
        }

        public async Task<ResultBase<SegmentResponse>> GetSegment(int segmentId)
        {
            var result = await _masterDataService.GetSegment(segmentId);

            if (result != null)
            {
                return new ResultBase<SegmentResponse>()
                {
                    Success = true,
                    Message = MessageConstants.S_SUCCESSFULLY,
                    Model = result
                };
            }

            var errorResponse = new ResultBase<SegmentResponse>()
            {
                Success = false,
                Message = MessageConstants.S_DATA_NOT_FOUND,
                Model = result
            };

            return errorResponse;
        }

        public async Task<ResultBase<SegmentListResponse>> GetSegmentList(BasicListRequest request)
        {
            var result = await _masterDataService.GetSegmentList(request.Filter, request.Limit, request.Page);

            if (result != null)
            {
                return new ResultBase<SegmentListResponse>()
                {
                    Success = true,
                    Message = MessageConstants.S_SUCCESSFULLY,
                    Model = result
                };
            }

            var errorResponse = new ResultBase<SegmentListResponse>()
            {
                Success = false,
                Message = MessageConstants.S_DATA_NOT_FOUND,
                Model = result
            };

            return errorResponse;
        }

        public async Task<ResultBase> SaveSegment(SaveSegmentRequest request)
        {
            var result = await _masterDataService.SaveSegment(request);

            return result;
        }

        public async Task<ResultBase> RemoveSegment(int segmentId)
        {
            var result = await _masterDataService.RemoveSegment(segmentId);

            return result;
        }

        public async Task<ResultBase<SourceResponse>> GetSource(int sourceId)
        {
            var result = await _masterDataService.GetSource(sourceId);

            if (result != null)
            {
                return new ResultBase<SourceResponse>()
                {
                    Success = true,
                    Message = MessageConstants.S_SUCCESSFULLY,
                    Model = result
                };
            }

            var errorResponse = new ResultBase<SourceResponse>()
            {
                Success = false,
                Message = MessageConstants.S_DATA_NOT_FOUND,
                Model = result
            };

            return errorResponse;
        }

        public async Task<ResultBase<SourceListResponse>> GetSourceList(BasicListRequest request)
        {
            var result = await _masterDataService.GetSourceList(request.Filter, request.Limit, request.Page);

            if (result != null)
            {
                return new ResultBase<SourceListResponse>()
                {
                    Success = true,
                    Message = MessageConstants.S_SUCCESSFULLY,
                    Model = result
                };
            }

            var errorResponse = new ResultBase<SourceListResponse>()
            {
                Success = false,
                Message = MessageConstants.S_DATA_NOT_FOUND,
                Model = result
            };

            return errorResponse;
        }

        public async Task<ResultBase> SaveSource(SaveSourceRequest request)
        {
            var result = await _masterDataService.SaveSource(request);

            return result;
        }

        public async Task<ResultBase> RemoveSource(int sourceId)
        {
            var result = await _masterDataService.RemoveSource(sourceId);

            return result;
        }

        public async Task<ResultBase<UserResponse>> GetUser(int UserId)
        {
            var result = await _masterDataService.GetUser(UserId);

            if (result != null)
            {
                return new ResultBase<UserResponse>()
                {
                    Success = true,
                    Message = MessageConstants.S_SUCCESSFULLY,
                    Model = result
                };
            }

            var errorResponse = new ResultBase<UserResponse>()
            {
                Success = false,
                Message = MessageConstants.S_DATA_NOT_FOUND,
                Model = result
            };

            return errorResponse;
        }

        public async Task<ResultBase<UserSetupByRoleResponse>> GetUserSetupByRole(int roleId)
        {
            var result = await _masterDataService.GetUserSetupByRole(roleId);

            if (result != null)
            {
                return new ResultBase<UserSetupByRoleResponse>()
                {
                    Success = true,
                    Message = MessageConstants.S_SUCCESSFULLY,
                    Model = result
                };
            }

            var errorResponse = new ResultBase<UserSetupByRoleResponse>()
            {
                Success = false,
                Message = MessageConstants.S_DATA_NOT_FOUND,
                Model = result
            };

            return errorResponse;
        }

        public async Task<ResultBase<UserListResponse>> GetUserList(BasicListRequest request)
        {
            var result = await _masterDataService.GetUserList(request.Filter, request.Limit, request.Page);

            if (result != null)
            {
                return new ResultBase<UserListResponse>()
                {
                    Success = true,
                    Message = MessageConstants.S_SUCCESSFULLY,
                    Model = result
                };
            }

            var errorResponse = new ResultBase<UserListResponse>()
            {
                Success = false,
                Message = MessageConstants.S_DATA_NOT_FOUND,
                Model = result
            };

            return errorResponse;
        }

        public async Task<ResultBase> SaveUser(SaveUserRequest request)
        {
            var result = await _masterDataService.SaveUser(request);

            return result;
        }

        public async Task<ResultBase> EmailTesting()
        {
            var result = await _adminService.EmailTesting();

            return result;
        }
    }
}
