﻿using Prometheus.Database;
using Prometheus.Database.Models;
using Prometheus.Models;
using Prometheus.Models.Interfaces;
using Prometheus.Module;
using Prometheus.BusinessLayer.Models.Module.DocumentUpload.Command.Create;
using Prometheus.BusinessLayer.Models.Module.DocumentUpload.Command.Delete;
using Prometheus.BusinessLayer.Models.Module.DocumentUpload.Command.Edit;
using Prometheus.BusinessLayer.Models.Module.DocumentUpload.Command.Find;
using Prometheus.BusinessLayer.Models.Module.DocumentUpload.Dto;
using Microsoft.AspNetCore.Http;
using Prometheus.Models.Permissions;

namespace Prometheus.BusinessLayer.Modules;

public interface IDocumentUploadModule : IERPModule<DocumentUpload, DocumentUploadDto, DocumentUploadListDto, DocumentUploadCreateCommand, DocumentUploadEditCommand, DocumentUploadDeleteCommand, DocumentUploadFindCommand>, IBaseERPModule
{
	Task<Response<DocumentUploadDto>> CreateOverride(IFormFile file, DocumentUploadCreateCommand commandModel);
    
}

public class DocumentUploadModule : BaseERPModule, IDocumentUploadModule
{
	public override Guid ModuleIdentifier => Guid.Parse("4b0ce064-9c4b-4e39-8812-79cc3f69e945");
	public override string ModuleName => "DocumentUpload";

	private IBaseERPContext _Context;
	private IFileStorageSettings? _StorageSettings;

    public DocumentUploadModule(IBaseERPContext context) : base(context)
    {
        _Context = context;
    }

    public DocumentUploadModule(IBaseERPContext context, IFileStorageSettings storageSettings) : base(context)
    {
		_Context = context;
        _StorageSettings = storageSettings;

    }

	public override void SeedPermissions()
	{
        var role = _Context.Roles.Any(m => m.name == "Document Users");
        var read_permission = _Context.ModulePermissions.Any(m => m.module_id == this.ModuleIdentifier.ToString() && m.internal_permission_name == DocumentPermissions.Read);
        var create_permission = _Context.ModulePermissions.Any(m => m.module_id == this.ModuleIdentifier.ToString() && m.internal_permission_name == DocumentPermissions.Create);
        var edit_permission = _Context.ModulePermissions.Any(m => m.module_id == this.ModuleIdentifier.ToString() && m.internal_permission_name == DocumentPermissions.Edit);
        var delete_permission = _Context.ModulePermissions.Any(m => m.module_id == this.ModuleIdentifier.ToString() && m.internal_permission_name == DocumentPermissions.Delete);

        if (role == false)
        {
            _Context.Roles.Add(new Role()
            {
                name = "Document Users",
                created_by = 1,
                created_on = DateTime.UtcNow,
                updated_by = 1,
                updated_on = DateTime.UtcNow,
            });

            _Context.SaveChanges();
        }

        var role_id = _Context.Roles.Where(m => m.name == "Customer Users").Select(m => m.id).Single();

        if (read_permission == false)
        {
            _Context.ModulePermissions.Add(new ModulePermission()
            {
                permission_name = "Read Document",
                internal_permission_name = DocumentPermissions.Read,
                module_id = this.ModuleIdentifier.ToString(),
                module_name = this.ModuleName,
                read = true,
            });

            _Context.SaveChanges();

            var read_perm_id = _Context.ModulePermissions.Where(m => m.internal_permission_name == DocumentPermissions.Read).Select(m => m.id).Single();

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
                permission_name = "Create Document",
                internal_permission_name = DocumentPermissions.Create,
                module_id = this.ModuleIdentifier.ToString(),
                module_name = this.ModuleName,
                write = true
            });

            _Context.SaveChanges();

            var create_perm_id = _Context.ModulePermissions.Where(m => m.internal_permission_name == DocumentPermissions.Create).Select(m => m.id).Single();

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
                permission_name = "Edit Document",
                internal_permission_name = DocumentPermissions.Delete,
                module_id = this.ModuleIdentifier.ToString(),
                module_name = this.ModuleName,
                edit = true
            });

            _Context.SaveChanges();

            var edit_perm_id = _Context.ModulePermissions.Where(m => m.internal_permission_name == DocumentPermissions.Edit).Select(m => m.id).Single();

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
                permission_name = "Delete Document",
                internal_permission_name = DocumentPermissions.Delete,
                module_id = this.ModuleIdentifier.ToString(),
                module_name = this.ModuleName,
                delete = true
            });

            _Context.SaveChanges();

            var delete_perm_id = _Context.ModulePermissions.Where(m => m.internal_permission_name == DocumentPermissions.Delete).Select(m => m.id).Single();

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

    public async Task<Response<DocumentUploadDto>> CreateOverride(IFormFile file, DocumentUploadCreateCommand commandModel)
	{
        throw new NotImplementedException();
    }


    public async Task<Response<DocumentUploadDto>> Create(DocumentUploadCreateCommand commandModel)
	{
		throw new NotImplementedException();
	}

	public async Task<Response<DocumentUploadDto>> Delete(DocumentUploadDeleteCommand commandModel)
	{
		throw new NotImplementedException();
	}

	public async Task<Response<DocumentUploadDto>> Edit(DocumentUploadEditCommand commandModel)
	{
		throw new NotImplementedException();
	}

	public async Task<PagingResult<DocumentUploadListDto>> Find(PagingSortingParameters parameters, DocumentUploadFindCommand commandModel)
	{
		throw new NotImplementedException();
	}

	public DocumentUpload? Get(int object_id)
	{
		throw new NotImplementedException();
	}

	public async Task<DocumentUpload?> GetAsync(int object_id)
	{
		throw new NotImplementedException();
	}

	public async Task<Response<DocumentUploadDto>> GetDto(int object_id)
	{
		var s = _StorageSettings == null ? "" : "asdasdasd";

		return new Response<DocumentUploadDto>(s, ResultCode.AlreadyExists);
		//throw new NotImplementedException();
	}

	public async Task<Response<List<DocumentUploadListDto>>> GlobalSearch(PagingSortingParameters parameters, string wildcard)
	{
		throw new NotImplementedException();
	}

	public DocumentUpload MapToDatabaseModel(DocumentUploadDto dtoModel)
	{
		throw new NotImplementedException();
	}

	public async Task<DocumentUploadDto> MapToDto(DocumentUpload databaseModel)
	{
		throw new NotImplementedException();
	}

	public async Task<DocumentUploadListDto> MapToListDto(DocumentUpload databaseModel)
	{
		throw new NotImplementedException();
	}
}
