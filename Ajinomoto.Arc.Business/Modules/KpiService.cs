using Ajinomoto.Arc.Business.Interfaces;
using Ajinomoto.Arc.Common.Constants;
using Ajinomoto.Arc.Common.DtoModels;
using Ajinomoto.Arc.Common.Enums;
using Ajinomoto.Arc.Data.Models;
using ClosedXML.Excel;
using Serilog;

namespace Ajinomoto.Arc.Business.Modules
{
    public class KpiService : IKpiService
    {
        private readonly IDomainService _domainService;
        private readonly IMasterDataService _masterDataService;
        private readonly IProfileService _profileService;

        public KpiService(IDomainService domainService, IMasterDataService masterDataService, IProfileService profileService)
        {
            _domainService = domainService;
            _masterDataService = masterDataService;
            _profileService = profileService;
        }

        public async Task<XLWorkbook> GenerateKpiReport(string period, int? picId, List<int> kpiProperties = null, string labels = null)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var data = AssembleKpiData(period, picId);

                    var fileTemplate = Path.Combine(Environment.CurrentDirectory, @"Template\template-kpi-report.xlsx");

                    XLWorkbook wb = new XLWorkbook(fileTemplate);
                    IXLWorksheet ws = wb.Worksheet("kpi summary");

                    var month = Convert.ToInt32(period.Substring(0, 2));
                    var year = Convert.ToInt32(period.Substring(3, 4));
                    var currPeriod = new DateTime(year, month, 1);
                    var text = currPeriod.ToString("MMM yyyy");
                    var currentRow = 5;
                    ws.Cell("D3").Value = "'" + currPeriod.ToString("MMM yyyy");

                    if (labels != null)
                    {
                        var rowLabel = 4;
                        var columnLabel = 4;
                        var listLabels = labels.Split(",");
                        // Change label column
                        for (int i = 0; i < listLabels.Length; i++)
                        {
                            ws.Cell(rowLabel, columnLabel++).Value = listLabels[i];
                        }
                    }

                    var noDataColor = XLColor.FromArgb(217, 217, 217);

                    foreach (var segment in data)
                    {
                        ws.Cell("A" + currentRow).Value = segment.SegmentName;
                        ws.Cell("A" + currentRow).Style.Font.SetBold();
                        ws.Cell("A" + currentRow).Style.Border.LeftBorder = XLBorderStyleValues.Medium;
                        ws.Cell("A" + currentRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;

                        if (kpiProperties == null)
                        {
                            kpiProperties = segment.KpiProperties.Split(",").Select(Int32.Parse).ToList();
                        }

                        foreach (var segmentPic in segment.SegmentPics)
                        {
                            ws.Cell("B" + currentRow).Value = segmentPic.PicName;
                            ws.Cell("B" + currentRow).Style.Font.SetBold();
                            ws.Cell("B" + currentRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;

                            foreach (var detail in segmentPic.Details)
                            {
                                ws.Cell("B" + currentRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                ws.Cell("B" + currentRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;

                                ws.Cell("C" + currentRow).Value = detail.Source;
                                ws.Cell("C" + currentRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;

                                if (kpiProperties.Contains((int)KpiPropertyEnum.ArTransaction))
                                {
                                    ws.Cell("D" + currentRow).Value = detail.ArTransaction;
                                }
                                else
                                {
                                    ws.Cell("D" + currentRow).Style.Fill.BackgroundColor = noDataColor;
                                }
                                ws.Cell("D" + currentRow).Style.NumberFormat.Format = ConfigConstants.S_ACCOUNTING_FORMAT;
                                ws.Cell("D" + currentRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;

                                if (kpiProperties.Contains((int)KpiPropertyEnum.BpkReceived))
                                {
                                    ws.Cell("E" + currentRow).Value = detail.BpkReceived;
                                }
                                else
                                {
                                    ws.Cell("E" + currentRow).Style.Fill.BackgroundColor = noDataColor;
                                }
                                ws.Cell("E" + currentRow).Style.NumberFormat.Format = ConfigConstants.S_ACCOUNTING_FORMAT;
                                ws.Cell("E" + currentRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;

                                if (kpiProperties.Contains((int)KpiPropertyEnum.ClearingAr))
                                {
                                    ws.Cell("F" + currentRow).Value = detail.ClearingAr;
                                }
                                else
                                {
                                    ws.Cell("F" + currentRow).Style.Fill.BackgroundColor = noDataColor;
                                }
                                ws.Cell("F" + currentRow).Style.NumberFormat.Format = ConfigConstants.S_ACCOUNTING_FORMAT;
                                ws.Cell("F" + currentRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;

                                if (kpiProperties.Contains((int)KpiPropertyEnum.UploadInvoice))
                                {
                                    ws.Cell("G" + currentRow).Value = detail.UploadInvoice;
                                }
                                else
                                {
                                    ws.Cell("G" + currentRow).Style.Fill.BackgroundColor = noDataColor;
                                }
                                ws.Cell("G" + currentRow).Style.NumberFormat.Format = ConfigConstants.S_ACCOUNTING_FORMAT;
                                ws.Cell("G" + currentRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;

                                if (kpiProperties.Contains((int)KpiPropertyEnum.CreateInvoice))
                                {
                                    ws.Cell("H" + currentRow).Value = detail.CreateInvoice;
                                }
                                else
                                {
                                    ws.Cell("H" + currentRow).Style.Fill.BackgroundColor = noDataColor;
                                }
                                ws.Cell("H" + currentRow).Style.NumberFormat.Format = ConfigConstants.S_ACCOUNTING_FORMAT;
                                ws.Cell("H" + currentRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;

                                ws.Cell("I" + currentRow).Value = detail.Achievement;
                                ws.Cell("I" + currentRow).Style.Border.RightBorder = XLBorderStyleValues.Medium;

                                currentRow++;
                            }

                            // total transaction
                            ws.Cell("A" + currentRow).Style.Border.LeftBorder = XLBorderStyleValues.Medium;
                            ws.Cell("A" + currentRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("B" + currentRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;

                            ws.Cell("C" + currentRow).Value = "Total (Trans)";
                            ws.Cell("C" + currentRow).Style.Font.SetBold();
                            ws.Cell("C" + currentRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("C" + currentRow).Style.Border.BottomBorder = XLBorderStyleValues.Medium;

                            var totalTransaction = segmentPic.TotalTransaction;
                            if (kpiProperties.Contains((int)KpiPropertyEnum.ArTransaction))
                            {
                                ws.Cell("D" + currentRow).Value = totalTransaction.ArTransaction;
                            }
                            else
                            {
                                ws.Cell("D" + currentRow).Style.Fill.BackgroundColor = noDataColor;
                            }
                            ws.Cell("D" + currentRow).Style.NumberFormat.Format = ConfigConstants.S_ACCOUNTING_FORMAT;
                            ws.Cell("D" + currentRow).Style.Font.SetBold();
                            ws.Cell("D" + currentRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("D" + currentRow).Style.Border.BottomBorder = XLBorderStyleValues.Medium;

                            if (kpiProperties.Contains((int)KpiPropertyEnum.BpkReceived))
                            {
                                ws.Cell("E" + currentRow).Value = totalTransaction.BpkReceived;
                            }
                            else
                            {
                                ws.Cell("E" + currentRow).Style.Fill.BackgroundColor = noDataColor;
                            }
                            ws.Cell("E" + currentRow).Style.NumberFormat.Format = ConfigConstants.S_ACCOUNTING_FORMAT;
                            ws.Cell("E" + currentRow).Style.Font.SetBold();
                            ws.Cell("E" + currentRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("E" + currentRow).Style.Border.BottomBorder = XLBorderStyleValues.Medium;

                            if (kpiProperties.Contains((int)KpiPropertyEnum.ClearingAr))
                            {
                                ws.Cell("F" + currentRow).Value = totalTransaction.ClearingAr;
                            }
                            else
                            {
                                ws.Cell("F" + currentRow).Style.Fill.BackgroundColor = noDataColor;
                            }
                            ws.Cell("F" + currentRow).Style.NumberFormat.Format = ConfigConstants.S_ACCOUNTING_FORMAT;
                            ws.Cell("F" + currentRow).Style.Font.SetBold();
                            ws.Cell("F" + currentRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("F" + currentRow).Style.Border.BottomBorder = XLBorderStyleValues.Medium;

                            if (kpiProperties.Contains((int)KpiPropertyEnum.UploadInvoice))
                            {
                                ws.Cell("G" + currentRow).Value = totalTransaction.UploadInvoice;
                            }
                            else
                            {
                                ws.Cell("G" + currentRow).Style.Fill.BackgroundColor = noDataColor;
                            }
                            ws.Cell("G" + currentRow).Style.NumberFormat.Format = ConfigConstants.S_ACCOUNTING_FORMAT;
                            ws.Cell("G" + currentRow).Style.Font.SetBold();
                            ws.Cell("G" + currentRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("G" + currentRow).Style.Border.BottomBorder = XLBorderStyleValues.Medium;

                            if (kpiProperties.Contains((int)KpiPropertyEnum.CreateInvoice))
                            {
                                ws.Cell("H" + currentRow).Value = totalTransaction.CreateInvoice;
                            }
                            else
                            {
                                ws.Cell("H" + currentRow).Style.Fill.BackgroundColor = noDataColor;
                            }
                            ws.Cell("H" + currentRow).Style.NumberFormat.Format = ConfigConstants.S_ACCOUNTING_FORMAT;
                            ws.Cell("H" + currentRow).Style.Font.SetBold();
                            ws.Cell("H" + currentRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("H" + currentRow).Style.Border.BottomBorder = XLBorderStyleValues.Medium;

                            ws.Cell("I" + currentRow).Value = totalTransaction.Achievement;
                            ws.Cell("I" + currentRow).Style.NumberFormat.NumberFormatId = 9;
                            ws.Cell("I" + currentRow).Style.Font.SetBold();
                            ws.Cell("I" + currentRow).Style.Border.RightBorder = XLBorderStyleValues.Medium;
                            ws.Cell("I" + currentRow).Style.Border.BottomBorder = XLBorderStyleValues.Medium;

                            currentRow++;

                            // total amount
                            ws.Cell("A" + currentRow).Style.Border.LeftBorder = XLBorderStyleValues.Medium;
                            ws.Cell("A" + currentRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("B" + currentRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;

                            ws.Cell("C" + currentRow).Value = "Total (IDR)";
                            ws.Cell("C" + currentRow).Style.Font.SetBold();
                            ws.Cell("C" + currentRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;

                            ws.Cell("D" + currentRow).Value = segmentPic.TotalAmountIdr;
                            ws.Cell("D" + currentRow).Style.NumberFormat.Format = ConfigConstants.S_ACCOUNTING_FORMAT;
                            ws.Cell("D" + currentRow).Style.Font.SetBold();
                            ws.Cell("D" + currentRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;

                            ws.Cell("E" + currentRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("F" + currentRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("G" + currentRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("H" + currentRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("I" + currentRow).Style.Border.RightBorder = XLBorderStyleValues.Medium;

                            if (segment.HasAmountUsd)
                            {
                                currentRow++;

                                ws.Cell("A" + currentRow).Style.Border.LeftBorder = XLBorderStyleValues.Medium;
                                ws.Cell("A" + currentRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                ws.Cell("A" + currentRow).Style.Border.BottomBorder = XLBorderStyleValues.Medium;
                                ws.Cell("B" + currentRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                ws.Cell("B" + currentRow).Style.Border.BottomBorder = XLBorderStyleValues.Medium;

                                ws.Cell("C" + currentRow).Value = "Total (USD)";
                                ws.Cell("C" + currentRow).Style.Font.SetBold();
                                ws.Cell("C" + currentRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                ws.Cell("C" + currentRow).Style.Border.BottomBorder = XLBorderStyleValues.Medium;

                                ws.Cell("D" + currentRow).Value = segmentPic.TotalAmountUsd;
                                ws.Cell("D" + currentRow).Style.NumberFormat.Format = ConfigConstants.S_ACCOUNTING_FORMAT;
                                ws.Cell("D" + currentRow).Style.Font.SetBold();
                                ws.Cell("D" + currentRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                ws.Cell("D" + currentRow).Style.Border.BottomBorder = XLBorderStyleValues.Medium;

                                ws.Cell("E" + currentRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                ws.Cell("E" + currentRow).Style.Border.BottomBorder = XLBorderStyleValues.Medium;
                                ws.Cell("F" + currentRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                ws.Cell("F" + currentRow).Style.Border.BottomBorder = XLBorderStyleValues.Medium;
                                ws.Cell("G" + currentRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                ws.Cell("G" + currentRow).Style.Border.BottomBorder = XLBorderStyleValues.Medium;
                                ws.Cell("H" + currentRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                ws.Cell("H" + currentRow).Style.Border.BottomBorder = XLBorderStyleValues.Medium;
                                ws.Cell("I" + currentRow).Style.Border.RightBorder = XLBorderStyleValues.Medium;
                                ws.Cell("I" + currentRow).Style.Border.BottomBorder = XLBorderStyleValues.Medium;
                            }

                            ws.Cell("A" + currentRow).Style.Border.BottomBorder = XLBorderStyleValues.Medium;
                            ws.Cell("B" + currentRow).Style.Border.BottomBorder = XLBorderStyleValues.Medium;
                            ws.Cell("C" + currentRow).Style.Border.BottomBorder = XLBorderStyleValues.Medium;
                            ws.Cell("D" + currentRow).Style.Border.BottomBorder = XLBorderStyleValues.Medium;
                            ws.Cell("E" + currentRow).Style.Border.BottomBorder = XLBorderStyleValues.Medium;
                            ws.Cell("F" + currentRow).Style.Border.BottomBorder = XLBorderStyleValues.Medium;
                            ws.Cell("G" + currentRow).Style.Border.BottomBorder = XLBorderStyleValues.Medium;
                            ws.Cell("H" + currentRow).Style.Border.BottomBorder = XLBorderStyleValues.Medium;
                            ws.Cell("I" + currentRow).Style.Border.BottomBorder = XLBorderStyleValues.Medium;

                            currentRow++;
                        }
                    }

                    return wb;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: GenerateKpiReport(), period: {period}" +
                        $"Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<List<KpiDataResponse>> GetKpiData(string period)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = new List<KpiDataResponse>();

                    result = AssembleKpiData(period);

                    return result;
                }
                catch (Exception)
                {

                    throw;
                }

            }).ConfigureAwait(false);
        }

        private List<KpiDataResponse> AssembleKpiData(string period, int? picId = null)
        {
            var result = new List<KpiDataResponse>();

            var periodMonth = Convert.ToInt32(period.Substring(0, 2));
            var periodYear = Convert.ToInt32(period.Substring(3, 4));

            var cutOffDate = new DateOnly();
            var incomingPaymentCutOff = _domainService.GetAllIncomingPaymentCutOff()
                .SingleOrDefault(x => x.Period.Month == periodMonth && x.Period.Year == periodYear);
            if (incomingPaymentCutOff == null)
            {
                cutOffDate = new DateOnly(periodYear, periodMonth, DateTime.DaysInMonth(periodYear, periodMonth));
            }
            else
            {
                cutOffDate = incomingPaymentCutOff.CutOffDate;
            }

            var periodDate = new DateOnly(periodYear, periodMonth, 1);
            var segments = _masterDataService.GetAllActiveSegment().OrderBy(x => x.SegmentId).ToList();
            foreach (var segment in segments)
            {
                var dataTemp = GetKpiData(periodDate, cutOffDate, segment.SegmentId, picId);

                result = result.Concat(dataTemp).ToList();
            }

            //modify result pic area
            result = updatePicArea(result);

            return result;
        }

        private List<KpiDataResponse> GetKpiData(DateOnly periodDate, DateOnly cutOffDate, int segmentId, int? picId)
        {
            var result = new List<KpiDataResponse>();

            var userLogin = _profileService.GetUserLogin();
            var nonSpmAllowedRole = new List<int>
            {
                (int)RoleEnum.Coec,
                (int) RoleEnum.Administrator
            };

            var data = new List<KpiDataModel>();
            if (segmentId == (int)SegmentEnum.Spm)
            {
                data = GetSpmKpiData(periodDate, cutOffDate, segmentId, picId);
            }
            else if (nonSpmAllowedRole.Contains(userLogin.RoleId))
            {
                data = GetNonSpmKpiData(periodDate, cutOffDate, segmentId, picId);
            }

            if (data.Count <= 0)
            {
                return result;
            }

            var segment = _domainService.GetAllSegment().Single(x => x.SegmentId == segmentId);
            var kpiProperties = _domainService.GetAllSegmentKpiProperty()
                .Where(x => x.SegmentId == segmentId)
                .Select(x => x.KpiPropertyId)
                .ToList();

            var branchGrouping = data.GroupBy(x => new
            {
                x.BranchId,
                x.BranchName,
            })
                .Select(x => new KpiDataResponse
                {
                    Period = periodDate.ToString("MM-yyyy"),
                    SegmentName = segmentId == (int)SegmentEnum.Spm ? segment.Name + " " + x.Key.BranchName : segment.Name,
                    BranchId = x.Key.BranchId,
                    KpiProperties = string.Join(",", kpiProperties.ToArray()),
                    KpiPropertyCurrentId = segment.KpiPropertyCurrentId,
                    KpiPropertyTotalId = segment.KpiPropertyTotalId,
                    HasAmountUsd = segment.HasAmountUsd,
                    SegmentPics = new List<KpiDataSegmentPicResponse>()
                }).ToList();

            foreach (var itemBranch in branchGrouping)
            {
                var picGrouping = data.Where(x => x.BranchId == itemBranch.BranchId).GroupBy(x => new
                {
                    x.PicId,
                    x.PicName,
                })
                    .Select(x => new KpiDataSegmentPicResponse
                    {
                        PicId = x.Key.PicId,
                        PicName = x.Key.PicName,
                        TotalAmountIdr = x.Sum(c => c.AmountIdr),
                        TotalAmountUsd = itemBranch.HasAmountUsd ? x.Sum(c => c.AmountUsd) : 0,
                        Details = new List<KpiDataDetailResponse>(),
                        TotalTransaction = new KpiDataDetailResponse
                        {
                            Source = "Total (Transaction)",
                            ArTransaction = x.Sum(c => c.ArTransaction),
                            BpkReceived = x.Sum(c => c.BpkReceived),
                            ClearingAr = x.Sum(c => c.ClearingAr),
                            UploadInvoice = x.Sum(c => c.UploadInvoice),
                            CreateInvoice = x.Sum(c => c.CreateInvoice),
                            Achievement = GetAchievement(segment, x)
                        },
                        Charts = new List<ChartResponse>(),
                    }).ToList();

                foreach (var itemPic in picGrouping)
                {
                    var sourceGrouping = data.Where(x =>
                        x.BranchId == itemBranch.BranchId
                        && x.PicId == itemPic.PicId).GroupBy(x => new
                        {
                            x.SourceId,
                            x.SourceName
                        })
                        .Select(x => new KpiDataDetailResponse
                        {
                            Source = x.Key.SourceName,
                            ArTransaction = x.Sum(c => c.ArTransaction),
                            BpkReceived = x.Sum(c => c.BpkReceived),
                            ClearingAr = x.Sum(c => c.ClearingAr),
                            UploadInvoice = x.Sum(c => c.UploadInvoice),
                            CreateInvoice = x.Sum(c => c.CreateInvoice),
                            Achievement = GetAchievement(segment, x)
                        }).ToList();

                    foreach (var itemSource in sourceGrouping)
                    {
                        itemPic.Details.Add(itemSource);
                    }

                    itemBranch.SegmentPics.Add(itemPic);

                    var valueAchieve = itemPic.TotalTransaction.Achievement.Replace("%", "");
                    var chart = new ChartResponse
                    {
                        Legend = "Achieve",
                        Value = Convert.ToDouble(valueAchieve)
                    };
                    itemPic.Charts.Add(chart);

                    chart = new ChartResponse
                    {
                        Legend = "Not Achieve",
                        Value = 100 - Convert.ToDouble(valueAchieve)
                    };
                    itemPic.Charts.Add(chart);
                }

                result.Add(itemBranch);
            }


            return result;
        }

        private List<KpiDataModel> GetNonSpmKpiData(DateOnly periodDate, DateOnly cutOffDate, int segmentId, int? picId)
        {
            var query = (from a in _domainService.GetAllIncomingPaymentNonSpm()
                         join c in _domainService.GetAllSource() on a.SourceId equals c.SourceId
                         join d in _domainService.GetAllAppUser() on a.PicId equals d.AppUserId
                         where a.SegmentId == segmentId
                         && a.PaymentDate.Month == periodDate.Month
                         && a.PaymentDate.Year == periodDate.Year
                         select new KpiDataModel
                         {
                             PicId = a.PicId,
                             PicName = d.FullName,
                             SourceId = a.SourceId,
                             SourceName = c.Name,
                             BranchId = 0,
                             BranchName = "",

                             ArTransaction = 1,
                             BpkReceived = 0,
                             ClearingAr = a.ClearingDate != null && a.ClearingDate <= cutOffDate ? 1 : 0,
                             UploadInvoice = a.UploadDateInputDate != null && a.UploadDateInputDate <= cutOffDate ? 1 : 0,
                             CreateInvoice = a.InvoiceDate != null && a.InvoiceDate <= cutOffDate ? 1 : 0,
                             AmountIdr = a.AmountIdr,
                             AmountUsd = a.AmountUsd ?? 0
                         });

            if (picId != null)
            {
                query = query.Where(x => x.PicId == picId);
            }

            var data = query.ToList();

            return data;
        }

        private List<KpiDataModel> GetSpmKpiData(DateOnly periodDate, DateOnly cutOffDate, int segmentId, int? picId)
        {
            var userLogin = _profileService.GetUserLogin();
            var areaIds = userLogin.AreaIds;

            var query = (from a in _domainService.GetAllIncomingPayment()
                         join bpk in _domainService.GetAllBpk() on a.BpkId equals bpk.BpkId into ab
                         from b in ab.DefaultIfEmpty()
                         join c in _domainService.GetAllSource() on a.SourceId equals c.SourceId
                         join d in _domainService.GetAllAppUser() on a.PicId equals d.AppUserId
                         join e in _domainService.GetAllArea() on a.AreaId equals e.AreaId
                         join f in _domainService.GetAllBranch() on e.BranchId equals f.BranchId
                         let g = _domainService.GetAllBpkHistory().Where(x =>
                                x.BpkStatusId == (int)BpkStatusEnum.BpkSubmitted
                                && b.BpkStatusId == (int)BpkStatusEnum.BpkSubmitted
                                && b.BpkId == x.BpkId)
                             .OrderByDescending(x => x.ActionAt)
                             .FirstOrDefault()
                         where a.SegmentId == segmentId
                         && a.PaymentDate.Month == periodDate.Month
                         && a.PaymentDate.Year == periodDate.Year
                         && areaIds.Contains(a.AreaId)
                         select new KpiDataModel
                         {
                             PicId = a.PicId,
                             PicName = d.FullName,
                             SourceId = a.SourceId,
                             SourceName = c.Name,
                             BranchId = f.BranchId,
                             BranchName = f.Name,

                             ArTransaction = 1,
                             BpkReceived = (b != null && (b.BpkStatusId == (int)BpkStatusEnum.BpkSubmitted)) ?
                                           (DateOnly.FromDateTime(g.ActionAt) <= cutOffDate ? 1 : 0) : 0,
                             ClearingAr = a.ClearingDate != null && a.ClearingDate <= cutOffDate ? 1 : 0,
                             UploadInvoice = 0,
                             CreateInvoice = 0,
                             AmountIdr = a.Amount,
                             AmountUsd = 0
                         });

            if (picId != null)
            {
                query = query.Where(x => x.PicId == picId);
            }

            var data = query.ToList();

            return data;
        }

        private string GetAchievement(Segment segment, IGrouping<object, KpiDataModel> x)
        {
            var userLogin = _profileService.GetUserLogin();
            var roleCoecAdministrator = new List<int>
            {
                (int)RoleEnum.Coec,
                (int)RoleEnum.Administrator
            };

            if (roleCoecAdministrator.Contains(userLogin.RoleId))
            {
                return GetAchievementCoecAdmin(segment, x);
            }
            else
            {
                return GetAchievementArea(x);
            }
        }

        private List<KpiDataResponse> updatePicArea(List<KpiDataResponse> itemPicArea)
        {
            var userLogin = _profileService.GetUserLogin();
            var roleCoecAdministrator = new List<int>
            {
                (int)RoleEnum.Coec,
                (int)RoleEnum.Administrator
            };

            if (!roleCoecAdministrator.Contains(userLogin.RoleId))
            {
                foreach (var item in itemPicArea)
                {
                    foreach (var segment in item.SegmentPics)
                    {
                        segment.PicId = userLogin.Id;
                        segment.PicName = userLogin.FullName;
                    }
                }
            }

            return itemPicArea;
        }

        private string GetAchievementArea(IGrouping<object, KpiDataModel> x)
        {
            var BpkReceived = x.Sum(c => c.BpkReceived);
            var ArTransaction = x.Sum(c => c.ArTransaction);

            decimal result = BpkReceived == 0 ? (decimal)0 : ((decimal)BpkReceived / ArTransaction);

            return result.ToString("0%");
        }

        private string GetAchievementCoecAdmin(Segment segment, IGrouping<object, KpiDataModel> x)
        {
            var kpiCurrent = 0;
            var kpiTotal = 0;

            // current
            switch (segment.KpiPropertyCurrentId)
            {
                case (int)KpiPropertyEnum.ClearingAr:
                    kpiCurrent = x.Sum(c => c.ClearingAr);
                    break;
                case (int)KpiPropertyEnum.UploadInvoice:
                    kpiCurrent = x.Sum(c => c.UploadInvoice);
                    break;
                case (int)KpiPropertyEnum.CreateInvoice:
                    kpiCurrent = x.Sum(c => c.CreateInvoice);
                    break;
            }

            // total
            switch (segment.KpiPropertyTotalId)
            {
                case (int)KpiPropertyEnum.BpkReceived:
                    kpiTotal = x.Sum(c => c.BpkReceived);
                    break;
                case (int)KpiPropertyEnum.ArTransaction:
                    kpiTotal = x.Sum(c => c.ArTransaction);
                    break;
            }

            decimal result = kpiTotal == 0 ? (decimal)0 : (decimal)kpiCurrent / kpiTotal;

            return result.ToString("0%");
        }
    }
}
