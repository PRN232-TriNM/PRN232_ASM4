using Microsoft.EntityFrameworkCore;
using EVCS.TriNM.Repositories;
using EVCS.TriNM.Repositories.Context;
using EVCS.TriNM.Repositories.Models;
using EVCS.TriNM.Services.Extensions;
using EVCS.TriNM.Services.Interfaces;
using EVCS.TriNM.Services.Implements;
using SoapCore;
using System.ServiceModel.Channels;
using System.Text.Json;
using System.Text.Json.Serialization;
using EVCS.SOAPAPIServices.TriNM.Services;
using EVCS.SOAPAPIServices.TriNM.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Database Configuration
builder.Services.AddDbContext<EVChargingDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repository and Service Registration
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IStationService, StationService>();
builder.Services.AddScoped<IServiceProviders, ServiceProviders>();
builder.Services.AddScoped<IStationSOAPServices, StationSOAPServices>();

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5000", 
                "https://localhost:5001", 
                "http://localhost:5046", 
                "https://localhost:5047", 
                "https://localhost:7176",
                "http://localhost:55124")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// JSON Serializer Configuration
var jsonOptions = new System.Text.Json.JsonSerializerOptions()
{
    ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
    PropertyNameCaseInsensitive = true,
    WriteIndented = true
};

// Swagger Configuration
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.WriteIndented = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo 
    { 
        Title = "EVCS SOAP API Services", 
        Version = "v1",
        Description = "SOAP-based API services for EV Charging Station Management System"
    });
});

builder.Services.AddHttpClient();

// SOAP Services Configuration
builder.Services.AddSoapCore();
builder.Services.AddScoped<IStationSOAPServices, StationSOAPServices>();

// SignalR Configuration
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
});

var app = builder.Build();

// Middleware Configuration
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "EVCS SOAP API V1");
    c.RoutePrefix = "swagger";
});

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.MapControllers();

// SignalR Hub
app.MapHub<StationHub>("/stationHub").RequireCors("AllowAll");

// SOAP Endpoint
app.UseSoapEndpoint<IStationSOAPServices>("/StationService.asmx", new SoapEncoderOptions
{
    MessageVersion = System.ServiceModel.Channels.MessageVersion.Soap11,
    WriteEncoding = System.Text.Encoding.UTF8,
    ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max
});

app.Run();
