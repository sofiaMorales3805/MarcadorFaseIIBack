using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MarcadorFaseIIApi.Services;
using MarcadorFaseIIApi.Models;
using MarcadorFaseIIApi.Models.DTOs.Equipo;
using MarcadorFaseIIApi.Models.DTOs.Common;

namespace MarcadorFaseIIApi.Constrollers // <- usa el mismo namespace que ya tienes en el proyecto
{
    [ApiController]
    [Route("api/[controller]")]
    public class EquiposController : ControllerBase
    {
        private readonly EquipoService _service;

        public EquiposController(EquipoService service)
        {
            _service = service;
        }

        // ----------------- GETs -----------------

        // GET api/equipos?search=&ciudad=
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EquipoResponseDto>>> Get(
            [FromQuery] string? search,
            [FromQuery] string? ciudad,
            CancellationToken ct)
        {
            var equipos = await _service.GetListAsync(search, ciudad, ct);
            return Ok(equipos.Select(ToResponseDto));
        }

        // GET api/equipos/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<EquipoResponseDto>> GetById(int id, CancellationToken ct)
        {
            var e = await _service.GetByIdAsync(id, ct);
            if (e is null) return NotFound();
            return Ok(ToResponseDto(e));
        }

        // GET api/equipos/paged?page=1&pageSize=10&search=&ciudad=&sortBy=nombre&sortDir=asc
        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<EquipoResponseDto>>> GetPaged(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? ciudad = null,
            [FromQuery] string? sortBy = "nombre",
            [FromQuery] string sortDir = "asc",
            CancellationToken ct = default)
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;
            var asc = (sortDir ?? "asc").Equals("asc", StringComparison.OrdinalIgnoreCase);

            var (items, total) = await _service.GetPagedAsync(search, ciudad, sortBy, asc, page, pageSize, ct);
            var dtos = items.Select(ToResponseDto).ToList();
            return Ok(new PagedResult<EquipoResponseDto>(dtos, total, page, pageSize));
        }

        // ----------------- Mutaciones -----------------

        // POST api/equipos
        #if DEBUG
        [AllowAnonymous]
        #else
        [Authorize(Roles = "Admin")]
        #endif
        [HttpPost]
        [RequestSizeLimit(5_000_000)]
        public async Task<ActionResult<EquipoResponseDto>> Create([FromForm] EquipoCreateFormDto form, CancellationToken ct)
        {
            try
            {
                var created = await _service.CreateAsync(form.Nombre, form.Ciudad, form.Logo, ct);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToResponseDto(created));
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("existe un equipo", StringComparison.OrdinalIgnoreCase))
            {
                return Conflict(new { message = ex.Message }); // 409 duplicado
            }
        }

        // PUT api/equipos/{id}
        #if DEBUG
        [AllowAnonymous]
        #else
        [Authorize(Roles = "Admin")]
        #endif
        [HttpPut("{id:int}")]
        [RequestSizeLimit(5_000_000)]
        public async Task<ActionResult<EquipoResponseDto>> Update(int id, [FromForm] EquipoUpdateFormDto form, CancellationToken ct)
        {
            try
            {
                var updated = await _service.UpdateAsync(id, form.Nombre, form.Ciudad, form.Logo, ct);
                if (updated is null) return NotFound();
                return Ok(ToResponseDto(updated));
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("existe un equipo", StringComparison.OrdinalIgnoreCase))
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // DELETE api/equipos/{id}
        #if DEBUG
        [AllowAnonymous]
        #else
        [Authorize(Roles = "Admin")]
        #endif
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            try
            {
                var ok = await _service.DeleteAsync(id, ct);
                if (!ok) return NotFound();
                return NoContent();
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("está en uso", StringComparison.OrdinalIgnoreCase))
            {
                // Si activaste el chequeo "en uso" en el service, devolvemos 409 aquí
                return Conflict(new { message = ex.Message });
            }
        }

        // ----------------- Helpers -----------------

        private EquipoResponseDto ToResponseDto(Equipo e) => new()
        {
            Id = e.Id,
            Nombre = e.Nombre,
            Puntos = e.Puntos,
            Faltas = e.Faltas,
            Ciudad = e.Ciudad ?? "",
            LogoUrl = BuildLogoUrl(e.LogoFileName)
        };

        private string? BuildLogoUrl(string? fileName)
            => string.IsNullOrWhiteSpace(fileName)
                ? null
                : $"{Request.Scheme}://{Request.Host}/uploads/logos/{fileName}";
    }
}
