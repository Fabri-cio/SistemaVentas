using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVentas.Application.DTOs;

public class ProductoQueryDto
{
    public int Page { get; set; } = 1; // Página actual (por defecto 1)

    public int PageSize { get; set; } = 10; // Cantidad de productos por página (por defecto 10)

    public string? Nombre { get; set; } // Filtro por nombre (opcional)

    public decimal? PrecioMin { get; set; } // Filtro por precio mínimo (opcional)

    public decimal? PrecioMax { get; set; } // Filtro por precio máximo (opcional)

    public string? SortBy { get; set; } // Campo por el cual ordenar (opcional)

    public bool Descending { get; set; } = false; // Indica si el orden es descendente (por defecto ascendente)
}
