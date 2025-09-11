using Application.Dtos.Common;
using Application.Validators.Common;
using FluentValidation.TestHelper;

namespace Application.Tests.Validators.Common;

public class PagedRequestValidatorTests
{
    private readonly PagedRequestValidator _validator = new();

    [Fact]
    public void Validate_WithPageLessThanOne_ReturnsError()
    {
        // Arrange
        var model = new PagedRequest(5, 0);

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Page);
    }

    [Fact]
    public void Validate_WithPageSizeOutOfRange_ReturnsError()
    {
        // Arrange
        var model = new PagedRequest(20, 1);

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }

    [Fact]
    public void Validate_WithValidRequest_ReturnsNoErrors()
    {
        // Arrange
        var model = new PagedRequest(5, 1);

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
