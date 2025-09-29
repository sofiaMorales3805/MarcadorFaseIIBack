using Microsoft.EntityFrameworkCore;
using MarcadorFaseIIApi.Repositories.Interfaces;
using MarcadorFaseIIApi.Data;
using MarcadorFaseIIApi.Models.DTOs;
using MarcadorFaseIIApi.Models;

namespace MarcadorFaseIIApi.Repositories;

public class RoleRepository(MarcadorDbContext context) : IRoleRepository
{
    private readonly MarcadorDbContext _context = context;
    /// <summary>
    /// Agrega el role a la persona
    /// </summary>
    /// 
    public Task<Role> AddRoleAsync(RoleDto role)
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Buscar role por nombre 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public async Task<Role?> GetByNameAsync(string name)
    {
        return await _context.Roles.FirstOrDefaultAsync(r => r.Name == name);
    }

}

