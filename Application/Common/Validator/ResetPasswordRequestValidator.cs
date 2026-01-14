using Application.DTOs.Users;
using FluentValidation;

namespace Application.Common.Validator;

public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.Usuario)
            .NotEmpty().WithMessage("El usuario es obligatorio.");
    }
}