using FluentValidation;
using SistemaVentas.Application.DTOs;

namespace SistemaVentas.Application.Validators;

public class UpdateProductoDtoValidator : AbstractValidator<UpdateProductoDto>
{
    public UpdateProductoDtoValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty()
            .WithMessage("El nombre es obligatorio")
            .MaximumLength(100)
            .WithMessage("El nombre no puede superar 100 caracteres")
            .MinimumLength(3)
            .WithMessage("El nombre debe tener al menos 3 caracteres");

        RuleFor(x => x.Precio)
            .GreaterThan(0)
            .WithMessage("El precio deber ser mayor que cero");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0)
            .WithMessage("El stock no puede ser negativo");
    }
}
