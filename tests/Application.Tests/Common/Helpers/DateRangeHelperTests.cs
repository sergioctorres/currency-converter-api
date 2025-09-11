using Application.Common.Helpers;

namespace Application.Tests.Common.Helpers;

public class DateRangeHelperTests
{
    [Fact]
    public void GetWeekdayPageRange_WithFullWeek_ReturnsOnlyWeekdays()
    {
        // Arrange
        var start = new DateTime(2025, 9, 1);
        var end = new DateTime(2025, 9, 7);

        // Act
        var result = DateRangeHelper.GetWeekdayPageRange(start, end, page: 1, pageSize: 10);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(new DateTime(2025, 9, 1), result?.StartDate);
        Assert.Equal(new DateTime(2025, 9, 5), result?.EndDate);
        Assert.Equal(5, result?.TotalDays);
    }

    [Fact]
    public void GetWeekdayPageRange_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var start = new DateTime(2025, 9, 1);
        var end = new DateTime(2025, 9, 14);

        // Act
        var page1 = DateRangeHelper.GetWeekdayPageRange(start, end, page: 1, pageSize: 3);
        var page2 = DateRangeHelper.GetWeekdayPageRange(start, end, page: 2, pageSize: 3);

        // Assert
        Assert.Equal(new DateTime(2025, 9, 1), page1?.StartDate);
        Assert.Equal(new DateTime(2025, 9, 3), page1?.EndDate);
        Assert.Equal(10, page1?.TotalDays);
        Assert.Equal(new DateTime(2025, 9, 4), page2?.StartDate);
        Assert.Equal(new DateTime(2025, 9, 8), page2?.EndDate);
        Assert.Equal(10, page2?.TotalDays);
    }

    [Fact]
    public void GetWeekdayPageRange_WithPageOutOfRange_ReturnsNull()
    {
        // Arrange
        var start = new DateTime(2025, 9, 1);
        var end = new DateTime(2025, 9, 7);

        // Act
        var result = DateRangeHelper.GetWeekdayPageRange(start, end, page: 3, pageSize: 5);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetWeekdayPageRange_WithStartEqualsEndOnWeekend_ReturnsNull()
    {
        // Arrange
        var start = new DateTime(2025, 9, 6);
        var end = new DateTime(2025, 9, 6);

        // Act
        var result = DateRangeHelper.GetWeekdayPageRange(start, end, page: 1, pageSize: 1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetWeekdayPageRange_WithStartEqualsEndOnWeekday_ReturnsThatDay()
    {
        // Arrange
        var start = new DateTime(2025, 9, 5);
        var end = new DateTime(2025, 9, 5);

        // Act
        var result = DateRangeHelper.GetWeekdayPageRange(start, end, page: 1, pageSize: 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(start, result?.StartDate);
        Assert.Equal(end, result?.EndDate);
        Assert.Equal(1, result?.TotalDays);
    }
}
