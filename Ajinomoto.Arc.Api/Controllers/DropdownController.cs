using Ajinomoto.Arc.Api.Authorization;
using Ajinomoto.Arc.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Ajinomoto.Arc.Api.Controllers
{
    [Route("[controller]")]
    [Authorize]
    [ApiController]
    public class DropdownController : ControllerBase
    {
        private readonly IDropdownFacade _dropdownFacade;

        public DropdownController(IDropdownFacade dropdownFacade)
        {
            _dropdownFacade = dropdownFacade;
        }

        [AllowAnonymous]
        [Route("Area")]
        [HttpGet]
        public async Task<ActionResult> GetDdlArea(string? filter)
        {
            var resVal = await _dropdownFacade.GetDdlArea(filter);

            return Ok(resVal);
        }

        [Route("Branch")]
        [HttpGet]
        public async Task<ActionResult> GetDdlBranch()
        {
            var resVal = await _dropdownFacade.GetDdlBranch();

            return Ok(resVal);
        }

        [AllowAnonymous]
        [Route("Customer")]
        [HttpGet]
        public async Task<ActionResult> GetDdlCustomer(string? filter)
        {
            var resVal = await _dropdownFacade.GetDdlCustomer(filter);

            return Ok(resVal);
        }

        [Route("Customer-v2")]
        [HttpGet]
        public async Task<ActionResult> GetDdlCustomer()
        {
            var resVal = await _dropdownFacade.GetDdlCustomer();

            return Ok(resVal);
        }

        [Route("DataLevel")]
        [HttpGet]
        public async Task<ActionResult> GetDdlDataLevel()
        {
            var resVal = await _dropdownFacade.GetDdlDataLevel();

            return Ok(resVal);
        }

        [Route("Invoice")]
        [HttpGet]
        public async Task<ActionResult> GetDdlInvoiceByCustomer(string? customerCode)
        {
            var resVal = await _dropdownFacade.GetDdlInvoiceByCustomer(customerCode);

            return Ok(resVal);
        }

        [Route("Invoice-v2")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> GetDdlInvoice(string? filter)
        {
            var resVal = await _dropdownFacade.GetDdlInvoice(filter);

            return Ok(resVal);
        }

        [Route("PotonganType")]
        [HttpGet]
        public async Task<ActionResult> GetDdlPotonganType()
        {
            var resVal = await _dropdownFacade.GetDdlPotonganType();

            return Ok(resVal);
        }

        [Route("KpiProperty")]
        [HttpGet]
        public async Task<ActionResult> GetDdlKpiProperty()
        {
            var resVal = await _dropdownFacade.GetDdlKpiProperty();

            return Ok(resVal);
        }

        [Route("KpiPropertyCurrent")]
        [HttpGet]
        public async Task<ActionResult> GetDdlKpiPropertyCurrent()
        {
            var resVal = await _dropdownFacade.GetDdlKpiPropertyCurrent();

            return Ok(resVal);
        }

        [Route("KpiPropertyTotal")]
        [HttpGet]
        public async Task<ActionResult> GetDdlKpiPropertyTotal()
        {
            var resVal = await _dropdownFacade.GetDdlKpiPropertyTotal();

            return Ok(resVal);
        }

        [Route("Role")]
        [HttpGet]
        public async Task<ActionResult> GetDdlRole()
        {
            var resVal = await _dropdownFacade.GetDdlRole();

            return Ok(resVal);
        }

        [Route("Source")]
        [HttpGet]
        public async Task<ActionResult> GetDdlSource()
        {
            var resVal = await _dropdownFacade.GetDdlSource();

            return Ok(resVal);
        }

        [Route("Segment")]
        [HttpGet]
        public async Task<ActionResult> GetDdlSegment()
        {
            var resVal = await _dropdownFacade.GetDdlSegment();

            return Ok(resVal);
        }

        [Route("TemplateUploadType")]
        [HttpGet]
        public async Task<ActionResult> GetDdlTemplateUploadType()
        {
            var resVal = await _dropdownFacade.GetDdlTemplateUploadType();

            return Ok(resVal);
        }

        [Route("User")]
        [HttpGet]
        public async Task<ActionResult> GetDdlUser()
        {
            var resVal = await _dropdownFacade.GetDdlUser();

            return Ok(resVal);
        }

        [Route("UserCoec")]
        [HttpGet]
        public async Task<ActionResult> GetDdlUserCoec()
        {
            var resVal = await _dropdownFacade.GetDdlUserCoec();

            return Ok(resVal);
        }
    }
}
