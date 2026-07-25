using FluentValidation;
using VeiCards.Aplicacao.Dtos;

namespace VeiCards.Aplicacao.Validadores;

public class CriarOuAtualizarCartaRequestValidator : AbstractValidator<CriarOuAtualizarCartaRequest>
{
    public CriarOuAtualizarCartaRequestValidator()
    {
        RuleFor(r => r.Nome).NotEmpty().MaximumLength(200);
        RuleFor(r => r.ImagemUrl).Must(SerUrlValidaOuVazia).WithMessage("URL de imagem inválida.");
    }

    private static bool SerUrlValidaOuVazia(string? url) =>
        string.IsNullOrWhiteSpace(url) || Uri.IsWellFormedUriString(url, UriKind.Absolute);
}
