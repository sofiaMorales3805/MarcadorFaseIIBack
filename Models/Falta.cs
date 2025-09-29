namespace MarcadorFaseIIApi.Models
{
    public class Falta
    {
        public int Id { get; set; }
        public int JugadorId { get; set; }
        public int EquipoId { get; set; }
        public Equipo Equipo { get; set; } = null!;
        public string Tipo { get; set; } = string.Empty;
        public int Minuto { get; set; }
    }
}
