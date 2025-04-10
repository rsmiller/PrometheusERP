﻿using Microsoft.EntityFrameworkCore;
using Prometheus.Database;
using Prometheus.Database.Models;
using Prometheus.Models;
using Prometheus.Models.Helpers;
using Prometheus.Models.Interfaces;
using Prometheus.Module;
using Prometheus.BusinessLayer.Models.Module.Product.Command.Create;
using Prometheus.BusinessLayer.Models.Module.Product.Command.Delete;
using Prometheus.BusinessLayer.Models.Module.Product.Command.Edit;
using Prometheus.BusinessLayer.Models.Module.Product.Command.Find;
using Prometheus.BusinessLayer.Models.Module.Product.Dto;
using Prometheus.Models.Permissions;

namespace Prometheus.BusinessLayer.Modules
{
    public interface IProductModule : IERPModule<Database.Models.Product, ProductDto, ProductListDto, ProductCreateCommand, ProductEditCommand, ProductDeleteCommand, ProductFindCommand>, IBaseERPModule
    {

    }

    public class ProductModule : BaseERPModule, IProductModule
    {
        public override Guid ModuleIdentifier => Guid.Parse("b8b0d255-3901-4007-b9c7-b0678f89c955");
        public override string ModuleName => "Products";

        private IBaseERPContext _Context;

        public ProductModule(IBaseERPContext context) : base(context)
        {
            _Context = context;
        }

        public override void SeedPermissions()
        {
            var role = _Context.Roles.Any(m => m.name == "Product Users");
            var read_permission = _Context.ModulePermissions.Any(m => m.module_id == this.ModuleIdentifier.ToString() && m.internal_permission_name == ProductPermissions.Read);
            var create_permission = _Context.ModulePermissions.Any(m => m.module_id == this.ModuleIdentifier.ToString() && m.internal_permission_name == ProductPermissions.Create);
            var edit_permission = _Context.ModulePermissions.Any(m => m.module_id == this.ModuleIdentifier.ToString() && m.internal_permission_name == ProductPermissions.Edit);
            var delete_permission = _Context.ModulePermissions.Any(m => m.module_id == this.ModuleIdentifier.ToString() && m.internal_permission_name == ProductPermissions.Delete);

            if (role == false)
            {
                _Context.Roles.Add(new Role()
                {
                    name = "Product Users",
                    created_by = 1,
                    created_on = DateTime.Now,
                    updated_by = 1,
                    updated_on = DateTime.Now,
                });

                _Context.SaveChanges();
            }

            var role_id = _Context.Roles.Where(m => m.name == "Product Users").Select(m => m.id).Single();

            if (read_permission == false)
            {
                _Context.ModulePermissions.Add(new ModulePermission()
                {
                    permission_name = "Read Product",
                    internal_permission_name = ProductPermissions.Read,
                    module_id = this.ModuleIdentifier.ToString(),
                    module_name = this.ModuleName,
                    read = true,
                });

                _Context.SaveChanges();

                var read_perm_id = _Context.ModulePermissions.Where(m => m.internal_permission_name == ProductPermissions.Read).Select(m => m.id).Single();

                _Context.RolePermissions.Add(new RolePermission()
                {
                    role_id = role_id,
                    module_permission_id = read_perm_id,
                    created_by = 1,
                    created_on = DateTime.Now,
                    updated_by = 1,
                    updated_on = DateTime.Now,
                });

                _Context.SaveChanges();
            }

            if (create_permission == false)
            {
                _Context.ModulePermissions.Add(new ModulePermission()
                {
                    permission_name = "Create Product",
                    internal_permission_name = ProductPermissions.Create,
                    module_id = this.ModuleIdentifier.ToString(),
                    module_name = this.ModuleName,
                    write = true
                });

                _Context.SaveChanges();

                var create_perm_id = _Context.ModulePermissions.Where(m => m.internal_permission_name == ProductPermissions.Create).Select(m => m.id).Single();

                _Context.RolePermissions.Add(new RolePermission()
                {
                    role_id = role_id,
                    module_permission_id = create_perm_id,
                    created_by = 1,
                    created_on = DateTime.Now,
                    updated_by = 1,
                    updated_on = DateTime.Now,
                });

                _Context.SaveChanges();
            }

            if (edit_permission == false)
            {
                _Context.ModulePermissions.Add(new ModulePermission()
                {
                    permission_name = "Edit Product",
                    internal_permission_name = ProductPermissions.Edit,
                    module_id = this.ModuleIdentifier.ToString(),
                    module_name = this.ModuleName,
                    edit = true
                });

                _Context.SaveChanges();

                var edit_perm_id = _Context.ModulePermissions.Where(m => m.internal_permission_name == ProductPermissions.Edit).Select(m => m.id).Single();

                _Context.RolePermissions.Add(new RolePermission()
                {
                    role_id = role_id,
                    module_permission_id = edit_perm_id,
                    created_by = 1,
                    created_on = DateTime.Now,
                    updated_by = 1,
                    updated_on = DateTime.Now,
                });

                _Context.SaveChanges();
            }

            if (delete_permission == false)
            {
                _Context.ModulePermissions.Add(new ModulePermission()
                {
                    permission_name = "Delete Product",
                    internal_permission_name = ProductPermissions.Delete,
                    module_id = this.ModuleIdentifier.ToString(),
                    module_name = this.ModuleName,
                    delete = true
                });

                _Context.SaveChanges();

                var delete_perm_id = _Context.ModulePermissions.Where(m => m.internal_permission_name == ProductPermissions.Delete).Select(m => m.id).Single();

                _Context.RolePermissions.Add(new RolePermission()
                {
                    role_id = role_id,
                    module_permission_id = delete_perm_id,
                    created_by = 1,
                    created_on = DateTime.Now,
                    updated_by = 1,
                    updated_on = DateTime.Now,
                });

                _Context.SaveChanges();
            }
        }

        public Database.Models.Product? Get(int object_id)
        {
            return _Context.Products.SingleOrDefault(m => m.id == object_id);
        }

        public async Task<Database.Models.Product?> GetAsync(int object_id)
        {
            return await _Context.Products.SingleOrDefaultAsync(m => m.id == object_id);
        }

        public async Task<Response<ProductDto>> GetDto(int object_id)
        {
            Response<ProductDto> response = new Response<ProductDto>();

            var result = await _Context.Products.SingleOrDefaultAsync(m => m.id == object_id);
            if (result == null)
            {
                response.SetException("Product not found", ResultCode.NotFound);
                return response;
            }

            response.Data = await this.MapToDto(result);
            return response;
        }

        public async Task<Response<ProductDto>> Create(ProductCreateCommand commandModel)
        {
            if (commandModel == null)
                return new Response<ProductDto>(ResultCode.NullItemInput);

            var validationResult = ModelValidationHelper.ValidateModel(commandModel);
            if (!validationResult.Success)
                return new Response<ProductDto>(validationResult.Exception, ResultCode.DataValidationError);

            var permission_result = await base.HasPermission(commandModel.calling_user_id, commandModel.token,ProductPermissions.Create, write: true);
            if (!permission_result)
                return new Response<ProductDto>("Invalid permission", ResultCode.InvalidPermission);

            try
            {
                var alreadyExists = ProductExists(commandModel);
                if (alreadyExists == true)
                    return new Response<ProductDto>(ResultCode.AlreadyExists);


                var item = MapToDatabaseModel(commandModel);

                await _Context.Products.AddAsync(item);
                await _Context.SaveChangesAsync();

                var dto = await GetDto(item.id);

                return new Response<ProductDto>(dto.Data);
            }
            catch (Exception ex)
            {
                return new Response<ProductDto>(ex.Message, ResultCode.Error);
            }
        }

        public async Task<Response<ProductDto>> Edit(ProductEditCommand commandModel)
        {
            if (commandModel == null)
                return new Response<ProductDto>(ResultCode.NullItemInput);

            var validationResult = ModelValidationHelper.ValidateModel(commandModel);
            if (!validationResult.Success)
                return new Response<ProductDto>(validationResult.Exception, ResultCode.DataValidationError);

            var permission_result = await base.HasPermission(commandModel.calling_user_id, commandModel.token,ProductPermissions.Edit, write: true);
            if (!permission_result)
                return new Response<ProductDto>("Invalid permission", ResultCode.InvalidPermission);

            throw new NotImplementedException();
        }

        public async Task<Response<ProductDto>> Delete(ProductDeleteCommand commandModel)
        {
            var validationResult = ModelValidationHelper.ValidateModel(commandModel);
            if (!validationResult.Success)
                return new Response<ProductDto>(validationResult.Exception, ResultCode.DataValidationError);

            var permission_result = await base.HasPermission(commandModel.calling_user_id, commandModel.token,ProductPermissions.Delete, delete: true);
            if (!permission_result)
                return new Response<ProductDto>("Invalid permission", ResultCode.InvalidPermission);


            var existingEntity = await GetAsync(commandModel.id);
            if (existingEntity == null)
                return new Response<ProductDto>("Product not found", ResultCode.NotFound);

            existingEntity.is_deleted = true;
            existingEntity.deleted_on = DateTime.Now;
            existingEntity.deleted_by = commandModel.calling_user_id;

            _Context.Products.Update(existingEntity);
            await _Context.SaveChangesAsync();

            var dto = await MapToDto(existingEntity);
            return new Response<ProductDto>(dto);
        }

        public async Task<PagingResult<ProductListDto>> Find(PagingSortingParameters parameters, ProductFindCommand commandModel)
        {
            throw new NotImplementedException();
        }

        public async Task<Response<List<ProductListDto>>> GlobalSearch(PagingSortingParameters parameters, string wildcard)
        {
            throw new NotImplementedException();
        }

        public async Task<ProductListDto> MapToListDto(Database.Models.Product databaseModel)
        {
            var vendor_name = await _Context.Vendors.Where(m => m.id == databaseModel.vendor_id).Select(m => m.vendor_name).SingleAsync();

            var dto = new ProductListDto()
            {
                sales_price = databaseModel.sales_price,
                unit_cost = databaseModel.unit_cost,
                list_price = databaseModel.list_price,
                product_class = databaseModel.product_class,
                product_name = databaseModel.product_name,
                is_sales_item = databaseModel.is_sales_item,
                category = databaseModel.category,
                guid = databaseModel.guid,
                id = databaseModel.id,
                internal_description = databaseModel.internal_description,
                identifier1 = databaseModel.identifier1,
                identifier2 = databaseModel.identifier2,
                identifier3 = databaseModel.identifier3,
                vendor_id = databaseModel.vendor_id,
                vendor_name = vendor_name,
                created_on = databaseModel.created_on,
                external_description = databaseModel.external_description,
            };

            return dto;
        }

        public async Task<ProductDto> MapToDto(Database.Models.Product databaseModel)
        {
            var dto = new ProductDto()
            {
                sales_price = databaseModel.sales_price,
                unit_cost = databaseModel.unit_cost,
                list_price = databaseModel.list_price,
                product_class = databaseModel.product_class,
                product_name = databaseModel.product_name,
                is_sales_item = databaseModel.is_sales_item,
                category = databaseModel.category,
                guid = databaseModel.guid,
                id = databaseModel.id,
                internal_description = databaseModel.internal_description,
                identifier1 = databaseModel.identifier1,
                identifier2 = databaseModel.identifier2,
                identifier3 = databaseModel.identifier3,
                is_labor = databaseModel.is_labor,
                is_material = databaseModel.is_material,
                is_rental_item = databaseModel.is_rental_item,
                is_retired = databaseModel.is_retired,
                is_shippable = databaseModel.is_shippable,
                is_stock = databaseModel.is_stock,
                is_taxable = databaseModel.is_taxable,
                vendor_id = databaseModel.vendor_id,
                rfid_id = databaseModel.rfid_id,
                required_min_order = databaseModel.required_min_order,
                required_reorder_level = databaseModel.required_reorder_level,
                required_stock_level = databaseModel.required_stock_level,
                our_cost = databaseModel.our_cost,
                retired_on = databaseModel.retired_on,
                created_on = databaseModel.created_on,
                updated_on = databaseModel.updated_on,
                external_description = databaseModel.external_description,
            };

            var attributes = await _Context.ProductAttributes.Where(m => m.product_id == databaseModel.id).ToListAsync();
            foreach (var attribute in attributes)
                dto.product_attributes.Add(MapToProductAttributeDto(attribute));

            return dto;
        }

        public Database.Models.Product MapToDatabaseModel(ProductDto dtoModel)
        {
            return new Database.Models.Product()
            {
                sales_price = dtoModel.sales_price,
                unit_cost = dtoModel.unit_cost,
                list_price = dtoModel.list_price,
                product_class = dtoModel.product_class,
                is_sales_item = dtoModel.is_sales_item,
                category = dtoModel.category,
                guid = dtoModel.guid,
                id = dtoModel.id,
                internal_description = dtoModel.internal_description,
                identifier1 = dtoModel.identifier1,
                identifier2 = dtoModel.identifier2,
                identifier3 = dtoModel.identifier3,
                is_labor = dtoModel.is_labor,
                is_material = dtoModel.is_material,
                is_rental_item = dtoModel.is_rental_item,
                is_retired = dtoModel.is_retired,
                is_shippable = dtoModel.is_shippable,
                is_stock = dtoModel.is_stock,
                is_taxable = dtoModel.is_taxable,
                vendor_id = dtoModel.vendor_id,
                rfid_id = dtoModel.rfid_id,
                required_min_order = dtoModel.required_min_order,
                required_reorder_level = dtoModel.required_reorder_level,
                required_stock_level = dtoModel.required_stock_level,
                our_cost = dtoModel.our_cost,
                retired_on = dtoModel.retired_on,
                created_on = dtoModel.created_on,
                updated_on = dtoModel.updated_on,
                external_description = dtoModel.external_description
            };
        }

        public Database.Models.Product MapToDatabaseModel(ProductCreateCommand createCommand)
        {
            return new Database.Models.Product()
            {
                sales_price = createCommand.sales_price,
                unit_cost = createCommand.unit_cost,
                list_price = createCommand.list_price,
                product_class = createCommand.product_class,
                is_sales_item = createCommand.is_sales_item,
                category = createCommand.category,
                guid = createCommand.guid,
                internal_description = createCommand.internal_description,
                identifier1 = createCommand.identifier1,
                identifier2 = createCommand.identifier2,
                identifier3 = createCommand.identifier3,
                is_labor = createCommand.is_labor,
                is_material = createCommand.is_material,
                is_rental_item = createCommand.is_rental_item,
                is_retired = createCommand.is_retired,
                is_shippable = createCommand.is_shippable,
                is_stock = createCommand.is_stock,
                is_taxable = createCommand.is_taxable,
                vendor_id = createCommand.vendor_id,
                rfid_id = createCommand.rfid_id,
                required_min_order = createCommand.required_min_order,
                required_reorder_level = createCommand.required_reorder_level,
                required_stock_level = createCommand.required_stock_level,
                our_cost = createCommand.our_cost,
                external_description = createCommand.external_description,
                created_on = DateTime.Now,
                updated_on = DateTime.Now,
            };
        }

        public ProductAttributeDto MapToProductAttributeDto(Database.Models.ProductAttribute databaseModel)
        {
            return new ProductAttributeDto()
            {
                attribute_name = databaseModel.attribute_name,
                id = databaseModel.id,
                attribute_value = databaseModel.attribute_value,
                attribute_value2 = databaseModel.attribute_value2,
                attribute_value3 = databaseModel.attribute_value3,
                product_id = databaseModel.product_id
            };
        }

        private bool ProductExists(ProductCreateCommand createCommand)
        {
            return false;
        }
    }
}
