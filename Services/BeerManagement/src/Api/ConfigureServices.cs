using System.Reflection;
using Api.Filters;
using FluentValidation.AspNetCore;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;
using SharedUtilities.Interfaces;
using SharedUtilities.Services;

namespace Api;

/// <summary>
///     The ConfigureServices class.
/// </summary>
public static class ConfigureServices
{
    /// <summary>
    ///     Adds api project services.
    /// </summary>
    /// <param name="services">The services</param>
    public static void AddApiServices(this IServiceCollection services)
    {
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddControllers(options => { options.Filters.Add<ApiExceptionFilterAttribute>(); });
        services.AddEndpointsApiExplorer();
        services.AddHttpContextAccessor();
        services.AddFluentValidationClientsideAdapters();
        services.AddFluentValidationRulesToSwagger();
        services.AddCors(options =>
        {
            options.AddPolicy("AngularApp", builder =>
            {
                builder.WithOrigins("https://happy-sky-0e76f8203-92.westeurope.5.azurestaticapps.net/").AllowAnyHeader()
                    .AllowAnyMethod().WithExposedHeaders("X-Pagination");
            });
        });
        services.AddSwaggerGen(setupAction =>
        {
            setupAction.SwaggerDoc(
                "BeerManagementSpecification",
                new OpenApiInfo
                {
                    Title = "HoppyHub - BeerManagement",
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

            OpenApiSecurityScheme securityDefinition = new()
            {
                Name = "Bearer",
                BearerFormat = "JWT",
                Scheme = "bearer",
                Description = "Specify the authorization token.",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http
            };

            setupAction.AddSecurityDefinition("jwt_auth", securityDefinition);

            OpenApiSecurityScheme securityScheme = new()
            {
                Reference = new OpenApiReference
                {
                    Id = "jwt_auth",
                    Type = ReferenceType.SecurityScheme
                }
            };
            OpenApiSecurityRequirement securityRequirements = new()
            {
                { securityScheme, Array.Empty<string>() }
            };

            setupAction.AddSecurityRequirement(securityRequirements);

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
    }
}