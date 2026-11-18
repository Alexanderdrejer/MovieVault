using Microsoft.AspNetCore.Identity;

namespace MovieVault.Api.Domain.Models;

// Login-kontoen. Arver Id, UserName, PasswordHash, Email m.m. fra IdentityUser.
// Koblingen til Actor/Director ligger på domæne-siden (Actor.UserId / Director.UserId), trin 6.
public class ApplicationUser : IdentityUser
{
}
