using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MarcadorFaseIIApi.Models.DTOs.Equipo;

public class EquipoUpdateFormDto
{
    [Required, MinLength(2)]
    public string Nombre { get; set; } = string.Empty;

    [Required, MinLength(2)]
    public string Ciudad { get; set; } = string.Empty;

    // Si viene, reemplaza el logo
    public IFormFile? Logo { get; set; }
}
