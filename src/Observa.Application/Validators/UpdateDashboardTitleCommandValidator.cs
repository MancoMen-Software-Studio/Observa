using FluentValidation;
using Observa.Application.Commands.Dashboards;

namespace Observa.Application.Validators;

/// <summary>
/// Validador para el comando de actualizacion de titulo de dashboard.
/// </summary>
public sealed class UpdateDashboardTitleCommandValidator : AbstractValidator<UpdateDashboardTitleCommand>
{
    public UpdateDashboardTitleCommandValidator()
    {
        RuleFor(x => x.DashboardId)
            .NotEmpty()
            .WithMessage("El identificador del dashboard es obligatorio.");

        RuleFor(x => x.NewTitle)
            .NotEmpty()
            .WithMessage("El nuevo titulo es obligatorio.")
            .MaximumLength(200)
            .WithMessage("El titulo no puede superar 200 caracteres.");
    }
}
