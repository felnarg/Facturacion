using Clientes.Application.DTOs;
using FluentValidation;

namespace Clientes.Application.Validators;

public sealed class UpdateCustomerRequestValidator : AbstractValidator<UpdateCustomerRequest>
{
    public UpdateCustomerRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(200);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es obligatorio.")
            .EmailAddress().WithMessage("El email no es válido.")
            .MaximumLength(200);

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("La ciudad es obligatoria.")
            .MaximumLength(120);

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("El teléfono es obligatorio.")
            .MaximumLength(40);

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("La dirección es obligatoria.")
            .MaximumLength(200);

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("El tipo de cliente no es válido.");

        RuleFor(x => x.IdentificationType)
            .IsInEnum().WithMessage("El tipo de identificación no es válido.");

        RuleFor(x => x.IdentificationNumber)
            .NotEmpty().WithMessage("El número de identificación es obligatorio.")
            .MaximumLength(50);

        When(x => x.IsCreditApproved, () =>
        {
            RuleFor(x => x.ApprovedCreditLimit)
                .GreaterThan(0).WithMessage("El cupo aprobado debe ser mayor a cero.");

            RuleFor(x => x.ApprovedPaymentTermDays)
                .GreaterThan(0).WithMessage("Los días permitidos deben ser mayores a cero.");
        });
    }
}
