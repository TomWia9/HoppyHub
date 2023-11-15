using FluentValidation;
using MassTransit;

namespace SharedUtilities.Filters;

/// <summary>
///     MessageValidationFilter class.
/// </summary>
public class MessageValidationFilter<T> : IFilter<ConsumeContext<T>> where T : class
{
    /// <summary>
    ///     The validators.
    /// </summary>
    private readonly IEnumerable<IValidator<T>> _validators;

    /// <summary>
    ///     Initializes MessageValidationFilter.
    /// </summary>
    /// <param name="validators">The validators</param>
    public MessageValidationFilter(IEnumerable<IValidator<T>> validators)
    {
        _validators = validators;
    }

    /// <summary>
    ///     Validates and sends the message.
    /// </summary>
    /// <param name="context">The consume context</param>
    /// <param name="next">The pipe</param>
    public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
    {
        if (_validators.Any())
        {
            var validationContext = new ValidationContext<T>(context.Message);

            var validationResults = await Task.WhenAll(
                _validators.Select(v =>
                    v.ValidateAsync(validationContext)));

            var failures = validationResults
                .Where(x => x.Errors.Any())
                .SelectMany(x => x.Errors)
                .ToList();

            if (failures.Any())
                throw new ValidationException(failures);
        }

        await next.Send(context);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateFilterScope(nameof(MessageValidationFilter<T>));
    }
}