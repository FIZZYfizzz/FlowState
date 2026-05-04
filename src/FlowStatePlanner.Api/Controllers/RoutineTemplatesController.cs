using FlowStatePlanner.Application.RoutineTemplates;
using FlowStatePlanner.Application.RoutineTemplates.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace FlowStatePlanner.Api.Controllers;

[ApiController]
[Route("api/routine-templates")]
public sealed class RoutineTemplatesController(IRoutineTemplateService routineTemplateService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RoutineTemplateResponse>>> GetAll(CancellationToken cancellationToken)
        => Ok(await routineTemplateService.GetAllAsync(cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RoutineTemplateResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var template = await routineTemplateService.GetByIdAsync(id, cancellationToken);
        return template is null ? NotFound() : Ok(template);
    }

    [HttpPost]
    public async Task<ActionResult<RoutineTemplateResponse>> Create(CreateRoutineTemplateRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var created = await routineTemplateService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<RoutineTemplateResponse>> Update(Guid id, UpdateRoutineTemplateRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var updated = await routineTemplateService.UpdateAsync(id, request, cancellationToken);
            return updated is null ? NotFound() : Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        => await routineTemplateService.DeleteAsync(id, cancellationToken) ? NoContent() : NotFound();
}
