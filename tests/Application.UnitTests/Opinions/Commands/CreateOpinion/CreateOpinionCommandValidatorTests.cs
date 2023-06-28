using Application.Common.Interfaces;
using Application.Opinions.Commands.CreateOpinion;
using Domain.Entities;
using FluentValidation.TestHelper;
using MockQueryable.Moq;
using Moq;

namespace Application.UnitTests.Opinions.Commands.CreateOpinion;

/// <summary>
///     Unit tests for the <see cref="CreateOpinionCommandValidator" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class CreateOpinionCommandValidatorTests
{
    /// <summary>
    ///     The database context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     The validator.
    /// </summary>
    private readonly CreateOpinionCommandValidator _validator;

    /// <summary>
    ///     Setups CreateOpinionCommandValidatorTests.
    /// </summary>
    public CreateOpinionCommandValidatorTests()
    {
        var opinionsDbSetMock = Enumerable.Empty<Opinion>().AsQueryable().BuildMockDbSet();
        _contextMock = new Mock<IApplicationDbContext>();
        Mock<ICurrentUserService> currentUserServiceMock = new();
        _contextMock.Setup(x => x.Opinions).Returns(opinionsDbSetMock.Object);
        _validator = new CreateOpinionCommandValidator(_contextMock.Object, currentUserServiceMock.Object);
    }

    /// <summary>
    ///     Tests that validation should not have error for BeerId when BeerId is valid.
    /// </summary>
    [Fact]
    public async Task CreateOpinionCommand_ShouldNotHaveValidationErrorForBeerId_WhenBeerIdIsValid()
    {
        // Arrange
        var command = new CreateOpinionCommand
        {
            BeerId = Guid.NewGuid()
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.BeerId);
    }

    /// <summary>
    ///     Tests that validation should have error for BeerId when user is adding second opinion for the same beer.
    /// </summary>
    [Fact]
    public async Task
        CreateOpinionCommand_ShouldHaveValidationErrorForBeerId_WhenUserIsAddingSecondOpinionForTheSameBeer()
    {
        // Arrange
        var beerId = Guid.NewGuid();
        var command = new CreateOpinionCommand
        {
            BeerId = beerId,
            Rating = 5
        };
        var opinions = new List<Opinion>
        {
            new()
            {
                BeerId = beerId,
                Rating = 4
            }
        };
        var opinionsDbSetMock = opinions.AsQueryable().BuildMockDbSet();

        _contextMock.Setup(x => x.Opinions).Returns(opinionsDbSetMock.Object);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BeerId).WithErrorMessage("Only one opinion per beer is allowed.");
    }
}