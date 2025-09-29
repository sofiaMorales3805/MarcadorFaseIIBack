namespace MarcadorFaseIIApi.Models.DTOs;

public class RegisterRequestDto
{
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required RoleDto Role { get; set; }
}
