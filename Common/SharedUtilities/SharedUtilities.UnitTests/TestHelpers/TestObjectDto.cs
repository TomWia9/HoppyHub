using SharedUtilities.Mappings;

namespace SharedUtilities.UnitTests.TestHelpers;

/// <summary>
///     TestObjectDto class.
/// </summary>
[ExcludeFromCodeCoverage]
public record TestObjectDto : IMapFrom<TestObject>
{
    /// <summary>
    ///     The id.
    /// </summary>
    public int Id { get; set; }
}