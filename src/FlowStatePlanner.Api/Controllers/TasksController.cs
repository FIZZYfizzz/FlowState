using FlowStatePlanner.Application.TaskItems;
using FlowStatePlanner.Application.TaskItems.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace FlowStatePlanner.Api.Controllers;

[ApiController]
[Route("api/tasks")]
public sealed class TasksController(ITaskItemService taskItemService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TaskItemResponse>>> GetAll(CancellationToken cancellationToken)
        => Ok(await taskItemService.GetAllAsync(cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TaskItemResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var task = await taskItemService.GetByIdAsync(id, cancellationToken);
        return task is null ? NotFound() : Ok(task);
    }

    [HttpPost]
    public async Task<ActionResult<TaskItemResponse>> Create(CreateTaskItemRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var created = await taskItemService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TaskItemResponse>> Update(Guid id, UpdateTaskItemRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var updated = await taskItemService.UpdateAsync(id, request, cancellationToken);
            return updated is null ? NotFound() : Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        => await taskItemService.DeleteAsync(id, cancellationToken) ? NoContent() : NotFound();
}
