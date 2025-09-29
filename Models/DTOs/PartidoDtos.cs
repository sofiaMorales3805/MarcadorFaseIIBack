namespace MarcadorFaseIIApi.Models.DTOs.Playoffs;

public record PartidoDto(
    int Id, int? TorneoId, int? SeriePlayoffId, int? GameNumber,
    DateTime FechaHora, string Estado,
    int EquipoLocalId, int EquipoVisitanteId,
    int? MarcadorLocal, int? MarcadorVisitante,
    string? Ronda, int? SeedA, int? SeedB);

public record CerrarPartidoDto(int MarcadorLocal, int MarcadorVisitante);
public record AsignarRosterDto(int EquipoId, List<RosterItemDto> Jugadores);
public record RosterItemDto(int JugadorId, bool Titular);
public record CrearAmistosoDto(DateTime FechaHora, int EquipoLocalId, int EquipoVisitanteId);
public record CambiarEstadoDto(string Estado);

public class PartidoQuery
{
    public int? TorneoId { get; set; }
    public string? Estado { get; set; }      
    public string? Ronda { get; set; }       
    public int? EquipoId { get; set; }
    public DateTime? FechaDesde { get; set; }
    public DateTime? FechaHasta { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public record PagedResult<T>(IEnumerable<T> Items, int Total, int Page, int PageSize);