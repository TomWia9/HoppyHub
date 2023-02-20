using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Infrastructure.Persistence;

/// <summary>
///     ApplicationDbContextInitialiser class
/// </summary>
public class ApplicationDbContextInitialiser
{
    /// <summary>
    ///     The database context
    /// </summary>
    private readonly ApplicationDbContext _context;

    /// <summary>
    ///     Initializes ApplicationDbContextInitialiser
    /// </summary>
    /// <param name="context">The database context</param>
    public ApplicationDbContextInitialiser(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    ///     Initialises database asynchronously 
    /// </summary>
    public async Task InitialiseAsync()
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
            Log.Logger.Error("An error occurred while initialising the database. {E}", e.Message);
            throw;
        }
    }

    /// <summary>
    ///     Seed database asynchronously
    /// </summary>
    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception e)
        {
            Log.Logger.Error("An error occurred while seeding the database. {E}", e.Message);
            throw;
        }
    }

    /// <summary>
    ///     Tries seed database with given values asynchronously
    /// </summary>
    private async Task TrySeedAsync()
    {
        //TODO seed users and roles

        await SeedBeers();
    }

    /// <summary>
    ///     Seeds beers table
    /// </summary>
    private async Task SeedBeers()
    {
        if (!await _context.Beers.AnyAsync())
        {
            Log.Logger.Information("Seeding beers...");

            var beers = new List<Beer>
            {
                new()
                {
                    Id = new Guid(),
                    Name = "Hazy morning"
                },
                new()
                {
                    Id = new Guid(),
                    Name = "PanIPAni"
                }
            };

            await _context.AddRangeAsync(beers);
            await _context.SaveChangesAsync();
        }
    }
}