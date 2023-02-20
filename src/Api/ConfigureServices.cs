using System.Reflection;
using Api.Services;
using Application.Common.Interfaces;
using Microsoft.OpenApi.Models;

namespace Api;

/// <summary>
///     The ConfigureServices class
/// </summary>
public static class ConfigureServices
{
    /// <summary>
    ///     Adds api project services
    /// </summary>
    /// <param name="services">The services</param>
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(setupAction =>
        {
            setupAction.SwaggerDoc(
                "HoppyHubSpecification",
                new OpenApiInfo
                {
                    Title = "Hoppy Hub",
                    Version = "1",
                    Contact = new OpenApiContact
                    {
                        Email = "tomaszwiatrowski9@gmail.com",
                        Name = "Tomasz Wiatrowski"
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT License",
                        Url = new Uri("https://opensource.org/licenses/MIT")
                    }
                });

            //Collect all referenced projects output XML document file paths  
            var currentAssembly = Assembly.GetExecutingAssembly();
            var xmlDocs = currentAssembly.GetReferencedAssemblies()
                .Union(new[] { currentAssembly.GetName() })
                .Select(a => Path.Combine(Path.GetDirectoryName(currentAssembly.Location) ?? string.Empty,
                    $"{a.Name}.xml"))
                .Where(File.Exists).ToList();

            foreach (var d in xmlDocs)
            {
                setupAction.IncludeXmlComments(d);
            }
        });

        return services;
    }
}