using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prometheus.Api.Models.Module.Product.Dto;
using Prometheus.Api.Models.Module.Product.Command.Create;
using Prometheus.Api.Models.Module.Product.Command.Delete;
using Prometheus.Api.Models.Module.Product.Command.Edit;
using Prometheus.Api.Models.Module.Product.Command.Find;
using Prometheus.Api.Modules;
using Prometheus.Models;
using Prometheus.Module;
using Prometheus.Api.Models.Module.User.ListProfiles;

namespace Prometheus.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ProductController : ERPApiController
    {
        private IProductModule _Module;

        public ProductController(IProductModule module) : base(module)
        {
            _Module = module;
        }

        
        [HttpGet("GetProduct", Name = "GetProduct")]
        [ProducesResponseType(typeof(Response<ProductDto>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        public async Task<ActionResult> Get([FromQuery] int id)
        {
            var result = await _Module.GetDto(id);

            return new JsonResult(result);
        }

        [HttpPost("FindProduct", Name = "FindProduct")]
        [ProducesResponseType(typeof(PagingResult<ProductListDto>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> Find([FromQuery] GeneralListProfile listProfile, [FromBody] ProductFindCommand command)
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

        [HttpPost("CreateProduct", Name = "CreateProduct")]
        [ProducesResponseType(typeof(Response<ProductDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> Create([FromBody] ProductCreateCommand createCommand)
        {
            var result = await _Module.Create(createCommand);

            if (!result.Success)
                return BadRequest(result);

            return new JsonResult(result);
        }

        [HttpPut("UpdateProduct", Name = "UpdateProduct")]
        [ProducesResponseType(typeof(Response<ProductDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> Edit([FromBody] ProductEditCommand editCommand)
        {
            var result = await _Module.Edit(editCommand);

            if (!result.Success)
                return BadRequest(result);

            return new JsonResult(result);
        }

        [HttpDelete("DeleteProduct", Name = "DeleteProduct")]
        [ProducesResponseType(typeof(Response<ProductDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> Delete([FromBody] ProductDeleteCommand deleteCommand)
        {
            var result = await _Module.Delete(deleteCommand);

            if (!result.Success)
                return BadRequest(result);

            return new JsonResult(result);
        }
    }
}
