using Ajinomoto.Arc.Api.Authorization;
using Ajinomoto.Arc.Business.Interfaces;
using Ajinomoto.Arc.Common.DtoModels;
using Ajinomoto.Arc.Common.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Ajinomoto.Arc.Api.Controllers
{
    [Route("[controller]")]
    [Authorize]
    [ApiController]
    public class BpkController : ControllerBase
    {
        private readonly IBpkFacade _bpkFacade;

        public BpkController(IBpkFacade bpkFacade)
        {
            _bpkFacade = bpkFacade;
        }

        [Route("SaveBpk")]
        [HttpPost]
        public async Task<ActionResult> SaveBpk(SaveBpkRequest param)
        {
            var resVal = await _bpkFacade.SaveBpk(param);

            return Ok(resVal);
        }

        [Route("GetByIncomingPayment")]
        [HttpPost]
        public async Task<ActionResult> GetByIncomingPayment(BpkRequest param)
        {
            var resVal = await _bpkFacade.GetBpk(param.IncomingPaymentId);

            return Ok(resVal);
        }

        [Route("SubmitBpk")]
        [HttpPost]
        public async Task<ActionResult> SubmitBpk(SubmitBpkRequest param)
        {
            var resVal = await _bpkFacade.SubmitBpk(param);

            return Ok(resVal);
        }

        [Route("OpenClearingBpk")]
        [HttpPost]
        [Authorize((int)RoleEnum.Coec)]
        public async Task<ActionResult> OpenClearingBpk(Guid incomingPaymentId)
        {
            var resVal = await _bpkFacade.OpenClearingBpk(incomingPaymentId);

            return Ok(resVal);
        }

        [Route("RequestForRevisionBpk")]
        [HttpPost]
        public async Task<ActionResult> RequestForRevisionBpk(Guid incomingPaymentId)
        {
            var resVal = await _bpkFacade.RequestForRevisionBpk(incomingPaymentId);

            return Ok(resVal);
        }

        [Route("ReviseApproveBpk")]
        [HttpPost]
        [Authorize((int)RoleEnum.Coec)]
        public async Task<ActionResult> ReviseApproveBpk(Guid incomingPaymentId)
        {
            var resVal = await _bpkFacade.ReviseApproveBpk(incomingPaymentId);

            return Ok(resVal);
        }

        [Route("ReviseRejectBpk")]
        [HttpPost]
        [Authorize((int)RoleEnum.Coec)]
        public async Task<ActionResult> ReviseRejectBpk(RejectBpkRequest param)
        {
            var resVal = await _bpkFacade.ReviseRejectBpk(param);

            return Ok(resVal);
        }

        [Route("GetInvoice")]
        [HttpGet]
        public async Task<ActionResult> GetInvoice(string invoiceNumber)
        {
            var resVal = await _bpkFacade.GetInvoice(invoiceNumber);

            return Ok(resVal);
        }

        [Route("ValidateMultipleInvoices")]
        [HttpPost]
        public async Task<ActionResult> ValidateMultipleInvoices(MultipleInvoiceRequest request)
        {
            var resVal = await _bpkFacade.ValidateMultipleInvoices(request.Invoices);

            return Ok(resVal);
        }

        [AllowAnonymous]
        [HttpGet("SendReminder")]
        public async Task<ActionResult> SendReminder(string executeBy, string executeApp)
        {
            var resVal = await _bpkFacade.SendReminder(executeBy, executeApp);

            return Ok(resVal);
        }

        // New Route: Get Master BPK Status List
        [Route("getMasterBpkStatus")]
        [HttpGet]
        public async Task<ActionResult> GetMasterBpkStatus()
        {
            var statuses = await _bpkFacade.GetBpkStatusesAsync();
            return Ok(statuses);
        }

        // New Route: Get Master Clearing Status List
        [Route("getMasterClearingStatus")]
        [HttpGet]
        public async Task<ActionResult> GetMasterClearingStatus()
        {
            var statuses = await _bpkFacade.GetMasterClearingStatusAsync();
            return Ok(statuses);
        }
    }
}
