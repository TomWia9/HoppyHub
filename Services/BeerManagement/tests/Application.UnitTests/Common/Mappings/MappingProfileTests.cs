﻿using System.Runtime.CompilerServices;
using Application.Beers.Dtos;
using Application.BeerStyles.Dtos;
using Application.Breweries.Dtos;
using Application.Common.Mappings;
using AutoMapper;
using Domain.Entities;

namespace Application.UnitTests.Common.Mappings;

/// <summary>
///     Unit tests for the <see cref="MappingProfile" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class MappingTests
{
    /// <summary>
    ///     The mapper.
    /// </summary>
    private readonly IMapper _mapper;

    /// <summary>
    ///     Setups MappingTests.
    /// </summary>
    public MappingTests()
    {
        IConfigurationProvider configuration = new MapperConfiguration(config =>
            config.AddProfile<MappingProfile>());

        _mapper = configuration.CreateMapper();
    }

    /// <summary>
    ///     Tests that mapping supports mapping from source to destination.
    /// </summary>
    /// <param name="source">The source</param>
    /// <param name="destination">The destination</param>
    [Theory]
    [InlineData(typeof(Beer), typeof(BeerDto))]
    [InlineData(typeof(Brewery), typeof(BreweryDto))]
    [InlineData(typeof(Address), typeof(AddressDto))]
    [InlineData(typeof(BeerStyle), typeof(BeerStyleDto))]
    public void Mapping_Should_SupportMappingFromSourceToDestination(Type source, Type destination)
    {
        // Arrange
        var instance = GetInstanceOf(source);

        // Act
        var dto = _mapper.Map(instance, source, destination);

        // Assert
        dto.Should().BeOfType(destination);
    }

    private static object GetInstanceOf(Type type)
    {
        if (type.GetConstructor(Type.EmptyTypes) is not null)
            return Activator.CreateInstance(type)!;

        // Type without parameterless constructor
        return RuntimeHelpers.GetUninitializedObject(type);
    }
}