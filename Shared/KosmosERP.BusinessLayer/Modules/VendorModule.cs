﻿using KosmosERP.Database.Models;
using KosmosERP.Database;
using KosmosERP.Models.Helpers;
using KosmosERP.Models.Interfaces;
using KosmosERP.Models;
using KosmosERP.Module;
using Microsoft.EntityFrameworkCore;
using KosmosERP.BusinessLayer.Models.Module.Vendor.Command.Create;
using KosmosERP.BusinessLayer.Models.Module.Vendor.Command.Delete;
using KosmosERP.BusinessLayer.Models.Module.Vendor.Command.Edit;
using KosmosERP.BusinessLayer.Models.Module.Vendor.Command.Find;
using KosmosERP.BusinessLayer.Models.Module.Vendor.Dto;
using KosmosERP.Models.Permissions;
using KosmosERP.BusinessLayer.Helpers;

namespace KosmosERP.BusinessLayer.Modules;

public interface IVendorModule : IERPModule<
Vendor,
VendorDto,
VendorListDto,
VendorCreateCommand,
VendorEditCommand,
VendorDeleteCommand,
VendorFindCommand>, IBaseERPModule
{

}

public class VendorModule : BaseERPModule, IVendorModule
{
    public override Guid ModuleIdentifier => Guid.Parse("dae2593c-678b-4f6d-9c84-f4f74e066428");
    public override string ModuleName => "Vendors";

    private readonly IBaseERPContext _Context;
    private readonly IAddressModule _AddressModule;

    public VendorModule(IBaseERPContext context, IAddressModule address_module) : base(context)
    {
        _Context = context;
        _AddressModule = address_module;
    }

    public override void SeedPermissions()
    {
        var role = _Context.Roles.Any(m => m.name == "Vendor Users");
        var read_permission = _Context.ModulePermissions.Any(m => m.module_id == this.ModuleIdentifier.ToString() && m.internal_permission_name == VendorPermissions.Read);
        var create_permission = _Context.ModulePermissions.Any(m => m.module_id == this.ModuleIdentifier.ToString() && m.internal_permission_name == VendorPermissions.Create);
        var edit_permission = _Context.ModulePermissions.Any(m => m.module_id == this.ModuleIdentifier.ToString() && m.internal_permission_name == VendorPermissions.Edit);
        var delete_permission = _Context.ModulePermissions.Any(m => m.module_id == this.ModuleIdentifier.ToString() && m.internal_permission_name == VendorPermissions.Delete);

        if (role == false)
        {
            _Context.Roles.Add(new Role()
            {
                name = "Vendor Users",
                created_by = 1,
                created_on = DateTime.UtcNow,
                updated_by = 1,
                updated_on = DateTime.UtcNow,
            });

            _Context.SaveChanges();
        }

        var role_id = _Context.Roles.Where(m => m.name == "Vendor Users").Select(m => m.id).Single();

        if (read_permission == false)
        {
            _Context.ModulePermissions.Add(new ModulePermission()
            {
                permission_name = "Read Vendor",
                internal_permission_name = VendorPermissions.Read,
                module_id = this.ModuleIdentifier.ToString(),
                module_name = this.ModuleName,
                read = true,
            });

            _Context.SaveChanges();

            var read_perm_id = _Context.ModulePermissions.Where(m => m.internal_permission_name == VendorPermissions.Read).Select(m => m.id).Single();

            _Context.RolePermissions.Add(new RolePermission()
            {
                role_id = role_id,
                module_permission_id = read_perm_id,
                created_by = 1,
                created_on = DateTime.UtcNow,
                updated_by = 1,
                updated_on = DateTime.UtcNow,
            });

            _Context.SaveChanges();
        }

        if (create_permission == false)
        {
            _Context.ModulePermissions.Add(new ModulePermission()
            {
                permission_name = "Create Vendor",
                internal_permission_name = VendorPermissions.Create,
                module_id = this.ModuleIdentifier.ToString(),
                module_name = this.ModuleName,
                write = true
            });

            _Context.SaveChanges();

            var create_perm_id = _Context.ModulePermissions.Where(m => m.internal_permission_name == VendorPermissions.Create).Select(m => m.id).Single();

            _Context.RolePermissions.Add(new RolePermission()
            {
                role_id = role_id,
                module_permission_id = create_perm_id,
                created_by = 1,
                created_on = DateTime.UtcNow,
                updated_by = 1,
                updated_on = DateTime.UtcNow,
            });

            _Context.SaveChanges();
        }

        if (edit_permission == false)
        {
            _Context.ModulePermissions.Add(new ModulePermission()
            {
                permission_name = "Edit Vendor",
                internal_permission_name = VendorPermissions.Edit,
                module_id = this.ModuleIdentifier.ToString(),
                module_name = this.ModuleName,
                edit = true
            });

            _Context.SaveChanges();

            var edit_perm_id = _Context.ModulePermissions.Where(m => m.internal_permission_name == VendorPermissions.Edit).Select(m => m.id).Single();

            _Context.RolePermissions.Add(new RolePermission()
            {
                role_id = role_id,
                module_permission_id = edit_perm_id,
                created_by = 1,
                created_on = DateTime.UtcNow,
                updated_by = 1,
                updated_on = DateTime.UtcNow,
            });

            _Context.SaveChanges();
        }

        if (delete_permission == false)
        {
            _Context.ModulePermissions.Add(new ModulePermission()
            {
                permission_name = "Delete Vendor",
                internal_permission_name = VendorPermissions.Delete,
                module_id = this.ModuleIdentifier.ToString(),
                module_name = this.ModuleName,
                delete = true
            });

            _Context.SaveChanges();

            var delete_perm_id = _Context.ModulePermissions.Where(m => m.internal_permission_name == VendorPermissions.Delete).Select(m => m.id).Single();

            _Context.RolePermissions.Add(new RolePermission()
            {
                role_id = role_id,
                module_permission_id = delete_perm_id,
                created_by = 1,
                created_on = DateTime.UtcNow,
                updated_by = 1,
                updated_on = DateTime.UtcNow,
            });

            _Context.SaveChanges();
        }
    }

    public Vendor? Get(int object_id)
    {
        return _Context.Vendors.SingleOrDefault(m => m.id == object_id);
    }

    public async Task<Vendor?> GetAsync(int object_id)
    {
        return await _Context.Vendors.SingleOrDefaultAsync(m => m.id == object_id);
    }

    public async Task<Response<VendorDto>> GetDto(int object_id)
    {
        var entity = await GetAsync(object_id);
        if (entity == null)
            return new Response<VendorDto>("Vendor not found", ResultCode.NotFound);

        var dto = await MapToDto(entity);
        return new Response<VendorDto>(dto);
    }

    public async Task<Response<VendorDto>> Create(VendorCreateCommand commandModel)
    {
        var validationResult = ModelValidationHelper.ValidateModel(commandModel);
        if (!validationResult.Success)
            return new Response<VendorDto>(validationResult.Exception, ResultCode.DataValidationError);

        var vendor_permission_result = await base.HasPermission(commandModel.calling_user_id, commandModel.token,VendorPermissions.Create, write: true);
        if (!vendor_permission_result)
            return new Response<VendorDto>("Invalid vendor permission", ResultCode.InvalidPermission);

        var address_permission_result = await base.HasPermission(commandModel.calling_user_id, commandModel.token,AddressPermissions.Create, write: true);
        if (!address_permission_result)
            return new Response<VendorDto>("Invalid address permission", ResultCode.InvalidPermission);


        commandModel.address.calling_user_id = commandModel.calling_user_id;

        var address_response = await _AddressModule.Create(commandModel.address);

        if(!address_response.Success || address_response.Data == null)
            return new Response<VendorDto>(address_response.Exception, ResultCode.Error);


        var newVendor = this.MapForCreate(commandModel);
        newVendor.address_id = address_response.Data.id;


        _Context.Vendors.Add(newVendor);
        await _Context.SaveChangesAsync();

        var dto = await MapToDto(newVendor);
        return new Response<VendorDto>(dto);
    }

    public async Task<Response<VendorDto>> Edit(VendorEditCommand commandModel)
    {
        var validationResult = ModelValidationHelper.ValidateModel(commandModel);
        if (!validationResult.Success)
            return new Response<VendorDto>(validationResult.Exception, ResultCode.DataValidationError);

        var permission_result = await base.HasPermission(commandModel.calling_user_id, commandModel.token,VendorPermissions.Edit, edit: true);
        if (!permission_result)
            return new Response<VendorDto>("Invalid permission", ResultCode.InvalidPermission);

        var existingEntity = await GetAsync(commandModel.id);
        if (existingEntity == null)
            return new Response<VendorDto>("Vendor not found", ResultCode.NotFound);

        if (existingEntity.vendor_name != commandModel.vendor_name)
            existingEntity.vendor_name = commandModel.vendor_name;

        if (existingEntity.vendor_description != commandModel.vendor_description)
            existingEntity.vendor_description = commandModel.vendor_description;

        if (existingEntity.phone != commandModel.phone)
            existingEntity.phone = commandModel.phone;

        if (commandModel.address_id.HasValue && existingEntity.address_id != commandModel.address_id)
            existingEntity.address_id = commandModel.address_id.Value;

        if (existingEntity.fax != commandModel.fax)
            existingEntity.fax = commandModel.fax;

        if (existingEntity.general_email != commandModel.general_email)
            existingEntity.general_email = commandModel.general_email;

        if (existingEntity.website != commandModel.website)
            existingEntity.website = commandModel.website;

        if (existingEntity.category != commandModel.category)
            existingEntity.category = commandModel.category;

        if (commandModel.is_critial_vendor.HasValue && existingEntity.is_critial_vendor != commandModel.is_critial_vendor)
            existingEntity.is_critial_vendor = commandModel.is_critial_vendor.HasValue;

        // Additional vendor-specific fields
        if (existingEntity.approved_on != commandModel.approved_on)
            existingEntity.approved_on = commandModel.approved_on;

        if (existingEntity.approved_by != commandModel.approved_by)
            existingEntity.approved_by = commandModel.approved_by;

        if (existingEntity.audit_on != commandModel.audit_on)
            existingEntity.audit_on = commandModel.audit_on;

        if (existingEntity.audit_by != commandModel.audit_by)
            existingEntity.audit_by = commandModel.audit_by;

        if (existingEntity.retired_on != commandModel.retired_on)
            existingEntity.retired_on = commandModel.retired_on;

        if (existingEntity.retired_by != commandModel.retired_by)
            existingEntity.retired_by = commandModel.retired_by;


        existingEntity = CommonDataHelper<Vendor>.FillUpdateFields(existingEntity, commandModel.calling_user_id);


        _Context.Vendors.Update(existingEntity);
        await _Context.SaveChangesAsync();

        var dto = await MapToDto(existingEntity);
        return new Response<VendorDto>(dto);
    }

    public async Task<Response<VendorDto>> Delete(VendorDeleteCommand commandModel)
    {
        var validationResult = ModelValidationHelper.ValidateModel(commandModel);
        if (!validationResult.Success)
            return new Response<VendorDto>(validationResult.Exception, ResultCode.DataValidationError);

        var permission_result = await base.HasPermission(commandModel.calling_user_id, commandModel.token,VendorPermissions.Delete, delete: true);
        if (!permission_result)
            return new Response<VendorDto>("Invalid permission", ResultCode.InvalidPermission);

        var existingEntity = await GetAsync(commandModel.id);
        if (existingEntity == null)
            return new Response<VendorDto>("Vendor not found", ResultCode.NotFound);

        // Delete
        existingEntity = CommonDataHelper<Vendor>.FillDeleteFields(existingEntity, commandModel.calling_user_id);

        _Context.Vendors.Update(existingEntity);
        await _Context.SaveChangesAsync();

        var dto = await MapToDto(existingEntity);
        return new Response<VendorDto>(dto);
    }

    public async Task<PagingResult<VendorListDto>> Find(PagingSortingParameters parameters, VendorFindCommand commandModel)
    {
        var response = new PagingResult<VendorListDto>();
        try
        {
            var permission_result = await base.HasPermission(commandModel.calling_user_id, commandModel.token,VendorPermissions.Read, read: true);
            if (!permission_result)
            {
                response.SetException("Invalid permission", ResultCode.InvalidPermission);
                return response;
            }

            var query = _Context.Vendors
                .Where(m => !m.is_deleted);

            if (!string.IsNullOrEmpty(commandModel.wildcard))
            {
                var wild = commandModel.wildcard.ToLower();
                query = query.Where(m =>
                    m.vendor_name.ToLower().Contains(wild)
                    || (m.vendor_description != null && m.vendor_description.ToLower().Contains(wild))
                    || m.phone.ToLower().Contains(wild)
                    || (m.fax != null && m.fax.ToLower().Contains(wild))
                    || (m.general_email != null && m.general_email.ToLower().Contains(wild))
                    || (m.website != null && m.website.ToLower().Contains(wild))
                    || m.category.ToLower().Contains(wild)
                    || m.guid.ToLower().Contains(wild)
                );
            }

            var totalCount = await query.CountAsync();
            var pagedItems = await query.SortAndPageBy(parameters).ToListAsync();

            var dtos = new List<VendorListDto>();
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

    public async Task<Response<List<VendorListDto>>> GlobalSearch(PagingSortingParameters parameters, string wildcard)
    {
        var response = new Response<List<VendorListDto>>();
        try
        {
            var query = _Context.Vendors
                .Where(m => !m.is_deleted);

            if (!string.IsNullOrEmpty(wildcard))
            {
                var lower = wildcard.ToLower();
                query = query.Where(m =>
                    m.vendor_name.ToLower().Contains(lower)
                    || (m.vendor_description != null && m.vendor_description.ToLower().Contains(lower))
                    || m.phone.ToLower().Contains(lower)
                    || (m.fax != null && m.fax.ToLower().Contains(lower))
                    || (m.general_email != null && m.general_email.ToLower().Contains(lower))
                    || (m.website != null && m.website.ToLower().Contains(lower))
                    || m.category.ToLower().Contains(lower)
                    || m.guid.ToLower().Contains(lower)
                );
            }

            var pagedItems = await query.SortAndPageBy(parameters).ToListAsync();
            var dtos = new List<VendorListDto>();
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

    public async Task<VendorListDto> MapToListDto(Vendor databaseModel)
    {
        var address = await _Context.Addresses.SingleAsync(m => m.id == databaseModel.address_id);
        var address_dto = await _AddressModule.MapToDto(address);

        return new VendorListDto
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
            vendor_number = databaseModel.vendor_number,
            vendor_name = databaseModel.vendor_name,
            vendor_description = databaseModel.vendor_description,
            phone = databaseModel.phone,
            fax = databaseModel.fax,
            general_email = databaseModel.general_email,
            website = databaseModel.website,
            category = databaseModel.category,
            is_critial_vendor = databaseModel.is_critial_vendor,
            guid = databaseModel.guid,
            approved_on = databaseModel.approved_on,
            approved_by = databaseModel.approved_by,
            audit_on = databaseModel.audit_on,
            audit_by = databaseModel.audit_by,
            retired_on = databaseModel.retired_on,
            retired_by = databaseModel.retired_by,
            address_id = databaseModel.address_id,
            address = address_dto
        };
    }

    public async Task<VendorDto> MapToDto(Vendor databaseModel)
    {
        var address = await _Context.Addresses.SingleAsync(m => m.id == databaseModel.address_id);
        var address_dto = await _AddressModule.MapToDto(address);

        return new VendorDto
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
            vendor_number = databaseModel.vendor_number,
            vendor_name = databaseModel.vendor_name,
            vendor_description = databaseModel.vendor_description,
            phone = databaseModel.phone,
            fax = databaseModel.fax,
            general_email = databaseModel.general_email,
            website = databaseModel.website,
            category = databaseModel.category,
            is_critial_vendor = databaseModel.is_critial_vendor,
            guid = databaseModel.guid,
            approved_on = databaseModel.approved_on,
            approved_by = databaseModel.approved_by,
            audit_on = databaseModel.audit_on,
            audit_by = databaseModel.audit_by,
            retired_on = databaseModel.retired_on,
            retired_by = databaseModel.retired_by,
            address_id = databaseModel.address_id,
            address = address_dto
        };
    }

    public Vendor MapToDatabaseModel(VendorDto dtoModel)
    {
        return new Vendor
        {
            id = dtoModel.id,
            is_deleted = dtoModel.is_deleted,
            created_on = dtoModel.created_on,
            created_by = dtoModel.created_by,
            updated_on = dtoModel.updated_on,
            updated_by = dtoModel.updated_by,
            deleted_on = dtoModel.deleted_on,
            deleted_by = dtoModel.deleted_by,
            vendor_number = dtoModel.vendor_number,
            vendor_name = dtoModel.vendor_name,
            vendor_description = dtoModel.vendor_description,
            address_id = dtoModel.address_id,
            phone = dtoModel.phone,
            fax = dtoModel.fax,
            general_email = dtoModel.general_email,
            website = dtoModel.website,
            category = dtoModel.category,
            is_critial_vendor = dtoModel.is_critial_vendor,
            guid = dtoModel.guid,
            approved_on = dtoModel.approved_on,
            approved_by = dtoModel.approved_by,
            audit_on = dtoModel.audit_on,
            audit_by = dtoModel.audit_by,
            retired_on = dtoModel.retired_on,
            retired_by = dtoModel.retired_by
        };
    }

    private Vendor MapForCreate(VendorCreateCommand createCommandModel)
    {
        var now = DateTime.UtcNow;

        var vendor = CommonDataHelper<Vendor>.FillCommonFields(new Vendor
        {
            vendor_name = createCommandModel.vendor_name,
            vendor_description = createCommandModel.vendor_description,
            phone = createCommandModel.phone,
            fax = createCommandModel.fax,
            general_email = createCommandModel.general_email,
            website = createCommandModel.website,
            category = createCommandModel.category,
            is_critial_vendor = createCommandModel.is_critial_vendor,
            approved_on = createCommandModel.approved_on,
            approved_by = createCommandModel.approved_by,
            audit_on = createCommandModel.audit_on,
            audit_by = createCommandModel.audit_by,
            retired_on = createCommandModel.retired_on,
            retired_by = createCommandModel.retired_by,
            is_deleted = false,
        }, createCommandModel.calling_user_id);

        return vendor;
    }
}
