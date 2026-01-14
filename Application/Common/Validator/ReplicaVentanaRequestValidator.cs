using Application.DTOs.Users;
using FluentValidation;

namespace Application.Common.Validator;

public class ReplicaVentanaRequestValidator : AbstractValidator<ReplicaVentanaRequest>
{
    public ReplicaVentanaRequestValidator()
    {
        RuleFor(x => x.UsuarioOrigen)
            .NotEmpty().WithMessage("El usuario origen es obligatorio.");

        RuleFor(x => x.UsuarioDestino)
            .NotEmpty().WithMessage("El usuario destino es obligatorio.");

        RuleFor(x => x.UsuarioDestino)
            .NotEqual(x => x.UsuarioOrigen)
            .WithMessage("El usuario origen y destino no pueden ser iguales.");
    }
}