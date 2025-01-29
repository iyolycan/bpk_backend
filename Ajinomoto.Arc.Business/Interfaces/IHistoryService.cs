using Ajinomoto.Arc.Data.Models;

namespace Ajinomoto.Arc.Business.Interfaces
{
    public interface IHistoryService
    {
        void AddBpkHistory(IncomingPayment incomingPayment, Bpk bpk, DateTime now, int actionId);
    }
}
