using MarcadorFaseIIApi.Data;
using MarcadorFaseIIApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MarcadorFaseIIApi.Services;

public class JugadorService
{
    private readonly MarcadorDbContext _context;

    public JugadorService(MarcadorDbContext context)
    {
        _context = context;
    }

    // LISTA con filtros (nombre, equipoNombre, equipoId, posicion)
    public async Task<List<Jugador>> GetListAsync(
        string? search, string? equipoNombre, int? equipoId, string? posicion, CancellationToken ct = default)
    {
        IQueryable<Jugador> q = _context.Jugadores
            .AsNoTracking()
            .Include(j => j.Equipo);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            q = q.Where(j => j.Nombre.ToLower().Contains(s));
        }
        if (!string.IsNullOrWhiteSpace(equipoNombre))
        {
            var en = equipoNombre.Trim().ToLower();
            q = q.Where(j => j.Equipo != null && j.Equipo.Nombre.ToLower().Contains(en));
        }
        if (equipoId.HasValue)
        {
            q = q.Where(j => j.EquipoId == equipoId.Value);
        }
        if (!string.IsNullOrWhiteSpace(posicion))
        {
            var p = posicion.Trim().ToLower();
            q = q.Where(j => j.Posicion != null && j.Posicion.ToLower().Contains(p));
        }

        return await q.OrderBy(j => j.Nombre).ToListAsync(ct);
    }

    // Paginado + ordenamiento
    public async Task<(List<Jugador> Items, int Total)> GetPagedAsync(
        string? search, string? equipoNombre, int? equipoId, string? posicion,
        string? sortBy, bool asc, int page, int pageSize, CancellationToken ct = default)
    {
        IQueryable<Jugador> q = _context.Jugadores
            .AsNoTracking()
            .Include(j => j.Equipo);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            q = q.Where(j => j.Nombre.ToLower().Contains(s));
        }
        if (!string.IsNullOrWhiteSpace(equipoNombre))
        {
            var en = equipoNombre.Trim().ToLower();
            q = q.Where(j => j.Equipo != null && j.Equipo.Nombre.ToLower().Contains(en));
        }
        if (equipoId.HasValue)
        {
            q = q.Where(j => j.EquipoId == equipoId.Value);
        }
        if (!string.IsNullOrWhiteSpace(posicion))
        {
            var p = posicion.Trim().ToLower();
            q = q.Where(j => j.Posicion != null && j.Posicion.ToLower().Contains(p));
        }

        sortBy = (sortBy ?? "nombre").ToLower();
        q = sortBy switch
        {
            "equipo"    => asc ? q.OrderBy(j => j.Equipo!.Nombre) : q.OrderByDescending(j => j.Equipo!.Nombre),
            "posicion"  => asc ? q.OrderBy(j => j.Posicion)      : q.OrderByDescending(j => j.Posicion),
            "puntos"    => asc ? q.OrderBy(j => j.Puntos)        : q.OrderByDescending(j => j.Puntos),
            "faltas"    => asc ? q.OrderBy(j => j.Faltas)        : q.OrderByDescending(j => j.Faltas),
            _           => asc ? q.OrderBy(j => j.Nombre)        : q.OrderByDescending(j => j.Nombre),
        };

        var total = await q.CountAsync(ct);
        var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return (items, total);
    }

    public Task<Jugador?> GetByIdAsync(int id, CancellationToken ct = default)
        => _context.Jugadores.AsNoTracking().Include(j => j.Equipo).FirstOrDefaultAsync(j => j.Id == id, ct);

    // Evitar duplicado de nombre dentro del mismo equipo (case-insensitive)
    public Task<bool> ExistsNombreInEquipoAsync(string nombre, int equipoId, CancellationToken ct = default)
    {
        var n = nombre.Trim().ToLower();
        return _context.Jugadores.AnyAsync(j => j.EquipoId == equipoId && j.Nombre.ToLower() == n, ct);
    }

    public Task<bool> ExistsNombreInEquipoExceptIdAsync(int id, string nombre, int equipoId, CancellationToken ct = default)
    {
        var n = nombre.Trim().ToLower();
        return _context.Jugadores.AnyAsync(j => j.Id != id && j.EquipoId == equipoId && j.Nombre.ToLower() == n, ct);
    }

    public async Task<Jugador> CreateAsync(string nombre, int equipoId, string? posicion, CancellationToken ct = default)
    {
        if (await ExistsNombreInEquipoAsync(nombre, equipoId, ct))
            throw new InvalidOperationException("Ya existe un jugador con ese nombre en el equipo.");

        var jugador = new Jugador
        {
            Nombre = nombre.Trim(),
            EquipoId = equipoId,
            Posicion = string.IsNullOrWhiteSpace(posicion) ? null : posicion.Trim(),
            Puntos = 0,
            Faltas = 0
        };

        _context.Jugadores.Add(jugador);
        await _context.SaveChangesAsync(ct);
        return jugador;
    }

    public async Task<Jugador?> UpdateAsync(int id, string nombre, int equipoId, string? posicion, CancellationToken ct = default)
    {
        var jugador = await _context.Jugadores.FirstOrDefaultAsync(j => j.Id == id, ct);
        if (jugador is null) return null;

        if (await ExistsNombreInEquipoExceptIdAsync(id, nombre, equipoId, ct))
            throw new InvalidOperationException("Ya existe un jugador con ese nombre en el equipo.");

        jugador.Nombre = nombre.Trim();
        jugador.EquipoId = equipoId;
        jugador.Posicion = string.IsNullOrWhiteSpace(posicion) ? null : posicion.Trim();

        await _context.SaveChangesAsync(ct);
        return jugador;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var jugador = await _context.Jugadores.FirstOrDefaultAsync(j => j.Id == id, ct);
        if (jugador is null) return false;

        _context.Jugadores.Remove(jugador);
        await _context.SaveChangesAsync(ct);
        return true;
    }
}
