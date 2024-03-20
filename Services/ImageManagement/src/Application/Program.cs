using Application;
using Azure.Identity;
using Serilog;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        if (context.HostingEnvironment.IsProduction())
        {
            var configuration = config.Build();

            config.AddAzureKeyVault(
                new Uri($"https://{configuration["KeyVaultName"]}.vault.azure.net/"),
                new DefaultAzureCredential());
        }
    })
    .ConfigureServices((hostContext, services) =>
    {
        var configuration = hostContext.Configuration;
        services.AddApplicationServices(configuration);
    })
    .UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
        .ReadFrom.Configuration(hostingContext.Configuration))
    .Build();

host.Run();