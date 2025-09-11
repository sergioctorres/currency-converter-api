using Application.Constants;
using Application.Dtos.CurrencyRate;
using FluentValidation;

namespace Application.Validators.Currency;

public class LatestRequestValidator : AbstractValidator<LatestRequest>
{
    public LatestRequestValidator()
    {
        RuleFor(x => x.Base)
            .NotEmpty()
            .WithMessage("Base currency is required.")
            .Must(x => CurrencyRateRulesConstants.BlockedCurrencies.Contains(x.ToUpper()) is false)
            .WithMessage("The '{PropertyValue}' currency is not allowed to use.");

        RuleForEach(x => x.Symbols)
            .Must(x => CurrencyRateRulesConstants.BlockedCurrencies.Contains(x.ToUpper()) is false)
            .WithMessage("The '{PropertyValue}' currency is not allowed to use.")
            .When(x => x.Symbols is not null);
    }
}
