using Microsoft.AspNetCore.Http;

namespace MarcadorFaseIIApi.Services.Interfaces
{
    public interface IFileStorageService
    {
        /// Guarda un logo y devuelve el nombre de archivo persistido.
        Task<string> SaveLogoAsync(IFormFile file, CancellationToken ct = default);

        /// Elimina un logo existente si aplica (ignora null/empty).
        Task DeleteLogoAsync(string? fileName);
    }
}
