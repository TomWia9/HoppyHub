﻿using Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Infrastructure.Persistence;

/// <summary>
///     ApplicationDbContextInitializer class.
/// </summary>
public class ApplicationDbContextInitializer : IApplicationDbContextInitializer
{
    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;

    /// <summary>
    ///     Initializes ApplicationDbContextInitializer.
    /// </summary>
    /// <param name="context">The database context</param>
    public ApplicationDbContextInitializer(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    ///     Initializes database asynchronously.
    /// </summary>
    public async Task InitializeAsync()
    {
        try
        {
            if (_context.Database.IsSqlServer())
            {
                await _context.Database.MigrateAsync();
            }
        }
        catch (Exception e)
        {
            Log.Logger.Error("An error occurred while initializing the database. {E}", e.Message);
            throw;
        }
    }

    /// <summary>
    ///     Seed database asynchronously.
    /// </summary>
    public async Task SeedAsync()
    {
        try
        {
            Log.Logger.Information("Seeding database started");

            //await SeedOpinionsAsync();

            Log.Logger.Information("Seeding database completed");
        }
        catch (Exception e)
        {
            Log.Logger.Error("An error occurred while seeding the database. {E}", e.Message);
            throw;
        }
    }

    /// <summary>
    ///     Seeds opinions asynchronously.
    /// </summary>
    private async Task SeedOpinionsAsync()
    {
        if (!await _context.Opinions.AnyAsync())
        {
            const string tableName = nameof(_context.Opinions);

            await SeedDatabaseFromSql(tableName);
        }
    }

    /// <summary>
    ///     Seeds database from sql file.
    /// </summary>
    /// <param name="tableName">The table name</param>
    private async Task SeedDatabaseFromSql(string tableName)
    {
        Log.Logger.Information("Seeding {TableName}...", tableName);

        var sqlFilePath = "../Infrastructure/Persistence/Data/" + tableName + ".sql";

        if (!File.Exists(sqlFilePath))
        {
            throw new FileNotFoundException($"File not found at {sqlFilePath}");
        }

        try
        {
            var sqlScript = await File.ReadAllTextAsync(sqlFilePath);
            var result = await _context.Database.ExecuteSqlRawAsync(sqlScript);

            if (result > 0)
            {
                Log.Logger.Information("{TableName} successfully seeded", tableName);
            }
            else
            {
                Log.Error("Seeding {TableName} failed", tableName);
            }
        }
        catch
        {
            Log.Error("Seeding {TableName} failed", tableName);
            throw;
        }
    }
}