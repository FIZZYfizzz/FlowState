namespace FlowStatePlanner.Application.Abstractions;

public interface ICurrentUserService
{
    Guid UserId { get; }
}
