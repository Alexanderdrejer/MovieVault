using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MovieVault.Api.Domain.DTOs;
using MovieVault.Api.Domain.Models;
using MovieVault.Api.Services;

namespace MovieVault.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly TokenService _tokenService;

    public AuthController(UserManager<ApplicationUser> userManager, TokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TokenDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TokenDto>> Login(LoginDto input)
    {
        var user = await _userManager.FindByNameAsync(input.UserName);

        if (user == null || !await _userManager.CheckPasswordAsync(user, input.Password))
            return Unauthorized(); 

        return Ok(await _tokenService.CreateTokenAsync(user));
    }
}
