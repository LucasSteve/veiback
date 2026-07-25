using FluentValidation;
using VeiCards.Aplicacao.Dtos;

namespace VeiCards.Aplicacao.Validadores;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(r => r.NomeUsuario).NotEmpty();
        RuleFor(r => r.Senha).NotEmpty();
    }
}
