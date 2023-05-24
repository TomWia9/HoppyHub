using Application.Opinions.Commands.UpdateOpinion;
using FluentValidation.TestHelper;

namespace Application.UnitTests.Opinions.Commands.UpdateOpinion;

/// <summary>
///     Unit tests for the <see cref="UpdateOpinionCommandValidator"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class UpdateOpinionCommandValidatorTests
{
    /// <summary>
    ///     The validator.
    /// </summary>
    private readonly UpdateOpinionCommandValidator _validator;

    /// <summary>
    ///     Initializes UpdateOpinionCommandValidatorTests.
    /// </summary>
    public UpdateOpinionCommandValidatorTests()
    {
        _validator = new UpdateOpinionCommandValidator();
    }

    /// <summary>
    ///     Tests that validation should not have error for Rate when Rate is valid.
    /// </summary>
    [Fact]
    public async Task UpdateOpinionCommand_ShouldNotHaveValidationErrorForRate_WhenRateIsValid()
    {
        // Arrange
        var command = new UpdateOpinionCommand()
        {
            Rate = 5
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Rate);
    }

    /// <summary>
    ///     Tests that validation should have error for Rate when Rate is out of range.
    /// </summary>
    [Theory]
    [InlineData(-1)]
    [InlineData(11)]
    public async Task UpdateOpinionCommand_ShouldHaveValidationErrorForRate_WhenRateIsOutOfRange(
        int rate)
    {
        // Arrange
        var command = new UpdateOpinionCommand
        {
            Rate = rate
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Rate);
    }

    /// <summary>
    ///     Tests that validation should not have error for Comment when Comment is valid.
    /// </summary>
    [Fact]
    public async Task UpdateOpinionCommand_ShouldNotHaveValidationErrorForComment_WhenCommentIsValid()
    {
        // Arrange
        var command = new UpdateOpinionCommand
        {
            Comment = "Test description"
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Comment);
    }

    /// <summary>
    ///     Tests that validation should have error for Comment when Comment exceeds maximum length.
    /// </summary>
    [Fact]
    public async Task UpdateOpinionCommand_ShouldHaveValidationErrorForComment_WhenCommentExceedsMaximumLength()
    {
        // Arrange
        var command = new UpdateOpinionCommand
        {
            Comment = new string('x', 1001)
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Comment);
    }
}