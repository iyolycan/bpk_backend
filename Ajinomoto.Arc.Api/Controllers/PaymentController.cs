using Ajinomoto.Arc.Api.Authorization;
using Ajinomoto.Arc.Business.Interfaces;
using Ajinomoto.Arc.Common.DtoModels;
using Ajinomoto.Arc.Common.Enums;
using Ajinomoto.Arc.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ajinomoto.Arc.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentFacade _paymentFacade;
        private readonly DbContextOptions<DataContext> _dbContextOptions;

        public PaymentController(
            IPaymentFacade paymentFacade,
            DbContextOptions<DataContext> dbContextOptions)
        {
            _paymentFacade = paymentFacade;
            _dbContextOptions = dbContextOptions;
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

        [Route("InboxList")]
        [HttpPost]
        public async Task<ActionResult> InboxList()
        {
            using (var context = new DataContext(_dbContextOptions))
            {
                var itemsRes = await context.InvoiceInbox.FromSqlRaw("SELECT * FROM invoice_inbox").ToListAsync();

                var response = new
                {
                    success = true,
                    message = "Successfully!",
                    model = new
                    {
                        currentPage = 1,
                        totalPages = 1,
                        pageSize = 10,
                        totalCount = itemsRes.Count,
                        hasPrevious = false,
                        hasNext = false,
                        filter = "",
                        currentSort = "DocDate",
                        sortDirection = "Descending",
                        items = itemsRes
                    }
                };

                return Ok(response);
            }
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
