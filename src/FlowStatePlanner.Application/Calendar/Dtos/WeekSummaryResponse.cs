namespace FlowStatePlanner.Application.Calendar.Dtos;

public sealed class WeekSummaryResponse
{
    public DateOnly WeekStartDate { get; set; }
    public DateOnly WeekEndDate { get; set; }
    public IReadOnlyList<CalendarDaySummaryResponse> Days { get; set; } = [];
}
