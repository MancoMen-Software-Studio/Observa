using FluentValidation;
using Observa.Application.Commands.Dashboards;

namespace Observa.Application.Validators;

/// <summary>
/// Validador para el comando de creacion de dashboard.
/// </summary>
public sealed class CreateDashboardCommandValidator : AbstractValidator<CreateDashboardCommand>
{
    public CreateDashboardCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("El titulo del dashboard es obligatorio.")
            .MaximumLength(200)
            .WithMessage("El titulo no puede superar 200 caracteres.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("La descripcion no puede superar 1000 caracteres.");
    }
}
