using Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;

namespace Application.UnitTests.TestHelpers;

/// <summary>
///     Database facade mock.
/// </summary>
public class MockDatabaseFacade : DatabaseFacade
{
    /// <summary>
    ///     Initializes MockDatabaseFacade.
    /// </summary>
    /// <param name="context">The database context</param>
    public MockDatabaseFacade(IApplicationDbContext context) : base((context as DbContext)!)
    {
    }

    /// <summary>
    ///     BeginTransactionAsync mock override. 
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    public override Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(Mock.Of<IDbContextTransaction>());
}