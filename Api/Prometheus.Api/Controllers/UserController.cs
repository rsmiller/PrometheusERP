using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prometheus.Api.Models.Module.User.Command.Create;
using Prometheus.Api.Models.Module.User.Command.Delete;
using Prometheus.Api.Models.Module.User.Command.Edit;
using Prometheus.Api.Models.Module.User.Command.Find;
using Prometheus.Api.Models.Module.User.Dto;
using Prometheus.Api.Models.Module.User.ListProfiles;
using Prometheus.Api.Modules;
using Prometheus.Models;
using Prometheus.Module;

namespace Prometheus.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserController : ERPApiController
    {
        private IUserModule _Module;

        public UserController(IUserModule userModule) : base(userModule)
        {
            _Module = userModule;
        }

        
        [HttpGet("GetUser", Name = "GetUser")]
        [ProducesResponseType(typeof(Response<UserDto>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        public async Task<ActionResult> Get([FromQuery] int id)
        {
            var result = await _Module.GetDto(id);

            return new JsonResult(result);
        }

        [HttpGet("GetUserBySessionId", Name = "GetUserBySessionId")]
        [ProducesResponseType(typeof(Response<UserDto>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        public async Task<ActionResult> GetUserBySessionId([FromQuery] string session_id)
        {
            var result = await _Module.GetBySession(session_id);

            return new JsonResult(result);
        }

        [HttpGet("AuthenticateUser", Name = "AuthenticateUser")]
        [ProducesResponseType(typeof(Response<AuthenticatedUserDto>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        public async Task<ActionResult> Authenticate([FromQuery] AuthenticateListProfile listProfile)
        {
            var result = await _Module.Authenticate(listProfile.username, listProfile.password);

            if (!result.Success)
                return BadRequest(result);

            return new JsonResult(result);
        }

        [HttpPost("FindUser", Name = "FindUser")]
        [ProducesResponseType(typeof(PagingResult<UserListDto>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> Find([FromQuery] GeneralListProfile listProfile, [FromBody] UserFindCommand command)
        {
            try
            {
                if (command != null)
                {
                    var sortingParams = new PagingSortingParameters(listProfile.Start, listProfile.ResultCount, listProfile.SortOrder);

                    var result = await _Module.Find(sortingParams, command);

                    return Ok(result);
                }
                else
                {
                    return StatusCode(500, "Api body is null");
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost("CreateUser", Name = "CreateUser")]
        [ProducesResponseType(typeof(Response<UserDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> Create([FromBody] UserCreateCommand createCommand)
        {
            var result = await _Module.Create(createCommand);

            if (!result.Success)
                return BadRequest(result);

            return new JsonResult(result);
        }

        [HttpPut("UpdateUser", Name = "UpdateUser")]
        [ProducesResponseType(typeof(Response<UserDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> Edit([FromBody] UserEditCommand editCommand)
        {
            var result = await _Module.Edit(editCommand);

            if (!result.Success)
                return BadRequest(result);

            return new JsonResult(result);
        }

        [HttpGet("GetUsersByDepartment", Name = "GetUsersByDepartment")]
        [ProducesResponseType(typeof(Response<UserDto>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        public async Task<ActionResult> GetUsersByDepartmentName([FromQuery] int department_id)
        {
            var result = await _Module.GetUsersByDepartment(department_id);

            return new JsonResult(result);
        }


        [HttpDelete("DeleteUser", Name = "DeleteUser")]
        [ProducesResponseType(typeof(Response<UserDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> Delete([FromBody] UserDeleteCommand deleteCommand)
        {
            var result = await _Module.Delete(deleteCommand);

            if (!result.Success)
                return BadRequest(result);

            return new JsonResult(result);
        }
    }
}
