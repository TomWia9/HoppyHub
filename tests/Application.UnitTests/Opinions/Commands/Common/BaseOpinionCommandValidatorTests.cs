using Application.Opinions.Commands.Common;
using FluentValidation.TestHelper;

namespace Application.UnitTests.Opinions.Commands.Common;

/// <summary>
///     Unit tests for the <see cref="BaseOpinionCommandValidator{TCommand}"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class BaseOpinionCommandValidatorTests
{
    /// <summary>
    ///     The TestBaseOpinion command.
    /// </summary>
    private record TestBaseOpinionCommand : BaseOpinionCommand;

    /// <summary>
    ///     The TestBaseOpinionCommand validator.
    /// </summary>
    private class TestBaseOpinionCommandValidator : BaseOpinionCommandValidator<TestBaseOpinionCommand>
    {
    }

    /// <summary>
    ///     The TestBaseOpinionCommand validator instance.
    /// </summary>
    private readonly TestBaseOpinionCommandValidator _validator;

    /// <summary>
    ///     Setups BaseOpinionCommandValidatorTests.
    /// </summary>
    public BaseOpinionCommandValidatorTests()
    {
        _validator = new TestBaseOpinionCommandValidator();
    }

    /// <summary>
    ///     Tests that validation should not have error for Rating when Rating is valid.
    /// </summary>
    [Fact]
    public void BaseOpinionCommand_ShouldNotHaveValidationErrorForRating_WhenRatingIsValid()
    {
        // Arrange
        var command = new TestBaseOpinionCommand
        {
            Rating = 5
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Rating);
    }

    /// <summary>
    ///     Tests that validation should have error for Rating when Rating is out of range.
    /// </summary>
    [Theory]
    [InlineData(-1)]
    [InlineData(11)]
    public void BaseOpinionCommand_ShouldHaveValidationErrorForRating_WhenRatingIsOutOfRange(
        int rating)
    {
        // Arrange
        var command = new TestBaseOpinionCommand
        {
            Rating = rating
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Rating);
    }

    /// <summary>
    ///     Tests that validation should not have error for Comment when Comment is valid.
    /// </summary>
    [Fact]
    public void BaseOpinionCommand_ShouldNotHaveValidationErrorForComment_WhenCommentIsValid()
    {
        // Arrange
        var command = new TestBaseOpinionCommand
        {
            Comment = "Test description"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Comment);
    }

    /// <summary>
    ///     Tests that validation should have error for Comment when Comment exceeds maximum length.
    /// </summary>
    [Fact]
    public void BaseOpinionCommand_ShouldHaveValidationErrorForComment_WhenCommentExceedsMaximumLength()
    {
        // Arrange
        var command = new TestBaseOpinionCommand
        {
            Comment = new string('x', 1001)
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Comment);
    }
}