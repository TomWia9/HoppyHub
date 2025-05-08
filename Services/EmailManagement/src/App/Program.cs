using System.Reflection;
using Azure.Identity;
using MassTransit;
using Serilog;
using SharedUtilities.Filters;

var builder = Host.CreateApplicationBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Services.AddSerilog();
builder.Services.AddMassTransit(x =>
{
    x.AddConsumers(Assembly.GetExecutingAssembly());

    if (builder.Environment.IsDevelopment())
    {
        x.UsingRabbitMq((context, cfg) =>
        {
            cfg.Host(builder.Configuration.GetValue<string>("RabbitMQ:Host"), "/", h =>
            {
                h.Username(builder.Configuration.GetValue<string>("RabbitMQ:Username") ?? "");
                h.Password(builder.Configuration.GetValue<string>("RabbitMQ:Password") ?? "");
            });

            cfg.ConfigureEndpoints(context,
                endpointNameFormatter: new DefaultEndpointNameFormatter(prefix: "BeerManagement"));
            cfg.UseConsumeFilter(typeof(MessageValidationFilter<>), context);
        });
    }
    else
    {
        x.UsingAzureServiceBus((context, cfg) =>
        {
            cfg.Host(builder.Configuration.GetConnectionString("AzureServiceBusConnection"));
            cfg.ConfigureEndpoints(context,
                endpointNameFormatter: new DefaultEndpointNameFormatter(prefix: "BeerManagement"));
            cfg.UseConsumeFilter(typeof(MessageValidationFilter<>), context);
        });
    }
});

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