using Application.Dtos.CurrencyRate;
using Application.Validators.Currency;
using FluentValidation.TestHelper;

namespace Application.Tests.Validators.Currency;

public class HistoricalRequestValidatorTests
{
    private readonly HistoricalRequestValidator _validator = new();

    [Fact]
    public void Validate_WithEmptyStartDate_ReturnsError()
    {
        // Arrange
        var model = new HistoricalRequest(default);

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.StartDate);
    }

    [Fact]
    public void Validate_WithEndDateBeforeStartDate_ReturnsError()
    {
        // Arrange
        var model = new HistoricalRequest(DateTime.UtcNow) { EndDate = DateTime.UtcNow.AddDays(-1) };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EndDate);
    }

    [Fact]
    public void Validate_WithBlockedBaseCurrency_ReturnsError()
    {
        // Arrange
        var model = new HistoricalRequest(DateTime.UtcNow.AddDays(-5), "TRY", [ "EUR" ]) { EndDate = DateTime.UtcNow };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Base);
    }

    [Fact]
    public void Validate_WithBlockedSymbolCurrency_ReturnsError()
    {
        // Arrange
        var model = new HistoricalRequest(DateTime.UtcNow.AddDays(-5), "USD", ["EUR", "PLN"]) { EndDate = DateTime.UtcNow };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Symbols)
            .WithErrorMessage("The 'PLN' currency is not allowed to use.");
    }

    [Fact]
    public void Validate_WithValidRequest_ReturnsNoErrors()
    {
        // Arrange
        var model = new HistoricalRequest(DateTime.UtcNow.AddDays(-5), "USD", ["EUR", "GBP"]) { EndDate = DateTime.UtcNow };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
