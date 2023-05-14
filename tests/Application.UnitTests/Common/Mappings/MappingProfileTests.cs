﻿using System.Runtime.Serialization;
using Application.Beers.Dtos;
using Application.BeerStyles.Dtos;
using Application.Breweries.Dtos;
using Application.Common.Mappings;
using Application.PrimaryBeerStyles.Dtos;
using AutoMapper;
using Domain.Entities;

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
    public void Mapper_Should_HaveValidConfiguration()
    {
        _configuration.AssertConfigurationIsValid();
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
    [InlineData(typeof(PrimaryBeerStyle), typeof(PrimaryBeerStyleDto))]
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
        if (type.GetConstructor(Type.EmptyTypes) != null)
            return Activator.CreateInstance(type)!;

        // Type without parameterless constructor
        return FormatterServices.GetUninitializedObject(type);
    }
}