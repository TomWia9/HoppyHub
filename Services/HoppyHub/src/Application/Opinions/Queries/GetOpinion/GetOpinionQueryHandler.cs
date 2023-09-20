using Application.Common.Interfaces;
using Application.Opinions.Dtos;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedUtilities.Exceptions;

namespace Application.Opinions.Queries.GetOpinion;

/// <summary>
///     GetOpinionQuery handler.
/// </summary>
public class GetOpinionQueryHandler : IRequestHandler<GetOpinionQuery, OpinionDto>
{
    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;

    /// <summary>
    ///     The mapper.
    /// </summary>
    private readonly IMapper _mapper;
    

    /// <summary>
    ///     Initializes GetOpinionQueryHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="mapper">The mapper</param>
    public GetOpinionQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    ///     Handles GetOpinionQuery.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task<OpinionDto> Handle(GetOpinionQuery request, CancellationToken cancellationToken)
    {
        var opinion = await _context.Opinions.Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

        if (opinion is null)
        {
            throw new NotFoundException(nameof(Opinion), request.Id);
        }

        var opinionDto = _mapper.Map<OpinionDto>(opinion);
        
        return opinionDto;
    }
}