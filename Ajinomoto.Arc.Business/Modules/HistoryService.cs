using Ajinomoto.Arc.Business.Interfaces;
using Ajinomoto.Arc.Data.Models;

namespace Ajinomoto.Arc.Business.Modules
{
    public class HistoryService : IHistoryService
    {
        private readonly IDomainService _domainService;
        private readonly IProfileService _profileService;

        public HistoryService(IDomainService domainService, IProfileService profileService)
        {
            _domainService = domainService;
            _profileService = profileService;
        }

        public void AddBpkHistory(IncomingPayment incomingPayment, Bpk bpk, DateTime now, int actionId)
        {
            var userLogin = _profileService.GetUserLogin();

            var bpkHistory = new BpkHistory
            {
                BpkHistoryId = Guid.NewGuid(),
                BpkId = bpk.BpkId,
                BpkStatusId = bpk.BpkStatusId,
                ClearingStatusId = incomingPayment.ClearingStatusId,
                AppActionId = actionId,
                ActionBy = userLogin.Username,
                ActionAt = now,

                CreatedAt = now,
                CreatedApp = userLogin.App,
                CreatedBy = userLogin.Username
            };

            _domainService.InsertBpkHistory(bpkHistory);
        }
    }
}
