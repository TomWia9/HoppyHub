using Application.Common.Interfaces;
using MassTransit;
using SharedEvents.Events;

namespace Application.Users.EventConsumers;

/// <summary>
///     UserDeleted consumer.
/// </summary>
public class UserDeletedConsumer : IConsumer<UserDeleted>
{
    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;

    /// <summary>
    ///     Initializes UserDeletedConsumer.
    /// </summary>
    /// <param name="context">The database context</param>
    public UserDeletedConsumer(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    ///     Consumes UserDeletedConsumer event.
    /// </summary>
    /// <param name="context">The consume context</param>
    public async Task Consume(ConsumeContext<UserDeleted> context)
    {
        var message = context.Message;
        var user = await _context.Users.FindAsync(message.Id);

        if (user != null)
        {
            user.Deleted = true;
            await _context.SaveChangesAsync(CancellationToken.None);
        }
    }
}