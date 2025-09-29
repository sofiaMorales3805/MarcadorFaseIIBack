using System.ComponentModel.DataAnnotations;
namespace MarcadorFaseIIApi.Models.DTOs;

public class ConfigTiempoDto
{
	// Duración de cada cuarto en segundos (por ej. 600 = 10 min)
	[Range(60, 3600, ErrorMessage = "Duración entre 60 y 3600 segundos.")]
	public int DuracionSegundosPorCuarto { get; set; } = 300;

}
