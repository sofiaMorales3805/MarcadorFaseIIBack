namespace MarcadorFaseIIApi.Models.DTOs.Equipo;

public class EquipoResponseDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int Puntos { get; set; }
    public int Faltas { get; set; }
    public string? Ciudad { get; set; }
    public string? LogoUrl { get; set; } 
}
