using Application.DTOs.Users;
using FluentValidation;

namespace Application.Common.Validator;

public class ActualizaPuestoRequestValidator :  AbstractValidator<ActualizaPuestoRequest>
{
    public ActualizaPuestoRequestValidator()
    {
        RuleFor(x => x.Usuario)
            .NotEmpty().WithMessage("El usuario es obligatorio.");
    }
}