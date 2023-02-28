using Application.Common.Mappings;
using AutoMapper;

namespace Application.UnitTests.Common.Mappings;

/// <summary>
///     Unit tests for the <see cref="MappingProfile"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class MappingTests
{
    /// <summary>
    ///     The configuration.
    /// </summary>
    private readonly IConfigurationProvider _configuration;
    
    /// <summary>
    ///     The mapper.
    /// </summary>
    private readonly IMapper _mapper;

    /// <summary>
    ///     Setups MappingTests.
    /// </summary>
    public MappingTests()
    {
        _configuration = new MapperConfiguration(config =>
            config.AddProfile<MappingProfile>());

        _mapper = _configuration.CreateMapper();
    }

    /// <summary>
    ///     Tests that configuration is valid.
    /// </summary>
    [Fact]
    public void ShouldHaveValidConfiguration()
    {
        _configuration.AssertConfigurationIsValid();
    }

    //TODO Add when entities will be added
    // [Theory]
    // [InlineData(typeof(Beer), typeof(BeerDto))]
    // public void ShouldSupportMappingFromSourceToDestination(Type source, Type destination)
    // {
    //     var instance = GetInstanceOf(source);
    //
    //     _mapper.Map(instance, source, destination);
    // }

    // private object GetInstanceOf(Type type)
    // {
    //     if (type.GetConstructor(Type.EmptyTypes) != null)
    //         return Activator.CreateInstance(type)!;
    //
    //     // Type without parameterless constructor
    //     return FormatterServices.GetUninitializedObject(type);
    // }
}