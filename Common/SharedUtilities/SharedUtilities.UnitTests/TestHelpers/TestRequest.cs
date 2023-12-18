using MediatR;

namespace SharedUtilities.UnitTests.TestHelpers;

/// <summary>
///     TestRequest record.
/// </summary>
[ExcludeFromCodeCoverage]
public record TestRequest : IRequest<TestResponse>;