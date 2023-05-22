using Application.BeerStyles.Dtos;
using Application.Common.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.BeerStyles.Commands.CreateBeerStyle;

/// <summary>
///     CreateBeerStyleCommand handler.
/// </summary>
public class CreateBeerStyleCommandHandler : IRequestHandler<CreateBeerStyleCommand, BeerStyleDto>
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
    ///     Initializes CreateBeerStyleCommandHandler.
    /// </summary>
    public CreateBeerStyleCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    ///     Handles CreateBeerStyleCommand.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task<BeerStyleDto> Handle(CreateBeerStyleCommand request, CancellationToken cancellationToken)
    {
        var entity = new BeerStyle
        {
            Name = request.Name,
            Description = request.Description,
            CountryOfOrigin = request.CountryOfOrigin
        };

        await _context.BeerStyles.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var beerStyleDto = _mapper.Map<BeerStyleDto>(entity);

        return beerStyleDto;
    }
}