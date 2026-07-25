using FluentValidation;
using VeiCards.Aplicacao.Dtos;

namespace VeiCards.Aplicacao.Validadores;

public class CriarOuAtualizarNoticiaRequestValidator : AbstractValidator<CriarOuAtualizarNoticiaRequest>
{
    public CriarOuAtualizarNoticiaRequestValidator()
    {
        RuleFor(r => r.Titulo).NotEmpty().MaximumLength(200);
        RuleFor(r => r.Resumo).MaximumLength(500);
        RuleFor(r => r.Categoria).MaximumLength(50);
        RuleFor(r => r.TempoLeituraMinutos).GreaterThan(0).When(r => r.TempoLeituraMinutos.HasValue);
    }
}
