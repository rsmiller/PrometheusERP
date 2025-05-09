﻿using Microsoft.EntityFrameworkCore;
using KosmosERP.Models.Permissions;
using KosmosERP.Database.Models;
using KosmosERP.Database;
using KosmosERP.Models.Helpers;
using KosmosERP.Models.Interfaces;
using KosmosERP.Models;
using KosmosERP.Module;
using KosmosERP.BusinessLayer.Models.Module.Contact.Command.Create;
using KosmosERP.BusinessLayer.Models.Module.Contact.Command.Delete;
using KosmosERP.BusinessLayer.Models.Module.Contact.Command.Edit;
using KosmosERP.BusinessLayer.Models.Module.Contact.Command.Find;
using KosmosERP.BusinessLayer.Models.Module.Contact.Dto;
using KosmosERP.BusinessLayer.Helpers;


namespace KosmosERP.BusinessLayer.Modules;

public interface IContactModule : IERPModule<
Contact,
ContactDto,
ContactListDto,
ContactCreateCommand,
ContactEditCommand,
ContactDeleteCommand,
ContactFindCommand>, IBaseERPModule
{

}

public class ContactModule : BaseERPModule, IContactModule
{
    public override Guid ModuleIdentifier => Guid.Parse("e89c86b7-44e8-4cac-aee3-3e7bcea845ef");
    public override string ModuleName => "Contacts";

    private readonly IBaseERPContext _Context;

    public ContactModule(IBaseERPContext context) : base(context)
    {
        _Context = context;
    }

    public override void SeedPermissions()
    {
        var role = _Context.Roles.Any(m => m.name == "Contact Users");
        var read_permission = _Context.ModulePermissions.Any(m => m.module_id == this.ModuleIdentifier.ToString() && m.internal_permission_name == ContactPermissions.Read);
        var create_permission = _Context.ModulePermissions.Any(m => m.module_id == this.ModuleIdentifier.ToString() && m.internal_permission_name == ContactPermissions.Create);
        var edit_permission = _Context.ModulePermissions.Any(m => m.module_id == this.ModuleIdentifier.ToString() && m.internal_permission_name == ContactPermissions.Edit);
        var delete_permission = _Context.ModulePermissions.Any(m => m.module_id == this.ModuleIdentifier.ToString() && m.internal_permission_name == ContactPermissions.Delete);

        if (role == false)
        {
            _Context.Roles.Add(CommonDataHelper<Role>.FillCommonFields(new Role()
            {
                name = "Contact Users",
            }, 1));

            _Context.SaveChanges();
        }

        var role_id = _Context.Roles.Where(m => m.name == "Contact Users").Select(m => m.id).Single();

        if (read_permission == false)
        {
            _Context.ModulePermissions.Add(new ModulePermission()
            {
                permission_name = "Read Contact",
                internal_permission_name = ContactPermissions.Read,
                module_id = this.ModuleIdentifier.ToString(),
                module_name = this.ModuleName,
                read = true,
            });

            _Context.SaveChanges();

            var read_perm_id = _Context.ModulePermissions.Where(m => m.internal_permission_name == ContactPermissions.Read).Select(m => m.id).Single();

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
                permission_name = "Create Contact",
                internal_permission_name = ContactPermissions.Create,
                module_id = this.ModuleIdentifier.ToString(),
                module_name = this.ModuleName,
                write = true
            });

            _Context.SaveChanges();

            var create_perm_id = _Context.ModulePermissions.Where(m => m.internal_permission_name == ContactPermissions.Create).Select(m => m.id).Single();

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
                permission_name = "Edit Contact",
                internal_permission_name = ContactPermissions.Edit,
                module_id = this.ModuleIdentifier.ToString(),
                module_name = this.ModuleName,
                edit = true
            });

            _Context.SaveChanges();

            var edit_perm_id = _Context.ModulePermissions.Where(m => m.internal_permission_name == ContactPermissions.Edit).Select(m => m.id).Single();

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
                permission_name = "Delete Contact",
                internal_permission_name = ContactPermissions.Delete,
                module_id = this.ModuleIdentifier.ToString(),
                module_name = this.ModuleName,
                delete = true
            });

            _Context.SaveChanges();

            var delete_perm_id = _Context.ModulePermissions.Where(m => m.internal_permission_name == ContactPermissions.Delete).Select(m => m.id).Single();

            _Context.RolePermissions.Add(CommonDataHelper<RolePermission>.FillCommonFields(new RolePermission()
            {
                role_id = role_id,
                module_permission_id = delete_perm_id,
            }, 1));

            _Context.SaveChanges();
        }
    }

    public Contact? Get(int object_id)
    {
        return _Context.Contacts
            .SingleOrDefault(m => m.id == object_id);
    }

    public async Task<Contact?> GetAsync(int object_id)
    {
        return await _Context.Contacts
            .SingleOrDefaultAsync(m => m.id == object_id);
    }

    public async Task<Response<ContactDto>> GetDto(int object_id)
    {
        var entity = await GetAsync(object_id);
        if (entity == null)
            return new Response<ContactDto>("Contact not found", ResultCode.NotFound);

        var dto = await MapToDto(entity);
        return new Response<ContactDto>(dto);
    }

    public async Task<Response<ContactDto>> Create(ContactCreateCommand commandModel)
    {
        var validationResult = ModelValidationHelper.ValidateModel(commandModel);
        if (!validationResult.Success)
            return new Response<ContactDto>(validationResult.Exception, ResultCode.DataValidationError);

        var newContact = this.MapForCreate(commandModel);

        _Context.Contacts.Add(newContact);
        await _Context.SaveChangesAsync();

        var dto = await MapToDto(newContact);
        return new Response<ContactDto>(dto);
    }

    public async Task<Response<ContactDto>> Edit(ContactEditCommand commandModel)
    {
        var validationResult = ModelValidationHelper.ValidateModel(commandModel);
        if (!validationResult.Success)
            return new Response<ContactDto>(validationResult.Exception, ResultCode.DataValidationError);

        var permission_result = await base.HasPermission(commandModel.calling_user_id, commandModel.token,ContactPermissions.Edit, edit: true);
        if (!permission_result)
            return new Response<ContactDto>("Invalid permission", ResultCode.InvalidPermission);


        var existingEntity = await GetAsync(commandModel.id);
        if (existingEntity == null)
            return new Response<ContactDto>("Contact not found", ResultCode.NotFound);

        if (commandModel.customer_id.HasValue && existingEntity.customer_id != commandModel.customer_id)
            existingEntity.customer_id = commandModel.customer_id.Value;

        if (existingEntity.first_name != commandModel.first_name)
            existingEntity.first_name = commandModel.first_name;

        if (existingEntity.last_name != commandModel.last_name)
            existingEntity.last_name = commandModel.last_name;

        if (existingEntity.title != commandModel.title)
            existingEntity.title = commandModel.title;

        if (existingEntity.email != commandModel.email)
            existingEntity.email = commandModel.email;

        if (existingEntity.phone != commandModel.phone)
            existingEntity.phone = commandModel.phone;

        if (existingEntity.cell_phone != commandModel.cell_phone)
            existingEntity.cell_phone = commandModel.cell_phone;

        // Update auditing fields
        existingEntity = CommonDataHelper<Contact>.FillUpdateFields(existingEntity, commandModel.calling_user_id);


        _Context.Contacts.Update(existingEntity);
        await _Context.SaveChangesAsync();

        var dto = await MapToDto(existingEntity);
        return new Response<ContactDto>(dto);
    }

    public async Task<Response<ContactDto>> Delete(ContactDeleteCommand commandModel)
    {
        var validationResult = ModelValidationHelper.ValidateModel(commandModel);
        if (!validationResult.Success)
            return new Response<ContactDto>(validationResult.Exception, ResultCode.DataValidationError);

        var permission_result = await base.HasPermission(commandModel.calling_user_id, commandModel.token,ContactPermissions.Delete, delete: true);
        if (!permission_result)
            return new Response<ContactDto>("Invalid permission", ResultCode.InvalidPermission);

        var existingEntity = await GetAsync(commandModel.id);
        if (existingEntity == null)
            return new Response<ContactDto>("Contact not found", ResultCode.NotFound);

        //  Soft delete
        existingEntity = CommonDataHelper<Contact>.FillDeleteFields(existingEntity, commandModel.calling_user_id);

        _Context.Contacts.Update(existingEntity);
        await _Context.SaveChangesAsync();

        var dto = await MapToDto(existingEntity);
        return new Response<ContactDto>(dto);
    }

    public async Task<PagingResult<ContactListDto>> Find(PagingSortingParameters parameters, ContactFindCommand commandModel)
    {
        var response = new PagingResult<ContactListDto>();
        try
        {
            var permission_result = await base.HasPermission(commandModel.calling_user_id, commandModel.token,ContactPermissions.Read, read: true);
            if (!permission_result)
            {
                response.SetException("Invalid permission", ResultCode.InvalidPermission);
                return response;
            }

            var query = _Context.Contacts
                .Where(m => !m.is_deleted);

            if (!string.IsNullOrEmpty(commandModel.wildcard))
            {
                var wild = commandModel.wildcard.ToLower();
                query = query.Where(m =>
                    m.first_name.ToLower().Contains(wild)
                    || m.last_name.ToLower().Contains(wild)
                    || (m.title != null && m.title.ToLower().Contains(wild))
                    || (m.email != null && m.email.ToLower().Contains(wild))
                    || (m.phone != null && m.phone.ToLower().Contains(wild))
                    || (m.cell_phone != null && m.cell_phone.ToLower().Contains(wild))
                    || m.guid.ToLower().Contains(wild)
                );
            }

            var totalCount = await query.CountAsync();
            var pagedItems = await query.SortAndPageBy(parameters).ToListAsync();

            var dtos = new List<ContactListDto>();
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

    public async Task<Response<List<ContactListDto>>> GlobalSearch(PagingSortingParameters parameters, string wildcard)
    {
        var response = new Response<List<ContactListDto>>();
        try
        {
            var query = _Context.Contacts
                .Where(m => !m.is_deleted);

            if (!string.IsNullOrEmpty(wildcard))
            {
                var lower = wildcard.ToLower();
                query = query.Where(m =>
                    m.first_name.ToLower().Contains(lower)
                    || m.last_name.ToLower().Contains(lower)
                    || (m.title != null && m.title.ToLower().Contains(lower))
                    || (m.email != null && m.email.ToLower().Contains(lower))
                    || (m.phone != null && m.phone.ToLower().Contains(lower))
                    || (m.cell_phone != null && m.cell_phone.ToLower().Contains(lower))
                    || m.guid.ToLower().Contains(lower)
                );
            }

            var pagedItems = await query.SortAndPageBy(parameters).ToListAsync();

            var dtos = new List<ContactListDto>();
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

    public async Task<ContactListDto> MapToListDto(Contact databaseModel)
    {
        return new ContactListDto
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
            customer_id = databaseModel.customer_id,
            first_name = databaseModel.first_name,
            last_name = databaseModel.last_name,
            title = databaseModel.title,
            email = databaseModel.email,
            phone = databaseModel.phone,
            cell_phone = databaseModel.cell_phone,
            guid = databaseModel.guid
        };
    }

    public async Task<ContactDto> MapToDto(Contact databaseModel)
    {
        return new ContactDto
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
            customer_id = databaseModel.customer_id,
            first_name = databaseModel.first_name,
            last_name = databaseModel.last_name,
            title = databaseModel.title,
            email = databaseModel.email,
            phone = databaseModel.phone,
            cell_phone = databaseModel.cell_phone,
            guid = databaseModel.guid
        };
    }

    public Contact MapToDatabaseModel(ContactDto dtoModel)
    {
        return new Contact
        {
            id = dtoModel.id,
            is_deleted = dtoModel.is_deleted,
            created_on = dtoModel.created_on,
            created_by = dtoModel.created_by,
            updated_on = dtoModel.updated_on,
            updated_by = dtoModel.updated_by,
            deleted_on = dtoModel.deleted_on,
            deleted_by = dtoModel.deleted_by,
            customer_id = dtoModel.customer_id,
            first_name = dtoModel.first_name,
            last_name = dtoModel.last_name,
            title = dtoModel.title,
            email = dtoModel.email,
            phone = dtoModel.phone,
            cell_phone = dtoModel.cell_phone,
            guid = dtoModel.guid
        };
    }

    private Contact MapForCreate(ContactCreateCommand createCommandModel)
    {
        var now = DateTime.UtcNow;

        var contact = CommonDataHelper<Contact>.FillCommonFields(new Contact
        {
            customer_id = createCommandModel.customer_id,
            first_name = createCommandModel.first_name,
            last_name = createCommandModel.last_name,
            title = createCommandModel.title,
            email = createCommandModel.email,
            phone = createCommandModel.phone,
            cell_phone = createCommandModel.cell_phone,
            is_deleted = false,
        }, createCommandModel.calling_user_id);

        return contact;
    }
}
