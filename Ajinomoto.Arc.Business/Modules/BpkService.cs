using Ajinomoto.Arc.Business.Helper;
using Ajinomoto.Arc.Business.Interfaces;
using Ajinomoto.Arc.Common.AppModels;
using Ajinomoto.Arc.Common.Constants;
using Ajinomoto.Arc.Common.DtoModels;
using Ajinomoto.Arc.Common.Enums;
using Ajinomoto.Arc.Common.Extensions;
using Ajinomoto.Arc.Common.Helpers;
using Ajinomoto.Arc.Data.Models;
using ClosedXML.Excel;
using Microsoft.Extensions.Options;
using Serilog;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace Ajinomoto.Arc.Business.Modules
{
    public class BpkService : IBpkService
    {
        private readonly IDomainService _domainService;
        private readonly IMailService _mailService;
        private readonly IIncomingPaymentService _incomingPaymentService;
        private readonly IProfileService _profileService;
        private readonly IUserService _userService;
        private readonly IHistoryService _historyService;
        private readonly IMasterDataService _masterDataService;

        private readonly AppSettings _appSettings;
        private readonly string _formatDate = "dd-MM-yyyy";
        private readonly string _dbFormatDate = "yyyy-MM-dd";

        public BpkService(IDomainService domainService,
            IMailService mailService,
            IOptions<AppSettings> appSettings,
            IIncomingPaymentService incomingPaymentService,
            IProfileService profileService,
            IUserService userService,
            IHistoryService historyService,
            IMasterDataService masterDataService)
        {
            _domainService = domainService;
            _mailService = mailService;
            _appSettings = appSettings.Value;
            _incomingPaymentService = incomingPaymentService;
            _profileService = profileService;
            _userService = userService;
            _historyService = historyService;
            _masterDataService = masterDataService;
        }

        public async Task<XLWorkbook> GenerateBpkReport(Guid incomingPaymentId)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var Data = GetBpk(incomingPaymentId);
                    // windows
                    // var fileTemplate = Path.Combine(Environment.CurrentDirectory, @"Template\template-bpk-report.xlsx");
                    // mac
                    var fileTemplate = Path.Combine(Environment.CurrentDirectory, @"Template/template-bpk-report-v2.xlsx");

                    XLWorkbook wb = new XLWorkbook(fileTemplate);
                    IXLWorksheet ws = wb.Worksheet("Case - Retur");

                    #region Header
                    ws.Cell("A2").Value = Data.Result.BpkNumber;

                    var incomingDate = DateTime.ParseExact(Data.Result.Date, _formatDate, CultureInfo.InvariantCulture);
                    ws.Cell("C2").Value = incomingDate.ToLocalTime();

                    ws.Cell("C3").Value = Data.Result.AmountReceived;
                    ws.Cell("F3").Value = Data.Result.InterfaceNumber;
                    ws.Cell("C4").Value = Data.Result.PaymentType;
                    ws.Cell("F4").Value = Data.Result.CustomerName;
                    ws.Cell("O4").Value = DateTime.Now.ToLocalTime();
                    ws.Cell("F5").Value = Data.Result.CustomerCode;
                    #endregion

                    #region Table
                    int initialRow = 8;
                    int rows = initialRow; //last Row Used
                    int rowLength = 38; //Max Row in Template
                    int invoiceCount = Data.Result.Invoices.Count;

                    #region Invoice
                    foreach (var item in Data.Result.Invoices)
                    {
                        rows++;

                        //insert new row
                        if (rows >= rowLength)
                        {
                            ws.Row(rows).InsertRowsAbove(1);
                            rowLength++;
                        }

                        ws.Cell("B" + rows).Value = item.CustomerName;
                        ws.Cell("C" + rows).Value = item.InvoiceNumber;
                        ws.Cell("D" + rows).Value = item.Amount;

                        var invoiceDate = DateTime.ParseExact(item.InvoiceDate, _formatDate, CultureInfo.InvariantCulture);
                        ws.Cell("E" + rows).Value = invoiceDate.ToLocalTime();

                    }
                    #endregion

                    #region Potongan
                    int potonganCount = 0;
                    rows = initialRow;
                    int rowsRetur, rowsPromosi, rowsCreditMemo, rowsChargePo, rowsBiayaBank, RowsPembulatan;
                    rowsRetur = rowsPromosi = rowsCreditMemo = rowsChargePo = rowsBiayaBank = RowsPembulatan = initialRow;
                    foreach (var item in Data.Result.Potongans)
                    {
                        switch (item.PotonganTypeId)
                        {
                            case (int)PotonganTypeEnum.Return:
                                rowsRetur++;
                                rows = rowsRetur;
                                if (rows >= rowLength)
                                {
                                    ws.Row(rows).InsertRowsAbove(1);
                                    rowLength++;
                                }
                                ws.Cell("F" + rows).Value = item.Amount;
                                ws.Cell("L" + rows).Value = ExcelConstant.S_STRING_CONVERTER + item.PotonganNumber;
                                break;
                            case (int)PotonganTypeEnum.Promosi:
                                rowsPromosi++;
                                rows = rowsPromosi;
                                if (rows >= rowLength)
                                {
                                    ws.Row(rows).InsertRowsAbove(1);
                                    rowLength++;
                                }
                                ws.Cell("G" + rows).Value = item.Amount;
                                ws.Cell("M" + rows).Value = ExcelConstant.S_STRING_CONVERTER + item.PotonganNumber;
                                ws.Cell("N" + rows).Value = ExcelConstant.S_STRING_CONVERTER + item.nomorPoEps;
                                break;
                            case (int)PotonganTypeEnum.CreditMemo:
                            case (int)PotonganTypeEnum.DebitMemo:
                            case (int)PotonganTypeEnum.KurangBayar:
                            case (int)PotonganTypeEnum.LebihBayar:
                                rowsCreditMemo++;
                                rows = rowsCreditMemo;
                                if (rows >= rowLength)
                                {
                                    ws.Row(rows).InsertRowsAbove(1);
                                    rowLength++;
                                }
                                ws.Cell("H" + rows).Value = item.Amount;
                                // ws.Cell("O" + rows).Value = ExcelConstant.S_STRING_CONVERTER + item.PotonganNumber;
                                ws.Cell("P" + rows).Value = ExcelConstant.S_STRING_CONVERTER + item.PotonganNumber;
                                break;
                            case (int)PotonganTypeEnum.ChargePO:
                                rowsChargePo++;
                                rows = rowsChargePo;
                                if (rows >= rowLength)
                                {
                                    ws.Row(rows).InsertRowsAbove(1);
                                    rowLength++;
                                }
                                ws.Cell("I" + rows).Value = item.Amount;
                                break;
                            case (int)PotonganTypeEnum.BankCharge:
                                rowsBiayaBank++;
                                rows = rowsBiayaBank;
                                if (rows >= rowLength)
                                {
                                    ws.Row(rows).InsertRowsAbove(1);
                                    rowLength++;
                                }
                                ws.Cell("J" + rows).Value = item.Amount;
                                break;
                            case (int)PotonganTypeEnum.Pembulatan:
                                RowsPembulatan++;
                                rows = RowsPembulatan;
                                if (rows >= rowLength)
                                {
                                    ws.Row(rows).InsertRowsAbove(1);
                                    rowLength++;
                                }
                                ws.Cell("K" + rows).Value = item.Amount;
                                break;
                            default:
                                break;
                        }

                        potonganCount = potonganCount < rows ? rows : potonganCount;
                    }

                    potonganCount -= initialRow;

                    #endregion

                    #region Netto Pembayaran
                    int rowCount = (invoiceCount >= potonganCount) ? invoiceCount : potonganCount;

                    for (int i = 1; i <= rowCount; i++)
                    {
                        ws.Cell("A" + (i + initialRow)).Value = i;
                        // ws.Cell("O" + (i + initialRow)).FormulaR1C1 = "=RC[-11]-RC[-9]-RC[-8]-RC[-7]-RC[-5]-RC[-4]";
                        ws.Cell("P" + (i + initialRow)).FormulaR1C1 = "=RC[-12]-RC[-10]-RC[-9]-RC[-8]-RC[-6]-RC[-5]";
                    }
                    #endregion


                    #endregion

                    return wb;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: ExportIncomingPaymentList(), filter: {incomingPaymentId} Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<BpkResponse?> GetBpk(Guid incomingPaymentId)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var incomingPayment = _domainService.GetAllIncomingPayment()
                        .SingleOrDefault(x => x.IncomingPaymentId == incomingPaymentId);

                    if (incomingPayment == null) return null;

                    var result = new BpkResponse
                    {
                        InterfaceNumber = incomingPayment.InterfaceNumber,
                        CustomerCode = incomingPayment.CustomerCode,
                        CustomerName = _domainService.GetAllCustomer().First(x => x.CustomerCode == incomingPayment.CustomerCode).Name,
                        Date = incomingPayment.PaymentDate.ToString(_formatDate),
                        PaymentType = BpkConstants.S_FORM_TRANSFER,
                        AmountReceived = incomingPayment.Amount,
                        Status = BpkConstants.S_BPK_NOT_CREATED
                    };

                    if (incomingPayment.BpkId == null) return result;

                    var limitPembulatan = _domainService.GetAllAppConfig()
                        .Single(x => x.AppConfigId == (int)AppConfigEnum.MAX_PEMBULATAN)
                        .IntValue;

                    result.LimitPembulatan = limitPembulatan;

                    var bpk = _domainService.GetAllBpk().Single(x => x.BpkId == incomingPayment.BpkId);
                    result.BpkNumber = bpk.BpkNumber;

                    var bpkDetails = _masterDataService.GetAllActiveBpkDetail().Where(x => x.BpkId == bpk.BpkId).ToList();

                    var queryInvoice = from a in bpkDetails.Where(x => x.InvoiceNumber != null)
                                       join b in _domainService.GetAllInvoice() on a.InvoiceNumber equals b.InvoiceNumber
                                       join c in _domainService.GetAllCustomer() on b.CustomerCode equals c.CustomerCode
                                       orderby b.InvoiceDate ascending
                                       select new BpkResponseInvoice
                                       {
                                           BpkDetailId = a.BpkDetailId,
                                           InvoiceNumber = a.InvoiceNumber ?? "",
                                           InvoiceDate = b.InvoiceDate.ToString(_formatDate),
                                           CustomerName = c.Name,
                                           Amount = b.Amount
                                       };

                    var queryPotongan = from a in bpkDetails.Where(x => x.PotonganId != null)
                                        join b in _domainService.GetAllPotongan() on a.PotonganId equals b.PotonganId
                                        join customer in _domainService.GetAllCustomer() on b.CustomerCode.Trim() equals customer.CustomerCode.Trim() into bc
                                        from c in bc.DefaultIfEmpty()
                                        join d in _domainService.GetAllPotonganType() on b.PotonganTypeId equals d.PotonganTypeId
                                        join branch in _domainService.GetAllBranch() on b.BranchId equals branch.BranchId into be
                                        from e in be.DefaultIfEmpty()
                                        select new BpkResponsePotongan
                                        {
                                            BpkDetailId = a.BpkDetailId,
                                            PotonganId = a.PotonganId,
                                            PotonganTypeId = b.PotonganTypeId,
                                            PotonganTypeName = d.Name,
                                            BranchId = b.BranchId,
                                            BranchName = e == null ? "" : string.Format("{0} ({1})", e.Name, e.BusinessArea),
                                            PotonganNumber = b.PotonganNumber,
                                            nomorPoEps = b.nomorPoEps,
                                            PotonganDate = b.PotonganDate == null ? "" : b.PotonganDate.Value.ToString(_formatDate),
                                            CustomerCode = b.CustomerCode,
                                            CustomerName = c == null ? "" : b.CustomerCode + " - " + c.Name,
                                            Amount = b.Amount
                                        };

                    var queryHistory = from a in _domainService.GetAllBpkHistory().Where(x => x.BpkId == bpk.BpkId)
                                       join user in _domainService.GetAllAppUser() on a.ActionBy equals user.Username into ac
                                       from c in ac.DefaultIfEmpty()
                                       join d in _domainService.GetAllAppAction() on a.AppActionId equals d.AppActionId
                                       orderby a.CreatedAt descending
                                       select new BpkResponseHistory
                                       {
                                           StatusName = d.Name,
                                           ActionByName = c == null ? a.ActionBy : c.FullName,
                                           ActionAt = a.CreatedAt.ToString(_formatDate + " HH:mm")
                                       };

                    result.Status = _domainService.GetAllBpkStatus().Single(x => x.BpkStatusId == bpk.BpkStatusId).Name;
                    result.Invoices = queryInvoice.ToList();
                    result.Potongans = queryPotongan.OrderBy(x => x.PotonganTypeId).ToList();
                    result.Histories = queryHistory.ToList();

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: GetBpkById(), Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<List<BpkStatusResponse>> GetBpkStatus()
        {
            try
            {
                var resStatusBpk = await _domainService.GetAllBpkStatus().ToListAsync().ConfigureAwait(false);

                if (resStatusBpk == null || !resStatusBpk.Any()) 
                    return new List<BpkStatusResponse>();

                var result = resStatusBpk.Select(status => new BpkStatusResponse
                {
                    BpkStatusId = status.BpkStatusId,
                    Name = status.Name
                }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: GetStatusBpk(), Message: {ex.Message}");
                throw;
            }
        }

        public async Task<List<BpkMasterClearingStatusResponse>> GetMasterClearingStatus()
        {
            try
            {
                var resStatusBpk = await _domainService.GetAllClearingStatus().ToListAsync().ConfigureAwait(false);

                if (resStatusBpk == null || !resStatusBpk.Any()) 
                    return new List<BpkMasterClearingStatusResponse>();

                var result = resStatusBpk.Select(status => new BpkMasterClearingStatusResponse
                {
                    ClearingStatusId = status.ClearingStatusId,
                    Name = status.Name
                }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: GetStatusBpk(), Message: {ex.Message}");
                throw;
            }
        }


        public async Task<ResultBase> OpenClearingBpk(Guid incomingPaymentId)
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

                    var incomingPayment = _domainService.GetAllIncomingPayment().SingleOrDefault(x => x.IncomingPaymentId == incomingPaymentId);
                    if (incomingPayment == null || incomingPayment.BpkId == null)
                    {
                        result.Message = MessageConstants.S_BPK_NOT_FOUND;
                        return result;
                    }

                    if (incomingPayment.ClearingStatusId != (int)ClearingStatusEnum.ClearingOK)
                    {
                        result.Message = MessageConstants.S_INCOMING_PAYMENT_NOT_CLEAR;

                        return result;
                    }

                    var userLogin = _profileService.GetUserLogin();

                    var now = DateTime.Now;
                    var currentUser = userLogin.Username;
                    var currentApp = userLogin.App;

                    incomingPayment.ClearingStatusId = (int)ClearingStatusEnum.NotYet;
                    incomingPayment.ClearingNumber = null;
                    incomingPayment.ClearingDate = null;
                    incomingPayment.IsClearingManual = false;

                    incomingPayment.UpdatedApp = currentApp;
                    incomingPayment.UpdatedAt = now;
                    incomingPayment.UpdatedBy = currentUser;
                    incomingPayment.Revision += 1;

                    _domainService.UpdateIncomingPayment(incomingPayment);


                    var bpk = _domainService.GetAllBpk().Single(x => x.BpkId == incomingPayment.BpkId);
                    bpk.BpkStatusId = (int)BpkStatusEnum.BpkDraft;

                    bpk.UpdatedApp = currentApp;
                    bpk.UpdatedAt = now;
                    bpk.UpdatedBy = currentUser;
                    bpk.Revision += 1;

                    _domainService.UpdateBpk(bpk);

                    _historyService.AddBpkHistory(incomingPayment, bpk, now, (int)AppActionEnum.OpenClearing);

                    result.Success = true;
                    result.Message = MessageConstants.S_INCOMING_PAYMENT_OPEN_CLEARING;

                    _domainService.SaveChanges();

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: OpenClearingBpk(), incomingPaymentId: {incomingPaymentId}" +
                        $"Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<ResultBase> RequestForRevisionBpk(Guid incomingPaymentId)
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
                    if (incomingPayment == null || incomingPayment.BpkId == null)
                    {
                        result.Message = MessageConstants.S_BPK_NOT_FOUND;
                        return result;
                    }

                    var userLogin = _profileService.GetUserLogin();

                    var now = DateTime.Now;
                    var currentUser = userLogin.Username;
                    var currentApp = userLogin.App;

                    if (!userLogin.AreaIds.Contains(incomingPayment.AreaId))
                    {
                        result.Message = MessageConstants.S_NOT_AUTHORIZE;

                        return result;
                    }

                    var bpk = _domainService.GetAllBpk().Single(x => x.BpkId == incomingPayment.BpkId);
                    bpk.BpkStatusId = (int)BpkStatusEnum.BpkRevised;

                    bpk.UpdatedApp = currentApp;
                    bpk.UpdatedAt = now;
                    bpk.UpdatedBy = currentUser;
                    bpk.Revision += 1;

                    _domainService.UpdateBpk(bpk);

                    _historyService.AddBpkHistory(incomingPayment, bpk, now, (int)AppActionEnum.RequestForRevision);

                    filePath = BusinessHelper.GetSapFilePath(_appSettings.SharingFolder,
                        incomingPayment.InterfaceNumber, incomingPayment.PaymentDate);

                    if (!File.Exists(filePath))
                    {
                        result.Message = MessageConstants.S_BPK_REQUEST_FILE_NOT_EXIST;

                        return result;
                    }

                    result.Success = true;
                    result.Message = MessageConstants.S_BPK_REQUEST_FOR_REVISION;

                    _domainService.SaveChanges();

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: RequestForRevisionBpk(), incomingPaymentId: {incomingPaymentId}" +
                        $"Message: {ex.Message}");
                    throw;
                }
                finally
                {
                    if (result.Success && filePath != "")
                        File.Delete(filePath);
                }
            }).ConfigureAwait(false);
        }

        public async Task<ResultBase> ReviseApproveBpk(Guid incomingPaymentId)
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

                    var incomingPayment = _domainService.GetAllIncomingPayment().SingleOrDefault(x => x.IncomingPaymentId == incomingPaymentId);
                    if (incomingPayment == null || incomingPayment.BpkId == null)
                    {
                        result.Message = MessageConstants.S_BPK_NOT_FOUND;
                        return result;
                    }

                    var userLogin = _profileService.GetUserLogin();

                    var now = DateTime.Now;
                    var currentUser = userLogin.Username;
                    var currentApp = userLogin.App;

                    var bpk = _domainService.GetAllBpk().Single(x => x.BpkId == incomingPayment.BpkId);
                    if (bpk.BpkStatusId != (int)BpkStatusEnum.BpkRevised)
                    {
                        result.Message = MessageConstants.S_BPK_NOT_REQUEST_FOR_REVISION;

                        return result;
                    }

                    bpk.BpkStatusId = (int)BpkStatusEnum.BpkDraft;

                    bpk.UpdatedApp = currentApp;
                    bpk.UpdatedAt = now;
                    bpk.UpdatedBy = currentUser;
                    bpk.Revision += 1;

                    _domainService.UpdateBpk(bpk);

                    _historyService.AddBpkHistory(incomingPayment, bpk, now, (int)AppActionEnum.ReviseApprove);

                    result.Success = true;
                    result.Message = MessageConstants.S_BPK_REVISE_APPROVE;

                    _domainService.SaveChanges();

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: ReviseApproveBpk(), incomingPaymentId: {incomingPaymentId}" +
                        $"Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<ResultBase> ReviseRejectBpk(RejectBpkRequest param)
        {
            return await Task.Run(async () =>
            {
                var filepath = "";
                var result = new ResultBase
                {
                    Success = false,
                    Message = ""
                };

                try
                {
                    var incomingPaymentId = param.IncomingPaymentId;
                    var reason = param.Reason;

                    var incomingPayment = _domainService.GetAllIncomingPayment().SingleOrDefault(x => x.IncomingPaymentId == incomingPaymentId);
                    if (incomingPayment == null || incomingPayment.BpkId == null)
                    {
                        result.Message = MessageConstants.S_BPK_NOT_FOUND;

                        return result;
                    }

                    var bpk = _domainService.GetAllBpk().Single(x => x.BpkId == incomingPayment.BpkId);

                    var userLogin = _profileService.GetUserLogin();

                    var now = DateTime.Now;
                    var currentUser = userLogin.Username;
                    var currentApp = userLogin.App;

                    bpk.BpkStatusId = (int)BpkStatusEnum.BpkSubmitted;
                    bpk.RejectedAt = now;
                    bpk.RejectedBy = currentUser;
                    bpk.RejectedReason = reason;

                    bpk.UpdatedAt = now;
                    bpk.UpdatedApp = currentApp;
                    bpk.UpdatedBy = currentUser;
                    bpk.Revision += 1;

                    _domainService.UpdateBpk(bpk);

                    _historyService.AddBpkHistory(incomingPayment, bpk, now, (int)AppActionEnum.ReviseReject);

                    var fileGeneratedResult = await _incomingPaymentService.GenerateSapFile(incomingPayment.IncomingPaymentId);
                    if (!fileGeneratedResult.Success)
                    {
                        result.Message = fileGeneratedResult.Message;

                        return result;
                    }

                    filepath = fileGeneratedResult.Message;

                    result.Success = true;
                    result.Message = MessageConstants.S_BPK_REVISE_REJECT;

                    _domainService.SaveChanges();

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: ReviseRejectBpk(), param: {param}" +
                        $"Message: {ex.Message}");
                    throw;
                }
                finally
                {
                    if (!result.Success && !string.IsNullOrEmpty(filepath))
                    {
                        File.Delete(filepath);
                    }
                }
            }).ConfigureAwait(false);
        }

        public async Task<ResultBase> SaveBpk(SaveBpkRequest param)
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

                    var validStatusIds = new List<int>
                    {
                        (int) BpkStatusEnum.BpkNotCreated,
                        (int) BpkStatusEnum.BpkDraft,
                        (int) BpkStatusEnum.BpkRejected
                    };

                    var userLogin = _profileService.GetUserLogin();

                    var now = DateTime.Now;
                    var currentUser = userLogin.Username;
                    var currentApp = userLogin.App;

                    var incomingPaymentId = param.IncomingPaymentId;
                    var invoices = param.Invoices;
                    var potongans = param.Potongans;

                    var incomingPayment = _domainService.GetAllIncomingPayment().Single(x => x.IncomingPaymentId == incomingPaymentId);

                    Bpk bpk;
                    var actionId = (int)AppActionEnum.Created;
                    if (incomingPayment.BpkId == null)
                    {
                        var documentNumber = BusinessHelper.GetBpkNumber(now, incomingPayment.InterfaceNumber);
                        bpk = new Bpk
                        {
                            BpkId = Guid.NewGuid(),
                            BpkNumber = documentNumber,
                            BpkStatusId = (int)BpkStatusEnum.BpkDraft,

                            CreatedAt = now,
                            CreatedApp = currentApp,
                            CreatedBy = currentUser,
                            UpdatedAt = now,
                            UpdatedApp = currentApp,
                            UpdatedBy = currentUser,
                            Revision = ConfigConstants.N_INIT_REVISION
                        };

                        _domainService.InsertBpk(bpk);

                        // update incoming payment
                        incomingPayment.BpkId = bpk.BpkId;

                        incomingPayment.UpdatedApp = currentApp;
                        incomingPayment.UpdatedAt = now;
                        incomingPayment.UpdatedBy = currentUser;
                        incomingPayment.Revision += 1;

                        _domainService.UpdateIncomingPayment(incomingPayment);

                        actionId = (int)AppActionEnum.Created;
                    }
                    else
                    {
                        bpk = _domainService.GetAllBpk().Single(x => x.BpkId == incomingPayment.BpkId);

                        bpk.BpkStatusId = (int)BpkStatusEnum.BpkDraft;

                        bpk.UpdatedAt = now;
                        bpk.UpdatedApp = currentApp;
                        bpk.UpdatedBy = currentUser;
                        bpk.Revision += 1;

                        _domainService.UpdateBpk(bpk);

                        actionId = (int)AppActionEnum.SaveAsDraft;
                    }

                    _historyService.AddBpkHistory(incomingPayment, bpk, now, actionId);

                    var originDetails = _masterDataService.GetAllActiveBpkDetail().Where(x => x.BpkId == bpk.BpkId).ToList();

                    // add
                    var newInvoices = invoices.Where(x => x.BpkDetailId == null).ToList();
                    foreach (var item in newInvoices)
                    {
                        var found = originDetails.SingleOrDefault(x => x.InvoiceNumber == item.InvoiceNumber);
                        if (found == null)
                        {
                            var newitem = new BpkDetail
                            {
                                BpkDetailId = Guid.NewGuid(),
                                BpkId = bpk.BpkId,
                                InvoiceNumber = item.InvoiceNumber,
                                CreatedAt = now,
                                CreatedApp = currentApp,
                                CreatedBy = currentUser
                            };

                            _domainService.InsertBpkDetail(newitem);
                        }
                        else
                        {
                            result.Message = string.Format(MessageConstants.S_INVOICE_NOT_AVAILABLE, item.InvoiceNumber);

                            return result;
                        }
                    }

                    var singleTypes = new List<int>
                    {
                        (int)PotonganTypeEnum.BankCharge,
                        (int)PotonganTypeEnum.Pembulatan
                    };

                    // validate potongan
                    var needToValidatePotongans = potongans.Where(x => x.IsDelete == false).ToList();
                    foreach (var typeId in singleTypes)
                    {
                        var tempData = needToValidatePotongans.Where(x => x.PotonganTypeId == typeId).ToList();
                        if (tempData.Count > 1)
                        {
                            result.Message = MessageConstants.S_POTONGAN_ONLY_ONE;

                            return result;
                        }
                    }

                    // validate MAX length potonganNumber (return & promosi max length 17, default max length 50)
                    foreach (var item in needToValidatePotongans)
                    {
                        switch (item.PotonganTypeId)
                        {
                            case (int)PotonganTypeEnum.Return:
                            case (int)PotonganTypeEnum.Promosi:
                                if (item.PotonganNumber.Count() > 17)
                                {
                                    result.Message = MessageConstants.S_POTONGAN_NUMBER_MAX_RETURN_PROMOSI;

                                    return result;
                                };
                                break;
                            default:
                                if (item.PotonganNumber.Count() > 50)
                                {
                                    result.Message = MessageConstants.S_POTONGAN_NUMBER_MAX_DEFAULT;

                                    return result;
                                }
                                break;
                        }
                    }

                    var limitPembulatan = _domainService.GetAllAppConfig()
                        .Single(x => x.AppConfigId == (int)AppConfigEnum.MAX_PEMBULATAN)
                        .IntValue;

                    if (limitPembulatan != null)
                    {
                        foreach (var potongan in potongans
                            .Where(x => !x.IsDelete && x.PotonganTypeId == (int)PotonganTypeEnum.Pembulatan))
                        {
                            if (potongan.Amount > limitPembulatan || potongan.Amount < limitPembulatan)
                            {
                                result.Message = string.Format(MessageConstants.S_PEMBULATAN_MAX_INVALID, limitPembulatan);

                                return result;
                            }
                        }
                    }

                    var newPotongans = potongans.Where(x => x.BpkDetailId == null).ToList();
                    var originPotonganIds = originDetails.Where(x => x.PotonganId != null).Select(x => x.PotonganId).ToList();
                    var originPotongans = _domainService.GetAllPotongan().Where(x => originPotonganIds.Contains(x.PotonganId)).ToList();

                    foreach (var item in newPotongans)
                    {
                        var found = originPotongans.SingleOrDefault(x =>
                            x.PotonganNumber == item.PotonganNumber
                            && x.PotonganTypeId == item.PotonganTypeId);
                        if (found == null)
                        {
                            // create new potongan
                            var newPotongan = new Potongan
                            {
                                PotonganId = Guid.NewGuid(),
                                PotonganNumber = item.PotonganNumber,
                                nomorPoEps = item.nomorPoEps,
                                PotonganTypeId = item.PotonganTypeId,
                                PotonganDate = item.PotonganDate.ToDbDatetime(_dbFormatDate),
                                CustomerCode = item.CustomerCode,
                                Amount = item.Amount,
                                BranchId = item.BranchId,

                                CreatedAt = now,
                                CreatedApp = currentApp,
                                CreatedBy = currentUser,
                                UpdatedAt = now,
                                UpdatedApp = currentApp,
                                UpdatedBy = currentUser,
                                Revision = ConfigConstants.N_INIT_REVISION
                            };
                            _domainService.InsertPotongan(newPotongan);

                            // new BPK Detail
                            var newDetail = new BpkDetail
                            {
                                BpkDetailId = Guid.NewGuid(),
                                BpkId = bpk.BpkId,
                                PotonganId = newPotongan.PotonganId,

                                CreatedAt = now,
                                CreatedApp = currentApp,
                                CreatedBy = currentUser,
                            };
                            _domainService.InsertBpkDetail(newDetail);
                        }
                        else
                        {
                            result.Message = string.Format(MessageConstants.S_POTONGAN_NOT_AVAILABLE, item.PotonganNumber);

                            return result;
                        }
                    }

                    // edit
                    var updatedPotongans = potongans.Where(x => x.IsUpdate).ToList();
                    foreach (var item in updatedPotongans)
                    {
                        var found = originPotongans.SingleOrDefault(x => x.PotonganId == item.PotonganId);
                        if (found != null)
                        {
                            found.PotonganNumber = item.PotonganNumber;
                            found.nomorPoEps = item.nomorPoEps;
                            found.PotonganTypeId = item.PotonganTypeId;
                            found.PotonganDate = item.PotonganDate.ToDbDatetime(_dbFormatDate);
                            found.CustomerCode = item.CustomerCode;
                            found.Amount = item.Amount;
                            found.BranchId = item.BranchId;

                            found.UpdatedAt = now;
                            found.UpdatedApp = currentApp;
                            found.UpdatedBy = currentUser;
                            found.Revision += 1;

                            _domainService.UpdatePotongan(found);
                        }
                    }

                    // delete
                    var deleteInvoices = invoices.Where(x => x.IsDelete).ToList();
                    foreach (var item in deleteInvoices)
                    {
                        var found = originDetails.SingleOrDefault(x => x.BpkDetailId == item.BpkDetailId);
                        if (found != null)
                        {
                            _domainService.DeleteBpkDetail(found);
                        }
                    }

                    var deletePotongans = potongans.Where(x => x.IsDelete).ToList();
                    foreach (var item in deletePotongans)
                    {
                        var found = originPotongans.SingleOrDefault(x => x.PotonganId == item.PotonganId);
                        if (found != null)
                        {
                            var detail = originDetails.Single(x => x.PotonganId == found.PotonganId);
                            _domainService.DeleteBpkDetail(detail);

                            _domainService.DeletePotongan(found);
                        }
                    }

                    result.Success = true;
                    result.Message = MessageConstants.S_BPK_SAVED;

                    _domainService.SaveChanges();

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: SaveBpk(), param: {param}" +
                        $"Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<ResultBase> SendReminder(string executeBy, string executeApp)
        {
            return await Task.Run(async () =>
            {
                try
                {
                    var result = new ResultBase
                    {
                        Success = false,
                        Message = ""
                    };

                    var now = DateTime.Now;
                    var currentUser = executeBy;
                    var currentApp = executeApp;

                    var daysToReminder = _domainService.GetAllAppConfig()
                        .Single(x => x.AppConfigId == (int)AppConfigEnum.DAYS_TO_BPK_REMINDER)
                        .IntValue ?? 0;

                    var daysToRemindAgain = _domainService.GetAllAppConfig()
                        .Single(x => x.AppConfigId == (int)AppConfigEnum.DAYS_RE_SEND_EMAIL)
                        .IntValue ?? 0; ;

                    var incomingPayments = _domainService.GetAllIncomingPayment()
                        .Where(x => x.SegmentId == (int)SegmentEnum.Spm
                            && x.BpkId == null
                            && x.CreatedAt.AddDays(daysToReminder) < now
                            && (x.BpkEmailRemindAt == null || x.BpkEmailRemindAt.Value.AddDays(daysToRemindAgain) < now)
                            )
                        .ToList();

                    var incomingPaymentRef =
                        (from a in incomingPayments
                         let b = _domainService.GetAllCustomer().Where(p2 => a.CustomerCode == p2.CustomerCode).FirstOrDefault()
                         join c in _domainService.GetAllArea() on a.AreaId equals c.AreaId
                         join d in _domainService.GetAllBranch() on c.BranchId equals d.BranchId
                         select new
                         {
                             a.IncomingPaymentId,
                             a.CustomerCode,
                             CustomerName = b.Name,
                             AreaName = c.Name,
                             BranchName = d.Name
                         }).ToList();

                    var areaIds = incomingPayments.Select(x => x.AreaId).Distinct().ToList();

                    foreach (var areaId in areaIds)
                    {
                        var mailRequest = new MailRequest
                        {
                            ToEmail = new List<string>(),
                            Cc = new List<string>(),
                            Subject = EmailConstants.S_BPK_NOT_CREATED_REMINDER_SUBJECT
                        };

                        var emails = _userService.GetPicAreaEmails(areaId);
                        foreach (var item in emails)
                        {
                            mailRequest.ToEmail.Add(item);
                        }

                        if (!mailRequest.ToEmail.Any())
                        {
                            // no admin area, skip
                            continue;
                        }

                        var currentAreaIncomingPayments = incomingPayments.Where(x => x.AreaId == areaId)
                            .OrderBy(x => x.PaymentDate)
                            .ToList();

                        var content = "";
                        var count = 1;
                        foreach (var item in currentAreaIncomingPayments)
                        {
                            var itemRef = incomingPaymentRef.Single(x => x.IncomingPaymentId == item.IncomingPaymentId);
                            content += string.Format(EmailConstants.S_BPK_NOT_CREATED_REMINDER_CONTENT,
                                count++,
                                item.PaymentDate.ToString("dd-MM-yyyy"),
                                item.Amount,
                                itemRef.CustomerName,
                                itemRef.AreaName,
                                itemRef.BranchName,
                                item.InterfaceNumber,
                                item.ClearingNumber);
                        }

                        mailRequest.Body = string.Format(EmailConstants.S_BPK_NOT_CREATED_REMINDER_BODY,
                            now.ToString("dd MMM yyyy HH:mm:ss"),
                            now.ToString("dd MMM yyyy"),
                            content);

                        await _mailService.SendEmailAsync(mailRequest);
                        Log.Logger.Information($"Method: SendReminder(), mailRequest: {mailRequest}");

                        foreach (var picIncomingPayment in currentAreaIncomingPayments)
                        {
                            picIncomingPayment.BpkEmailRemindAt = now;

                            picIncomingPayment.UpdatedApp = currentApp;
                            picIncomingPayment.UpdatedAt = now;
                            picIncomingPayment.UpdatedBy = currentUser;
                            picIncomingPayment.Revision += 1;

                            _domainService.UpdateIncomingPayment(picIncomingPayment);
                        }
                    }

                    result.Success = true;

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: SendReminder()" +
                        $"Message: {ex.Message}");

                    throw;
                }
                finally
                {
                    _domainService.SaveChanges();
                }
            }).ConfigureAwait(false);
        }

        public async Task<ResultBase> SubmitBpk(Guid incomingPaymentId)
        {
            return await Task.Run(async () =>
            {
                var filepath = "";
                var result = new ResultBase
                {
                    Success = false,
                    Message = ""
                };

                try
                {
                    var incomingPayment = _domainService.GetAllIncomingPayment()
                        .SingleOrDefault(x => x.IncomingPaymentId == incomingPaymentId);
                    if (incomingPayment == null || incomingPayment.BpkId == null)
                    {
                        result.Message = BpkConstants.S_BPK_NOT_FOUND;
                        return result;
                    }

                    var bpk = _domainService.GetAllBpk().Single(x => x.BpkId == incomingPayment.BpkId);

                    var originDetails = _masterDataService.GetAllActiveBpkDetail().Where(x => x.BpkId == bpk.BpkId).ToList();
                    var invoiceNumbers = originDetails.Where(x => x.InvoiceNumber != null)
                        .Select(x => x.InvoiceNumber)
                        .ToList();

                    var invoiceAmount = _domainService.GetAllInvoice()
                        .Where(x => invoiceNumbers.Contains(x.InvoiceNumber))
                        .Sum(x => x.Amount);

                    var potonganIds = originDetails.Where(x => x.PotonganId != null)
                        .Select(x => x.PotonganId)
                        .ToList();

                    var potonganAmount = _domainService.GetAllPotongan()
                        .Where(x => potonganIds.Contains(x.PotonganId))
                        .Sum(x => x.Amount);

                    var transactionDiff = invoiceAmount - potonganAmount;
                    var difference = incomingPayment.Amount - transactionDiff;
                    if (difference != 0)
                    {
                        result.Message = MessageConstants.S_BPK_DIFFERENCE_NOT_ZERO;
                        return result;
                    }

                    var userLogin = _profileService.GetUserLogin();

                    var now = DateTime.Now;
                    var currentUser = userLogin.Username;
                    var currentApp = userLogin.App;

                    bpk.BpkStatusId = (int)BpkStatusEnum.BpkSubmitted;

                    bpk.UpdatedAt = now;
                    bpk.UpdatedApp = currentApp;
                    bpk.UpdatedBy = currentUser;
                    _domainService.UpdateBpk(bpk);

                    _historyService.AddBpkHistory(incomingPayment, bpk, now, (int)AppActionEnum.Submitted);

                    var fileGeneratedResult = await _incomingPaymentService.GenerateSapFile(incomingPayment.IncomingPaymentId);
                    if (!fileGeneratedResult.Success)
                    {
                        result.Message = fileGeneratedResult.Message;

                        return result;
                    }

                    filepath = fileGeneratedResult.Message;

                    _domainService.SaveChanges();

                    result.Success = true;
                    result.Message = MessageConstants.S_BPK_SUBMITTED;

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: SubmitBpk(), incomingPaymentId: {incomingPaymentId}" +
                        $"Message: {ex.Message}");

                    throw;
                }
                finally
                {
                    if (!result.Success && !string.IsNullOrEmpty(filepath))
                    {
                        File.Delete(filepath);
                    }
                }
            }).ConfigureAwait(false);
        }
    }
}
