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
}
