using MarcadorFaseIIApi.Models;

namespace MarcadorFaseIIApi.Repositories.Interfaces
{
    public interface IUserRepository
    {
        /// <summary>
        /// Busca un usuario junto con su rol, usando username y password.
        /// </summary>
        Task<User?> GetUserWithRoleAsync(string username, string password);

        /// <summary>
        /// Busca un usuario solo por su nombre de usuario.
        /// </summary>
        Task<User?> GetByUsernameAsync(string username);

        /// <summary>
        /// Agrega un nuevo usuario.
        /// </summary>
        Task AddAsync(User user);
    }
}
