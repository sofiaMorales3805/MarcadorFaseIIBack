using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarcadorFaseIIApi.Data;
using MarcadorFaseIIApi.Models;
using MarcadorFaseIIApi.Models.DTOs.Playoffs;

namespace MarcadorFaseIIApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PartidosController : ControllerBase
    {
        private readonly MarcadorDbContext _db;
        public PartidosController(MarcadorDbContext db) { _db = db; }

        // ==========================================
// PUT api/partidos/{id}/marcador  (cierra partido y avanza serie si aplica)
[HttpPut("{id:int}/marcador")]
public async Task<IActionResult> Cerrar(int id, [FromBody] CerrarPartidoDto dto)
{
    var p = await _db.Partidos
        .Include(x => x.Serie)
        .ThenInclude(s => s.Torneo)
        .FirstOrDefaultAsync(x => x.Id == id);

    if (p is null) return NotFound();

    // Actualiza marcador y estado
    p.MarcadorLocal = dto.MarcadorLocal;
    p.MarcadorVisitante = dto.MarcadorVisitante;
    p.Estado = PartidoEstado.Finalizado;

    var s = p.Serie;

    // === Amistoso: no hay serie/torneo, solo guarda y termina ===
    if (s is null)
    {
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // === Series de playoff ===
    int bestOf = s.BestOf > 0 ? s.BestOf : (s.Torneo?.BestOf ?? 0);
    int winsNecesarios = bestOf > 0 ? (bestOf / 2 + 1) : 0;

    int ganadorId = p.MarcadorLocal > p.MarcadorVisitante ? p.EquipoLocalId : p.EquipoVisitanteId;
    if (ganadorId == s.EquipoAId) s.WinsA++; else s.WinsB++;

    // ¿Se cerró la serie?
    if (winsNecesarios > 0 && (s.WinsA >= winsNecesarios || s.WinsB >= winsNecesarios))
    {
        s.Cerrada = true;
        s.GanadorEquipoId = s.WinsA > s.WinsB ? s.EquipoAId : s.EquipoBId;
    }
    else
    {
        // Programa el siguiente juego de la serie
        int nextGame = (await _db.Partidos.CountAsync(x => x.SeriePlayoffId == s.Id)) + 1;
        bool localEsA = (nextGame % 2 == 1); // patrón simple A-B-A-B...
        _db.Partidos.Add(new Partido
        {
            TorneoId = s.TorneoId,
            SeriePlayoffId = s.Id,
            GameNumber = nextGame,
            FechaHora = (p.FechaHora.Date.AddDays(1)).AddHours(19),
            EquipoLocalId = localEsA ? s.EquipoAId : s.EquipoBId,
            EquipoVisitanteId = localEsA ? s.EquipoBId : s.EquipoAId
        });
    }

    await _db.SaveChangesAsync();
    return NoContent();
}

        [HttpPost("{id:int}/roster")]
        public async Task<IActionResult> AsignarRoster(int id, [FromBody] AsignarRosterDto dto)
        {
            var p = await _db.Partidos.Include(x => x.Roster).FirstOrDefaultAsync(x => x.Id == id);
            if (p is null) return NotFound();

            var actuales = p.Roster.Where(r => r.EquipoId == dto.EquipoId).ToList();
            _db.PartidosJugadores.RemoveRange(actuales);

            var nuevos = dto.Jugadores?.Take(12).Select(j => new PartidoJugador
            {
                PartidoId = id,
                EquipoId = dto.EquipoId,
                JugadorId = j.JugadorId,
                Titular = j.Titular
            }).ToList() ?? new List<PartidoJugador>();

            await _db.PartidosJugadores.AddRangeAsync(nuevos);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // POST api/partidos  (crear partido amistoso)
        [HttpPost]
        public async Task<ActionResult<object>> CrearAmistoso([FromBody] CrearAmistosoDto dto)
        {
            if (dto.EquipoLocalId == dto.EquipoVisitanteId)
                return BadRequest("Los equipos no pueden ser iguales.");

            var p = new Partido
            {
                FechaHora = dto.FechaHora,
                EquipoLocalId = dto.EquipoLocalId,
                EquipoVisitanteId = dto.EquipoVisitanteId,
                Estado = PartidoEstado.Programado
                // TorneoId/SeriePlayoffId/GameNumber: amistoso => permanecen null si el modelo lo permite
            };

            _db.Partidos.Add(p);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = p.Id }, new { id = p.Id });
        }

        // GET api/partidos/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<PartidoDto>> GetById(int id)
        {
            var p = await _db.Partidos
                .Include(x => x.Serie)
                .ThenInclude(s => s.Torneo)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (p is null) return NotFound();

            string? ronda = p.Serie != null ? p.Serie.Ronda.ToString() : null;
            int? seedA = p.Serie?.SeedA;
            int? seedB = p.Serie?.SeedB;

            return new PartidoDto(
                p.Id, p.TorneoId, p.SeriePlayoffId, p.GameNumber,
                p.FechaHora, p.Estado.ToString(),
                p.EquipoLocalId, p.EquipoVisitanteId,
                p.MarcadorLocal, p.MarcadorVisitante,
                ronda, seedA, seedB
            );
        }
        // NUEVO: PUT api/partidos/{id}/estado
        [HttpPut("{id:int}/estado")]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] CambiarEstadoDto dto)
        {
            if (!Enum.TryParse<PartidoEstado>(dto.Estado, out var nuevo))
                return BadRequest("Estado inválido.");

            var p = await _db.Partidos.FirstOrDefaultAsync(x => x.Id == id);
            if (p is null) return NotFound();

            p.Estado = nuevo;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // GET api/partidos/historial?torneoId=&estado=&ronda=&equipoId=&...
        //  con null-checks para soportar partidos sin Serie)
        [HttpGet("historial")]
        public async Task<ActionResult<PagedResult<PartidoDto>>> Historial([FromQuery] PartidoQuery q)
        {
            var qry = _db.Partidos.Include(p => p.Serie).ThenInclude(s => s.Torneo).AsQueryable();

            if (q.TorneoId.HasValue) qry = qry.Where(p => p.TorneoId == q.TorneoId.Value);
            if (!string.IsNullOrWhiteSpace(q.Estado) && Enum.TryParse<PartidoEstado>(q.Estado, out var est))
                qry = qry.Where(p => p.Estado == est);
            if (!string.IsNullOrWhiteSpace(q.Ronda) && Enum.TryParse<RondaTipo>(q.Ronda, out var r))
                qry = qry.Where(p => p.Serie != null && p.Serie.Ronda == r);
            if (q.EquipoId.HasValue)
                qry = qry.Where(p => p.EquipoLocalId == q.EquipoId.Value || p.EquipoVisitanteId == q.EquipoId.Value);
            if (q.FechaDesde.HasValue) qry = qry.Where(p => p.FechaHora >= q.FechaDesde);
            if (q.FechaHasta.HasValue) qry = qry.Where(p => p.FechaHora <= q.FechaHasta);

            // auto-EN_JUEGO si la hora ya pasó y no hay marcador final
            var now = DateTime.Now;
            await qry.Where(p => p.Estado == PartidoEstado.Programado &&
                                 p.FechaHora <= now &&
                                 p.MarcadorLocal == null &&
                                 p.MarcadorVisitante == null)
                     .ExecuteUpdateAsync(s => s.SetProperty(p => p.Estado, PartidoEstado.EnJuego));

            int page = q.Page <= 0 ? 1 : q.Page;
            int size = q.PageSize <= 0 ? 10 : q.PageSize;

            var total = await qry.CountAsync();
            var items = await qry.OrderByDescending(p => p.FechaHora)
                                 .Skip((page - 1) * size)
                                 .Take(size)
                                 .Select(p => new PartidoDto(
                                     p.Id, p.TorneoId, p.SeriePlayoffId, p.GameNumber,
                                     p.FechaHora, p.Estado.ToString(),
                                     p.EquipoLocalId, p.EquipoVisitanteId,
                                     p.MarcadorLocal, p.MarcadorVisitante,
                                     p.Serie != null ? p.Serie.Ronda.ToString() : null,
                                     p.Serie != null ? p.Serie.SeedA : (int?)null,
                                     p.Serie != null ? p.Serie.SeedB : (int?)null
                                 ))
                                 .ToListAsync();

            return Ok(new PagedResult<PartidoDto>(items, total, page, size));
        }
    }
}
