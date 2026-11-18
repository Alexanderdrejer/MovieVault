using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MovieVault.Api.Domain.DTOs;
using MovieVault.Api.Domain.Models;

namespace MovieVault.Api.Services;

public class TokenService
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _userManager;

    public TokenService(IConfiguration configuration, UserManager<ApplicationUser> userManager)
    {
        _configuration = configuration;
        _userManager = userManager;
    }

    public async Task<TokenDto> CreateTokenAsync(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName!)
        };
        foreach (var role in await _userManager.GetRolesAsync(user))
            claims.Add(new Claim(ClaimTypes.Role, role));

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SigningKey"]!)),
            SecurityAlgorithms.HmacSha256);

        var expiresAt = DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("JWT:ExpiresMinutes"));

        var jwt = new JwtSecurityToken(
            issuer: _configuration["JWT:Issuer"],
            audience: _configuration["JWT:Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: signingCredentials);

        return new TokenDto
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(jwt),
            ExpiresAt = expiresAt
        };
    }
}
