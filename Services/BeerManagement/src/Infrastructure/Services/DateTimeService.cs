using Application.Common.Interfaces;

namespace Infrastructure.Services;

/// <summary>
///     The date time service class
/// </summary>
public class DateTimeService : IDateTime
{
    /// <summary>
    ///     Current date time
    /// </summary>
    public DateTime Now => DateTime.Now;
}