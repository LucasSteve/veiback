using FluentValidation;
using VeiCards.Aplicacao.Dtos;

namespace VeiCards.Aplicacao.Validadores;

public class AtualizarPerfilRequestValidator : AbstractValidator<AtualizarPerfilRequest>
{
    public AtualizarPerfilRequestValidator()
    {
        RuleFor(r => r.NomeExibicao).NotEmpty().MaximumLength(100);
        RuleFor(r => r.Email).NotEmpty().EmailAddress();
    }
}
