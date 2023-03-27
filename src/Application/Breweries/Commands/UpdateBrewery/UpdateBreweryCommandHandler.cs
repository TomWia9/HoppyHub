using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Breweries.Commands.UpdateBrewery;

/// <summary>
///     UpdateBreweryCommand handler.
/// </summary>
public class UpdateBreweryCommandHandler : IRequestHandler<UpdateBreweryCommand>
{
    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;

    /// <summary>
    ///     Initializes UpdateBreweryCommandHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    public UpdateBreweryCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    ///     Handles UpdateBreweryCommand.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task Handle(UpdateBreweryCommand request, CancellationToken cancellationToken)
    {
        var entity =
            await _context.Breweries.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Brewery), request.Id);
        }

        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.FoundationYear = request.FoundationYear;
        entity.WebsiteUrl = request.WebsiteUrl;

        if (entity.Address != null)
        {
            entity.Address.City = request.City;
            entity.Address.Street = request.Street;
            entity.Address.Number = request.Number;
            entity.Address.PostCode = request.PostCode;
            entity.Address.Country = request.Country;
            entity.Address.State = request.State;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}