namespace FlowStatePlanner.Application.Calendar.Dtos;

public sealed class CalendarDaySummaryResponse
{
    public DateOnly Date { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public bool IsInCurrentMonth { get; set; }
    public bool HasGeneratedDailyPlan { get; set; }
    public Guid? DailyPlanId { get; set; }
    public int TotalPlannedItemCount { get; set; }
    public int CompletedItemCount { get; set; }
    public int IncompleteItemCount { get; set; }
    public int DueOneOffTaskCount { get; set; }
    public int RecurringTaskCount { get; set; }
    public int RoutineBlockCount { get; set; }
    public bool HasActiveRoutine { get; set; }
    public IReadOnlyList<CalendarPreviewItemResponse> PreviewItems { get; set; } = [];
}
