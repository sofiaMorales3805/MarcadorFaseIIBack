namespace MarcadorFaseIIApi.Models;

public class Torneo
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public int Temporada { get; set; } = DateTime.UtcNow.Year;
    public int BestOf { get; set; } = 5; // 3,5 o 7
    public TorneoEstado Estado { get; set; } = TorneoEstado.Planificado;

    public ICollection<SeriePlayoff> Series { get; set; } = new List<SeriePlayoff>();
}
