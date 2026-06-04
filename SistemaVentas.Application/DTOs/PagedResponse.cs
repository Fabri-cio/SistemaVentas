namespace SistemaVentas.Application.DTOs;

public class PagedResponse<T>
{
    public int Page { get; set; } // Página actual
    public int PageSize { get; set; } // Cantidad de elementos por página
    public int TotalRecords { get; set; } // Total de registros disponibles
    public IEnumerable<T> Data { get; set; } = []; // Datos de la página actual (Enumerable.Empty<T>(); = []);
}
