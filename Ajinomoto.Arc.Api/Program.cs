using Ajinomoto.Arc.Api.Authorization;
using Ajinomoto.Arc.Api.Helpers;
using Ajinomoto.Arc.Api.Services;
using Ajinomoto.Arc.Business;
using Ajinomoto.Arc.Business.Facades;
using Ajinomoto.Arc.Business.Interfaces;
using Ajinomoto.Arc.Business.Modules;
using Ajinomoto.Arc.Common.Helpers;
using Ajinomoto.Arc.Common.Utils;
using Ajinomoto.Arc.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(Log.Logger);


builder.Services.AddControllers().AddJsonOptions(x =>
{
    // serialize enums as strings in api responses (e.g. Role)
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Ajinomoto Arc API",
        Description = "API for AR Clearing application",
        Contact = new OpenApiContact
        {
            Name = "Ajinomoto Arc API",
            Email = string.Empty
            //Url = new Uri("https://twitter.com/spboyer"),
        }
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");


builder.Services.AddDbContext<DataContext>(
    x => x.UseMySql(connectionString, Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.6.5-mariadb"))
                // The following three options help with debugging, but should
                // be changed or removed for production.
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
                );
// configure strongly typed settings object
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

builder.Services.AddTransient<IDomainService, DomainService>();
builder.Services.AddScoped<IPaymentFacade, PaymentFacade>();
builder.Services.AddScoped<IBpkFacade, BpkFacade>();
builder.Services.AddScoped<IKpiFacade, KpiFacade>();
builder.Services.AddScoped<IUserFacade, UserFacade>();
builder.Services.AddScoped<IDropdownFacade, DropdownFacade>();
builder.Services.AddScoped<IReportFacade, ReportFacade>();
builder.Services.AddScoped<IAdminFacade, AdminFacade>();
builder.Services.AddScoped<IJwtUtils, JwtUtils>(); 
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IIncomingPaymentService, IncomingPaymentService>();
builder.Services.AddScoped<IBpkService, BpkService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IMasterDataService, MasterDataService>();
builder.Services.AddScoped<IKpiService, KpiService>();
builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IConfigFacade, ConfigFacade> ();
builder.Services.AddScoped<IConfigService, ConfigService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IHistoryService, HistoryService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddHttpContextAccessor();


builder.Services.AddCors(option =>
{
    option.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

// global error handler
app.UseMiddleware<ErrorHandlerMiddleware>();

//app.UseAuthorization();

// custom jwt auth middleware
app.UseMiddleware<JwtMiddleware>();

app.MapControllers();

app.Run();
