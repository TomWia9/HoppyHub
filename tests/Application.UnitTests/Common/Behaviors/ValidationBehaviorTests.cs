using Application.Common.Behaviors;
using Application.UnitTests.TestHelpers;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;
using ValidationException = Application.Common.Exceptions.ValidationException;

namespace Application.UnitTests.Common.Behaviors;

/// <summary>
///     Unit tests for the <see cref="ValidationBehavior{TRequest,TResponse}" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class ValidationBehaviorTests
{
    /// <summary>
    ///     The request handler delegate mock.
    /// </summary>
    private readonly Mock<RequestHandlerDelegate<TestResponse>> _requestHandlerDelegateMock;

    /// <summary>
    ///     The validator mock.
    /// </summary>
    private readonly Mock<IValidator<TestRequest>> _validatorMock;

    /// <summary>
    ///     The validators.
    /// </summary>
    private IEnumerable<IValidator<TestRequest>> _validators;

    /// <summary>
    ///     Setups ValidationBehaviorTests.
    /// </summary>
    public ValidationBehaviorTests()
    {
        _validatorMock = new Mock<IValidator<TestRequest>>();
        _requestHandlerDelegateMock = new Mock<RequestHandlerDelegate<TestResponse>>();
        _validators = new List<IValidator<TestRequest>>();
    }

    /// <summary>
    ///     Tests that Handle method with no validators returns next.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnNext_WhenNoValidators()
    {
        // Arrange
        var request = new TestRequest();
        var cancellationToken = new CancellationToken();
        var validationBehavior = new ValidationBehavior<TestRequest, TestResponse>(_validators);

        // Act
        await validationBehavior.Handle(request, _requestHandlerDelegateMock.Object, cancellationToken);

        // Assert
        _requestHandlerDelegateMock.Verify(next => next(), Times.Once);
    }

    /// <summary>
    ///     Tests that Handle method with validators when validator pass returns next.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnNext_WhenValidatorsPass()
    {
        // Arrange
        _validatorMock.Setup(x =>
                x.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _validators = new List<IValidator<TestRequest>> { _validatorMock.Object };
        var validationBehavior = new ValidationBehavior<TestRequest, TestResponse>(_validators);
        var request = new TestRequest();
        var cancellationToken = new CancellationToken();

        // Act
        await validationBehavior.Handle(request, _requestHandlerDelegateMock.Object, cancellationToken);

        // Assert
        _requestHandlerDelegateMock.Verify(next => next(), Times.Once);
        _validatorMock.Verify(
            v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    ///     Tests that Handle method with validators when validator fail throws ValidationException.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenValidatorsFail()
    {
        // Arrange
        _validatorMock.Setup(x =>
                x.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new List<ValidationFailure>
            {
                new("TestProperty", "Test Error message")
            }));
        _validators = new List<IValidator<TestRequest>> { _validatorMock.Object };
        var validationBehavior = new ValidationBehavior<TestRequest, TestResponse>(_validators);
        var request = new TestRequest();
        var cancellationToken = new CancellationToken();

        // Act
        Func<Task> action = async () =>
            await validationBehavior.Handle(request, _requestHandlerDelegateMock.Object, cancellationToken);

        // Assert
        await action.Should().ThrowAsync<ValidationException>();
        _requestHandlerDelegateMock.Verify(next => next(), Times.Never);
        _validatorMock.Verify(
            x => x.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}