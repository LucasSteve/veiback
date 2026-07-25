using FluentValidation;
using VeiCards.Aplicacao.Dtos;
using VeiCards.Dominio.Enums;

namespace VeiCards.Aplicacao.Validadores;

public class CriarOuAtualizarEventoRequestValidator : AbstractValidator<CriarOuAtualizarEventoRequest>
{
    public CriarOuAtualizarEventoRequestValidator()
    {
        RuleFor(r => r.Nome).NotEmpty().MaximumLength(200);
        RuleFor(r => r.Capacidade).GreaterThan(0).When(r => r.Capacidade.HasValue);
        RuleFor(r => r.Tipo)
            .NotEmpty()
            .Must(tipo => Enum.TryParse<TipoEvento>(tipo, ignoreCase: true, out _))
            .WithMessage($"Tipo deve ser um dos seguintes valores: {string.Join(", ", Enum.GetNames<TipoEvento>())}.");
    }
}
