using System.Text.Json.Serialization;
using Claims.Common.Constants;
using Claims.Common.Extensions;
using Claims.Data;
using Claims.Middleware;
using Claims.Services;
using Claims.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

builder.Services.AddDataAccess(builder.Configuration);
builder.Services.AddScoped<IClaimService, ClaimService>();
builder.Services.AddScoped<ICoverService, CoverService>();

var app = builder.Build();

var applyMigrationsOnStartup = app.Configuration.GetValue("Database:ApplyMigrationsOnStartup", true);
if (applyMigrationsOnStartup)
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ClaimsDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks(ApiConstants.HealthRoute);

await app.RunAsync();

public partial class Program { }
