using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Common.Interfaces;
using Application.Common.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SharedUtilities.Exceptions;
using SharedUtilities.Models;

namespace Infrastructure.Identity;

/// <summary>
///     IdentityService class.
/// </summary>
public class IdentityService : IIdentityService
{
    /// <summary>
    ///     The json web token settings.
    /// </summary>
    private readonly JwtSettings _jwtSettings;

    /// <summary>
    ///     The user manager.
    /// </summary>
    private readonly UserManager<ApplicationUser> _userManager;

    /// <summary>
    ///     Initializes IdentityService.
    /// </summary>
    /// <param name="userManager">The user manager</param>
    /// <param name="jwtSettings">The json web token settings</param>
    public IdentityService(UserManager<ApplicationUser> userManager, JwtSettings jwtSettings)
    {
        _userManager = userManager;
        _jwtSettings = jwtSettings;
    }

    /// <summary>
    ///     Registers new user.
    /// </summary>
    /// <param name="email">The email</param>
    /// <param name="username">The username</param>
    /// <param name="password">The password</param>
    /// <returns>An AuthenticationResult</returns>
    public async Task<AuthenticationResult> RegisterAsync(string? email, string? username, string? password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(password))
        {
            throw new ValidationException();
        }

        var newUser = new ApplicationUser
        {
            Email = email,
            UserName = username
        };

        var createdUser = await _userManager.CreateAsync(newUser, password);

        if (!createdUser.Succeeded)
            return AuthenticationResult.Failure(createdUser.Errors.Select(x => x.Description));

        await _userManager.AddToRoleAsync(newUser, Roles.User);

        return await GenerateAuthenticationResult(newUser);
    }

    /// <summary>
    ///     Authenticates the user.
    /// </summary>
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <returns>An AuthenticationResult</returns>
    public async Task<AuthenticationResult> LoginAsync(string? email, string? password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            throw new ValidationException();
        }

        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
            return AuthenticationResult.Failure(new[] { "User doesn't exist" });

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);

        if (!isPasswordValid)
        {
            return AuthenticationResult.Failure(new[] { "Password is incorrect" });
        }

        return await GenerateAuthenticationResult(user);
    }

    /// <summary>
    ///     Generates authentication result.
    /// </summary>
    /// <param name="user">The user</param>
    /// <returns>An AuthenticationResult</returns>
    private async Task<AuthenticationResult> GenerateAuthenticationResult(ApplicationUser user)
    {
        var secret = _jwtSettings.Secret;

        if (secret.IsNullOrEmpty())
        {
            return AuthenticationResult.Failure(new List<string> { "Secret not found!" });
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secret!);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email ?? throw new InvalidOperationException())
        };

        var userRoles = await _userManager.GetRolesAsync(user);
        claims.AddRange(userRoles.Select(userRole => new Claim(ClaimTypes.Role, userRole)));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return AuthenticationResult.Success(tokenHandler.WriteToken(token));
    }
}