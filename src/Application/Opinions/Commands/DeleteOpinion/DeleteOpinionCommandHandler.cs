using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Opinions.Commands.DeleteOpinion;

/// <summary>
///     DeleteOpinionCommand handler
/// </summary>
public class DeleteOpinionCommandHandler : IRequestHandler<DeleteOpinionCommand>
{
    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;

    /// <summary>
    ///     Initializes DeleteOpinionCommandHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    public DeleteOpinionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    ///     Handles DeleteOpinionCommand.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task Handle(DeleteOpinionCommand request, CancellationToken cancellationToken)
    {
        var entity =
            await _context.BeerStyles.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Opinion), request.Id);
        }

        _context.BeerStyles.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}