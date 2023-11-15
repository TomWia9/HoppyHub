using Application.Interfaces;
using MassTransit;
using SharedEvents;

namespace Application.EventConsumers;

/// <summary>
///     ImageCreated consumer.
/// </summary>
public class ImageCreatedConsumer : IConsumer<ImageCreated>
{
    /// <summary>
    ///     The images service.
    /// </summary>
    private readonly IImagesService _imagesService;

    /// <summary>
    ///     Initializes ImageCreatedConsumer.
    /// </summary>
    /// <param name="imagesService">The images service</param>
    public ImageCreatedConsumer(IImagesService imagesService)
    {
        _imagesService = imagesService;
    }

    /// <summary>
    ///     Consumes ImageCreated event.
    /// </summary>
    /// <param name="context">The consume context</param>
    public async Task Consume(ConsumeContext<ImageCreated> context)
    {
        var message = context.Message;

        if (!string.IsNullOrEmpty(message.Path) && message.Image is not null)
        {
            await _imagesService.UploadImageAsync(message.Path, message.Image);
        }
    }
}