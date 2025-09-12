using Application.Dtos.CurrencyRate;
using Application.Validators.Currency;
using FluentValidation.TestHelper;

namespace Application.Tests.Validators.Currency;

public class ConvertRequestValidatorTests
{
    private readonly ConvertRequestValidator _validator = new();

    [Fact]
    public void Validate_WithEmptyBase_ReturnsError()
    {
        // Arrange
        var model = new ConvertRequest("", "EUR", 100);

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Base);
    }

    [Fact]
    public void Validate_WithBlockedBaseCurrency_ReturnsError()
    {
        // Arrange
        var model = new ConvertRequest("TRY", "EUR", 100);

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Base);
    }

    [Fact]
    public void Validate_WithEmptyTarget_ReturnsError()
    {
        // Arrange
        var model = new ConvertRequest("USD", "", 100);

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Target);
    }

    [Fact]
    public void Validate_WithBlockedTargetCurrency_ReturnsError()
    {
        // Arrange
        var model = new ConvertRequest("USD", "PLN", 100);

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Target);
    }

    [Fact]
    public void Validate_WithAmountLessThanOrEqualZero_ReturnsError()
    {
        // Arrange
        var model = new ConvertRequest("USD", "EUR", 0);

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public void Validate_WithValidRequest_ReturnsNoErrors()
    {
        // Arrange
        var model = new ConvertRequest("USD", "EUR", 100);

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
