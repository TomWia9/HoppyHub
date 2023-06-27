using AutoMapper;

namespace Application.Common.Mappings;

/// <summary>
///     MapFrom interface.
/// </summary>
/// <typeparam name="T">The source type</typeparam>
public interface IMapFrom<T>
{
    /// <summary>
    ///     Creates a map.
    /// </summary>
    /// <param name="profile"></param>
    void Mapping(Profile profile)
    {
        profile.CreateMap(typeof(T), GetType());
    }
}