using Application.Interfaces;
using MassTransit;
using SharedEvents.Events;

namespace Application.EventConsumers;

/// <summary>
///     ImageDeleted consumer.
/// </summary>
public class ImageDeletedConsumer : IConsumer<ImageDeleted>
{
    /// <summary>
    ///     The images service.
    /// </summary>
    private readonly IImagesService _imagesService;

    /// <summary>
    ///     Initializes ImageDeletedConsumer.
    /// </summary>
    /// <param name="imagesService">The images service</param>
    public ImageDeletedConsumer(IImagesService imagesService)
    {
        _imagesService = imagesService;
    }

    /// <summary>
    ///     Consumes ImageDeleted event.
    /// </summary>
    /// <param name="context">The consume context</param>
    public async Task Consume(ConsumeContext<ImageDeleted> context)
    {
        var message = context.Message;

        if (!string.IsNullOrEmpty(message.Uri))
        {
            await _imagesService.DeleteImageAsync(message.Uri);
        }
    }
}