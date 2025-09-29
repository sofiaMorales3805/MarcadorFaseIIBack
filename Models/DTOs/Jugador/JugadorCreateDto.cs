using System.ComponentModel.DataAnnotations;

namespace MarcadorFaseIIApi.Models.DTOs.Jugador;

public class JugadorCreateDto
{
    [Required, MinLength(2)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    public int EquipoId { get; set; }

    public string? Posicion { get; set; }
}
