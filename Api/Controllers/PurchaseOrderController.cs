using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KosmosERP.BusinessLayer.Models.Module.PurchaseOrder.Dto;
using KosmosERP.BusinessLayer.Models.Module.PurchaseOrder.Command.Create;
using KosmosERP.BusinessLayer.Models.Module.PurchaseOrder.Command.Delete;
using KosmosERP.BusinessLayer.Models.Module.PurchaseOrder.Command.Find;
using KosmosERP.BusinessLayer.Modules;
using KosmosERP.Models;
using KosmosERP.Module;
using KosmosERP.BusinessLayer.Models.Module.User.ListProfiles;
using KosmosERP.BusinessLayer.Models.Module.PurchaseOrder.Command.Edit;

namespace KosmosERP.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class PurchaseOrderController : ERPApiController
{
    private IPurchaseOrderModule _Module;

    public PurchaseOrderController(IPurchaseOrderModule module) : base(module)
    {
        _Module = module;
    }

    
    [HttpGet("GetPurchaseOrderHeader", Name = "GetPurchaseOrderHeader")]
    [ProducesResponseType(typeof(Response<PurchaseOrderHeaderDto>), 200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult> Get([FromQuery] int id)
    {
        var result = await _Module.GetDto(id);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("GetPurchaseOrderLine", Name = "GetPurchaseOrderLine")]
    [ProducesResponseType(typeof(Response<PurchaseOrderLineDto>), 200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult> GetLine([FromQuery] int id)
    {
        var result = await _Module.GetLineDto(id);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("FindPurchaseOrderHeader", Name = "FindPurchaseOrderHeader")]
    [ProducesResponseType(typeof(PagingResult<PurchaseOrderHeaderListDto>), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult> Find([FromQuery] GeneralListProfile listProfile, [FromBody] PurchaseOrderHeaderFindCommand command)
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

    [HttpPost("CreatePurchaseOrderHeader", Name = "CreatePurchaseOrderHeader")]
    [ProducesResponseType(typeof(Response<PurchaseOrderHeaderDto>), 200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult> Create([FromBody] PurchaseOrderHeaderCreateCommand createCommand)
    {
        var result = await _Module.Create(createCommand);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("CreatePurchaseOrderLine", Name = "CreatePurchaseOrderLine")]
    [ProducesResponseType(typeof(Response<PurchaseOrderLineDto>), 200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult> CreateLine([FromBody] PurchaseOrderLineCreateCommand createCommand)
    {
        var result = await _Module.CreateLine(createCommand);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPut("UpdatePurchaseOrderHeader", Name = "UpdatePurchaseOrderHeader")]
    [ProducesResponseType(typeof(Response<PurchaseOrderHeaderDto>), 200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult> Edit([FromBody] PurchaseOrderHeaderEditCommand editCommand)
    {
        var result = await _Module.Edit(editCommand);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPut("UpdatePurchaseOrderLine", Name = "UpdatePurchaseOrderLine")]
    [ProducesResponseType(typeof(Response<PurchaseOrderLineDto>), 200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult> EditLine([FromBody] PurchaseOrderLineEditCommand editCommand)
    {
        var result = await _Module.EditLine(editCommand);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("DeletePurchaseOrderHeader", Name = "DeletePurchaseOrderHeader")]
    [ProducesResponseType(typeof(Response<PurchaseOrderHeaderDto>), 200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult> Delete([FromBody] PurchaseOrderHeaderDeleteCommand deleteCommand)
    {
        var result = await _Module.Delete(deleteCommand);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("DeletePurchaseOrderLine", Name = "DeletePurchaseOrderLine")]
    [ProducesResponseType(typeof(Response<PurchaseOrderHeaderDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> DeleteLine([FromBody] PurchaseOrderLineDeleteCommand deleteCommand)
    {
        var result = await _Module.DeleteLine(deleteCommand);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}
