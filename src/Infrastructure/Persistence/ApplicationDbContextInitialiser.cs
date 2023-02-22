using Application.Common.Models;
using Domain.Entities;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Infrastructure.Persistence;

/// <summary>
///     ApplicationDbContextInitialiser class.
/// </summary>
public class ApplicationDbContextInitialiser
{
    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly ApplicationDbContext _context;

    /// <summary>
    ///     Role manager.
    /// </summary>
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    /// <summary>
    ///     User manager.
    /// </summary>
    private readonly UserManager<ApplicationUser> _userManager;

    /// <summary>
    ///     Initializes ApplicationDbContextInitialiser
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="roleManager">The role manager</param>
    /// <param name="userManager">The user manager</param>
    public ApplicationDbContextInitialiser(ApplicationDbContext context, RoleManager<IdentityRole<Guid>> roleManager,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _roleManager = roleManager;
        _userManager = userManager;
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
            Log.Logger.Information("Seeding database started");
            
            await SeedRolesAsync();
            await SeedUsersAsync();
            await SeedBeersAsync();
            
            Log.Logger.Information("Seeding database completed");
        }
        catch (Exception e)
        {
            Log.Logger.Error("An error occurred while seeding the database. {E}", e.Message);
            throw;
        }
    }

    /// <summary>
    ///     Seeds roles asynchronously.
    /// </summary>
    private async Task SeedRolesAsync()
    {
        var administratorRole = new IdentityRole<Guid>(Roles.Administrator);
        var userRole = new IdentityRole<Guid>(Roles.User);

        if (await _roleManager.Roles.AllAsync(x => x.Name != administratorRole.Name))
        {
            Log.Logger.Information("Seeding administrator role...");
            await _roleManager.CreateAsync(administratorRole);
        }

        if (await _roleManager.Roles.AllAsync(x => x.Name != userRole.Name))
        {
            Log.Logger.Information("Seeding user role...");
            await _roleManager.CreateAsync(userRole);
        }
    }

    /// <summary>
    ///     Seeds users asynchronously.
    /// </summary>
    private async Task SeedUsersAsync()
    {
        var administrator = new ApplicationUser
        {
            UserName = "administrator@localhost",
            Email = "administrator@localhost"
        };

        var user = new ApplicationUser()
        {
            UserName = "user@localhost",
            Email = "user@localhost"
        };

        if (_userManager.Users.All(x => x.UserName != administrator.UserName))
        {
            await _userManager.CreateAsync(administrator, "Admin123!");
            await _userManager.AddToRolesAsync(administrator, new[] { Roles.Administrator });
        }

        if (_userManager.Users.All(x => x.UserName != user.UserName))
        {
            await _userManager.CreateAsync(user, "User123!");
            await _userManager.AddToRolesAsync(user, new[] { Roles.User });
        }
    }

    /// <summary>
    ///     Seeds beers asynchronously.
    /// </summary>
    private async Task SeedBeersAsync()
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