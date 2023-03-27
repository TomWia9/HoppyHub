using Application.Breweries.Dtos;
using Application.Common.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Breweries.Commands.CreateBrewery;

/// <summary>
///     CreateBreweryCommand handler
/// </summary>
public class CreateBreweryCommandHandler : IRequestHandler<CreateBreweryCommand, BreweryDto>
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
    ///     Setups CreateBreweryCommandHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="mapper">The mapper</param>
    public CreateBreweryCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    ///     Handles CreateBreweryCommand.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task<BreweryDto> Handle(CreateBreweryCommand request, CancellationToken cancellationToken)
    {
        var entity = new Brewery
        {
            Name = request.Name,
            Description = request.Description,
            FoundationYear = request.FoundationYear,
            WebsiteUrl = request.WebsiteUrl,
            Address = new Address
            {
                Street = request.Street,
                Number = request.Street,
                PostCode = request.Street,
                City = request.Street,
                State = request.Street,
                Country = request.Street
            }
        };

        await _context.Breweries.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var breweryDto = _mapper.Map<BreweryDto>(entity);

        return breweryDto;
    }
}