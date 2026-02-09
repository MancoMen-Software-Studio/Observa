using FluentValidation;
using Observa.Application.Commands.DataSources;

namespace Observa.Application.Validators;

/// <summary>
/// Validador para el comando de creacion de origen de datos.
/// </summary>
public sealed class CreateDataSourceCommandValidator : AbstractValidator<CreateDataSourceCommand>
{
    public CreateDataSourceCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("El nombre del origen de datos es obligatorio.")
            .MaximumLength(200)
            .WithMessage("El nombre no puede superar 200 caracteres.");

        RuleFor(x => x.ConnectionString)
            .NotEmpty()
            .WithMessage("La cadena de conexion es obligatoria.");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("El tipo de origen de datos no es valido.");
    }
}
