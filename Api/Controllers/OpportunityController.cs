using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KosmosERP.BusinessLayer.Models.Module.Opportunity.Dto;
using KosmosERP.BusinessLayer.Models.Module.Opportunity.Command.Create;
using KosmosERP.BusinessLayer.Models.Module.Opportunity.Command.Delete;
using KosmosERP.BusinessLayer.Models.Module.Opportunity.Command.Edit;
using KosmosERP.BusinessLayer.Models.Module.Opportunity.Command.Find;
using KosmosERP.BusinessLayer.Modules;
using KosmosERP.Models;
using KosmosERP.Module;
using KosmosERP.BusinessLayer.Models.Module.User.ListProfiles;

namespace KosmosERP.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class OpportunityController : ERPApiController
{
    private IOpportunityModule _Module;

    public OpportunityController(IOpportunityModule module) : base(module)
    {
        _Module = module;
    }

    
    [HttpGet("GetOpportunity", Name = "GetOpportunity")]
    [ProducesResponseType(typeof(Response<OpportunityDto>), 200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult> Get([FromQuery] int id)
    {
        var result = await _Module.GetDto(id);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("FindOpportunity", Name = "FindOpportunity")]
    [ProducesResponseType(typeof(PagingResult<OpportunityListDto>), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult> Find([FromQuery] GeneralListProfile listProfile, [FromBody] OpportunityFindCommand command)
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

    [HttpPost("CreateOpportunity", Name = "CreateOpportunity")]
    [ProducesResponseType(typeof(Response<OpportunityDto>), 200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult> Create([FromBody] OpportunityCreateCommand createCommand)
    {
        var result = await _Module.Create(createCommand);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPut("UpdateOpportunity", Name = "UpdateOpportunity")]
    [ProducesResponseType(typeof(Response<OpportunityDto>), 200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult> Edit([FromBody] OpportunityEditCommand editCommand)
    {
        var result = await _Module.Edit(editCommand);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("DeleteOpportunity", Name = "DeleteOpportunity")]
    [ProducesResponseType(typeof(Response<OpportunityDto>), 200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult> Delete([FromBody] OpportunityDeleteCommand deleteCommand)
    {
        var result = await _Module.Delete(deleteCommand);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}
