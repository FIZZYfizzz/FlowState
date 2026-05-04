using FlowStatePlanner.Application.Abstractions;
using Microsoft.AspNetCore.Http;

namespace FlowStatePlanner.Infrastructure.Identity;

public sealed class DevelopmentCurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private static readonly Guid DefaultUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public Guid UserId
    {
        get
        {
            var headerValue = httpContextAccessor.HttpContext?.Request.Headers["X-User-Id"].FirstOrDefault();
            return Guid.TryParse(headerValue, out var userId) ? userId : DefaultUserId;
        }
    }
}
