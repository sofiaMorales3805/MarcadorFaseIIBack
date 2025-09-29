using MarcadorFaseIIApi.Models;
using MarcadorFaseIIApi.Models.DTOs;

namespace MarcadorFaseIIApi.Repositories.Interfaces
{
    public interface IRoleRepository
    {
        /// <summary>
        /// Busca un rol por su nombre.
        /// </summary>
        Task<Role?> GetByNameAsync(string name);

        /// <summary>
        /// Agrega un rol nuevo desde un DTO.
        /// </summary>
        Task<Role> AddRoleAsync(RoleDto role);
    }
}
