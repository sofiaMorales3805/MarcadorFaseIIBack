namespace MarcadorFaseIIApi.Models;

public class SeriePlayoff
{
    public int Id { get; set; }

    public int TorneoId { get; set; }
    public Torneo Torneo { get; set; } = null!;

    public RondaTipo Ronda { get; set; }
    public int SeedA { get; set; }
    public int SeedB { get; set; }

    public int EquipoAId { get; set; }
    public int EquipoBId { get; set; }

    public int BestOf { get; set; } // 0 => usar Torneo.BestOf
    public int WinsA { get; set; }
    public int WinsB { get; set; }
    public bool Cerrada { get; set; }
    public int? GanadorEquipoId { get; set; }

    public ICollection<Partido> Partidos { get; set; } = new List<Partido>();
}
