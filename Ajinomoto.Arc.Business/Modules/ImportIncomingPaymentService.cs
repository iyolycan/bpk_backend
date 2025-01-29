using Ajinomoto.Arc.Business.Helper;
using Ajinomoto.Arc.Common.AppModels;
using Ajinomoto.Arc.Common.Constants;
using Ajinomoto.Arc.Common.Enums;
using Ajinomoto.Arc.Data.Models;
using ClosedXML.Excel;
using Serilog;
using System.Data;
using System.Globalization;

namespace Ajinomoto.Arc.Business.Modules
{
    public partial class IncomingPaymentService
    {
        private ResultBase ImportIncomingPaymentTemplate01(int segmentId, int sourceId, int picId, string filePath)
        {
            var result = new ResultBase
            {
                Success = false,
                Message = ""
            };
            List<string> sapFileNeedToDeleteList = new List<string>();

            try
            {
                var userLogin = _profileService.GetUserLogin();

                var now = DateTime.Now;
                var currentUser = userLogin.Username;
                var currentApp = userLogin.App;

                XLWorkbook wb = new XLWorkbook(filePath);
                IXLWorksheet ws = wb.Worksheet(2);

                var columns = "B,E,F,G,H,K,L,M,P,Q".Split(",").ToArray();
                var headers = "Virtual Account,Group / Area,Amount,Tanggal,Posting Date,Berita,Customer Code,VA,Interface,Clearing"
                .Split(",").ToArray();
                var idx = 0;
                foreach (var column in columns)
                {
                    var value = ws.Cell(column + 6).Value.ToString();
                    if (!string.Equals(value?.Trim(), headers[idx].Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        result.Message = MessageConstants.S_INCORRECT_TEMPLATE;
                        return result;
                    }

                    idx++;
                }

                var keyColumn = "P";
                var initialRow = 7;
                var lastRow = ws.Column(keyColumn).LastCellUsed().Address.RowNumber;

                #region "Check duplicates key"
                var columnDocInterface = ws.Range(keyColumn + initialRow, keyColumn + lastRow).CellsUsed().Select(x => x.Value.ToString()).ToList();
                var columnYearPaymentDate = ws.Range("G" + initialRow, "G" + lastRow).CellsUsed().Select(x => DateTime.ParseExact(x.GetFormattedString(), "dd.MM.yyyy", null).Year).ToList();
                var duplicates = new List<string>();
                for (int i = 0; i < columnDocInterface.Count; i++)
                {
                    duplicates.Add(columnDocInterface[i] + columnYearPaymentDate[i]);
                }

                if (duplicates.Count != duplicates.Distinct().Count())
                {
                    result.Message = "Duplicate key in Excel";
                    return result;
                }

                #endregion

                for (int i = initialRow; i <= lastRow; i++)
                {
                    var keyValue = ws.Cell(keyColumn + i).Value.ToString();
                    if (string.IsNullOrEmpty(keyValue))
                    {
                        var keyHeader = ws.Cell(keyColumn + (initialRow - 1)).Value.ToString();
                        result.Message = keyHeader + " not found in Excel Row: " + i;
                        return result;
                    }

                    DateTime PaymentDate = DateTime.Now;
                    if (string.IsNullOrEmpty(ws.Cell("G" + i).Value.ToString())
                        || !DateTime.TryParseExact(ws.Cell("G" + i).GetFormattedString(), "dd.MM.yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out PaymentDate))
                    {
                        result.Message = "Payment Date not found or incorrect in Excel Cell: G:" + i;
                        return result;
                    }

                    DateTime PostingDate = DateTime.Now;
                    if (string.IsNullOrEmpty(ws.Cell("H" + i).Value.ToString())
                        || !DateTime.TryParseExact(ws.Cell("H" + i).GetFormattedString(), "dd.MM.yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out PostingDate))
                    {
                        result.Message = "Posting Date not found or incorrect in Excel Cell: H:" + i;
                        return result;
                    }

                    double amountIdr = 0.00;
                    if (!string.IsNullOrEmpty(Convert.ToString(ws.Cell("F" + i).Value)))
                    {
                        amountIdr = Convert.ToDouble(ws.Cell("F" + i).Value);
                    }

                    var customers = _domainService.GetAllCustomer().SingleOrDefault(x => x.CustomerCode == Convert.ToString(ws.Cell("L" + i).Value));
                    if (customers == null)
                    {
                        result.Message = "Customer not found in Excel Cell: L:" + i;

                        return result;
                    }

                    var area = _domainService.GetAllArea().SingleOrDefault(x => x.Name == Convert.ToString(ws.Cell("E" + i).Value));
                    if (area == null)
                    {
                        result.Message = "Area not found in Excel Cell: E:" + i;
                        return result;
                    }

                    var incomingPayment = _domainService.GetAllIncomingPayment().Where(x =>
                        x.InterfaceNumber == keyValue.Trim()
                        && x.PaymentDate.Year == PaymentDate.Year)
                        .FirstOrDefault();

                    var excelClearingNumber = ws.Cell("Q" + i).Value.ToString();
                    if (incomingPayment != null)
                    {
                        if (!string.IsNullOrEmpty(excelClearingNumber)
                            && !string.IsNullOrEmpty(incomingPayment.ClearingNumber)
                            && incomingPayment.ClearingNumber != excelClearingNumber)
                        {
                            result.Message = string.Format(MessageConstants.S_CLEARING_NUMBER_DIFF,
                                "Q:" + i,
                                incomingPayment.InterfaceNumber,
                                incomingPayment.PaymentDate.Year);

                            return result;
                        }

                        incomingPayment.PicId = picId;

                        if (string.IsNullOrEmpty(incomingPayment.ClearingNumber))
                        {
                            incomingPayment.SourceId = sourceId;

                            incomingPayment.VirtualAccount = ws.Cell("B" + i).Value.ToString();
                            incomingPayment.CustomerCode = ws.Cell("L" + i).Value.ToString();
                            incomingPayment.AreaId = area.AreaId;
                            incomingPayment.Amount = (double)amountIdr;
                            incomingPayment.PaymentDate = DateOnly.FromDateTime(PaymentDate);
                            incomingPayment.PostingDate = DateOnly.FromDateTime(PostingDate);
                            incomingPayment.Remark = ws.Cell("K" + i).Value.ToString();
                            incomingPayment.VirtualAccount = ws.Cell("M" + i).Value.ToString();

                            if (!string.IsNullOrEmpty(excelClearingNumber))
                            {
                                incomingPayment.ClearingNumber = excelClearingNumber;
                                incomingPayment.ClearingDate = DateOnly.FromDateTime(now);
                                incomingPayment.ClearingStatusId = (int)ClearingStatusEnum.ClearingOK;
                                incomingPayment.IsClearingManual = true;

                                if (incomingPayment.BpkId != null)
                                {
                                    var existingBpk = _domainService.GetAllBpk().Single(x => x.BpkId == incomingPayment.BpkId);

                                    var sapFilePath = BusinessHelper.GetSapFilePath(_appSettings.SharingFolder,
                                        incomingPayment.InterfaceNumber, incomingPayment.PaymentDate);

                                    if (existingBpk.BpkStatusId == (int)BpkStatusEnum.BpkSubmitted && !File.Exists(sapFilePath))
                                    {
                                        result.Message = string.Format(MessageConstants.S_CLEARING_MANUAL_SUBMITTED_FILE_NOT_EXIST,
                                            "Q:" + i);

                                        return result;
                                    }

                                    sapFileNeedToDeleteList.Add(sapFilePath);

                                    if (existingBpk.BpkStatusId != (int)BpkStatusEnum.BpkSubmitted)
                                    {
                                        existingBpk.BpkStatusId = (int)BpkStatusEnum.BpkSubmitted;

                                        existingBpk.UpdatedAt = now;
                                        existingBpk.UpdatedApp = currentApp;
                                        existingBpk.UpdatedBy = currentUser;
                                        existingBpk.Revision += 1;

                                        _domainService.UpdateBpk(existingBpk);
                                    }

                                    _historyService.AddBpkHistory(incomingPayment, existingBpk, now, (int)AppActionEnum.Clearing);
                                }
                            }

                        }

                        incomingPayment.UpdatedAt = now;
                        incomingPayment.UpdatedApp = currentApp;
                        incomingPayment.UpdatedBy = currentUser;
                        incomingPayment.Revision += 1;

                        _domainService.UpdateIncomingPayment(incomingPayment);
                    }
                    else
                    {
                        incomingPayment = new IncomingPayment
                        {
                            IncomingPaymentId = Guid.NewGuid(),
                            SegmentId = segmentId,
                            SourceId = sourceId,
                            PicId = picId,

                            CreatedAt = now,
                            CreatedApp = currentApp,
                            CreatedBy = currentUser,
                            Revision = ConfigConstants.N_INIT_REVISION
                        };

                        incomingPayment.VirtualAccount = ws.Cell("B" + i).Value.ToString();
                        incomingPayment.CustomerCode = ws.Cell("L" + i).Value.ToString();
                        incomingPayment.AreaId = area.AreaId;
                        incomingPayment.Amount = (double)amountIdr;
                        incomingPayment.PaymentDate = DateOnly.FromDateTime(PaymentDate);
                        incomingPayment.PostingDate = DateOnly.FromDateTime(PostingDate);
                        incomingPayment.Remark = ws.Cell("K" + i).Value.ToString();
                        incomingPayment.VirtualAccount = ws.Cell("M" + i).Value.ToString();
                        incomingPayment.InterfaceNumber = ws.Cell("P" + i).Value.ToString();
                        incomingPayment.BillingNumber = ws.Cell("R" + i).Value.ToString();

                        if (!string.IsNullOrEmpty(excelClearingNumber))
                        {
                            incomingPayment.ClearingNumber = excelClearingNumber;
                            incomingPayment.ClearingDate = DateOnly.FromDateTime(now);
                            incomingPayment.ClearingStatusId = (int)ClearingStatusEnum.ClearingOK;
                            incomingPayment.IsClearingManual = true;

                            var documentNumber = BusinessHelper.GetBpkNumber(now, incomingPayment.InterfaceNumber);
                            var bpk = new Bpk
                            {
                                BpkId = Guid.NewGuid(),
                                BpkNumber = documentNumber,
                                BpkStatusId = (int)BpkStatusEnum.BpkSubmitted,

                                CreatedAt = now,
                                CreatedApp = currentApp,
                                CreatedBy = currentUser,
                                Revision = ConfigConstants.N_INIT_REVISION
                            };

                            _domainService.InsertBpk(bpk);
                            _historyService.AddBpkHistory(incomingPayment, bpk, now, (int)AppActionEnum.Created);
                            _historyService.AddBpkHistory(incomingPayment, bpk, now, (int)AppActionEnum.Clearing);

                            incomingPayment.BpkId = bpk.BpkId;
                        }

                        _domainService.InsertIncomingPayment(incomingPayment);
                    }
                }

                result.Success = true;
                result.Message = MessageConstants.S_INCOMINGPAYMENT_UPLOAD;

                _domainService.SaveChanges();

                return result;
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: ImportIncomingPaymentTemplate01, filePath: {filePath}" +
                    $"Message: {ex.Message}");
                throw;
            }
            finally
            {
                if (result.Success)
                {
                    foreach(var sapFilePath in sapFileNeedToDeleteList)
                    {
                        File.Delete(sapFilePath);
                    }
                }

                File.Delete(filePath);
            }
        }

        private ResultBase ImportIncomingPaymentTemplate02(int segmentId, int sourceId, int picId, string filePath)
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

                XLWorkbook wb = new XLWorkbook(filePath);
                IXLWorksheet ws = wb.Worksheet(1);

                var columns = "B,C,D,H,I,J,K,N,P".Split(",").ToArray();
                var headers = "Tanggal,Keterangan,Jumlah,Nama Customer,Area,Cabang,ID,Doc Interface,Doc Clearing"
                .Split(",").ToArray();
                var idx = 0;
                foreach (var column in columns)
                {
                    var value = ws.Cell(column + 6).Value.ToString();
                    if (!string.Equals(value?.Trim(), headers[idx].Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        result.Message = MessageConstants.S_INCORRECT_TEMPLATE;
                        return result;
                    }

                    idx++;
                }

                var keyColumn = "N";
                var initialRow = 7;
                var lastRow = ws.Column(keyColumn).LastCellUsed().Address.RowNumber;

                #region "Check duplicates key"
                var columnDocInterface = ws.Range(keyColumn + initialRow, keyColumn + lastRow).CellsUsed().Select(x => x.Value.ToString()).ToList();
                var columnYearPaymentDate = ws.Range("B" + initialRow, "B" + lastRow).CellsUsed().Select(x => Convert.ToDateTime(x.CachedValue).Year).ToList();
                var duplicates = new List<string>();
                for (int i = 0; i < columnDocInterface.Count; i++)
                {
                    duplicates.Add(columnDocInterface[i] + columnYearPaymentDate[i]);
                }

                if (duplicates.Count != duplicates.Distinct().Count())
                {
                    result.Message = "duplicate key in excel";
                    return result;
                }
                #endregion

                for (int i = initialRow; i <= lastRow; i++)
                {
                    var keyValue = ws.Cell(keyColumn + i).Value.ToString();
                    if (string.IsNullOrEmpty(keyValue))
                    {
                        var keyHeader = ws.Cell(keyColumn + (initialRow - 1)).Value.ToString();
                        result.Message = keyHeader + " not found in excel row:" + i;
                        return result;
                    }

                    if (string.IsNullOrEmpty(ws.Cell("B" + i).Value.ToString()))
                    {
                        result.Message = "Payment Date not found in excel row: B" + i;
                        return result;
                    }
                    DateTime paymentDate = DateTime.ParseExact(ws.Cell("B" + i).GetFormattedString(), "dd.MM.yyyy", null);

                    double amountIdr = 0.00;
                    if (!string.IsNullOrEmpty(Convert.ToString(ws.Cell("D" + i).Value)))
                    {
                        amountIdr = Convert.ToDouble(ws.Cell("D" + i).Value);
                    }

                    var incomingPayment = _domainService.GetAllIncomingPaymentNonSpm().Where(x =>
                        x.InterfaceNumber == keyValue.Trim()
                        && x.PaymentDate.Year == paymentDate.Year
                        && x.SegmentId == segmentId)
                        .FirstOrDefault();

                    if (incomingPayment != null)
                    {
                        incomingPayment.SegmentId = segmentId;
                        incomingPayment.SourceId = sourceId;
                        incomingPayment.PicId = picId;

                        incomingPayment.UpdatedAt = now;
                        incomingPayment.UpdatedApp = currentApp;
                        incomingPayment.UpdatedBy = currentUser;
                        incomingPayment.Revision++;

                        incomingPayment.PaymentDate = DateOnly.FromDateTime(paymentDate);
                        incomingPayment.Remark = ws.Cell("C" + i).Value.ToString();
                        incomingPayment.AmountIdr = (double)amountIdr;
                        incomingPayment.CustomerName = ws.Cell("H" + i).Value.ToString();
                        incomingPayment.AreaCode = ws.Cell("I" + i).Value.ToString();
                        incomingPayment.BranchCode = ws.Cell("J" + i).Value.ToString();
                        incomingPayment.CustomerCode = ws.Cell("K" + i).Value.ToString();
                        incomingPayment.InterfaceNumber = ws.Cell("N" + i).Value.ToString();

                        if (incomingPayment.ClearingNumber != ws.Cell("P" + i).Value.ToString())
                        {
                            incomingPayment.ClearingNumber = ws.Cell("P" + i).Value.ToString();
                            incomingPayment.ClearingDate = DateOnly.FromDateTime(now);
                        }

                        _domainService.UpdateIncomingPaymentNonSpm(incomingPayment);
                    }
                    else
                    {
                        var newItem = new IncomingPaymentNonSpm
                        {
                            SegmentId = segmentId,
                            SourceId = sourceId,
                            PicId = picId,

                            CreatedAt = now,
                            CreatedApp = currentApp,
                            CreatedBy = currentUser,
                            Revision = ConfigConstants.N_INIT_REVISION
                        };

                        newItem.PaymentDate = DateOnly.FromDateTime(paymentDate);
                        newItem.Remark = ws.Cell("C" + i).Value.ToString();
                        newItem.AmountIdr = (double)amountIdr;
                        newItem.CustomerName = ws.Cell("H" + i).Value.ToString();
                        newItem.AreaCode = ws.Cell("I" + i).Value.ToString();
                        newItem.BranchCode = ws.Cell("J" + i).Value.ToString();
                        newItem.CustomerCode = ws.Cell("K" + i).Value.ToString();
                        newItem.InterfaceNumber = ws.Cell("N" + i).Value.ToString();

                        if (!string.IsNullOrEmpty(ws.Cell("P" + i).Value.ToString()))
                        {
                            newItem.ClearingNumber = ws.Cell("P" + i).Value.ToString();
                            newItem.ClearingDate = DateOnly.FromDateTime(now);
                        }

                        _domainService.InsertIncomingPaymentNonSpm(newItem);
                    }
                }

                result.Success = true;
                result.Message = MessageConstants.S_INCOMINGPAYMENT_UPLOAD;

                _domainService.SaveChanges();

                return result;
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: ImportIncomingPaymentTemplate02, filePath: {filePath}" +
                    $"Message: {ex.Message}");
                throw;
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        private ResultBase ImportIncomingPaymentTemplate03(int segmentId, int sourceId, int picId, string filePath)
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

                XLWorkbook wb = new XLWorkbook(filePath);
                IXLWorksheet ws = wb.Worksheet(1);

                var columns = "B,D,E,G,H,I,J,K,L".Split(",").ToArray();
                var headers = "No BII,Amount,Tanggal,Nama Cust.,Cust. Code,Cab,Interface,Clearing,BILLING"
                .Split(",").ToArray();
                var idx = 0;
                foreach (var column in columns)
                {
                    var value = ws.Cell(column + 4).Value.ToString();
                    if (!string.Equals(value?.Trim(), headers[idx].Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        result.Message = MessageConstants.S_INCORRECT_TEMPLATE;
                        return result;
                    }

                    idx++;
                }

                var keyColumn = "L";
                var initialRow = 5;
                var lastRow = ws.Column(keyColumn).LastCellUsed().Address.RowNumber;

                #region "Check duplicates key"
                var duplicates = ws.Range(keyColumn + initialRow, keyColumn + lastRow).CellsUsed().Select(x => x.Value.ToString()).ToList();
                if (duplicates.Count != duplicates.Distinct().Count())
                {
                    result.Message = "duplicate key in excel";
                    return result;
                }
                #endregion

                for (int i = initialRow; i <= lastRow; i++)
                {
                    var keyValue = Convert.ToString(ws.Cell(keyColumn + i).Value);
                    if (string.IsNullOrEmpty(keyValue))
                    {
                        var keyHeader = (string)ws.Cell(keyColumn + (initialRow - 1)).Value;
                        result.Message = keyHeader + "not found in excel row:" + i;

                        return result;
                    }

                    if (string.IsNullOrEmpty(ws.Cell("E" + i).Value.ToString()))
                    {
                        result.Message = "Payment Date not found in excel row: " + i;

                        return result;
                    }
                    DateTime PaymentDate = (DateTime)ws.Cell("E" + i).Value;

                    double amountIdr = 0.00;
                    if (!string.IsNullOrEmpty(Convert.ToString(ws.Cell("D" + i).Value)))
                    {
                        amountIdr = Convert.ToDouble(ws.Cell("D" + i).Value);
                    }

                    var incomingPayment = _domainService.GetAllIncomingPaymentNonSpm().FirstOrDefault(x =>
                        x.BillingNumber == keyValue.Trim()
                        && x.SegmentId == segmentId);

                    if (incomingPayment != null)
                    {
                        incomingPayment.UpdatedAt = now;
                        incomingPayment.UpdatedApp = currentApp;
                        incomingPayment.UpdatedBy = currentUser;
                        incomingPayment.Revision++;

                        incomingPayment.BiiNumber = ws.Cell("B" + i).Value.ToString();
                        incomingPayment.AmountIdr = (double)amountIdr;
                        incomingPayment.PaymentDate = DateOnly.FromDateTime(PaymentDate);
                        incomingPayment.CustomerName = ws.Cell("G" + i).CachedValue.ToString();
                        incomingPayment.CustomerCode = ws.Cell("H" + i).CachedValue.ToString();
                        incomingPayment.BranchCode = ws.Cell("I" + i).CachedValue.ToString();
                        incomingPayment.InterfaceNumber = ws.Cell("J" + i).Value.ToString();

                        if (incomingPayment.ClearingNumber != ws.Cell("K" + i).Value.ToString())
                        {
                            incomingPayment.ClearingNumber = ws.Cell("K" + i).Value.ToString();
                            incomingPayment.ClearingDate = DateOnly.FromDateTime(now);
                        }

                        _domainService.UpdateIncomingPaymentNonSpm(incomingPayment);
                    }
                    else
                    {
                        var newItem = new IncomingPaymentNonSpm
                        {
                            SegmentId = segmentId,
                            SourceId = sourceId,
                            PicId = picId,

                            CreatedAt = now,
                            CreatedApp = currentApp,
                            CreatedBy = currentUser,
                            Revision = ConfigConstants.N_INIT_REVISION
                        };

                        newItem.BiiNumber = ws.Cell("B" + i).Value.ToString();
                        newItem.AmountIdr = (double)amountIdr;
                        newItem.PaymentDate = DateOnly.FromDateTime(PaymentDate);
                        newItem.CustomerName = ws.Cell("G" + i).CachedValue.ToString();
                        newItem.CustomerCode = ws.Cell("H" + i).CachedValue.ToString();
                        newItem.BranchCode = ws.Cell("I" + i).CachedValue.ToString();
                        newItem.InterfaceNumber = ws.Cell("J" + i).Value.ToString();
                        newItem.BillingNumber = ws.Cell("L" + i).Value.ToString();

                        if (!string.IsNullOrEmpty(ws.Cell("K" + i).Value.ToString()))
                        {
                            newItem.ClearingNumber = ws.Cell("K" + i).Value.ToString();
                            newItem.ClearingDate = DateOnly.FromDateTime(now);
                        }

                        _domainService.InsertIncomingPaymentNonSpm(newItem);
                    }
                }

                result.Success = true;
                result.Message = MessageConstants.S_INCOMINGPAYMENT_UPLOAD;

                _domainService.SaveChanges();

                return result;
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: ImportIncomingPaymentTemplate03, filePath: {filePath}" +
                    $"Message: {ex.Message}");
                throw;
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        private ResultBase ImportIncomingPaymentTemplate04(int segmentId, int sourceId, int picId, string filePath)
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

                XLWorkbook wb = new XLWorkbook(filePath);
                IXLWorksheet ws = wb.Worksheet(1);

                var columns = "B,C,D,E,I,J,K,L,M,N".Split(",").ToArray();
                var headers = "No VA,Nama VA,Amount,Tanggal,Nama Cust,Cust Code,Cab,Interface,Clearing,No billing"
                .Split(",").ToArray();
                var idx = 0;
                foreach (var column in columns)
                {
                    var value = ws.Cell(column + 4).Value.ToString();
                    if (!string.Equals(value?.Trim(), headers[idx].Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        result.Message = MessageConstants.S_INCORRECT_TEMPLATE;
                        return result;
                    }

                    idx++;
                }

                var keyColumn = "N";
                var initialRow = 5;
                var lastRow = ws.Column(keyColumn).LastCellUsed().Address.RowNumber;

                #region "Check duplicates key"
                var duplicates = ws.Range(keyColumn + initialRow, keyColumn + lastRow).CellsUsed().Select(x => x.Value.ToString()).ToList();
                if (duplicates.Count != duplicates.Distinct().Count())
                {
                    result.Message = "duplicate key in excel";
                    return result;
                }
                #endregion

                for (int i = initialRow; i <= lastRow; i++)
                {
                    var keyValue = ws.Cell(keyColumn + i).Value.ToString();
                    if (string.IsNullOrEmpty(keyValue))
                    {
                        var keyHeader = ws.Cell(keyColumn + (initialRow - 1)).Value.ToString();
                        result.Message = keyHeader + "not found in excel row:" + i;

                        return result;
                    }

                    if (string.IsNullOrEmpty(ws.Cell("E" + i).Value.ToString()))
                    {
                        result.Message = "Payment Date not found in excel row: " + i;
                        return result;
                    }
                    DateTime PaymentDate = DateTime.ParseExact(ws.Cell("E" + i).GetFormattedString(), "dd.MM.yyyy", null);

                    double amountIdr = 0.00;
                    if (!string.IsNullOrEmpty(Convert.ToString(ws.Cell("D" + i).Value)))
                    {
                        amountIdr = Convert.ToDouble(ws.Cell("D" + i).Value);
                    }

                    var incomingPayment = _domainService.GetAllIncomingPaymentNonSpm().Where(x =>
                        x.BillingNumber.Equals(keyValue.Trim())
                        && x.SegmentId == segmentId)
                        .FirstOrDefault();

                    if (incomingPayment != null)
                    {
                        incomingPayment.SegmentId = segmentId;
                        incomingPayment.SourceId = sourceId;
                        incomingPayment.PicId = picId;

                        incomingPayment.UpdatedAt = now;
                        incomingPayment.UpdatedApp = currentApp;
                        incomingPayment.UpdatedBy = currentUser;
                        incomingPayment.Revision++;

                        incomingPayment.VirtualAccountNumber = ws.Cell("B" + i).Value.ToString();
                        incomingPayment.VirtualAccountName = ws.Cell("C" + i).Value.ToString();
                        incomingPayment.AmountIdr = (double)amountIdr;
                        incomingPayment.PaymentDate = DateOnly.FromDateTime(PaymentDate);
                        incomingPayment.CustomerName = ws.Cell("I" + i).CachedValue.ToString();
                        incomingPayment.CustomerCode = ws.Cell("J" + i).CachedValue.ToString();
                        incomingPayment.BranchCode = ws.Cell("K" + i).CachedValue.ToString();
                        incomingPayment.InterfaceNumber = ws.Cell("L" + i).Value.ToString();
                        incomingPayment.ClearingNumber = ws.Cell("M" + i).Value.ToString();
                        incomingPayment.BillingNumber = ws.Cell("N" + i).Value.ToString();

                        if (incomingPayment.ClearingNumber != ws.Cell("M" + i).Value.ToString())
                        {
                            incomingPayment.ClearingNumber = ws.Cell("M" + i).Value.ToString();
                            incomingPayment.ClearingDate = DateOnly.FromDateTime(now);
                        }

                        _domainService.UpdateIncomingPaymentNonSpm(incomingPayment);
                    }
                    else
                    {
                        var newItem = new IncomingPaymentNonSpm
                        {
                            SegmentId = segmentId,
                            SourceId = sourceId,
                            PicId = picId,

                            CreatedAt = now,
                            CreatedApp = currentApp,
                            CreatedBy = currentUser,
                            Revision = ConfigConstants.N_INIT_REVISION
                        };

                        newItem.VirtualAccountNumber = ws.Cell("B" + i).Value.ToString();
                        newItem.VirtualAccountName = ws.Cell("C" + i).Value.ToString();
                        newItem.AmountIdr = (double)ws.Cell("D" + i).Value;
                        newItem.PaymentDate = DateOnly.FromDateTime(PaymentDate);
                        newItem.CustomerName = ws.Cell("I" + i).CachedValue.ToString();
                        newItem.CustomerCode = ws.Cell("J" + i).CachedValue.ToString();
                        newItem.BranchCode = ws.Cell("K" + i).CachedValue.ToString();
                        newItem.InterfaceNumber = ws.Cell("L" + i).Value.ToString();
                        newItem.BillingNumber = ws.Cell("N" + i).Value.ToString().Trim();

                        if (!string.IsNullOrEmpty(ws.Cell("M" + i).Value.ToString()))
                        {
                            newItem.ClearingNumber = ws.Cell("M" + i).Value.ToString();
                            newItem.ClearingDate = DateOnly.FromDateTime(now);
                        }

                        _domainService.InsertIncomingPaymentNonSpm(newItem);
                    }
                }

                result.Success = true;
                result.Message = MessageConstants.S_INCOMINGPAYMENT_UPLOAD;

                _domainService.SaveChanges();

                return result;
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: ImportIncomingPaymentTemplate04, filePath: {filePath}" +
                    $"Message: {ex.Message}");
                throw;
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        private ResultBase ImportIncomingPaymentTemplate05(int segmentId, int sourceId, int picId, string filePath)
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

                XLWorkbook wb = new XLWorkbook(filePath);
                IXLWorksheet ws = wb.Worksheet(1);

                var columns = "B,C,D,E,I,J,K,L,M,N".Split(",").ToArray();
                var headers = "No VA,Nama VA,Amount,Tanggal,Nama Cust.,Cust. Code,Cab,Interface,Clearing,No Billing"
                .Split(",").ToArray();
                var idx = 0;
                foreach (var column in columns)
                {
                    var value = ws.Cell(column + 4).Value.ToString();
                    if (!string.Equals(value?.Trim(), headers[idx].Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        result.Message = MessageConstants.S_INCORRECT_TEMPLATE;
                        return result;
                    }

                    idx++;
                }

                var keyColumn = "N";
                var initialRow = 5;
                var lastRow = ws.Column(keyColumn).LastCellUsed().Address.RowNumber;

                #region "Check duplicates key"
                var duplicates = ws.Range(keyColumn + initialRow, keyColumn + lastRow).CellsUsed().Select(x => x.Value.ToString()).ToList();
                if (duplicates.Count != duplicates.Distinct().Count())
                {
                    result.Message = "duplicate key in excel";
                    return result;
                }
                #endregion

                for (int i = initialRow; i <= lastRow; i++)
                {
                    var keyValue = ws.Cell(keyColumn + i).Value.ToString();
                    if (string.IsNullOrEmpty(keyValue))
                    {
                        var keyHeader = ws.Cell(keyColumn + (initialRow - 1)).Value.ToString();
                        result.Message = keyHeader + "not found in excel row:" + i;

                        return result;
                    }

                    if (string.IsNullOrEmpty(ws.Cell("E" + i).Value.ToString()))
                    {
                        result.Message = "Payment Date not found in excel row: " + i;
                        return result;
                    }
                    DateTime PaymentDate = DateTime.ParseExact(ws.Cell("E" + i).GetFormattedString(), "dd.MM.yyyy", null);

                    double amountIdr = 0.00;
                    if (!string.IsNullOrEmpty(Convert.ToString(ws.Cell("D" + i).Value)))
                    {
                        amountIdr = Convert.ToDouble(ws.Cell("D" + i).Value);
                    }

                    var incomingPayment = _domainService.GetAllIncomingPaymentNonSpm().Where(x =>
                        x.BillingNumber.Equals(keyValue.Trim())
                        && x.SegmentId == segmentId)
                        .FirstOrDefault();

                    if (incomingPayment != null)
                    {
                        incomingPayment.SegmentId = segmentId;
                        incomingPayment.SourceId = sourceId;
                        incomingPayment.PicId = picId;

                        incomingPayment.UpdatedAt = now;
                        incomingPayment.UpdatedApp = currentApp;
                        incomingPayment.UpdatedBy = currentUser;
                        incomingPayment.Revision++;

                        incomingPayment.VirtualAccountNumber = ws.Cell("B" + i).Value.ToString();
                        incomingPayment.VirtualAccountName = ws.Cell("C" + i).Value.ToString();
                        incomingPayment.AmountIdr = (double)amountIdr;
                        incomingPayment.PaymentDate = DateOnly.FromDateTime(PaymentDate);
                        incomingPayment.CustomerName = ws.Cell("I" + i).CachedValue.ToString();
                        incomingPayment.CustomerCode = ws.Cell("J" + i).CachedValue.ToString();
                        incomingPayment.BranchCode = ws.Cell("K" + i).CachedValue.ToString();
                        incomingPayment.InterfaceNumber = ws.Cell("L" + i).Value.ToString();
                        incomingPayment.BillingNumber = ws.Cell("N" + i).Value.ToString();

                        if (incomingPayment.ClearingNumber != ws.Cell("M" + i).Value.ToString())
                        {
                            incomingPayment.ClearingNumber = ws.Cell("M" + i).Value.ToString();
                            incomingPayment.ClearingDate = DateOnly.FromDateTime(now);
                        }

                        _domainService.UpdateIncomingPaymentNonSpm(incomingPayment);
                    }
                    else
                    {
                        var newItem = new IncomingPaymentNonSpm
                        {
                            SegmentId = segmentId,
                            SourceId = sourceId,
                            PicId = picId,

                            CreatedAt = now,
                            CreatedApp = currentApp,
                            CreatedBy = currentUser,
                            Revision = ConfigConstants.N_INIT_REVISION
                        };

                        newItem.VirtualAccountNumber = ws.Cell("B" + i).Value.ToString();
                        newItem.VirtualAccountName = ws.Cell("C" + i).Value.ToString();
                        newItem.AmountIdr = (double)amountIdr;
                        newItem.PaymentDate = DateOnly.FromDateTime(PaymentDate);
                        newItem.CustomerName = ws.Cell("I" + i).CachedValue.ToString();
                        newItem.CustomerCode = ws.Cell("J" + i).CachedValue.ToString();
                        newItem.BranchCode = ws.Cell("K" + i).CachedValue.ToString();
                        newItem.InterfaceNumber = ws.Cell("L" + i).Value.ToString();
                        newItem.BillingNumber = ws.Cell("N" + i).Value.ToString().Trim();

                        if (!string.IsNullOrEmpty(ws.Cell("M" + i).Value.ToString()))
                        {
                            newItem.ClearingNumber = ws.Cell("M" + i).Value.ToString();
                            newItem.ClearingDate = DateOnly.FromDateTime(now);
                        }

                        _domainService.InsertIncomingPaymentNonSpm(newItem);
                    }
                }

                result.Success = true;
                result.Message = MessageConstants.S_INCOMINGPAYMENT_UPLOAD;

                _domainService.SaveChanges();

                return result;
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: ImportIncomingPaymentTemplate05, filePath: {filePath}" +
                    $"Message: {ex.Message}");
                throw;
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        private ResultBase ImportIncomingPaymentTemplate06(int segmentId, int sourceId, int picId, string filePath)
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

                XLWorkbook wb = new XLWorkbook(filePath);
                IXLWorksheet ws = wb.Worksheet(1);

                var columns = "B,C,E,F,G,K,L,M,N,O".Split(",").ToArray();
                var headers = "No VA,Nama VA,Group,Amount,Tanggal,Cust. Code,Cab,Interface,Clearing,No Billing"
                .Split(",").ToArray();
                var idx = 0;
                foreach (var column in columns)
                {
                    var value = ws.Cell(column + 4).Value.ToString();
                    if (!string.Equals(value?.Trim(), headers[idx].Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        result.Message = MessageConstants.S_INCORRECT_TEMPLATE;
                        return result;
                    }

                    idx++;
                }

                var keyColumn = "O";
                var initialRow = 5;
                var lastRow = ws.Column(keyColumn).LastCellUsed().Address.RowNumber;

                #region "Check duplicates key"
                var duplicates = ws.Range(keyColumn + initialRow, keyColumn + lastRow).CellsUsed().Select(x => x.Value.ToString()).ToList();
                if (duplicates.Count != duplicates.Distinct().Count())
                {
                    result.Message = "duplicate key in excel";
                    return result;
                }
                #endregion

                for (int i = initialRow; i <= lastRow; i++)
                {
                    var keyValue = ws.Cell(keyColumn + i).Value.ToString();
                    if (string.IsNullOrEmpty(keyValue))
                    {
                        var keyHeader = ws.Cell(keyColumn + (initialRow - 1)).Value.ToString();
                        result.Message = keyHeader + "not found in excel row:" + i;

                        return result;
                    }

                    if (string.IsNullOrEmpty(ws.Cell("G" + i).Value.ToString()))
                    {
                        result.Message = "Payment Date not found in excel row: " + i;
                        return result;
                    }
                    DateTime PaymentDate = DateTime.ParseExact(ws.Cell("G" + i).GetFormattedString(), "dd.MM.yyyy", null);

                    double amountIdr = 0.00;
                    if (!string.IsNullOrEmpty(Convert.ToString(ws.Cell("F" + i).Value)))
                    {
                        amountIdr = Convert.ToDouble(ws.Cell("F" + i).Value);
                    }

                    var incomingPayment = _domainService.GetAllIncomingPaymentNonSpm().Where(x =>
                        x.BillingNumber.Equals(keyValue.Trim())
                        && x.SegmentId == segmentId)
                        .FirstOrDefault();

                    if (incomingPayment != null)
                    {
                        incomingPayment.SegmentId = segmentId;
                        incomingPayment.SourceId = sourceId;
                        incomingPayment.PicId = picId;

                        incomingPayment.UpdatedAt = now;
                        incomingPayment.UpdatedApp = currentApp;
                        incomingPayment.UpdatedBy = currentUser;
                        incomingPayment.Revision++;

                        incomingPayment.VirtualAccountNumber = ws.Cell("B" + i).Value.ToString();
                        incomingPayment.VirtualAccountName = ws.Cell("C" + i).Value.ToString();
                        incomingPayment.GroupCode = ws.Cell("E" + i).Value.ToString();
                        incomingPayment.AmountIdr = (double)amountIdr;
                        incomingPayment.PaymentDate = DateOnly.FromDateTime(PaymentDate);
                        incomingPayment.CustomerCode = ws.Cell("K" + i).CachedValue.ToString();
                        incomingPayment.BranchCode = ws.Cell("L" + i).CachedValue.ToString();
                        incomingPayment.InterfaceNumber = ws.Cell("M" + i).Value.ToString();
                        incomingPayment.BillingNumber = ws.Cell("O" + i).Value.ToString();

                        if (incomingPayment.ClearingNumber != ws.Cell("N" + i).Value.ToString())
                        {
                            incomingPayment.ClearingNumber = ws.Cell("N" + i).Value.ToString();
                            incomingPayment.ClearingDate = DateOnly.FromDateTime(now);
                        }

                        _domainService.UpdateIncomingPaymentNonSpm(incomingPayment);
                    }
                    else
                    {
                        var newItem = new IncomingPaymentNonSpm
                        {
                            SegmentId = segmentId,
                            SourceId = sourceId,
                            PicId = picId,

                            CreatedAt = now,
                            CreatedApp = currentApp,
                            CreatedBy = currentUser,
                            Revision = ConfigConstants.N_INIT_REVISION
                        };

                        newItem.VirtualAccountNumber = ws.Cell("B" + i).Value.ToString();
                        newItem.VirtualAccountName = ws.Cell("C" + i).Value.ToString();
                        newItem.GroupCode = ws.Cell("E" + i).Value.ToString();
                        newItem.AmountIdr = (double)amountIdr;
                        newItem.PaymentDate = DateOnly.FromDateTime(PaymentDate);
                        newItem.CustomerCode = ws.Cell("K" + i).CachedValue.ToString();
                        newItem.BranchCode = ws.Cell("L" + i).CachedValue.ToString();
                        newItem.InterfaceNumber = ws.Cell("M" + i).Value.ToString();
                        newItem.BillingNumber = ws.Cell("O" + i).Value.ToString().Trim();

                        if (!string.IsNullOrEmpty(ws.Cell("N" + i).Value.ToString()))
                        {
                            newItem.ClearingNumber = ws.Cell("N" + i).Value.ToString();
                            newItem.ClearingDate = DateOnly.FromDateTime(now);
                        }

                        _domainService.InsertIncomingPaymentNonSpm(newItem);
                    }
                }

                result.Success = true;
                result.Message = MessageConstants.S_INCOMINGPAYMENT_UPLOAD;

                _domainService.SaveChanges();

                return result;
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: ImportIncomingPaymentTemplate06, filePath: {filePath}" +
                    $"Message: {ex.Message}");
                throw;
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        private ResultBase ImportIncomingPaymentTemplate07(int segmentId, int sourceId, int picId, string filePath)
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

                XLWorkbook wb = new XLWorkbook(filePath);
                IXLWorksheet ws = wb.Worksheet(1);

                var columns = "B,C,D,E,F,G,H,K,L".Split(",").ToArray();
                var headers = "No VA,Nama VA,Amount,Tanggal,Nama Cust.,Cust. Code,Cab,Clearing,No Billing"
                .Split(",").ToArray();
                var idx = 0;
                foreach (var column in columns)
                {
                    var value = ws.Cell(column + 4).Value.ToString();
                    if (!string.Equals(value?.Trim(), headers[idx].Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        result.Message = MessageConstants.S_INCORRECT_TEMPLATE;
                        return result;
                    }

                    idx++;
                }

                var keyColumn = "L";
                var initialRow = 5;
                var lastRow = ws.Column(keyColumn).LastCellUsed().Address.RowNumber;

                #region "Check duplicates key"
                var duplicates = ws.Range(keyColumn + initialRow, keyColumn + lastRow).CellsUsed().Select(x => x.Value.ToString()).ToList();
                if (duplicates.Count != duplicates.Distinct().Count())
                {
                    result.Message = "duplicate key in excel";
                    return result;
                }
                #endregion

                for (int i = initialRow; i <= lastRow; i++)
                {
                    var keyValue = Convert.ToString(ws.Cell(keyColumn + i).Value);
                    if (string.IsNullOrEmpty(keyValue))
                    {
                        var keyHeader = Convert.ToString(ws.Cell(keyColumn + (initialRow - 1)).Value);
                        result.Message = keyHeader + "not found in excel row: " + i;

                        return result;
                    }

                    if (string.IsNullOrEmpty(ws.Cell("E" + i).Value.ToString()))
                    {
                        result.Message = "Payment Date not found in excel row: " + i;

                        return result;
                    }
                    DateTime PaymentDate = DateTime.ParseExact(ws.Cell("E" + i).GetFormattedString(), "dd.MM.yyyy", null);

                    double amountIdr = 0.00;
                    if (!string.IsNullOrEmpty(Convert.ToString(ws.Cell("D" + i).Value)))
                    {
                        amountIdr = Convert.ToDouble(ws.Cell("D" + i).Value);
                    }

                    var incomingPayment = _domainService.GetAllIncomingPaymentNonSpm().FirstOrDefault(x =>
                        x.BillingNumber.Equals(keyValue.Trim())
                        && x.SegmentId == segmentId);

                    if (incomingPayment != null)
                    {
                        incomingPayment.UpdatedAt = now;
                        incomingPayment.UpdatedApp = currentApp;
                        incomingPayment.UpdatedBy = currentUser;
                        incomingPayment.Revision++;

                        incomingPayment.VirtualAccountNumber = ws.Cell("B" + i).Value.ToString();
                        incomingPayment.VirtualAccountName = ws.Cell("C" + i).Value.ToString();
                        incomingPayment.AmountIdr = (double)amountIdr;
                        incomingPayment.PaymentDate = DateOnly.FromDateTime(PaymentDate);
                        incomingPayment.CustomerName = ws.Cell("F" + i).CachedValue.ToString();
                        incomingPayment.CustomerCode = ws.Cell("G" + i).CachedValue.ToString();
                        incomingPayment.BranchCode = ws.Cell("H" + i).CachedValue.ToString();

                        if (incomingPayment.ClearingNumber != ws.Cell("K" + i).Value.ToString())
                        {
                            incomingPayment.ClearingNumber = ws.Cell("K" + i).Value.ToString();
                            incomingPayment.ClearingDate = DateOnly.FromDateTime(now);
                        }

                        _domainService.UpdateIncomingPaymentNonSpm(incomingPayment);
                    }
                    else
                    {
                        var newItem = new IncomingPaymentNonSpm
                        {
                            SegmentId = segmentId,
                            SourceId = sourceId,
                            PicId = picId,

                            CreatedAt = now,
                            CreatedApp = currentApp,
                            CreatedBy = currentUser,
                            Revision = ConfigConstants.N_INIT_REVISION
                        };

                        newItem.VirtualAccountNumber = ws.Cell("B" + i).Value.ToString();
                        newItem.VirtualAccountName = ws.Cell("C" + i).Value.ToString();
                        newItem.AmountIdr = (double)amountIdr;
                        newItem.PaymentDate = DateOnly.FromDateTime(PaymentDate);
                        newItem.CustomerName = ws.Cell("F" + i).CachedValue.ToString();
                        newItem.CustomerCode = ws.Cell("G" + i).CachedValue.ToString();
                        newItem.BranchCode = ws.Cell("H" + i).CachedValue.ToString();
                        newItem.BillingNumber = ws.Cell("L" + i).Value.ToString().Trim();

                        if (!string.IsNullOrEmpty(ws.Cell("K" + i).Value.ToString()))
                        {
                            newItem.ClearingNumber = ws.Cell("K" + i).Value.ToString();
                            newItem.ClearingDate = DateOnly.FromDateTime(now);
                        }

                        _domainService.InsertIncomingPaymentNonSpm(newItem);
                    }
                }

                result.Success = true;
                result.Message = MessageConstants.S_INCOMINGPAYMENT_UPLOAD;

                _domainService.SaveChanges();

                return result;
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: ImportIncomingPaymentTemplate07, segment: {segmentId}, source: {sourceId}, pic: {picId},  filePath: {filePath} " +
                    $"Message: {ex.Message}");
                throw;
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        private ResultBase ImportIncomingPaymentTemplate08(int segmentId, int sourceId, int picId, string filePath)
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

                XLWorkbook wb = new XLWorkbook(filePath);
                IXLWorksheet ws = wb.Worksheet(1);

                var columns = "B,C,D,E,I,J,K,M,N".Split(",").ToArray();
                var headers = "No VA,Nama VA,Amount,Tanggal,Nama Cust,Cust Code,Cab,Clearing,No billing"
                .Split(",").ToArray();
                var idx = 0;
                foreach (var column in columns)
                {
                    var value = ws.Cell(column + 5).Value.ToString();
                    if (!string.Equals(value?.Trim(), headers[idx].Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        result.Message = MessageConstants.S_INCORRECT_TEMPLATE;
                        return result;
                    }

                    idx++;
                }

                var keyColumn = "N";
                var initialRow = 6;
                var lastRow = ws.Column(keyColumn).LastCellUsed().Address.RowNumber;

                #region "Check duplicates key"
                var duplicates = ws.Range(keyColumn + initialRow, keyColumn + lastRow).CellsUsed().Select(x => x.Value.ToString()).ToList();
                if (duplicates.Count != duplicates.Distinct().Count())
                {
                    result.Message = "duplicate key in excel";
                    return result;
                }
                #endregion

                for (int i = initialRow; i <= lastRow; i++)
                {
                    var keyValue = Convert.ToString(ws.Cell(keyColumn + i).Value);
                    if (string.IsNullOrEmpty(keyValue))
                    {
                        var keyHeader = Convert.ToString(ws.Cell(keyColumn + (initialRow - 1)).Value);
                        result.Message = keyHeader + "not found in excel row: " + i;

                        return result;
                    }

                    if (string.IsNullOrEmpty(ws.Cell("E" + i).Value.ToString()))
                    {
                        result.Message = "Payment Date not found in excel row: " + i;

                        return result;
                    }
                    DateTime PaymentDate = DateTime.ParseExact(ws.Cell("E" + i).GetFormattedString(), "dd.MM.yyyy", null);

                    double amountIdr = 0.00;
                    if (!string.IsNullOrEmpty(Convert.ToString(ws.Cell("D" + i).Value)))
                    {
                        amountIdr = Convert.ToDouble(ws.Cell("D" + i).Value);
                    }

                    var incomingPayment = _domainService.GetAllIncomingPaymentNonSpm().FirstOrDefault(x =>
                        x.BillingNumber.Equals(keyValue.Trim())
                        && x.SegmentId == segmentId);

                    if (incomingPayment != null)
                    {
                        incomingPayment.UpdatedAt = now;
                        incomingPayment.UpdatedApp = currentApp;
                        incomingPayment.UpdatedBy = currentUser;
                        incomingPayment.Revision++;

                        incomingPayment.VirtualAccountNumber = ws.Cell("B" + i).Value.ToString();
                        incomingPayment.VirtualAccountName = ws.Cell("C" + i).Value.ToString();
                        incomingPayment.AmountIdr = (double)amountIdr;
                        incomingPayment.PaymentDate = DateOnly.FromDateTime(PaymentDate);
                        incomingPayment.CustomerName = ws.Cell("I" + i).CachedValue.ToString();
                        incomingPayment.CustomerCode = ws.Cell("J" + i).CachedValue.ToString();
                        incomingPayment.BranchCode = ws.Cell("K" + i).CachedValue.ToString();

                        if (incomingPayment.ClearingNumber != ws.Cell("M" + i).Value.ToString())
                        {
                            incomingPayment.ClearingNumber = ws.Cell("M" + i).Value.ToString();
                            incomingPayment.ClearingDate = DateOnly.FromDateTime(now);
                        }

                        _domainService.UpdateIncomingPaymentNonSpm(incomingPayment);
                    }
                    else
                    {
                        var newItem = new IncomingPaymentNonSpm
                        {
                            SegmentId = segmentId,
                            SourceId = sourceId,
                            PicId = picId,

                            CreatedAt = now,
                            CreatedApp = currentApp,
                            CreatedBy = currentUser,
                            Revision = ConfigConstants.N_INIT_REVISION
                        };

                        newItem.VirtualAccountNumber = ws.Cell("B" + i).Value.ToString();
                        newItem.VirtualAccountName = ws.Cell("C" + i).Value.ToString();
                        newItem.AmountIdr = (double)amountIdr;
                        newItem.PaymentDate = DateOnly.FromDateTime(PaymentDate);
                        newItem.CustomerName = ws.Cell("I" + i).CachedValue.ToString();
                        newItem.CustomerCode = ws.Cell("J" + i).CachedValue.ToString();
                        newItem.BranchCode = ws.Cell("K" + i).CachedValue.ToString();
                        newItem.BillingNumber = ws.Cell("N" + i).Value.ToString().Trim();

                        if (!string.IsNullOrEmpty(ws.Cell("M" + i).Value.ToString()))
                        {
                            newItem.ClearingNumber = ws.Cell("M" + i).Value.ToString();
                            newItem.ClearingDate = DateOnly.FromDateTime(now);
                        }

                        _domainService.InsertIncomingPaymentNonSpm(newItem);
                    }
                }

                result.Success = true;
                result.Message = MessageConstants.S_INCOMINGPAYMENT_UPLOAD;

                _domainService.SaveChanges();

                return result;
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: ImportIncomingPaymentTemplate08, segment: {segmentId}, source: {sourceId}, pic: {picId},  filePath: {filePath} " +
                    $"Message: {ex.Message}");
                throw;
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        private ResultBase ImportIncomingPaymentTemplate09(int segmentId, int sourceId, int picId, string filePath)
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

                XLWorkbook wb = new XLWorkbook(filePath);
                IXLWorksheet ws = wb.Worksheet(1);

                var columns = "B,C,D,E,F,G,H".Split(",").ToArray();
                var headers = "No VA,Customer,Amount,Tanggal,ID Customer,Clearing,no billing"
                .Split(",").ToArray();
                var idx = 0;
                foreach (var column in columns)
                {
                    var value = ws.Cell(column + 5).Value.ToString();
                    if (!string.Equals(value?.Trim(), headers[idx].Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        result.Message = MessageConstants.S_INCORRECT_TEMPLATE;
                        return result;
                    }

                    idx++;
                }

                var keyColumn = "H";
                var initialRow = 6;
                var lastRow = ws.Column(keyColumn).LastCellUsed().Address.RowNumber;

                #region "Check duplicates key"
                var duplicates = ws.Range(keyColumn + initialRow, keyColumn + lastRow).CellsUsed().Select(x => x.Value.ToString()).ToList();
                if (duplicates.Count != duplicates.Distinct().Count())
                {
                    result.Message = "duplicate key in excel";
                    return result;
                }
                #endregion

                for (int i = initialRow; i <= lastRow; i++)
                {
                    var keyValue = Convert.ToString(ws.Cell(keyColumn + i).Value);
                    if (string.IsNullOrEmpty(keyValue))
                    {
                        var keyHeader = Convert.ToString(ws.Cell(keyColumn + (initialRow - 1)).Value);
                        result.Message = keyHeader + "not found in excel row: " + i;

                        return result;
                    }

                    if (string.IsNullOrEmpty(ws.Cell("E" + i).Value.ToString()))
                    {
                        result.Message = "Payment Date not found in excel row: " + i;

                        return result;
                    }
                    DateTime PaymentDate = DateTime.ParseExact(ws.Cell("E" + i).GetFormattedString(), "dd/MM/yy", null);

                    double amountIdr = 0.00;
                    if (!string.IsNullOrEmpty(Convert.ToString(ws.Cell("D" + i).Value)))
                    {
                        amountIdr = Convert.ToDouble(ws.Cell("D" + i).Value);
                    }

                    var incomingPayment = _domainService.GetAllIncomingPaymentNonSpm().FirstOrDefault(x =>
                        x.BillingNumber.Equals(keyValue.Trim())
                        && x.SegmentId == segmentId);

                    if (incomingPayment != null)
                    {
                        incomingPayment.UpdatedAt = now;
                        incomingPayment.UpdatedApp = currentApp;
                        incomingPayment.UpdatedBy = currentUser;
                        incomingPayment.Revision = incomingPayment.Revision + 1;

                        incomingPayment.VirtualAccountNumber = ws.Cell("B" + i).Value.ToString();
                        incomingPayment.CustomerName = ws.Cell("C" + i).Value.ToString();
                        incomingPayment.AmountIdr = (double)amountIdr;
                        incomingPayment.PaymentDate = DateOnly.FromDateTime(PaymentDate);
                        incomingPayment.CustomerCode = ws.Cell("F" + i).Value.ToString();

                        if (incomingPayment.ClearingNumber != ws.Cell("G" + i).Value.ToString())
                        {
                            incomingPayment.ClearingNumber = ws.Cell("G" + i).Value.ToString();
                            incomingPayment.ClearingDate = DateOnly.FromDateTime(now);
                        }

                        _domainService.UpdateIncomingPaymentNonSpm(incomingPayment);
                    }
                    else
                    {
                        var newItem = new IncomingPaymentNonSpm
                        {
                            SegmentId = segmentId,
                            SourceId = sourceId,
                            PicId = picId,

                            CreatedAt = now,
                            CreatedApp = currentApp,
                            CreatedBy = currentUser,
                            Revision = ConfigConstants.N_INIT_REVISION
                        };

                        newItem.VirtualAccountNumber = ws.Cell("B" + i).Value.ToString();
                        newItem.CustomerName = ws.Cell("C" + i).Value.ToString();
                        newItem.AmountIdr = (double)amountIdr;
                        newItem.PaymentDate = DateOnly.FromDateTime(PaymentDate);
                        newItem.CustomerCode = ws.Cell("F" + i).Value.ToString();
                        newItem.BillingNumber = ws.Cell("H" + i).Value.ToString().Trim();

                        if (!string.IsNullOrEmpty(ws.Cell("G" + i).Value.ToString()))
                        {
                            newItem.ClearingNumber = ws.Cell("G" + i).Value.ToString();
                            newItem.ClearingDate = DateOnly.FromDateTime(now);
                        }

                        _domainService.InsertIncomingPaymentNonSpm(newItem);
                    }
                }

                result.Success = true;
                result.Message = MessageConstants.S_INCOMINGPAYMENT_UPLOAD;

                _domainService.SaveChanges();

                return result;
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: ImportIncomingPaymentTemplate09, segment: {segmentId}, source: {sourceId}, pic: {picId},  filePath: {filePath} " +
                    $"Message: {ex.Message}");
                throw;
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        private ResultBase ImportIncomingPaymentTemplate10(int segmentId, int sourceId, int picId, string filePath)
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

                XLWorkbook wb = new XLWorkbook(filePath);
                IXLWorksheet ws = wb.Worksheet(1);

                var columns = "B,C,D,E,F,G,H".Split(",").ToArray();
                var headers = "No VA,Customer,Amount,Tanggal,ID Customer,Interface,Clearing"
                .Split(",").ToArray();
                var idx = 0;
                foreach (var column in columns)
                {
                    var value = ws.Cell(column + 5).Value.ToString();
                    if (!string.Equals(value?.Trim(), headers[idx].Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        result.Message = MessageConstants.S_INCORRECT_TEMPLATE;
                        return result;
                    }

                    idx++;
                }

                var keyColumn = "G";
                var initialRow = 6;
                var lastRow = ws.Column(keyColumn).LastCellUsed().Address.RowNumber;

                #region "Check duplicates key"
                var columnDocInterface = ws.Range(keyColumn + initialRow, keyColumn + lastRow).CellsUsed().Select(x => x.Value.ToString()).ToList();
                var columnYearPaymentDate = ws.Range("E" + initialRow, "E" + lastRow).CellsUsed().Select(x => Convert.ToDateTime(x.Value).Year).ToList();
                var duplicates = new List<string>();
                for (int i = 0; i < columnDocInterface.Count; i++)
                {
                    duplicates.Add(columnDocInterface[i] + columnYearPaymentDate[i]);
                }


                if (duplicates.Count != duplicates.Distinct().Count())
                {
                    result.Message = "duplicate key in excel";
                    return result;
                }
                #endregion

                for (int i = initialRow; i <= lastRow; i++)
                {
                    var keyValue = Convert.ToString(ws.Cell(keyColumn + i).Value);
                    if (string.IsNullOrEmpty(keyValue))
                    {
                        var keyHeader = Convert.ToString(ws.Cell(keyColumn + (initialRow - 1)).Value);
                        result.Message = keyHeader + "not found in excel row: " + i;

                        return result;
                    }

                    if (string.IsNullOrEmpty(ws.Cell("E" + i).Value.ToString()))
                    {
                        result.Message = "Payment Date not found in excel row: " + i;

                        return result;
                    }
                    DateTime PaymentDate = (DateTime)ws.Cell("E" + i).Value;

                    double amountIdr = 0.00;
                    if (!string.IsNullOrEmpty(Convert.ToString(ws.Cell("D" + i).Value)))
                    {
                        amountIdr = Convert.ToDouble(ws.Cell("D" + i).Value);
                    }

                    var incomingPayment = _domainService.GetAllIncomingPaymentNonSpm().FirstOrDefault(x =>
                        x.InterfaceNumber.Equals(keyValue.Trim())
                        && x.PaymentDate.Year == DateOnly.FromDateTime((DateTime)ws.Cell("E" + i).Value).Year
                        && x.SegmentId == segmentId);

                    if (incomingPayment != null)
                    {
                        incomingPayment.UpdatedAt = now;
                        incomingPayment.UpdatedApp = currentApp;
                        incomingPayment.UpdatedBy = currentUser;
                        incomingPayment.Revision++;

                        incomingPayment.VirtualAccountNumber = ws.Cell("B" + i).Value.ToString();
                        incomingPayment.CustomerName = ws.Cell("C" + i).Value.ToString();
                        incomingPayment.AmountIdr = (double)amountIdr;
                        incomingPayment.CustomerCode = ws.Cell("F" + i).CachedValue.ToString();

                        if (incomingPayment.ClearingNumber != ws.Cell("H" + i).Value.ToString())
                        {
                            incomingPayment.ClearingNumber = ws.Cell("H" + i).Value.ToString();
                            incomingPayment.ClearingDate = DateOnly.FromDateTime(now);
                        }

                        _domainService.UpdateIncomingPaymentNonSpm(incomingPayment);
                    }
                    else
                    {
                        var newItem = new IncomingPaymentNonSpm
                        {
                            SegmentId = segmentId,
                            SourceId = sourceId,
                            PicId = picId,

                            CreatedAt = now,
                            CreatedApp = currentApp,
                            CreatedBy = currentUser,
                            Revision = ConfigConstants.N_INIT_REVISION
                        };

                        newItem.VirtualAccountNumber = ws.Cell("B" + i).Value.ToString();
                        newItem.CustomerName = ws.Cell("C" + i).Value.ToString();
                        newItem.AmountIdr = (double)amountIdr;
                        newItem.PaymentDate = DateOnly.FromDateTime(PaymentDate);
                        newItem.CustomerCode = ws.Cell("F" + i).CachedValue.ToString();
                        newItem.InterfaceNumber = ws.Cell("G" + i).Value.ToString().Trim();

                        if (!string.IsNullOrEmpty(ws.Cell("H" + i).Value.ToString()))
                        {
                            newItem.ClearingNumber = ws.Cell("H" + i).Value.ToString();
                            newItem.ClearingDate = DateOnly.FromDateTime(now);
                        }

                        _domainService.InsertIncomingPaymentNonSpm(newItem);
                    }
                }

                result.Success = true;
                result.Message = MessageConstants.S_INCOMINGPAYMENT_UPLOAD;

                _domainService.SaveChanges();

                return result;
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: ImportIncomingPaymentTemplate10, segment: {segmentId}, source: {sourceId}, pic: {picId},  filePath: {filePath} " +
                    $"Message: {ex.Message}");
                throw;
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        private ResultBase ImportIncomingPaymentTemplate11(int segmentId, int sourceId, int picId, string filePath)
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

                XLWorkbook wb = new XLWorkbook(filePath);
                IXLWorksheet ws = wb.Worksheet(1);

                var columns = "F,J,K,L".Split(",").ToArray();
                var headers = "PERIODE,NO BILLING / NO INVOICE,NOMINAL,TGL UPLOAD"
                .Split(",").ToArray();
                var idx = 0;
                foreach (var column in columns)
                {
                    var value = ws.Cell(column + 1).Value.ToString();
                    if (!string.Equals(value?.Trim(), headers[idx].Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        result.Message = MessageConstants.S_INCORRECT_TEMPLATE;
                        return result;
                    }

                    idx++;
                }

                var keyColumn = "J";
                var initialRow = 2;
                var lastRow = ws.Column(keyColumn).LastCellUsed().Address.RowNumber;

                #region "Check duplicates key"
                var duplicates = ws.Range(keyColumn + initialRow, keyColumn + lastRow).CellsUsed().Select(x => x.Value.ToString()).ToList();
                if (duplicates.Count != duplicates.Distinct().Count())
                {
                    result.Message = "duplicate key in excel";
                    return result;
                }
                #endregion

                for (int i = initialRow; i <= lastRow; i++)
                {
                    var keyValue = Convert.ToString(ws.Cell(keyColumn + i).Value);
                    if (string.IsNullOrEmpty(keyValue))
                    {
                        var keyHeader = Convert.ToString(ws.Cell(keyColumn + (initialRow - 1)).Value);
                        result.Message = keyHeader + "not found in excel row: " + i;

                        return result;
                    }

                    if (string.IsNullOrEmpty(ws.Cell("F" + i).Value.ToString()))
                    {
                        var keyHeader = Convert.ToString(ws.Cell("F" + (initialRow - 1)).Value);
                        result.Message = keyHeader + "not found in excel row: " + i;

                        return result;
                    }
                    DateTime paymentDate = (DateTime)ws.Cell("F" + i).Value;
                    DateTime uploadDate = DateTime.ParseExact(ws.Cell("L" + i).GetFormattedString(), "d-MMM", null);

                    double amountIdr = 0.00;
                    if (!string.IsNullOrEmpty(Convert.ToString(ws.Cell("K" + i).Value)))
                    {
                        amountIdr = Convert.ToDouble(ws.Cell("K" + i).Value);
                    }

                    var incomingPayment = _domainService.GetAllIncomingPaymentNonSpm().FirstOrDefault(x =>
                        x.BillingNumber.Equals(keyValue.Trim())
                        && x.SegmentId == segmentId);
                    if (incomingPayment != null)
                    {
                        incomingPayment.UpdatedAt = now;
                        incomingPayment.UpdatedApp = currentApp;
                        incomingPayment.UpdatedBy = currentUser;
                        incomingPayment.Revision++;

                        incomingPayment.PaymentDate = DateOnly.FromDateTime(paymentDate);
                        incomingPayment.AmountIdr = (double)amountIdr;

                        if (incomingPayment.UploadDate != DateOnly.FromDateTime(uploadDate))
                        {
                            incomingPayment.UploadDate = DateOnly.FromDateTime(uploadDate);
                            incomingPayment.UploadDateInputDate = DateOnly.FromDateTime(uploadDate);
                        }

                        _domainService.UpdateIncomingPaymentNonSpm(incomingPayment);
                    }
                    else
                    {
                        var newItem = new IncomingPaymentNonSpm
                        {
                            SegmentId = segmentId,
                            SourceId = sourceId,
                            PicId = picId,

                            CreatedAt = now,
                            CreatedApp = currentApp,
                            CreatedBy = currentUser,
                            Revision = ConfigConstants.N_INIT_REVISION
                        };

                        newItem.PaymentDate = DateOnly.FromDateTime(paymentDate);
                        newItem.BillingNumber = ws.Cell("J" + i).Value.ToString();
                        newItem.InvoiceNumber = ws.Cell("J" + i).Value.ToString();
                        newItem.AmountIdr = Convert.ToDouble(amountIdr);
                        newItem.UploadDate = DateOnly.FromDateTime(uploadDate);
                        newItem.UploadDateInputDate = DateOnly.FromDateTime(uploadDate);

                        _domainService.InsertIncomingPaymentNonSpm(newItem);
                    }
                }

                result.Success = true;
                result.Message = MessageConstants.S_INCOMINGPAYMENT_UPLOAD;

                _domainService.SaveChanges();

                return result;
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: ImportIncomingPaymentTemplate11, segment: {segmentId}, source: {sourceId}, pic: {picId},  filePath: {filePath} " +
                    $"Message: {ex.Message}");
                throw;
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        private ResultBase ImportIncomingPaymentTemplate12(int segmentId, int sourceId, int picId, string filePath)
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

                XLWorkbook wb = new XLWorkbook(filePath);
                IXLWorksheet ws = wb.Worksheet(1);

                var columns = "B,C,D,E,F,H,I,J".Split(",").ToArray();
                var headers = "CUSTOMER PO NO.,INVOICE NO.,DESTINATION,ETD,Billing,Amount,Amount IDR,Customer"
                .Split(",").ToArray();
                var idx = 0;
                foreach (var column in columns)
                {
                    var value = ws.Cell(column + 2).Value.ToString();
                    if (!string.Equals(value?.Trim(), headers[idx].Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        result.Message = MessageConstants.S_INCORRECT_TEMPLATE;
                        return result;
                    }

                    idx++;
                }

                var keyColumn = "F";
                var initialRow = 3;
                var lastRow = ws.Column(keyColumn).LastCellUsed().Address.RowNumber;

                #region "Check duplicates key"
                var duplicates = ws.Range(keyColumn + initialRow, keyColumn + lastRow).CellsUsed().Select(x => x.Value.ToString()).ToList();
                if (duplicates.Count != duplicates.Distinct().Count())
                {
                    result.Message = "duplicate key in excel";
                    return result;
                }
                #endregion

                for (int i = initialRow; i <= lastRow; i++)
                {
                    var keyValue = Convert.ToString(ws.Cell(keyColumn + i).Value);
                    if (string.IsNullOrEmpty(keyValue))
                    {
                        var keyHeader = Convert.ToString(ws.Cell(keyColumn + (initialRow - 1)).Value);
                        result.Message = keyHeader + "not found in excel row: " + i;

                        return result;
                    }

                    if (string.IsNullOrEmpty(ws.Cell("E" + i).Value.ToString()))
                    {
                        result.Message = "Payment Date not found in excel row: " + i;
                        return result;
                    }
                    DateTime PaymentDate = DateTime.ParseExact(ws.Cell("E" + i).GetFormattedString(), "d-MMM-yy", null);

                    double amountUsd = 0.00;
                    if (!string.IsNullOrEmpty(Convert.ToString(ws.Cell("H" + i).Value)))
                    {
                        amountUsd = Convert.ToDouble(ws.Cell("H" + i).Value);
                    }

                    double amountIdr = 0.00;
                    if (!string.IsNullOrEmpty(Convert.ToString(ws.Cell("I" + i).Value)))
                    {
                        amountIdr = Convert.ToDouble(ws.Cell("I" + i).Value);
                    }

                    var incomingPayment = _domainService.GetAllIncomingPaymentNonSpm().FirstOrDefault(x =>
                        x.BillingNumber.Equals(keyValue.Trim())
                        && x.SegmentId == segmentId);

                    if (incomingPayment != null)
                    {
                        incomingPayment.UpdatedAt = now;
                        incomingPayment.UpdatedApp = currentApp;
                        incomingPayment.UpdatedBy = currentUser;
                        incomingPayment.Revision++;

                        incomingPayment.CustomerPoNumber = ws.Cell("B" + i).Value.ToString();

                        incomingPayment.Destination = ws.Cell("D" + i).Value.ToString();
                        incomingPayment.PaymentDate = DateOnly.FromDateTime(PaymentDate);
                        incomingPayment.AmountUsd = Convert.ToDouble(amountUsd);
                        incomingPayment.AmountIdr = Convert.ToDouble(amountIdr);
                        incomingPayment.CustomerName = ws.Cell("J" + i).Value.ToString();

                        if (incomingPayment.InvoiceNumber != ws.Cell("C" + i).Value.ToString())
                        {
                            incomingPayment.InvoiceNumber = ws.Cell("C" + i).Value.ToString();
                            incomingPayment.InvoiceDate = DateOnly.FromDateTime(PaymentDate);
                        }

                        _domainService.UpdateIncomingPaymentNonSpm(incomingPayment);
                    }
                    else
                    {
                        var newItem = new IncomingPaymentNonSpm
                        {
                            SegmentId = segmentId,
                            SourceId = sourceId,
                            PicId = picId,

                            CreatedAt = now,
                            CreatedApp = currentApp,
                            CreatedBy = currentUser,
                            Revision = ConfigConstants.N_INIT_REVISION
                        };

                        newItem.CustomerPoNumber = ws.Cell("B" + i).Value.ToString();
                        newItem.InvoiceNumber = ws.Cell("C" + i).Value.ToString();
                        newItem.Destination = ws.Cell("D" + i).Value.ToString();
                        newItem.PaymentDate = DateOnly.FromDateTime(PaymentDate);
                        newItem.BillingNumber = ws.Cell("F" + i).Value.ToString().Trim();
                        newItem.AmountUsd = (double)amountUsd;
                        newItem.AmountIdr = (double)amountIdr;
                        newItem.CustomerName = ws.Cell("J" + i).Value.ToString();
                        newItem.InvoiceDate = DateOnly.FromDateTime(PaymentDate);

                        _domainService.InsertIncomingPaymentNonSpm(newItem);
                    }
                }

                result.Success = true;
                result.Message = MessageConstants.S_INCOMINGPAYMENT_UPLOAD;

                _domainService.SaveChanges();

                return result;
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: ImportIncomingPaymentTemplate12, segment: {segmentId}, source: {sourceId}, pic: {picId},  filePath: {filePath} " +
                    $"Message: {ex.Message}");
                throw;
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        private ResultBase ImportIncomingPaymentTemplate13(int segmentId, int sourceId, int picId, string filePath)
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

                XLWorkbook wb = new XLWorkbook(filePath);
                IXLWorksheet ws = wb.Worksheet(1);

                var columns = "B,C,G,H,I".Split(",").ToArray();
                var headers = "USD CAMSLINK,IDR CAMSLINK,TGL CAMSLINK,NO.CLEARING,CUSTOMER"
                .Split(",").ToArray();
                var idx = 0;
                foreach (var column in columns)
                {
                    var value = ws.Cell(column + 4).Value.ToString();
                    if (!string.Equals(value?.Trim(), headers[idx].Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        result.Message = MessageConstants.S_INCORRECT_TEMPLATE;
                        return result;
                    }

                    idx++;
                }

                var keyColumn01 = "B";
                var keyColumn02 = "C";
                var keyColumn03 = "G";
                var keyColumn04 = "I";
                var initialRow = 5;
                var lastRow = ws.Column(keyColumn04).LastCellUsed().Address.RowNumber;

                for (int i = initialRow; i <= lastRow; i++)
                {
                    var keyValue01 = Convert.ToString(ws.Cell(keyColumn01 + i).Value);
                    var keyValue02 = Convert.ToString(ws.Cell(keyColumn02 + i).Value);
                    var keyValue03 = Convert.ToString(ws.Cell(keyColumn03 + i).Value);
                    var keyValue04 = Convert.ToString(ws.Cell(keyColumn04 + i).Value);


                    if (!string.IsNullOrEmpty(keyValue03) && !string.IsNullOrEmpty(keyValue04))
                    {
                        if (string.IsNullOrEmpty(keyValue01) && string.IsNullOrEmpty(keyValue02))
                        {
                            result.Message = "Please check USD Camslink & IDR Camslink, row: " + i;
                            return result;
                        }

                        var paymentDate = Convert.ToString(ws.Cell("G" + i).Value);
                        if (string.IsNullOrEmpty(paymentDate))
                        {
                            var keyHeader = Convert.ToString(ws.Cell(keyColumn03 + (initialRow - 1)).Value);
                            result.Message = keyHeader + "not found in excel row: " + i;
                            return result;
                        }
                        DateTime PaymentDate = DateTime.ParseExact(ws.Cell("G" + i).GetFormattedString(), "yyyy.MM.dd", null);
                        var arnexCode = keyValue01.Replace(".", "") + keyValue02.Replace(".", "") + keyValue03.Replace(".", "") + keyValue04.Replace(" ", "");

                        double amountIdr = 0.00;
                        if (!string.IsNullOrEmpty(Convert.ToString(ws.Cell("C" + i).Value)))
                        {
                            amountIdr = Convert.ToDouble(ws.Cell("C" + i).Value);
                        }

                        double amountUsd = 0.00;
                        if (!string.IsNullOrEmpty(Convert.ToString(ws.Cell("B" + i).Value)))
                        {
                            amountUsd = Convert.ToDouble(ws.Cell("B" + i).Value);
                        }

                        var incomingPayment = _domainService.GetAllIncomingPaymentNonSpm().FirstOrDefault(x =>
                            x.ArnexCode == arnexCode
                            && x.SegmentId == segmentId);
                        if (incomingPayment != null)
                        {
                            incomingPayment.UpdatedAt = now;
                            incomingPayment.UpdatedApp = currentApp;
                            incomingPayment.UpdatedBy = currentUser;
                            incomingPayment.Revision = incomingPayment.Revision + 1;

                            if (incomingPayment.ClearingNumber != ws.Cell("H" + i).Value.ToString())
                            {
                                incomingPayment.ClearingNumber = ws.Cell("H" + i).Value.ToString();
                                incomingPayment.ClearingDate = DateOnly.FromDateTime(now);
                            }
                            incomingPayment.CustomerName = ws.Cell("I" + i).Value.ToString();

                            _domainService.UpdateIncomingPaymentNonSpm(incomingPayment);
                        }
                        else
                        {
                            var newItem = new IncomingPaymentNonSpm
                            {
                                SegmentId = segmentId,
                                SourceId = sourceId,
                                PicId = picId,

                                CreatedAt = now,
                                CreatedApp = currentApp,
                                CreatedBy = currentUser,
                                Revision = ConfigConstants.N_INIT_REVISION
                            };

                            newItem.ArnexCode = arnexCode;
                            newItem.AmountUsd = (double)amountUsd;
                            newItem.AmountIdr = (double)amountIdr;
                            newItem.PaymentDate = DateOnly.FromDateTime(PaymentDate);
                            newItem.CustomerName = ws.Cell("I" + i).Value.ToString();

                            if (!string.IsNullOrEmpty(ws.Cell("H" + i).Value.ToString()))
                            {
                                newItem.ClearingNumber = ws.Cell("H" + i).Value.ToString();
                                newItem.ClearingDate = DateOnly.FromDateTime(now);
                            }

                            _domainService.InsertIncomingPaymentNonSpm(newItem);
                        }
                    }
                }

                result.Success = true;
                result.Message = MessageConstants.S_INCOMINGPAYMENT_UPLOAD;

                _domainService.SaveChanges();

                return result;
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: ImportIncomingPaymentTemplate13, segment: {segmentId}, source: {sourceId}, pic: {picId},  filePath: {filePath} " +
                    $"Message: {ex.Message}");
                throw;
            }
            finally
            {
                File.Delete(filePath);
            }
        }
    }
}
