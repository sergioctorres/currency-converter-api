using Application.Constants;
using Application.Dtos.CurrencyRate;
using FluentValidation;

namespace Application.Validators.Currency;

public class ConvertRequestValidator : AbstractValidator<ConvertRequest>
{
    public ConvertRequestValidator()
    {
        RuleFor(x => x.Base)
            .NotEmpty()
            .WithMessage("From currency is required.")
            .Must(x => CurrencyRateRulesConstants.BlockedCurrencies.Contains(x.ToUpper()) is false)
            .WithMessage("The '{PropertyValue}' currency is not allowed to use.");

        RuleFor(x => x.Target)
            .NotEmpty()
            .WithMessage("To currency is required.")
            .Must(x => CurrencyRateRulesConstants.BlockedCurrencies.Contains(x.ToUpper()) is false)
            .WithMessage("The '{PropertyValue}' currency is not allowed to use.");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount is required.");
    }
}
