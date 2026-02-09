using FluentValidation;
using Observa.Application.Commands.Widgets;

namespace Observa.Application.Validators;

/// <summary>
/// Validador para el comando de adicion de widget.
/// </summary>
public sealed class AddWidgetCommandValidator : AbstractValidator<AddWidgetCommand>
{
    public AddWidgetCommandValidator()
    {
        RuleFor(x => x.DashboardId)
            .NotEmpty()
            .WithMessage("El identificador del dashboard es obligatorio.");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("El titulo del widget es obligatorio.")
            .MaximumLength(100)
            .WithMessage("El titulo del widget no puede superar 100 caracteres.");

        RuleFor(x => x.DataSourceId)
            .NotEmpty()
            .WithMessage("El origen de datos es obligatorio.");

        RuleFor(x => x.Column)
            .GreaterThanOrEqualTo(0)
            .WithMessage("La columna no puede ser negativa.");

        RuleFor(x => x.Row)
            .GreaterThanOrEqualTo(0)
            .WithMessage("La fila no puede ser negativa.");

        RuleFor(x => x.Width)
            .GreaterThan(0)
            .WithMessage("El ancho debe ser mayor a cero.");

        RuleFor(x => x.Height)
            .GreaterThan(0)
            .WithMessage("El alto debe ser mayor a cero.");
    }
}
