﻿using KosmosERP.Database.Models;
using KosmosERP.Database;
using KosmosERP.Models.Helpers;
using KosmosERP.Models.Interfaces;
using KosmosERP.Models;
using KosmosERP.Module;
using Microsoft.EntityFrameworkCore;
using KosmosERP.BusinessLayer.Models.Module.Opportunity.Dto;
using KosmosERP.BusinessLayer.Models.Module.Opportunity.Command.Create;
using KosmosERP.BusinessLayer.Models.Module.Opportunity.Command.Edit;
using KosmosERP.BusinessLayer.Models.Module.Opportunity.Command.Delete;
using KosmosERP.BusinessLayer.Models.Module.Opportunity.Command.Find;
using KosmosERP.Models.Permissions;
using KosmosERP.BusinessLayer.Helpers;

namespace KosmosERP.BusinessLayer.Modules;

public interface IOpportunityModule : IERPModule<
    Opportunity,
    OpportunityDto,
    OpportunityListDto,
    OpportunityCreateCommand,
    OpportunityEditCommand,
    OpportunityDeleteCommand,
    OpportunityFindCommand>, IBaseERPModule
{

}

public class OpportunityModule : BaseERPModule, IOpportunityModule
{
    public override Guid ModuleIdentifier => Guid.Parse("0c3959c3-15dc-44ab-8e2c-9b9e2773e65f");
    public override string ModuleName => "Opportunities";

    private readonly IBaseERPContext _Context;

    public OpportunityModule(IBaseERPContext context) : base(context)
    {
        _Context = context;
    }

    public override void SeedPermissions()
    {
        var role = _Context.Roles.Any(m => m.name == "CRM Users");
        var read_permission = _Context.ModulePermissions.Any(m => m.module_id == this.ModuleIdentifier.ToString() && m.internal_permission_name == OpportunityPermissions.Read);
        var create_permission = _Context.ModulePermissions.Any(m => m.module_id == this.ModuleIdentifier.ToString() && m.internal_permission_name == OpportunityPermissions.Create);
        var edit_permission = _Context.ModulePermissions.Any(m => m.module_id == this.ModuleIdentifier.ToString() && m.internal_permission_name == OpportunityPermissions.Edit);
        var delete_permission = _Context.ModulePermissions.Any(m => m.module_id == this.ModuleIdentifier.ToString() && m.internal_permission_name == OpportunityPermissions.Delete);

        if (role == false)
        {
            _Context.Roles.Add(CommonDataHelper<Role>.FillCommonFields(new Role()
            {
                name = "CRM Users",
            }, 1));

            _Context.SaveChanges();
        }

        var role_id = _Context.Roles.Where(m => m.name == "CRM Users").Select(m => m.id).Single();

        if (read_permission == false)
        {
            _Context.ModulePermissions.Add(new ModulePermission()
            {
                permission_name = "Read Opportunity",
                internal_permission_name = OpportunityPermissions.Read,
                module_id = this.ModuleIdentifier.ToString(),
                module_name = this.ModuleName,
                read = true,
            });

            _Context.SaveChanges();

            var read_perm_id = _Context.ModulePermissions.Where(m => m.internal_permission_name == OpportunityPermissions.Read).Select(m => m.id).Single();

            _Context.RolePermissions.Add(CommonDataHelper<RolePermission>.FillCommonFields(new RolePermission()
            {
                role_id = role_id,
                module_permission_id = read_perm_id,
            }, 1));

            _Context.SaveChanges();
        }

        if (create_permission == false)
        {
            _Context.ModulePermissions.Add(new ModulePermission()
            {
                permission_name = "Create Opportunity",
                internal_permission_name = OpportunityPermissions.Create,
                module_id = this.ModuleIdentifier.ToString(),
                module_name = this.ModuleName,
                write = true
            });

            _Context.SaveChanges();

            var create_perm_id = _Context.ModulePermissions.Where(m => m.internal_permission_name == OpportunityPermissions.Create).Select(m => m.id).Single();

            _Context.RolePermissions.Add(CommonDataHelper<RolePermission>.FillCommonFields(new RolePermission()
            {
                role_id = role_id,
                module_permission_id = create_perm_id,
            }, 1));

            _Context.SaveChanges();
        }

        if (edit_permission == false)
        {
            _Context.ModulePermissions.Add(new ModulePermission()
            {
                permission_name = "Edit Opportunity",
                internal_permission_name = OpportunityPermissions.Edit,
                module_id = this.ModuleIdentifier.ToString(),
                module_name = this.ModuleName,
                edit = true
            });

            _Context.SaveChanges();

            var edit_perm_id = _Context.ModulePermissions.Where(m => m.internal_permission_name == OpportunityPermissions.Edit).Select(m => m.id).Single();

            _Context.RolePermissions.Add(CommonDataHelper<RolePermission>.FillCommonFields(new RolePermission()
            {
                role_id = role_id,
                module_permission_id = edit_perm_id,
            }, 1));

            _Context.SaveChanges();
        }

        if (delete_permission == false)
        {
            _Context.ModulePermissions.Add(new ModulePermission()
            {
                permission_name = "Delete Opportunity",
                internal_permission_name = OpportunityPermissions.Delete,
                module_id = this.ModuleIdentifier.ToString(),
                module_name = this.ModuleName,
                delete = true
            });

            _Context.SaveChanges();

            var delete_perm_id = _Context.ModulePermissions.Where(m => m.internal_permission_name == OpportunityPermissions.Delete).Select(m => m.id).Single();

            _Context.RolePermissions.Add(CommonDataHelper<RolePermission>.FillCommonFields(new RolePermission()
            {
                role_id = role_id,
                module_permission_id = delete_perm_id,
            }, 1));

            _Context.SaveChanges();
        }


        var prospecting_stage = _Context.KeyValueStores.Where(m => m.module_id == this.ModuleIdentifier.ToString() 
                                    && m.key == "opporunity_stage_prospecting").SingleOrDefault();
        var qualifying_stage = _Context.KeyValueStores.Where(m => m.module_id == this.ModuleIdentifier.ToString() 
                                    && m.key == "opporunity_stage_qualifying").SingleOrDefault();
        var analysis_stage = _Context.KeyValueStores.Where(m => m.module_id == this.ModuleIdentifier.ToString() 
                                    && m.key == "opporunity_stage_analysis").SingleOrDefault();
        var proposition_stage = _Context.KeyValueStores.Where(m => m.module_id == this.ModuleIdentifier.ToString() 
                                    && m.key == "opporunity_stage_proposition").SingleOrDefault();
        var proposal_stage = _Context.KeyValueStores.Where(m => m.module_id == this.ModuleIdentifier.ToString() 
                                    && m.key == "opporunity_stage_proposal").SingleOrDefault();
        var negotiation_stage = _Context.KeyValueStores.Where(m => m.module_id == this.ModuleIdentifier.ToString() 
                                    && m.key == "opporunity_stage_negotiation").SingleOrDefault();
        var closed_won_stage = _Context.KeyValueStores.Where(m => m.module_id == this.ModuleIdentifier.ToString()
                                    && m.key == "opporunity_stage_closed_won").SingleOrDefault();
        var closed_lost_stage = _Context.KeyValueStores.Where(m => m.module_id == this.ModuleIdentifier.ToString()
                                    && m.key == "opporunity_stage_closed_lost").SingleOrDefault();

        if (prospecting_stage == null)
        {
            _Context.KeyValueStores.Add(CommonDataHelper<KeyValueStore>.FillCommonFields(new KeyValueStore()
            {
                key = "opporunity_stage_prospecting",
                value = "Prospecting",
            }, 1));

            _Context.SaveChanges();
        }

        if (qualifying_stage == null)
        {
            _Context.KeyValueStores.Add(CommonDataHelper<KeyValueStore>.FillCommonFields(new KeyValueStore()
            {
                key = "opporunity_stage_qualifying",
                value = "Qualifying",
            }, 1));

            _Context.SaveChanges();
        }

        if (analysis_stage == null)
        {
            _Context.KeyValueStores.Add(CommonDataHelper<KeyValueStore>.FillCommonFields(new KeyValueStore()
            {
                key = "opporunity_stage_analysis",
                value = "Analysis",
            }, 1));

            _Context.SaveChanges();
        }

        if (proposition_stage == null)
        {
            _Context.KeyValueStores.Add(CommonDataHelper<KeyValueStore>.FillCommonFields(new KeyValueStore()
            {
                key = "opporunity_stage_proposition",
                value = "Proposition",
            }, 1));

            _Context.SaveChanges();
        }

        if (proposal_stage == null)
        {
            _Context.KeyValueStores.Add(CommonDataHelper<KeyValueStore>.FillCommonFields(new KeyValueStore()
            {
                key = "opporunity_stage_proposal",
                value = "Proposal",
            }, 1));

            _Context.SaveChanges();
        }

        if (closed_won_stage == null)
        {
            _Context.KeyValueStores.Add(CommonDataHelper<KeyValueStore>.FillCommonFields(new KeyValueStore()
            {
                key = "opporunity_stage_closed_won",
                value = "Closed Won",
            }, 1));

            _Context.SaveChanges();
        }

        if (closed_lost_stage == null)
        {
            _Context.KeyValueStores.Add(CommonDataHelper<KeyValueStore>.FillCommonFields(new KeyValueStore()
            {
                key = "opporunity_stage_closed_lost",
                value = "Closed Lost",
            }, 1));

            _Context.SaveChanges();
        }
    }

    public Opportunity? Get(int object_id)
    {
        return _Context.Opportunities
            .SingleOrDefault(m => m.id == object_id);
    }

    public async Task<Opportunity?> GetAsync(int object_id)
    {
        return await _Context.Opportunities
            .SingleOrDefaultAsync(m => m.id == object_id);
    }

    public async Task<Response<OpportunityDto>> GetDto(int object_id)
    {
        var entity = await GetAsync(object_id);
        if (entity == null)
            return new Response<OpportunityDto>("Opportunity not found", ResultCode.NotFound);

        var dto = await MapToDto(entity);
        return new Response<OpportunityDto>(dto);
    }

    public async Task<Response<OpportunityDto>> Create(OpportunityCreateCommand commandModel)
    {
        var validationResult = ModelValidationHelper.ValidateModel(commandModel);
        if (!validationResult.Success)
            return new Response<OpportunityDto>(validationResult.Exception, ResultCode.DataValidationError);

        var permission_result = await base.HasPermission(commandModel.calling_user_id, commandModel.token,OpportunityPermissions.Create, write: true);
        if (!permission_result)
            return new Response<OpportunityDto>("Invalid permission", ResultCode.InvalidPermission);

        var record = this.MapToDatabaseModel(commandModel);
        record.owner_id = commandModel.calling_user_id;

        _Context.Opportunities.Add(record);
        await _Context.SaveChangesAsync();

        var dto = await MapToDto(record);
        return new Response<OpportunityDto>(dto);
    }

    public async Task<Response<OpportunityDto>> Edit(OpportunityEditCommand commandModel)
    {
        var validationResult = ModelValidationHelper.ValidateModel(commandModel);
        if (!validationResult.Success)
            return new Response<OpportunityDto>(validationResult.Exception, ResultCode.DataValidationError);

        var permission_result = await base.HasPermission(commandModel.calling_user_id, commandModel.token,OpportunityPermissions.Edit, edit: true);
        if (!permission_result)
            return new Response<OpportunityDto>("Invalid permission", ResultCode.InvalidPermission);

        var existingEntity = await GetAsync(commandModel.id);
        if (existingEntity == null)
            return new Response<OpportunityDto>("Opportunity not found", ResultCode.NotFound);

        if (!string.IsNullOrEmpty(commandModel.opportunity_name)
            && existingEntity.opportunity_name != commandModel.opportunity_name)
        {
            existingEntity.opportunity_name = commandModel.opportunity_name;
        }

        if (commandModel.customer_id.HasValue && existingEntity.customer_id != commandModel.customer_id)
            existingEntity.customer_id = commandModel.customer_id.Value;

        if (commandModel.contact_id.HasValue && existingEntity.contact_id != commandModel.contact_id)
            existingEntity.contact_id = commandModel.contact_id.Value;

        if (commandModel.amount.HasValue && existingEntity.amount != commandModel.amount)
            existingEntity.amount = commandModel.amount.Value;

        if (!string.IsNullOrEmpty(commandModel.stage)
            && existingEntity.stage != commandModel.stage)
        {
            existingEntity.stage = commandModel.stage;
        }

        if (commandModel.win_chance.HasValue && existingEntity.win_chance != commandModel.win_chance)
            existingEntity.win_chance = commandModel.win_chance.Value;

        if (commandModel.expected_close.HasValue && existingEntity.expected_close != commandModel.expected_close)
            existingEntity.expected_close = commandModel.expected_close.Value;

        if (commandModel.owner_id.HasValue && existingEntity.owner_id != commandModel.owner_id)
            existingEntity.owner_id = commandModel.owner_id.Value;

        existingEntity = CommonDataHelper<Opportunity>.FillUpdateFields(existingEntity, commandModel.calling_user_id);


        _Context.Opportunities.Update(existingEntity);
        await _Context.SaveChangesAsync();

        var dto = await MapToDto(existingEntity);
        return new Response<OpportunityDto>(dto);
    }

    public async Task<Response<OpportunityDto>> Delete(OpportunityDeleteCommand commandModel)
    {
        var permission_result = await base.HasPermission(commandModel.calling_user_id, commandModel.token,OpportunityPermissions.Delete, delete: true);
        if (!permission_result)
            return new Response<OpportunityDto>("Invalid permission", ResultCode.InvalidPermission);

        var existingEntity = await GetAsync(commandModel.id);
        if (existingEntity == null)
            return new Response<OpportunityDto>("Opportunity not found", ResultCode.NotFound);


        // Do delete
        existingEntity = CommonDataHelper<Opportunity>.FillDeleteFields(existingEntity, commandModel.calling_user_id);


        _Context.Opportunities.Update(existingEntity);
        await _Context.SaveChangesAsync();

        var dto = await MapToDto(existingEntity);
        return new Response<OpportunityDto>(dto);
    }

    public async Task<PagingResult<OpportunityListDto>> Find(PagingSortingParameters parameters, OpportunityFindCommand commandModel)
    {
        var response = new PagingResult<OpportunityListDto>();

        try
        {
            // Example permission check (could be read_opportunity, etc.)
            var permission_result = await base.HasPermission(commandModel.calling_user_id, commandModel.token,OpportunityPermissions.Read, read: true);
            if (!permission_result)
            {
                response.SetException("Invalid permission", ResultCode.InvalidPermission);
                return response;
            }

            var query = _Context.Opportunities
                .Where(m => m.is_deleted == false);

            // If wildcard is not empty, filter by string fields
            if (!string.IsNullOrEmpty(commandModel.wildcard))
            {
                var wild = commandModel.wildcard.ToLower();
                query = query.Where(m =>
                    (m.opportunity_name.ToLower().Contains(wild))
                    || (m.stage.ToLower().Contains(wild))
                    || (m.guid.ToLower().Contains(wild))
                );
            }

            // Sort and page
            var totalCount = await query.CountAsync();
            var pagedItems = await query.SortAndPageBy(parameters).ToListAsync();

            // Convert to DTO
            var dtos = new List<OpportunityListDto>();
            foreach (var item in pagedItems)
            {
                dtos.Add(await MapToListDto(item));
            }

            response.Data = dtos;
            response.TotalResultCount = totalCount;
        }
        catch (Exception ex)
        {
            await LogError(50, this.GetType().Name, nameof(Find), ex);
            response.SetException(ex.Message, ResultCode.Error);
            response.TotalResultCount = 0;
        }

        return response;
    }

    public async Task<Response<List<OpportunityListDto>>> GlobalSearch(PagingSortingParameters parameters, string wildcard)
    {
        // Simple example
        var response = new Response<List<OpportunityListDto>>();
        try
        {
            // If you do permission checks here, implement similarly

            var query = _Context.Opportunities
                .Where(m => !m.is_deleted);

            if (!string.IsNullOrEmpty(wildcard))
            {
                var lower = wildcard.ToLower();
                query = query.Where(m =>
                    m.opportunity_name.ToLower().Contains(lower)
                    || m.stage.ToLower().Contains(lower)
                    || m.guid.ToLower().Contains(lower));
            }

            var pagedItems = await query.SortAndPageBy(parameters).ToListAsync();
            var dtos = new List<OpportunityListDto>();
            foreach (var item in pagedItems)
            {
                dtos.Add(await MapToListDto(item));
            }

            response.Data = dtos;
        }
        catch (Exception ex)
        {
            await LogError(50, this.GetType().Name, nameof(GlobalSearch), ex);
            response.SetException(ex.Message, ResultCode.Error);
        }

        return response;
    }

    public async Task<OpportunityListDto> MapToListDto(Opportunity databaseModel)
    {
        return new OpportunityListDto
        {
            id = databaseModel.id,
            is_deleted = databaseModel.is_deleted,
            created_on = databaseModel.created_on,
            created_by = databaseModel.created_by,
            updated_on = databaseModel.updated_on,
            updated_by = databaseModel.updated_by,
            deleted_on = databaseModel.deleted_on,
            deleted_by = databaseModel.deleted_by,
            created_on_string = databaseModel.created_on_string,
            created_on_timezone = databaseModel.created_on_timezone,
            updated_on_string = databaseModel.updated_on_string,
            updated_on_timezone = databaseModel.updated_on_timezone,
            deleted_on_string = databaseModel.deleted_on_string,
            deleted_on_timezone = databaseModel.deleted_on_timezone,
            opportunity_name = databaseModel.opportunity_name,
            customer_id = databaseModel.customer_id,
            contact_id = databaseModel.contact_id,
            amount = databaseModel.amount,
            stage = databaseModel.stage,
            win_chance = databaseModel.win_chance,
            expected_close = databaseModel.expected_close,
            owner_id = databaseModel.owner_id,
            guid = databaseModel.guid
        };
    }

    public async Task<OpportunityDto> MapToDto(Opportunity databaseModel)
    {
        return new OpportunityDto
        {
            id = databaseModel.id,
            is_deleted = databaseModel.is_deleted,
            created_on = databaseModel.created_on,
            created_by = databaseModel.created_by,
            updated_on = databaseModel.updated_on,
            updated_by = databaseModel.updated_by,
            deleted_on = databaseModel.deleted_on,
            deleted_by = databaseModel.deleted_by,
            created_on_string = databaseModel.created_on_string,
            created_on_timezone = databaseModel.created_on_timezone,
            updated_on_string = databaseModel.updated_on_string,
            updated_on_timezone = databaseModel.updated_on_timezone,
            deleted_on_string = databaseModel.deleted_on_string,
            deleted_on_timezone = databaseModel.deleted_on_timezone,
            opportunity_name = databaseModel.opportunity_name,
            customer_id = databaseModel.customer_id,
            contact_id = databaseModel.contact_id,
            amount = databaseModel.amount,
            stage = databaseModel.stage,
            win_chance = databaseModel.win_chance,
            expected_close = databaseModel.expected_close,
            owner_id = databaseModel.owner_id,
            guid = databaseModel.guid
        };
    }

    public Opportunity MapToDatabaseModel(OpportunityDto dtoModel)
    {
        return new Opportunity
        {
            id = dtoModel.id,
            opportunity_name = dtoModel.opportunity_name,
            customer_id = dtoModel.customer_id,
            contact_id = dtoModel.contact_id,
            amount = dtoModel.amount,
            stage = dtoModel.stage,
            win_chance = dtoModel.win_chance,
            expected_close = dtoModel.expected_close,
            owner_id = dtoModel.owner_id,
        };
    }

    public Opportunity MapToDatabaseModel(OpportunityCreateCommand createCommand)
    {
        return CommonDataHelper<Opportunity>.FillCommonFields(new Opportunity
        {
            opportunity_name = createCommand.opportunity_name,
            customer_id = createCommand.customer_id,
            contact_id = createCommand.contact_id,
            amount = createCommand.amount,
            stage = createCommand.stage,
            win_chance = createCommand.win_chance,
            expected_close = createCommand.expected_close,
        }, createCommand.calling_user_id);
        
    }
}
