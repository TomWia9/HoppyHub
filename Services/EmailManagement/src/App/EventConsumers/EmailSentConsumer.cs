using MassTransit;
using SharedEvents.Events;

namespace App.EventConsumers;

/// <summary>
///     EmailSent consumer.
/// </summary>
public class EmailSentConsumer : IConsumer<EmailSent>
{
    /// <summary>
    ///     Consumes BeerOpinionChanged event.
    /// </summary>
    /// <param name="context">The consume context</param>
    public async Task Consume(ConsumeContext<EmailSent> context)
    {
        var message = context.Message;

        //TODO: Send email 
    }
}