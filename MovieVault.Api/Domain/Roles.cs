namespace MovieVault.Api.Domain;

public static class Roles
{
    public const string Actor = "Actor";
    public const string Director = "Director";
    public const string Reviewer = "Reviewer";
    public const string Admin = "Admin";

    public static readonly string[] All = [Actor, Director, Reviewer, Admin];
}