namespace Api.DTOs;

public record RegisterDto(string Email, string Password, string ConfirmPassword);

public record LoginDto(string Email, string Password);

public record AuthResponseDto(string Token, string Email, IList<string> Roles, DateTime ExpiresAt);
