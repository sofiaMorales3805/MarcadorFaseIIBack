namespace MarcadorFaseIIApi.Models;

public class Partido
{
    public int Id { get; set; }

    // Amistosos: sin torneo/serie
    public int? TorneoId { get; set; }

    public int? SeriePlayoffId { get; set; }
    public SeriePlayoff? Serie { get; set; }

    public int? GameNumber { get; set; }      // para series; amistoso puede ser null
    public DateTime FechaHora { get; set; }
    public PartidoEstado Estado { get; set; } = PartidoEstado.Programado;

    public int EquipoLocalId { get; set; }
    public int EquipoVisitanteId { get; set; }

    public int? MarcadorLocal { get; set; }
    public int? MarcadorVisitante { get; set; }

    public ICollection<PartidoJugador> Roster { get; set; } = new List<PartidoJugador>();
}
