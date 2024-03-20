using Api;
using Application;
using Application.Common.Interfaces;
using Azure.Identity;
using Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApiServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("swagger/OpinionManagementSpecification/swagger.json", "HoppyHub - Opinion Management");
        c.RoutePrefix = string.Empty;
    });
    app.UseCors("AngularApp");
}
else
{
    builder.Configuration.AddAzureKeyVault(
        new Uri($"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/"),
        new DefaultAzureCredential());
}

app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseHealthChecks("/health");

try
{
    Log.Information("Starting HoppyHub - Opinion Management service");

    using (var scope = app.Services.CreateScope())
    {
        var initializer = scope.ServiceProvider.GetRequiredService<IApplicationDbContextInitializer>();
        await initializer.InitializeAsync();
    }

    app.Run();
}
catch (Exception e)
{
    Log.Fatal(e, "HoppyHub - OpinionManagement terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}