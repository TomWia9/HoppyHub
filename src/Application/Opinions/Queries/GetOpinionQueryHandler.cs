using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Opinions.Dtos;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Opinions.Queries;

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
    ///     The mapper
    /// </summary>
    private readonly IMapper _mapper;
    
    /// <summary>
    ///     Initializes GetOpinionQueryHandler.
    /// </summary>
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
        var opinion = await _context.Opinions
            .FindAsync(new object?[] { request.Id }, cancellationToken);

        if (opinion == null)
        {
            throw new NotFoundException(nameof(Opinion), request.Id);
        }

        //TODO Get username
        return _mapper.Map<OpinionDto>(opinion);
    }
}