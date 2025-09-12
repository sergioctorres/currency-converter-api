using Application.Dtos.CurrencyRate;
using Application.Validators.Currency;
using FluentValidation.TestHelper;

namespace Application.Tests.Validators.Currency;

public class LatestRequestValidatorTestsTests
{
    private readonly LatestRequestValidator _validator = new();

    [Fact]
    public void Validate_WithEmptyBase_ReturnsError()
    {
        // Arrange
        var model = new LatestRequest(string.Empty, ["EUR"]);

        //Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Base);
    }

    [Fact]
    public void Validate_WithBlockedBaseCurrency_ReturnsError()
    {
        // Arrange
        var model = new LatestRequest("THB", ["EUR"]);

        //Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Base);
    }

    [Fact]
    public void Validate_WithBlockedSymbolCurrency_ReturnsError()
    {
        // Arrange
        var model = new LatestRequest("USD", ["EUR", "MXN"]);

        //Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Symbols)
            .WithErrorMessage("The 'MXN' currency is not allowed to use.");
    }

    [Fact]
    public void Validate_WithValidRequest_ReturnsNoErrors()
    {
        // Arrange
        var model = new LatestRequest("USD", ["EUR", "GBP"]);

        //Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
