using Cycle_App.Data;
using Cycle_App.Services.WhatsApp;
using CycleMaintenance.Services.Authentication;
using FastEndpoints;
using FastEndpoints.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFastEndpoints();
builder.Services.AddSwaggerDocument();
builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
builder.Services.AddScoped<IWhatsAppService, WhatsAppService>();
builder.Services.AddScoped<IPasswordHasher, Argon2PasswordHasher>();
builder.Services.AddScoped<IAccountActivationService, AccountActivationService>();


var app = builder.Build();

app.UseSwaggerGen();

app.UseFastEndpoints();

app.Run();