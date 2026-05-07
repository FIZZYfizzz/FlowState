using FlowStatePlanner.Application.Calendar.Dtos;

namespace FlowStatePlanner.Application.Calendar;

public interface ICalendarSummaryService
{
    Task<WeekSummaryResponse> GetWeekSummaryAsync(DateOnly date, CancellationToken cancellationToken = default);
    Task<MonthSummaryResponse> GetMonthSummaryAsync(int year, int month, CancellationToken cancellationToken = default);
}
