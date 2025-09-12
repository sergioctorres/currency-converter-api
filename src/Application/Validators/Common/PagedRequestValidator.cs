using Application.Dtos.Common;
using FluentValidation;

namespace Application.Validators.Common;

public class PagedRequestValidator : AbstractValidator<PagedRequest>
{
    public PagedRequestValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page number must be greater or equal to 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 10)
            .WithMessage("Page size must be between 1 and 10.");
    }
}
