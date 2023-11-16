using Application.Interfaces;
using MassTransit;
using SharedEvents.Events;
using SharedEvents.Responses;

namespace Application.EventConsumers;

/// <summary>
///     ImagesDeleted consumer.
/// </summary>
public class ImagesDeletedConsumer : IConsumer<ImagesDeleted>
{
    /// <summary>
    ///     The images service.
    /// </summary>
    private readonly IImagesService _imagesService;

    /// <summary>
    ///     Initializes ImagesDeletedConsumer.
    /// </summary>
    /// <param name="imagesService">The images service</param>
    public ImagesDeletedConsumer(IImagesService imagesService)
    {
        _imagesService = imagesService;
    }

    /// <summary>
    ///     Consumes ImagesDeleted event.
    /// </summary>
    /// <param name="context">The consume context</param>
    public async Task Consume(ConsumeContext<ImagesDeleted> context)
    {
        var message = context.Message;

        if (context.Message.Paths != null)
        {
            foreach (var path in message.Paths!.ToList())
            {
                await _imagesService.DeleteAllImagesInPathAsync(path);
            }

            var imagesDeletedFromBlobStorageResponse = new ImagesDeletedFromBlobStorage
            {
                Success = true
            };

            await context.RespondAsync(imagesDeletedFromBlobStorageResponse);
        }
    }
}