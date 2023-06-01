using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Opinions.Dtos;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Opinions.Commands.CreateOpinion;

/// <summary>
///     CreateOpinionCommand handler.
/// </summary>
public class CreateOpinionCommandHandler : IRequestHandler<CreateOpinionCommand, OpinionDto>
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
    ///     Initializes CreateOpinionCommandHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="mapper">The mapper</param>
    /// <param name="usersService">The users service</param>
    public CreateOpinionCommandHandler(IApplicationDbContext context, IMapper mapper, IUsersService usersService)
    {
        _context = context;
        _mapper = mapper;
        _usersService = usersService;
    }

    /// <summary>
    ///     Handles CreateOpinionCommand.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task<OpinionDto> Handle(CreateOpinionCommand request, CancellationToken cancellationToken)
    {
        if (!await _context.Beers.AnyAsync(x => x.Id == request.BeerId, cancellationToken))
        {
            throw new NotFoundException(nameof(Beer), request.BeerId);
        }
        
        var entity = new Opinion
        {
            Rating = request.Rating,
            Comment = request.Comment,
            BeerId = request.BeerId
        };

        await _context.Opinions.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var opinionDto = _mapper.Map<OpinionDto>(entity);
        opinionDto.Username = await _usersService.GetUsernameAsync(opinionDto.CreatedBy!.Value);

        return opinionDto;
    }
}