using App;
using Azure.Identity;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Services.AddServices(builder.Configuration, builder.Environment);

var host = builder.Build();

if (!builder.Environment.IsDevelopment())
{
    builder.Configuration.AddAzureKeyVault(
        new Uri($"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/"),
        new DefaultAzureCredential());
}

try
{
    Log.Information("Starting HoppyHub - EmailManagement service");

    host.Run();
}
catch (Exception e)
{
    Log.Fatal(e, "HoppyHub - EmailManagement service terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}