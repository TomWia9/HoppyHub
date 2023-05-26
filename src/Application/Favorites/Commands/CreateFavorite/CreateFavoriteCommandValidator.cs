using Application.Common.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Favorites.Commands.CreateFavorite;

/// <summary>
///     CreateFavoriteCommand validator.
/// </summary>
public class CreateFavoriteCommandValidator : AbstractValidator<CreateFavoriteCommand>
{
    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;

    /// <summary>
    ///     The current user service.
    /// </summary>
    private readonly ICurrentUserService _currentUserService;

    /// <summary>
    ///     Initializes CreateOpinionCommandValidator.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="currentUserService">The current user service</param>
    public CreateFavoriteCommandValidator(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
        _context = context;

        RuleFor(x => x.BeerId).MustAsync(BeSingleFavoritePerBeer)
            .WithMessage("Only one opinion per beer is allowed.");
    }

    /// <summary>
    ///     The custom rule indicating whether user is adding single favorite per beer
    ///     (user should not be able to add the same beer to favorites twice).
    /// </summary>
    /// <param name="model">The CreateFavoriteCommand</param>
    /// <param name="beerId">The beer id</param>
    /// <param name="cancellationToken">The cancellation token</param>
    private async Task<bool> BeSingleFavoritePerBeer(CreateFavoriteCommand model, Guid beerId,
        CancellationToken cancellationToken)
    {
        return !await _context.Favorites.AnyAsync(x =>
            x.CreatedBy == _currentUserService.UserId && x.BeerId == beerId, cancellationToken);
    }
}