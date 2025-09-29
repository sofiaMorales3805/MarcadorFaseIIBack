namespace MarcadorFaseIIApi.Models;

public class PartidoJugador
{
    public int PartidoId { get; set; }
    public int EquipoId { get; set; }
    public int JugadorId { get; set; }
    public bool Titular { get; set; }

    public Partido Partido { get; set; } = null!;
}
