using System.Security.Claims;
using BorrowService.Presentation.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace BorrowkService.Presentation.Authorization;

public class NumericRoleHandler : AuthorizationHandler<NumericRoleRequirement>
{
    private readonly ILogger<NumericRoleHandler> _logger;

    public NumericRoleHandler(ILogger<NumericRoleHandler> logger)
    {
        _logger = logger;
    }
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        NumericRoleRequirement requirement)
    {
        _logger.LogInformation("Handling requirement {requirement}.", requirement.GetType().Name);
        var roleClaim = context.User.FindFirst(ClaimTypes.Role)?.Value;
        if (roleClaim != null && int.TryParse(roleClaim, out int roleValue))
        {
            if ((roleValue & requirement.RoleRequirement) != 0)
            {
                _logger.LogInformation("Successfully authorized for requirement {requirement}", requirement.GetType().Name);
                context.Succeed(requirement);
            }
        }
        
        return Task.CompletedTask;
    }
}