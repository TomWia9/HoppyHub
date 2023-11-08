using FluentValidation;
using FluentValidation.Results;
using MassTransit;
using Moq;
using SharedUtilities.Filters;
using SharedUtilities.UnitTests.TestHelpers;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace SharedUtilities.UnitTests.Filters;

/// <summary>
///     Unit tests for the <see cref="MessageValidationFilter{T}" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class MessageValidationFilterTests
{
    /// <summary>
    ///     The consume context mock.
    /// </summary>
    private readonly Mock<ConsumeContext<TestEvent>> _consumeContextMock;

    /// <summary>
    ///     The next mock.
    /// </summary>
    private readonly Mock<IPipe<ConsumeContext<TestEvent>>> _nextMock;

    /// <summary>
    ///     The validator mock.
    /// </summary>
    private readonly Mock<IValidator<TestEvent>> _validatorMock;

    /// <summary>
    ///     The validators.
    /// </summary>
    private IEnumerable<IValidator<TestEvent>> _validators;

    /// <summary>
    ///     The message validation filter.
    /// </summary>
    private MessageValidationFilter<TestEvent> _messageValidationFilter;

    /// <summary>
    ///     Setups MessageValidationFilterTests.
    /// </summary>
    public MessageValidationFilterTests()
    {
        _validatorMock = new Mock<IValidator<TestEvent>>();
        _consumeContextMock = new Mock<ConsumeContext<TestEvent>>();
        _nextMock = new Mock<IPipe<ConsumeContext<TestEvent>>>();
        _validators = new List<IValidator<TestEvent>>();
        _messageValidationFilter = new MessageValidationFilter<TestEvent>(_validators);
    }

    /// <summary>
    ///     Tests that Send method passes message to next when no validators found for message.
    /// </summary>
    [Fact]
    public async Task Send_ShouldPassMessageToNext_WhenNoValidatorsFoundForMessage()
    {
        // Arrange
        var message = new TestEvent();
        _consumeContextMock.Setup(c => c.Message).Returns(message);

        // Act
        await _messageValidationFilter.Send(_consumeContextMock.Object, _nextMock.Object);

        // Assert
        _nextMock.Verify(p => p.Send(_consumeContextMock.Object), Times.Once);
    }

    /// <summary>
    ///     Tests that Send method passes message to next when validators passes validation.
    /// </summary>
    [Fact]
    public async Task Send_ShouldPassMessageToNext_WhenValidatorsPassesValidation()
    {
        // Arrange
        _validatorMock.Setup(x =>
                x.ValidateAsync(It.IsAny<ValidationContext<TestEvent>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _validators = new List<IValidator<TestEvent>> { _validatorMock.Object };

        // Act
        await _messageValidationFilter.Send(_consumeContextMock.Object, _nextMock.Object);

        // Assert
        _nextMock.Verify(x => x.Send(_consumeContextMock.Object), Times.Once);
    }

    /// <summary>
    ///     Tests that Send method throws ValidationException when validators fail validation.
    /// </summary>
    [Fact]
    public async Task Send_ShouldThrowValidationException_WhenValidatorsFailValidation()
    {
        // Arrange
        _validatorMock.Setup(x =>
                x.ValidateAsync(It.IsAny<ValidationContext<TestEvent>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new List<ValidationFailure>
            {
                new("TestProperty", "Test Error message")
            }));
        _validators = new List<IValidator<TestEvent>> { _validatorMock.Object };
        _messageValidationFilter = new MessageValidationFilter<TestEvent>(_validators);

        // Act
        var act = async () => await _messageValidationFilter.Send(_consumeContextMock.Object, _nextMock.Object);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
        _nextMock.Verify(x => x.Send(_consumeContextMock.Object), Times.Never);
    }
}