using Ajinomoto.Arc.Business.Interfaces;
using Ajinomoto.Arc.Common.DtoModels;
using Ajinomoto.Arc.Common.Enums;
using ClosedXML.Excel;

namespace Ajinomoto.Arc.Business.Facades
{
    public class ReportFacade : IReportFacade
    {
        private readonly IBpkService _bpkService;
        private readonly IIncomingPaymentService _incomingPaymentService;
        private readonly IKpiService _kpiService;
        private readonly IProfileService _profileService;

        public ReportFacade(IBpkService bpkService, IIncomingPaymentService incomingPaymentService, IKpiService kpiService, IProfileService profileService)
        {
            _bpkService = bpkService;
            _incomingPaymentService = incomingPaymentService;
            _kpiService = kpiService;
            _profileService = profileService;
        }

        public async Task<XLWorkbook> GenerateBpkReport(Guid incomingPaymentId)
        {
            var result = await _bpkService.GenerateBpkReport(incomingPaymentId);

            return result;
        }

        public async Task<XLWorkbook> GenerateIncomingPaymentListReport(IncomingPaymentListRequest param)
        {
            var result = await _incomingPaymentService.GenerateIncomingPaymentListReport(param);

            return result;
        }

        public async Task<XLWorkbook> GenerateKpiReport(string period, int? picId)
        {
            var userLogin = _profileService.GetUserLogin();
            var allowRoleCoecAdministrator = new List<int>
            {
                (int)RoleEnum.Coec,
                (int)RoleEnum.Administrator
            };

            XLWorkbook result;
            if (allowRoleCoecAdministrator.Contains(userLogin.RoleId))
            {
                result = await _kpiService.GenerateKpiReport(period, picId);
            }
            else
            {
                var kpiProperties = new List<int>
                {
                    (int)KpiPropertyEnum.ArTransaction,
                    (int)KpiPropertyEnum.BpkReceived
                };
                string labels = "Incoming Payment,BPK Submitted";

                result = await _kpiService.GenerateKpiReport(period, picId, kpiProperties, labels);
            }

            return result;
        }
    }
}
