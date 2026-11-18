using System.ComponentModel.DataAnnotations;

namespace MovieVault.Api.Domain.DTOs;

public class LoginDto
{
    [Required]
    public string UserName { get; set; } = "";

    [Required]
    public string Password { get; set; } = "";
}

public class TokenDto
{
    public string AccessToken { get; set; } = "";
    public DateTime ExpiresAt { get; set; }
}
