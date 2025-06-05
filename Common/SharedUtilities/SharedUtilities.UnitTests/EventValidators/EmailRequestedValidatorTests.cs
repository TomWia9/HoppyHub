using FluentValidation.TestHelper;
using SharedEvents.Events;
using SharedUtilities.EventValidators;

namespace SharedUtilities.UnitTests.EventValidators;

/// <summary>
///     Unit tests for the <see cref="EmailRequestedValidator" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class EmailRequestedValidatorTests
{
    /// <summary>
    ///     The validator.
    /// </summary>
    private readonly EmailRequestedValidator _validator;

    /// <summary>
    ///     Setups EmailRequestedValidatorTests.
    /// </summary>
    public EmailRequestedValidatorTests()
    {
        _validator = new EmailRequestedValidator();
    }

    /// <summary>
    ///     Tests that validation should not have error for Recipient when Recipient is valid.
    /// </summary>
    [Fact]
    public void EmailRequested_ShouldNotHaveValidationErrorForRecipient_WhenRecipientIsValid()
    {
        // Arrange
        var emailRequested = new EmailRequested
        {
            Recipient = "test@localhost"
        };

        // Act
        var result = _validator.TestValidate(emailRequested);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Recipient);
    }

    /// <summary>
    ///     Tests that validation should have error for Recipient when Recipient is empty.
    /// </summary>
    [Fact]
    public void EmailRequested_ShouldHaveValidationErrorForRecipient_WhenRecipientIsEmpty()
    {
        // Arrange
        var emailRequested = new EmailRequested
        {
            Recipient = string.Empty
        };

        // Act
        var result = _validator.TestValidate(emailRequested);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Recipient);
    }

    /// <summary>
    ///     Tests that validation should have error for Recipient when Recipient is not valid email address.
    /// </summary>
    [Fact]
    public void EmailRequested_ShouldHaveValidationErrorForRecipient_WhenRecipientIsNotValidEmailAddress()
    {
        // Arrange
        var emailRequested = new EmailRequested
        {
            Recipient = "test"
        };

        // Act
        var result = _validator.TestValidate(emailRequested);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Recipient);
    }

    /// <summary>
    ///     Tests that validation should not have error for Subject when Subject is valid.
    /// </summary>
    [Fact]
    public void EmailRequested_ShouldNotHaveValidationErrorForSubject_WhenSubjectIsValid()
    {
        // Arrange
        var emailRequested = new EmailRequested
        {
            Subject = new string('x', 20)
        };

        // Act
        var result = _validator.TestValidate(emailRequested);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Subject);
    }

    /// <summary>
    ///     Tests that validation should have error for Subject when Subject is empty.
    /// </summary>
    [Fact]
    public void EmailRequested_ShouldHaveValidationErrorForSubject_WhenSubjectIsEmpty()
    {
        // Arrange
        var emailRequested = new EmailRequested
        {
            Subject = string.Empty
        };

        // Act
        var result = _validator.TestValidate(emailRequested);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Subject);
    }

    /// <summary>
    ///     Tests that validation should not have error for Content when Content is valid.
    /// </summary>
    [Fact]
    public void EmailRequested_ShouldNotHaveValidationErrorForContent_WhenContentIsValid()
    {
        // Arrange
        var emailRequested = new EmailRequested
        {
            Content = new string('x', 200)
        };

        // Act
        var result = _validator.TestValidate(emailRequested);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Content);
    }

    /// <summary>
    ///     Tests that validation should have error for Content when Content is empty.
    /// </summary>
    [Fact]
    public void EmailRequested_ShouldHaveValidationErrorForContent_WhenContentIsEmpty()
    {
        // Arrange
        var emailRequested = new EmailRequested
        {
            Content = string.Empty
        };

        // Act
        var result = _validator.TestValidate(emailRequested);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Content);
    }
}