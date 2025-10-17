namespace Shared.Dto;

public class AuthDto
{
    public string? Token { get; set; }
}
public class AuthRequest
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}

public class AuthResponse
{
    public required string Token { get; set; }
}
