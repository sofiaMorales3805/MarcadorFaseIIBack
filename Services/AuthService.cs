using MarcadorFaseIIApi.Models;
using MarcadorFaseIIApi.Models.DTOs;
using MarcadorFaseIIApi.Repositories.Interfaces;
using MarcadorFaseIIApi.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

namespace MarcadorFaseIIApi.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IConfiguration _config;

    public AuthService(IUserRepository userRepository, IRoleRepository roleRepository, IConfiguration config)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _config = config;
    }

    public async Task<LoginResponseDto?> AuthenticateAsync(LoginRequestDto request)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            return null;

        var token = GenerateJwtToken(user);

        return new LoginResponseDto
        {
            Username = user.Username,
            Role = new RoleDto { Name = user.Role!.Name },
            Token = token
        };
    }

    public async Task<string?> RegisterAsync(RegisterRequestDto request)
    {
        var existingUser = await _userRepository.GetByUsernameAsync(request.Username);
        if (existingUser != null)
            return null;
        var role = await _roleRepository.GetByNameAsync(request.Role.Name);
        if (role == null)
            return null;

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            Username = request.Username,
            Password = hashedPassword,
            RoleId = role.Id
        };

        await _userRepository.AddAsync(user);
        return "Usuario registrado correctamente.";
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _config.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? "YourSuperSecretKey123!"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role!.Name)
            };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["ExpiresInMinutes"])),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
