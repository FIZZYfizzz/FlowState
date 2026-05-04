using FlowStatePlanner.Application.Calendar;
using FlowStatePlanner.Application.Calendar.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace FlowStatePlanner.Api.Controllers;

[ApiController]
[Route("api/calendar")]
public sealed class CalendarController(ICalendarSummaryService calendarSummaryService) : ControllerBase
{
    [HttpGet("week/{date}")]
    public async Task<ActionResult<WeekSummaryResponse>> GetWeekSummary(string date, CancellationToken cancellationToken)
    {
        if (!DateOnly.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
            return BadRequest(new { error = "Date must be in yyyy-MM-dd format." });

        return Ok(await calendarSummaryService.GetWeekSummaryAsync(parsedDate, cancellationToken));
    }

    [HttpGet("month/{year:int}/{month:int}")]
    public async Task<ActionResult<MonthSummaryResponse>> GetMonthSummary(int year, int month, CancellationToken cancellationToken)
    {
        if (year < 1 || year > 9999 || month is < 1 or > 12)
            return BadRequest(new { error = "Year/month values are invalid. Month must be 1-12." });

        return Ok(await calendarSummaryService.GetMonthSummaryAsync(year, month, cancellationToken));
    }
}
