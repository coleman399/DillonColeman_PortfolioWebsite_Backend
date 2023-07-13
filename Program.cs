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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add User Secrets to the configuration <- Change this to key vault in production
builder.Configuration.AddEnvironmentVariables().AddUserSecrets(Assembly.GetExecutingAssembly(), true);

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddDbContext<UserContext>();
builder.Services.AddDbContext<ContactContext>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "PortfolioWebsite_Backend", Version = "v1" });
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
            ValidateIssuerSigningKey = false,
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


app.Run();
