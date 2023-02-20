using Api;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();
builder.Services.AddApiServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

try
{
    Log.Information("Starting Hoppy Hub");
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