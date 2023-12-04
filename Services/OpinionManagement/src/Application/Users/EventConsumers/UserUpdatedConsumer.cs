using Application.Common.Interfaces;
using MassTransit;
using SharedEvents.Events;

namespace Application.Users.EventConsumers;

/// <summary>
///     UserUpdated consumer.
/// </summary>
public class UserUpdatedConsumer : IConsumer<UserUpdated>
{
    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;

    /// <summary>
    ///     Initializes UserUpdatedConsumer.
    /// </summary>
    /// <param name="context">The database context</param>
    public UserUpdatedConsumer(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    ///     Consumes UserUpdated event.
    /// </summary>
    /// <param name="context">The consume context</param>
    public async Task Consume(ConsumeContext<UserUpdated> context)
    {
        var message = context.Message;
        var user = await _context.Users.FindAsync(message.Id);

        if (user != null)
        {
            user.Username = message.Username;
            await _context.SaveChangesAsync(CancellationToken.None);
        }
    }
}