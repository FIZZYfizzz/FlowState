namespace FlowStatePlanner.Application.Calendar.Dtos;

public sealed class MonthSummaryResponse
{
    public int Year { get; set; }
    public int Month { get; set; }
    public DateOnly MonthStartDate { get; set; }
    public DateOnly MonthEndDate { get; set; }
    public DateOnly CalendarGridStartDate { get; set; }
    public DateOnly CalendarGridEndDate { get; set; }
    public IReadOnlyList<CalendarDaySummaryResponse> Days { get; set; } = [];
}
