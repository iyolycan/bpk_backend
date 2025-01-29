using Ajinomoto.Arc.Business.Interfaces;
using Ajinomoto.Arc.Common.AppModels;
using Ajinomoto.Arc.Common.Constants;
using Ajinomoto.Arc.Common.DtoModels;


namespace Ajinomoto.Arc.Business.Facades
{
    public class BpkFacade : IBpkFacade
    {
        private readonly IBpkService _bpkService;
        private readonly IInvoiceService _invoiceService;

        public BpkFacade(IBpkService bpkService, IInvoiceService invoiceService)
        {
            _bpkService = bpkService;
            _invoiceService = invoiceService;
        }

        public async Task<ResultBase> SaveBpk(SaveBpkRequest param)
        {
            var result = await _bpkService.SaveBpk(param);

            return result;
        }

        public async Task<ResultBase> SubmitBpk(SubmitBpkRequest param)
        {
            var result = await _bpkService.SubmitBpk(param.IncomingPaymentId);

            return result;
        }

        public async Task<ResultBase> OpenClearingBpk(Guid incomingPaymentId)
        {
            var result = await _bpkService.OpenClearingBpk(incomingPaymentId);

            return result;
        }

        public async Task<ResultBase> RequestForRevisionBpk(Guid incomingPaymentId)
        {
            var result = await _bpkService.RequestForRevisionBpk(incomingPaymentId);

            return result;
        }

        public async Task<ResultBase> ReviseApproveBpk(Guid incomingPaymentId)
        {
            var result = await _bpkService.ReviseApproveBpk(incomingPaymentId);

            return result;
        }

        public async Task<ResultBase> ReviseRejectBpk(RejectBpkRequest param)
        {
            var result = await _bpkService.ReviseRejectBpk(param);

            return result;
        }

        public async Task<ResultBase> SendReminder(string executeBy, string executeApp)
        {
            var result = await _bpkService.SendReminder(executeBy, executeApp);

            return result;
        }

        public async Task<ResultBase<BpkResponse>> GetBpk(Guid incomingPaymentId)
        {
            var result = await _bpkService.GetBpk(incomingPaymentId);

            if (result != null)
            {
                return new ResultBase<BpkResponse>()
                {
                    Success = true,
                    Message = MessageConstants.S_SUCCESSFULLY,
                    Model = result
                };
            }

            var errorResponse = new ResultBase<BpkResponse>()
            {
                Success = false,
                Message = MessageConstants.S_DATA_NOT_FOUND,
                Model = result
            };

            return errorResponse;
        }

        public async Task<ResultBase<InvoiceResponse>> GetInvoice(string invoiceNumber)
        {
            var result = await _invoiceService.GetInvoice(invoiceNumber);

            if (result != null)
            {
                return new ResultBase<InvoiceResponse>()
                {
                    Success = true,
                    Message = MessageConstants.S_SUCCESSFULLY,
                    Model = result
                };
            }

            var errorResponse = new ResultBase<InvoiceResponse>()
            {
                Success = false,
                Message = MessageConstants.S_DATA_NOT_FOUND,
                Model = result
            };

            return errorResponse;
        }

        public async Task<ResultBase<ValidateMultipleInvoiceResponse>> ValidateMultipleInvoices(string invoices)
        {
            var invoiceList = invoices.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var result = await _invoiceService.ValidateMultipleInvoices(invoiceList);

            if (result != null)
            {
                return new ResultBase<ValidateMultipleInvoiceResponse>()
                {
                    Success = true,
                    Message = MessageConstants.S_SUCCESSFULLY,
                    Model = result
                };
            }

            var errorResponse = new ResultBase<ValidateMultipleInvoiceResponse>()
            {
                Success = false,
                Message = MessageConstants.S_DATA_NOT_FOUND,
                Model = result
            };

            return errorResponse;
        }

        public async Task<List<BpkStatusResponse>> GetBpkStatusesAsync()
        {
            // Fetch data from the bpk_status table and map it to BpkStatusDto
            var result = await _bpkService.GetBpkStatus();
             return result;
        }
        public async Task<List<BpkMasterClearingStatusResponse>> GetMasterClearingStatusAsync()
        {
            // Fetch data from the bpk_status table and map it to BpkStatusDto
            var result = await _bpkService.GetMasterClearingStatus();
             return result;
        }
    }
}
