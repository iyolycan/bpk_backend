using Ajinomoto.Arc.Api.Authorization;
using Ajinomoto.Arc.Business.Interfaces;
using Ajinomoto.Arc.Common.DtoModels;
using Ajinomoto.Arc.Common.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Ajinomoto.Arc.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentFacade _paymentFacade;

        public PaymentController(IPaymentFacade paymentFacade)
        {
            _paymentFacade = paymentFacade;
        }

        [Route("IncomingPayments")]
        [HttpPost]
        public async Task<ActionResult> GetIncomingPaymentList(IncomingPaymentListRequest request)
        {
            var resVal = await _paymentFacade.GetIncomingPaymentList(request);

            return Ok(resVal);
        }

        [Route("InvoiceDetails")]
        [HttpPost]
        public async Task<ActionResult> GetInvoiceDetailsList(InvoiceDetailsRequest request)
        {
            var resVal = await _paymentFacade.GetInvoiceDetailsList(request);

            return Ok(resVal);
        }

        [Route("UpdateBaseLine")]
        [HttpPost]
        public async Task<ActionResult> UpdateBaseLine(UpdateBaseLineRequest request)
        {
            var resVal = await _paymentFacade.UpdateBaseLine(request);

            return Ok(resVal);
        }

        [Authorize((int)RoleEnum.Administrator, (int)RoleEnum.Coec)]
        [Route("ImportIncomingPayment")]
        [HttpPost]
        public async Task<ActionResult> ImportIncomingPayment([FromForm] ImportIncomingPaymentRequest request)
        {
            var resVal = await _paymentFacade.ImportIncomingPayment(request);

            return Ok(resVal);
        }

        [Authorize((int)RoleEnum.Administrator, (int)RoleEnum.Coec)]
        [Route("ImportInvoice")]
        [HttpPost]
        public async Task<ActionResult> ImportInvoice([FromForm] ImportInvoiceRequest request)
        {
            var resVal = await _paymentFacade.ImportInvoice(request);

            return Ok(resVal);
        }

        [Route("RemoveIncomingPayment")]
        [HttpPost]
        public async Task<ActionResult> RemoveIncomingPayment(Guid incomingPaymentId)
        {
            var resVal = await _paymentFacade.RemoveIncomingPayment(incomingPaymentId);

            return Ok(resVal);
        }

        [Route("RevertResettedBpkFromClearing")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> RevertResettedBpkFromClearing()
        {
            var resVal = await _paymentFacade.RevertResettedBpkFromClearing();

            return Ok(resVal);
        }


    }
}
