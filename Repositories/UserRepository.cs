using MarcadorFaseIIApi.Repositories.Interfaces;
using MarcadorFaseIIApi.Data;
using MarcadorFaseIIApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MarcadorFaseIIApi.Repositories;

public class UserRepository : IUserRepository
{
    private readonly MarcadorDbContext _context;
    public UserRepository(MarcadorDbContext context)
    {
        _context = context;
    }
    /// <summary>
    /// Busca un usuario en la base de datos y retorna la coincidencia junto con su rol.
    /// </summary>
    /// <param name="username">Nombre de usuario (string requerido).</param>
    /// <param name="password">Contraseña del usuario (string requerida).</param>
    /// <returns>El usuario encontrado con su rol o <c>null</c> si no existe.</returns>
    public async Task<User?> GetUserWithRoleAsync(string username, string password)
    {
        return await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
    }
    /// <summary>
    /// Busca un usuario únicamente por su nombre de usuario.
    /// </summary>
    /// <param name="username">Nombre de usuario (string requerido).</param>
    /// <returns>El usuario encontrado o <c>null</c> si no existe.</returns>
    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }
    /// <summary>
    /// Agrega un nuevo usuario a la base de datos.
    /// </summary>
    /// <param name="user">Objeto de tipo <see cref="User"/> que representa al usuario a registrar.</param>
    public async Task AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }
}
