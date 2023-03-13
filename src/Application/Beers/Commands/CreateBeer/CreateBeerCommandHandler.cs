using Application.Common.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Beers.Commands.CreateBeer;

/// <summary>
///     CreateBeerCommand handler.
/// </summary>
public class CreateBeerCommandHandler : IRequestHandler<CreateBeerCommand, BeerDto>
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
    ///     Initializes CreateBeerCommandHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="mapper">The mapper</param>
    public CreateBeerCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    ///     Handles CreateBeerCommand.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    public async Task<BeerDto> Handle(CreateBeerCommand request, CancellationToken cancellationToken)
    {
        var entity = new Beer
        {
            Name = request.Name,
            Brewery = request.Brewery,
            AlcoholByVolume = request.AlcoholByVolume,
            Description = request.Description,
            SpecificGravity = request.SpecificGravity,
            Blg = request.Blg,
            Plato = request.Plato,
            Style = request.Style,
            Ibu = request.Ibu,
            Country = request.Country
        };

        _context.Beers.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        var beerDto = _mapper.Map<BeerDto>(entity);

        return beerDto;
    }
}