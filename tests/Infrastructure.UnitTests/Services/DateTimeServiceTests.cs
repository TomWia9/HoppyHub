using System.Diagnostics.CodeAnalysis;
using Application.Common.Interfaces;
using Infrastructure.Services;
using Moq;

namespace Infrastructure.UnitTests.Services;

/// <summary>
///     Tests for the <see cref="DateTimeService"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class DateTimeServiceTests
{
    /// <summary>
    ///     DateTimeService mock.
    /// </summary>
    private readonly Mock<IDateTime> _dateTimeService;

    /// <summary>
    ///     Setups DateTimeServiceTests.
    /// </summary>
    public DateTimeServiceTests()
    {
        _dateTimeService = new Mock<IDateTime>();
    }

    /// <summary>
    ///     Tests that the Now property returns the current date time.
    /// </summary>
    [Fact]
    public void Now_ReturnsCurrentDateTime()
    {
        // Arrange
        var expected = DateTime.Now;
        _dateTimeService.Setup(x => x.Now).Returns(expected);

        // Act
        var result = _dateTimeService.Object.Now;

        // Assert
        result.Should().Be(expected);
    }
}