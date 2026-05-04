using FlowStatePlanner.Application.DailyPlans;
using FlowStatePlanner.Application.DailyPlans.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace FlowStatePlanner.Api.Controllers;

[ApiController]
[Route("api/daily-plans")]
public sealed class DailyPlansController(IDailyPlanService dailyPlanService) : ControllerBase
{
    [HttpGet("{date}")]
    public async Task<ActionResult<DailyPlanResponse>> GetByDate(string date, CancellationToken cancellationToken)
    {
        if (!TryParse(date, out var planDate)) return BadRequest(new { error = "Date must be in yyyy-MM-dd format." });
        var plan = await dailyPlanService.GetByDateAsync(planDate, cancellationToken);
        return plan is null ? NotFound() : Ok(plan);
    }

    [HttpPost("{date}/generate")]
    public async Task<ActionResult<DailyPlanResponse>> Generate(string date, CancellationToken cancellationToken)
    {
        if (!TryParse(date, out var planDate)) return BadRequest(new { error = "Date must be in yyyy-MM-dd format." });
        return Ok(await dailyPlanService.GenerateAsync(planDate, cancellationToken));
    }

    [HttpPatch("{date}/items/{itemId:guid}/completion")]
    public async Task<ActionResult<DailyPlanItemResponse>> UpdateCompletion(string date, Guid itemId, UpdateDailyPlanItemCompletionRequest request, CancellationToken cancellationToken)
    {
        if (!TryParse(date, out var planDate)) return BadRequest(new { error = "Date must be in yyyy-MM-dd format." });
        var item = await dailyPlanService.UpdateCompletionAsync(planDate, itemId, request.IsCompleted, cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    private static bool TryParse(string value, out DateOnly date) => DateOnly.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
}
