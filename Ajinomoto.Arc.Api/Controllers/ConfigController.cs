using Ajinomoto.Arc.Api.Authorization;
using Ajinomoto.Arc.Business.Interfaces;
using Ajinomoto.Arc.Common.DtoModels;
using Ajinomoto.Arc.Common.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Ajinomoto.Arc.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize((int)RoleEnum.Administrator)]
    public class ConfigController : ControllerBase
    {
        private readonly IConfigFacade _configFacade;

        public ConfigController(IConfigFacade configFacade)
        {
            _configFacade = configFacade;
        }

        [Route("GetGeneral")]
        [HttpGet]
        public async Task<ActionResult> GetGeneral()
        {
            var resVal = await _configFacade.GetAppConfig();

            return Ok(resVal);
        }

        [Route("UpdateGeneral")]
        [HttpPost]
        public async Task<ActionResult> UpdateGeneral(UpdateAppConfigRequest request)
        {
            var resVal = await _configFacade.UpdateAppConfig(request);

            return Ok(resVal);
        }

        [Route("GetArCutOff")]
        [HttpGet]
        public async Task<ActionResult> GetArCutOff(string yearPeriod)
        {
            var resVal = await _configFacade.GetIncomingPaymentCutOff(yearPeriod);

            return Ok(resVal);
        }

        [Route("CreateArCutOff")]
        [HttpPost]
        public async Task<ActionResult> CreateArCutOff(string yearPeriod)
        {
            var resVal = await _configFacade.CreateIncomingPaymentCutOff(yearPeriod);

            return Ok(resVal);
        }

        [Route("UpdateArCutOff")]
        [HttpPost]
        public async Task<ActionResult> UpdateArCutOff(ConfigRequest configRequest)
        {
            var resVal = await _configFacade.UpdateIncomingPaymentCutOff(configRequest);

            return Ok(resVal);
        }
    }
}
