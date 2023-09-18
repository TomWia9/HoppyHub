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

if (builder.Environment.IsProduction())
{
    builder.Configuration.AddAzureKeyVault(
        new Uri($"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/"),
        new DefaultAzureCredential());
}

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApiServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("swagger/HoppyHubSpecification/swagger.json", "Hoppy Hub");
        c.RoutePrefix = string.Empty;
    });
}

app.UseSerilogRequestLogging();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

try
{
    Log.Information("Starting Hoppy Hub");

    using (var scope = app.Services.CreateScope())
    {
        var initializer = scope.ServiceProvider.GetRequiredService<IApplicationDbContextInitializer>();
        await initializer.InitializeAsync();

        if (app.Environment.IsDevelopment())
        {
            await initializer.SeedAsync();
        }
    }

    app.Run();
}
catch (Exception e)
{
    Log.Fatal(e, "Hoppy Hub terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}