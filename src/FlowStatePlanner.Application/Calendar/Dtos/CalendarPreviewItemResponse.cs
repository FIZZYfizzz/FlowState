namespace FlowStatePlanner.Application.Calendar.Dtos;

public sealed class CalendarPreviewItemResponse
{
    public string Title { get; set; } = string.Empty;
    public string SourceType { get; set; } = string.Empty;
    public TimeOnly? StartTime { get; set; }
    public bool? IsCompleted { get; set; }
}
