using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using MarcadorFaseIIApi.Services.Interfaces;

namespace MarcadorFaseIIApi.Services
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly string _logosDirPhysical;

        private static readonly HashSet<string> _allowedExtensions =
            new(StringComparer.OrdinalIgnoreCase) { ".png", ".jpg", ".jpeg", ".webp" };

        private const long MaxLogoBytes = 2 * 1024 * 1024; // 2 MB

        public LocalFileStorageService(IWebHostEnvironment env)
        {
            // Carpeta física: <raíz-proyecto>/wwwroot/uploads/logos
            var wwwroot = Path.Combine(env.ContentRootPath, "wwwroot");
            _logosDirPhysical = Path.Combine(wwwroot, "uploads", "logos");
            Directory.CreateDirectory(_logosDirPhysical);
        }

        public async Task<string> SaveLogoAsync(IFormFile file, CancellationToken ct = default)
        {
            if (file == null || file.Length == 0)
                throw new InvalidOperationException("Archivo de logo vacío.");

            if (file.Length > MaxLogoBytes)
                throw new InvalidOperationException("El logo excede 2MB.");

            var ext = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(ext) || !_allowedExtensions.Contains(ext))
                throw new InvalidOperationException("Formato no permitido. Usa PNG, JPG, JPEG o WEBP.");

            var newName = $"{Guid.NewGuid():N}{ext}";
            var destPath = Path.Combine(_logosDirPhysical, newName);

            using var stream = File.Create(destPath);
            await file.CopyToAsync(stream, ct);

            return newName; // devolvemos solo el nombre
        }

        public Task DeleteLogoAsync(string? fileName)
        {
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                var path = Path.Combine(_logosDirPhysical, fileName);
                if (File.Exists(path)) File.Delete(path);
            }
            return Task.CompletedTask;
        }
    }
}
