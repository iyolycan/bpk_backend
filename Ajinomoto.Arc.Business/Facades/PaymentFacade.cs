using Ajinomoto.Arc.Business.Interfaces;
using Ajinomoto.Arc.Common.AppModels;
using Ajinomoto.Arc.Common.Constants;
using Ajinomoto.Arc.Common.DtoModels;
using Ajinomoto.Arc.Data.Models;

namespace Ajinomoto.Arc.Business.Facades
{
    public class PaymentFacade : IPaymentFacade
    {
        private readonly IIncomingPaymentService _incomingPaymentService;

        public PaymentFacade(IIncomingPaymentService incomingPaymentService)
        {
            _incomingPaymentService = incomingPaymentService;
        }

        public async Task<ResultBase<IncomingPaymentListResponse>> GetIncomingPaymentList(IncomingPaymentListRequest param)
        {
            var result = await _incomingPaymentService.GetIncomingPaymentList(param.Filter, param.SortOrder, 
                param.CurrentSort, param.SortDirection, param.Limit, param.Page, param.Cabang, param.Customer, param.StatusBpk, param.StatusClearing, param.Area, param.FromDate, param.ToDate);

            if (result != null)
            {
                return new ResultBase<IncomingPaymentListResponse>()
                {
                    Success = true,
                    Message = MessageConstants.S_SUCCESSFULLY,
                    Model = result
                };
            }

            var errorResponse = new ResultBase<IncomingPaymentListResponse>()
            {
                Success = false,
                Message = MessageConstants.S_DATA_NOT_FOUND,
                Model = result
            };

            return errorResponse;
        }

        public async Task<ResultBase> ImportIncomingPayment(ImportIncomingPaymentRequest param)
        {
            var result = await _incomingPaymentService.ImportIncomingPayment(param);

            return result;
        }

        public async Task<ResultBase> RemoveIncomingPayment(Guid incomingPaymentId)
        {
            var result = await _incomingPaymentService.RemoveIncomingPayment(incomingPaymentId);

            return result;
        }

        public async Task<ResultBase> RevertResettedBpkFromClearing()
        {
            var result = await _incomingPaymentService.RevertResettedBpkFromClearing();

            return result;
        }
    }
}
