using Infrastructure.Converters;

namespace Infrastructure.UnitTests.Converters;

/// <summary>
///     Tests for the <see cref="DateOnlyConverter" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class DateOnlyConverterTests
{
    /// <summary>
    ///     The converter.
    /// </summary>
    private readonly DateOnlyConverter _converter;

    /// <summary>
    ///     Setups DateOnlyConverterTests.
    /// </summary>
    public DateOnlyConverterTests()
    {
        _converter = new DateOnlyConverter();
    }

    /// <summary>
    ///     Tests that DateOnlyConverter converts DateOnly to DateTime.
    /// </summary>
    [Fact]
    public void DateOnlyConverter_ShouldConvertDateOnlyToDateTime()
    {
        // Arrange
        var dateOnly = new DateOnly(2023, 5, 23);
        var expectedDateTime = new DateTime(2023, 5, 23);

        // Act
        var dateTime = _converter.ConvertToProvider(dateOnly);

        // Assert
        dateTime.Should().Be(expectedDateTime);
    }

    /// <summary>
    ///     Tests that DateOnlyConverter converts DateTime to DateOnly.
    /// </summary>
    [Fact]
    public void DateOnlyConverter_ShouldConvertDateTimeToDateOnly()
    {
        // Arrange
        var dateTime = new DateTime(2023, 5, 23, 0, 0, 0);
        var expectedDateOnly = new DateOnly(2023, 5, 23);

        // Act
        var dateOnly = _converter.ConvertFromProvider(dateTime);

        // Assert
        dateOnly.Should().Be(expectedDateOnly);
    }
}