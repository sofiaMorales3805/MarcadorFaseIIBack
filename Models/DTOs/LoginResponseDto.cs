namespace MarcadorFaseIIApi.Models.DTOs;

public class LoginResponseDto
{
        public string? Token { get; set; }
        public string? Username { get; set; }
        public RoleDto? Role { get; set; }
}
