using Serilog;

var builder = Host.CreateApplicationBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Services.AddSerilog();

var host = builder.Build();

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