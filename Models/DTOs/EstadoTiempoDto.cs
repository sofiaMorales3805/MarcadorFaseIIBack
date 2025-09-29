namespace MarcadorFaseIIApi.Models.DTOs;

public class EstadoTiempoDto
{
        public string Estado { get; set; } = "Stopped"; // "Running" | "Paused" | "Stopped"
        public int CuartoActual { get; set; }
        public int SegundosRestantes { get; set; }
        public int DuracionCuarto { get; set; }
}
