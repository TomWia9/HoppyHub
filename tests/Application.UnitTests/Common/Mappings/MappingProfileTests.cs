using Application.Common.Mappings;
using AutoMapper;

namespace Application.UnitTests.Common.Mappings;

/// <summary>
///     Unit tests for the <see cref="MappingProfile"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class MappingTests
{
    private readonly IConfigurationProvider _configuration;
    private readonly IMapper _mapper;

    public MappingTests()
    {
        _configuration = new MapperConfiguration(config =>
            config.AddProfile<MappingProfile>());

        _mapper = _configuration.CreateMapper();
    }

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