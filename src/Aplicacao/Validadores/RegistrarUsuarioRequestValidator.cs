using FluentValidation;
using VeiCards.Aplicacao.Dtos;

namespace VeiCards.Aplicacao.Validadores;

public class RegistrarUsuarioRequestValidator : AbstractValidator<RegistrarUsuarioRequest>
{
    public RegistrarUsuarioRequestValidator()
    {
        RuleFor(r => r.NomeUsuario).NotEmpty().MinimumLength(3).MaximumLength(50);
        RuleFor(r => r.Email).NotEmpty().EmailAddress();
        RuleFor(r => r.NomeExibicao).NotEmpty().MaximumLength(100);
        RuleFor(r => r.Senha).NotEmpty().MinimumLength(4).MaximumLength(100);
    }
}
