using Application.Common.Interfaces;
using Application.Opinions.Commands.Common;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Opinions.Commands.CreateOpinion;

/// <summary>
///     CreateOpinionCommand validator.
/// </summary>
public class CreateOpinionCommandValidator : BaseOpinionCommandValidator<CreateOpinionCommand>
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
    public CreateOpinionCommandValidator(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;

        RuleFor(x => x.BeerId).NotEmpty().MustAsync(BeSingleOpinionPerBeer)
            .WithMessage("Only one opinion per beer is allowed.");
    }

    /// <summary>
    ///     The custom rule indicating whether user is adding single opinion per beer.
    /// </summary>
    /// <param name="model">The CreateBeerCommand</param>
    /// <param name="beerId">The beer id</param>
    /// <param name="cancellationToken">The cancellation token</param>
    private async Task<bool> BeSingleOpinionPerBeer(CreateOpinionCommand model, Guid beerId,
        CancellationToken cancellationToken)
    {
        return !await _context.Opinions.AnyAsync(x =>
            x.CreatedBy == _currentUserService.UserId && x.BeerId == beerId, cancellationToken);
    }
}