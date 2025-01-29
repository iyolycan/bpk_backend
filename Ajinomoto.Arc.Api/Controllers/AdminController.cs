using Ajinomoto.Arc.Api.Authorization;
using Ajinomoto.Arc.Business.Interfaces;
using Ajinomoto.Arc.Common.DtoModels;
using Ajinomoto.Arc.Common.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Ajinomoto.Arc.Api.Controllers
{
    [Route("[controller]")]
    [Authorize((int)RoleEnum.Administrator)]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminFacade _adminFacade;

        public AdminController(IAdminFacade adminFacade)
        {
            _adminFacade = adminFacade;
        }

        [Route("GetArea")]
        [HttpGet]
        public async Task<ActionResult> GetArea(int areaId)
        {
            var resVal = await _adminFacade.GetArea(areaId);

            return Ok(resVal);
        }

        [Route("GetAreaList")]
        [HttpPost]
        public async Task<ActionResult> GetAreaList(BasicListRequest request)
        {
            var resVal = await _adminFacade.GetAreaList(request);

            return Ok(resVal);
        }

        [Route("SaveArea")]
        [HttpPost]
        public async Task<ActionResult> SaveArea(SaveAreaRequest request)
        {
            var resVal = await _adminFacade.SaveArea(request);

            return Ok(resVal);
        }

        [Route("RemoveArea")]
        [HttpGet]
        public async Task<ActionResult> RemoveArea(int areaId)
        {
            var resVal = await _adminFacade.RemoveArea(areaId);

            return Ok(resVal);
        }

        [Route("GetBranch")]
        [HttpGet]
        public async Task<ActionResult> GetBranch(int branchId)
        {
            var resVal = await _adminFacade.GetBranch(branchId);

            return Ok(resVal);
        }

        [Route("GetBranchList")]
        [HttpPost]
        public async Task<ActionResult> GetBranchList(BasicListRequest request)
        {
            var resVal = await _adminFacade.GetBranchList(request);

            return Ok(resVal);
        }

        [Route("SaveBranch")]
        [HttpPost]
        public async Task<ActionResult> SaveBranch(SaveBranchRequest request)
        {
            var resVal = await _adminFacade.SaveBranch(request);

            return Ok(resVal);
        }

        [Route("RemoveBranch")]
        [HttpGet]
        public async Task<ActionResult> RemoveBranch(int branchId)
        {
            var resVal = await _adminFacade.RemoveBranch(branchId);

            return Ok(resVal);
        }

        [Route("GetRole")]
        [HttpGet]
        public async Task<ActionResult> GetRole(int roleId)
        {
            var resVal = await _adminFacade.GetRole(roleId);

            return Ok(resVal);
        }

        [Route("GetRoleList")]
        [HttpPost]
        public async Task<ActionResult> GetRoleList(BasicListRequest request)
        {
            var resVal = await _adminFacade.GetRoleList(request);

            return Ok(resVal);
        }

        [Route("SaveRole")]
        [HttpPost]
        public async Task<ActionResult> SaveRole(SaveRoleRequest request)
        {
            var resVal = await _adminFacade.SaveRole(request);

            return Ok(resVal);
        }

        [Route("RemoveRole")]
        [HttpGet]
        public async Task<ActionResult> RemoveRole(int roleId)
        {
            var resVal = await _adminFacade.RemoveRole(roleId);

            return Ok(resVal);
        }

        [Route("GetPotonganType")]
        [HttpGet]
        public async Task<ActionResult> GetPotonganType(int PotonganTypeId)
        {
            var resVal = await _adminFacade.GetPotonganType(PotonganTypeId);

            return Ok(resVal);
        }

        [Route("GetPotonganTypeList")]
        [HttpPost]
        public async Task<ActionResult> GetPotonganTypeList(BasicListRequest request)
        {
            var resVal = await _adminFacade.GetPotonganTypeList(request);

            return Ok(resVal);
        }

        [Route("SavePotonganType")]
        [HttpPost]
        public async Task<ActionResult> SavePotonganType(SavePotonganTypeRequest request)
        {
            var resVal = await _adminFacade.SavePotonganType(request);

            return Ok(resVal);
        }

        [Route("RemovePotonganType")]
        [HttpGet]
        public async Task<ActionResult> RemovePotonganType(int PotonganTypeId)
        {
            var resVal = await _adminFacade.RemovePotonganType(PotonganTypeId);

            return Ok(resVal);
        }

        [Route("GetSegment")]
        [HttpGet]
        public async Task<ActionResult> GetSegment(int segmentId)
        {
            var resVal = await _adminFacade.GetSegment(segmentId);

            return Ok(resVal);
        }

        [Route("GetSegmentList")]
        [HttpPost]
        public async Task<ActionResult> GetSegmentList(BasicListRequest request)
        {
            var resVal = await _adminFacade.GetSegmentList(request);

            return Ok(resVal);
        }

        [Route("SaveSegment")]
        [HttpPost]
        public async Task<ActionResult> SaveSegment(SaveSegmentRequest request)
        {
            var resVal = await _adminFacade.SaveSegment(request);

            return Ok(resVal);
        }

        [Route("RemoveSegment")]
        [HttpGet]
        public async Task<ActionResult> RemoveSegment(int segmentId)
        {
            var resVal = await _adminFacade.RemoveSegment(segmentId);

            return Ok(resVal);
        }

        [Route("GetSource")]
        [HttpGet]
        public async Task<ActionResult> GetSource(int sourceId)
        {
            var resVal = await _adminFacade.GetSource(sourceId);

            return Ok(resVal);
        }

        [Route("GetSourceList")]
        [HttpPost]
        public async Task<ActionResult> GetSourceList(BasicListRequest request)
        {
            var resVal = await _adminFacade.GetSourceList(request);

            return Ok(resVal);
        }

        [Route("SaveSource")]
        [HttpPost]
        public async Task<ActionResult> SaveSource(SaveSourceRequest request)
        {
            var resVal = await _adminFacade.SaveSource(request);

            return Ok(resVal);
        }

        [Route("RemoveSource")]
        [HttpGet]
        public async Task<ActionResult> RemoveSource(int sourceId)
        {
            var resVal = await _adminFacade.RemoveSource(sourceId);

            return Ok(resVal);
        }

        [Route("GetUser")]
        [HttpGet]
        public async Task<ActionResult> GetUser(int userId)
        {
            var resVal = await _adminFacade.GetUser(userId);

            return Ok(resVal);
        }

        [Route("GetUserSetupByRole")]
        [HttpGet]
        public async Task<ActionResult> GetUserSetupByRole(int roleId)
        {
            var resVal = await _adminFacade.GetUserSetupByRole(roleId);

            return Ok(resVal);
        }

        [Route("GetUserList")]
        [HttpPost]
        public async Task<ActionResult> GetUserList(BasicListRequest request)
        {
            var resVal = await _adminFacade.GetUserList(request);

            return Ok(resVal);
        }

        [Route("SaveUser")]
        [HttpPost]
        public async Task<ActionResult> SaveUser(SaveUserRequest request)
        {
            var resVal = await _adminFacade.SaveUser(request);

            return Ok(resVal);
        }

        [Route("EmailTesting")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> EmailTesting()
        {
            var resVal = await _adminFacade.EmailTesting();

            return Ok(resVal);
        }
    }
}
