using FluentValidation;
using SistemaVentas.Application.DTOs;

namespace SistemaVentas.Application.Validators;

public class CreateProductoDtoValidator : AbstractValidator<CreateProductoDto>
{
    public CreateProductoDtoValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty() // en dataanotations es [Required]
            .WithMessage("El nombre del producto es obligatorio.") // en dataanotations es el mensaje del atributo [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
            .MaximumLength(100) // en dataanotations es [MaxLength(100)]
            .WithMessage("El nombre del producto no puede exceder los 100 caracteres.")
            .MinimumLength(3) // en dataanotations no hay un equivalente directo, pero se puede usar [StringLength(100, MinimumLength = 3)]
            .WithMessage("El nombre del producto debe tener al menos 3 caracteres.");

        RuleFor(x => x.Precio)
            .GreaterThan(0) // en dataanotations es [Range(0.01, double.MaxValue)]
            .WithMessage("El precio del producto debe ser mayor que 0.");
            
        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0) // en dataanotations es [Range(0, int.MaxValue)]
            .WithMessage("El stock del producto no puede ser negativo.");
    }
}
