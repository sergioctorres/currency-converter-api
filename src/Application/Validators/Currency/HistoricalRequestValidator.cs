using Application.Constants;
using Application.Dtos.CurrencyRate;
using Application.Validators.Common;
using FluentValidation;

namespace Application.Validators.Currency;

public class HistoricalRequestValidator : AbstractValidator<HistoricalRequest>
{
    public HistoricalRequestValidator()
    {
        RuleFor(x => x)
            .SetValidator(new PagedRequestValidator());

        RuleFor(x => x.StartDate)
            .NotEmpty()
            .WithMessage("StartDate is required.");

        RuleFor(x => x.EndDate)
            .NotEmpty()
            .WithMessage("EndDate is required.")
            .GreaterThanOrEqualTo(x => x.StartDate)
            .WithMessage("EndDate must be after StartDate.");

        RuleFor(x => x.Base)
            .NotEmpty()
            .WithMessage("Base currency is required.")
            .Must(x => x is not null && CurrencyRateRulesConstants.BlockedCurrencies.Contains(x!.ToUpper()) is false)
            .WithMessage("The '{PropertyValue}' currency is not allowed to use.");

        RuleForEach(x => x.Symbols)
            .Must(x => CurrencyRateRulesConstants.BlockedCurrencies.Contains(x.ToUpper()) is false)
            .WithMessage("The '{PropertyValue}' currency is not allowed to use.")
            .When(x => x.Symbols is not null);
    }
}
