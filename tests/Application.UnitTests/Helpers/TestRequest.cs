using MediatR;

namespace Application.UnitTests.Helpers;

/// <summary>
///     TestRequest record.
/// </summary>
[ExcludeFromCodeCoverage]
public record TestRequest : IRequest<TestResponse>
{
}