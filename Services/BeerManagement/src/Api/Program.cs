using Api;
using Application;
using Application.Common.Interfaces;
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
        c.SwaggerEndpoint("swagger/BeerManagementSpecification/swagger.json", "HoppyHub - Beer Management");
        c.RoutePrefix = string.Empty;
    });
}

app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

try
{
    Log.Information("Starting HoppyHub - BeerManagement service");

    using (var scope = app.Services.CreateScope())
    {
        var initializer = scope.ServiceProvider.GetRequiredService<IApplicationDbContextInitializer>();
        await initializer.InitializeAsync();
    }

    app.Run();
}
catch (Exception e)
{
    Log.Fatal(e, "HoppyHub - BeerManagement service terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}