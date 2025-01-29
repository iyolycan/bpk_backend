using Ajinomoto.Arc.Common.AppModels;
using Ajinomoto.Arc.Common.Enums;
using Serilog;

namespace Ajinomoto.Arc.Business.Modules
{
    public partial class MasterDataService
    {
        public async Task<IEnumerable<DropdownDto>> GetDdlArea(string? filter)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = new List<DropdownDto>();

                    filter = (filter ?? "").ToUpper();
                    if (filter.Length < 1)
                    {
                        return result;
                    }

                    result = (from a in GetAllActiveArea()
                              where a.Name.ToUpper().Contains(filter)
                              orderby a.AreaId ascending
                              select new DropdownDto
                              {
                                  Value = a.AreaId.ToString(),
                                  Text = a.Name
                              }).ToList();

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: GetDdlArea()" +
                        $"Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<IEnumerable<DropdownDto>> GetDdlBranch()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = new List<DropdownDto>();

                    result = (from a in GetAllActiveBranch()
                              orderby a.BranchId ascending
                              select new DropdownDto
                              {
                                  Value = a.BranchId.ToString(),
                                  Text = a.Name + " (" + a.BusinessArea + ")"
                              }).ToList();

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: GetDdlBranch()" +
                        $"Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<IEnumerable<DropdownDto>> GetDdlCustomer()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = new List<DropdownDto>();

                    result = (from a in _domainService.GetAllCustomer()
                              orderby a.CustomerCode ascending
                              select new DropdownDto
                              {
                                  Value = a.CustomerCode,
                                  Text = a.CustomerCode + " - " + a.Name,
                              }).ToList();

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: GetDdlCustomer()" +
                        $"Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<IEnumerable<DropdownDto>> GetDdlCustomer(string? filter)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = new List<DropdownDto>();

                    filter = (filter ?? "").ToUpper();
                    if (filter.Length < 3)
                    {
                        return result;
                    }

                    result = (from a in _domainService.GetAllCustomer()
                              where (a.CustomerCode + " - " + a.Name).ToUpper().Contains(filter)
                              orderby a.CustomerCode ascending
                              select new DropdownDto
                              {
                                  Value = a.CustomerCode,
                                  Text = a.CustomerCode + " - " + a.Name,
                              }).ToList();

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: GetDdlCustomer(), filter: {filter}" +
                        $"Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<IEnumerable<DropdownDto>> GetDdlDataLevel()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = new List<DropdownDto>();

                    result = (from a in _domainService.GetAllDataLevel()
                              orderby a.DataLevelId ascending
                              select new DropdownDto
                              {
                                  Value = a.DataLevelId.ToString(),
                                  Text = a.Name
                              }).ToList();

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: GetDdlDataLevel(), " +
                        $"Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<IEnumerable<DropdownDto>> GetDdlInvoice(string filter)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = new List<DropdownDto>();

                    filter = (filter ?? "").ToUpper();
                    if (filter.Length < 1)
                    {
                        return result;
                    }

                    var unassignInvoices = from a in _domainService.GetAllInvoice()
                                           join bpkDetail in GetAllActiveBpkDetail() on a.InvoiceNumber equals bpkDetail.InvoiceNumber into ab
                                           from b in ab.DefaultIfEmpty()
                                           where b == null
                                           select a;

                    var data = from a in unassignInvoices
                               join b in _domainService.GetAllCustomer() on a.CustomerCode equals b.CustomerCode
                               where (a.InvoiceNumber + " - " + b.Name).ToUpper().Contains(filter)
                               orderby a.InvoiceNumber ascending
                               select new DropdownDto
                               {
                                   Value = a.InvoiceNumber,
                                   Text = a.InvoiceNumber + " - " + b.Name
                               };

                    result = data.Take(_appSettings.DropdownMaxItem).ToList();

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: GetDdlInvoice(), filter: {filter}" +
                        $"Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<IEnumerable<DropdownDto>> GetDdlInvoiceByCustomer(string customerCode)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = new List<DropdownDto>();

                    var unassignInvoices = from a in _domainService.GetAllInvoice()
                                           join bpkDetail in GetAllActiveBpkDetail() on a.InvoiceNumber equals bpkDetail.InvoiceNumber into ab
                                           from b in ab.DefaultIfEmpty()
                                           where b == null
                                           select a;


                    if (!string.IsNullOrEmpty(customerCode))
                    {
                        unassignInvoices = unassignInvoices.Where(x => x.CustomerCode == customerCode);
                    }

                    result = (from a in unassignInvoices
                              select new DropdownDto
                              {
                                  Value = a.InvoiceNumber,
                                  Text = a.InvoiceNumber
                              }).ToList();

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: GetDdlInvoiceByCustomer(), customerCode: {customerCode}" +
                        $"Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<IEnumerable<DropdownDto>> GetDdlKpiProperty()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = new List<DropdownDto>();

                    result = (from a in _domainService.GetAllKpiProperty()
                              orderby a.KpiPropertyId ascending
                              select new DropdownDto
                              {
                                  Value = a.KpiPropertyId.ToString(),
                                  Text = a.Name,
                              }).ToList();

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: GetDdlKpiProperty()" +
                        $"Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<IEnumerable<DropdownDto>> GetDdlPotonganType()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = new List<DropdownDto>();

                    result = (from a in GetAllActivePotonganType().OrderBy(x => x.PotonganTypeId)
                              select new DropdownDto
                              {
                                  Value = a.PotonganTypeId.ToString(),
                                  Text = a.Name
                              }).ToList();

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: GetDdlPotonganType(), " +
                        $"Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<IEnumerable<DropdownDto>> GetDdlRole()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = new List<DropdownDto>();

                    result = (from a in _domainService.GetAllRole().OrderBy(x => x.RoleId)
                              select new DropdownDto
                              {
                                  Value = a.RoleId.ToString(),
                                  Text = a.Name
                              }).ToList();

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: GetDdlRole(), " +
                        $"Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<IEnumerable<DropdownDto>> GetDdlSegment()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = new List<DropdownDto>();

                    result = (from a in GetAllActiveSegment()
                              orderby a.SegmentId ascending
                              select new DropdownDto
                              {
                                  Value = a.SegmentId.ToString(),
                                  Text = a.Name,
                              }).ToList();

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: GetDdlSegment()" +
                        $"Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<IEnumerable<DropdownDto>> GetDdlSource()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = new List<DropdownDto>();

                    result = (from a in GetAllActiveSource()
                              orderby a.SourceId ascending
                              select new DropdownDto
                              {
                                  Value = a.SourceId.ToString(),
                                  Text = a.Name,
                              }).ToList();

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: GetDdlSource()" +
                        $"Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<IEnumerable<DropdownDto>> GetDdlTemplateUploadType()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = new List<DropdownDto>();

                    result = (from a in _domainService.GetAllTemplateUploadType()
                              orderby a.TemplateUploadTypeId ascending
                              select new DropdownDto
                              {
                                  Value = a.TemplateUploadTypeId.ToString(),
                                  Text = a.Name,
                              }).ToList();

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: GetDdlTemplateUploadType()" +
                        $"Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<IEnumerable<DropdownDto>> GetDdlUser()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = new List<DropdownDto>();

                    result = (from a in GetAllActiveAppUser()
                              where a.RoleId != (int)RoleEnum.Administrator
                              orderby a.FullName ascending
                              select new DropdownDto
                              {
                                  Value = a.AppUserId.ToString(),
                                  Text = a.FullName,
                              }).ToList();

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: GetDdlUser()" +
                        $"Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<IEnumerable<DropdownDto>> GetDdlUserCoec()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = new List<DropdownDto>();

                    result = (from a in GetAllActiveAppUser()
                              where a.RoleId == (int)RoleEnum.Coec
                              orderby a.FullName ascending
                              select new DropdownDto
                              {
                                  Value = a.AppUserId.ToString(),
                                  Text = a.FullName,
                              }).ToList();

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: GetDdlUserCoec()" +
                        $"Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }
    }
}
