using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Opinions.Dtos;
using AutoMapper;
using Domain.Entities;
using MediatR;

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
    ///     The users service.
    /// </summary>
    private readonly IUsersService _usersService;

    /// <summary>
    ///     Initializes GetOpinionQueryHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="mapper">The mapper</param>
    /// <param name="usersService">The users service</param>
    public GetOpinionQueryHandler(IApplicationDbContext context, IMapper mapper, IUsersService usersService)
    {
        _context = context;
        _mapper = mapper;
        _usersService = usersService;
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

        var opinionDto = _mapper.Map<OpinionDto>(opinion);

        if (opinion.CreatedBy == null)
        {
            return opinionDto;
        }

        var username = await _usersService.GetUsernameAsync(opinion.CreatedBy.Value);
        opinionDto.Username = username;

        return opinionDto;
    }
}