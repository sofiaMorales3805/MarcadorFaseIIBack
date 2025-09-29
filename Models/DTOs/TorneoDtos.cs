namespace MarcadorFaseIIApi.Models.DTOs.Playoffs;

public record CrearTorneoDto(string Nombre, int Temporada, int BestOf, List<int> EquipoIdsSeed);
public record TorneoDto(int Id, string Nombre, int Temporada, int BestOf, string Estado);
