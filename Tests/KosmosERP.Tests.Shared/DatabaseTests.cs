using Microsoft.EntityFrameworkCore;
using KosmosERP.BusinessLayer.Helpers;
using KosmosERP.Database;
using KosmosERP.Database.Models;

namespace KosmosERP.Tests.Shared;

public class DatabaseTests
{
    private ERPDbContext _Context;
    private User _User;

    private string _ModuleId = Guid.NewGuid().ToString();

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<DbContext>()
                  .UseInMemoryDatabase(Guid.NewGuid().ToString())
                  .Options;

        _Context = new ERPDbContext(options);

        var baseUser = CommonDataHelper<KosmosERP.Database.Models.User>.FillCommonFields(new User()
        {
            first_name = "test",
            last_name = "user",
            email = "test@email.com",
            username = "test",
            password = "password",
            password_salt = "asdasd",
            employee_number = "10001",
            department = 1,
            guid = Guid.NewGuid().ToString(),
            is_admin = true,
        }, 1);

        _Context.Users.Add(baseUser);
        _Context.SaveChanges();

        _User = _Context.Users.First();
    }

    [TearDown]
    public void Destroy()
    {
        _Context.Dispose();
    }

    [Test]
    public async Task UserRoles()
    {
        var roleModel1 = CommonDataHelper<Role>.FillCommonFields(new Role()
        {
            name = "ExampleRole",
        }, 1);

        var roleModel2 = CommonDataHelper<Role>.FillCommonFields(new Role()
        {
            name = "AnotherRole",
        }, 1);

        await _Context.Roles.AddAsync(roleModel1);
        await _Context.Roles.AddAsync(roleModel2);
        await _Context.SaveChangesAsync();

        var userRole1 = CommonDataHelper<UserRole>.FillCommonFields(new UserRole()
        {
            role_id = roleModel1.id,
            user_id = _User.id,
        }, 1);

        var userRole2 = CommonDataHelper<UserRole>.FillCommonFields(new UserRole()
        {
            role_id = roleModel2.id,
            user_id = _User.id,
        }, 1);

        await _Context.UserRoles.AddAsync(userRole1);
        await _Context.UserRoles.AddAsync(userRole2);
        await _Context.SaveChangesAsync();

        _User = await _Context.Users.FirstAsync();

        var userRoles = await _Context.UserRoles.Where(m => m.user_id == _User.id).ToListAsync();
        
        Assert.That(userRoles.Count() == 2);
        Assert.That(_User.roles.Count() == 2);
    }

    [Test]
    public async Task RolesAndPermissions_ReadEdit()
    {
        var roleModel = CommonDataHelper<Role>.FillCommonFields(new Role()
        {
            name = "ReadEditModuleRole",
        }, 1);

        var moduleReadModel = new ModulePermission()
        {
            module_id = _ModuleId,
            permission_name = "module_read",
            internal_permission_name = "module_read",
            module_name = "module",
            read = true,
            edit = false,
            delete = false,
            write = false,
            is_active = true
        };

        var moduleEditModel = new ModulePermission()
        {
            module_id = _ModuleId,
            permission_name = "module_edit",
            internal_permission_name = "module_edit",
            module_name = "module",
            read = false,
            edit = true,
            delete = false,
            write = false,
            is_active = true
        };

        await _Context.Roles.AddAsync(roleModel);
        await _Context.ModulePermissions.AddAsync(moduleReadModel);
        await _Context.ModulePermissions.AddAsync(moduleEditModel);

        await _Context.SaveChangesAsync();

        var role = await _Context.Roles.SingleAsync(m => m.name == roleModel.name);
        var readModulePermission = await _Context.ModulePermissions.SingleAsync(m => m.module_id == _ModuleId && m.permission_name == moduleReadModel.permission_name);
        var editModulePermission = await _Context.ModulePermissions.SingleAsync(m => m.module_id == _ModuleId && m.permission_name == moduleEditModel.permission_name);

        var rolePermissionReadModel = CommonDataHelper<RolePermission>.FillCommonFields(new RolePermission()
        {
            module_permission_id = readModulePermission.id,
            role_id = role.id,
        }, 1);

        var rolePermissionEditModel = CommonDataHelper<RolePermission>.FillCommonFields(new RolePermission()
        {
            module_permission_id = editModulePermission.id,
            role_id = role.id,
        }, 1);


        await _Context.RolePermissions.AddAsync(rolePermissionReadModel);
        await _Context.RolePermissions.AddAsync(rolePermissionEditModel);

        await _Context.SaveChangesAsync();

        // Reget the role to populate the permissions
        role = await _Context.Roles.SingleAsync(m => m.name == roleModel.name);

        // Going to test read and edit
        var can_read = role.role_permissions.Select(m => m.module_permission).Where(m => m.is_active == true && m.read == true).Any();
        var can_edit = role.role_permissions.Select(m => m.module_permission).Where(m => m.is_active == true && m.edit == true).Any();
        var can_delete = role.role_permissions.Select(m => m.module_permission).Where(m => m.is_active == true && m.delete == true).Any();
        var can_write = role.role_permissions.Select(m => m.module_permission).Where(m => m.is_active == true && m.write == true).Any();


        Assert.True(can_read);
        Assert.True(can_edit);
        Assert.False(can_delete);
        Assert.False(can_write);
    }

    [Test]
    public async Task RolesAndPermissions_WriteDelete()
    {
        var roleModel = CommonDataHelper<Role>.FillCommonFields(new Role()
        {
            name = "WriteDeleteModuleRole",
            created_by = 1,
            created_on = DateTime.UtcNow,
            updated_by = 1,
            updated_on = DateTime.UtcNow
        }, 1);

        var moduleWriteModel = new ModulePermission()
        {
            module_id = _ModuleId,
            permission_name = "module_write",
            internal_permission_name = "module_write",
            module_name = "module",
            read = false,
            edit = false,
            delete = false,
            write = true,
            is_active = true
        };

        var moduleDeleteModel = new ModulePermission()
        {
            module_id = _ModuleId,
            permission_name = "module_delete",
            internal_permission_name = "module_delete",
            module_name = "module",
            read = false,
            edit = false,
            delete = true,
            write = false,
            is_active = true
        };

        await _Context.Roles.AddAsync(roleModel);
        await _Context.ModulePermissions.AddAsync(moduleWriteModel);
        await _Context.ModulePermissions.AddAsync(moduleDeleteModel);

        await _Context.SaveChangesAsync();

        var role = await _Context.Roles.SingleAsync(m => m.name == roleModel.name);
        var writeModulePermission = await _Context.ModulePermissions.SingleAsync(m => m.module_id == _ModuleId && m.permission_name == moduleWriteModel.permission_name);
        var deleteModulePermission = await _Context.ModulePermissions.SingleAsync(m => m.module_id == _ModuleId && m.permission_name == moduleDeleteModel.permission_name);

        var rolePermissionWriteModel = CommonDataHelper<RolePermission>.FillCommonFields(new RolePermission()
        {
            module_permission_id = writeModulePermission.id,
            role_id = role.id,
        }, 1);

        var rolePermissionDeleteModel = CommonDataHelper<RolePermission>.FillCommonFields(new RolePermission()
        {
            module_permission_id = deleteModulePermission.id,
            role_id = role.id,
        }, 1);


        await _Context.RolePermissions.AddAsync(rolePermissionWriteModel);
        await _Context.RolePermissions.AddAsync(rolePermissionDeleteModel);

        await _Context.SaveChangesAsync();

        // Reget the role to populate the permissions
        role = await _Context.Roles.SingleAsync(m => m.name == roleModel.name);

        // Going to test read and edit
        var can_read = role.role_permissions.Select(m => m.module_permission).Where(m => m.is_active == true && m.read == true).Any();
        var can_edit = role.role_permissions.Select(m => m.module_permission).Where(m => m.is_active == true && m.edit == true).Any();
        var can_delete = role.role_permissions.Select(m => m.module_permission).Where(m => m.is_active == true && m.delete == true).Any();
        var can_write = role.role_permissions.Select(m => m.module_permission).Where(m => m.is_active == true && m.write == true).Any();


        Assert.False(can_read);
        Assert.False(can_edit);
        Assert.True(can_delete);
        Assert.True(can_write);
    }

    [Test]
    public async Task VendorProducts()
    {
        var address_model = CommonDataHelper<Address>.FillCommonFields(new Address()
        {
            street_address1 = "123 St",
            city = "City",
            state = "TX",
            postal_code = "77777",
            country = "USA",
        }, 1);

        await _Context.Addresses.AddAsync(address_model);
        await _Context.SaveChangesAsync();

        var vendor_model = CommonDataHelper<Vendor>.FillCommonFields(new Vendor()
        {
            vendor_name = "Vendor",
            
            phone = "123-456-7890",
            category = "Cat1",
            address_id = address_model.id,
        }, 1);

        await _Context.Vendors.AddAsync(vendor_model);
        await _Context.SaveChangesAsync();

        var product_model = CommonDataHelper<Product>.FillCommonFields(new Product()
        {
            vendor_id = vendor_model.id,
            product_class = "Class",
            category = "Cat1",
            identifier1 = "SKU-1",
            internal_description = "A product description",
            product_name = "A product",
        }, 1);

        await _Context.Products.AddAsync(product_model);
        await _Context.SaveChangesAsync();

        var product_attribute1 = CommonDataHelper<ProductAttribute>.FillCommonFields(new ProductAttribute()
        {
            product_id = product_model.id,
            attribute_name = "Weight",
            attribute_value = "10",
        }, 1);

        var product_attribute2 = CommonDataHelper<ProductAttribute>.FillCommonFields(new ProductAttribute()
        {
            product_id = product_model.id,
            attribute_name = "Length",
            attribute_value = "12",
        }, 1);

        await _Context.ProductAttributes.AddAsync(product_attribute1);
        await _Context.ProductAttributes.AddAsync(product_attribute2);
        await _Context.SaveChangesAsync();

        vendor_model = await _Context.Vendors.SingleAsync(m => m.id == vendor_model.id);        

        Assert.That(vendor_model.products.Count() == 1);
        Assert.That(vendor_model.products.First().attributes.Count() == 2);
    }

    [Test]
    public async Task CustomerAddresses()
    {
        var customer_model = CommonDataHelper<Customer>.FillCommonFields(new Customer()
        { 
            customer_name = "Customer",
            phone = "123-456-7890",
            category = "Cat1",
        }, 1);

        await _Context.Customers.AddAsync(customer_model);
        await _Context.SaveChangesAsync();


        var address_model1 = CommonDataHelper<Address>.FillCommonFields(new Address()
        {
            street_address1 = "123 St",
            city = "City",
            state = "TX",
            postal_code = "77777",
            country = "USA",
            created_by = 1,
            created_on = DateTime.UtcNow,
            updated_by = 1,
            updated_on = DateTime.UtcNow
        }, 1);

        var address_model2 = CommonDataHelper<Address>.FillCommonFields(new Address()
        {
            street_address1 = "9999 St",
            city = "Place",
            state = "OK",
            postal_code = "99999",
            country = "USA",
        }, 1);

        await _Context.Addresses.AddAsync(address_model1);
        await _Context.Addresses.AddAsync(address_model2);
        await _Context.SaveChangesAsync();

        var customer_address_model1 = CommonDataHelper<CustomerAddress>.FillCommonFields(new CustomerAddress()
        {
            customer_id = customer_model.id,
            address_id = address_model1.id,
        }, 1);

        var customer_address_model2 = CommonDataHelper<CustomerAddress>.FillCommonFields(new CustomerAddress()
        {
            customer_id = customer_model.id,
            address_id = address_model2.id,
            created_by = 1,
            created_on = DateTime.UtcNow,
            updated_by = 1,
            updated_on = DateTime.UtcNow
        }, 1);

        await _Context.CustomerAddresses.AddAsync(customer_address_model1);
        await _Context.CustomerAddresses.AddAsync(customer_address_model2);
        await _Context.SaveChangesAsync();

        customer_model = await _Context.Customers.FirstAsync();

        Assert.That(customer_model.addresses.Count() == 2);
    }

    [Test]
    public async Task Orders()
    {
        var address_model = CommonDataHelper<Address>.FillCommonFields(new Address()
        {
            street_address1 = "123 St",
            city = "City",
            state = "TX",
            postal_code = "77777",
            country = "USA",
        }, 1);

        await _Context.Addresses.AddAsync(address_model);
        await _Context.SaveChangesAsync();

        var order_model = CommonDataHelper<OrderHeader>.FillCommonFields(new OrderHeader()
        {
            customer_id = 1,
            order_type = "Q",
            ship_to_address_id = address_model.id,
            shipping_method_id = 1,
            pay_method_id = 1,
            order_date = DateOnly.FromDateTime(DateTime.UtcNow),
            required_date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            price = 1,
            tax = 1,
        }, 1);

        await _Context.OrderHeaders.AddAsync(order_model);
        await _Context.SaveChangesAsync();

        var order_line = CommonDataHelper<OrderLine>.FillCommonFields(new OrderLine()
        { 
            order_header_id = order_model.id,
            product_id = 1,
            quantity = 1,
            line_number = 1,
            unit_price = 10,
            line_description = "ASDASDASDASD",
        }, 1);

        await _Context.OrderLines.AddAsync(order_line);
        await _Context.SaveChangesAsync();

        var line_attribute1 = CommonDataHelper<OrderLineAttribute>.FillCommonFields(new OrderLineAttribute()
        {
            order_line_id = order_line.id,
            attribute_name = "Weight",
            attribute_value = "10",
        }, 1);

        var line_attribute2 = CommonDataHelper<OrderLineAttribute>.FillCommonFields(new OrderLineAttribute()
        {
            order_line_id = order_line.id,
            attribute_name = "Height",
            attribute_value = "1",
            created_by = 1,
            created_on = DateTime.UtcNow,
            updated_by = 1,
            updated_on = DateTime.UtcNow
        }, 1);

        await _Context.OrderLineAttributes.AddAsync(line_attribute1);
        await _Context.OrderLineAttributes.AddAsync(line_attribute2);
        await _Context.SaveChangesAsync();

        order_model = await _Context.OrderHeaders.SingleAsync(m => m.id == order_model.id);

        Assert.That(order_model.order_lines.Count() == 1);
        Assert.That(order_model.order_lines[0].attributes.Count() == 2);
    }

    [Test]
    public async Task APInvoices()
    {
        var invoice_header_model = CommonDataHelper<APInvoiceHeader>.FillCommonFields(new APInvoiceHeader()
        {
            invoice_number = "1",
            invoice_due_date = DateTime.UtcNow.AddDays(30),
            invoice_received_date = DateTime.UtcNow.AddDays(-10),
            invoice_date = DateTime.UtcNow,
            invoice_total = 100,
            vendor_id = 1,
            memo = "ASDASDSD",
        }, 1);

        await _Context.APInvoiceHeaders.AddAsync(invoice_header_model);
        await _Context.SaveChangesAsync();

        var invoice_line_model = CommonDataHelper<APInvoiceLine>.FillCommonFields(new APInvoiceLine()
        {
            ap_invoice_header_id = invoice_header_model.id,
            association_is_ar_invoice = false,
            association_is_purchase_order = true,
            association_is_sales_order = false,
            association_object_id = 1,
            gl_account_id = 1,
            description = "",
            line_total = 100,
            association_object_line_id = 1,
            qty_invoiced = 1,
        }, 1);

        await _Context.APInvoiceLines.AddAsync(invoice_line_model);
        await _Context.SaveChangesAsync();

        invoice_header_model = await _Context.APInvoiceHeaders.SingleAsync(m => m.id == invoice_header_model.id);

        Assert.That(invoice_header_model.ap_invoice_lines.Count() == 1);
    }

    [Test]
    public async Task ARInvoices()
    {
        var order_model = CommonDataHelper<OrderHeader>.FillCommonFields(new OrderHeader()
        {
            customer_id = 1,
            ship_to_address_id = 1,
            shipping_method_id = 1,
            pay_method_id = 1,
            order_type = "R",
            order_date = DateOnly.FromDateTime(DateTime.UtcNow),
            required_date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            price = 1,
            tax = 1,
        }, 1);

        await _Context.OrderHeaders.AddAsync(order_model);
        await _Context.SaveChangesAsync();

        var order_line_model = CommonDataHelper<OrderLine>.FillCommonFields(new OrderLine()
        {
            order_header_id = order_model.id,
            product_id = 1,
            quantity = 10,
            line_number = 1,
            unit_price = 10,
            line_description = "ASDASDASDASD",
        }, 1);

        await _Context.OrderLines.AddAsync(order_line_model);
        await _Context.SaveChangesAsync();

        var product_model = CommonDataHelper<Product>.FillCommonFields(new Product()
        {
            vendor_id = 1,
            product_class = "Class",
            category = "Cat1",
            identifier1 = "SKU-1",
            internal_description = "A product description",
            product_name = "A product",
        }, 1);

        await _Context.Products.AddAsync(product_model);
        await _Context.SaveChangesAsync();

        var invoice_header_model = CommonDataHelper<ARInvoiceHeader>.FillCommonFields(new ARInvoiceHeader()
        {
            invoice_number = 10001,
            customer_id = 1,
            invoice_date = DateOnly.FromDateTime(DateTime.UtcNow),
            invoice_total = 100,
            order_header_id = order_model.id,
            tax_percentage = 8.25M,
            paid_on = DateOnly.FromDateTime(DateTime.UtcNow),
            is_taxable = true,
        }, 1);

        await _Context.ARInvoiceHeaders.AddAsync(invoice_header_model);
        await _Context.SaveChangesAsync();

        var invoice_line_model = CommonDataHelper<ARInvoiceLine>.FillCommonFields(new ARInvoiceLine()
        {
            ar_invoice_header_id = invoice_header_model.id,
            product_id = product_model.id,
            order_line_id = order_line_model.id,
            line_description = "ASDASDASDASD",
            line_number = 1,
            line_tax = 1,
            order_qty = 10,
            invoice_qty = 10,
            line_total = 100,
        }, 1);

        await _Context.ARInvoiceLines.AddAsync(invoice_line_model);
        await _Context.SaveChangesAsync();

        invoice_header_model = await _Context.ARInvoiceHeaders.SingleAsync(m => m.id == invoice_header_model.id);

        Assert.That(invoice_header_model.ar_invoice_lines.Count() == 1);
        Assert.NotNull(invoice_header_model.ar_invoice_lines[0].product);
        Assert.NotNull(invoice_header_model.ar_invoice_lines[0].order_line);
    }

    [Test]
    public async Task Shipments()
    {
        var order_model = CommonDataHelper<OrderHeader>.FillCommonFields(new OrderHeader()
        {
            customer_id = 1,
            ship_to_address_id = 1,
            shipping_method_id = 1,
            pay_method_id = 1,
            order_type = "R",
            order_date = DateOnly.FromDateTime(DateTime.UtcNow),
            required_date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            price = 1,
            tax = 1,
        }, 1);

        await _Context.OrderHeaders.AddAsync(order_model);
        await _Context.SaveChangesAsync();

        var order_line_model = CommonDataHelper<OrderLine>.FillCommonFields(new OrderLine()
        {
            order_header_id = order_model.id,
            product_id = 1,
            quantity = 10,
            line_number = 1,
            unit_price = 10,
            line_description = "ASDASDASDASD",
        }, 1);

        await _Context.OrderLines.AddAsync(order_line_model);
        await _Context.SaveChangesAsync();

        var address_model = CommonDataHelper<Address>.FillCommonFields(new Address()
        {
            street_address1 = "123 St",
            city = "City",
            state = "TX",
            postal_code = "77777",
            country = "USA",
        }, 1);

        await _Context.Addresses.AddAsync(address_model);
        await _Context.SaveChangesAsync();

        var shipment_header_model = CommonDataHelper<ShipmentHeader>.FillCommonFields(new ShipmentHeader()
        {
            order_id = order_model.id,
            shipment_number = 10001,
            tax = 10,
            ship_via = "UPS",
            units_shipped = 0,
            units_to_ship = 10,
            ship_attn = "Hello",
            address_id = address_model.id,
            freight_charge_amount = 100,
        }, 1);

        await _Context.ShipmentHeaders.AddAsync(shipment_header_model);
        await _Context.SaveChangesAsync();

        var shipment_line_model = CommonDataHelper<ShipmentLine>.FillCommonFields(new ShipmentLine()
        {
            shipment_header_id = shipment_header_model.id,
            order_line_id = order_line_model.id,
            units_shipped = 0,
            units_to_ship = 10,

        }, 1);

        await _Context.ShipmentLines.AddAsync(shipment_line_model);
        await _Context.SaveChangesAsync();

        shipment_header_model = await _Context.ShipmentHeaders.SingleAsync(m => m.id == shipment_header_model.id);
        
        Assert.NotNull(shipment_header_model.address);
        Assert.That(shipment_header_model.shipment_lines.Count() == 1);
        Assert.NotNull(shipment_header_model.shipment_lines[0].order_line);
    }

    [Test]
    public async Task PurchaseOrders()
    {
        var address_model = CommonDataHelper<Address>.FillCommonFields(new Address()
        {
            street_address1 = "123 St",
            city = "City",
            state = "TX",
            postal_code = "77777",
            country = "USA",
            created_by = 1,
            created_on = DateTime.UtcNow,
            updated_by = 1,
            updated_on = DateTime.UtcNow
        }, 1);

        await _Context.Addresses.AddAsync(address_model);
        await _Context.SaveChangesAsync();

        var vendor_model = CommonDataHelper<Vendor>.FillCommonFields(new Vendor()
        {
            vendor_name = "Vendor",
            address_id = address_model.id,
            phone = "123-456-7890",
            category = "Cat1",
        }, 1);

        await _Context.Vendors.AddAsync(vendor_model);
        await _Context.SaveChangesAsync();

        var product_model = CommonDataHelper<Product>.FillCommonFields(new Product()
        {
            vendor_id = vendor_model.id,
            product_class = "Class",
            category = "Cat1",
            identifier1 = "SKU-1",
            internal_description = "A product description",
            product_name = "A product",
        }, 1);

        await _Context.Products.AddAsync(product_model);
        await _Context.SaveChangesAsync();

        var purchase_order_model = CommonDataHelper<PurchaseOrderHeader>.FillCommonFields(new PurchaseOrderHeader()
        {
            vendor_id = vendor_model.id,
            po_number = 10002,
            po_type = "R",
            revision_number = 2,

        }, 1);

        await _Context.PurchaseOrderHeaders.AddAsync(purchase_order_model);
        await _Context.SaveChangesAsync();

        var purchase_order_line = CommonDataHelper<PurchaseOrderLine>.FillCommonFields(new PurchaseOrderLine()
        {
            purchase_order_header_id = purchase_order_model.id,
            product_id = product_model.id,
            description = "Some product description",
            quantity = 12,
            unit_price = 10.2M,
            line_number = 1,
            revision_number = 1,
            is_taxable = true,
            tax = 1.9M,
        }, 1);

        await _Context.PurchaseOrderLines.AddAsync(purchase_order_line);
        await _Context.SaveChangesAsync();

        purchase_order_model = await _Context.PurchaseOrderHeaders.SingleAsync(m => m.id == purchase_order_model.id);

        Assert.NotNull(purchase_order_model.vendor);
        Assert.That(purchase_order_model.purchase_order_lines.Count() == 1);
        Assert.NotNull(purchase_order_model.purchase_order_lines[0].product);
    }

    [Test]
    public async Task PurchaseOrderReceive()
    {
        var purchase_order_model = CommonDataHelper<PurchaseOrderHeader>.FillCommonFields(new PurchaseOrderHeader()
        {
            vendor_id = 1,
            po_number = 10002,
            po_type = "R",
            revision_number = 2,
        }, 1);

        await _Context.PurchaseOrderHeaders.AddAsync(purchase_order_model);
        await _Context.SaveChangesAsync();

        var purchase_order_line = CommonDataHelper<PurchaseOrderLine>.FillCommonFields(new PurchaseOrderLine()
        {
            purchase_order_header_id = purchase_order_model.id,
            product_id = 1,
            description = "Some product description",
            quantity = 12,
            unit_price = 10.2M,
            line_number = 1,
            revision_number = 1,
            is_taxable = true,
            tax = 1.9M,
        }, 1);

        await _Context.PurchaseOrderLines.AddAsync(purchase_order_line);
        await _Context.SaveChangesAsync();


        var purchase_order_receive_model = CommonDataHelper<PurchaseOrderReceiveHeader>.FillCommonFields(new PurchaseOrderReceiveHeader()
        {
            purchase_order_id = purchase_order_line.id,
            units_ordered = 10,
            units_received = 2,
        }, 1);

        await _Context.PurchaseOrderReceiveHeaders.AddAsync(purchase_order_receive_model);
        await _Context.SaveChangesAsync();

        var purchase_order_receive_line = CommonDataHelper<PurchaseOrderReceiveLine>.FillCommonFields(new PurchaseOrderReceiveLine()
        {
            purchase_order_receive_header_id = purchase_order_receive_model.id,
            purchase_order_line_id = purchase_order_line.id,
            units_ordered = 10,
            units_received = 2,
        }, 1);

        await _Context.PurchaseOrderReceiveLines.AddAsync(purchase_order_receive_line);
        await _Context.SaveChangesAsync();


        purchase_order_receive_model = await _Context.PurchaseOrderReceiveHeaders.SingleAsync(m => m.id == purchase_order_receive_model.id);

        Assert.NotNull(purchase_order_receive_model.purchase_order);
        Assert.That(purchase_order_receive_model.purchase_order_receive_lines.Count() == 1);
        Assert.NotNull(purchase_order_receive_model.purchase_order_receive_lines[0].purchase_order_line);
    }

    [Test]
    public async Task Opportunities()
    {
        var customer_model = CommonDataHelper<Customer>.FillCommonFields(new Customer()
        {
            customer_name = "Customer",
            phone = "123-456-7890",
            category = "Cat1",
        }, 1);

        await _Context.Customers.AddAsync(customer_model);
        await _Context.SaveChangesAsync();

        var contact_model = CommonDataHelper<Contact>.FillCommonFields(new Contact()
        {
            customer_id = customer_model.id,
            first_name = "Bob",
            last_name = "Guy",
            title = "CEO",
            email = "test@test.com",
            cell_phone = "555-555-5555",
        }, 1);

        await _Context.Contacts.AddAsync(contact_model);
        await _Context.SaveChangesAsync();

        var opportunity_model = CommonDataHelper<Opportunity>.FillCommonFields(new Opportunity()
        {
            contact_id = contact_model.id,
            customer_id = customer_model.id,
            opportunity_name = "Opportunity 1",
            amount = 100,
            win_chance = 50,
            stage = "Prospect",
            expected_close = DateOnly.FromDateTime(DateTime.UtcNow),
        }, 1);

        await _Context.Opportunities.AddAsync(opportunity_model);
        await _Context.SaveChangesAsync();

        var oppotunity_line = CommonDataHelper<OpportunityLine>.FillCommonFields(new OpportunityLine()
        {
            opportunity_id = opportunity_model.id,
            description = "A description line",
            unit_price = 100.20M,
            line_number = 1,
            quantity = 1,
        }, 1);

        await _Context.OpportunityLines.AddAsync(oppotunity_line);
        await _Context.SaveChangesAsync();

        opportunity_model = await _Context.Opportunities.SingleAsync(m => m.id == opportunity_model.id);

        Assert.NotNull(opportunity_model.customer);
        Assert.NotNull(opportunity_model.contact);
        Assert.That(opportunity_model.opportunity_lines.Count() == 1);
    }

    [Test]
    public async Task Documents()
    {
        var document_upload_object = new DocumentUploadObject()
        {
            friendly_name = "AP Invoice",
            internal_name = "ap_invoice",
            requires_approval = false,
        };

        await _Context.DocumentUploadObjects.AddAsync(document_upload_object);
        await _Context.SaveChangesAsync();

        var document_upload_tag = new DocumentUploadObjectTagTemplate()
        {
            document_object_id = document_upload_object.id,

            name = "PO Number",
        };

        await _Context.DocumentUploadObjectTags.AddAsync(document_upload_tag);
        await _Context.SaveChangesAsync();

        document_upload_object = await _Context.DocumentUploadObjects.SingleAsync(m => m.id == document_upload_object.id);

        Assert.That(document_upload_object.object_tags.Count() == 1);

        var document_upload = CommonDataHelper<DocumentUpload>.FillCommonFields(new DocumentUpload()
        {
            document_object_id = document_upload_object.id,
            rev_num = 1,
        }, 1);

        await _Context.DocumentUploads.AddAsync(document_upload);
        await _Context.SaveChangesAsync();

        var document_revision = CommonDataHelper<DocumentUploadRevision>.FillCommonFields(new DocumentUploadRevision()
        {
            document_upload_id = document_upload_object.id,
            document_name = "Document.jpg",
            document_path = "https://asdkasdjasdk.com/Document.jpg",
            document_type = "UPLOAD",
            rev_num = 1,
        }, 1);

        await _Context.DocumentUploadRevisions.AddAsync(document_revision);
        await _Context.SaveChangesAsync();

        var document_revision_tag = CommonDataHelper<DocumentUploadRevisionTag>.FillCommonFields(new DocumentUploadRevisionTag()
        {
            document_upload_revision_id = document_revision.id,
            document_upload_object_tag_id = document_upload_tag.id,
            tag_name = "PO Number",
            tag_value = "123442",
        }, 1);

        await _Context.DocumentUploadRevisionsTags.AddAsync(document_revision_tag);
        await _Context.SaveChangesAsync();

        document_upload = await _Context.DocumentUploads.SingleAsync(m => m.id == document_upload.id);

        Assert.That(document_upload.document_revisions.Count() == 1);
        Assert.That(document_upload.document_revisions[0].revision_tags.Count() == 1);
    }
}