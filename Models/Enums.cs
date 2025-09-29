namespace MarcadorFaseIIApi.Models;

public enum TorneoEstado { Planificado = 0, Activo = 1, Finalizado = 2 }
public enum RondaTipo { Final = 2, Semifinal = 4, Cuartos = 8, Octavos = 16 }
public enum PartidoEstado { Programado = 0, EnJuego = 1, Finalizado = 2, Pospuesto = 3, Cancelado = 4 }
