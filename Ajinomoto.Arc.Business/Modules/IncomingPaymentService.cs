using Ajinomoto.Arc.Business.Helper;
using Ajinomoto.Arc.Business.Interfaces;
using Ajinomoto.Arc.Common.AppModels;
using Ajinomoto.Arc.Common.Constants;
using Ajinomoto.Arc.Common.DtoModels;
using Ajinomoto.Arc.Common.Enums;
using Ajinomoto.Arc.Common.Helpers;
using Ajinomoto.Arc.Common.Extensions;
using ClosedXML.Excel;
using Microsoft.Extensions.Options;
using Serilog;
using System.Data;
using Ajinomoto.Arc.Data.Models;
using Microsoft.EntityFrameworkCore;
using Ajinomoto.Arc.Data;

namespace Ajinomoto.Arc.Business.Modules
{
    public partial class IncomingPaymentService : IIncomingPaymentService
    {
        private readonly IDomainService _domainService;
        private readonly IProfileService _profileService;
        private readonly IHistoryService _historyService;
        private readonly IMasterDataService _masterDataService;

        private readonly IMailService _mailService;
        private readonly AppSettings _appSettings;

        private readonly DbContextOptions<DataContext> _dbContextOptions;

        public IncomingPaymentService(IDomainService domainService,
            IOptions<AppSettings> appSettings,
            IProfileService profileService,
            IHistoryService historyService,
            IMasterDataService masterDataService,
            IMailService mailService,
            DbContextOptions<DataContext> dbContextOptions)
        {
            _domainService = domainService;
            _appSettings = appSettings.Value;
            _profileService = profileService;
            _historyService = historyService;
            _masterDataService = masterDataService;
            _mailService = mailService;
             _dbContextOptions = dbContextOptions;
        }

        public async Task<IncomingPaymentListResponse> GetIncomingPaymentList(string filter,
            IncomingPaymentColumn? sortOrder,
            IncomingPaymentColumn currentSort,
            SortingDirection? sortDirection,
            int limit = ConfigConstants.N_DEFAULT_PAGESIZE,
            int page = ConfigConstants.N_DEFAULT_PAGE,
            string? Cabang = null, 
            string? Customer = null,
            string? StatusBpk = null,
            string? StatusClearing = null,
            string? Area = null,
            DateOnly? FromDate = null,
            DateOnly? ToDate = null)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = new IncomingPaymentListResponse();

                    var query = QueryIncomingPaymentList(filter, sortOrder, sortDirection);

                    // Add conditional filtering based on the provided parameters.
                    if (Cabang != "")
                        query = query.Where(p => p.Branch == Cabang);

                    if (Customer != "")
                        query = query.Where(p => p.CustomerCode == Customer);

                    if (StatusBpk != "")
                        query = query.Where(p => p.BpkStatus == StatusBpk);

                    if (StatusClearing != "")
                        query = query.Where(p => p.ClearingStatus == StatusClearing);

                    if (Area != "")
                        query = query.Where(p => p.Area == Area);
                    
                    // Filter by date range
                    if (FromDate.HasValue)
                        query = query.Where(p => p.PaymentDate >= FromDate);

                    if (ToDate.HasValue)
                        query = query.Where(p => p.PaymentDate <= ToDate);

                    var data = PagedList<IncomingPaymentDto>.ToPagedList(query, page, limit);

                    return result = new IncomingPaymentListResponse
                    {
                        CurrentPage = data.CurrentPage,
                        TotalCount = data.TotalCount,
                        PageSize = data.PageSize,
                        TotalPages = data.TotalPages,
                        Filter = filter,
                        CurrentSort = sortOrder.Value,
                        SortDirection = sortDirection.Value,

                        Items = data
                    };
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: GetIncomingPaymentList(), filter: {filter}, " +
                        $"sortOrder: {sortOrder}, currentSort: {currentSort}, " +
                        $"limit: {limit}, page: {page} Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }
        
        public async Task<InvoiceDetailsListResponse> GetInvoiceDetailsList(
            string filter,
            InvoiceDetailsColumn? sortOrder,
            InvoiceDetailsColumn currentSort,
            SortingDirection? sortDirection,
            int limit = ConfigConstants.N_DEFAULT_PAGESIZE,
            int page = ConfigConstants.N_DEFAULT_PAGE,
            string? Cabang = null,
            int? Customer = null,
            string? Status = null,
            string? StatusTukarFaktur = null,
            DateOnly? FromDate = null,
            DateOnly? ToDate = null)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = new InvoiceDetailsListResponse();

                    // Base query to fetch data from the invoice_details table
                    var query = QueryInvoiceDetailstList(filter, sortOrder, sortDirection);

                    // Add conditional filtering based on the provided parameters.
                    if (Cabang != "")
                        query = query.Where(p => p.CabangId == Cabang);

                    if (Customer != null && Customer > 0)
                        query = query.Where(p => p.IDCustomerSoldTo == Customer);

                    if (Status != "")
                        query = query.Where(p => p.Status == Status);

                    if (StatusTukarFaktur != "")
                        query = query.Where(p => p.StatusTukarFaktur == StatusTukarFaktur);
                    
                    // Filter by date range
                    if (FromDate.HasValue)
                        query = query.Where(p => p.DocDate >= FromDate);

                    if (ToDate.HasValue)
                        query = query.Where(p => p.DocDate <= ToDate);

                    // Paginate the results
                    var data = PagedList<InvoiceDetailsDto>.ToPagedList(query, page, limit);
                    // Console.WriteLine("Fetched data: " + query.ToQueryString());
                    // Map the results to the response object
                    return result =  new InvoiceDetailsListResponse
                    {
                        CurrentPage = data.CurrentPage,
                        TotalCount = data.TotalCount,
                        PageSize = data.PageSize,
                        TotalPages = data.TotalPages,
                        Filter = filter,
                        CurrentSort = sortOrder.Value,
                        SortDirection = sortDirection.Value,
                        Items = data
                    };
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: GetInvoiceDetailsList(), filter: {filter}, " +
                        $"sortOrder: {sortOrder}, currentSort: {currentSort}, " +
                        $"limit: {limit}, page: {page} Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<ResultBase> ImportIncomingPayment(ImportIncomingPaymentRequest param)
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

                    var segmentId = param.SegmentId;
                    var sourceId = param.SourceId;
                    var picId = param.PicId;
                    var fileUpload = param.FileUpload;

                    Directory.CreateDirectory(_appSettings.StoredFilesPath);
                    var filePath = Path.Combine(_appSettings.StoredFilesPath, Path.GetRandomFileName().Replace(".", string.Empty) + ".xlsx");
                    using (var stream = File.Create(filePath))
                    {
                        fileUpload.CopyTo(stream);
                    }

                    var segment = _domainService.GetAllSegment().Single(x => x.SegmentId == segmentId);
                    switch (segment.TemplateUploadTypeId)
                    {
                        case (int)TemplateUploadTypeEnum.TemplateUpload01:
                            result = ImportIncomingPaymentTemplate01(segmentId, sourceId, picId, filePath);
                            break;
                        case (int)TemplateUploadTypeEnum.TemplateUpload02:
                            result = ImportIncomingPaymentTemplate02(segmentId, sourceId, picId, filePath);
                            break;
                        case (int)TemplateUploadTypeEnum.TemplateUpload03:
                            result = ImportIncomingPaymentTemplate03(segmentId, sourceId, picId, filePath);
                            break;
                        case (int)TemplateUploadTypeEnum.TemplateUpload04:
                            result = ImportIncomingPaymentTemplate04(segmentId, sourceId, picId, filePath);
                            break;
                        case (int)TemplateUploadTypeEnum.TemplateUpload05:
                            result = ImportIncomingPaymentTemplate05(segmentId, sourceId, picId, filePath);
                            break;
                        case (int)TemplateUploadTypeEnum.TemplateUpload06:
                            result = ImportIncomingPaymentTemplate06(segmentId, sourceId, picId, filePath);
                            break;
                        case (int)TemplateUploadTypeEnum.TemplateUpload07:
                            result = ImportIncomingPaymentTemplate07(segmentId, sourceId, picId, filePath);
                            break;
                        case (int)TemplateUploadTypeEnum.TemplateUpload08:
                            result = ImportIncomingPaymentTemplate08(segmentId, sourceId, picId, filePath);
                            break;
                        case (int)TemplateUploadTypeEnum.TemplateUpload09:
                            result = ImportIncomingPaymentTemplate09(segmentId, sourceId, picId, filePath);
                            break;
                        case (int)TemplateUploadTypeEnum.TemplateUpload10:
                            result = ImportIncomingPaymentTemplate10(segmentId, sourceId, picId, filePath);
                            break;
                        case (int)TemplateUploadTypeEnum.TemplateUpload11:
                            result = ImportIncomingPaymentTemplate11(segmentId, sourceId, picId, filePath);
                            break;
                        case (int)TemplateUploadTypeEnum.TemplateUpload12:
                            result = ImportIncomingPaymentTemplate12(segmentId, sourceId, picId, filePath);
                            break;
                        case (int)TemplateUploadTypeEnum.TemplateUpload13:
                            result = ImportIncomingPaymentTemplate13(segmentId, sourceId, picId, filePath);
                            break;
                        default:
                            result.Message = "Template not found.";
                            break;
                    }

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: ImportIncomingPayment(), param: {param}, Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }
        public async Task<ResultBase> ImportInvoice(ImportInvoiceRequest param)
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

                    var fileUpload = param.FileUpload;

                    Directory.CreateDirectory(_appSettings.StoredFilesPath);
                    var filePath = Path.Combine(_appSettings.StoredFilesPath, Path.GetRandomFileName().Replace(".", string.Empty) + ".xlsx");
                    using (var stream = File.Create(filePath))
                    {
                        fileUpload.CopyTo(stream);
                    }
                    
                    result = ImportInvoiceTemplate01(filePath);

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: ImportIncomingPayment(), param: {param}, Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        // origin code

        public async Task<XLWorkbook> GenerateIncomingPaymentListReport(IncomingPaymentListRequest param)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var query = QueryIncomingPaymentList(param.Filter, param.SortOrder, param.SortDirection);
                    // Add conditional filtering based on the provided parameters.
                    if (param.Cabang != "")
                        query = query.Where(p => p.Branch == param.Cabang);

                    if (param.Customer != "")
                        query = query.Where(p => p.CustomerCode == param.Customer);

                    if (param.StatusBpk != "")
                        query = query.Where(p => p.BpkStatus == param.StatusBpk);

                    if (param.StatusClearing != "")
                        query = query.Where(p => p.ClearingStatus == param.StatusClearing);

                    if (param.Area != "")
                        query = query.Where(p => p.Area == param.Area);
                    
                    // Filter by date range
                    if (param.FromDate.HasValue)
                        query = query.Where(p => p.PaymentDate >= param.FromDate);

                    if (param.ToDate.HasValue)
                        query = query.Where(p => p.PaymentDate <= param.ToDate);
                    // windows only
                    // var fileTemplate = Path.Combine(Environment.CurrentDirectory, @"Template\template-IncomingPaymentList-report.xlsx");
                    // temp use this for mac path
                    var fileTemplate = Path.Combine(Environment.CurrentDirectory, @"Template/template-IncomingPaymentList-report.xlsx");

                    Console.WriteLine("Current Directory: " + Environment.CurrentDirectory);
                    Console.WriteLine("File Path: " + fileTemplate);

                    if (File.Exists(fileTemplate))
                    {
                        Console.WriteLine("File exists and is accessible.");
                        // Proceed with your logic
                    }
                    else
                    {
                        Console.WriteLine("File does not exist or is not accessible.");
                    }

                    XLWorkbook wb = new XLWorkbook(fileTemplate);
                    IXLWorksheet ws = wb.Worksheet(1);

                    var bpkHistory = _domainService.GetAllBpkHistory().ToList();
                    var config = _domainService.GetAllAppConfig().ToList();

                    var maxExportPaymentList = config.Single(x => x.AppConfigId == (int)AppConfigEnum.MAX_EXPORT_PAYMENT_LIST);

                    var data = query;
                    if (maxExportPaymentList.IntValue.HasValue && maxExportPaymentList.IntValue > 0)
                    {
                        data = query.Take(maxExportPaymentList.IntValue.Value);
                    }

                    int row = 4;
                    int i = 1;
                    foreach (var item in data)
                    {
                        // The original GUID string
                        string guidString = item.IncomingPaymentId.ToString();
                        
                        // Parse the string into a GUID object
                        Guid guid = Guid.Parse(guidString);
                        
                        // Convert GUID to a byte array
                        byte[] bytes = guid.ToByteArray();
                        
                        // Reverse specific parts of the byte array to match SQL Server's format:
                        // Reverse the first 4 bytes (0-3)
                        Array.Reverse(bytes, 0, 4);
                        // Reverse the next 2 bytes (4-5)
                        Array.Reverse(bytes, 4, 2);
                        // Reverse the next 2 bytes (6-7)
                        Array.Reverse(bytes, 6, 2);

                        // Convert the byte array to a hexadecimal string
                        string hexString = BitConverter.ToString(bytes).Replace("-", "");

                        // Format the hexadecimal string as desired, adding '0x' at the start
                        string formattedHexString = "0x" + hexString.ToLower();
                        
                        ws.Cell("A" + row).Value = i++;
                        ws.Cell("B" + row).Value = formattedHexString;
                        ws.Cell("C" + row).Value = item.PaymentDate;
                        ws.Cell("C" + row).Style.NumberFormat.Format = "dd.MM.yyyy";
                        ws.Cell("D" + row).Value = item.Amount;
                        ws.Cell("E" + row).Value = item.CustomerName;
                        ws.Cell("F" + row).Value = item.Area;
                        ws.Cell("G" + row).Value = item.Branch;
                        ws.Cell("H" + row).Value = item.CustomerCode;
                        ws.Cell("I" + row).Value = item.InterfaceNumber;
                        ws.Cell("J" + row).Value = item.ClearingNumber;
                        ws.Cell("K" + row).Value = item.ClearingStatus;
                        ws.Cell("L" + row).Value = item.BpkNumber;
                        ws.Cell("M" + row).Value = item.BpkStatus;

                        // Historical Data Created
                        var created = bpkHistory.Where(x => x.BpkId == item.BpkId && x.AppActionId == (int)AppActionEnum.Created)
                                     .OrderByDescending(x => x.ActionAt)
                                     .FirstOrDefault();

                        ws.Cell("N" + row).Value = created == null ? "" : created.ActionAt;
                        ws.Cell("O" + row).Value = created == null ? "" : created.ActionBy;

                        // Historical Data Save as Draft
                        var saveAsDraft = bpkHistory.Where(x => x.BpkId == item.BpkId && x.AppActionId == (int)AppActionEnum.SaveAsDraft)
                                         .OrderByDescending(x => x.ActionAt)
                                         .FirstOrDefault();

                        ws.Cell("P" + row).Value = saveAsDraft == null ? "" : saveAsDraft.ActionAt;
                        ws.Cell("Q" + row).Value = saveAsDraft == null ? "" : saveAsDraft.ActionBy;

                        // Historical Data Submitted
                        var submitted = bpkHistory.Where(x => x.BpkId == item.BpkId && x.AppActionId == (int)AppActionEnum.Submitted)
                                       .OrderByDescending(x => x.ActionAt)
                                       .FirstOrDefault();

                        ws.Cell("R" + row).Value = submitted == null ? "" : submitted.ActionAt;
                        ws.Cell("S" + row).Value = submitted == null ? "" : submitted.ActionBy;

                        // Historical Data Clearing
                        var clearing = bpkHistory.Where(x => x.BpkId == item.BpkId && x.AppActionId == (int)AppActionEnum.Clearing)
                                      .OrderByDescending(x => x.ActionAt)
                                      .FirstOrDefault();

                        ws.Cell("T" + row).Value = clearing == null ? "" : clearing.ActionAt;
                        ws.Cell("U" + row).Value = clearing == null ? "" : clearing.ActionBy;

                        // Historical Data Request For
                        var requestForRevision = bpkHistory.Where(x => x.BpkId == item.BpkId && x.AppActionId == (int)AppActionEnum.RequestForRevision)
                                                .OrderByDescending(x => x.ActionAt)
                                                .FirstOrDefault();

                        ws.Cell("V" + row).Value = requestForRevision == null ? "" : requestForRevision.ActionAt;
                        ws.Cell("W" + row).Value = requestForRevision == null ? "" : requestForRevision.ActionBy;

                        // Historical Data Revise Approve
                        var reviseApprove = bpkHistory.Where(x => x.BpkId == item.BpkId && x.AppActionId == (int)AppActionEnum.ReviseApprove)
                                           .OrderByDescending(x => x.ActionAt)
                                           .FirstOrDefault();

                        ws.Cell("X" + row).Value = reviseApprove == null ? "" : reviseApprove.ActionAt;
                        ws.Cell("Y" + row).Value = reviseApprove == null ? "" : reviseApprove.ActionBy;

                        // Historical Data Revise Reject
                        var reviseReject = bpkHistory.Where(x => x.BpkId == item.BpkId && x.AppActionId == (int)AppActionEnum.ReviseReject)
                                          .OrderByDescending(x => x.ActionAt)
                                          .FirstOrDefault();

                        ws.Cell("Z" + row).Value = reviseReject == null ? "" : reviseReject.ActionAt;
                        ws.Cell("AA" + row).Value = reviseReject == null ? "" : reviseReject.ActionBy;

                        row++;
                    }
                    ws.RangeUsed().Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                    ws.RangeUsed().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    return wb;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: ExportIncomingPaymentList(), filter: {param.Filter}, " +
                       $"sortOrder: {param.SortOrder}, " +
                       $"limit: {param.Limit}, page: {param.Page} Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        // try optimize cahyo 1: only can contain around 4.032 data row
        
        // public async Task<XLWorkbook> GenerateIncomingPaymentListReport(IncomingPaymentListRequest param)
        // {
        //     return await Task.Run(() =>
        //     {
        //         try
        //         {
        //             // Step 1: Query data with applied filters
        //             var query = QueryIncomingPaymentList(param.Filter, param.SortOrder, param.SortDirection);

        //             if (!string.IsNullOrEmpty(param.Cabang))
        //                 query = query.Where(p => p.Branch == param.Cabang);

        //             if (!string.IsNullOrEmpty(param.Customer))
        //                 query = query.Where(p => p.CustomerCode == param.Customer);

        //             if (!string.IsNullOrEmpty(param.StatusBpk))
        //                 query = query.Where(p => p.BpkStatus == param.StatusBpk);

        //             if (!string.IsNullOrEmpty(param.StatusClearing))
        //                 query = query.Where(p => p.ClearingStatus == param.StatusClearing);

        //             if (!string.IsNullOrEmpty(param.Area))
        //                 query = query.Where(p => p.Area == param.Area);

        //             if (param.FromDate.HasValue)
        //                 query = query.Where(p => p.PaymentDate >= param.FromDate);

        //             if (param.ToDate.HasValue)
        //                 query = query.Where(p => p.PaymentDate <= param.ToDate);

        //             // Step 2: Enforce maximum export limit
        //             var config = _domainService.GetAllAppConfig().ToList();
        //             var maxExportPaymentList = config.SingleOrDefault(x => x.AppConfigId == (int)AppConfigEnum.MAX_EXPORT_PAYMENT_LIST);

        //             int maxExportLimit = maxExportPaymentList?.IntValue ?? 2000; // Default limit to 1000 rows
        //             var totalRows = query.Count();
        //             // if (totalRows > maxExportLimit)
        //             // {
        //             //     throw new Exception($"Export exceeds the maximum limit of {maxExportLimit} rows. Please narrow down your filters.");
        //             // }

        //             query = query.Take(totalRows);

        //             // Step 3: Pre-fetch and group historical data
        //             var bpkHistory = _domainService.GetAllBpkHistory().ToList()
        //                 .GroupBy(x => x.BpkId)
        //                 .ToDictionary(g => g.Key, g => g.ToList());

        //             // Step 4: Load the Excel template
        //             // var fileTemplate = Path.Combine(Environment.CurrentDirectory, @"Template/template-IncomingPaymentList-report.xlsx");
        //             var fileTemplate = Path.Combine(Environment.CurrentDirectory, @"Template/template-IncomingPaymentList-report-update.xlsx");
        //             if (!File.Exists(fileTemplate))
        //             {
        //                 throw new Exception($"Excel template not found at path: {fileTemplate}");
        //             }

        //             XLWorkbook wb = new XLWorkbook(fileTemplate);
        //             IXLWorksheet ws = wb.Worksheet(1);

        //             // Step 5: Paginate and write data in chunks
        //             int pageSize = 500;
        //             int pageIndex = 0;
        //             int row = 4;
        //             int i = 1;

        //             while (true)
        //             {
        //                 var dataChunk = query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        //                 if (!dataChunk.Any()) break;

        //                 foreach (var item in dataChunk)
        //                 {
        //                     ws.Cell("A" + row).Value = i++;
        //                     ws.Cell("B" + row).Value = item.PaymentDate;
        //                     ws.Cell("B" + row).Style.NumberFormat.Format = "dd.MM.yyyy";
        //                     ws.Cell("C" + row).Value = item.Amount;
        //                     ws.Cell("D" + row).Value = item.CustomerName;
        //                     ws.Cell("E" + row).Value = item.Area;
        //                     ws.Cell("F" + row).Value = item.Branch;
        //                     ws.Cell("G" + row).Value = item.CustomerCode;
        //                     ws.Cell("H" + row).Value = item.InterfaceNumber;
        //                     ws.Cell("I" + row).Value = item.ClearingNumber;
        //                     ws.Cell("J" + row).Value = item.ClearingStatus;
        //                     ws.Cell("K" + row).Value = item.BpkNumber;
        //                     ws.Cell("L" + row).Value = item.BpkStatus;

        //                     // Fetch historical data for BpkId
        //                     if (item.BpkId.HasValue && bpkHistory.TryGetValue(item.BpkId.Value, out var history))
        //                     {
        //                         // Created
        //                         var created = history.FirstOrDefault(h => h.AppActionId == (int)AppActionEnum.Created);
        //                         ws.Cell("M" + row).Value = created?.ActionAt.ToString("yyyy-MM-dd HH:mm:ss") ?? ""; // Format DateTime to string
        //                         ws.Cell("N" + row).Value = created?.ActionBy ?? "";

        //                         // SaveAsDraft
        //                         var saveAsDraft = history.FirstOrDefault(h => h.AppActionId == (int)AppActionEnum.SaveAsDraft);
        //                         ws.Cell("O" + row).Value = saveAsDraft?.ActionAt.ToString("yyyy-MM-dd HH:mm:ss") ?? "";
        //                         ws.Cell("P" + row).Value = saveAsDraft?.ActionBy ?? "";

        //                         // Submitted
        //                         var submitted = history.FirstOrDefault(h => h.AppActionId == (int)AppActionEnum.Submitted);
        //                         ws.Cell("Q" + row).Value = submitted?.ActionAt.ToString("yyyy-MM-dd HH:mm:ss") ?? "";
        //                         ws.Cell("R" + row).Value = submitted?.ActionBy ?? "";

        //                         // Clearing
        //                         var clearing = history.FirstOrDefault(h => h.AppActionId == (int)AppActionEnum.Clearing);
        //                         ws.Cell("S" + row).Value = clearing?.ActionAt.ToString("yyyy-MM-dd HH:mm:ss") ?? "";
        //                         ws.Cell("T" + row).Value = clearing?.ActionBy ?? "";

        //                         // RequestForRevision
        //                         var requestForRevision = history.FirstOrDefault(h => h.AppActionId == (int)AppActionEnum.RequestForRevision);
        //                         ws.Cell("U" + row).Value = requestForRevision?.ActionAt.ToString("yyyy-MM-dd HH:mm:ss") ?? "";
        //                         ws.Cell("V" + row).Value = requestForRevision?.ActionBy ?? "";

        //                         // ReviseApprove
        //                         var reviseApprove = history.FirstOrDefault(h => h.AppActionId == (int)AppActionEnum.ReviseApprove);
        //                         ws.Cell("W" + row).Value = reviseApprove?.ActionAt.ToString("yyyy-MM-dd HH:mm:ss") ?? "";
        //                         ws.Cell("X" + row).Value = reviseApprove?.ActionBy ?? "";

        //                         // ReviseReject
        //                         var reviseReject = history.FirstOrDefault(h => h.AppActionId == (int)AppActionEnum.ReviseReject);
        //                         ws.Cell("Y" + row).Value = reviseReject?.ActionAt.ToString("yyyy-MM-dd HH:mm:ss") ?? "";
        //                         ws.Cell("Z" + row).Value = reviseReject?.ActionBy ?? "";
        //                     }


        //                     row++;
        //                 }

        //                 pageIndex++;
        //             }

        //             // Apply borders to the data range
        //             ws.RangeUsed().Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        //             ws.RangeUsed().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

        //             return wb;
        //         }
        //         catch (Exception ex)
        //         {
        //             Log.Logger.Error($"Method: ExportIncomingPaymentList(), filter: {param.Filter}, sortOrder: {param.SortOrder}, limit: {param.Limit}, page: {param.Page} Message: {ex.Message}");
        //             throw;
        //         }
        //     }).ConfigureAwait(false);
        // }
        
        // try optimize cahyo 2:still only can contain around 4.032 data row
        // public async Task<XLWorkbook> GenerateIncomingPaymentListReport(IncomingPaymentListRequest param)
        // {
        //     return await Task.Run(() =>
        //     {
        //         try
        //         {
        //             // Step 1: Query data with applied filters
        //             var query = QueryIncomingPaymentList(param.Filter, param.SortOrder, param.SortDirection);

        //             if (!string.IsNullOrEmpty(param.Cabang))
        //                 query = query.Where(p => p.Branch == param.Cabang);

        //             if (!string.IsNullOrEmpty(param.Customer))
        //                 query = query.Where(p => p.CustomerCode == param.Customer);

        //             if (!string.IsNullOrEmpty(param.StatusBpk))
        //                 query = query.Where(p => p.BpkStatus == param.StatusBpk);

        //             if (!string.IsNullOrEmpty(param.StatusClearing))
        //                 query = query.Where(p => p.ClearingStatus == param.StatusClearing);

        //             if (!string.IsNullOrEmpty(param.Area))
        //                 query = query.Where(p => p.Area == param.Area);

        //             if (param.FromDate.HasValue)
        //                 query = query.Where(p => p.PaymentDate >= param.FromDate);

        //             if (param.ToDate.HasValue)
        //                 query = query.Where(p => p.PaymentDate <= param.ToDate);

        //             // Step 2: Materialize data
        //             var data = query.ToList(); // Ensure full data is fetched

        //             // Step 3: Pre-fetch and group historical data
        //             var bpkHistory = _domainService.GetAllBpkHistory().ToList()
        //                 .GroupBy(x => x.BpkId)
        //                 .ToDictionary(g => g.Key, g => g.ToList());

        //             // Step 4: Load the Excel template
        //             var fileTemplate = Path.Combine(Environment.CurrentDirectory, @"Template/template-IncomingPaymentList-report.xlsx");
        //             if (!File.Exists(fileTemplate))
        //             {
        //                 throw new Exception($"Excel template not found at path: {fileTemplate}");
        //             }

        //             XLWorkbook wb = new XLWorkbook(fileTemplate);
        //             IXLWorksheet ws = wb.Worksheet(1);

        //             // Step 5: Write all data rows
        //             int row = 4;
        //             int i = 1;

        //             foreach (var item in data)
        //             {
        //                 ws.Cell("A" + row).Value = i++;
        //                 ws.Cell("B" + row).Value = item.PaymentDate;
        //                 ws.Cell("B" + row).Style.NumberFormat.Format = "dd.MM.yyyy";
        //                 ws.Cell("C" + row).Value = item.Amount;
        //                 ws.Cell("D" + row).Value = item.CustomerName;
        //                 ws.Cell("E" + row).Value = item.Area;
        //                 ws.Cell("F" + row).Value = item.Branch;
        //                 ws.Cell("G" + row).Value = item.CustomerCode;
        //                 ws.Cell("H" + row).Value = item.InterfaceNumber;
        //                 ws.Cell("I" + row).Value = item.ClearingNumber;
        //                 ws.Cell("J" + row).Value = item.ClearingStatus;
        //                 ws.Cell("K" + row).Value = item.BpkNumber;
        //                 ws.Cell("L" + row).Value = item.BpkStatus;

        //                 if (item.BpkId.HasValue && bpkHistory.TryGetValue(item.BpkId.Value, out var history))
        //                 {
        //                     var created = history.FirstOrDefault(h => h.AppActionId == (int)AppActionEnum.Created);
        //                     ws.Cell("M" + row).Value = created?.ActionAt.ToString("yyyy-MM-dd HH:mm:ss") ?? "";
        //                     ws.Cell("N" + row).Value = created?.ActionBy ?? "";
        //                 }

        //                 row++;
        //             }

        //             // Apply borders
        //             ws.RangeUsed().Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        //             ws.RangeUsed().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

        //             return wb;
        //         }
        //         catch (Exception ex)
        //         {
        //             Log.Logger.Error($"Method: ExportIncomingPaymentList(), Message: {ex.Message}");
        //             throw;
        //         }
        //     }).ConfigureAwait(false);
        // }



        public async Task<ResultBase> GenerateSapFile(Guid incomingPaymentId)
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

                    #region VariableTemp
                    // var fileTemplate = Path.Combine(Environment.CurrentDirectory, @"Template/template-bpk-rpa-report.xlsx");
                    var fileTemplate = Path.Combine(Environment.CurrentDirectory, @"Template/template-bpk-rpa-report_update.xlsx");
                    Dictionary<string, int> listMonth = new Dictionary<string, int>();
                    listMonth.Add("April", 1);
                    listMonth.Add("May", 2);
                    listMonth.Add("June", 3);
                    listMonth.Add("July", 4);
                    listMonth.Add("August", 5);
                    listMonth.Add("September", 6);
                    listMonth.Add("October", 7);
                    listMonth.Add("November", 8);
                    listMonth.Add("December", 9);
                    listMonth.Add("January", 10);
                    listMonth.Add("February", 11);
                    listMonth.Add("March", 12);
                    #endregion

                    XLWorkbook wb = new XLWorkbook(fileTemplate);
                    IXLWorksheet ws = wb.Worksheet(1);

                    #region Data
                    var incomingPayment = _domainService.GetAllIncomingPayment()
                            .Where(x => x.IncomingPaymentId == incomingPaymentId)
                            .FirstOrDefault();
                    
                    if (incomingPayment == null || incomingPayment.BpkId == null)
                    {
                        result.Message = MessageConstants.S_BPK_NOT_FOUND;

                        return result;
                    }
                    Console.WriteLine($"Incoming Payment Details: {incomingPayment}");

                    var bpk = _domainService.GetAllBpk().Single(x => x.BpkId == incomingPayment.BpkId);

                    var areaName = _domainService.GetAllArea().Single(x => x.AreaId == incomingPayment.AreaId).Name;

                    var branchDropdown = _domainService.GetAllBranch().Select(x => x.BusinessArea).ToList();
                    var validOptions = $"\"{String.Join(",", branchDropdown)}\"";

                    var branch = _domainService.GetAllBranch().Single(x => x.BranchId == incomingPayment.Area.BranchId);
                    var source = _domainService.GetAllSource().Single(x => x.SourceId == incomingPayment.SourceId);
                    var customer = _domainService.GetAllCustomer().Single(x => x.CustomerCode == incomingPayment.CustomerCode);

                    var data = from a in _masterDataService.GetAllActiveBpkDetail().Where(x => x.BpkId == bpk.BpkId)
                               join invoice in _domainService.GetAllInvoice() on a.InvoiceNumber equals invoice.InvoiceNumber into ab
                               from b in ab.DefaultIfEmpty()
                               join potongan in _domainService.GetAllPotongan() on a.PotonganId equals potongan.PotonganId into ac
                               from c in ac.DefaultIfEmpty()
                               join potonganType in _domainService.GetAllPotonganType() on c.PotonganTypeId equals potonganType.PotonganTypeId into cd
                               from d in cd.DefaultIfEmpty()
                               select new BpkResponseExportSap
                               {
                                   InvoiceNumber = a.InvoiceNumber,
                                   CustomerId = a.PotonganId == null ? "" : c.CustomerCode,
                                   BpkNumber = bpk.BpkNumber,
                                   DocumentDate = incomingPayment.PaymentDate.ToString("dd.MM.yyyy"),
                                   PostinganDate = incomingPayment.PostingDate.ToString("dd.MM.yyyy"),
                                   HeaderText = customer.Name,
                                   ValueDate = "",
                                   BlineDate = "",
                                   Period = incomingPayment.PostingDate.ToString("MMMM"),
                                   Type = !String.IsNullOrEmpty(a.InvoiceNumber) ? "Invoice" : d.Name,
                                   TypeId = !String.IsNullOrEmpty(a.InvoiceNumber) ? 0 : d.PotonganTypeId,
                                   Amount = !String.IsNullOrEmpty(a.InvoiceNumber) ? b.Amount : c.Amount,
                                   OtherCriteria = "",
                                   SelectedArea = "",
                                   InterfaceNumber = incomingPayment.InterfaceNumber,
                                   GlAccountNumber = d.GlAccount,
                                   NomorPoEps = c.nomorPoEps,
                                   PstKey = d.PostingKey,
                                   TaxCode = d.TaxCode,
                                   SubAccount = d.SubAccount,
                                   Material = d.Material,
                                   BusinessArea = d.BusinessArea,
                                   CostCentre = d.CostCenter,
                                   TextInSap = d.TextInSap,
                                   AssignmentInSap = "",
                                   PotonganNumber = c.PotonganNumber,
                                   BranchBusinessArea = c.Branch == null ? "" : c.Branch.BusinessArea,
                                   CostCenterChargePO = c.Branch == null ? "" : c.Branch.ChargePoCostCenter
                               };
                    #endregion

                    int row = 1;
                    foreach (var item in data)
                    {
                        row++;
                        #region Condition if Potongan
                        if (item.Type != "Invoice")
                        {
                            switch (item.TypeId)
                            {
                                case (int)PotonganTypeEnum.BankCharge:
                                    item.OtherCriteria = source.Name;
                                    item.SubAccount = source.BankChargeSubAccount;
                                    item.TextInSap = source.BankChargeSubAccount;
                                    item.AssignmentInSap = item.Type;
                                    item.SelectedArea = item.BusinessArea;
                                    break;
                                case (int)PotonganTypeEnum.ChargePO:
                                    item.OtherCriteria = item.BranchBusinessArea;
                                    item.SelectedArea = item.BranchBusinessArea;
                                    item.AssignmentInSap = item.Type;
                                    item.CostCentre = item.CostCenterChargePO;
                                    item.TextInSap = item.SubAccount;
                                    break;
                                case (int)PotonganTypeEnum.Pembulatan:
                                    if (item.Amount < 0)
                                    {
                                        if ((-1) * item.Amount >= 100)
                                        {
                                            item.GlAccountNumber = "7011000010";
                                            item.OtherCriteria = "Amount ≥ -100";
                                            item.PstKey = "50";
                                            item.TaxCode = "OX";
                                            item.TextInSap = "Gain Payment";
                                            item.AssignmentInSap = item.TextInSap;
                                        }

                                        if ((-1) * item.Amount < 100)
                                        {
                                            item.GlAccountNumber = "7011000040";
                                            item.OtherCriteria = "Amount < -100";
                                            item.PstKey = "50";
                                            item.TextInSap = "Gain Payment";
                                            item.AssignmentInSap = item.TextInSap;
                                        }
                                    }
                                    else
                                    {
                                        if (item.Amount >= 100)
                                        {
                                            item.GlAccountNumber = "7510900010";
                                            item.OtherCriteria = "Amount ≥ 100";
                                            item.PstKey = "40";
                                            item.TaxCode = "VX";
                                            item.TextInSap = "Loss Payment";
                                            item.AssignmentInSap = item.TextInSap;
                                        }

                                        if (item.Amount < 100)
                                        {
                                            item.GlAccountNumber = "7510900060";
                                            item.OtherCriteria = "Amount < 100";
                                            item.PstKey = "40";
                                            item.TextInSap = "Loss Payment";
                                            item.AssignmentInSap = item.TextInSap;
                                        }
                                    }

                                    item.SelectedArea = branch.BusinessArea;
                                    break;
                                case (int)PotonganTypeEnum.LebihBayar:
                                case (int)PotonganTypeEnum.KurangBayar:
                                case (int)PotonganTypeEnum.Promosi:
                                case (int)PotonganTypeEnum.Return:
                                    item.GlAccountNumber = item.CustomerId;
                                    if (item.TypeId == (int)PotonganTypeEnum.Promosi || item.TypeId == (int)PotonganTypeEnum.Return)
                                    {
                                        if (item.TypeId == (int)PotonganTypeEnum.Promosi){
                                            item.TextInSap = item.NomorPoEps + "_" + item.PotonganNumber + "_" + item.InterfaceNumber;
                                        }else{
                                            item.TextInSap = item.PotonganNumber + "_" + item.InterfaceNumber;
                                        }
                                    }
                                    else if (item.TypeId == (int)PotonganTypeEnum.KurangBayar || item.TypeId == (int)PotonganTypeEnum.LebihBayar)
                                    {
                                        if (item.TypeId == (int)PotonganTypeEnum.KurangBayar)
                                        {
                                            item.TextInSap = "KURANG BAYAR_" + item.PotonganNumber + "_" + item.BpkNumber;
                                        }
                                        else
                                        {
                                            item.TextInSap = "LEBIH BAYAR_" + item.PotonganNumber + "_" + item.BpkNumber;
                                        }
                                    }
                                    else
                                    {
                                        item.TextInSap = item.Type + "_" + item.BpkNumber;
                                    }
                                    item.SelectedArea = item.BranchBusinessArea;
                                    item.AssignmentInSap = item.Type;
                                    break;
                                default:
                                    item.SelectedArea = item.BranchBusinessArea;
                                    item.AssignmentInSap = item.Type;
                                    break;
                            }

                            #region ValueDate & BlineDate
                            if (item.PstKey == "06" || item.PstKey == "16")
                            {
                                item.BlineDate = item.DocumentDate;
                            }
                            else if (item.PstKey == "40" || item.PstKey == "50" || !String.IsNullOrEmpty(item.GlAccountNumber))
                            {
                                item.ValueDate = item.DocumentDate;
                            }
                            #endregion
                        }
                        #endregion

                        // Invoice No
                        ws.Cell("A" + row).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("A" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("A" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("A" + row).Value = item.InvoiceNumber;

                        // Customer ID
                        ws.Cell("B" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("B" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("B" + row).Value = incomingPayment.CustomerCode;

                        // BPK No
                        ws.Cell("C" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("C" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("C" + row).Value = item.BpkNumber;

                        // Document Date
                        ws.Cell("D" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("D" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("D" + row).Value = ExcelConstant.S_STRING_CONVERTER + item.DocumentDate;

                        // Posting Date
                        ws.Cell("E" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("E" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("E" + row).Value = ExcelConstant.S_STRING_CONVERTER + item.PostinganDate;

                        // Doc Header Text
                        ws.Cell("F" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F" + row).Value = item.HeaderText;

                        // Clearing Text
                        ws.Cell("G" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G" + row).Value = item.HeaderText;

                        // Value Date
                        ws.Cell("H" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H" + row).Value = ExcelConstant.S_STRING_CONVERTER + item.ValueDate;

                        // Bline Date
                        ws.Cell("I" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I" + row).Value = ExcelConstant.S_STRING_CONVERTER + item.BlineDate;

                        // Periode
                        ws.Cell("J" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J" + row).Value = listMonth.FirstOrDefault(x => x.Key == item.Period).Value.ToString();

                        // Tipe
                        ws.Cell("K" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K" + row).Value = item.Type;

                        // Amount
                        ws.Cell("L" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("L" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("L" + row).Value = item.Amount;

                        // Other Criteria
                        ws.Cell("M" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M" + row).Value = item.OtherCriteria;

                        // Selected Area Based on DDL Area Bukti Potong
                        ws.Cell("N" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("N" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("N" + row).Value = item.SelectedArea;

                        // No Interface
                        ws.Cell("O" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O" + row).Value = item.InterfaceNumber;

                        // GL Account No
                        ws.Cell("P" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("P" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("P" + row).Value = item.GlAccountNumber;

                        // Pst Key (40/50) GL Promosi
                        if(item.AssignmentInSap == "Promosi"){
                            ws.Cell("Q" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell("Q" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            // ws.Cell("Q" + row).Value = ExcelConstant.S_STRING_CONVERTER + item.NomorPoEps;
                            ws.Cell("Q" + row).Value = "1011602040";
                        }
                        // else{
                        //     // Pst Key (40/50)
                        //     // ws.Cell("Q" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        //     // ws.Cell("Q" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        //     // ws.Cell("Q" + row).Value = ExcelConstant.S_STRING_CONVERTER + item.PstKey;
                        // }

                        // Pst Key (40/50)
                        ws.Cell("R" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("R" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("R" + row).Value = ExcelConstant.S_STRING_CONVERTER + item.PstKey;

                        // Tax Code
                        ws.Cell("S" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("S" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("S" + row).Value = item.TaxCode;

                        // Sub Account
                        ws.Cell("T" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("T" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("T" + row).Value = item.SubAccount;

                        // Material
                        ws.Cell("U" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("U" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("U" + row).Value = item.Material;

                        // Bussiness Area
                        ws.Cell("V" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("V" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("V" + row).Value = item.SelectedArea;
                        ws.Cell("V" + row).DataValidation.List(validOptions, true);

                        // Cost Center
                        ws.Cell("W" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("W" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("W" + row).Value = item.CostCentre;

                        // Text In SAP
                        ws.Cell("X" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("X" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("X" + row).Value = item.TextInSap;

                        // Assignment in SAP
                        ws.Cell("Y" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Y" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Y" + row).Value = item.AssignmentInSap;
                    }

                    IXLWorksheet ws2 = wb.Worksheet(2);
                    ws2.Cell("B1").Value = incomingPayment.IncomingPaymentId.ToHexString();
                    ws2.Cell("B2").Value = incomingPayment.BpkId.Value.ToHexString();

                    var filePath = BusinessHelper.GetSapFilePath(_appSettings.SharingFolder,
                        incomingPayment.InterfaceNumber, incomingPayment.PaymentDate);

                    wb.SaveAs(filePath);

                    result.Success = true;
                    result.Message = filePath;

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: GenerateSapFile(), incomingPaymentId: {incomingPaymentId}, " +
                        $"Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        private IQueryable<IncomingPaymentDto> QueryIncomingPaymentList(string filter,
            IncomingPaymentColumn? sortOrder,
            SortingDirection? sortDirection)
        {
            var userLogin = _profileService.GetUserLogin();
            var areaIds = userLogin.AreaIds;

            // Adjust the query to handle null values using ?? (null coalescing operator)
            var query = from a in _domainService.GetAllIncomingPaymentView()
                        where areaIds.Contains(a.AreaId)
                        select new IncomingPaymentDto
                        {
                            IncomingPaymentId = a.IncomingPaymentId,
                            BpkId = a.BpkId,
                            BpkNumber = a.BpkNumber ?? "", // Ensure BpkNumber is never null
                            PaymentDate = a.PaymentDate,
                            PaymentDateString = a.PaymentDateString ?? "", // Ensure PaymentDateString is never null
                            Amount = a.Amount,
                            AmountString = a.AmountString ?? "", // Ensure AmountString is never null
                            CustomerCode = a.CustomerCode ?? "", // Ensure CustomerCode is never null
                            CustomerName = a.CustomerName ?? "", // Ensure CustomerName is never null
                            Area = a.AreaName ?? "", // Ensure AreaName is never null
                            Branch = a.BranchName ?? "", // Ensure BranchName is never null
                            InterfaceNumber = a.InterfaceNumber ?? "", // Ensure InterfaceNumber is never null
                            ClearingNumber = a.ClearingNumber ?? "", // Ensure ClearingNumber is never null
                            BpkStatusId = a.BpkId == null ? (int)BpkStatusEnum.BpkNotCreated : a.BpkStatusId,
                            BpkStatus = a.BpkId == null ? BpkConstants.S_BPK_NOT_CREATED : a.BpkStatus ?? "", // Ensure BpkStatus is never null
                            ClearingStatusId = a.ClearingStatusId,
                            ClearingStatus = a.ClearingStatus ?? "", // Ensure ClearingStatus is never null
                            IsClearingManual = a.IsClearingManual
                        };

            // Apply filter if provided
            if (!string.IsNullOrEmpty(filter))
            {
                filter = filter.ToUpper();
                query = query.Where(x =>
                    x.PaymentDateString.ToUpper().Contains(filter)
                    || x.CustomerName.ToUpper().Contains(filter)
                    || x.CustomerCode.ToUpper().Contains(filter)
                    || x.Area.ToUpper().Contains(filter)
                    || x.Branch.ToUpper().Contains(filter)
                    || x.InterfaceNumber.ToUpper().Contains(filter)
                    || x.ClearingNumber.ToUpper().Contains(filter)
                    || x.ClearingStatus.ToUpper().Contains(filter)
                    || x.BpkNumber.ToUpper().Contains(filter)
                    || x.BpkStatus.ToUpper().Contains(filter)
                    || x.AmountString.Contains(filter)
                );
            }

            // Apply sorting based on sortOrder and sortDirection
            sortOrder = sortOrder ?? IncomingPaymentColumn.PaymentDate;
            sortDirection = sortDirection ?? SortingDirection.Descending;

            switch (sortOrder)
            {
                case IncomingPaymentColumn.PaymentDate:
                    query = sortDirection == SortingDirection.Ascending
                        ? query.OrderBy(x => x.PaymentDate)
                        : query.OrderByDescending(x => x.PaymentDate);
                    break;
                case IncomingPaymentColumn.CustomerName:
                    query = sortDirection == SortingDirection.Ascending
                        ? query.OrderBy(x => x.CustomerName)
                        : query.OrderByDescending(x => x.CustomerName);
                    break;
                default:
                    query = query.OrderByDescending(x => x.PaymentDate);
                    break;
            }

            return query;
        }

        private IQueryable<InvoiceDetailsDto> QueryInvoiceDetailstList(string filter,
            InvoiceDetailsColumn? sortOrder,
            SortingDirection? sortDirection)
        {
            var userLogin = _profileService.GetUserLogin();
            var areaIds = userLogin.AreaIds;

            // Adjust the query to match invoice_details table structure
            var query = from a in _domainService.GetAllInvoiceDetailsView()
                        select new InvoiceDetailsDto
                        {
                            InvoiceDetailsId = a.InvoiceDetailsId,
                            CabangId = a.CabangId ?? "", 
                            CustomerName = a.CustomerName ?? "", 
                            SalesGrup = a.SalesGrup ?? "", 
                            FiscalYear = a.FiscalYear ?? "",
                            IDCustomerSoldTo = a.IDCustomerSoldTo,
                            DocumentNumber = a.DocumentNumber,
                            NoInvoice = a.NoInvoice ?? "", 
                            NoPO = a.NoPO ?? "", 
                            AmtInLocCur = a.AmtInLocCur,
                            ShipTo = a.ShipTo,
                            Store = a.Store ?? "", 
                            TextDesc = a.TextDesc ?? "", 
                            DocDate = a.DocDate,
                            BaselineDate = a.BaselineDate,
                            NetDueDt = a.NetDueDt,
                            NoSubmitted = a.NoSubmitted ?? "", 
                            StatusTukarFaktur = a.StatusTukarFaktur ?? "Not Ok",
                            Status = a.Status ?? "Not Created",
                            Action = a.Action ?? "Draft",
                            BusA = a.BusA,
                            TglKirimBarang = a.TglKirimBarang,
                            TempatTukarFaktur = a.TempatTukarFaktur ?? "",
                            TgKirimBerkasKeKAMT = a.TgKirimBerkasKeKAMT,
                            TglTerimaDOBack = a.TglTerimaDOBack,
                            TglTerimaFakturPajak = a.TglTerimaFakturPajak,
                            TglCompletedDoc = a.TglCompletedDoc,
                            TglTukarFaktur = a.TglTukarFaktur,
                            TanggalBayar = a.TanggalBayar,
                            TglTerimaBerkas = a.TglTerimaBerkas,
                            IdealTukarFaktur = a.IdealTukarFaktur,
                            TOPOutlet = a.TOPOutlet,
                            OverdueDatabaseSAP = a.OverdueDatabaseSAP ?? "",
                            StatusOverdueDatabase = a.StatusOverdueDatabase ?? "",
                            StatusInternalSAP = a.StatusInternalSAP ?? "",
                            StatusExternalTOPOutlet = a.StatusExternalTOPOutlet ?? "",
                            Keterangan = a.Keterangan ?? "",
                            ActionCabangMT = a.ActionCabangMT ?? "",
                            RealisasiCabangMT = a.RealisasiCabangMT ?? "",
                            CreatedBy = a.CreatedBy ?? "",
                            UpdatedAt = a.UpdatedAt,
                            UpdatedBy = a.UpdatedBy
                        };

            // Apply filter if provided
            if (!string.IsNullOrEmpty(filter))
            {
                filter = filter.ToUpper();
                query = query.Where(x =>
                    x.CabangId.ToUpper().Contains(filter)
                    || x.IDCustomerSoldTo.ToString().Contains(filter)
                    || x.Status.ToUpper().Contains(filter)
                    || x.StatusTukarFaktur.ToUpper().Contains(filter)
                );
            }

            // Apply sorting based on sortOrder and sortDirection
            sortOrder = sortOrder ?? InvoiceDetailsColumn.DocDate;
            sortDirection = sortDirection ?? SortingDirection.Descending;

            switch (sortOrder)
            {
                case InvoiceDetailsColumn.DocDate:
                    query = sortDirection == SortingDirection.Ascending
                        ? query.OrderBy(x => x.DocDate)
                        : query.OrderByDescending(x => x.DocDate);
                    break;
                case InvoiceDetailsColumn.CustomerName:
                    query = sortDirection == SortingDirection.Ascending
                        ? query.OrderBy(x => x.CustomerName)
                        : query.OrderByDescending(x => x.CustomerName);
                    break;
                default:
                    query = query.OrderByDescending(x => x.DocDate);
                    break;
            }

            return query;
        }

        // private IQueryable<IncomingPaymentDto> QueryIncomingPaymentList(string filter,
        // IncomingPaymentColumn? sortOrder,
        // SortingDirection? sortDirection)
        // {
        //     var userLogin = _profileService.GetUserLogin();
        //     var areaIds = userLogin.AreaIds;

        //     var query = from a in _domainService.GetAllIncomingPaymentView()
        //                 where areaIds.Contains(a.AreaId)
        //                 select new IncomingPaymentDto
        //                 {
        //                     IncomingPaymentId = a.IncomingPaymentId,
        //                     BpkId = a.BpkId,
        //                     BpkNumber = a.BpkNumber ?? "",
        //                     PaymentDate = a.PaymentDate,
        //                     PaymentDateString = a.PaymentDateString ?? "",
        //                     Amount = a.Amount,
        //                     AmountString = a.AmountString,
        //                     CustomerCode = a.CustomerCode,
        //                     CustomerName = a.CustomerName,
        //                     Area = a.AreaName,
        //                     Branch = a.BranchName,
        //                     InterfaceNumber = a.InterfaceNumber,
        //                     ClearingNumber = a.ClearingNumber ?? "",
        //                     BpkStatusId = a.BpkId == null ? (int)BpkStatusEnum.BpkNotCreated : a.BpkStatusId,
        //                     BpkStatus = a.BpkId == null ? BpkConstants.S_BPK_NOT_CREATED : a.BpkStatus ?? "",
        //                     ClearingStatusId = a.ClearingStatusId,
        //                     ClearingStatus = a.ClearingStatus,
        //                     IsClearingManual = a.IsClearingManual
        //                 };

        //     if (string.IsNullOrEmpty(filter) == false)
        //     {
        //         filter = filter.ToUpper();
        //         query = query.Where(x =>
        //                x.PaymentDateString.ToUpper().Contains(filter)
        //             || x.CustomerName.ToUpper().Contains(filter)
        //             || x.CustomerCode.ToUpper().Contains(filter)
        //             || x.Area.ToUpper().Contains(filter)
        //             || x.Branch.ToUpper().Contains(filter)
        //             || x.InterfaceNumber.ToUpper().Contains(filter)
        //             || x.ClearingNumber.ToUpper().Contains(filter)
        //             || x.ClearingStatus.ToUpper().Contains(filter)
        //             || x.BpkNumber.ToUpper().Contains(filter)
        //             || x.BpkStatus.ToUpper().Contains(filter)
        //             || x.AmountString.Contains(filter)
        //             );
        //     }

        //     sortOrder = sortOrder == null ? IncomingPaymentColumn.PaymentDate : sortOrder;
        //     sortDirection = sortDirection == null ? SortingDirection.Descending : sortDirection;

        //     switch (sortOrder)
        //     {
        //         case IncomingPaymentColumn.PaymentDate:
        //             if (sortDirection == SortingDirection.Ascending)
        //                 query = query.OrderBy(x => x.PaymentDate);
        //             else
        //                 query = query.OrderByDescending(x => x.PaymentDate);
        //             break;
        //         case IncomingPaymentColumn.CustomerName:
        //             if (sortDirection == SortingDirection.Ascending)
        //                 query = query.OrderBy(x => x.CustomerName);
        //             else
        //                 query = query.OrderByDescending(x => x.CustomerName);
        //             break;
        //         default:
        //             query = query.OrderByDescending(x => x.PaymentDate);
        //             break;
        //     }

        //     return query;
        // }

        public async Task<ResultBase> RemoveIncomingPayment(Guid incomingPaymentId)
        {
            return await Task.Run(() =>
            {
                var result = new ResultBase
                {
                    Success = false,
                    Message = ""
                };
                var filePath = "";

                try
                {
                    var incomingPayment = _domainService.GetAllIncomingPayment().SingleOrDefault(x => x.IncomingPaymentId == incomingPaymentId);

                    if (incomingPayment == null)
                    {
                        result.Message = MessageConstants.S_INCOMING_PAYMENT_NOT_FOUND;
                        return result;
                    }

                    if (incomingPayment.ClearingStatusId != (int)ClearingStatusEnum.NotYet)
                    {
                        result.Message = MessageConstants.S_INCOMING_PAYMENT_OPEN_CLEARING;
                        return result;
                    }

                    var bpk = _domainService.GetAllBpk().SingleOrDefault(x => x.BpkId == incomingPayment.BpkId);

                    if (bpk != null && bpk.BpkStatusId == (int)BpkStatusEnum.BpkSubmitted)
                    {
                        filePath = BusinessHelper.GetSapFilePath(_appSettings.SharingFolder,
                         incomingPayment.InterfaceNumber, incomingPayment.PaymentDate);

                        if (!File.Exists(filePath))
                        {
                            result.Message = MessageConstants.S_BPK_REQUEST_FILE_NOT_EXIST;
                            return result;
                        }
                    }

                    //Delete Bpk History
                    var bpkHistory = _domainService.GetAllBpkHistory().Where(x => x.BpkId == incomingPayment.BpkId);
                    foreach (var item in bpkHistory)
                    {
                        _domainService.DeleteBpkHistory(item);
                    }

                    var bpkDetail = _domainService.GetAllBpkDetail().Where(x => x.BpkId == incomingPayment.BpkId).ToList();
                    foreach (var item in bpkDetail)
                    {
                        // Delete Potongan
                        if (item.PotonganId != null)
                        {
                            var potongan = _domainService.GetAllPotongan().Single(x => x.PotonganId == item.PotonganId);
                            _domainService.DeletePotongan(potongan);
                        }

                        // Delete BpkDetail
                        _domainService.DeleteBpkDetail(item);
                    }

                    // Delete Incoming Payment
                    _domainService.DeleteIncomingPayment(incomingPayment);

                    // Delete BPK
                    if (bpk != null)
                        _domainService.DeleteBpk(bpk);

                    result.Success = true;
                    result.Message = MessageConstants.S_INCOMING_PAYMENT_REMOVED;

                    _domainService.SaveChanges();

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: GenerateSapFile(), incomingPaymentId: {incomingPaymentId}, " +
                        $"Message: {ex.Message}");
                    throw;
                }
                finally
                {
                    if (result.Success && filePath != "")
                        // Delete File Clearing
                        File.Delete(filePath);
                }
            }).ConfigureAwait(false);
        }

        public async Task<ResultBase> RevertResettedBpkFromClearing()
        {
            return await Task.Run(() =>
            {
                var transaction = _domainService.BeginTransaction();

                try
                {
                    var result = new ResultBase
                    {
                        Success = false
                    };

                    var limitDate = new DateTime(2023, 3, 31);
                    var limitDateDateOnly = new DateOnly(limitDate.Year, limitDate.Month, limitDate.Day);
                    var incPaymentLimitDate = _domainService.GetAllIncomingPayment()
                        .Where(x => x.PaymentDate > limitDateDateOnly)
                        .OrderBy(x => x.CreatedAt)
                        .First().CreatedAt;

                    var query = from a in _domainService.GetAllBpk()
                                join ip in _domainService.GetAllIncomingPayment() on a.BpkNumber.Substring(11) equals ip.InterfaceNumber into ab
                                from b in ab.DefaultIfEmpty()
                                where
                                    a.DeletedFlag &&
                                    a.CreatedAt >= incPaymentLimitDate &&
                                    b.PaymentDate > limitDateDateOnly
                                select new
                                {
                                    BpkOriginalId = a.BpkId,
                                    BpkReplacementId = b.BpkId ?? Guid.Empty,
                                    b.IncomingPaymentId,
                                    b.InterfaceNumber
                                };

                    var tobeFixedBpk = query.ToList();

                    if (tobeFixedBpk.Where(x => x.IncomingPaymentId == null).ToList().Count > 0)
                    {
                        throw new Exception("empty incoming payment id detected");
                    }

                    if (tobeFixedBpk.Count > tobeFixedBpk.Select(x => x.InterfaceNumber).Distinct().ToList().Count)
                    {
                        throw new Exception("same interface number detected");
                    }

                    var ids = tobeFixedBpk.Select(x => x.IncomingPaymentId).ToList();
                    var incPayments = _domainService.GetAllIncomingPayment()
                        .Where(x => ids.Contains(x.IncomingPaymentId))
                        .ToList();

                    ids = tobeFixedBpk.Select(x => x.BpkOriginalId).ToList();
                    var bpkOriginals = _domainService.GetAllBpk()
                        .Where(x => ids.Contains(x.BpkId))
                        .ToList();

                    ids = bpkOriginals.Select(x => x.BpkId).ToList();
                    var bpkOriginalDetails = _domainService.GetAllBpkDetail()
                        .Where(x => ids.Contains(x.BpkId))
                        .ToList();

                    ids = tobeFixedBpk.Select(x => x.BpkReplacementId).ToList();
                    var bpkReplacements = _domainService.GetAllBpk()
                        .Where(x => ids.Contains(x.BpkId))
                        .ToList();

                    ids = bpkReplacements.Select(x => x.BpkId).ToList();
                    var bpkReplacementDetails = _domainService.GetAllBpkDetail()
                        .Where(x => ids.Contains(x.BpkId))
                        .ToList();

                    var updateBpks = new List<Bpk>();
                    var updateBpkDetails = new List<BpkDetail>();
                    var updateIncPayments = new List<IncomingPayment>();

                    foreach (var item in tobeFixedBpk)
                    {
                        // set flag delete bpk replacement and its details
                        var bpkReplacement = bpkReplacements.Single(x => x.BpkId == item.BpkReplacementId);
                        bpkReplacement.DeletedFlag = true;

                        updateBpks.Add(bpkReplacement);

                        var replaceDatetime = bpkReplacement.CreatedAt;

                        var lsDetails = bpkReplacementDetails.Where(x => x.BpkId == bpkReplacement.BpkId).ToList();
                        foreach (var dtl in lsDetails)
                        {
                            dtl.DeletedFlag = true;

                            updateBpkDetails.Add(dtl);
                        }

                        // restore bpk original and its details
                        var bpkOriginal = bpkOriginals.Single(x => x.BpkId == item.BpkOriginalId);
                        bpkOriginal.DeletedFlag = false;

                        updateBpks.Add(bpkOriginal);

                        lsDetails = bpkOriginalDetails.Where(x => x.BpkId == bpkOriginal.BpkId).ToList();
                        foreach (var dtl in lsDetails)
                        {
                            dtl.DeletedFlag = false;

                            updateBpkDetails.Add(dtl);
                        }

                        // re-assign original to incoming payment
                        var payment = incPayments.Single(x => x.IncomingPaymentId == item.IncomingPaymentId);
                        payment.BpkId = item.BpkOriginalId;

                        updateIncPayments.Add(payment);

                        // give history clearing on bpk original
                        _historyService.AddBpkHistory(payment, bpkOriginal, replaceDatetime, (int)AppActionEnum.OpenClearing);
                    }

                    // execute bulk
                    _domainService.UpdateBulkIncomingPayment(updateIncPayments);
                    _domainService.UpdateBulkBpk(updateBpks);
                    _domainService.UpdateBulkBpkDetail(updateBpkDetails);

                    _domainService.SaveChanges();
                    transaction.Commit();

                    result.Success = true;
                    result.Message = string.Format("{0} incoming payment, {1} bpk, {2} bpk details.", 
                        updateIncPayments.Count,
                        updateBpks.Count,
                        updateBpkDetails.Count);

                    return result;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    Log.Logger.Error($"Method: RevertResettedBpkFromClearing(), " +
                       $"Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<ResultBase> UpdateBaseLine(UpdateBaseLineRequest request)
        {
        
            var result = new ResultBase { Success = false };
            return await Task.Run(async () =>
            {    
                try
                {
                    if (request.InvoiceDetailsId == null)
                    {
                        result.Message = "Invoice details ID is null.";
                        return result;
                    }

                    var invoiceDetails = _domainService.GetInvoiceDetailsById(request.InvoiceDetailsId).FirstOrDefault();
                    if (invoiceDetails == null)
                    {
                        result.Message = "Invoice details not found.";
                        return result;
                    }

                    if (request.TglTerimaDoBack != null)
                    {
                        invoiceDetails.TglTerimaDOBack = request.TglTerimaDoBack;
                    }
                    if (request.TglTerimaFakturPajak != null)
                    {
                        invoiceDetails.TglTerimaFakturPajak = request.TglTerimaFakturPajak;
                    }
                    if (request.TglCompletedDoc != null)
                    {
                        invoiceDetails.TglCompletedDoc = request.TglCompletedDoc;
                    }
                    if (request.TglTukarFaktur != null)
                    {
                        invoiceDetails.TglTukarFaktur = request.TglTukarFaktur;
                    }

                    _domainService.UpdateInvoiceDetails(invoiceDetails);

                    result.Success = true;
                    result.Message = "Invoice details updated successfully.";
                    return result;
                }
                catch (Exception ex)
                {
                    result.Message = $"Error updating invoice details: {ex.Message}";
                    return result;
                }
            });
        }
    }
}
