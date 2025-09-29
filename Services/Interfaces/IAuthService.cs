using MarcadorFaseIIApi.Models.DTOs;

namespace MarcadorFaseIIApi.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?> AuthenticateAsync(LoginRequestDto request);
    Task<string?> RegisterAsync(RegisterRequestDto request);
}
