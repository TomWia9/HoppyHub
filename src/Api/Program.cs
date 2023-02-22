using Api;
using Application;
using Infrastructure;
using Infrastructure.Persistence;
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
        c.SwaggerEndpoint("swagger/HoppyHubSpecification/swagger.json", "Hoppy Hub");
        c.RoutePrefix = string.Empty;
    });
}

app.UseSerilogRequestLogging();

app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();

try
{
    Log.Information("Starting Hoppy Hub");
    
    using (var scope = app.Services.CreateScope())
    {
        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
        await initialiser.InitialiseAsync();
        await initialiser.SeedAsync();
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