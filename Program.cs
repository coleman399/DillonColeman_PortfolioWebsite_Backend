global using AutoMapper;
global using Microsoft.EntityFrameworkCore;
global using PortfolioWebsite_Backend.Dtos.ContactDtos;
global using PortfolioWebsite_Backend.Dtos.EmailDtos;
global using PortfolioWebsite_Backend.Dtos.UserDtos;
global using PortfolioWebsite_Backend.Exceptions;
global using PortfolioWebsite_Backend.Helpers;
global using PortfolioWebsite_Backend.Models.ContactModel;
global using PortfolioWebsite_Backend.Models.EmailModel;
global using PortfolioWebsite_Backend.Models.UserModel;
global using PortfolioWebsite_Backend.Services.ContactService;
global using PortfolioWebsite_Backend.Services.EmailService;
global using PortfolioWebsite_Backend.Services.UserService;
global using System.Text.Json.Serialization;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Formatting.Json;
using System.Reflection;
using System.Text;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add User Secrets to the configuration <- Change this to key vault in production
    builder.Configuration.AddEnvironmentVariables().AddUserSecrets(Assembly.GetExecutingAssembly(), true);

    builder.Services.AddHttpContextAccessor();
    // Add Serilog to the logging pipeline
    builder.Host.UseSerilog((context, lc) => lc
        .Enrich.WithCorrelationIdHeader("Correlation-ID")
            .Enrich.FromLogContext().WriteTo.File(new JsonFormatter(), "C:/Logs/log.txt"));
    // Add services to the container.
    builder.Services.AddDbContext<UserContext>();
    builder.Services.AddDbContext<ContactContext>();
    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "PortfolioWebsite_Backend", Version = "0.0.0.1" });
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
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Security:Keys:JWT"]!)),
            };
        });
    builder.Services.AddAutoMapper(typeof(Program), typeof(AutoMapperProfile));
    builder.Services.AddTransient<IEmailService, EmailService>();
    builder.Services.AddScoped<IContactService, ContactService>();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.Configure<EmailConfiguration>(builder.Configuration.GetSection("EmailConfiguration"));
    builder.Services.AddHealthChecks()
                    .AddDbContextCheck<UserContext>(name: "UserCheck",
                                                    tags: new[] { "ServiceCheck" })
                    .AddDbContextCheck<ContactContext>(name: "ContactCheck",
                                                       tags: new[] { "ServiceCheck" })
                    .AddCheck<ApiHealthCheck>(name: "ApiHealthCheck",
                                              tags: new[] { "SuperUserCheck", "LoggingCheck" });
    // Healthcheck UI doesn't work with MySql yet <- Entity Framework (EF) doesn't like .net 7
    //builder.Services.AddHealthChecksUI().AddMySqlStorage(builder.Configuration["ConnectionStrings:LocalMySqlDb"]!);

    var app = builder.Build();

    //Healthcheck
    app.MapHealthChecks("/healthcheck", new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });


    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();

    app.MapControllers();

    //app.MapHealthChecksUI();

    //app.UseHealthChecksUI(setup =>
    //{
    //    setup.PageTitle = "Health Check";
    //    setup.ApiPath = "/healthcheck";
    //    setup.UIPath = "/healthcheck-ui";
    //});

    app.Run();
}
catch (Exception exception)
{
    Log.Fatal(exception, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}


