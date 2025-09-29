using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MarcadorFaseIIApi.Models.DTOs.Equipo;

public class EquipoCreateFormDto
{
    [Required, MinLength(2)]
    public string Nombre { get; set; } = string.Empty;

    [Required, MinLength(2)]
    public string Ciudad { get; set; } = string.Empty;

    // Opcional en creación
    public IFormFile? Logo { get; set; }
}
