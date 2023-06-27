using System.Reflection;
using AutoMapper;

namespace Application.Common.Mappings;

/// <summary>
///     MappingProfile class.
///     Creates maps based on IMapFrom interface.
/// </summary>
public class MappingProfile : Profile
{
    /// <summary>
    ///     Initializes MappingProfile.
    /// </summary>
    public MappingProfile()
    {
        ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly());
    }

    /// <summary>
    ///     Applies based on IMapFrom interface mappings from assembly.
    /// </summary>
    /// <param name="assembly">The assembly</param>
    private void ApplyMappingsFromAssembly(Assembly assembly)
    {
        const string mappingMethodName = nameof(IMapFrom<object>.Mapping);

        var mapFromType = typeof(IMapFrom<>);

        bool HasInterface(Type t) => t.IsGenericType && t.GetGenericTypeDefinition() == mapFromType;

        var types = assembly.GetExportedTypes().Where(t => t.GetInterfaces().Any(HasInterface)).ToList();

        var argumentTypes = new[] { typeof(Profile) };

        foreach (var type in types)
        {
            var instance = Activator.CreateInstance(type);

            var methodInfo = type.GetMethod(mappingMethodName);

            if (methodInfo is not null)
            {
                methodInfo.Invoke(instance, new object[] { this });
            }
            else
            {
                var interfaces = type.GetInterfaces().Where(HasInterface).ToList();

                if (interfaces.Count > 0)
                {
                    foreach (var @interface in interfaces)
                    {
                        var interfaceMethodInfo = @interface.GetMethod(mappingMethodName, argumentTypes);

                        interfaceMethodInfo?.Invoke(instance, new object[] { this });
                    }
                }
            }
        }
    }
}