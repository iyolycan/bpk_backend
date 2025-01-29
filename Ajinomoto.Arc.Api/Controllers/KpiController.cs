using Ajinomoto.Arc.Api.Authorization;
using Ajinomoto.Arc.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Ajinomoto.Arc.Api.Controllers
{
    [Route("[controller]")]
    [Authorize]
    [ApiController]
    public class KpiController : ControllerBase
    {
        private readonly IKpiFacade _kpiFacade;

        public KpiController(IKpiFacade kpiFacade)
        {
            _kpiFacade = kpiFacade;
        }

        [Route("GetData")]
        [HttpPost]
        public async Task<ActionResult> GetData(string period)
        {
            var resVal = await _kpiFacade.GetKpiData(period);

            return Ok(resVal);
        }
    }
}
