using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using KosmosERP.BusinessLayer;
using KosmosERP.BusinessLayer.Interfaces;
using KosmosERP.BusinessLayer.Modules;
using KosmosERP.Database;
using KosmosERP.Models;
using KosmosERP.Models.Interfaces;
using KosmosERP.Module;

var builder = WebApplication.CreateBuilder(args);


/// For development debug
Environment.SetEnvironmentVariable("DatabaseConnectionString", "server=localhost;uid=auser;pwd=12345;database=KosmosERP_erp");
Environment.SetEnvironmentVariable("MessagePublisherAccountProvider", "Mock");
Environment.SetEnvironmentVariable("TransactionMovementTopic", "transaction_movement");
Environment.SetEnvironmentVariable("FileStorageAccountProvider", "Mock");
Environment.SetEnvironmentVariable("APIUsername", "test");
Environment.SetEnvironmentVariable("APIPassword", "password");
Environment.SetEnvironmentVariable("APIPrivateKey", "444333");

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setup =>
{
    // Include 'SecurityScheme' to use JWT Authentication
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "JWT Authentication",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

    setup.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });

});

builder.Services.AddSignalR();

//////////////////////////////////////////////////////////////////////////////////////////////////////////
//
var authenticationSettings = new AuthenticationSettings()
{
    APIPrivateKey = Environment.GetEnvironmentVariable("APIPrivateKey")
};

builder.Services.AddSingleton<IAuthenticationSettings>(authenticationSettings);


var storageAccountSettings = new FileStorageSettings()
{
    account_provider = Environment.GetEnvironmentVariable("FileStorageAccountProvider"),
    azure_connection_string = Environment.GetEnvironmentVariable("AzureConnectionString"),
    azure_container_name = Environment.GetEnvironmentVariable("AzureContainerName"),
    azure_access_key = Environment.GetEnvironmentVariable("AzureAccessKey"),
    aws_access_key = Environment.GetEnvironmentVariable("AWSAccessKey"),
    aws_secret_key = Environment.GetEnvironmentVariable("AWSSecretKey"),
    aws_bucket_name = Environment.GetEnvironmentVariable("AWSBucketName"),
    aws_region = Environment.GetEnvironmentVariable("AWSRegion"),
    gpc_json_file_path = Environment.GetEnvironmentVariable("GPCJsonFilePath"),
    gpc_bucket_name = Environment.GetEnvironmentVariable("GPCBucketName")
};

builder.Services.AddSingleton<IFileStorageSettings>(storageAccountSettings);


///////////////////////////////
// Factories
var sorage_provider = StorageFactory.Create(storageAccountSettings);
builder.Services.AddSingleton<IStorageProvider>(sorage_provider);


var messagePublisherSettings = new MessagePublisherSettings()
{
    account_provider = Environment.GetEnvironmentVariable("MessagePublisherAccountProvider"),
    rabbitmq_host = Environment.GetEnvironmentVariable("RabbitMQHost"),
    rabbitmq_username = Environment.GetEnvironmentVariable("RabbitMQUsername"),
    rabbitmq_password = Environment.GetEnvironmentVariable("RabbitMQPassword"),
    rabbitmq_port = Environment.GetEnvironmentVariable("RabbitMQPort"),
    rabbitmq_virtual_host = Environment.GetEnvironmentVariable("RabbitMQVirtualHost"),
    rabbitmq_exchange = Environment.GetEnvironmentVariable("RabbitMQExchange"),
    aws_region = Environment.GetEnvironmentVariable("AWSRegion"),
    azure_connection_string = Environment.GetEnvironmentVariable("AzureConnectionString"),
    rabbitmq_routing_key = Environment.GetEnvironmentVariable("RabbitMQRoutingKey"),
    transaction_movement_topic = Environment.GetEnvironmentVariable("TransactionMovementTopic")
};



var messaging_provider = MessageFactory.Create(messagePublisherSettings);
builder.Services.AddSingleton<IMessagePublisher>(messaging_provider);

// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

builder.Services.AddScoped<ITokenModule, TokenModule>();

builder.Services.AddScoped<IUserModule, UserModule>();
builder.Services.AddScoped<IAddressModule, AddressModule>();
builder.Services.AddScoped<IStateModule, StateModule>();
builder.Services.AddScoped<ICountryModule, CountryModule>();

builder.Services.AddScoped<ICustomerModule, CustomerModule>();
builder.Services.AddScoped<IOpportunityModule, OpportunityModule>();
builder.Services.AddScoped<ILeadModule, LeadModule>();
builder.Services.AddScoped<IContactModule, ContactModule>();

builder.Services.AddScoped<IShipmentModule, ShipmentModule>();

builder.Services.AddScoped<IOrderModule, OrderModule>();
builder.Services.AddScoped<IPurchaseOrderModule, PurchaseOrderModule>();
builder.Services.AddScoped<IPurchaseOrderReceiveModule, PurchaseOrderReceiveModule>();
builder.Services.AddScoped<IAPInvoiceModule, APInvoiceModule>();
builder.Services.AddScoped<IARInvoiceModule, ARInvoiceModule>();
builder.Services.AddScoped<IBOMModule, BOMModule>();
builder.Services.AddScoped<IProductionOrderModule, ProductionOrderModule>();

builder.Services.AddScoped<IDocumentUploadModule, DocumentUploadModule>();

builder.Services.AddScoped<IProductModule, ProductModule>();
builder.Services.AddScoped<IVendorModule, VendorModule>();
builder.Services.AddScoped<ITransactionModule, TransactionModule>();
//builder.Services.AddScoped<IInventoryModule, InventoryModule>();

builder.Services.AddScoped<INotificationModule, NotificationModule>();
//builder.Services.AddScoped<IValuesModule, ValuesModule>();


builder.Services.AddDbContext<IBaseERPContext, ERPDbContext>(options => options.UseMySQL(Environment.GetEnvironmentVariable("DatabaseConnectionString")));

//////////////////////////////////////////////////////////////////////////////////////////////////////////
// Add authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(m =>
{
    m.TokenValidationParameters =
        new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            ValidateAudience = false,
            ValidIssuer = TokenModule.Issuer,
            IssuerSigningKey = TokenModule.CreateSecurityKey(Environment.GetEnvironmentVariable("APIPrivateKey"))
        };
});

var app = builder.Build();

//////////////////////////////////////////////////////////////////////////////////////////////////////////
/// Run all module initialization methods
/// 
var scope = app.Services.CreateScope();

var modules = typeof(Program).Assembly.GetTypes()
    .Where(x => !x.IsAbstract && x.IsClass && x.GetInterface(nameof(IBaseERPModule)) == typeof(IBaseERPModule));


foreach (var module in modules)
{
    var class_interface = module.GetInterfaces().Where(m => !m.Name.Contains("ERP")).FirstOrDefault();
    
    if(class_interface != null)
    {
        var activated_module = scope.ServiceProvider.GetService(class_interface);

        if (activated_module != null)
        {
            var casted_module = (IBaseERPModule)activated_module;
            try
            {
                casted_module.SeedPermissions();
            }
            catch (Exception) { }
        }    
    }
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////
// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Hubs
app.MapHub<NotificationModule>("/notification_hub");


app.Run();
