using Microsoft.AspNetCore.Mvc;
using MarcadorFaseIIApi.Models;
using MarcadorFaseIIApi.Models.DTOs;
using MarcadorFaseIIApi.Services;
using MarcadorFaseIIApi.Constrollers;

namespace MarcadorFaseIIApi.Constrollers;

[ApiController]
[Route("api/[controller]")]
public class MarcadorController : ControllerBase
{
    private readonly MarcadorService _service;
    public MarcadorController(MarcadorService service) => _service = service;

    // ---- Lecturas ----
    [HttpGet]
    public ActionResult<MarcadorGlobal> GetMarcador() => Ok(_service.GetMarcador());

    [HttpGet("tiempo")]
    public ActionResult<EstadoTiempoDto> GetTiempo() => Ok(_service.GetEstadoTiempo());

    // ---- Puntos ----
    [HttpPost("puntos/sumar")]
    public IActionResult SumarPuntos([FromQuery] string equipo, [FromQuery] int puntos)
    {
        if (!TryNormalizarEquipo(equipo, out var eq)) return BadRequest("Parametro 'equipo' debe ser 'local' o 'visitante'.");
        _service.SumarPuntos(eq, puntos);
        return Ok(_service.GetMarcador());
    }

    [HttpPost("puntos/restar")]
    public IActionResult RestarPuntos([FromQuery] string equipo, [FromQuery] int puntos)
    {
        if (!TryNormalizarEquipo(equipo, out var eq)) return BadRequest("Parametro 'equipo' debe ser 'local' o 'visitante'.");
        _service.RestarPuntos(eq, puntos);
        return Ok(_service.GetMarcador());
    }

    // ---- Faltas ----
    [HttpPost("falta")]
    public IActionResult RegistrarFalta([FromQuery] string equipo)
    {
        if (!TryNormalizarEquipo(equipo, out var eq)) return BadRequest("Parametro 'equipo' debe ser 'local' o 'visitante'.");
        _service.RegistrarFalta(eq);
        return Ok(_service.GetMarcador());
    }

    // ---- Cuartos ----
    [HttpPost("cuarto/siguiente")]
    public IActionResult AvanzarCuarto()
    {
        var siguiente = _service.SiguienteCuarto();
        return Ok(siguiente);
    }

    // ---- Tiempo (rutas estilo tiempo/*) ----
    [HttpPost("tiempo/iniciar")]
    public IActionResult IniciarTiempo()
    {
        _service.IniciarTiempo();
        return Ok(_service.GetMarcador());
    }

    [HttpPost("tiempo/pausar")]
    public IActionResult PausarTiempo()
    {
        _service.PausarTiempo();
        return Ok(_service.GetMarcador());
    }

    [HttpPost("tiempo/reanudar")]
    public IActionResult ReanudarTiempo()
    {
        _service.ReanudarTiempo();
        return Ok(_service.GetMarcador());
    }

    [HttpPost("tiempo/reiniciar")]
    public ActionResult<MarcadorGlobal> ReiniciarTiempo([FromQuery] int? seg)
    {
        var resultado = _service.ReiniciarTiempo(seg ?? 600);
        return Ok(resultado);
    }

    [HttpPost("tiempo/establecer")]
    public ActionResult<MarcadorGlobal> EstablecerTiempo([FromQuery] int seg)
    {
        var resultado = _service.EstablecerTiempo(seg);

        return Ok(resultado);
    }

    // ---- Tiempo (rutas estilo reloj/*) ----
    [HttpPost("reloj/iniciar")]
    public ActionResult<MarcadorGlobal> IniciarReloj()
    {
        _service.IniciarReloj();
        return Ok(_service.GetMarcador());
    }

    [HttpPost("reloj/pausar")]
    public ActionResult<MarcadorGlobal> PausarReloj()
    {
        _service.PausarReloj();
        return Ok(_service.GetMarcador());
    }

    // ---- Equipos ----
    public record RenombrarEquiposDto(string? Local, string? Visitante);

    [HttpPost("equipos/renombrar")]
    public ActionResult<MarcadorGlobal> RenombrarEquipos([FromBody] RenombrarEquiposDto dto)
    {
        var res = _service.RenombrarEquipos(dto.Local, dto.Visitante);
        return Ok(res);
    }

    [HttpPost("equipos/renombrar-nuevo")]
    public ActionResult<MarcadorGlobal> RenombrarCreandoNuevaFicha([FromQuery] string? local, [FromQuery] string? visitante)
    {
        var nuevaFicha = _service.RenombrarCreandoNuevaFicha(local, visitante);
        return Ok(nuevaFicha);
    }
    //Exponer el endpoint para usarlo enel front 
    [HttpPost("reset-en-cero")]
    public ActionResult<MarcadorGlobal> ResetEnCero() => Ok(_service.InicializarEnCero());


    //______________PARTIDOS_____________
    public class FinPartidoDto { public string? Motivo { get; set; } }

    [HttpPost("partido/terminar")]
    public ActionResult<MarcadorGlobal> Terminar([FromQuery] string? motivo, [FromBody] FinPartidoDto? body)
    {
        var reason = body?.Motivo ?? motivo;
        var res = _service.TerminarPartido(MarcadorService.EstadoPartido.Terminado, reason);
        return Ok(res);
    }

    [HttpPost("partido/finalizar-auto")]
    public ActionResult<MarcadorGlobal> FinalizarAuto()
    {
        var data = _service.TerminarPartido(motivo: null);
        return Ok(data);
    }

    // ---- Nuevo partido (resetea todo a 0 / duración actual) ----
    [HttpPost("nuevo")]
    public ActionResult<MarcadorGlobal> Nuevo() => Ok(_service.NuevoPartido());

    // -------- helpers --------
    private static bool TryNormalizarEquipo(string? equipo, out string normalizado)
    {
        normalizado = "";
        if (string.IsNullOrWhiteSpace(equipo)) return false;
        var e = equipo.Trim().ToLowerInvariant();
        if (e == "local") { normalizado = "Local"; return true; }
        if (e == "visitante") { normalizado = "Visitante"; return true; }
        return false;
    }
}
