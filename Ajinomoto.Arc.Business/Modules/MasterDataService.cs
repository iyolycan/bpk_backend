using Ajinomoto.Arc.Business.Interfaces;
using Ajinomoto.Arc.Common.AppModels;
using Ajinomoto.Arc.Common.Constants;
using Ajinomoto.Arc.Common.DtoModels;
using Ajinomoto.Arc.Common.Enums;
using Ajinomoto.Arc.Common.Helpers;
using Ajinomoto.Arc.Data.Models;
using Microsoft.Extensions.Options;
using Serilog;
using System.Net.Mail;

namespace Ajinomoto.Arc.Business.Modules
{
    public partial class MasterDataService : IMasterDataService
    {
        private readonly IDomainService _domainService;
        private readonly IProfileService _profileService;
        private readonly AppSettings _appSettings;

        public MasterDataService(IDomainService domainService,
            IOptions<AppSettings> appSettings,
            IProfileService profileService)
        {
            _domainService = domainService;
            _appSettings = appSettings.Value;
            _profileService = profileService;
        }

        public async Task<AreaResponse?> GetArea(int areaId)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = new AreaResponse();

                    var data = from a in _domainService.GetAllArea()
                               where a.AreaId == areaId
                               select new AreaResponse
                               {
                                   AreaId = a.AreaId,
                                   BranchId = a.BranchId,
                                   AreaName = a.Name,
                                   IsActive = a.IsActive
                               };

                    return data.SingleOrDefault();
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: GetArea(), " +
                        $"branchId: {areaId}, Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<AreaListResponse> GetAreaList(string filter, int limit, int page)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = new AreaListResponse();

                    var query = from a in _domainService.GetAllArea()
                                join b in _domainService.GetAllBranch() on a.BranchId equals b.BranchId
                                orderby a.AreaId ascending
                                select new AreaDto
                                {
                                    AreaId = a.AreaId,
                                    Branch = b.Name,
                                    AreaName = a.Name,
                                    IsActive = a.IsActive
                                };

                    if (string.IsNullOrEmpty(filter) == false)
                    {
                        filter = filter.ToUpper();
                        query = query.Where(x =>
                               x.Branch.ToUpper().Contains(filter)
                            || x.AreaName.ToUpper().Contains(filter)
                            );
                    }

                    var data = PagedList<AreaDto>.ToPagedList(query, page, limit);

                    return result = new AreaListResponse
                    {
                        CurrentPage = data.CurrentPage,
                        TotalCount = data.TotalCount,
                        PageSize = data.PageSize,
                        TotalPages = data.TotalPages,
                        Filter = filter,

                        Items = data
                    };
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: GetAreaList(), filter: {filter}, " +
                        $"limit: {limit}, page: {page} Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<ResultBase> SaveArea(SaveAreaRequest model)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = new ResultBase
                    {
                        Success = false,
                        Message = ""
                    };

                    var userLogin = _profileService.GetUserLogin();

                    var now = DateTime.Now;
                    var currentUser = userLogin.Username;
                    var currentApp = userLogin.App;

                    if (model.AreaId == null)
                    {
                        var existName = _domainService.GetAllArea().SingleOrDefault(x =>
                            x.Name.Equals(model.AreaName));
                        if (existName != null)
                        {
                            result.Message = MessageConstants.S_AREA_EXIST;

                            return result;
                        }

                        var nextid = _domainService.GetAllArea().Max(x => x.AreaId) + 1;
                        var newItem = new Area
                        {
                            AreaId = nextid,
                            Name = model.AreaName,
                            BranchId = model.BranchId,
                            IsActive = model.IsActive,

                            CreatedAt = now,
                            CreatedApp = currentApp,
                            CreatedBy = currentUser,
                            UpdatedAt = now,
                            UpdatedApp = currentApp,
                            UpdatedBy = currentUser,
                            Revision = ConfigConstants.N_INIT_REVISION
                        };

                        _domainService.InsertArea(newItem);

                    }
                    else
                    {
                        var found = _domainService.GetAllArea().SingleOrDefault(x => x.AreaId == model.AreaId);
                        if (found == null)
                        {
                            result.Message = MessageConstants.S_AREA_NOT_FOUND;

                            return result;
                        }

                        var existName = _domainService.GetAllArea().SingleOrDefault(x =>
                            x.Name.Equals(model.AreaName)
                            && x.AreaId != found.AreaId);
                        if (existName != null)
                        {
                            result.Message = MessageConstants.S_AREA_EXIST;

                            return result;
                        }

                        found.Name = model.AreaName;
                        found.BranchId = model.BranchId;
                        found.IsActive = model.IsActive;

                        found.UpdatedAt = now;
                        found.UpdatedApp = currentApp;
                        found.UpdatedBy = currentUser;
                        found.Revision += 1;

                        _domainService.UpdateArea(found);
                    }

                    result.Success = true;
                    result.Message = MessageConstants.S_AREA_SAVED;

                    _domainService.SaveChanges();

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: SaveArea(), model: {model}" +
                        $"Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<ResultBase> RemoveArea(int AreaId)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = new ResultBase
                    {
                        Success = false,
                        Message = ""
                    };

                    var Area = _domainService.GetAllArea().SingleOrDefault(x => x.AreaId == AreaId);
                    if (Area == null)
                    {
                        result.Message = MessageConstants.S_AREA_NOT_FOUND;

                        return result;
                    }

                    var hasTransaction = _domainService.GetAllIncomingPayment()
                        .Where(x => x.AreaId == AreaId)
                        .Any();
                    if (hasTransaction)
                    {
                        result.Message = MessageConstants.S_AREA_IS_USED;

                        return result;
                    }

                    _domainService.DeleteArea(Area);

                    result.Success = true;
                    result.Message = MessageConstants.S_AREA_REMOVED;

                    _domainService.SaveChanges();

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: RemoveArea(), AreaId: {AreaId}" +
                        $"Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<BranchResponse?> GetBranch(int branchId)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = new BranchResponse();

                    var data = from a in _domainService.GetAllBranch()
                               where a.BranchId == branchId
                               select new BranchResponse
                               {
                                   BranchId = a.BranchId,
                                   BranchName = a.Name,
                                   BusinessArea = a.BusinessArea,
                                   ChargePoCostCenter = a.ChargePoCostCenter,
                                   IsActive = a.IsActive
                               };

                    return data.SingleOrDefault();
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: GetBranch(), " +
                        $"branchId: {branchId}, Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<BranchListResponse> GetBranchList(string filter, int limit, int page)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = new BranchListResponse();

                    var query = from a in _domainService.GetAllBranch()
                                orderby a.BranchId ascending
                                select new BranchDto
                                {
                                    BranchId = a.BranchId,
                                    BranchName = a.Name,
                                    BusinessArea = a.BusinessArea,
                                    ChargePoCostCenter = a.ChargePoCostCenter,
                                    IsActive = a.IsActive
                                };

                    if (string.IsNullOrEmpty(filter) == false)
                    {
                        filter = filter.ToUpper();
                        query = query.Where(x =>
                               x.BranchName.ToUpper().Contains(filter)
                            || x.BusinessArea.ToUpper().Contains(filter)
                            || x.ChargePoCostCenter.ToUpper().Contains(filter)
                            );
                    }

                    var data = PagedList<BranchDto>.ToPagedList(query, page, limit);

                    return result = new BranchListResponse
                    {
                        CurrentPage = data.CurrentPage,
                        TotalCount = data.TotalCount,
                        PageSize = data.PageSize,
                        TotalPages = data.TotalPages,
                        Filter = filter,

                        Items = data
                    };
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: GetBranchList(), filter: {filter}, " +
                        $"limit: {limit}, page: {page} Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<ResultBase> SaveBranch(SaveBranchRequest model)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = new ResultBase
                    {
                        Success = false,
                        Message = ""
                    };

                    var userLogin = _profileService.GetUserLogin();

                    var now = DateTime.Now;
                    var currentUser = userLogin.Username;
                    var currentApp = userLogin.App;

                    if (model.BranchId == null)
                    {
                        var existName = _domainService.GetAllBranch().SingleOrDefault(x =>
                            x.Name.Equals(model.BranchName));
                        if (existName != null)
                        {
                            result.Message = MessageConstants.S_BRANCH_EXIST;

                            return result;
                        }

                        var nextid = _domainService.GetAllBranch().Max(x => x.BranchId) + 1;
                        var newItem = new Branch
                        {
                            BranchId = nextid,
                            Name = model.BranchName,
                            BusinessArea = model.BusinessArea,
                            ChargePoCostCenter = model.ChargePoCostCenter,
                            IsActive = model.IsActive,

                            CreatedAt = now,
                            CreatedApp = currentApp,
                            CreatedBy = currentUser,
                            UpdatedAt = now,
                            UpdatedApp = currentApp,
                            UpdatedBy = currentUser,
                            Revision = ConfigConstants.N_INIT_REVISION
                        };

                        _domainService.InsertBranch(newItem);

                    }
                    else
                    {
                        var found = _domainService.GetAllBranch().SingleOrDefault(x => x.BranchId == model.BranchId);
                        if (found == null)
                        {
                            result.Message = MessageConstants.S_BRANCH_NOT_FOUND;

                            return result;
                        }

                        var existName = _domainService.GetAllBranch().SingleOrDefault(x =>
                            x.Name.Equals(model.BranchName)
                            && x.BranchId != found.BranchId);
                        if (existName != null)
                        {
                            result.Message = MessageConstants.S_BRANCH_EXIST;

                            return result;
                        }

                        found.Name = model.BranchName;
                        found.BusinessArea = model.BusinessArea;
                        found.ChargePoCostCenter = model.ChargePoCostCenter;
                        found.IsActive = model.IsActive;

                        found.UpdatedAt = now;
                        found.UpdatedApp = currentApp;
                        found.UpdatedBy = currentUser;
                        found.Revision += 1;

                        _domainService.UpdateBranch(found);
                    }

                    result.Success = true;
                    result.Message = MessageConstants.S_BRANCH_SAVED;

                    _domainService.SaveChanges();

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: SaveBranch(), model: {model}" +
                        $"Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<ResultBase> RemoveBranch(int BranchId)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = new ResultBase
                    {
                        Success = false,
                        Message = ""
                    };

                    var Branch = _domainService.GetAllBranch().SingleOrDefault(x => x.BranchId == BranchId);
                    if (Branch == null)
                    {
                        result.Message = MessageConstants.S_BRANCH_NOT_FOUND;

                        return result;
                    }

                    var coreBranchs = new List<int>();
                    foreach (int i in Enum.GetValues(typeof(BranchEnum)))
                    {
                        coreBranchs.Add(i);
                    }

                    var isCoreBranch = coreBranchs.Contains(Branch.BranchId);
                    if (isCoreBranch)
                    {
                        result.Message = MessageConstants.S_BRANCH_IS_CORE;

                        return result;
                    }

                    var hasTransaction = _domainService.GetAllIncomingPayment()
                        .Where(x => x.Area.BranchId == BranchId)
                        .Any();
                    if (hasTransaction)
                    {
                        result.Message = MessageConstants.S_BRANCH_IS_USED;

                        return result;
                    }

                    _domainService.DeleteBranch(Branch);

                    result.Success = true;
                    result.Message = MessageConstants.S_BRANCH_REMOVED;

                    _domainService.SaveChanges();

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: RemoveBranch(), BranchId: {BranchId}" +
                        $"Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<PotonganTypeResponse?> GetPotonganType(int potonganTypeId)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = new PotonganTypeResponse();

                    var data = from a in _domainService.GetAllPotonganType()
                               where a.PotonganTypeId == potonganTypeId
                               select new PotonganTypeResponse
                               {
                                   PotonganTypeId = a.PotonganTypeId,
                                   PotonganTypeName = a.Name,
                                   GlAccount = a.GlAccount,
                                   PostingKey = a.PostingKey,
                                   TaxCode = a.TaxCode,
                                   SubAccount = a.SubAccount,
                                   Material = a.Material,
                                   BusinessArea = a.BusinessArea,
                                   CostCentre = a.CostCenter,
                                   TextInSap = a.TextInSap,
                                   IsActive = a.IsActive
                               };

                    return data.SingleOrDefault();
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: GetPotonganType(), " +
                        $"PotonganTypeId: {potonganTypeId}, Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<PotonganTypeListResponse> GetPotonganTypeList(string filter, int limit, int page)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = new PotonganTypeListResponse();

                    var query = from a in _domainService.GetAllPotonganType()
                                orderby a.PotonganTypeId ascending
                                select new PotonganTypeDto
                                {
                                    PotonganTypeId = a.PotonganTypeId,
                                    PotonganTypeName = a.Name,
                                    GlAccount = a.GlAccount,
                                    PostingKey = a.PostingKey,
                                    TaxCode = a.TaxCode,
                                    SubAccount = a.SubAccount,
                                    Material = a.Material,
                                    BusinessArea = a.BusinessArea,
                                    CostCentre = a.CostCenter,
                                    TextInSap = a.TextInSap,
                                    IsActive = a.IsActive
                                };

                    if (string.IsNullOrEmpty(filter) == false)
                    {
                        filter = filter.ToUpper();
                        query = query.Where(x =>
                               x.PotonganTypeName.ToUpper().Contains(filter)
                            || x.GlAccount.ToUpper().Contains(filter)
                            || x.PostingKey.ToUpper().Contains(filter)
                            || x.TaxCode.ToUpper().Contains(filter)
                            || x.SubAccount.ToUpper().Contains(filter)
                            || x.Material.ToUpper().Contains(filter)
                            || x.BusinessArea.ToUpper().Contains(filter)
                            || x.CostCentre.ToUpper().Contains(filter)
                            || x.TextInSap.ToUpper().Contains(filter)
                            );
                    }

                    var data = PagedList<PotonganTypeDto>.ToPagedList(query, page, limit);

                    return result = new PotonganTypeListResponse
                    {
                        CurrentPage = data.CurrentPage,
                        TotalCount = data.TotalCount,
                        PageSize = data.PageSize,
                        TotalPages = data.TotalPages,
                        Filter = filter,

                        Items = data
                    };
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: GetPotonganTypeList(), filter: {filter}, " +
                        $"limit: {limit}, page: {page} Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<ResultBase> SavePotonganType(SavePotonganTypeRequest model)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = new ResultBase
                    {
                        Success = false,
                        Message = ""
                    };

                    var userLogin = _profileService.GetUserLogin();

                    var now = DateTime.Now;
                    var currentUser = userLogin.Username;
                    var currentApp = userLogin.App;

                    if (model.PotonganTypeId == null)
                    {
                        var existName = _domainService.GetAllPotonganType().SingleOrDefault(x =>
                            x.Name.Equals(model.PotonganTypeName));
                        if (existName != null)
                        {
                            result.Message = MessageConstants.S_POTONGAN_TYPE_EXIST;

                            return result;
                        }

                        var nextid = _domainService.GetAllPotonganType().Max(x => x.PotonganTypeId) + 1;
                        var newItem = new PotonganType
                        {
                            PotonganTypeId = nextid,
                            Name = model.PotonganTypeName,
                            GlAccount = model.GlAccount,
                            PostingKey = model.PostingKey,
                            TaxCode = model.TaxCode,
                            SubAccount = model.SubAccount,
                            Material = model.Material,
                            BusinessArea = model.BusinessArea,
                            CostCenter = model.CostCentre,
                            TextInSap = model.TextInSap,
                            IsActive = model.IsActive,

                            CreatedAt = now,
                            CreatedApp = currentApp,
                            CreatedBy = currentUser,
                            UpdatedAt = now,
                            UpdatedApp = currentApp,
                            UpdatedBy = currentUser,
                            Revision = ConfigConstants.N_INIT_REVISION
                        };

                        _domainService.InsertPotonganType(newItem);
                    }
                    else
                    {
                        var found = _domainService.GetAllPotonganType().SingleOrDefault(x => x.PotonganTypeId == model.PotonganTypeId);
                        if (found == null)
                        {
                            result.Message = MessageConstants.S_POTONGAN_TYPE_NOT_FOUND;

                            return result;
                        }

                        var existName = _domainService.GetAllPotonganType().SingleOrDefault(x =>
                            x.Name.Equals(model.PotonganTypeName)
                            && x.PotonganTypeId != found.PotonganTypeId);
                        if (existName != null)
                        {
                            result.Message = MessageConstants.S_POTONGAN_TYPE_EXIST;

                            return result;
                        }

                        found.Name = model.PotonganTypeName;
                        found.GlAccount = model.GlAccount;
                        found.PostingKey = model.PostingKey;
                        found.TaxCode = model.TaxCode;
                        found.SubAccount = model.SubAccount;
                        found.Material = model.Material;
                        found.BusinessArea = model.BusinessArea;
                        found.CostCenter = model.CostCentre;
                        found.TextInSap = model.TextInSap;
                        found.IsActive = model.IsActive;

                        found.UpdatedAt = now;
                        found.UpdatedApp = currentApp;
                        found.UpdatedBy = currentUser;
                        found.Revision += 1;

                        _domainService.UpdatePotonganType(found);
                    }

                    result.Success = true;
                    result.Message = MessageConstants.S_POTONGAN_TYPE_SAVED;

                    _domainService.SaveChanges();

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: SavePotonganType(), model: {model}" +
                        $"Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<ResultBase> RemovePotonganType(int PotonganTypeId)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = new ResultBase
                    {
                        Success = false,
                        Message = ""
                    };

                    var PotonganType = _domainService.GetAllPotonganType().SingleOrDefault(x => x.PotonganTypeId == PotonganTypeId);
                    if (PotonganType == null)
                    {
                        result.Message = MessageConstants.S_POTONGAN_TYPE_NOT_FOUND;

                        return result;
                    }

                    var corePotonganTypes = new List<int>();
                    foreach (int i in Enum.GetValues(typeof(PotonganTypeEnum)))
                    {
                        corePotonganTypes.Add(i);
                    }

                    var isCorePotonganType = corePotonganTypes.Contains(PotonganType.PotonganTypeId);
                    if (isCorePotonganType)
                    {
                        result.Message = MessageConstants.S_POTONGAN_TYPE_IS_CORE;

                        return result;
                    }

                    var hasTransaction = _domainService.GetAllPotongan()
                        .Where(x => x.PotonganTypeId == PotonganTypeId)
                        .Any();
                    if (hasTransaction)
                    {
                        result.Message = MessageConstants.S_POTONGAN_TYPE_IS_USED;

                        return result;
                    }

                    _domainService.DeletePotonganType(PotonganType);

                    result.Success = true;
                    result.Message = MessageConstants.S_POTONGAN_TYPE_REMOVED;

                    _domainService.SaveChanges();

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: RemovePotonganType(), PotonganTypeId: {PotonganTypeId}" +
                        $"Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<RoleResponse?> GetRole(int roleId)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = new RoleResponse();

                    var data = (from a in _domainService.GetAllRole()
                               where a.RoleId == roleId
                               select new RoleResponse
                               {
                                   RoleId = a.RoleId,
                                   RoleName = a.Name,
                                   DataLevelId = a.DataLevelId,
                                   IsSetOnSpecificBranch = a.IsSetOnSpecificBranch,
                                   IsSetOnSpecificArea = a.IsSetOnSpecificArea,
                               }).SingleOrDefault();

                    if (data != null)
                    {
                        var roleAreas = _domainService.GetAllRoleArea()
                            .Where(x => x.RoleId == data.RoleId)
                            .ToList();

                        data.AreaIds = roleAreas.Select(x => x.AreaId.ToString()).ToList();
                        data.AreaTexts = (from a in roleAreas
                                          join b in _domainService.GetAllArea() on a.AreaId equals b.AreaId
                                          select new
                                          {
                                              b.Name
                                          }).Select(x => x.Name).ToList();

                        var roleBranchs = _domainService.GetAllRoleBranch()
                            .Where(x => x.RoleId == data.RoleId)
                            .ToList();

                        data.BranchIds = roleBranchs.Select(x => x.BranchId.ToString()).ToList();
                        data.BranchTexts = (from a in roleBranchs
                                            join b in _domainService.GetAllBranch() on a.BranchId equals b.BranchId
                                            select new
                                            {
                                                b.Name
                                            }).Select(x => x.Name).ToList();
                    }

                    result = data;

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: GetRole(), " +
                        $"RoleId: {roleId}, Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<RoleListResponse> GetRoleList(string filter, int limit, int page)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = new RoleListResponse();

                    var query = from a in _domainService.GetAllRole()
                                join b in _domainService.GetAllDataLevel() on a.DataLevelId equals b.DataLevelId
                                orderby a.RoleId ascending
                                select new RoleDto
                                {
                                    RoleId = a.RoleId,
                                    RoleName = a.Name,
                                    DataLevelName = b.Name,
                                    SetOnSpecificBranch = a.IsSetOnSpecificBranch ?
                                        string.Join(", ", a.RoleBranches.Select(x => x.Branch.Name).ToList())
                                        : "",
                                    SetOnSpecificArea = a.IsSetOnSpecificArea ?
                                        string.Join(", ", a.RoleAreas.Select(x => x.Area.Name).ToList())
                                        : "",
                                };

                    if (string.IsNullOrEmpty(filter) == false)
                    {
                        filter = filter.ToUpper();
                        query = query.Where(x =>
                               x.RoleName.ToUpper().Contains(filter)
                            );
                    }

                    var data = PagedList<RoleDto>.ToPagedList(query, page, limit);

                    return result = new RoleListResponse
                    {
                        CurrentPage = data.CurrentPage,
                        TotalCount = data.TotalCount,
                        PageSize = data.PageSize,
                        TotalPages = data.TotalPages,
                        Filter = filter,

                        Items = data
                    };
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: GetRoleList(), filter: {filter}, " +
                        $"limit: {limit}, page: {page} Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<ResultBase> SaveRole(SaveRoleRequest model)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = new ResultBase
                    {
                        Success = false,
                        Message = ""
                    };

                    var userLogin = _profileService.GetUserLogin();

                    var now = DateTime.Now;
                    var currentUser = userLogin.Username;
                    var currentApp = userLogin.App;

                    var role = new Role();
                    if (model.RoleId == null)
                    {
                        var existName = _domainService.GetAllRole().SingleOrDefault(x =>
                            x.Name.Equals(model.RoleName));
                        if (existName != null)
                        {
                            result.Message = MessageConstants.S_ROLE_EXIST;

                            return result;
                        }

                        var nextid = _domainService.GetAllRole().Max(x => x.RoleId) + 1;
                        role = new Role
                        {
                            RoleId = nextid,
                            Name = model.RoleName,
                            DataLevelId = model.DataLevelId,
                            IsSetOnSpecificBranch = model.DataLevelId == (int)DataLevelEnum.BranchLevel ?
                                model.IsSetOnSpecificBranch : false,
                            IsSetOnSpecificArea = model.DataLevelId == (int)DataLevelEnum.AreaLevel ?
                                model.IsSetOnSpecificArea : false,

                            CreatedAt = now,
                            CreatedApp = currentApp,
                            CreatedBy = currentUser,
                            UpdatedAt = now,
                            UpdatedApp = currentApp,
                            UpdatedBy = currentUser,
                            Revision = ConfigConstants.N_INIT_REVISION
                        };

                        _domainService.InsertRole(role);
                    }
                    else
                    {
                        // existing
                        role = _domainService.GetAllRole().SingleOrDefault(x => x.RoleId == model.RoleId);
                        if (role == null)
                        {
                            result.Message = MessageConstants.S_ROLE_NOT_FOUND;

                            return result;
                        }

                        var existName = _domainService.GetAllRole().SingleOrDefault(x =>
                            x.Name.Equals(model.RoleName)
                            && x.RoleId != role.RoleId);
                        if (existName != null)
                        {
                            result.Message = MessageConstants.S_ROLE_EXIST;

                            return result;
                        }

                        role.Name = model.RoleName;
                        role.DataLevelId = model.DataLevelId;
                        role.IsSetOnSpecificBranch = model.DataLevelId == (int)DataLevelEnum.BranchLevel ?
                                model.IsSetOnSpecificBranch : false;
                        role.IsSetOnSpecificArea = model.DataLevelId == (int)DataLevelEnum.AreaLevel ?
                                model.IsSetOnSpecificArea : false;

                        role.UpdatedAt = now;
                        role.UpdatedApp = currentApp;
                        role.UpdatedBy = currentUser;
                        role.Revision += 1;

                        _domainService.UpdateRole(role);
                    }

                    var existingBranch = _domainService.GetAllRoleBranch().Where(x => x.RoleId == role.RoleId);
                    foreach (var item in existingBranch)
                    {
                        _domainService.DeleteRoleBranch(item);
                    }

                    var existingArea = _domainService.GetAllRoleArea().Where(x => x.RoleId == role.RoleId);
                    foreach (var item in existingArea)
                    {
                        _domainService.DeleteRoleArea(item);
                    }

                    if (role.IsSetOnSpecificBranch)
                    {
                        foreach (var branchId in model.BranchIds)
                        {
                            var roleBranch = new RoleBranch
                            {
                                RoleId = role.RoleId,
                                BranchId = branchId
                            };

                            _domainService.InsertRoleBranch(roleBranch);
                        }
                    }

                    if (role.IsSetOnSpecificArea)
                    {
                        foreach (var areaId in model.AreaIds)
                        {
                            var roleArea = new RoleArea
                            {
                                RoleId = role.RoleId,
                                AreaId = areaId
                            };

                            _domainService.InsertRoleArea(roleArea);
                        }
                    }

                    result.Success = true;
                    result.Message = MessageConstants.S_ROLE_SAVED;

                    _domainService.SaveChanges();

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: SaveRole(), model: {model}" +
                        $"Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<ResultBase> RemoveRole(int roleId)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = new ResultBase
                    {
                        Success = false,
                        Message = ""
                    };

                    var role = _domainService.GetAllRole().SingleOrDefault(x => x.RoleId == roleId);
                    if (role == null)
                    {
                        result.Message = MessageConstants.S_ROLE_NOT_FOUND;

                        return result;
                    }

                    var coreRoles = new List<int>();
                    foreach (int i in Enum.GetValues(typeof(RoleEnum)))
                    {
                        coreRoles.Add(i);
                    }

                    var isCoreRole = coreRoles.Contains(role.RoleId);
                    if (isCoreRole)
                    {
                        result.Message = MessageConstants.S_ROLE_IS_CORE;

                        return result;
                    }

                    var hasTransaction = _domainService.GetAllAppUser()
                        .Where(x => x.RoleId == roleId)
                        .Any();
                    if (hasTransaction)
                    {
                        result.Message = MessageConstants.S_ROLE_IS_USED;

                        return result;
                    }

                    var existingBranch = _domainService.GetAllRoleBranch().Where(x => x.RoleId == role.RoleId);
                    foreach (var item in existingBranch)
                    {
                        _domainService.DeleteRoleBranch(item);
                    }

                    var existingArea = _domainService.GetAllRoleArea().Where(x => x.RoleId == role.RoleId);
                    foreach (var item in existingArea)
                    {
                        _domainService.DeleteRoleArea(item);
                    }

                    _domainService.DeleteRole(role);

                    result.Success = true;
                    result.Message = MessageConstants.S_ROLE_REMOVED;

                    _domainService.SaveChanges();

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: RemoveRole(), RoleId: {roleId}" +
                        $"Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<SegmentResponse?> GetSegment(int segmentId)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = new SegmentResponse();

                    var data = from a in _domainService.GetAllSegment()
                               where a.SegmentId == segmentId
                               select new SegmentResponse
                               {
                                   SegmentId = a.SegmentId,
                                   SegmentName = a.Name,
                                   TemplateUploadTypeId = a.TemplateUploadTypeId,
                                   KpiPropertyCurrentId = a.KpiPropertyCurrentId,
                                   KpiPropertyTotalId = a.KpiPropertyTotalId,
                                   HasAmountUsd = a.HasAmountUsd,
                                   IsActive = a.IsActive
                               };

                    return data.SingleOrDefault();
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: GetSegment(), " +
                        $"segmentId: {segmentId}, Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<SegmentListResponse> GetSegmentList(string filter, int limit, int page)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = new SegmentListResponse();

                    var query = from a in _domainService.GetAllSegment()
                                join b in _domainService.GetAllTemplateUploadType() on a.TemplateUploadTypeId equals b.TemplateUploadTypeId
                                join c in _domainService.GetAllKpiProperty() on a.KpiPropertyCurrentId equals c.KpiPropertyId
                                join d in _domainService.GetAllKpiProperty() on a.KpiPropertyTotalId equals d.KpiPropertyId
                                orderby a.SegmentId ascending
                                select new SegmentDto
                                {
                                    SegmentId = a.SegmentId,
                                    SegmentName = a.Name,
                                    TemplateUploadType = b.Name,
                                    KpiPropertyCurrent = c.Name,
                                    KpiPropertyTotal = d.Name,
                                    HasAmountUsd = a.HasAmountUsd,
                                    IsActive = a.IsActive
                                };

                    if (string.IsNullOrEmpty(filter) == false)
                    {
                        filter = filter.ToUpper();
                        query = query.Where(x =>
                               x.SegmentName.ToUpper().Contains(filter)
                            || x.TemplateUploadType.ToUpper().Contains(filter)
                            || x.KpiPropertyCurrent.ToUpper().Contains(filter)
                            || x.KpiPropertyTotal.ToUpper().Contains(filter)
                            );
                    }

                    var data = PagedList<SegmentDto>.ToPagedList(query, page, limit);

                    return result = new SegmentListResponse
                    {
                        CurrentPage = data.CurrentPage,
                        TotalCount = data.TotalCount,
                        PageSize = data.PageSize,
                        TotalPages = data.TotalPages,
                        Filter = filter,

                        Items = data
                    };
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: GetSegmentList(), filter: {filter}, " +
                        $"limit: {limit}, page: {page} Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<ResultBase> SaveSegment(SaveSegmentRequest model)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = new ResultBase
                    {
                        Success = false,
                        Message = ""
                    };

                    var userLogin = _profileService.GetUserLogin();

                    var now = DateTime.Now;
                    var currentUser = userLogin.Username;
                    var currentApp = userLogin.App;

                    if (model.SegmentId == null)
                    {
                        var existName = _domainService.GetAllSegment().SingleOrDefault(x =>
                            x.Name.Equals(model.SegmentName));
                        if (existName != null)
                        {
                            result.Message = MessageConstants.S_SEGMENT_EXIST;

                            return result;
                        }

                        var nextid = _domainService.GetAllSegment().Max(x => x.SegmentId) + 1;
                        var newItem = new Segment
                        {
                            SegmentId = nextid,
                            Name = model.SegmentName,
                            TemplateUploadTypeId = model.TemplateUploadTypeId,
                            KpiPropertyCurrentId = model.KpiPropertyCurrentId,
                            KpiPropertyTotalId = model.KpiPropertyTotalId,
                            HasAmountUsd = model.HasAmountUsd,
                            IsActive = model.IsActive,

                            CreatedAt = now,
                            CreatedApp = currentApp,
                            CreatedBy = currentUser,
                            UpdatedAt = now,
                            UpdatedApp = currentApp,
                            UpdatedBy = currentUser,
                            Revision = ConfigConstants.N_INIT_REVISION
                        };

                        _domainService.InsertSegment(newItem);

                    }
                    else
                    {
                        var found = _domainService.GetAllSegment().SingleOrDefault(x => x.SegmentId == model.SegmentId);
                        if (found == null)
                        {
                            result.Message = MessageConstants.S_SEGMENT_NOT_FOUND;

                            return result;
                        }

                        var existName = _domainService.GetAllSegment().SingleOrDefault(x =>
                            x.Name.Equals(model.SegmentName)
                            && x.SegmentId != found.SegmentId);
                        if (existName != null)
                        {
                            result.Message = MessageConstants.S_SEGMENT_EXIST;

                            return result;
                        }

                        found.Name = model.SegmentName;
                        found.TemplateUploadTypeId = model.TemplateUploadTypeId;
                        found.KpiPropertyCurrentId = model.KpiPropertyCurrentId;
                        found.KpiPropertyTotalId = model.KpiPropertyTotalId;
                        found.HasAmountUsd = model.HasAmountUsd;
                        found.IsActive = model.IsActive;

                        found.UpdatedAt = now;
                        found.UpdatedApp = currentApp;
                        found.UpdatedBy = currentUser;
                        found.Revision += 1;

                        _domainService.UpdateSegment(found);
                    }

                    result.Success = true;
                    result.Message = MessageConstants.S_SEGMENT_SAVED;

                    _domainService.SaveChanges();

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: SaveSegment(), model: {model}" +
                        $"Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<ResultBase> RemoveSegment(int segmentId)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = new ResultBase
                    {
                        Success = false,
                        Message = ""
                    };

                    var segment = _domainService.GetAllSegment().SingleOrDefault(x => x.SegmentId == segmentId);
                    if (segment == null)
                    {
                        result.Message = MessageConstants.S_SEGMENT_NOT_FOUND;

                        return result;
                    }

                    var coreSegments = new List<int>();
                    foreach (int i in Enum.GetValues(typeof(SegmentEnum)))
                    {
                        coreSegments.Add(i);
                    }

                    var isCoreSegment = coreSegments.Contains(segment.SegmentId);
                    if (isCoreSegment)
                    {
                        result.Message = MessageConstants.S_SEGMENT_IS_CORE;

                        return result;
                    }

                    var hasTransaction = _domainService.GetAllIncomingPaymentNonSpm()
                        .Where(x => x.SegmentId == segmentId)
                        .Any();
                    if (hasTransaction)
                    {
                        result.Message = MessageConstants.S_SEGMENT_IS_USED;

                        return result;
                    }

                    _domainService.DeleteSegment(segment);

                    result.Success = true;
                    result.Message = MessageConstants.S_SEGMENT_REMOVED;

                    _domainService.SaveChanges();

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: RemoveSegment(), segmentId: {segmentId}" +
                        $"Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<SourceResponse?> GetSource(int sourceId)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = new SourceResponse();

                    var data = from a in _domainService.GetAllSource()
                               where a.SourceId == sourceId
                               select new SourceResponse
                               {
                                   SourceId = a.SourceId,
                                   SourceName = a.Name,
                                   BankChargeSubAccount = a.BankChargeSubAccount,
                                   IsActive = a.IsActive
                               };

                    return data.SingleOrDefault();
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: GetSource(), " +
                        $"segmentId: {sourceId}, Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<SourceListResponse> GetSourceList(string filter, int limit, int page)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = new SourceListResponse();

                    var query = from a in _domainService.GetAllSource()
                                orderby a.SourceId ascending
                                select new SourceDto
                                {
                                    SourceId = a.SourceId,
                                    SourceName = a.Name,
                                    BankChargeSubAccount = a.BankChargeSubAccount,
                                    IsActive = a.IsActive
                                };

                    if (string.IsNullOrEmpty(filter) == false)
                    {
                        filter = filter.ToUpper();
                        query = query.Where(x =>
                               x.SourceName.ToUpper().Contains(filter)
                            || x.BankChargeSubAccount.ToUpper().Contains(filter)
                            );
                    }

                    var data = PagedList<SourceDto>.ToPagedList(query, page, limit);

                    return result = new SourceListResponse
                    {
                        CurrentPage = data.CurrentPage,
                        TotalCount = data.TotalCount,
                        PageSize = data.PageSize,
                        TotalPages = data.TotalPages,
                        Filter = filter,

                        Items = data
                    };
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: GetSourceList(), filter: {filter}, " +
                        $"limit: {limit}, page: {page} Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<ResultBase> SaveSource(SaveSourceRequest model)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = new ResultBase
                    {
                        Success = false,
                        Message = ""
                    };

                    var userLogin = _profileService.GetUserLogin();

                    var now = DateTime.Now;
                    var currentUser = userLogin.Username;
                    var currentApp = userLogin.App;

                    if (model.SourceId == null)
                    {
                        var existName = _domainService.GetAllSource().SingleOrDefault(x =>
                            x.Name.Equals(model.SourceName));
                        if (existName != null)
                        {
                            result.Message = MessageConstants.S_SOURCE_EXIST;

                            return result;
                        }

                        var nextid = _domainService.GetAllSource().Max(x => x.SourceId) + 1;
                        var newItem = new Source
                        {
                            SourceId = nextid,
                            Name = model.SourceName,
                            BankChargeSubAccount = model.BankChargeSubAccount,
                            IsActive = model.IsActive,

                            CreatedAt = now,
                            CreatedApp = currentApp,
                            CreatedBy = currentUser,
                            UpdatedAt = now,
                            UpdatedApp = currentApp,
                            UpdatedBy = currentUser,
                            Revision = ConfigConstants.N_INIT_REVISION
                        };

                        _domainService.InsertSource(newItem);
                    }
                    else
                    {
                        var found = _domainService.GetAllSource().SingleOrDefault(x => x.SourceId == model.SourceId);
                        if (found == null)
                        {
                            result.Message = MessageConstants.S_SOURCE_NOT_FOUND;

                            return result;
                        }

                        var existName = _domainService.GetAllSource().SingleOrDefault(x =>
                            x.Name.Equals(model.SourceName)
                            && x.SourceId != found.SourceId);
                        if (existName != null)
                        {
                            result.Message = MessageConstants.S_SOURCE_EXIST;

                            return result;
                        }

                        found.Name = model.SourceName;
                        found.BankChargeSubAccount = model.BankChargeSubAccount;
                        found.IsActive = model.IsActive;

                        found.UpdatedAt = now;
                        found.UpdatedApp = currentApp;
                        found.UpdatedBy = currentUser;
                        found.Revision += 1;

                        _domainService.UpdateSource(found);
                    }

                    result.Success = true;
                    result.Message = MessageConstants.S_SOURCE_SAVED;

                    _domainService.SaveChanges();

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: SaveSource(), model: {model}" +
                        $"Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<ResultBase> RemoveSource(int sourceId)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = new ResultBase
                    {
                        Success = false,
                        Message = ""
                    };

                    var segment = _domainService.GetAllSource().SingleOrDefault(x => x.SourceId == sourceId);
                    if (segment == null)
                    {
                        result.Message = MessageConstants.S_SEGMENT_NOT_FOUND;

                        return result;
                    }

                    var coreSources = new List<int>();
                    foreach (int i in Enum.GetValues(typeof(SourceEnum)))
                    {
                        coreSources.Add(i);
                    }

                    var isCoreSource = coreSources.Contains(segment.SourceId);
                    if (isCoreSource)
                    {
                        result.Message = MessageConstants.S_SEGMENT_IS_CORE;

                        return result;
                    }

                    var hasSpmTransaction = _domainService.GetAllIncomingPayment()
                        .Where(x => x.SourceId == sourceId)
                        .Any();

                    var hasNonSpmTransaction = _domainService.GetAllIncomingPaymentNonSpm()
                        .Where(x => x.SourceId == sourceId)
                        .Any();
                    if (hasSpmTransaction || hasNonSpmTransaction)
                    {
                        result.Message = MessageConstants.S_SOURCE_IS_USED;

                        return result;
                    }

                    _domainService.DeleteSource(segment);

                    result.Success = true;
                    result.Message = MessageConstants.S_SOURCE_REMOVED;

                    _domainService.SaveChanges();

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: RemoveSource(), segmentId: {sourceId}" +
                        $"Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<UserResponse?> GetUser(int userId)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = new UserResponse();

                    var data = (from a in _domainService.GetAllUserView()
                                join b in _domainService.GetAllRole() on a.RoleId equals b.RoleId
                                where a.AppUserId != (int)AppUserEnum.Administrator
                                 && a.AppUserId == userId
                                select new UserResponse
                                {
                                    UserId = a.AppUserId,
                                    Email = a.Email,
                                    FullName = a.FullName,
                                    IsActive = a.IsActive,
                                    RoleId = a.RoleId,
                                    RoleInvoice = a.RoleInvoice,
                                    ApprovalId = a.ApprovalId ?? 0,
                                    ApprovalName = a.ApprovalName,
                                    ApprovalEmail = a.ApprovalEmail,
                                    IsSetUserArea = b.DataLevelId == (int)DataLevelEnum.AreaLevel ? !b.IsSetOnSpecificArea : false,
                                    IsSetUserBranch = b.DataLevelId == (int)DataLevelEnum.BranchLevel ? !b.IsSetOnSpecificBranch : false
                                }).SingleOrDefault();

                    if (data != null)
                    {
                        var userAreas = _domainService.GetAllAppUserArea()
                            .Where(x => x.AppUserId == data.UserId)
                            .ToList();

                        data.AreaIds = userAreas.Select(x => x.AreaId.ToString()).ToList();
                        data.AreaTexts = (from a in userAreas
                                          join b in _domainService.GetAllArea() on a.AreaId equals b.AreaId
                                          select new
                                          {
                                              b.Name
                                          }).Select(x => x.Name).ToList();

                        var userBranchs = _domainService.GetAllAppUserBranch()
                            .Where(x => x.AppUserId == data.UserId)
                            .ToList();

                        data.BranchIds = userBranchs.Select(x => x.BranchId.ToString()).ToList();
                        data.BranchTexts = (from a in userBranchs
                                            join b in _domainService.GetAllBranch() on a.BranchId equals b.BranchId
                                            select new
                                            {
                                                b.Name
                                            }).Select(x => x.Name).ToList();
                    }

                    result = data;

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: GetUser(), " +
                        $"UserId: {userId}, Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<UserSetupByRoleResponse?> GetUserSetupByRole(int roleId)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = new UserSetupByRoleResponse();

                    var data = (from a in _domainService.GetAllRole()
                                where a.RoleId == roleId
                                select new UserSetupByRoleResponse
                                {
                                    RoleId = a.RoleId,
                                    IsSetUserArea = a.DataLevelId == (int)DataLevelEnum.AreaLevel ? !a.IsSetOnSpecificArea : false,
                                    IsSetUserBranch = a.DataLevelId == (int)DataLevelEnum.BranchLevel ? !a.IsSetOnSpecificBranch : false
                                }).SingleOrDefault();

                    result = data;

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: GetUserSetupByRole(), " +
                        $"UserId: {roleId}, Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<UserListResponse> GetUserList(string filter, int limit, int page)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = new UserListResponse();

                    var query = from a in _domainService.GetAllUserView()
                                where a.RoleId != (int)RoleEnum.Administrator
                                orderby a.AppUserId ascending
                                select new UserDto
                                {
                                    UserId = a.AppUserId,
                                    FullName = a.FullName,
                                    Email = a.Email,
                                    Area = a.Areas,
                                    Branch = a.Branches,
                                    Role = a.RoleName,
                                    IsActive = a.IsActive
                                };

                    if (string.IsNullOrEmpty(filter) == false)
                    {
                        filter = filter.ToUpper();
                        query = query.Where(x =>
                               x.FullName.ToUpper().Contains(filter)
                            || x.Email.ToUpper().Contains(filter)
                            || x.Area.ToUpper().Contains(filter)
                            || x.Branch.ToUpper().Contains(filter)
                            || x.Role.ToUpper().Contains(filter)
                            );
                    }

                    var data = PagedList<UserDto>.ToPagedList(query, page, limit);

                    return result = new UserListResponse
                    {
                        CurrentPage = data.CurrentPage,
                        TotalCount = data.TotalCount,
                        PageSize = data.PageSize,
                        TotalPages = data.TotalPages,
                        Filter = filter,

                        Items = data
                    };
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: GetUserList(), filter: {filter}, " +
                        $"limit: {limit}, page: {page} Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<ResultBase> SaveUser(SaveUserRequest model)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = new ResultBase
                    {
                        Success = false,
                        Message = ""
                    };

                    var userLogin = _profileService.GetUserLogin();

                    var now = DateTime.Now;
                    var currentUser = userLogin.Username;
                    var currentApp = userLogin.App;

                    MailAddress addr;
                    if (!AppHelper.TryParseEmail(model.Email, out addr) || addr.Host != _appSettings.AjinomotoDomain)
                    {
                        result.Message = MessageConstants.S_INVALID_EMAIL;

                        return result;
                    }

                    AppUser user;
                    if (model.UserId == null)
                    {
                        var existName = _domainService.GetAllAppUser().SingleOrDefault(x =>
                            x.Username.Equals(model.Email));
                        if (existName != null)
                        {
                            result.Message = MessageConstants.S_USER_EXIST;

                            return result;
                        }

                        var nextid = _domainService.GetAllAppUser().Max(x => x.AppUserId) + 1;
                        user = new AppUser
                        {
                            AppUserId = nextid,
                            Username = addr.User,
                            FullName = model.FullName,
                            Email = model.Email,
                            RoleId = model.RoleId,
                            IsActive = model.IsActive,
                            Password = BCrypt.Net.BCrypt.HashPassword(ConfigConstants.S_DEFAULT_PASSWORD),

                            CreatedAt = now,
                            CreatedApp = currentApp,
                            CreatedBy = currentUser,
                            UpdatedAt = now,
                            UpdatedApp = currentApp,
                            UpdatedBy = currentUser,
                            Revision = ConfigConstants.N_INIT_REVISION
                        };

                        _domainService.InsertAppUser(user);
                    }
                    else
                    {
                        user = _domainService.GetAllAppUser().SingleOrDefault(x => x.AppUserId == model.UserId);
                        if (user == null)
                        {
                            result.Message = MessageConstants.S_USER_NOT_FOUND;

                            return result;
                        }

                        if (user.RoleId == (int)RoleEnum.Administrator)
                        {
                            result.Message = MessageConstants.S_USER_IS_ADMIN;

                            return result;
                        }

                        var existName = _domainService.GetAllAppUser().SingleOrDefault(x =>
                            x.Username.Equals(addr.User)
                            && x.AppUserId != user.AppUserId);
                        if (existName != null)
                        {
                            result.Message = MessageConstants.S_USER_EXIST;

                            return result;
                        }

                        user.Username = addr.User;
                        user.FullName = model.FullName;
                        user.Email = model.Email;
                        user.RoleId = model.RoleId;
                        user.IsActive = model.IsActive;

                        user.UpdatedAt = now;
                        user.UpdatedApp = currentApp;
                        user.UpdatedBy = currentUser;
                        user.Revision += 1;

                        _domainService.UpdateAppUser(user);
                    }

                    // delete all
                    var originUserBranches = _domainService.GetAllAppUserBranch().Where(x => x.AppUserId == user.AppUserId).ToList();
                    foreach (var item in originUserBranches)
                    {
                        _domainService.DeleteAppUserBranch(item);
                    }

                    var originUserAreas = _domainService.GetAllAppUserArea().Where(x => x.AppUserId == user.AppUserId).ToList();
                    foreach (var item in originUserAreas)
                    {
                        _domainService.DeleteAppUserArea(item);
                    }

                    // insert
                    var role = _domainService.GetAllRole().Single(x => x.RoleId == user.RoleId);

                    if (role.DataLevelId == (int)DataLevelEnum.BranchLevel && !role.IsSetOnSpecificBranch)
                    {
                        foreach (var branchId in model.BranchIds)
                        {
                            var newItem = new AppUserBranch
                            {
                                AppUserBranchId = Guid.NewGuid(),
                                AppUserId = user.AppUserId,
                                BranchId = branchId
                            };

                            _domainService.InsertAppUserBranch(newItem);
                        }
                    }

                    if (role.DataLevelId == (int)DataLevelEnum.AreaLevel && !role.IsSetOnSpecificArea)
                    {
                        foreach (var areaId in model.AreaIds)
                        {
                            var newItem = new AppUserArea
                            {
                                AppUserAreaId = Guid.NewGuid(),
                                AppUserId = user.AppUserId,
                                AreaId = areaId
                            };

                            _domainService.InsertAppUserArea(newItem);
                        }
                    }

                    result.Success = true;
                    result.Message = MessageConstants.S_USER_SAVED;

                    _domainService.SaveChanges();

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: SaveUser(), model: {model}" +
                        $"Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }
    }
}
