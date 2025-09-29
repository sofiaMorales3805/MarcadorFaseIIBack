using MarcadorFaseIIApi.Data;
using MarcadorFaseIIApi.Models;
using MarcadorFaseIIApi.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MarcadorFaseIIApi.Services;

public class EquipoService
{
    private readonly MarcadorDbContext _context;
    private readonly IFileStorageService _fileStorage;

    public EquipoService(MarcadorDbContext context, IFileStorageService fileStorage)
    {
        _context = context;
        _fileStorage = fileStorage;
    }

    // --------- Listados ---------

    public async Task<List<Equipo>> GetListAsync(string? search, string? ciudad, CancellationToken ct = default)
    {
        IQueryable<Equipo> q = _context.Equipos.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            q = q.Where(e => e.Nombre.ToLower().Contains(s));
        }

        if (!string.IsNullOrWhiteSpace(ciudad))
        {
            var c = ciudad.Trim().ToLower();
            q = q.Where(e => e.Ciudad != null && e.Ciudad.ToLower().Contains(c));
        }

        return await q.OrderBy(e => e.Nombre).ToListAsync(ct);
    }

    public Task<Equipo?> GetByIdAsync(int id, CancellationToken ct = default)
        => _context.Equipos.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<(List<Equipo> Items, int Total)> GetPagedAsync(
        string? search, string? ciudad, string? sortBy, bool asc, int page, int pageSize, CancellationToken ct = default)
    {
        IQueryable<Equipo> q = _context.Equipos.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            q = q.Where(e => e.Nombre.ToLower().Contains(s));
        }
        if (!string.IsNullOrWhiteSpace(ciudad))
        {
            var c = ciudad.Trim().ToLower();
            q = q.Where(e => e.Ciudad != null && e.Ciudad.ToLower().Contains(c));
        }

        sortBy = (sortBy ?? "nombre").ToLower();
        q = sortBy switch
        {
            "ciudad" => asc ? q.OrderBy(e => e.Ciudad) : q.OrderByDescending(e => e.Ciudad),
            "puntos" => asc ? q.OrderBy(e => e.Puntos) : q.OrderByDescending(e => e.Puntos),
            "faltas" => asc ? q.OrderBy(e => e.Faltas) : q.OrderByDescending(e => e.Faltas),
            _        => asc ? q.OrderBy(e => e.Nombre) : q.OrderByDescending(e => e.Nombre),
        };

        var total = await q.CountAsync(ct);
        var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return (items, total);
    }

    // --------- Validaciones ---------

    public Task<bool> ExistsByNombreAsync(string nombre, CancellationToken ct = default)
    {
        var n = nombre.Trim().ToLower();
        return _context.Equipos.AnyAsync(e => e.Nombre.ToLower() == n, ct);
    }

    public Task<bool> ExistsByNombreExceptIdAsync(int id, string nombre, CancellationToken ct = default)
    {
        var n = nombre.Trim().ToLower();
        return _context.Equipos.AnyAsync(e => e.Id != id && e.Nombre.ToLower() == n, ct);
    }

    // --------- Crear / Actualizar / Eliminar ---------

    public async Task<Equipo> CreateAsync(string nombre, string ciudad, IFormFile? logo, CancellationToken ct = default)
    {
        // valida duplicado ANTES de guardar archivo
        if (await ExistsByNombreAsync(nombre, ct))
            throw new InvalidOperationException("Ya existe un equipo con ese nombre.");

        var equipo = new Equipo
        {
            Nombre = nombre.Trim(),
            Ciudad = string.IsNullOrWhiteSpace(ciudad) ? "Pendiente" : ciudad.Trim(),
            Puntos = 0,
            Faltas = 0
        };

        if (logo is not null && logo.Length > 0)
        {
            equipo.LogoFileName = await _fileStorage.SaveLogoAsync(logo, ct);
        }

        _context.Equipos.Add(equipo);
        await _context.SaveChangesAsync(ct);
        return equipo;
    }

    public async Task<Equipo?> UpdateAsync(
        int id, string nombre, string ciudad, IFormFile? logo, CancellationToken ct = default)
    {
        var equipo = await _context.Equipos.FirstOrDefaultAsync(e => e.Id == id, ct);
        if (equipo is null) return null;

        if (await ExistsByNombreExceptIdAsync(id, nombre, ct))
            throw new InvalidOperationException("Ya existe un equipo con ese nombre.");

        equipo.Nombre = nombre.Trim();
        equipo.Ciudad = string.IsNullOrWhiteSpace(ciudad) ? "Pendiente" : ciudad.Trim();

        if (logo != null && logo.Length > 0)
        {
            // 1) guarda el nuevo logo
            var nuevoFileName = await _fileStorage.SaveLogoAsync(logo, ct);

            // 2) borra el anterior si existía
            if (!string.IsNullOrWhiteSpace(equipo.LogoFileName))
            {
                await _fileStorage.DeleteLogoAsync(equipo.LogoFileName);
            }

            equipo.LogoFileName = nuevoFileName;
        }

        await _context.SaveChangesAsync(ct);
        return equipo;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var equipo = await _context.Equipos.FirstOrDefaultAsync(e => e.Id == id, ct);
        if (equipo is null) return false;

        // (opcional) bloquear si está en uso en el marcador actual
        // if (await _context.Marcadores.AnyAsync(m => m.EquipoLocalId == id || m.EquipoVisitanteId == id, ct))
        //     throw new InvalidOperationException("No se puede eliminar: el equipo está en uso en el marcador actual.");

        _context.Equipos.Remove(equipo);
        await _context.SaveChangesAsync(ct);

        // borrar logo si existía
        if (!string.IsNullOrWhiteSpace(equipo.LogoFileName))
        {
            await _fileStorage.DeleteLogoAsync(equipo.LogoFileName);
        }

        return true;
    }
}
