global using AutoMapper;
global using F23.StringSimilarity;
global using Microsoft.EntityFrameworkCore;
global using MimeKit;
global using PortfolioBackend.Dtos.ContactDtos;
global using PortfolioBackend.Dtos.EmailDtos;
global using PortfolioBackend.Dtos.UserDtos;
global using PortfolioBackend.Exceptions;
global using PortfolioBackend.Helpers;
global using PortfolioBackend.Models.ContactModel;
global using PortfolioBackend.Models.EmailModel;
global using PortfolioBackend.Models.UserModel;
global using PortfolioBackend.Services.ContactService;
global using PortfolioBackend.Services.EmailService;
global using PortfolioBackend.Services.UserService;
global using Serilog;
using Asp.Versioning;
using Azure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog.Formatting.Json;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


if (builder.Environment.IsProduction())
{
    //Add Azure Key Vault to config
    var vaultUri = builder.Configuration.GetSection("AzureKeyVault").Value!;
    var keyVaultEndpoint = new Uri(vaultUri);
    builder.Configuration.AddAzureKeyVault(keyVaultEndpoint, new DefaultAzureCredential());
}
else
{
    //Add User Secrets to config
    builder.Configuration.AddEnvironmentVariables().AddUserSecrets(Assembly.GetExecutingAssembly(), true);
}

// Add Serilog to the logging pipeline
builder.Host.UseSerilog((context, lc) =>
    lc.Enrich.WithCorrelationIdHeader("Correlation-ID")
      .Enrich.FromLogContext().WriteTo.File(new JsonFormatter(), Constants.LOGGING_ADDRESS).WriteTo.Console());

// Add services to the container.
builder.Services.AddHttpContextAccessor();
var connectionString = builder.Configuration["AzureMySqlDb"];
builder.Services.AddDbContext<UserContext>(options =>
{
    if (builder.Environment.IsProduction())
    {
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    }
    else
    {
        options.UseInMemoryDatabase("TestingUserDB");
    }
});
builder.Services.AddDbContext<ContactContext>(options =>
{
    if (builder.Environment.IsProduction())
    {
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    }
    else
    {
        options.UseInMemoryDatabase("TestingContactDB");
    }
});
builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(Program), typeof(AutoMapperProfile));
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddHealthChecks()
                    .AddDbContextCheck<UserContext>(name: "UserCheck", tags: new[] { "ServiceCheck" })
                    .AddDbContextCheck<ContactContext>(name: "ContactCheck", tags: new[] { "ServiceCheck" })
                    .AddCheck<ApiHealthCheck>(name: "ApiHealthCheck", tags: new[] { "SuperUserCheck" });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "PortfolioWebsiteBackend", Version = "1.0" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: Bearer <token>",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer",
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Id = "Bearer",
                        Type = ReferenceType.SecurityScheme,
                    },
                },
                Array.Empty<string>()
            },
        });
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateAudience = false,
            ValidateIssuer = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTSecurityKey"]!)),
        };
    });

builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
});
builder.Services.AddApplicationInsightsTelemetry(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/healthcheck");

app.Run();

public partial class Program { };