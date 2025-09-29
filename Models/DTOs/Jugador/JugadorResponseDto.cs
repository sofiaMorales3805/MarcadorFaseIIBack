namespace MarcadorFaseIIApi.Models.DTOs.Jugador;

public class JugadorResponseDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int Puntos { get; set; }
    public int Faltas { get; set; }
    public string? Posicion { get; set; }
    public int EquipoId { get; set; }
    public string EquipoNombre { get; set; } = string.Empty;
}
