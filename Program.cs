global using AutoMapper;
global using DillonColeman_PortfolioWebsite.Dtos.ContactDtos;
global using DillonColeman_PortfolioWebsite.Models.ContactModel;
global using DillonColeman_PortfolioWebsite.Services.ContactService;
global using Microsoft.EntityFrameworkCore;
global using PortfolioWebsite_Backend.Exceptions;
global using PortfolioWebsite_Backend.Helpers;
global using System.Text.Json.Serialization;
using DillonColeman_PortfolioWebsite;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Configuration.AddEnvironmentVariables().AddUserSecrets(Assembly.GetExecutingAssembly(), true);
builder.Services.AddDbContext<ContactContext>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Program), typeof(AutoMapperProfile));
builder.Services.AddScoped<IContactService, ContactService>();

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
