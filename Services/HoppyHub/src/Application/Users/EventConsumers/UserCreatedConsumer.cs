using Application.Common.Interfaces;
using Domain.Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedEvents;

namespace Application.Users.EventConsumers;

/// <summary>
///     UserCreated consumer.
/// </summary>
public class UserCreatedConsumer : IConsumer<UserCreated>
{
    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;

    /// <summary>
    ///     Initializes UserCreatedConsumer.
    /// </summary>
    /// <param name="context">The database context</param>
    public UserCreatedConsumer(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    ///     Consumes UserCreated event.
    /// </summary>
    /// <param name="context">The consume context</param>
    public async Task Consume(ConsumeContext<UserCreated> context)
    {
        var message = context.Message;

        var user = new User
        {
            Id = message.Id,
            Username = message.Username,
            Role = message.Role
        };

        //TODO user validation
        if (!await _context.Users.AnyAsync(x => x.Id == user.Id))
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync(CancellationToken.None);
        }
    }
}