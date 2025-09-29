namespace MarcadorFaseIIApi.Models.DTOs.Common;

public record PagedResult<T>(IReadOnlyList<T> Items, int TotalItems, int Page, int PageSize);
