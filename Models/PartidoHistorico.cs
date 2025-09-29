namespace MarcadorFaseIIApi.Models;

public class PartidoHistorico
{

        public int Id { get; set; }

        // Cuándo se guardó el partido
        public DateTime Fecha { get; set; }

        // Identidad + nombres “congelados” (por si luego renombraste equipos)
        public int? EquipoLocalId { get; set; }
        public string? NombreLocal { get; set; }
        public int PuntosLocal { get; set; }
        public int FaltasLocal { get; set; }

        public int? EquipoVisitanteId { get; set; }
        public string? NombreVisitante { get; set; }
        public int PuntosVisitante { get; set; }
        public int FaltasVisitante { get; set; }

        // Estado del juego al cerrar
        public int Cuarto { get; set; }
        public bool EnProrroga { get; set; }
        public int NumeroProrroga { get; set; }

        // Tiempo/duración (segundos)
        public int DuracionCuartoSeg { get; set; }   // la duración configurada del cuarto normal al momento del cierre
        public int TiempoFinalSeg { get; set; }      // tiempo que quedaba cuando se cerró

        // Resultado administrativo
        public string Estado { get; set; } = "Terminado"; // “Terminado”, “Suspendido”, “Cancelado”
        public string? MotivoFin { get; set; }
}
