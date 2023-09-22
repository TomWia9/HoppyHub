﻿using Application.Common.Interfaces;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SharedUtilities.Models;

namespace Infrastructure.Persistence;

/// <summary>
///     ApplicationDbContextInitializer class.
/// </summary>
public class ApplicationDbContextInitializer : IApplicationDbContextInitializer
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
    ///     Initializes ApplicationDbContextInitializer.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="roleManager">The role manager</param>
    /// <param name="userManager">The user manager</param>
    public ApplicationDbContextInitializer(ApplicationDbContext context, RoleManager<IdentityRole<Guid>> roleManager,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _roleManager = roleManager;
        _userManager = userManager;
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

            await SeedRolesAsync();
            await SeedUsersAsync();

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
            Id = new Guid("D21A5205-6122-4F76-9F5D-DE586CC42DB0"),
            UserName = "administrator@localhost",
            Email = "administrator@localhost"
        };

        var user = new ApplicationUser
        {
            Id = new Guid("2E5990D2-DADF-417B-9499-4A7B7ACFE9F0"),
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
}