using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace LMS.BookService.Presentation.Authorization;

public class NumericRoleHandler : AuthorizationHandler<NumericRoleRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        NumericRoleRequirement requirement)
    {
        var roleClaim = context.User.FindFirst(ClaimTypes.Role)?.Value;
        if (roleClaim != null && int.TryParse(roleClaim, out int roleValue))
        {
            if ((roleValue & requirement.RoleRequirement) != 0)
            {
                context.Succeed(requirement);
            }
        }
        
    }
}