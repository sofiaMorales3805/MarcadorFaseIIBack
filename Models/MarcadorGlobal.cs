namespace MarcadorFaseIIApi.Models;

public class MarcadorGlobal
{
    public int Id { get; set; }

    public int EquipoLocalId { get; set; }
    public Equipo EquipoLocal { get; set; } = new();

    public int EquipoVisitanteId { get; set; }
    public Equipo EquipoVisitante { get; set; } = new();

    public int CuartoActual { get; set; } = 1;
    public int TiempoRestante { get; set; } = 600;
    public bool EnProrroga { get; set; } = false;
    public int NumeroProrroga { get; set; } = 0;

    // Para el front (sonido temporizador)
    public bool RelojCorriendo { get; set; } = false;
}
