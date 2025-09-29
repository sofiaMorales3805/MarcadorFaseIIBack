using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarcadorFaseIIApi.Data;
using MarcadorFaseIIApi.Models;
using MarcadorFaseIIApi.Models.DTOs.Playoffs;

namespace MarcadorFaseIIApi.Constrollers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TorneosController : ControllerBase
    {
        private readonly MarcadorDbContext _db;
        public TorneosController(MarcadorDbContext db) { _db = db; }

        // POST api/torneos
        [HttpPost]
        public async Task<ActionResult<TorneoDto>> Crear([FromBody] CrearTorneoDto dto)
        {
            if (dto.EquipoIdsSeed is null || dto.EquipoIdsSeed.Count < 2)
                return BadRequest("Se requieren al menos 2 equipos.");

            var existenTodos = await _db.Equipos
                .Where(e => dto.EquipoIdsSeed.Contains(e.Id))
                .CountAsync() == dto.EquipoIdsSeed.Count;

            if (!existenTodos) return BadRequest("Algún equipo no existe.");

            var t = new Torneo
            {
                Nombre = dto.Nombre,
                Temporada = dto.Temporada,
                BestOf = dto.BestOf <= 0 ? 5 : dto.BestOf,
                Estado = TorneoEstado.Planificado
            };
            _db.Torneos.Add(t);
            await _db.SaveChangesAsync();

            await GenerarRondaInicialInterno(t.Id, dto.EquipoIdsSeed);
            t.Estado = TorneoEstado.Activo;
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = t.Id },
                new TorneoDto(t.Id, t.Nombre, t.Temporada, t.BestOf, t.Estado.ToString()));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TorneoDto>> GetById(int id)
        {
            var t = await _db.Torneos.FindAsync(id);
            if (t is null) return NotFound();
            return Ok(new TorneoDto(t.Id, t.Nombre, t.Temporada, t.BestOf, t.Estado.ToString()));
        }

        // POST api/torneos/{id}/generar-siguiente-ronda
        [HttpPost("{id:int}/generar-siguiente-ronda")]
        public async Task<IActionResult> GenerarSiguienteRonda(int id)
        {
            var t = await _db.Torneos.Include(x => x.Series).ThenInclude(s => s.Partidos)
                                     .FirstOrDefaultAsync(x => x.Id == id);
            if (t is null) return NotFound();

            // Si hay series abiertas en la ronda actual, no se puede avanzar
            if (t.Series.Any(s => !s.Cerrada))
                return BadRequest("Aún hay series abiertas.");

            // Si la última ronda fue la Final -> torneo finalizado
            var rondaMax = t.Series.Max(s => s.Ronda);
            if (rondaMax == RondaTipo.Final)
            {
                t.Estado = TorneoEstado.Finalizado;
                await _db.SaveChangesAsync();
                return Ok(new { message = "Torneo finalizado." });
            }

            // Parear ganadores 1-2, 3-4...
            var ganadores = t.Series.Where(s => s.Ronda == rondaMax)
                                    .OrderBy(s => s.SeedA)
                                    .Select(s => s.GanadorEquipoId ?? (s.WinsA > s.WinsB ? s.EquipoAId : s.EquipoBId))
                                    .ToList();

            var nuevaRonda = (RondaTipo)((int)rondaMax / 2);
            var seriesNuevas = new List<SeriePlayoff>();
            for (int i = 0; i < ganadores.Count; i += 2)
            {
                seriesNuevas.Add(new SeriePlayoff
                {
                    TorneoId = t.Id,
                    Ronda = nuevaRonda,
                    SeedA = i + 1,
                    SeedB = i + 2,
                    EquipoAId = ganadores[i],
                    EquipoBId = ganadores[i + 1],
                    BestOf = 0
                });
            }
            _db.Series.AddRange(seriesNuevas);
            await _db.SaveChangesAsync();

            // Programa Juego 1 de cada serie: hoy+2 a las 19:00
            var baseDt = DateTime.Now.Date.AddDays(2).AddHours(19);
            int offset = 0;
            foreach (var s in seriesNuevas)
            {
                _db.Partidos.Add(new Partido
                {
                    TorneoId = t.Id,
                    SeriePlayoffId = s.Id,
                    GameNumber = 1,
                    FechaHora = baseDt.AddDays(offset++),
                    EquipoLocalId = s.EquipoAId,
                    EquipoVisitanteId = s.EquipoBId
                });
            }
            await _db.SaveChangesAsync();
            return Ok(new { message = "Siguiente ronda generada." });
        }

        // ---- Helper ----
        private async Task GenerarRondaInicialInterno(int torneoId, List<int> seeds)
        {
            var t = await _db.Torneos.FindAsync(torneoId)
                ?? throw new InvalidOperationException("Torneo no existe.");

            int n = seeds.Count;
            RondaTipo ronda = n switch
            {
                2 => RondaTipo.Final,
                4 => RondaTipo.Semifinal,
                8 => RondaTipo.Cuartos,
                16 => RondaTipo.Octavos,
                _ => throw new InvalidOperationException("Cantidad de equipos no válida para bracket.")
            };

            var series = new List<SeriePlayoff>();
            for (int i = 0; i < n / 2; i++)
            {
                series.Add(new SeriePlayoff
                {
                    TorneoId = torneoId,
                    Ronda = ronda,
                    SeedA = i + 1,
                    SeedB = n - i,
                    EquipoAId = seeds[i],
                    EquipoBId = seeds[n - 1 - i],
                    BestOf = 0
                });
            }
            _db.Series.AddRange(series);
            await _db.SaveChangesAsync();

            // Programa Juego 1 (hoy + 1 día a las 19:00)
            var baseDt = DateTime.Now.Date.AddDays(1).AddHours(19);
            int d = 0;
            foreach (var s in series)
            {
                _db.Partidos.Add(new Partido
                {
                    TorneoId = torneoId,
                    SeriePlayoffId = s.Id,
                    GameNumber = 1,
                    FechaHora = baseDt.AddDays(d++),
                    EquipoLocalId = s.EquipoAId,
                    EquipoVisitanteId = s.EquipoBId
                });
            }
            await _db.SaveChangesAsync();
        }

        // SEED DEMO: crea un torneo en juego con partidos pasados/hoy/futuros
        // POST api/torneos/seed-demo
        [HttpPost("seed-demo")]
        public async Task<IActionResult> SeedDemo()
        {
            var equipos = await _db.Equipos.OrderBy(e => e.Id).Take(4).Select(e => e.Id).ToListAsync();
            if (equipos.Count < 4) return BadRequest("Se requieren al menos 4 equipos en la base.");

            var dto = new CrearTorneoDto("Playoffs Apertura (Demo)", DateTime.UtcNow.Year, 5, equipos);
            var created = await Crear(dto);
            var torneoId = ((TorneoDto)((CreatedAtActionResult)created.Result!).Value!).Id;

            var partidos = await _db.Partidos.Where(p => p.TorneoId == torneoId).OrderBy(p => p.Id).ToListAsync();
            if (partidos.Count >= 3)
            {
                // Partido pasado (finalizado)
                partidos[0].FechaHora = DateTime.Now.AddDays(-2).Date.AddHours(19);
                partidos[0].MarcadorLocal = 82; partidos[0].MarcadorVisitante = 76;
                partidos[0].Estado = PartidoEstado.Finalizado;
                var s0 = await _db.Series.FirstAsync(x => x.Id == partidos[0].SeriePlayoffId);
                if (partidos[0].EquipoLocalId == s0.EquipoAId) s0.WinsA++; else s0.WinsB++;

                // Hoy en juego
                partidos[1].FechaHora = DateTime.Now;
                partidos[1].Estado = PartidoEstado.EnJuego;

                // Mañana
                partidos[2].FechaHora = DateTime.Now.AddDays(1).Date.AddHours(19);
                partidos[2].Estado = PartidoEstado.Programado;

                await _db.SaveChangesAsync();
            }
            return Ok(new { message = "Seed demo creado.", torneoId });
        }
    }
}
