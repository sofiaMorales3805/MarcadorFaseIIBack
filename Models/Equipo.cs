using System.ComponentModel.DataAnnotations;
using MarcadorFaseIIApi.Services.Interfaces;
namespace MarcadorFaseIIApi.Models
{
    public class Equipo
    {
        public int Id { get; set; }
        [Required]                 
        public string Nombre { get; set; } = string.Empty;
        public int Puntos { get; set; }
        public int Faltas { get; set; }
        public string Ciudad { get; set; } = string.Empty; 
        public string? LogoFileName { get; set; }
        public List<Jugador>? Jugadores { get; set; }
    }
}
