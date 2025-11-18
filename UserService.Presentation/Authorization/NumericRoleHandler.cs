using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace UserService.Presentation.Authorization;

public class NumericRoleHandler : AuthorizationHandler<NumericRoleRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, NumericRoleRequirement requirement)
    {
        var roleClaim = context.User.FindFirst(ClaimTypes.Role)?.Value;
        if (roleClaim != null && int.TryParse(roleClaim, out var roleId) == true)
        {
            if ((roleId & requirement.NumberRequirement) != 0)
            {
                context.Succeed(requirement);
            } 
        }
        return Task.CompletedTask;
    }
}