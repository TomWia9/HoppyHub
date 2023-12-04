using MediatR;

namespace Application.UnitTests.TestHelpers;

/// <summary>
///     TestRequest record.
/// </summary>
[ExcludeFromCodeCoverage]
public record TestRequest : IRequest<TestResponse>;