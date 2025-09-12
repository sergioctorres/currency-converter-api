namespace Application.Common.Helpers;

public static class DateRangeHelper
{
    public static (DateTime StartDate, DateTime EndDate, int TotalDays)? GetWeekdayPageRange(DateTime start, DateTime end, int page, int pageSize)
    {
        var totalDays = Enumerable.Range(0, (end - start).Days + 1);

        var weekdays = totalDays
            .Select(day => start.AddDays(day))
            .Where(day => day.DayOfWeek is not DayOfWeek.Saturday and not DayOfWeek.Sunday)
            .ToList();

        var skip = (page - 1) * pageSize;
        var rangeDays = weekdays.Skip(skip).Take(pageSize).ToList();

        if (!rangeDays.Any()) return null;

        return (rangeDays.First(), rangeDays.Last(), weekdays.Count);
    }
}
